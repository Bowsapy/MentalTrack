using System.ComponentModel.DataAnnotations;
namespace MentalTrack.Models;
public enum MoodEnum
{
    [Display(Name = "Very Bad")]
    VeryBad = 1,

    [Display(Name = "Bad")]
    Bad = 2,

    [Display(Name = "Neutral")]
    Neutral = 3,

    [Display(Name = "Good")]
    Good = 4,

    [Display(Name = "Very Good")]
    VeryGood = 5
}