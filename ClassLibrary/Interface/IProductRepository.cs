using ClassLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> Search(string FindProductByName);

        Task<List<Product>> GetProductsWithShopAsync(int take);
        Task<Product?> GetProductWithShopAndRatingsAsync(Guid productId);
        Task<double> GetAverageRatingAsync(Guid productId);
        Task<bool> HasUserRatingAsync (Guid productId, Guid customerId);

    }
}
