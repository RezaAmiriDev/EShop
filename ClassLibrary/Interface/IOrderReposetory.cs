using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Repository
{
    public interface IOrderReposetory 
    {
        Task<Order> AddSaleAsync(Guid customerId, Guid productId, int quantity);
        Task<List<Order>> GetSalesByCustomerAsync(Guid customerId);
        Task<List<Order>> GetSalesByProductAsync(Guid productId);
        Task<decimal> GetTotalSalesAsync();
    }
}
