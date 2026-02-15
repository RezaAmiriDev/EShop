using ClassLibrary.Repository;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Interface
{
    public interface IPaymentRepository 
    {
        Task<Payment> AddAsync(Payment payment);
        Task<List<Payment>> GetByOrderIdAsync(Guid OrderId);
        Task<decimal> GetTotalPaidByOrderAsync(Guid OrderId);
        Task<Payment?> GetByProviderTransactionIdAsync(string  ProviderTransactionId);
        Task UpdateAsync(Payment payment);

    }
}
