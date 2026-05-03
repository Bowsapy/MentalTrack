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
    }
}
