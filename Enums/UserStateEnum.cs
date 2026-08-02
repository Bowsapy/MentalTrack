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

        [Display(Name = "Invalid")]
        Partner = 6,

        [Display(Name = "Drinking energy drinks")]
        EnergyDrinks = 7,

        [Display(Name = "Drinking coffee")]
        Coffee = 8,
        [Display(Name = "Drinking alcohol")]
        Alcohol = 9,


        [Display(Name = "Sleeping")]
        Sleeping = 10,

        [Display(Name = "Did not sleep")]
        PoorSleep = 11,

        [Display(Name = "Studying")]
        Studying = 12,

        [Display(Name = "Reading")]
        Reading = 13,


        [Display(Name = "Being outside")]
        BeingOutside = 14,

        [Display(Name = "Being with your friend/partner")]
        Friend = 15,

        [Display(Name = "Scrolling on a phone")]
        Scrolling = 16,

        [Display(Name = "Being productive")]
        Productive = 17,


        [Display(Name = "Listening to music")]
        Music = 18,


        [Display(Name = "Being with your family")]
        Family = 19,

        [Display(Name = "Going for a walk")]
        Walk = 20,
        [Display(Name = "Watching movies")]
         Movies= 21,
        [Display(Name = "Self reflection")]
        SelfReflection = 22,
        [Display(Name = "Playing PC/Console games")]
        Games = 23,
        [Display(Name = "Coding/ Programming")]
        Coding = 24,
        [Display(Name = "Cleaning")]
        Cleaning = 25,
        [Display(Name = "Smoking cigarettes")]
        Cigarettes = 26,
        [Display(Name = "Smoking weed")]
        Weed = 27,
        [Display(Name = "Avoiding responsibilities (Procrastinating)")] 
        Procrastination = 28,
        [Display(Name = "Arguing with someone")]
        Arguing = 29,
        [Display(Name = "Being stressed")]
        Stress = 30,



    }
}
