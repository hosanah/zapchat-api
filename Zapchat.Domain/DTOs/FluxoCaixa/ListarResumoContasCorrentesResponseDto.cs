using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Zapchat.Domain.DTOs.FluxoCaixa
{
    public class ListarResumoContasCorrentesResponseDto
    {
        [JsonPropertyName("pagina")]
        public string Pagina { get; set; } = string.Empty;

        [JsonPropertyName("total_de_paginas")]
        public int Total_de_paginas { get; set; }

        [JsonPropertyName("registros")]
        public int Registros { get; set; }

        [JsonPropertyName("total_de_registros")]
        public int Total_de_registros { get; set; }

        [JsonPropertyName("conta_corrente_lista")]
        public List<ContaCorrenteDto> Conta_corrente_lista { get; set; }
    }

    public class ContaCorrenteDto
    {

        [JsonPropertyName("cCategoria")]
        public string CCategoria { get; set; } = string.Empty;

        [JsonPropertyName("codigo_banco")]
        public string Codigo_banco { get; set; } = string.Empty;

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [JsonPropertyName("nCodCC")]
        public long NCodCC { get; set; }

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; } = string.Empty;

        [JsonPropertyName("codigo_agencia")]
        public string CodigoAgencia { get; set; } = string.Empty;

        [JsonPropertyName("conta_corrente")]
        public string ContaCorrente { get; set; } = string.Empty;
    }
}
