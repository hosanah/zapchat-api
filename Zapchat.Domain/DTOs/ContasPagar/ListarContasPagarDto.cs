using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Zapchat.Domain.DTOs.ContasPagar
{
    public class ListarContasPagarDto
    {
        [JsonPropertyName("pagina")]
        public int Pagina { get; set; }

        [JsonPropertyName("total_de_paginas")]
        public int TotalDePaginas { get; set; }

        [JsonPropertyName("registros")]
        public int Registros { get; set; }

        [JsonPropertyName("total_de_registros")]
        public int TotalDeRegistros { get; set; }

        [JsonPropertyName("conta_pagar_cadastro")]
        public List<ContaPagarCadastroDto> ContaPagarCadastro { get; set; }
    }
}
