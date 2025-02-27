using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Zapchat.Domain.DTOs.Categoria;
using Zapchat.Domain.DTOs.Clientes;
using Zapchat.Domain.DTOs.ContasPagar;
using Zapchat.Domain.DTOs.ContasReceber;
using Zapchat.Domain.Entities;
using Zapchat.Domain.Interfaces;
using Zapchat.Domain.Interfaces.Categoria;
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
        private readonly ICategoriaService _categoriaService;
        private readonly IParametroSistemaService _parametroSistemaService;
        private readonly SemaphoreSlim _semaphore = new(5);
        public ContasPagarService(IUtilsService utilsService, 
            IClientesService clientesService, 
            IConfiguration configuration, 
            INotificator notificator, 
            ICategoriaService categoriaService, 
            IParametroSistemaService 
            parametroSistemaService) : base(notificator) 
        {
            _configuration = configuration;
            _utilsService = utilsService;
            _clientesService = clientesService;
            _categoriaService = categoriaService;
            _parametroSistemaService = parametroSistemaService;
        }

        public async Task<string> ListarContasPagarExcel(ListarContasPagarExcelDto listarContasPagarExcelDto)
        {
            if (!string.IsNullOrWhiteSpace(listarContasPagarExcelDto.GrupoIdentificador))
            {
                try
                {
                    return await ExportarContasPagarXlsxBase64(listarContasPagarExcelDto.GrupoIdentificador);
                }
                catch (Exception ex)
                {
                    Notify($"A solicitação não retornou dados!{ex.Message}");
                    return string.Empty;
                }
            } 
            else 
            {
                Notify($"É necessário informar o Grupo Identificador");
                return string.Empty;
            }
            
        }

        public async Task<string> ExportarContasPagarXlsxBase64(string grupoIdentificador)
        {

            var parametros = await _parametroSistemaService.BuscarParammetroPorGrupoIdentificador(grupoIdentificador);
            if (parametros == null)
                return string.Empty;
            // Criar o arquivo Excel em memória
            using (var workbook = new XLWorkbook())
            {
                var listaAVencer = await ListarContasPagarSeteDias(parametros);
                var listaAtrasado = await ListarContasPagarAtrasados(parametros);
                var listaVenceHoje = await ListarContasPagarVenceHoje(parametros);

                if(listaAVencer.ContaPagarCadastro.Any())
                    listaAVencer.ContaPagarCadastro = listaAVencer.ContaPagarCadastro
                    .Where(c => DateTime.ParseExact(c.DataVencimento, "dd/MM/yyyy", CultureInfo.InvariantCulture) >= DateTime.Now && DateTime.ParseExact(c.DataVencimento, "dd/MM/yyyy", CultureInfo.InvariantCulture) <= DateTime.Now.AddDays(7))
                    .ToList();

                if (listaAtrasado.ContaPagarCadastro.Any())
                    listaAtrasado.ContaPagarCadastro = listaAtrasado.ContaPagarCadastro
                    .Where(c => DateTime.ParseExact(c.DataVencimento, "dd/MM/yyyy", CultureInfo.InvariantCulture) >= DateTime.Now.AddDays(-30))
                    .ToList();

                var listaClientes = new List<DadosClientesDto>();
                var listaCategorias = new List<DadosCategoriaDto>();

                var codigosUnicosFornecedores = listaAVencer.ContaPagarCadastro
                                    .Concat(listaAtrasado.ContaPagarCadastro)
                                    .Concat(listaVenceHoje.ContaPagarCadastro)
                                    .Select(x => x.CodigoClienteFornecedor)
                                    .Distinct()
                                    .ToList();

                foreach (var codigo in codigosUnicosFornecedores)
                {
                    listaClientes.Add(await _clientesService.ListarDadosClientesPorCod(codigo.ToString(), grupoIdentificador));
                }

                var codigosUnicosCategorias = listaAVencer.ContaPagarCadastro
                                    .Concat(listaAtrasado.ContaPagarCadastro)
                                    .Concat(listaVenceHoje.ContaPagarCadastro)
                                    .Select(x => x.CodigoCategoria)
                                    .Distinct()
                                    .ToList();

                foreach (var codigo in codigosUnicosCategorias)
                {
                    listaCategorias.Add(await _categoriaService.ListarDadosCategoriaPorCod(codigo.ToString(), grupoIdentificador));
                }

                List<string> worksheetsNames = new()
                {
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
                AdicionarPlanilha(workbook, "Vence em 7 dias", listaAVencer.ContaPagarCadastro, listaClientes, listaCategorias, worksheetsNames);
                AdicionarPlanilha(workbook, "Lista Atrasado", listaAtrasado.ContaPagarCadastro, listaClientes, listaCategorias, worksheetsNames);
                AdicionarPlanilha(workbook, "Vence Hoje", listaVenceHoje.ContaPagarCadastro, listaClientes, listaCategorias, worksheetsNames);

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

        private async Task<ListarContasPagarDto> ListarContasPagarAtrasados(ParamGrupoWhatsApp param)
        {
            var baseUri = _configuration.GetSection("BasesUrl")["BaseUrlOmie"];
            if (string.IsNullOrEmpty(baseUri))
                Notify($"A URL da API não foi configurada.!");

            var fulluri = baseUri + "financas/contapagar/";

            var request = new
            {
                call = "ListarContasPagar",
                app_key = $"{param.AppKey}",
                app_secret = $"{param.AppSecret}",
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

        private async Task<ListarContasPagarDto> ListarContasPagarVenceHoje(ParamGrupoWhatsApp param)
        {
            var baseUri = _configuration.GetSection("BasesUrl")["BaseUrlOmie"];
            if (string.IsNullOrEmpty(baseUri))
                Notify($"A URL da API não foi configurada.!");

            var fulluri = baseUri + "financas/contapagar/";

            var request = new
            {
                call = "ListarContasPagar",
                app_key = $"{param.AppKey}",
                app_secret = $"{param.AppSecret}",
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

        private async Task<ListarContasPagarDto> ListarContasPagarSeteDias(ParamGrupoWhatsApp param)
        {
            var baseUri = _configuration.GetSection("BasesUrl")["BaseUrlOmie"];
            if (string.IsNullOrEmpty(baseUri))
                Notify($"A URL da API não foi configurada.!");

            var fulluri = baseUri + "financas/contapagar/";

            var request = new
            {
                call = "ListarContasPagar",
                app_key = $"{param.AppKey}",
                app_secret = $"{param.AppSecret}",
                param = new[]
                {
                    new
                    {
                        pagina = 1,
                        registros_por_pagina = 999,
                        apenas_importado_api = "N",
                        filtrar_por_status = "AVENCER"
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

        private static void AdicionarPlanilha(XLWorkbook workbook, string nomePlanilha, List<ContaPagarCadastroDto> contas, List<DadosClientesDto> listaClientes, List<DadosCategoriaDto> listaCategorias, List<string> worksheetsNames)
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
                worksheet.Cell(row, 1).Value = DateTime.ParseExact(conta.DataEmissao, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                worksheet.Cell(row, 2).Value = DateTime.ParseExact(conta.DataVencimento, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                worksheet.Cell(row, 3).Value = DateTime.ParseExact(conta.DataPrevisao, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                worksheet.Cell(row, 4).Value = conta.StatusTitulo == "ATRASADO" ? conta.StatusTitulo : "RECEBIDO";
                worksheet.Cell(row, 5).Value = conta.ValorDocumento;
                worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(row, 6).Value = listaCategorias.Where(e => e.Codigo == conta.CodigoCategoria).First().Descricao;
                worksheet.Cell(row, 7).Value = conta.NumeroDocumentoFiscal; 
                worksheet.Cell(row, 8).Value = listaClientes.Where(e => e.CodClienteOmie == conta.CodigoClienteFornecedor).First().RazaoSocial;
                worksheet.Cell(row, 9).Value = listaClientes.Where(e => e.CodClienteOmie == conta.CodigoClienteFornecedor).First().CnpjCpf;
                worksheet.Cell(row, 10).FormulaA1 = $"=F{row}";
                DateTime dataConvertida = DateTime.ParseExact(conta.DataVencimento, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                worksheet.Cell(row, 11).Value = dataConvertida.Date < DateTime.Today ? "Sim" : "Não";

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
                var col = sheet.Column("H"); // Coluna G onde estão os fornecedores

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
                worksheet.Cell(row, 2).FormulaA1 = $"=IFERROR(VLOOKUP(A{row}, 'Lista Atrasado'!H:I, 2, 0), \"-\")";
                worksheet.Cell(row, 3).FormulaA1 = $"=SUMIFS('Lista Atrasado'!E:E, 'Lista Atrasado'!F:F, $A{row})";
                worksheet.Cell(row, 4).FormulaA1 = $"=SUMIFS('Lista Atrasado'!E:E, 'Lista Atrasado'!D:D, \"PAGO\", 'Lista Atrasado'!H:H, $A{row})";
                worksheet.Cell(row, 5).FormulaA1 = $"=SUMIFS('Lista Atrasado'!E:E, 'Lista Atrasado'!D:D, \"ATRASADO\", 'Lista Atrasado'!H:H, $A{row})";
                worksheet.Cell(row, 6).FormulaA1 = $"=IF(D{row}=C{row}, \"Pago\", \"Em Atraso\")";
                worksheet.Cell(row, 7).FormulaA1 = $"=IF(F{row}=\"Em Atraso\", \"Pendente\", \"Conciliado\")";
                worksheet.Cell(row, 8).FormulaA1 = $"=IFERROR(VLOOKUP(B{row}, 'Lista Atrasado'!I:J, 2, 0), \"-\")";
                worksheet.Cell(row, 9).FormulaA1 = $"=COUNTIFS('Lista Atrasado'!K:K, \"SIM\", 'Lista Atrasado'!H:H, $A{row})";
                worksheet.Cell(row, 10).FormulaA1 = $"=SUMIFS('Lista Atrasado'!E:E, 'Lista Atrasado'!H:H, $A{row}, 'Lista Atrasado'!K:K, \"SIM\")";

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
