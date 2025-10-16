using ClassLibrary.Models;
using ClassLibrary.Repository;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Reposetotry;


namespace ClassLibrary.Services
{
    public class OrderRepo : Repos<Order> , IOrderReposetory
    {
        public OrderRepo(MobiContext context) : base(context)
        {
        }

        public async Task<Order> AddSaleAsync(Guid customerId, Guid productId, int quantity)
        {
            if(quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
            }

            var customer = await _mobiContext.Customers.FindAsync(customerId);
            if (customer == null) throw new Exception("خطا در دریافت اطلاعات مشتری: مشتری یافت نشد");

            var product = await _mobiContext.Products.FindAsync(productId);
            if (product == null) throw new Exception("خطا در دریافت اطلاعات محصول: محصول یافت نشد");

            decimal totalPrice = product.Price * quantity;

            var Sale = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                ProductId = productId,
                Quantity = quantity,
                TotalPrice = totalPrice,
                SaleDate = DateTime.Now
            };
            await Entities.AddAsync(Sale);
            await _mobiContext.SaveChangesAsync();
            return Sale;
        }

        public async Task<List<Order>> GetSalesByCustomerAsync(Guid customerId)
        {
            return await Entities
                .Where(s => s.CustomerId == customerId)
                .Include(s => s.Product)
                .Include(s => s.Customer)
                .ToListAsync();
        }

        public async Task<List<Order>> GetSalesByProductAsync(Guid productId)
        {
            return await Entities
                .Where(s => s.ProductId == productId)
                .Include(s => s.Product)
                .Include(s => s.Customer)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalSalesAsync()
        {
           return await Entities.SumAsync(s => s.TotalPrice);
        }
    }
}
