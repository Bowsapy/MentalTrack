using Microsoft.Identity.Client;
using System.Globalization;

namespace MentalTrack.Services
{
    public class EmbeddingConverter
    {
        public List<float> ConvertToFloatList(string[] stringArray)
        {
            List<float> floatList = new List<float>();
            for (int i = 0; i < stringArray.Length; i++)
            {
                floatList.Add(float.Parse(stringArray[i], CultureInfo.InvariantCulture));


            }
            return floatList;
        }
        public string[] ConvertToStringArray(float[] floatArray)
        {
            string[] stringArray = new string[floatArray.Length];
            for (int i = 0; i < floatArray.Length; i++)
            {
                stringArray[i] = (floatArray[i]).ToString(CultureInfo.InvariantCulture);

            }
            return stringArray;
        }
    }
}
