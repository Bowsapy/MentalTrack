using MentalTrack.Models;

namespace MentalTrack.ViewModels
{
    public class MoodGraphViewModel
    {
        public MoodGraphViewModel(string[] axisx, MoodEnum[] axisy, string desc = "Your mood over time") { 
        
        
            axis_x = axisx;
            axis_y = axisy;
            description = desc;
        
        
        }
        public string[] axis_x { get; set; }
        public MoodEnum[] axis_y { get; set; }
        public string description {  get; set; }
    }
}
