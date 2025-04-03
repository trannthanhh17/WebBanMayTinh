using WebBanHang.Models;
using WebBanHang.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebBanHang.Repositories;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Authorization;
using WebBanHang.Utilitys;
using Microsoft.EntityFrameworkCore;
using WebBanHang.DataAccess;
using WebBanHang.ViewModel;
using System.Drawing.Printing;
namespace WebBanHang.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private ApplicationDbContext _context;
        private readonly CommentController _commentController;
        public ProductController(IProductRepository productRepository, ApplicationDbContext context, CommentController commentController)
        {
            _productRepository = productRepository;
            _context = context;
            _commentController = commentController;
        }
        public async Task<IActionResult> Index(int pg = 1)
        {
            var products = await _productRepository.GetAllAsync();

            const int pageSize = 20;
            if (pg < 1)
                pg = 1;
            int recsCount = products.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recskip = (pg - 1) * pageSize;
            var data = products.Skip(recskip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;

            return View(data);
        }
        public async Task<IActionResult> Search(string searchString, string sortOrder, string priceRange)
        {
            List<Product> products;
            if (searchString != "" && searchString != null)
            {
                products = _context.Products.Where(n => n.Name.Contains(searchString)).ToList();

            }
            else
            {
                products = _context.Products.ToList();
            }

            if (!string.IsNullOrEmpty(priceRange))
            {
                string[] priceBounds = priceRange.Split('-');
                if (priceBounds.Length == 2 && decimal.TryParse(priceBounds[0], out decimal minPrice) && decimal.TryParse(priceBounds[1], out decimal maxPrice))
                {
                    products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
                }
            }
            return View(products);
        }


        public async Task<IActionResult> Display(int id)
        {
            var product = await _context.Products
                            .Include(p => p.Comments)
                            .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var comments = product.Comments;
            var averageRating = comments.Any() ? comments.Average(c => c.Rating) : 0;

            var ratingCounts = comments
            .GroupBy(c => c.Rating)
            .ToDictionary(g => g.Key, g => g.Count());

            for (int i = 1; i <= 5; i++)
            {
                if (!ratingCounts.ContainsKey(i))
                {
                    ratingCounts[i] = 0;
                }
            }

            var viewModel = new ProductDetailsReviewModel
            {
                Product = product,
                Comments = product.Comments.OrderByDescending(c => c.CommentDate).ToList(),
                AverageRating = averageRating,
                NewComment = new Comment { ProductId = id },
                RatingCounts = ratingCounts
            };

            return View(viewModel);
        }


    }
}
