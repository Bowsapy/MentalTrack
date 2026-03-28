using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class EmbeddingService
{
    private readonly HttpClient _http;
    private readonly ILogger<EmbeddingService> _logger;


    public EmbeddingService(HttpClient http, ILogger<EmbeddingService> logger)
    {
        _http = http;
        _logger = logger;

        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        _logger.LogInformation($"KEY: '{apiKey}'");
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("OPENAI_API_KEY není nastavený!");
        }


        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<float[]> GetEmbedding(string text)
    {
        var body = new
        {
            model = "text-embedding-3-small",
            input = text
        };

        var content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _http.PostAsync(
            "https://api.openai.com/v1/embeddings",
            content
        );

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"API error: {error}");
        }

        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("data")[0]
            .GetProperty("embedding")
            .EnumerateArray()
            .Select(x => x.GetSingle())
            .ToArray();
    }
}