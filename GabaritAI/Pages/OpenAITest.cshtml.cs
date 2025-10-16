using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GabaritAI.Services;
using GabaritAI.Helpers;

namespace GabaritAI.Pages
{
    public class OpenAITestModel : PageModel
    {
        private readonly IOpenAIService _openAIService;

        [BindProperty]
        public string? Prompt { get; set; }

        public List<string> ConversationHistory { get; set; } = new();

        public OpenAITestModel(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public void OnGet()
        {
            ConversationHistory = HttpContext.Session.GetObject<List<string>>("Conversation") ?? new List<string>();
        }

        public async Task<IActionResult> OnPostAsync([FromBody] PromptInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Prompt))
                return BadRequest();

            // ⚠️ garante que session não é nula
            var history = HttpContext.Session?.GetObject<List<string>>("Conversation") ?? new List<string>();

            history.Add($"{input.Prompt} :Você");
            HttpContext.Session?.SetObject("Conversation", history);

            var fullContext = string.Join("\n", history);
            var response = await _openAIService.GetResponseAsync(fullContext);

            history.Add($"IA: {response}");
            HttpContext.Session?.SetObject("Conversation", history);

            return new JsonResult(new { response });
        }

        // Classe auxiliar para receber JSON
        public class PromptInput
        {
            public string Prompt { get; set; } = string.Empty;
        }
    }
}
