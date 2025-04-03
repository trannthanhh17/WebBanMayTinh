using WebBanHang.Models;

namespace WebBanHang.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        //IEnumerable <Product> GetAllbycateID(int categoryId);
        Task<IEnumerable<Product>> GetProductsByPriceRange(string priceRange);

    }
}
