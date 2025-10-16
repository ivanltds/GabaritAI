using System.Text.Json;

namespace GabaritAI.Services
{

    public class RagService : IRagService
    {
        private readonly string _filePath;

        public RagService()
        {
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "knowledge.json");
        }

        public async Task<string> GetRelevantContextAsync(string prompt)
        {
            if (!File.Exists(_filePath))
                return prompt;

            try
            {
                var json = await File.ReadAllTextAsync(_filePath);
                var conhecimento = JsonSerializer.Deserialize<List<ItemConhecimento>>(json);

                var relevantes = conhecimento?
                    .Where(x => prompt.Contains(x.Pergunta, StringComparison.OrdinalIgnoreCase))
                    .Take(5)
                    .Select(x => $"Pergunta: {x.Pergunta}\nResposta: {x.Resposta}")
                    .ToList();

                if (relevantes == null || relevantes.Count == 0)
                    return prompt;

                var contexto = string.Join("\n\n", relevantes);
                return $"Base de conhecimento escolar:\n{contexto}\n\nPergunta do aluno: {prompt}";
            }
            catch
            {
                return prompt;
            }
        }

        private class ItemConhecimento
        {
            public string Pergunta { get; set; } = "";
            public string Resposta { get; set; } = "";
        }
    }
}
