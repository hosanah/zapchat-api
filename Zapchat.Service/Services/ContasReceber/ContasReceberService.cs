using Microsoft.Extensions.Configuration;
using Zapchat.Domain.Interfaces.Clientes;
using Zapchat.Domain.Interfaces;
using Zapchat.Domain.Interfaces.Messages;
using ClosedXML.Excel;
using Zapchat.Domain.DTOs.ContasReceber;
using Zapchat.Domain.Interfaces.ContasReceber;
using System.Globalization;
using Zapchat.Domain.DTOs.ContasPagar;

namespace Zapchat.Service.Services.ContasReceber
{
    public class ContasReceberService : BaseService, IContasReceberService
    {
        private readonly IConfiguration _configuration;
        private readonly IUtilsService _utilsService;
        private readonly IClientesService _clientesService;
        public ContasReceberService(IConfiguration configuration, IUtilsService utilsService, IClientesService clientesService, INotificator notificator) : base(notificator)
        {
            _configuration = configuration;
            _utilsService = utilsService;
            _clientesService = clientesService;
        }

        public async Task<string> ListarContasReceberExcel(ListarContasReceberExcelDto listarContasReceberExcelDto)
        {
            try
            {
                return await ExportarContasReceberXlsxBase64();
            }
            catch (Exception ex)
            {
                Notify($"A solicitação não retornou dados!{ex.Message}");
                return string.Empty;
            }
        }

        private async Task<string> ExportarContasReceberXlsxBase64()
        {
            // Criar o arquivo Excel em memória
            using (var workbook = new XLWorkbook())
            {
                var listaAVencer = await ListarContasReceberSeteDias();
                var listaAtrasado = await ListarContasReceberAtrasados();
                var listaVenceHoje = await ListarContasReceberVenceHoje();

                // Adiciona as planilhas
                await AdicionarPlanilha(workbook, "Vence em 7 dias", listaAVencer.ContaReceberCadastro);
                await AdicionarPlanilha(workbook, "Lista Atrasado", listaAtrasado.ContaReceberCadastro);
                await AdicionarPlanilha(workbook, "Vence Hoje", listaVenceHoje.ContaReceberCadastro);

                // Salvar o arquivo em memória
                using (var memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    var byteArray = memoryStream.ToArray();
                    return Convert.ToBase64String(byteArray);
                }
            }
        }

        private async Task<ListarContasReceberDto> ListarContasReceberAtrasados()
        {
            var baseUri = _configuration.GetSection("BasesUrl")["BaseUrlOmie"];
            if (string.IsNullOrEmpty(baseUri))
                Notify($"A URL da API não foi configurada.!");

            var fulluri = baseUri + "financas/contareceber/";

            var request = new
            {
                call = "ListarContasReceber",
                app_key = "1490222176443",
                app_secret = "6f2b10cb4d043172aa2e083613994aef",
                param = new[]
                {
                    new
                    {
                        pagina = 1,
                        registros_por_pagina = 999,
                        apenas_importado_api = "N",
                        filtrar_por_status = "ATRASADO"
                    }
                }
            };

            try
            {
                // Faz a chamada à API
                return await _utilsService.ExecuteApiCall<object, ListarContasReceberDto>(HttpMethod.Post, new Uri(fulluri), request);

            }
            catch (Exception)
            {
                Notify($"A solicitação não retornou dados!");
                return null;
            }
        }

        private async Task<ListarContasReceberDto> ListarContasReceberVenceHoje()
        {
            var baseUri = _configuration.GetSection("BasesUrl")["BaseUrlOmie"];
            if (string.IsNullOrEmpty(baseUri))
                Notify($"A URL da API não foi configurada.!");

            var fulluri = baseUri + "financas/contareceber/";

            var request = new
            {
                call = "ListarContasReceber",
                app_key = "1490222176443",
                app_secret = "6f2b10cb4d043172aa2e083613994aef",
                param = new[]
                {
                    new
                    {
                        pagina = 1,
                        registros_por_pagina = 999,
                        apenas_importado_api = "N",
                        filtrar_por_status = "VENCEHOJE"
                    }
                }
            };

            try
            {
                // Faz a chamada à API
                return await _utilsService.ExecuteApiCall<object, ListarContasReceberDto>(HttpMethod.Post, new Uri(fulluri), request);

            }
            catch (Exception)
            {
                Notify($"A solicitação não retornou dados!");
                return null;
            }
        }

        private async Task<ListarContasReceberDto> ListarContasReceberSeteDias()
        {
            var baseUri = _configuration.GetSection("BasesUrl")["BaseUrlOmie"];
            if (string.IsNullOrEmpty(baseUri))
                Notify($"A URL da API não foi configurada.!");

            var fulluri = baseUri + "financas/contareceber/";

            var request = new
            {
                call = "ListarContasReceber",
                app_key = "1490222176443",
                app_secret = "6f2b10cb4d043172aa2e083613994aef",
                param = new[]
                {
                    new
                    {
                        pagina = 1,
                        registros_por_pagina = 999,
                        apenas_importado_api = "N",
                        filtrar_por_status = "AVENCER",
                        filtrar_por_data_de = $"{DateTime.Now:dd/MM/yyyy}",
                        filtrar_por_data_ate = $"{DateTime.Now.AddDays(7):dd/MM/yyyy}",
                    }
                }
            };

            try
            {
                return await _utilsService.ExecuteApiCall<object, ListarContasReceberDto>(HttpMethod.Post, new Uri(fulluri), request);
            }
            catch (Exception)
            {
                Notify($"A solicitação não retornou dados!");
                return null;
            }
        }

        private async Task AdicionarPlanilha(XLWorkbook workbook, string nomePlanilha, List<ContasReceberCadastroDto> contas)
        {
            var worksheet = workbook.AddWorksheet(nomePlanilha);

            worksheet.Cell(1, 1).Value = "CodigoLancamentoOmie";
            worksheet.Cell(1, 2).Value = "CodigoClienteFornecedor";
            worksheet.Cell(1, 3).Value = "DataEmissao";
            worksheet.Cell(1, 4).Value = "DataVencimento";
            worksheet.Cell(1, 5).Value = "StatusTitulo";
            worksheet.Cell(1, 6).Value = "ValorDocumento";
            worksheet.Cell(1, 7).Value = "Cliente";

            int row = 2;
            foreach (var conta in contas)
            {
                var fornecedor = await _clientesService.ListarDadosClientesPorCod(conta.CodigoClienteFornecedor.ToString());

                worksheet.Cell(row, 1).Value = conta.CodigoLancamentoOmie;
                worksheet.Cell(row, 2).Value = conta.CodigoClienteFornecedor;
                worksheet.Cell(row, 3).Value = DateTime.ParseExact(conta.DataEmissao, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                worksheet.Cell(row, 4).Value = DateTime.ParseExact(conta.DataVencimento, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                worksheet.Cell(row, 5).Value = conta.StatusTitulo;
                worksheet.Cell(row, 6).Value = conta.ValorDocumento.ToString(CultureInfo.InvariantCulture);
                worksheet.Cell(row, 7).Value = !string.IsNullOrEmpty(fornecedor.RazaoSocial) ? fornecedor.RazaoSocial : "Razão Social não encontrada";

                row++;
            }
        }

    }
}
