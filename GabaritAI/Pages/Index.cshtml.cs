using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GabaritAI.Services;
using GabaritAI.Helpers; 
namespace GabaritAI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IOpenAIService _openAIService;
        private readonly IChatMemoryService _memoryService;

        [BindProperty]
        public string? UserMessage { get; set; }

        public List<string> Messages { get; set; } = new();

        public IndexModel(IOpenAIService openAIService, IChatMemoryService memoryService)
        {
            _openAIService = openAIService;
            _memoryService = memoryService;
        }

        public void OnGet()
        {
            Messages = _memoryService.GetHistory(HttpContext);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!string.IsNullOrWhiteSpace(UserMessage))
            {
                _memoryService.AddUserMessage(HttpContext, UserMessage);
                Messages = _memoryService.GetHistory(HttpContext);
                Messages.Add("💬 IA está digitando...");
                HttpContext.Session.SetObject("ChatHistory", Messages);

                string iaResponse;
                try
                {
                    var contextText = _memoryService.GetContextText(HttpContext, 6);
                    iaResponse = await _openAIService.GetResponseAsync(contextText);
                }
                catch (Exception ex)
                {
                    iaResponse = $"⚠️ Erro ao conectar com a IA: {ex.Message}";
                }

                Messages.Remove("💬 IA está digitando...");
                _memoryService.AddAIMessage(HttpContext, iaResponse);
            }

            return RedirectToPage();
        }
    }
}
