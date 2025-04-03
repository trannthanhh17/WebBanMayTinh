using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models;
using WebBanHang.Repositories;
using WebBanHang.Utilitys;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class StatisticsController : Controller
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }
		[HttpGet]
		public async Task<IActionResult> RevenueStatistics(DateTime? startDate, DateTime? endDate)
		{
			var start = startDate ?? DateTime.Now.AddMonths(-1); // Mặc định là tháng trước
			var end = endDate ?? DateTime.Now;

			var totalRevenue = await _statisticsService.GetTotalRevenueAsync(start, end);
			var totalProfit = await _statisticsService.GetTotalProfitAsync(start, end);
			var revenueByDay = await _statisticsService.GetRevenueByDayAsync(start, end);
			var profitByDay = await _statisticsService.GetProfitByDayAsync(start, end);

			var model = new RevenueStatistics
			{
				TotalRevenue = totalRevenue,
				TotalProfit = totalProfit,
				RevenueByDay = revenueByDay,
				ProfitByDay = profitByDay,
				StartDate = start,
				EndDate = end
			};

			return View(model);
		}

		public IActionResult Index()
		{
			return View();
		}
	}
}
