using System.ComponentModel.DataAnnotations;
namespace MentalTrack.Models;

public enum DayPhasesEnum
{
    [Display(Name = "Morning (06:00 - 11:59)")]
    Morning = 0,

    [Display(Name = "Afternoon (12:00 - 17:59)")]
    Afternoon = 1,

    [Display(Name = "Evening (18:00 - 23:59)")]
    Evening = 2,

    [Display(Name = "Night (24:00 - 05:59)")]
    Night = 3,


}