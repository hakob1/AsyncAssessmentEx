using TransactionScopeEx.Context;
using TransactionScopeEx.Models;

namespace TransactionScopeEx.Services
{
    public class PaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreatePaymentAsync(int userId, decimal amount)
        {
            var payment = new Payment { UserId = userId, Amount = amount };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }
    }
}
