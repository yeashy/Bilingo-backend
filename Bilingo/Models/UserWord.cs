using System.ComponentModel.DataAnnotations;

namespace Bilingo.Models
{
    public class UserWord
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int WordId { get; set; }
        public Word Word { get; set; }  

        public int WordStatus { get; set; }

        public DateTime? NextRepetitionDateTime { get; set; }
    }
}
