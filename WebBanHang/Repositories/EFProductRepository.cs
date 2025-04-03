using Microsoft.EntityFrameworkCore;
using WebBanHang.DataAccess;
using WebBanHang.Models;

namespace WebBanHang.Repositories
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.Include(p => p.Category).Include(p => p.Brand).ToListAsync();
        }
        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Category).Include(p => p.Brand).FirstOrDefaultAsync(p => p.Id == id);

             
        }
        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Product>> GetProductsByPriceRange(string priceRange)
        {
            IQueryable<Product> products = _context.Products;

            switch (priceRange)
            {
                case "under100":
                    products = products.Where(p => p.Price < 100000);
                    break;
                case "100to500":
                    products = products.Where(p => p.Price >= 100000 && p.Price <= 500000);
                    break;
                case "500to1000":
                    products = products.Where(p => p.Price >= 500000 && p.Price <= 1000000);
                    break;
                case "above1000":
                    products = products.Where(p => p.Price > 1000000);
                    break;
                default:
                    break;
            }

            return await products.ToListAsync();
        }
    }
}
