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
        private readonly IParametroSistemaService _parametroSistemaService;
        public CategoriaService(HttpClient httpClient, IUtilsService utilsService, IConfiguration configuration, INotificator notificator, IParametroSistemaService parametroSistemaService) : base(notificator)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _utilsService = utilsService;
            _parametroSistemaService = parametroSistemaService;
        }

        public async Task<DadosCategoriaDto> ListarDadosCategoriaPorCod(string codCategoria, string grupoIdentificador)
        {
            var parametros = await _parametroSistemaService.BuscarParammetroPorGrupoIdentificador(grupoIdentificador);

            var baseUri = _configuration.GetSection("BasesUrl")["BaseUrlOmie"];
            if (string.IsNullOrEmpty(baseUri))
                throw new InvalidOperationException("A URL da API não foi configurada.");

            var fulluri = baseUri + "geral/categorias/";
            var request = new
            {
                call = "ConsultarCategoria",
                app_key = $"{parametros.AppKey}",
                app_secret = $"{parametros.AppSecret}",
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
