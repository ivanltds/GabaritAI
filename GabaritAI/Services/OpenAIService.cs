using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GabaritAI.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IRagService _ragService;
        private readonly IChatMemoryService _memoryService;

        public OpenAIService(HttpClient httpClient, IConfiguration config, IRagService ragService, IChatMemoryService memoryService)
        {
            _httpClient = httpClient;
            _ragService = ragService;
            _memoryService = memoryService;

            _apiKey = config["API_KEY"]
                      ?? Environment.GetEnvironmentVariable("API_KEY")
                      ?? throw new Exception("❌ API_KEY não configurada!");

            // Configuração dos headers HTTP
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "GabaritAI/1.0");
        }

        public async Task<string> GetResponseAsync(string prompt)
        {
            // 🧠 Integra o RAG ao prompt
            var promptComConhecimento = await _ragService.GetRelevantContextAsync(prompt);

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "system", content = "Você é um assistente educacional gentil, que ajuda adolescentes a aprenderem com explicações claras e motivadoras. Nunca incentiva cola, e sempre explica o raciocínio por trás das respostas." },
                    new { role = "user", content = promptComConhecimento }
                },
                temperature = 0.7,
                max_tokens = 400
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Erro da API: {error}");
                return "⚠️ Erro ao conectar com a IA.";
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonResponse);
            var message = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return message ?? "⚠️ Resposta vazia da IA.";
        }
    }
}
