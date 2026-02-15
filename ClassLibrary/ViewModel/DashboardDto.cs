
namespace ModelLayer.ViewModel
{
    public class DashboardSummaryDto
    {
        public int CustomersCount { get; set; }
        public int ProductsCount { get; set; }
        public int ShopsCount { get; set; }
        public int OrdersCount { get; set; }
    }

    public class TopProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }    // یا TotalRevenue اگر خواستید مبلغ
        public decimal TotalRevenue { get; set; }
    }

    public class DailyTransactionDto
    {
        public DateTime Date { get; set; }   // تاریخ روز
        public decimal TotalAmount { get; set; }
    }

    public class DashboardDto
    {
        public DashboardSummaryDto Summary { get; set; } = new();
        public List<TopProductDto> TopProducts { get; set; } = new();
        public List<DailyTransactionDto> DailyTransactions { get; set; } = new();
    }

    public class CityOrderDto
    {
        public string City { get; set; } = string.Empty;
        public int OrderCount { get; set; }
    }

}
