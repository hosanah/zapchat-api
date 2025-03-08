using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zapchat.Domain.DTOs.Clientes;
using Zapchat.Domain.DTOs.ContasPagar;
using Zapchat.Domain.DTOs.FluxoCaixa;
using Zapchat.Domain.Entities;
using Zapchat.Domain.Interfaces;
using Zapchat.Domain.Interfaces.FluxoCaixa;
using Zapchat.Domain.Interfaces.Messages;

namespace Zapchat.Service.Services.FluxoCaixa
{
    public class FluxoCaixaService : BaseService, IFluxoCaixaService
    {
        private readonly IConfiguration _configuration;
        private readonly IUtilsService _utilsService;
        private readonly IParametroSistemaService _parametroSistemaService;

        public FluxoCaixaService(
            IConfiguration configuration,
            IUtilsService utilsService,
            INotificator notificator,
            IParametroSistemaService parametroSistemaService) : base(notificator)
        {
            _configuration = configuration;
            _utilsService = utilsService;
            _parametroSistemaService = parametroSistemaService;
        }

        public async Task<string> ListarFluxoCaixaExcel(ListarFluxoCaixaDto listarFluxoCaixaDto)
        {

            if (!string.IsNullOrWhiteSpace(listarFluxoCaixaDto.GrupoIdentificador))
            {
                try
                {
                    return await ExportarFluxoCaixaXlsxBase64(listarFluxoCaixaDto.GrupoIdentificador);
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

        private static string RemoverCaracteresInvalidos(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "Planilha"; // Nome padrão caso seja nulo ou vazio

            // Remove caracteres inválidos para nome de planilha no Excel
            string nomeLimpo = Regex.Replace(texto, @"[\/\\?*:()\[\]]", "");

            return nomeLimpo;
        }

        private async Task<string> ExportarFluxoCaixaXlsxBase64(string grupoIdentificador)
        {
            var parametros = await _parametroSistemaService.BuscarParammetroPorGrupoIdentificador(grupoIdentificador);
            if (parametros == null)
                return string.Empty;

            var listaTodasContas = await ListarContasCorrente(parametros);

            using (var workbook = new XLWorkbook())
            {
                foreach (var conta in listaTodasContas)
                {
                    var nomePaginaPlanilha = RemoverCaracteresInvalidos(conta.Descricao[..Math.Min(31, conta.Descricao.Length)]);
                    var wsResumo = workbook.AddWorksheet(nomePaginaPlanilha);
                    int row = 2;

                    // Cabeçalhos
                    var headerRange = wsResumo.Range("A1:G1");
                    headerRange.Style.Fill.BackgroundColor = XLColor.Green;
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Font.FontColor = XLColor.White;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    wsResumo.Cell(1, 1).Value = "Situação";
                    wsResumo.Cell(1, 2).Value = "Data";
                    wsResumo.Cell(1, 3).Value = "Cliente ou Fornecedor";
                    wsResumo.Cell(1, 4).Value = "Documento";
                    wsResumo.Cell(1, 5).Value = "Categoria";
                    wsResumo.Cell(1, 6).Value = "Valor";
                    wsResumo.Cell(1, 7).Value = "Saldo";

                    var extrato = await BuscarExtratoDaConta(conta, parametros);
                    var movimentosOrdenados = extrato.ListaMovimentos.OrderBy(m => DateTime.ParseExact(m.DDataLancamento, "dd/MM/yyyy", CultureInfo.InvariantCulture)).ToList();

                    foreach (var movimento in movimentosOrdenados)
                    {
                        bool isSaldo = string.IsNullOrWhiteSpace(movimento.CRazCliente) &&
                                       string.IsNullOrWhiteSpace(movimento.CNumero) &&
                                       string.IsNullOrWhiteSpace(movimento.CDesCategoria);

                        wsResumo.Cell(row, 1).Value = movimento.CSituacao;
                        wsResumo.Cell(row, 1).Style.Fill.BackgroundColor = movimento.CSituacao == "Conciliado" ? XLColor.Green : XLColor.White;
                        wsResumo.Cell(row, 1).Style.Font.FontColor = XLColor.White;
                        wsResumo.Cell(row, 2).Value = DateTime.ParseExact(movimento.DDataLancamento, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yy");
                        wsResumo.Cell(row, 3).Value = isSaldo ? "SALDO" : movimento.CRazCliente;
                        wsResumo.Cell(row, 4).Value = movimento.CNumero;
                        wsResumo.Cell(row, 5).Value = movimento.CDesCategoria;

                        double valor = Convert.ToDouble(movimento.NValorDocumento);
                        wsResumo.Cell(row, 6).Value = valor;
                        wsResumo.Cell(row, 6).Style.Font.FontColor = valor < 0 ? XLColor.Red : XLColor.Blue;
                        wsResumo.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";

                        wsResumo.Cell(row, 7).Value = Convert.ToDouble(movimento.NSaldo);
                        wsResumo.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
                        row++;
                    }

                    // Melhorando layout e removendo caixas
                    wsResumo.Columns().AdjustToContents();
                    wsResumo.Rows().AdjustToContents();
                    wsResumo.Rows().Style.Border.TopBorder = XLBorderStyleValues.None;
                    wsResumo.Rows().Style.Border.BottomBorder = XLBorderStyleValues.None;
                    wsResumo.Rows().Style.Border.LeftBorder = XLBorderStyleValues.None;
                    wsResumo.Rows().Style.Border.RightBorder = XLBorderStyleValues.None;
                    wsResumo.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    wsResumo.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                }

                // Retorna o Excel em Base64
                using (var memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    var byteArray = memoryStream.ToArray();
                    return Convert.ToBase64String(byteArray);
                }
            }
        }


        private async Task<ListarExtratoResponseDto> BuscarExtratoDaConta(ContaCorrenteDto contaCorrente, ParamGrupoWhatsApp param)
        {
            var baseUri = _configuration.GetSection("BasesUrl")["BaseUrlOmie"];
            if (string.IsNullOrEmpty(baseUri))
                throw new InvalidOperationException("A URL da API não foi configurada.");

            var hoje = DateTime.Today;
            var dataFinal = hoje.AddDays(-1); 
            var dataInicial = dataFinal.AddDays(-6); 

            var fulluri = baseUri + "financas/extrato/";
            var request = new
            {
                call = "ListarExtrato",
                app_key = $"{param.AppKey}",
                app_secret = $"{param.AppSecret}",
                param = new[]
                {
                    new
                    {
                        nCodCC = contaCorrente.NCodCC,
                        cCodIntCC = "",
                        dPeriodoInicial = dataInicial.ToString("dd/MM/yyyy"),
                        dPeriodoFinal = dataFinal.ToString("dd/MM/yyyy")
                    }
                }
            };

            try
            {
                return await _utilsService.ExecuteApiCall<object, ListarExtratoResponseDto>(HttpMethod.Post,new Uri(fulluri), request);
            }
            catch (Exception)
            {
                Notify($"A solicitação não retornou dados!");
                return null;
            }
        }

        private async Task<List<ContaCorrenteDto>> ListarContasCorrente(ParamGrupoWhatsApp param)
        {
            var baseUri = _configuration.GetSection("BasesUrl")["BaseUrlOmie"];
            if (string.IsNullOrEmpty(baseUri))
            {
                Notify("A URL base da Omie não foi configurada!");
                return null;
            }

            var fullUriFluxoCaixa = baseUri + "geral/contacorrente/";
            var requestFluxoCaixa = new
            {
                call = "ListarResumoContasCorrentes",
                app_key = $"{param.AppKey}",
                app_secret = $"{param.AppSecret}",
                param = new[]
                {
                    new
                    {
                        pagina = 1,
                        registros_por_pagina = 100,
                        apenas_importado_api = "N"
                    }
                }
            };

            // 3) Chama a API Omie para pegar a lista de contas correntes
            ListarResumoContasCorrentesResponseDto? responseFluxoCaixa;
            try
            {
                responseFluxoCaixa = await _utilsService.ExecuteApiCall<object, ListarResumoContasCorrentesResponseDto>(
                    HttpMethod.Post,
                    new Uri(fullUriFluxoCaixa),
                    requestFluxoCaixa
                );

               
            }
            catch (Exception ex)
            {
                Notify($"Erro ao consultar contas correntes: {ex.Message}");
                return null;
            }

            if (responseFluxoCaixa == null || !responseFluxoCaixa.Conta_corrente_lista.Any())
            {
                Notify("Não retornou nenhum dado de conta corrente!");
                return null;
            }

            // 4) Une todas as contas correntes em uma lista só (caso venha paginado)
            var listaTodasContas = responseFluxoCaixa.Conta_corrente_lista;

            if (!listaTodasContas.Any())
            {
                Notify("Não há contas para processar!");
                return null;
            }

            return listaTodasContas;
        }
    }
}
