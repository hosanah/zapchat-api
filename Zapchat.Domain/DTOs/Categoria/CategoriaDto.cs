using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Zapchat.Domain.DTOs.Categoria
{
    public class CategoriaDto
    {
        [JsonPropertyName("codigo_categoria")]
        public string CodigoCategoria { get; set; } = string.Empty;

        [JsonPropertyName("percentual")]
        public decimal Percentual { get; set; }

        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }
    }
}
