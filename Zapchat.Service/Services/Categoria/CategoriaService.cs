using Microsoft.Extensions.Configuration;
using Zapchat.Domain.DTOs.Categoria;
using Zapchat.Domain.Interfaces.Categoria;
using Zapchat.Domain.Interfaces.Messages;
using Zapchat.Domain.Interfaces;

namespace Zapchat.Service.Services.Categoria
{
    public class CategoriaService : BaseService, ICategoriaService
    {

        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IUtilsService _utilsService;
        public CategoriaService(HttpClient httpClient, IUtilsService utilsService, IConfiguration configuration, INotificator notificator) : base(notificator)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _utilsService = utilsService;
        }

        public async Task<DadosCategoriaDto> ListarDadosCategoriaPorCod(string codCategoria)
        {
            var baseUri = _configuration.GetSection("BasesUrl")["BaseUrlOmie"];
            if (string.IsNullOrEmpty(baseUri))
                throw new InvalidOperationException("A URL da API não foi configurada.");

            var fulluri = baseUri + "geral/categorias/";
            var request = new
            {
                call = "ConsultarCategoria",
                app_key = "1490222176443",
                app_secret = "6f2b10cb4d043172aa2e083613994aef",
                param = new[]
                {
                    new
                    {
                        codigo = codCategoria
                    }
                }
            };

            try
            {
                return await _utilsService.ExecuteApiCall<object, DadosCategoriaDto>(HttpMethod.Post, new Uri(fulluri), request);
            }
            catch (Exception)
            {
                Notify($"A solicitação não retornou dados!");
                return null;
            }
        }
    }
}
