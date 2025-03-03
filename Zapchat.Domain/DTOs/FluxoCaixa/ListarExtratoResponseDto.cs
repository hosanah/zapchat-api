using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Zapchat.Domain.DTOs.FluxoCaixa
{
    public class ListarExtratoResponseDto
    {
        [JsonPropertyName("nCodCC")]
        public long NCodCC { get; set; }

        [JsonPropertyName("cDescricao")]
        public string CDescricao { get; set; } = string.Empty;

        [JsonPropertyName("cCodTipo")]
        public string CCodTipo { get; set; } = string.Empty;

        [JsonPropertyName("nSaldoAnterior")]
        public decimal NSaldoAnterior { get; set; }

        [JsonPropertyName("nSaldoAtual")]
        public decimal NSaldoAtual { get; set; }

        [JsonPropertyName("nSaldoConciliado")]
        public decimal NSaldoConciliado { get; set; }

        [JsonPropertyName("nSaldoProvisorio")]
        public decimal NSaldoProvisorio { get; set; }

        [JsonPropertyName("nSaldoDisponivel")]
        public decimal NSaldoDisponivel { get; set; }

        [JsonPropertyName("listaMovimentos")]
        public List<MovimentoDto> ListaMovimentos { get; set; }
        // Outras propriedades do JSON que julgar necessárias
    }

    public class MovimentoDto
    {
        [JsonPropertyName("cDesCliente")]
        public string CDesCliente { get; set; } = string.Empty;

        [JsonPropertyName("dDataLancamento")]
        public string DDataLancamento { get; set; } = string.Empty;

        [JsonPropertyName("nSaldo")]
        public decimal NSaldo { get; set; }

        [JsonPropertyName("nValorDocumento")]
        public decimal NValorDocumento { get; set; }

        [JsonPropertyName("cDesCategoria")]
        public string CDesCategoria { get; set; } = string.Empty;

        [JsonPropertyName("cObservacoes")]
        public string CObservacoes { get; set; } = string.Empty;

        [JsonPropertyName("cOrigem")]
        public string COrigem { get; set; } = string.Empty;

        [JsonPropertyName("cSituacao")]
        public string CSituacao { get; set; } = string.Empty;
    }
}
