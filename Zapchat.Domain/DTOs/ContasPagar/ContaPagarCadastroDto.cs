using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Zapchat.Domain.DTOs.ContasPagar
{
    public class ContaPagarCadastroDto
    {
        [JsonPropertyName("baixa_bloqueada")]
        public string? BaixaBloqueada { get; set; }

        [JsonPropertyName("bloqueado")]
        public string? Bloqueado { get; set; }

        [JsonPropertyName("codigo_categoria")]
        public string? CodigoCategoria { get; set; }

        [JsonPropertyName("codigo_cliente_fornecedor")]
        public long CodigoClienteFornecedor { get; set; }

        [JsonPropertyName("codigo_lancamento_integracao")]
        public string? CodigoLancamentoIntegracao { get; set; }

        [JsonPropertyName("codigo_lancamento_omie")]
        public long CodigoLancamentoOmie { get; set; }

        [JsonPropertyName("codigo_tipo_documento")]
        public string? CodigoTipoDocumento { get; set; }

        [JsonPropertyName("data_emissao")]
        public string DataEmissao { get; set; } = string.Empty;

        [JsonPropertyName("data_entrada")]
        public string DataEntrada { get; set; } = string.Empty;

        [JsonPropertyName("data_previsao")]
        public string DataPrevisao { get; set; } = string.Empty;

        [JsonPropertyName("data_vencimento")]
        public string DataVencimento { get; set; } = string.Empty;

        [JsonPropertyName("distribuicao")]
        public List<object> Distribuicao { get; set; }

        [JsonPropertyName("id_conta_corrente")]
        public long IdContaCorrente { get; set; }

        [JsonPropertyName("id_origem")]
        public string? IdOrigem { get; set; }

        [JsonPropertyName("numero_documento")]
        public string? NumeroDocumento { get; set; }

        [JsonPropertyName("numero_documento_fiscal")]
        public string? NumeroDocumentoFiscal { get; set; }

        [JsonPropertyName("numero_parcela")]
        public string? NumeroParcela { get; set; }

        [JsonPropertyName("retem_cofins")]
        public string? RetemCofins { get; set; }

        [JsonPropertyName("retem_csll")]
        public string? RetemCsll { get; set; }

        [JsonPropertyName("retem_inss")]
        public string? RetemInss { get; set; }

        [JsonPropertyName("retem_ir")]
        public string? RetemIr { get; set; }

        [JsonPropertyName("retem_iss")]
        public string? RetemIss { get; set; }

        [JsonPropertyName("retem_pis")]
        public string? RetemPis { get; set; }

        [JsonPropertyName("status_titulo")]
        public string? StatusTitulo { get; set; }

        [JsonPropertyName("valor_documento")]
        public decimal ValorDocumento { get; set; }
    }
}