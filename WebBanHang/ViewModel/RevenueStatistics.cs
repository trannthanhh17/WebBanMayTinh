namespace WebBanHang.ViewModel
{
	public class RevenueStatistics
	{
		public decimal TotalRevenue { get; set; }
		public decimal TotalProfit { get; set; }
		public Dictionary<DateTime, decimal> RevenueByDay { get; set; }
		public Dictionary<DateTime, decimal> ProfitByDay { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}
}
