using Bilingo.Data;
using Bilingo.Models.UserDTO;
using Microsoft.EntityFrameworkCore;

namespace Bilingo.Services
{
    public interface IUserService
    {
        Task DeleteUser(string username);
        Task EditUser(string username, UserEditDTO model);
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

        public async Task EditUser(string username, UserEditDTO model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == username);
            if (user == null) throw new Exception("User wan't found");

            user.Password = model.Password ?? user.Password;
            user.FirstName = model.FirstName ?? user.FirstName;
            user.LastName = model.LastName ?? user.LastName;
            user.Age = model.Age ?? user.Age;

            await _context.SaveChangesAsync();
        }
    }
}
