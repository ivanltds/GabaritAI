using System.Collections.Generic;

namespace GabaritAI.Services
{
    public interface IChatMemoryService
    {
        List<string> GetHistory(HttpContext context);
        void AddUserMessage(HttpContext context, string message);
        void AddAIMessage(HttpContext context, string message);
        string GetContextText(HttpContext context, int maxMessages = 6);
        void ClearHistory(HttpContext context);
    }
}
