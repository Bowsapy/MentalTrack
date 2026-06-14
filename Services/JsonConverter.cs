using System.Text.Json;

namespace MentalTrack.Services
{
    public class JsonConverter
    {
        public string ConvertToJson<T>(T[] array)
        {
            return JsonSerializer.Serialize(array);
        }

        public T[] ConvertFromJson<T>(string json)
        {
            return JsonSerializer.Deserialize<T[]>(json);
        }
    }
}