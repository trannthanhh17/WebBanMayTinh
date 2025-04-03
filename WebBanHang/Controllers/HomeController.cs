using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models;
using WebBanHang.Repositories;
using WebBanHang.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace WebBanHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private ApplicationDbContext _context;
        public HomeController(IProductRepository productRepository, ICategoryRepository categoryRepository, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _context = context;
        }
        public async Task<IActionResult> Index(int pg = 1)
        {
            var products = await _productRepository.GetAllAsync();

            const int pageSize = 10;
            if (pg < 1)
                pg = 1;
            int recsCount = products.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recskip = (pg - 1) * pageSize;
            var data = products.Skip(recskip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            
            return View(data);
        }
        public async Task<IActionResult> Search(string searchString )
        {
            List<Product> products;
            if (searchString != "" && searchString != null)
            {
                products = _context.Products.Where(n => n.Name.Contains(searchString)).ToList();

            }
            else 
            { 
                products = _context.Products.ToList(); 
            }
               
            return View(products);

        }

        public IActionResult GetProductByCate(int id)
        {
           var products = _context.Products.Where(x => x.CategoryId == id).OrderBy(x=>x.Name).ToList();
            return View(products);
        }
        public IActionResult GetProductByBrand(int id)
        {
            var products = _context.Products.Where(x => x.BrandId == id).OrderBy(x => x.Name).ToList();
            return View(products);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        
    }
}
