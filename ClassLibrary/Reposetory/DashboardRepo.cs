using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Interface;
using ModelLayer.ViewModel;


namespace ModelLayer.Reposetory
{
    public class DashboardRepo : IDashboardRepository
    {
        private readonly MobiContext _context;

        public DashboardRepo(MobiContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var customersCountTask = await _context.Customers.CountAsync();
            var productCountTask = await _context.Products.CountAsync();
            var shopCountTask = await _context.Shops.CountAsync();
            var ordersCountTask = await _context.Orders.CountAsync();

            return new DashboardSummaryDto
            {
                CustomersCount = customersCountTask,
                ProductsCount = productCountTask,
                ShopsCount = shopCountTask,
                OrdersCount = ordersCountTask
            };
        }

        public async Task<List<DailyTransactionDto>> GetDailyTransactionAsync(int days = 10)
        {
            var from = DateTime.UtcNow.Date.AddDays(-days + 1);

            var raw = await _context.Orders
                .Where(o => o.SaleDate >= from)
                .GroupBy(o => new { o.SaleDate.Year, o.SaleDate.Month, o.SaleDate.Day })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Day = g.Key.Day,
                    Total = g.Sum(x => x.TotalPrice)
                }).ToListAsync();

            var result = new List<DailyTransactionDto>();
            for(int i  = 0; i < days; i++)
            {
                var d = from.AddDays(i);
                var item = raw.FirstOrDefault(r => r.Year == d.Year && r.Month == d.Month && r.Day == d.Day);
                result.Add(new DailyTransactionDto
                {
                    Date = d,
                    TotalAmount = item?.Total ?? 0m
                });
            }
            return result;
        }

        public async Task<List<TopProductDto>> GetTopProductAsync(int top = 6)
        {
            var q = await _context.Orders.GroupBy(o => o.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.TotalPrice),
                }).OrderByDescending(x => x.TotalQuantity)
                .Take(top).ToListAsync();
            var productId = q.Select(x => x.ProductId).ToList();
            var product = await _context.Products
                .Where(p => productId.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => (p.Brand ?? p.Name ?? p.Id.ToString()));

            return q.Select(x => new TopProductDto
            {
                ProductId = x.ProductId,
                ProductName = product.TryGetValue(x.ProductId, out var name) ? name : "نامشخص",
                TotalQuantity = x.TotalQuantity,
                TotalRevenue = x.TotalRevenue,
            }).ToList();
        }

        public async Task<List<CityOrderDto>> GetTopCitiesByOrdersAsync(int top = 5)
        {
            var q = await _context.Orders.Include(o => o.Customer)
                .ThenInclude(c => c.Address)
                .Where(o => o.Customer != null && o.Customer.Address != null && !string.IsNullOrEmpty(o.Customer.Address.City))
                .GroupBy(o => o.Customer.Address.City)
                .Select(g => new CityOrderDto
                {
                    City = g.Key!,
                    OrderCount = g.Count()
                }).OrderByDescending(x => x.OrderCount).Take(top).ToListAsync();

            return q;
        }
    }
}
