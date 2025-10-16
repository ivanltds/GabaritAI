using System.Threading.Tasks;

namespace GabaritAI.Services
{
    public interface IOpenAIService
    {
        Task<string> GetResponseAsync(string fullContext);
    }
}
