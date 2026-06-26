using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MentalTrack.Models;
using MentalTrack.Enums;

namespace MentalTrack.Services
{
    public class SentimentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        public SentimentService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? throw new InvalidOperationException("OPENAI_API_KEY není nastavena.");
        }

        public async Task<Sentiment> AnalyzeAsync(JournalEntryPart part)
        {
            var text = part.Content;

            var result = await CallOpenAiSentiment(text);

            var mainPolarity = GetMainPolarity(
                result.Positive,
                result.Neutral,
                result.Negative
            );

            return new Sentiment
            {
                JournalEntryPartId = part.Id,
                JournalEntryPart = part,
                Positive = result.Positive,
                Neutral = result.Neutral,
                Negative = result.Negative,
                MainPolarity = mainPolarity
            };
        }

        private async Task<SentimentDto> CallOpenAiSentiment(string text)
        {
            var requestBody = new
            {
                model = "gpt-4.1-mini",
                messages = new object[]
                {
                    new
                    {
                        role = "system",
                        content =
                            "You are a sentiment analysis API. " +
                            "Return ONLY JSON in this format: " +
                            "{ \"positive\": 0-1, \"neutral\": 0-1, \"negative\": 0-1 }. " +
                            "All values must sum to 1."
                    },
                    new
                    {
                        role = "user",
                        content = text
                    }
                },
                temperature = 0
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/chat/completions"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);

            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            var sentiment = JsonSerializer.Deserialize<SentimentDto>(content!,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return sentiment!;
        }

        private TextPolarityEnum GetMainPolarity(double pos, double neu, double neg)
        {
            if (pos > neu && pos > neg)
                return TextPolarityEnum.Positive;

            if (neg > pos && neg > neu)
                return TextPolarityEnum.Negative;

            return TextPolarityEnum.Neutral;
        }


    }
}