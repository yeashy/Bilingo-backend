namespace Bilingo.Models.UserDTO
{
    public class NewWordDTO
    {
        public int Id { get; set; }

        public string Word { get; set; }

        public string Level { get; set; }

        public List<string> Translations { get; set; }

        public List<Example> Examples { get; set; }
    }
}
