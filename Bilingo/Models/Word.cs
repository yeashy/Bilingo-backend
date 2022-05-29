namespace Bilingo.Models
{
    public class Word
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public string Level { get; set; }

        public ICollection<User> Users { get; set; }
        public List<UserWord> UserWords { get; set; }
    }
}
