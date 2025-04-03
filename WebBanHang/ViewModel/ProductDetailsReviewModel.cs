using WebBanHang.Models;

namespace WebBanHang.ViewModel
{
    public class ProductDetailsReviewModel
    {
        public Product Product { get; set; }
        public Category Category { get; set; }
        public Brand Brand { get; set; }
        public List<Comment> Comments { get; set; }
        public Comment NewComment { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<int, int> RatingCounts { get; set; }
    }
}
