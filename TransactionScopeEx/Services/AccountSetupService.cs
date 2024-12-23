using System.Transactions;
using TransactionScopeEx.Models;

namespace TransactionScopeEx.Services
{
    public class AccountSetupService
    {
        private readonly UserService _userService;
        private readonly PaymentService _paymentService;

        public AccountSetupService(UserService userService, PaymentService paymentService)
        {
            _userService = userService;
            _paymentService = paymentService;
        }

        //Tasktodo
        public async Task SetUpNewUserAccountAsync(string email, decimal initialPaymentAmount)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                User newUser = await _userService.CreateUserAsync(email);

                await _paymentService.CreatePaymentAsync(newUser.Id, initialPaymentAmount);

                scope.Complete();
            }
        }
    }
}
