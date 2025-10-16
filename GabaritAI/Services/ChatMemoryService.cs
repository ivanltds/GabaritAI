using GabaritAI.Helpers;

namespace GabaritAI.Services
{
    public class ChatMemoryService : IChatMemoryService
    {
        private const string SessionKey = "ChatHistory";

        public List<string> GetHistory(HttpContext context)
        {
            return context.Session.GetObject<List<string>>(SessionKey) ?? new List<string>();
        }

        public void AddUserMessage(HttpContext context, string message)
        {
            var history = GetHistory(context);
            history.Add($"🧑 Você: {message}");
            context.Session.SetObject(SessionKey, history);
        }

        public void AddAIMessage(HttpContext context, string message)
        {
            var history = GetHistory(context);
            history.Add($"🤖 IA: {message}");
            context.Session.SetObject(SessionKey, history);
        }

        public string GetContextText(HttpContext context, int maxMessages = 6)
        {
            var history = GetHistory(context);
            return string.Join("\n", history.TakeLast(maxMessages));
        }

        public void ClearHistory(HttpContext context)
        {
            context.Session.Remove(SessionKey);
        }
    }
}
