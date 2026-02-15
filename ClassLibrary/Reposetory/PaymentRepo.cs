using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Interface;
using ModelLayer.Models;
using ModelLayer.Reposetotry;

namespace ClassLibrary.Services
{
    public class PaymentRepo : Repos<Payment>, IPaymentRepository
    {
        public PaymentRepo(MobiContext context) : base(context)
        {
        }

        public async Task<Payment> AddAsync(Payment payment)
        {
            await Entities.AddAsync(payment);
            await _mobiContext.SaveChangesAsync();
            return payment;
        }

        public async Task<List<Payment>> GetByOrderIdAsync(Guid orderId)
        {
            return await Entities
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalPaidByOrderAsync(Guid orderId)
        {
            return await Entities
                .Where(p => p.OrderId == orderId && p.Status == PaymentStatus.Paid)
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;
        }

        public async Task<Payment?> GetByProviderTransactionIdAsync(string providerTransactionId)
        {
            return await Entities.FirstOrDefaultAsync(p =>
                p.ProviderTransactionId == providerTransactionId);
        }

        public async Task UpdateAsync(Payment payment)
        {
            Entities.Update(payment);
            await _mobiContext.SaveChangesAsync();
        }
    }
}
