namespace GabaritAI.Services
{
    public interface IRagService
    {
        /// <summary>
        /// Busca os contextos relevantes no arquivo JSON para a pergunta do usuário.
        /// </summary>
        Task<string> GetRelevantContextAsync(string query);
    }
}
