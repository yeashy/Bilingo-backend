using Bilingo.Data;
using Bilingo.Models.UserDTO;
using Microsoft.EntityFrameworkCore;

namespace Bilingo.Services
{
    public interface IUserService
    {
        Task DeleteUser(string username);
        Task EditUser(string username, UserEditDTO model);
        Task<StatisticsDTO> GetStatistics(string username);
        Task<UserGetInfoDTO> GetUserInfo(string username);
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
            if (user == null) throw new Exception("User wasn't found");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task EditUser(string username, UserEditDTO model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == username);
            if (user == null) throw new Exception("User wasn't found");

            user.Password = model.Password ?? user.Password;
            user.FirstName = model.FirstName ?? user.FirstName;
            user.LastName = model.LastName ?? user.LastName;
            user.Age = model.Age ?? user.Age;

            await _context.SaveChangesAsync();
        }

        public async Task<UserGetInfoDTO> GetUserInfo(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == username);
            if (user == null) throw new Exception("User wasn't found");

            return new UserGetInfoDTO
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Age = user.Age
            };
        }
        
        public async Task<StatisticsDTO> GetStatistics(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == username);
            if (user == null) throw new Exception("User wasn't found");

            var userWords = _context.UserWords.Where(x => x.User == user).ToList();
            var counts = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            var percentage = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            foreach (var userWord in userWords)
            {
                counts[userWord.WordStatus]++;
            }
            for (int i = 0; i < percentage.Length; i++)
            {
                percentage[i] = (double)counts[i] / _context.Words.Count() * 100;
            }
            return new StatisticsDTO
            {
                Counts = counts,
                Percentage = percentage
            };
        }
    }
}
