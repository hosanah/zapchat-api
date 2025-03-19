using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Zapchat.Domain.DTOs.ContasPagar;

namespace Zapchat.Domain.DTOs.ContaAzul
{
    public class ListarContaAzulDto
    {
        [JsonPropertyName("totalItems")]
        public int TotalItens { get; set; }

        [JsonPropertyName("items")]
        public List<ListarInadiplentesContaAzulDto> InadiplentesItens { get; set; }
    }
}
