using Microsoft.EntityFrameworkCore;
using WebBanHang.DataAccess;

namespace WebBanHang.Repositories
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext _context;

        public StatisticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
        {
            var totalRevenue = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .SumAsync(o => o.TotalPrice);

            return totalRevenue;
        }

        public async Task<decimal> GetTotalProfitAsync(DateTime startDate, DateTime endDate)
        {
            var totalProfit = await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.Order.OrderDate >= startDate && od.Order.OrderDate <= endDate)
                .SumAsync(od => (od.Price - od.Product.OriginalPrice) * od.Quantity);

            return totalProfit;
        }

        public async Task<Dictionary<DateTime, decimal>> GetRevenueByDayAsync(DateTime startDate, DateTime endDate)
        {
            var revenueByDay = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalPrice)
                })
                .ToDictionaryAsync(x => x.Date, x => x.Revenue);

            return revenueByDay;
        }

        public async Task<Dictionary<DateTime, decimal>> GetProfitByDayAsync(DateTime startDate, DateTime endDate)
        {
            var profitByDay = await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.Order.OrderDate >= startDate && od.Order.OrderDate <= endDate)
                .GroupBy(od => od.Order.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Profit = g.Sum(od => (od.Price - od.Product.OriginalPrice) * od.Quantity)
                })
                .ToDictionaryAsync(x => x.Date, x => x.Profit);

            return profitByDay;
        }
    }

}
