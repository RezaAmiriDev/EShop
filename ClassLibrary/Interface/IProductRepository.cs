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
        //bool Add (Product Model);
        //Task<bool> UpdateAsync(Product Model);
        //ServiceResponse Delete (int Id);
        //Product FindById (int Id);
        //IEnumerable<Product> FindAll ();
        Task<List<Product>> Search(string FindProductByName);
    }
}
