namespace WebBanHang.Models
{
    public class Reply
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public Review Review { get; set; }
        public ApplicationUser User { get; set; }
    }

}
