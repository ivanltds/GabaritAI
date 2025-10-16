    using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace GabaritAI.Helpers
{
    /// <summary>
    /// Extensões para salvar e recuperar objetos complexos da sessão em formato JSON.
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Serializa um objeto genérico e o armazena na sessão.
        /// </summary>
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            session.SetString(key, json);
        }

        /// <summary>
        /// Recupera e desserializa um objeto genérico da sessão.
        /// </summary>
        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (string.IsNullOrEmpty(value))
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            catch
            {
                return default;
            }
        }
    }
}
