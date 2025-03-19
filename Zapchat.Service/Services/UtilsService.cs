using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Zapchat.Domain.Interfaces.Messages;
using Zapchat.Domain.Interfaces;

namespace Zapchat.Service.Services
{
    public class UtilsService : BaseService, IUtilsService
    {
        private readonly HttpClient _httpClient;

        public UtilsService(HttpClient httpClient, INotificator notificator) : base (notificator)
        {
            _httpClient = httpClient;
        }

        public async Task<TResponse> ExecuteApiCall<TRequest, TResponse>(HttpMethod httpMethod, Uri fullUrl, TRequest request, Dictionary<string, string>? headers = null)
        {
            var jsonPayload = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var httpRequest = new HttpRequestMessage(httpMethod, fullUrl)
            {
                Content = content
            };

            // Adiciona os headers personalizados, se existirem
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpRequest.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await _httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                Notify($"Erro na API. Código: {response.StatusCode}, Resposta: {errorResponse}");
                throw new HttpRequestException($"Erro na requisição: {response.StatusCode}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseBody))
            {
                Notify("A solicitação não retornou dados!");
                throw new InvalidOperationException("A resposta da API está vazia.");
            }

            // Deserializa o JSON para o tipo TResponse
            var result = JsonSerializer.Deserialize<TResponse>(responseBody);

            if (result == null)
            {
                Notify("Falha ao deserializar a resposta da API.");
                throw new InvalidOperationException("A resposta da API não pôde ser deserializada.");
            }

            return result;
        }
    }
}
