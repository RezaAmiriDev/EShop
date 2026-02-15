using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.ViewModel
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string? CustomerName { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; } = "IRR";
        public string? Status { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? Provider { get; set; }
        public string? ProviderTransactionId { get; set; }
        public string? TransactionReference { get; set; }
        // آیا پرداخت تایید نهایی شده است (useful flag)
        public bool IsVerified { get; set; } = false;
        // هر note یا متادیتای کوتاه (در صورت نیاز)
        public string? Note { get; set; }
    }

}
