using MentalTrack.Models;

namespace MentalTrack.ViewModels
{
    public class WordCountAnalysisViewModel
    {
        public Dictionary<MoodEnum, Dictionary<string, int>> WordsCountForMood { get; set; }

        public WordCountAnalysisViewModel(Dictionary<MoodEnum, Dictionary<string, int>> wordsCountForMood)
        {
            WordsCountForMood = wordsCountForMood;
        }
    }
}