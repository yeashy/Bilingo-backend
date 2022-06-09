namespace Bilingo.Models.UserDTO
{
    public class WordDTO
    {
        public int Id { get; set; }

        public string Word { get; set; }

        public string Level { get; set; }

        public List<string> Translations { get; set; }

        public List<Example> Examples { get; set; }

        public List<string> PartsOfSpeech { get; set; }
    }

    public class WordRepetitionDTO : WordDTO
    {
        public int Stage { get; set; }
    }
}
