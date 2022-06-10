namespace Bilingo.Models
{
    public class User
    {
        public int Id { get ; set; }

        public string Email { get; set; }

        public string Password { get; set; } 

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public int Age { get; set; }

        public int Role { get; set; } = Convert.ToInt32(Models.Role.User);

        public ICollection<Word> Words { get; set; }
        public List<UserWord> UserWords { get; set; }
    }
}
