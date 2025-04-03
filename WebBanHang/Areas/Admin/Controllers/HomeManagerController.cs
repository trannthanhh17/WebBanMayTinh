using Microsoft.AspNetCore.Mvc;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class HomeManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
