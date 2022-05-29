namespace Bilingo.Models
{
    public class RepetitionTimings
    {
        public static List<TimeSpan> Repetitions { get; set; } = new List<TimeSpan>() 
        {
            new(0, 0, 20, 0),  // 20 minutes
            new(0, 3, 0, 0),   // 3 hours
            new(0, 24, 0, 0),  // 24 hours
            new(7, 0, 0, 0),   // 7 days
            new(14, 0, 0, 0),  // 14 days
            new(30, 0, 0, 0)   // 30 days
        };
    }
}
