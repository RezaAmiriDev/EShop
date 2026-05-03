using ClassLibrary.Models;
using ClassLibrary.Repository;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Reposetotry;

namespace ClassLibrary.Services
{
    public class ProductRepo :Repos<Product> , IProductRepository
    {


        public ProductRepo(MobiContext mobiContext) : base(mobiContext) { }


        public async Task<List<Product>> GetProductsWithShopAsync(int take)
        {
           return await TableNoTracking.Include(p => p.Shop).Take(take).ToListAsync();
        }

        public async Task<Product?> GetProductWithShopAndRatingsAsync(Guid productId)
        {
            return await TableNoTracking
                .Include(p => p.Shop)
                .Include(p => p.Ratings.Where(r => r.IsApproved)) // فقط نظرات تأیید شده
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<double> GetAverageRatingAsync(Guid productId)
        {
            return await _mobiContext.Ratings
                .Where(r => r.ProductId == productId && r.IsApproved)
                .AverageAsync(r => (double?)r.Review) ?? 0;
        }

  
        public async Task<bool> HasUserRatingAsync(Guid productId, Guid customerId)
        {
            return await _mobiContext.Ratings
                .AnyAsync(r => r.ProductId == productId && r.CustomerId == customerId);
        }

        public async Task<List<Product>> Search(string findProductByName)
        {
            if(string.IsNullOrEmpty(findProductByName)) return new List<Product>();
           return await TableNoTracking
                .Where(p =>  EF.Functions.Like(p.Brand ?? string.Empty , $"%{findProductByName}%"))
                .ToListAsync();
        }
    }
}
