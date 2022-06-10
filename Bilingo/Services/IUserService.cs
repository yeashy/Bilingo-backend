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
        Task ChangeAvatar(string username, IFormFile file);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public UserService(ApplicationDbContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
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
                Age = user.Age,
                Gender = user.Gender
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

        public async Task ChangeAvatar(string username, IFormFile file)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == username);
            if (user == null) throw new Exception("User wasn't found");

            Console.WriteLine(file.FileName);
            string path = GeneratePath(user.Id, file.FileName);
            using var fileStream = new FileStream(_appEnvironment.ContentRootPath + path, FileMode.Create);
            await file.CopyToAsync(fileStream);

            user.PathToAvatar = path;
            await _context.SaveChangesAsync();  
        }

        private static string GeneratePath(int id, string extension)
        {
            string timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            return $"Files\\{timeStamp}_{new Random().Next(0, 100000)}_{id}{extension}";
        }
    }
}
