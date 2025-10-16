using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GabaritAI.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenAIService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<string> GetResponseAsync(string prompt)
        {
            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                max_tokens = 300
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()
                ?.Trim() ?? "Sem resposta.";
        }
    }
}
