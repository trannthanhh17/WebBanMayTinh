using WebBanHang.Models;

namespace WebBanHang.ViewModel
{
    public class OrderDetailsViewModel
    {
        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
