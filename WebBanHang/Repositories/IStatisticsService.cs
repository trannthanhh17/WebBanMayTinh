namespace WebBanHang.Repositories
{
    public interface IStatisticsService
    {
        Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalProfitAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<DateTime, decimal>> GetRevenueByDayAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<DateTime, decimal>> GetProfitByDayAsync(DateTime startDate, DateTime endDate);
    }
}
