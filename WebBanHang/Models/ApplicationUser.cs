using Microsoft.AspNetCore.Identity;

namespace WebBanHang.Models
{
    public class ApplicationUser: IdentityUser
    {
        public required string FullName { get; set; }

    }
}
