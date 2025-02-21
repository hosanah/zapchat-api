using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Zapchat.Domain.DTOs.Categoria
{
    public class DadosDreDto
    {
        [JsonPropertyName("codigoDRE")]
        public string CodigoDre { get; set; } = string.Empty;

        [JsonPropertyName("descricaoDRE")]
        public string DescricaoDre { get; set; } = string.Empty;

        [JsonPropertyName("naoExibirDRE")]
        public string NaoExibirDre { get; set; } = string.Empty;

        [JsonPropertyName("nivelDRE")]
        public int NivelDre { get; set; }

        [JsonPropertyName("sinalDRE")]
        public string SinalDre { get; set; } = string.Empty;

        [JsonPropertyName("totalizaDRE")]
        public string TotalizaDre { get; set; } = string.Empty;
    }
}
