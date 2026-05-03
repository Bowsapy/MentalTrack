using System.ComponentModel.DataAnnotations;

namespace MentalTrack.Enums
{
    public enum UserStateEnum
    {
        [Display (Name = "Being anxious")]
        Anxious = 0,
        [Display(Name = "Being depressed")]

        Depressed = 1,
        [Display(Name = "Being tired")]

        Tired = 2,
        [Display(Name = "Work")]

        Working = 3,
        [Display(Name = "Being alone")]

        Alone = 4,

        [Display(Name = "Exercise / sport")]
        Sport = 5,

        [Display(Name = "Being with your girlfriend / boyfriend")]
        Partner = 6,


    }
}
