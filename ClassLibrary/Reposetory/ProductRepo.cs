using ClassLibrary.Models;
using ClassLibrary.Repository;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Reposetotry;


namespace ClassLibrary.Services
{
    public class ProductRepo :Repos<Product> , IProductRepository
    {


        public ProductRepo(MobiContext mobiContext) : base(mobiContext) { }

        public async Task<List<Product>> Search(string findProductByName)
        {
            if(string.IsNullOrEmpty(findProductByName)) return new List<Product>();
           return await TableNoTracking
                .Where(p =>  EF.Functions.Like(p.Brand ?? string.Empty , $"%{findProductByName}%"))
                .ToListAsync();
        }
    }
}
