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

            var contator = 0;
            using (var workbook = new XLWorkbook())
            {

                foreach (var conta in listaTodasContas)
                {
                    contator++;
                    var nomePaginaPlanilha = RemoverCaracteresInvalidos(conta.Descricao[..Math.Min(31, conta.Descricao.Length)]);
                    var wsResumo = workbook.AddWorksheet(nomePaginaPlanilha);
                    int row = 2;


                    wsResumo.Cell(1, 1).Value = "Data de Lançamento";
                    wsResumo.Cell(1, 2).Value = "Categoria";
                    wsResumo.Cell(1, 3).Value = "Descrição";
                    wsResumo.Cell(1, 4).Value = "Observação";
                    wsResumo.Cell(1, 5).Value = "Origem";
                    wsResumo.Cell(1, 6).Value = "Status";
                    wsResumo.Cell(1, 7).Value = "Saldo";
                    wsResumo.Cell(1, 8).Value = "Valor Documento";

                    var extrato = await BuscarExtratoDaConta(conta, parametros);

                    foreach (var movimento in extrato.ListaMovimentos)
                    {

                        wsResumo.Cell(row, 1).Value = DateTime.ParseExact(movimento.DDataLancamento, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        wsResumo.Cell(row, 2).Value = movimento.CDesCategoria;
                        wsResumo.Cell(row, 3).Value = RemoverCaracteresInvalidos(movimento.CDesCliente);
                        wsResumo.Cell(row, 4).Value = RemoverCaracteresInvalidos(movimento.CObservacoes);
                        wsResumo.Cell(row, 5).Value = movimento.COrigem;
                        wsResumo.Cell(row, 6).Value = movimento.CSituacao;
                        wsResumo.Cell(row, 7).Value = Convert.ToDouble(movimento.NSaldo);
                        wsResumo.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
                        wsResumo.Cell(row, 8).Value = Convert.ToDouble(movimento.NValorDocumento);
                        wsResumo.Cell(row, 8).Style.NumberFormat.Format = "#,##0.00";
                        row++;
                    }

                    wsResumo.Cells().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                    wsResumo.Cells().Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

                    // Aplicar alinhamento central em todas as células
                    wsResumo.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsResumo.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    // Ajusta todas as colunas automaticamente
                    wsResumo.Columns().AdjustToContents();
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
            var primeiroDiaMesAnterior = new DateTime(hoje.Year, hoje.Month, 1).AddMonths(-1);
            var ultimoDiaMesAnterior = primeiroDiaMesAnterior.AddMonths(1).AddDays(-1);

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
                        dPeriodoInicial = primeiroDiaMesAnterior.ToString("dd/MM/yyyy"),
                        dPeriodoFinal = ultimoDiaMesAnterior.ToString("dd/MM/yyyy")
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
