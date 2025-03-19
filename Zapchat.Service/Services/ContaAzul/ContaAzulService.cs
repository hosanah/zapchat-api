using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapchat.Domain.DTOs.ContaAzul;
using Zapchat.Domain.DTOs.ContasPagar;
using Zapchat.Domain.Interfaces;
using Zapchat.Domain.Interfaces.ContaAazul;
using Zapchat.Domain.Interfaces.Messages;

namespace Zapchat.Service.Services.ContaAzul
{
    public class ContaAzulService : BaseService, IContaAzulService
    {
        private readonly IConfiguration _configuration;
        private readonly IUtilsService _utilsService;
        public ContaAzulService(IConfiguration configuration, IUtilsService utilsService, INotificator notificator) : base(notificator)
        {
            _configuration = configuration;
            _utilsService = utilsService;
        }

        public async Task<ListarContaAzulDto> ListarInadiplentePorEmpresa(CapturaInadiplentelDto inadiplentelDto)
        {

            if (string.IsNullOrEmpty(inadiplentelDto.XAuthorization))
                Notify($"Parâmetro XAuthorization inválido!");

            var baseUri = _configuration.GetSection("BasesUrl")["BaseUrlContaAzul"];
            if (string.IsNullOrEmpty(baseUri))
                Notify($"A URL da API não foi configurada.!");


            var fullUri = baseUri + "finance-pro-reader/v1/installment-view?page=1&page_size=1000";

            var requestBody = new
            {
                dueDateFrom = $"{DateTime.Now.AddYears(-1):yyyy-MM-dd}",
                dueDateTo = $"{DateTime.Now:yyyy-MM-dd}",
                quickFilter = "ALL",
                search = "",
                status = new[] { "OVERDUE", "PARTIAL", "RENEGOTIATED" },
                type = "REVENUE"
            };

            var headers = new Dictionary<string, string>
            {
                { "Host", "services.contaazul.com" },
                { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:136.0) Gecko/20100101 Firefox/136.0" },
                { "Accept", "application/json" },
                { "X-Authorization", $"{inadiplentelDto.XAuthorization}" }
            };

            try
            {
                return await _utilsService.ExecuteApiCall<object, ListarContaAzulDto>(HttpMethod.Post, new Uri(fullUri), requestBody, headers);

            }
            catch (Exception)
            {
                Notify($"A solicitação não retornou dados!");
                return null;
            }
        }
    }
}
