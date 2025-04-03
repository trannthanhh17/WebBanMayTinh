using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Models
{
    public class Product
    {

        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public List<ProductImage>? Images { get; set; }
        public int Stock {  get; set; }
        public decimal OriginalPrice { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<OrderDetail> OrderDetails { get; set; }

    }
}
