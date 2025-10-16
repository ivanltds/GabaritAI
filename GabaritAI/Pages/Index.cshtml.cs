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
                Messages.Add($"Você: {UserMessage}");

                try
                {
                    var response = await _openAIService.GetResponseAsync(UserMessage);
                    Messages.Add($"🤖 IA: {response}");
                }
                catch (Exception ex)
                {
                    Messages.Add($"⚠️ Erro ao conectar com a IA: {ex.Message}");
                }

                HttpContext.Session.SetObject("ChatMessages", Messages);
            }

            return RedirectToPage();
        }
    }
}
