using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Zapchat.Domain.DTOs.ContasReceber
{
    public class ContasReceberCadastroDto
    {
        [JsonPropertyName("boleto")]
        public BoletoDto? Boleto { get; set; }

        [JsonPropertyName("categorias")]
        public List<CategoriaDto>? Categorias { get; set; }

        [JsonPropertyName("codigo_categoria")]
        public string CodigoCategoria { get; set; } = string.Empty;

        [JsonPropertyName("codigo_cliente_fornecedor")]
        public long CodigoClienteFornecedor { get; set; }

        [JsonPropertyName("codigo_lancamento_integracao")]
        public string CodigoLancamentoIntegracao { get; set; } = string.Empty;

        [JsonPropertyName("codigo_lancamento_omie")]
        public long CodigoLancamentoOmie { get; set; }

        [JsonPropertyName("codigo_tipo_documento")]
        public string CodigoTipoDocumento { get; set; } = string.Empty;

        [JsonPropertyName("data_emissao")]
        public string DataEmissao { get; set; } = string.Empty;

        [JsonPropertyName("data_previsao")]
        public string DataPrevisao { get; set; } = string.Empty;

        [JsonPropertyName("data_registro")]
        public string DataRegistro { get; set; } = string.Empty;

        [JsonPropertyName("data_vencimento")]
        public string DataVencimento { get; set; } = string.Empty;

        [JsonPropertyName("distribuicao")]
        public List<object>? Distribuicao { get; set; }

        [JsonPropertyName("id_conta_corrente")]
        public long IdContaCorrente { get; set; }

        [JsonPropertyName("id_origem")]
        public string IdOrigem { get; set; } = string.Empty;

        [JsonPropertyName("info")]
        public InfoDto? Info { get; set; }

        [JsonPropertyName("numero_parcela")]
        public string NumeroParcela { get; set; } = string.Empty;

        [JsonPropertyName("status_titulo")]
        public string StatusTitulo { get; set; } = string.Empty;

        [JsonPropertyName("tipo_agrupamento")]
        public string TipoAgrupamento { get; set; } = string.Empty;

        [JsonPropertyName("valor_documento")]
        public decimal ValorDocumento { get; set; }
    }

    public class BoletoDto
    {
        [JsonPropertyName("cGerado")]
        public string CGerado { get; set; } = string.Empty;

        [JsonPropertyName("cNumBancario")]
        public string CNumBancario { get; set; } = string.Empty;

        [JsonPropertyName("cNumBoleto")]
        public string CNumBoleto { get; set; } = string.Empty;

        [JsonPropertyName("dDtEmBol")]
        public string DataEmissaoBoleto { get; set; } = string.Empty;

        [JsonPropertyName("nPerJuros")]
        public decimal PercentualJuros { get; set; }

        [JsonPropertyName("nPerMulta")]
        public decimal PercentualMulta { get; set; }
    }

    public class CategoriaDto
    {
        [JsonPropertyName("codigo_categoria")]
        public string CodigoCategoria { get; set; } = string.Empty;

        [JsonPropertyName("percentual")]
        public decimal Percentual { get; set; }

        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }
    }

    public class InfoDto
    {
        [JsonPropertyName("cImpAPI")]
        public string CImpAPI { get; set; } = string.Empty;

        [JsonPropertyName("dAlt")]
        public string DataAlteracao { get; set; } = string.Empty;

        [JsonPropertyName("dInc")]
        public string DataInclusao { get; set; } = string.Empty;

        [JsonPropertyName("hAlt")]
        public string HoraAlteracao { get; set; } = string.Empty;

        [JsonPropertyName("hInc")]
        public string HoraInclusao { get; set; } = string.Empty;

        [JsonPropertyName("uAlt")]
        public string UsuarioAlteracao { get; set; } = string.Empty;

        [JsonPropertyName("uInc")]
        public string UsuarioInclusao { get; set; } = string.Empty;
    }
}
