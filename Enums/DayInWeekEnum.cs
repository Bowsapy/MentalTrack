using System.ComponentModel.DataAnnotations;

namespace MentalTrack.Enums
{
    public enum DayInWeekEnum
    {
        [Display(Name = "Monday")]
        Monday,

        [Display(Name = "Tuesday")]
        Tuesday,

        [Display(Name = "Wednesday")]
        Wednesday,

        [Display(Name = "Thursday")]
        Thursday,

        [Display(Name = "Friday")]
        Friday,

        [Display(Name = "Saturday")]
        Saturday,

        [Display(Name = "Sunday")]
        Sunday
    }
}