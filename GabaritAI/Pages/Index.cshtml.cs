using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GabaritAI.Services;
using GabaritAI.Helpers;

namespace GabaritAI.Pages
{
    [IgnoreAntiforgeryToken] // Necessário para chamadas fetch
    public class IndexModel : PageModel
    {
        private readonly IOpenAIService _openAIService;
        private readonly IChatMemoryService _memoryService;

        public IndexModel(IOpenAIService openAIService, IChatMemoryService memoryService)
        {
            _openAIService = openAIService;
            _memoryService = memoryService;
        }

        public List<string> Messages { get; set; } = new();

        public void OnGet()
        {
             // Carrega histórico anterior
            Messages = _memoryService.GetHistory(HttpContext);

            // Se for a primeira visita, adiciona a saudação inicial da IA
            if (Messages.Count == 0)
            {
                var hora = DateTime.Now.Hour;
                var saudacao = hora switch
                {
                    >= 5 and < 12 => "Bom dia ☀️",
                    >= 12 and < 18 => "Boa tarde 🌤️",
                    _ => "Boa noite 🌙"
                };

                var mensagemInicial = $"{saudacao}, eu sou o GabaritAI 🤖. " +
                                      "Sou seu assistente de estudos com IA — pronto para te ajudar a revisar, aprender e resolver dúvidas. " +
                                      "Envie uma pergunta para começar 🚀";

                _memoryService.AddAIMessage(HttpContext, mensagemInicial);
                Messages = _memoryService.GetHistory(HttpContext);
            }
        }

        public async Task<IActionResult> OnPostMessageAsync([FromBody] MessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
                return BadRequest(new { error = "Mensagem vazia" });

            // Adiciona mensagem do usuário
            _memoryService.AddUserMessage(HttpContext, request.Message);

            // Cria contexto (últimas mensagens)
            var context = _memoryService.GetContextText(HttpContext, 6);

            // Faz a chamada à IA
            string iaResponse;
            try
            {
                iaResponse = await _openAIService.GetResponseAsync(context);
            }
            catch (Exception ex)
            {
                iaResponse = $"⚠️ Erro ao conectar com a IA: {ex.Message}";
            }

            // Salva resposta
            _memoryService.AddAIMessage(HttpContext, iaResponse);

            return new JsonResult(new { response = iaResponse });
        }

        public class MessageRequest
        {
            public string Message { get; set; } = string.Empty;
        }
    }
}
