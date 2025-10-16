using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }      // لینک به سفارش/سِیل
        public decimal Amount { get; set; }    // decimal(18,2)
        public string Currency { get; set; } = "IRR";
        public PaymentProvider Provider { get; set; } = PaymentProvider.Zarinpal;
        public string? ProviderTransactionId { get; set; } // id برگشتی از درگاه (مثلاً Authority یا transaction id)
        public string? Authority { get; set; } // مخصوص زرین‌پال اگر لازم است
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaymentDate { get; set; }         // زمان موفقیت یا برگشت درگاه
        public string? CallbackUrl { get; set; }           // برای ثبت یا لاگ
        public string? Method { get; set; }                // Card, BankTransfer, Wallet...
        public decimal? Fee { get; set; }                  // هزینه درگاه
        public string? TransactionReference { get; set; }  // Reference برای reconciliation
        public string? Note { get; set; }                  // هر توضیح دلخواه
        public string? MetadataJson { get; set; }          // برای ذخیره داده‌های اضافی (json)
        public bool IsVerified { get; set; } = false;      // اگر تایید نهایی انجام شد
    }

    public enum PaymentProvider { Zarinpal = 0, PayIr = 1, Stripe = 2, Bank = 99 }
    public enum PaymentStatus { Pending = 0, Redirected = 1, Succeeded = 2, Failed = 3, Refunded = 4, Cancelled = 5 }


}
