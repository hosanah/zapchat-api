using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Zapchat.Domain.DTOs.Categoria
{
    public class DadosCategoriaDto
    {
        [JsonPropertyName("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [JsonPropertyName("descricao_padrao")]
        public string DescricaoPadrao { get; set; } = string.Empty;

        [JsonPropertyName("tipo_categoria")]
        public string TipoCategoria { get; set; } = string.Empty;

        [JsonPropertyName("conta_inativa")]
        public string ContaInativa { get; set; } = string.Empty;

        [JsonPropertyName("definida_pelo_usuario")]
        public string DefinidaPeloUsuario { get; set; } = string.Empty;

        [JsonPropertyName("id_conta_contabil")]
        public string IdContaContabil { get; set; } = string.Empty;

        [JsonPropertyName("tag_conta_contabil")]
        public string TagContaContabil { get; set; } = string.Empty;

        [JsonPropertyName("conta_despesa")]
        public string ContaDespesa { get; set; } = string.Empty;

        [JsonPropertyName("conta_receita")]
        public string ContaReceita { get; set; } = string.Empty;

        [JsonPropertyName("nao_exibir")]
        public string NaoExibir { get; set; } = string.Empty;

        [JsonPropertyName("natureza")]
        public string Natureza { get; set; } = string.Empty;

        [JsonPropertyName("totalizadora")]
        public string Totalizadora { get; set; } = string.Empty;

        [JsonPropertyName("transferencia")]
        public string Transferencia { get; set; } = string.Empty;

        [JsonPropertyName("codigo_dre")]
        public string CodigoDre { get; set; } = string.Empty;

        [JsonPropertyName("categoria_superior")]
        public string CategoriaSuperior { get; set; } = string.Empty;

        [JsonPropertyName("dadosDRE")]
        public DadosDreDto? DadosDre { get; set; }
    }
}
