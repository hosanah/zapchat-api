using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Zapchat.Domain.DTOs.Clientes;
using Zapchat.Domain.DTOs.ContasPagar;
using Zapchat.Domain.DTOs.ContasReceber;
using Zapchat.Domain.Interfaces;
using Zapchat.Domain.Interfaces.Clientes;
using Zapchat.Domain.Interfaces.ContasPagar;
using Zapchat.Domain.Interfaces.Messages;

namespace Zapchat.Service.Services.ContasPagar
{
    public class ContasPagarService : BaseService, IContasPagarService
    {
        private readonly IConfiguration _configuration;
        private readonly IUtilsService _utilsService;
        private readonly IClientesService _clientesService;
        private readonly SemaphoreSlim _semaphore = new(5);
        public ContasPagarService(IUtilsService utilsService, IClientesService clientesService, IConfiguration configuration, INotificator notificator) : base(notificator) 
        {
            _configuration = configuration;
            _utilsService = utilsService;
            _clientesService = clientesService;
        }

        public async Task<string> ListarContasPagarExcel(ListarContasPagarExcelDto listarContasPagarExcelDto)
        {

            try
            {
                return await ExportarContasPagarXlsxBase64();
            }
            catch (Exception ex)
            {
                Notify($"A solicitação não retornou dados!{ex.Message}");
                return string.Empty;
            }
        }

        public async Task<string> ExportarContasPagarXlsxBase64()
        {
            // Criar o arquivo Excel em memória
            using (var workbook = new XLWorkbook())
            {
                var listaAVencer = await ListarContasPagarSeteDias();
                var listaAtrasado = await ListarContasPagarAtrasados();
                var listaVenceHoje = await ListarContasPagarVenceHoje();

                var listaClientes = new List<DadosClientesDto>();

                var codigosUnicos = listaAVencer.ContaPagarCadastro
                                    .Concat(listaAtrasado.ContaPagarCadastro)
                                    .Concat(listaVenceHoje.ContaPagarCadastro)
                                    .Select(x => x.CodigoClienteFornecedor)
                                    .Distinct()
                                    .ToList();

                foreach (var codigo in codigosUnicos)
                {
                    listaClientes.Add(await _clientesService.ListarDadosClientesPorCod(codigo.ToString()));
                }


                // Adiciona as planilhas
                await AdicionarPlanilha(workbook, "Vence em 7 dias", listaAVencer.ContaPagarCadastro, listaClientes);
                await AdicionarPlanilha(workbook, "Lista Atrasado", listaAtrasado.ContaPagarCadastro, listaClientes);
                await AdicionarPlanilha(workbook, "Vence Hoje", listaVenceHoje.ContaPagarCadastro, listaClientes);

                // Salvar o arquivo em memória
                using (var memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    var byteArray = memoryStream.ToArray();
                    return Convert.ToBase64String(byteArray);
                }
            }
        }

        private async Task<ListarContasPagarDto> ListarContasPagarAtrasados()
        {
            var baseUri = _configuration.GetSection("BasesUrl")["BaseUrlOmie"];
            if (string.IsNullOrEmpty(baseUri))
                Notify($"A URL da API não foi configurada.!");

            var fulluri = baseUri + "financas/contapagar/";

            var request = new
            {
                call = "ListarContasPagar",
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
                return await _utilsService.ExecuteApiCall<object, ListarContasPagarDto>(HttpMethod.Post, new Uri(fulluri), request);

            }
            catch (Exception)
            {
                Notify($"A solicitação não retornou dados!");
                return null;
            }
        }

        private async Task<ListarContasPagarDto> ListarContasPagarVenceHoje()
        {
            var baseUri = _configuration.GetSection("BasesUrl")["BaseUrlOmie"];
            if (string.IsNullOrEmpty(baseUri))
                Notify($"A URL da API não foi configurada.!");

            var fulluri = baseUri + "financas/contapagar/";

            var request = new
            {
                call = "ListarContasPagar",
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
                return await _utilsService.ExecuteApiCall<object, ListarContasPagarDto>(HttpMethod.Post, new Uri(fulluri), request);

            }
            catch (Exception)
            {
                Notify($"A solicitação não retornou dados!");
                return null;
            }
        }

        private async Task<ListarContasPagarDto> ListarContasPagarSeteDias()
        {
            var baseUri = _configuration.GetSection("BasesUrl")["BaseUrlOmie"];
            if (string.IsNullOrEmpty(baseUri))
                Notify($"A URL da API não foi configurada.!");

            var fulluri = baseUri + "financas/contapagar/";

            var request = new
            {
                call = "ListarContasPagar",
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
                // Faz a chamada à API
                return await _utilsService.ExecuteApiCall<object, ListarContasPagarDto>(HttpMethod.Post, new Uri(fulluri), request);
            }
            catch (Exception)
            {
                Notify($"A solicitação não retornou dados!");
                return null;
            }
        }

        private async Task AdicionarPlanilha(XLWorkbook workbook, string nomePlanilha, List<ContaPagarCadastroDto> contas, List<DadosClientesDto> listaClientes)
        {
            var worksheet = workbook.AddWorksheet(nomePlanilha);

            worksheet.Cell(1, 1).Value = "CodigoLancamentoOmie";
            worksheet.Cell(1, 2).Value = "CodigoClienteFornecedor";
            worksheet.Cell(1, 3).Value = "DataEmissao";
            worksheet.Cell(1, 4).Value = "DataVencimento";
            worksheet.Cell(1, 5).Value = "StatusTitulo";
            worksheet.Cell(1, 6).Value = "ValorDocumento";
            worksheet.Cell(1, 7).Value = "Fornecedor";

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
                worksheet.Cell(row, 7).Value = listaClientes.Where(e => e.CodClienteOmie == conta.CodigoClienteFornecedor).First().RazaoSocial;

                row++;
            }
        }

    }
}
