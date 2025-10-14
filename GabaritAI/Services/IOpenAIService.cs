namespace GabaritAI.Services
{
    public interface IOpenAIService
    {
        Task<string> GetResponseAsync(string prompt);
    }
}