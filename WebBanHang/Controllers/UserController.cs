using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.DataAccess;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
	public class UserController : Controller
	{
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OrderHistory()
        {
            var userId = _userManager.GetUserId(User);
            var orders = _context.Orders
    .Where(o => o.UserId == userId && o.Status != "Cancelled")
    .Include(o => o.OrderDetails)
        .ThenInclude(od => od.Product)
    .ToList();

            if (orders.Count == 0)
            {
                Console.WriteLine("Không tìm thấy đơn hàng cho người dùng: " + userId);
            }
            else
            {
                foreach (var order in orders)
                {
                    Console.WriteLine("Đơn hàng ID: " + order.Id);
                }
            }

            return View(orders);
        }

    }
}
