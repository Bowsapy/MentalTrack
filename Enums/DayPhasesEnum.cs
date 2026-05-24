using System.ComponentModel.DataAnnotations;
namespace MentalTrack.Models;

public enum DayPhasesEnum
{
    [Display(Name = "Morning")]
    Morning = 0,

    [Display(Name = "Afternoon")]
    Afternoon = 1,

    [Display(Name = "Evening")]
    Evening = 2,

    [Display(Name = "Night")]
    Night = 3,


}