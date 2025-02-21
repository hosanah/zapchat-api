using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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

                List<string> worksheetsNames = new()
                {
                    "CodigoLancamentoOmie",
                    "CodigoClienteFornecedor",
                    "DataEmissao",
                    "DataVencimento",
                    "DataPrevisao",
                    "StatusTitulo",
                    "ValorDocumento",
                    "Categoria",
                    "NumeroDocumento",
                    "Fornecedor",
                    "CnpjFornecedor",
                    "ParaTabulacao1",
                    "ParaTabulacao2"
                };


                // Adiciona as planilhas
                AdicionarPlanilha(workbook, "Vence em 7 dias", listaAVencer.ContaPagarCadastro, listaClientes, worksheetsNames);
                AdicionarPlanilha(workbook, "Lista Atrasado", listaAtrasado.ContaPagarCadastro, listaClientes, worksheetsNames);
                AdicionarPlanilha(workbook, "Vence Hoje", listaVenceHoje.ContaPagarCadastro, listaClientes, worksheetsNames);

                List<string> worksheetsNamesConsolidado = new()
                {
                    "Fornecedor",
                    "Documento",
                    "Valor TT",
                    "Pago",
                    "Atrasado",
                    "Saldo",
                    "Situação",
                    "Categoria",
                    "Quantidade 7 dias",
                    "Total 7 dias",
                };
                AdicionarPlanilhaSumario(workbook, "Consolidado", worksheetsNamesConsolidado);

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
                        filtrar_por_status = "ATRASADO",
                        filtrar_por_data_de = $"{DateTime.Now.AddDays(-30):dd/MM/yyyy}"
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

        private static void AdicionarPlanilha(XLWorkbook workbook, string nomePlanilha, List<ContaPagarCadastroDto> contas, List<DadosClientesDto> listaClientes, List<string> worksheetsNames)
        {
            var worksheet = workbook.AddWorksheet(nomePlanilha);

            int worksheetrow = 1;
            foreach (var worksheetsName in worksheetsNames)
            {
                string corHex = "#00569d";
                var corXL = XLColor.FromHtml(corHex);
                worksheet.Cell(1, worksheetrow).Value = worksheetsName;
                // Definir uma cor de fundo em hexadecimal (Ex: Azul #3498db)
                worksheet.Cell(1, worksheetrow).Style.Fill.BackgroundColor = corXL;
                worksheet.Cell(1, worksheetrow).Style.Font.Bold = true;
                worksheet.Cell(1, worksheetrow).Style.Font.FontColor = XLColor.White;
                worksheetrow++;
            }
            

            int row = 2;
            foreach (var conta in contas)
            {
                worksheet.Cell(row, 1).Value = conta.CodigoLancamentoOmie;
                worksheet.Cell(row, 2).Value = conta.CodigoClienteFornecedor;
                worksheet.Cell(row, 3).Value = DateTime.ParseExact(conta.DataEmissao, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                worksheet.Cell(row, 4).Value = DateTime.ParseExact(conta.DataVencimento, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                worksheet.Cell(row, 5).Value = DateTime.ParseExact(conta.DataPrevisao, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                worksheet.Cell(row, 6).Value = conta.StatusTitulo;
                worksheet.Cell(row, 7).Value = conta.ValorDocumento;
                worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(row, 8).Value = string.Join(", ", conta.Categorias.Select(c => c.CodigoCategoria));
                worksheet.Cell(row, 9).Value = conta.NumeroDocumentoFiscal; 
                worksheet.Cell(row, 10).Value = listaClientes.Where(e => e.CodClienteOmie == conta.CodigoClienteFornecedor).First().RazaoSocial;
                worksheet.Cell(row, 11).Value = listaClientes.Where(e => e.CodClienteOmie == conta.CodigoClienteFornecedor).First().CnpjCpf;
                worksheet.Cell(row, 12).FormulaA1 = $"=H2"; ;
                worksheet.Cell(row, 13).Value = Convert.ToDateTime(conta.DataVencimento).Date < DateTime.Today ? "Sim" : "Não";

                row++;
            }
            // Aplicar bordas externas e internas para toda a planilha
            worksheet.Cells().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Cells().Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

            // Aplicar alinhamento central em todas as células
            worksheet.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            
            // Ajusta todas as colunas automaticamente
            worksheet.Columns().AdjustToContents();
            
        }

        private static void AdicionarPlanilhaSumario(XLWorkbook workbook, string nomePlanilha, List<string> worksheetsNames)
        {
            var worksheet = workbook.AddWorksheet(nomePlanilha);
            int worksheetrow = 1;
            foreach (var worksheetsName in worksheetsNames)
            {
                string corHex = "#00569d";
                var corXL = XLColor.FromHtml(corHex);
                worksheet.Cell(1, worksheetrow).Value = worksheetsName;
                // Definir uma cor de fundo em hexadecimal (Ex: Azul #3498db)
                worksheet.Cell(1, worksheetrow).Style.Fill.BackgroundColor = corXL;
                worksheet.Cell(1, worksheetrow).Style.Font.Bold = true;
                worksheet.Cell(1, worksheetrow).Style.Font.FontColor = XLColor.White;
                worksheetrow++;
            }


            var sheets = new List<string> { "Vence em 7 dias", "Lista Atrasado", "Vence Hoje" };
            var uniqueSuppliers = new HashSet<string>(); // Armazena fornecedores sem repetição

            // Percorrer todas as planilhas especificadas
            foreach (var sheetName in sheets)
            {
                var sheet = workbook.Worksheet(sheetName);
                var col = sheet.Column("J"); // Coluna G onde estão os fornecedores

                foreach (var cell in col.CellsUsed().Skip(1)) // Começa em G2
                {
                    string supplierName = cell.GetString().Trim();
                    if (!string.IsNullOrEmpty(supplierName))
                    {
                        uniqueSuppliers.Add(supplierName); // Adiciona ao HashSet (evita duplicatas)

                    }
                }
            }

            int row = 2;
            foreach (var supplier in uniqueSuppliers.OrderBy(x => x)) // Ordena por nome
            {
                worksheet.Cell(row, 1).Value = supplier;

                // Adicionar a fórmula na coluna B
                worksheet.Cell(row, 2).FormulaA1 = $"=IFERROR(VLOOKUP(A{row}, 'Lista Atrasado'!J:K, 2, 0), \"-\")";
                worksheet.Cell(row, 3).FormulaA1 = $"=SUMIFS('Lista Atrasado'!G:G, 'Lista Atrasado'!J:J, $A{row})";
                worksheet.Cell(row, 4).FormulaA1 = $"=SUMIFS('Lista Atrasado'!G:G, 'Lista Atrasado'!F:F, \"PAGO\", 'Lista Atrasado'!J:J, $A{row})";
                worksheet.Cell(row, 5).FormulaA1 = $"=SUMIFS('Lista Atrasado'!G:G, 'Lista Atrasado'!F:F, \"ATRASADO\", 'Lista Atrasado'!J:J, $A{row})";
                worksheet.Cell(row, 6).FormulaA1 = $"=IF(D{row}=C{row}, \"Pago\", \"Em Atraso\")";
                worksheet.Cell(row, 7).FormulaA1 = $"=IF(F{row}=\"Em Atraso\", \"Pendente\", \"Conciliado\")";
                worksheet.Cell(row, 8).FormulaA1 = $"=IFERROR(VLOOKUP(B{row}, 'Lista Atrasado'!K:L, 2, 0), \"-\")";
                worksheet.Cell(row, 9).FormulaA1 = $"=COUNTIFS('Lista Atrasado'!M:M, \"SIM\", 'Lista Atrasado'!J:J, $A{row})";
                worksheet.Cell(row, 10).FormulaA1 = $"=SUMIFS('Lista Atrasado'!G:G, 'Lista Atrasado'!J:J, $A{row}, 'Lista Atrasado'!M:M, \"SIM\")";

                row++;
            }


            // Aplicar bordas externas e internas para toda a planilha
            worksheet.Cells().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Cells().Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

            // Aplicar alinhamento central em todas as células
            worksheet.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Ajusta todas as colunas automaticamente
            worksheet.Columns().AdjustToContents();

        }

    }
}
