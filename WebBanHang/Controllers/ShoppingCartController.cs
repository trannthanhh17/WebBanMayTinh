using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.DataAccess;
using WebBanHang.Migrations;
using WebBanHang.Models;
using WebBanHang.Repositories;
using WebBanHang.Services;

namespace WebBanHang.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVnPayService _vnPayService;

        public ShoppingCartController(IProductRepository productRepository, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IVnPayService vnPayService)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
            _vnPayService = vnPayService;
        }
        public IActionResult AddToCart(int productId, int quantity)
        {

            var product = GetProductFromDatabase(productId);
            if (product == null || product.Stock < quantity)
            {
                // Xử lý nếu sản phẩm không tồn tại hoặc không đủ số lượng
                ModelState.AddModelError("", "Không đủ số lượng sản phẩm tồn kho.");
                return RedirectToAction("Index");
            }

            var cartItem = new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity,
                Product = product
            };

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }
        public IActionResult PlusCart(int productId, int quantity)
        {
            var product = GetProductFromDatabase(productId);
            if (product == null || product.Stock < quantity + 1)
            {
                // Xử lý nếu sản phẩm không tồn tại hoặc không đủ số lượng
                ModelState.AddModelError("", "Không đủ số lượng sản phẩm tồn kho.");
                return RedirectToAction("Index");
            }

            var cartItem = new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity + 1,
                Product = product
            };

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }
        public IActionResult MinusCart(int productId, int quantity)
        {
            var product = GetProductFromDatabase(productId);
            if (product == null)
            {
                // Xử lý nếu sản phẩm không tồn tại
                return NotFound();
            }

            var cartItem = new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity - 1,
                Product = product
            };

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");

        }

           
           
        

        public IActionResult Index()
        {
            var cart =HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            if(cart.Items.Count == 0)
            {
                return View("EmtyCart");
            }
            return View(cart);
        }
        // Các actions khác...
        private Product GetProductFromDatabase(int productId)
        {
           var product= _context.Products.FirstOrDefault(p=>p.Id == productId);
            return product;
        }
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            return View(new Order());
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order, string payment = "COD")
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                // Xử lý giỏ hàng trống...
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList();

            // Thiết lập giá trị cho Status
            order.Status = "Pending"; // Giá trị mặc định cho trạng thái đơn hàng

            // Kiểm tra và cập nhật số lượng tồn kho
            foreach (var item in order.OrderDetails)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null && product.Stock >= item.Quantity)
                {
                    product.Stock -= item.Quantity;
                }
                else
                {
                    ModelState.AddModelError("", "Không đủ số lượng sản phẩm tồn kho.");
                    return View("Index");
                }
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Lưu các thay đổi về số lượng tồn kho
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("Cart");

            if (payment == "Thanh toán VnPay")
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = (double)cart.Items.Sum(i => i.Price * i.Quantity),
                    CreatedDate = DateTime.Now,
                    Description = order.Notes,
                    FullName = order.Name,
                    OrderId = order.Id,
                };
                return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
            }

            return View("OrderCompleted", order.Id); // Trang xác nhận hoàn thành đơn hàng
        }



        public async Task<IActionResult> RemoveItem(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.RemoveItem(product.Id);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return Redirect(Request.Headers["Referer"].ToString());
        }
        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }


        [Authorize]
        public IActionResult PaymentCallBack(Order order)
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response == null || response.VnPayResponseCode !="00")
            {
                TempData["Message"] = $"Lỗi thanh toán VNPay: {response.VnPayResponseCode}";
                return View("PaymentFail");
            }

            //lưu vào database

            TempData["Message"] = $"Thanh toán VNPay thành công";
            return View("OrderCompleted", order.Id);
        }
    }
}

