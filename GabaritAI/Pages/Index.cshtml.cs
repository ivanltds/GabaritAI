using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GabaritAI.Helpers;
using GabaritAI.Services;

namespace GabaritAI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IOpenAIService _openAIService;

        [BindProperty]
        public string? UserMessage { get; set; }

        public List<string> Messages { get; set; } = new();

        public IndexModel(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public void OnGet()
        {
            Messages = HttpContext.Session.GetObject<List<string>>("ChatMessages") ?? new List<string>();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Messages = HttpContext.Session.GetObject<List<string>>("ChatMessages") ?? new List<string>();

            if (!string.IsNullOrWhiteSpace(UserMessage))
            {
                // Adiciona mensagem do usuário
                Messages.Add($"Você: {UserMessage}");

                // Adiciona indicador "IA está digitando..."
                Messages.Add("⏳ IA está digitando...");

                HttpContext.Session.SetObject("ChatMessages", Messages);

                // Gera a resposta da IA
                string responseText;
                try
                {
                    responseText = await _openAIService.GetResponseAsync(UserMessage);
                }
                catch (Exception ex)
                {
                    responseText = $"⚠️ Erro ao conectar com a IA: {ex.Message}";
                }

                // Remove o indicador de digitação
                Messages.Remove("⏳ IA está digitando...");
                Messages.Add($"🤖 {responseText}");

                HttpContext.Session.SetObject("ChatMessages", Messages);
            }

            return RedirectToPage();
        }
    }
}
