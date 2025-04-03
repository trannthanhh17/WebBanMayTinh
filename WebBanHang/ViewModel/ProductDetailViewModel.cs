using WebBanHang.Models;

namespace WebBanHang.ViewModel
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public List<Review> Reviews { get; set; }
        public Review NewReview { get; set; }
        public Reply NewReply { get; set; }
        public double AverageRating { get; set; }
    }
}
