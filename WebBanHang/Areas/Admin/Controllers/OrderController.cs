using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.DataAccess;
using WebBanHang.Models;
using WebBanHang.Utilitys;
using WebBanHang.ViewModel;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrderController : Controller
	{
		private readonly ApplicationDbContext _context;
		public OrderController( ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index(int pg = 1)
		{
            var order = await _context.Orders.ToListAsync();

            const int pageSize = 10;
            if (pg < 1)
                pg = 1;
            int recsCount = order.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recskip = (pg - 1) * pageSize;
            var data = order.Skip(recskip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;

            return View(data);
            //return View(await _context.Orders.ToListAsync());
		}

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            var viewModel = new OrderDetailsViewModel
            {
                Order = order,
                OrderDetails = order.OrderDetails.ToList()
            };

            return View(viewModel); // Chuyển ViewModel vào view
        }
        [HttpPost]
        public async Task<IActionResult> EditStatus(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = orderId });
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
