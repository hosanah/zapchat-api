using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Zapchat.Domain.DTOs.Clientes;
using Zapchat.Domain.DTOs.ContasPagar;
using Zapchat.Domain.Interfaces;
using Zapchat.Domain.Interfaces.Clientes;
using Zapchat.Domain.Interfaces.Messages;

namespace Zapchat.Service.Services.Clientes
{
    public class ClientesService : BaseService, IClientesService
    {

        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IUtilsService _utilsService;
        private readonly IParametroSistemaService _parametroSistemaService;
        public ClientesService(HttpClient httpClient, IUtilsService utilsService, 
            IConfiguration configuration, INotificator notificator, 
            IParametroSistemaService parametroSistemaService) : base(notificator)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _utilsService = utilsService;
            _parametroSistemaService = parametroSistemaService;
        }

        public async Task<DadosClientesDto> ListarDadosClientesPorCod(string codCliente, string grupoIdentificador)
        {
            var parametros = await _parametroSistemaService.BuscarParammetroPorGrupoIdentificador(grupoIdentificador);

            var baseUri = _configuration.GetSection("BasesUrl")["BaseUrlOmie"];
            if (string.IsNullOrEmpty(baseUri))
                throw new InvalidOperationException("A URL da API não foi configurada.");

            var fulluri = baseUri + "geral/clientes/";
            var request = new
            {
                call = "ConsultarCliente",
                app_key = $"{parametros.AppKey}",
                app_secret = $"{parametros.AppSecret}",
                param = new[]
                {
                    new
                    {
                        codigo_cliente_omie = codCliente,
                        codigo_cliente_integracao = ""
                    }
                }
            };

            try
            {
                return await _utilsService.ExecuteApiCall<object, DadosClientesDto>(HttpMethod.Post, new Uri(fulluri), request);
            }
            catch (Exception)
            {
                Notify($"A solicitação não retornou dados!");
                return null;
            }
        }
    }
}
