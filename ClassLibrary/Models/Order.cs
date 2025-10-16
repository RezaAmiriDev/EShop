using DataLayer.Base;


namespace ClassLibrary
{
    public class Order : BaseEntity
    {
        public Guid CustomerId { get; set; }  // مشتری مرتبط
        public Guid ProductId { get; set; }   // محصول مرتبط

        // پیگیری/آدرس
        public string? OrderNumber { get; set; }   // human-friendly id (مثلاً "ORD-20250901-0001")

        // snapshot: ذخیره نام و قیمت آن زمان (مهم برای audit)
        public string? ProductNameSnapshot { get; set; }
        public decimal UnitPrice { get; set; }    // قیمت هر واحد در زمان سفارش

        public int Quantity { get; set; }    // تعداد محصول فروخته‌شده
        public decimal AmountPaid { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalPrice { get; set; } // قیمت کل این فروش
        public DateTime SaleDate { get; set; } = DateTime.UtcNow; // تاریخ فروش

        public string Currency { get; set; } = "IRR";
        // وضعیت‌ها و پیگیری
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.NotPaid;

        // ارتباطات
        public Customer? Customer { get; set; }
        public Product? Product { get; set; }

        // helper کوتاه برای محاسبه باقیمانده
        public decimal BalanceDue => (TotalPrice + ShippingCost) - AmountPaid;
    }

        public enum OrderStatus
        {
            Pending = 0,
            Processing = 1,
            Shipped = 2,
            Completed = 3,
            Cancelled = 4,
            Refunded = 5
        }

        public enum PaymentStatus
        {
            NotPaid = 0,
            PartiallyPaid = 1,
            Paid = 2,
            Refunded = 3,
            Failed = 4
        }

}
