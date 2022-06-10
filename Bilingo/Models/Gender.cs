using System.ComponentModel.DataAnnotations;

namespace Bilingo.Models
{
    public enum Gender
    {
        [Display(Name = "Male")]
        Male = 0,

        [Display(Name = "Female")]
        Female = 1,

        [Display(Name = "Other")]
        Other = 2,

        [Display(Name = "I prefer not to say")]
        PreferNotToSay = 3
    }
}
