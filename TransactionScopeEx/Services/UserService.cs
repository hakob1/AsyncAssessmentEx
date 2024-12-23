using TransactionScopeEx.Context;
using TransactionScopeEx.Models;

namespace TransactionScopeEx.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUserAsync(string email)
        {
            var user = new User { Email = email };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUserEmailAsync(int userId, string newEmail)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            user.Email = newEmail;
            await _context.SaveChangesAsync();
        }
    }
}
