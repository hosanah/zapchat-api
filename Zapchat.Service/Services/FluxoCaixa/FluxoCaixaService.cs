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
using Zapchat.Domain.DTOs.FluxoCaixa;
using Zapchat.Domain.Interfaces;
using Zapchat.Domain.Interfaces.FluxoCaixa;
using Zapchat.Domain.Interfaces.Messages;

namespace Zapchat.Service.Services.FluxoCaixa
{
    public class FluxoCaixaService : BaseService, IFluxoCaixaService
    {
        private readonly IConfiguration _configuration;
        private readonly IUtilsService _utilsService;

        public FluxoCaixaService(
            IConfiguration configuration,
            IUtilsService utilsService,
            INotificator notificator) : base(notificator)
        {
            _configuration = configuration;
            _utilsService = utilsService;
        }

        public async Task<string> ListarFluxoCaixaExcel(ListarFluxoCaixaDto listarFluxoCaixaDto)
        {
            var baseUri = _configuration.GetSection("BasesUrl")["BaseUrlOmie"];
            if (string.IsNullOrEmpty(baseUri))
            {
                Notify("A URL base da Omie não foi configurada!");
                return string.Empty;
            }

            // 2) Monta a requisição para "ListarResumoContasCorrentes"
            var fullUriFluxoCaixa = baseUri + "geral/contacorrente/";
            var requestFluxoCaixa = new
            {
                call = "ListarResumoContasCorrentes",
                app_key = "1490222176443",
                app_secret = "6f2b10cb4d043172aa2e083613994aef",
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

                Console.WriteLine("🔹 JSON recebido da API:");
                Console.WriteLine(responseFluxoCaixa);
            }
            catch (Exception ex)
            {
                Notify($"Erro ao consultar contas correntes: {ex.Message}");
                return string.Empty;
            }

            if (responseFluxoCaixa == null || !responseFluxoCaixa.Conta_corrente_lista.Any())
            {
                Notify("Não retornou nenhum dado de conta corrente!");
                return string.Empty;
            }

            // 4) Une todas as contas correntes em uma lista só (caso venha paginado)
            var listaTodasContas = responseFluxoCaixa.Conta_corrente_lista;

            if (!listaTodasContas.Any())
            {
                Notify("Não há contas para processar!");
                return string.Empty;
            }

            return await ExportarFluxoCaixaXlsxBase64(listaTodasContas, listarFluxoCaixaDto);

        }

        private static string RemoverCaracteresInvalidos(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "Planilha"; // Nome padrão caso seja nulo ou vazio

            // Remove caracteres inválidos para nome de planilha no Excel
            string nomeLimpo = Regex.Replace(texto, @"[\/\\?*:()\[\]]", "");

            return nomeLimpo;
        }

        private async Task<string> ExportarFluxoCaixaXlsxBase64(List<ContaCorrenteDto> listaTodasContas, ListarFluxoCaixaDto listarFluxoCaixaDto)
        {
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
                    wsResumo.Cell(1, 2).Value = "Valor do Lançamento";
                    wsResumo.Cell(1, 3).Value = "Saldo";

                    var extrato = await BuscarExtratoDaConta(conta, listarFluxoCaixaDto);

                    foreach (var movimento in extrato.ListaMovimentos)
                    {

                        wsResumo.Cell(row, 1).Value = movimento.DDataLancamento;
                        wsResumo.Cell(row, 2).Value = Convert.ToDouble(movimento.NValorDocumento);
                        wsResumo.Cell(row, 3).Value= Convert.ToDouble(movimento.NSaldo);
                        row++;
                    }
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
        
        
        private async Task<ListarExtratoResponseDto> BuscarExtratoDaConta(ContaCorrenteDto contaCorrente, ListarFluxoCaixaDto listarFluxoCaixaDto)
        {
            var baseUri = _configuration.GetSection("BasesUrl")["BaseUrlOmie"];
            if (string.IsNullOrEmpty(baseUri))
                throw new InvalidOperationException("A URL da API não foi configurada.");

            var fulluri = baseUri + "financas/extrato/";
            var request = new
            {
                call = "ListarExtrato",
                app_key = "1490222176443",
                app_secret = "6f2b10cb4d043172aa2e083613994aef",
                param = new[]
                {
                    new
                        {
                            nCodCC = contaCorrente.NCodCC,
                            cCodIntCC = "",
                            dPeriodoInicial = listarFluxoCaixaDto.DataInicio?.ToString("dd/MM/yyyy") ?? "",
                            dPeriodoFinal = listarFluxoCaixaDto.DataFim?.ToString("dd/MM/yyyy") ?? ""
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
    }
}
