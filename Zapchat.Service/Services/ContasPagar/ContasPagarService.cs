using ClosedXML.Excel;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Zapchat.Domain.DTOs.ContasPagar;
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
        public ContasPagarService(IUtilsService utilsService, IClientesService clientesService, IConfiguration configuration, INotificator notificator) : base(notificator) 
        {
            _configuration = configuration;
            _utilsService = utilsService;
            _clientesService = clientesService;
        }

        public async Task<string> ListarContasPagarExcel(ListarContasPagarExcelDto listarContasPagarExcelDto)
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
                    }
                }
            };

            try
            {
                // Faz a chamada à API
                var response = await _utilsService.ExecuteApiCall<object, ListarContasPagarDto>(HttpMethod.Post, new Uri(fulluri), request);

                return await ExportarContasPagarXlsxBase64(response.ContaPagarCadastro);
            }
            catch (Exception)
            {
                Notify($"A solicitação não retornou dados!");
                return string.Empty;
            }
        }

        private async Task<string> ExportarContasPagarXlsxBase64(List<ContaPagarCadastroDto> ListContaPagarCadastroDto)
        {
            // Criar o arquivo Excel em memória
            using (var workbook = new XLWorkbook())
            {
                var listaAVencer = ListContaPagarCadastroDto
                            .Where(e => DateTime.TryParse(e.DataVencimento, out var dataVencimento) &&
                                        dataVencimento >= DateTime.Now.AddDays(7))
                            .ToList();

                var listaAtrasado = ListContaPagarCadastroDto
                            .Where(e => e.StatusTitulo == "ATRASADO")
                            .ToList();

                var ListaVenceHoje = ListContaPagarCadastroDto
                            .Where(e => e.StatusTitulo == "VENCEHOJE")
                            .ToList();

                

                // Preencher os dados
                int row = 2;
                foreach (var conta in listaAVencer)
                {
                    var worksheet = workbook.AddWorksheet("ContasPagar");

                    worksheet.Cell(1, 1).Value = "CodigoLancamentoOmie";
                    worksheet.Cell(1, 2).Value = "CodigoClienteFornecedor";
                    worksheet.Cell(1, 3).Value = "DataEmissao";
                    worksheet.Cell(1, 4).Value = "DataVencimento";
                    worksheet.Cell(1, 5).Value = "StatusTitulo";
                    worksheet.Cell(1, 6).Value = "ValorDocumento";
                    worksheet.Cell(1, 7).Value = "Fornecedor";

                    var fornecedor = await _clientesService.ListarDadosClientesPorCod(conta.CodigoClienteFornecedor.ToString());
                    worksheet.Cell(row, 1).Value = conta.CodigoLancamentoOmie;
                    worksheet.Cell(row, 2).Value = conta.CodigoClienteFornecedor;
                    worksheet.Cell(row, 3).Value = Convert.ToDateTime(conta.DataEmissao).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    worksheet.Cell(row, 4).Value = Convert.ToDateTime(conta.DataVencimento).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    worksheet.Cell(row, 5).Value = conta.StatusTitulo;
                    worksheet.Cell(row, 6).Value = conta.ValorDocumento.ToString(CultureInfo.InvariantCulture);
                    worksheet.Cell(row, 7).Value = !string.IsNullOrEmpty(fornecedor.RazaoSocial) ? fornecedor.RazaoSocial : "Razão Social não encontrada"; ;
                    row++;
                }

                foreach (var conta in listaAtrasado)
                {
                    var worksheet = workbook.AddWorksheet("Lista Atrasado");

                    worksheet.Cell(1, 1).Value = "CodigoLancamentoOmie";
                    worksheet.Cell(1, 2).Value = "CodigoClienteFornecedor";
                    worksheet.Cell(1, 3).Value = "DataEmissao";
                    worksheet.Cell(1, 4).Value = "DataVencimento";
                    worksheet.Cell(1, 5).Value = "StatusTitulo";
                    worksheet.Cell(1, 6).Value = "ValorDocumento";
                    worksheet.Cell(1, 7).Value = "Fornecedor";

                    var fornecedor = await _clientesService.ListarDadosClientesPorCod(conta.CodigoClienteFornecedor.ToString());
                    worksheet.Cell(row, 1).Value = conta.CodigoLancamentoOmie;
                    worksheet.Cell(row, 2).Value = conta.CodigoClienteFornecedor;
                    worksheet.Cell(row, 3).Value = Convert.ToDateTime(conta.DataEmissao).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    worksheet.Cell(row, 4).Value = Convert.ToDateTime(conta.DataVencimento).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    worksheet.Cell(row, 5).Value = conta.StatusTitulo;
                    worksheet.Cell(row, 6).Value = conta.ValorDocumento.ToString(CultureInfo.InvariantCulture);
                    worksheet.Cell(row, 7).Value = !string.IsNullOrEmpty(fornecedor.RazaoSocial) ? fornecedor.RazaoSocial : "Razão Social não encontrada"; ;
                    row++;
                }

                foreach (var conta in ListaVenceHoje)
                {
                    var worksheet = workbook.AddWorksheet("Vence Hoje");

                    worksheet.Cell(1, 1).Value = "CodigoLancamentoOmie";
                    worksheet.Cell(1, 2).Value = "CodigoClienteFornecedor";
                    worksheet.Cell(1, 3).Value = "DataEmissao";
                    worksheet.Cell(1, 4).Value = "DataVencimento";
                    worksheet.Cell(1, 5).Value = "StatusTitulo";
                    worksheet.Cell(1, 6).Value = "ValorDocumento";
                    worksheet.Cell(1, 7).Value = "Fornecedor";

                    var fornecedor = await _clientesService.ListarDadosClientesPorCod(conta.CodigoClienteFornecedor.ToString());
                    worksheet.Cell(row, 1).Value = conta.CodigoLancamentoOmie;
                    worksheet.Cell(row, 2).Value = conta.CodigoClienteFornecedor;
                    worksheet.Cell(row, 3).Value = Convert.ToDateTime(conta.DataEmissao).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    worksheet.Cell(row, 4).Value = Convert.ToDateTime(conta.DataVencimento).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    worksheet.Cell(row, 5).Value = conta.StatusTitulo;
                    worksheet.Cell(row, 6).Value = conta.ValorDocumento.ToString(CultureInfo.InvariantCulture);
                    worksheet.Cell(row, 7).Value = !string.IsNullOrEmpty(fornecedor.RazaoSocial) ? fornecedor.RazaoSocial : "Razão Social não encontrada"; ;
                    row++;
                }

                // Salvar o arquivo em memória
                using (var memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    var byteArray = memoryStream.ToArray();
                    return Convert.ToBase64String(byteArray);
                }
            }
        }

    }
}
