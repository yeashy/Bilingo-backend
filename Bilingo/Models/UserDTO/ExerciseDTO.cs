namespace Bilingo.Models.UserDTO
{
    public class ExerciseDTO
    {
        public string Type { get; set; }

        public int WordId { get; set; }

        public string Word { get; set; }
    }

    public class ExerciseType1DTO : ExerciseDTO
    {
        public string Sentence { get; set; }

        public List<string> SentenceRandomOrder { get; set; }
    }

    public class ExerciseType2DTO : ExerciseDTO
    {
        public string Sentence { get; set; }
    }

    public class ExerciseType3DTO : ExerciseDTO
    {
        public List<string> Meanings { get; set; }

        public int CorrectAnswer { get; set; }
    }

    public class ExerciseType4DTO : ExerciseDTO
    {
        public string SynOrAnt { get; set; }

        public List<string> Options { get; set; }

        public int CorrectAnswer { set; get; }
    }
}
