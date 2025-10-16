using System.Threading.Tasks;

namespace GabaritAI.Services
{
       public interface IOpenAIService
    {
        /// <summary>
        /// Envia um prompt para o OpenAI e retorna a resposta do modelo.
        /// </summary>
        /// <param name="prompt">O texto enviado pelo usuário.</param>
        /// <returns>Resposta do modelo como string.</returns>
        Task<string> GetResponseAsync(string prompt);
    }
}