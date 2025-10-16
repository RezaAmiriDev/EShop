using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.ViewModel
{
    // ModelLayer/ViewModel/SaleSummaryDto.cs
    public class OrderDto
    {
        public Guid? Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid CustomerId { get; set; }
        public string? OrderNumber { get; set; }
        public string? ProductNameSnapshot { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal AmountPaid { get; set; }       // جمع پرداخت‌های موفق برای این سفارش
        public string PaymentStatus { get; set; } = string.Empty; // برای نمایش مانند "Paid", "NotPaid"
        public string Currency { get; set; } = "IRR";
        public string Status { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        // اطلاعات کمکی
        public string? ProductName { get; set; }
        public string? CustomerName { get; set; }
    }
}
