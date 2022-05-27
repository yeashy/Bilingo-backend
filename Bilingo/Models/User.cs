using System.ComponentModel.DataAnnotations;

namespace Bilingo.Models
{
    public class User
    {
        [Required]
        public int Id { get ; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; } 

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public int Role { get; set; } = Convert.ToInt32(Models.Role.User);
    }
}
