using MentalTrack.Models;

namespace MentalTrack.ViewModels
{
    public class MoodGraphViewModel
    {
        public MoodGraphViewModel(string[] axisx, MoodEnum[] axisy) { 
        
        
            axis_x = axisx;
            axis_y = axisy;
        
        
        }
        public string[] axis_x { get; set; }
        public MoodEnum[] axis_y { get; set; }
    }
}
