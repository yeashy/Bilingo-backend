using Bilingo.Data;
using Bilingo.Models;
using Bilingo.Models.UserDTO;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Bilingo.Services
{
    public interface IRegisterService
    {
        Task<object> RegistrateUser(UserRegisterDTO model);
    }

    public class RegisterService : IRegisterService
    {
        private readonly ApplicationDbContext _context;

        public RegisterService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> RegistrateUser(UserRegisterDTO model)
        {
            var genderName = GetEnumDisplayAttributeByInt(model.Gender);
            if (genderName == null) throw new Exception("Incorrect gender value");

            await _context.Users.AddAsync(new User
            {
                Email = model.Email,
                Password = EncodePassword(model.Password),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Age = model.Age,
                Gender = genderName
            });
            await _context.SaveChangesAsync();

            var identity = GetIdentity(model.Email, Role.User);

            return GetToken(identity);
        }

        private static ClaimsIdentity GetIdentity(string username, Role role)
        {
            // Claims описывают набор базовых данных для авторизованного пользователя
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role.ToString())
            };

            //Claims identity и будет являться полезной нагрузкой в JWT токене, которая будет проверяться стандартным атрибутом Authorize
            var claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }

        private static object GetToken(ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                issuer: JwtConfigurations.Issuer,
                audience: JwtConfigurations.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(JwtConfigurations.Lifetime)),
                signingCredentials: new SigningCredentials(JwtConfigurations.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                token = encodedJwt
            };

            return response;
        }

        private static string EncodePassword(string password)
        {
            var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            var sb = new System.Text.StringBuilder();
            foreach (byte b in hash)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        private static string? GetEnumDisplayAttributeByInt(int id)
        {
            var enumValue = (Gender)id;
            return enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttribute<DisplayAttribute>()?.GetName();
        }
    }
}

