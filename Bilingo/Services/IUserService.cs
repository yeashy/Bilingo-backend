using Bilingo.Data;
using Microsoft.EntityFrameworkCore;

namespace Bilingo.Services
{
    public interface IUserService
    {
        Task DeleteUser(string username);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task DeleteUser(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == username);
            if (user == null) throw new Exception("User wan't found");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
