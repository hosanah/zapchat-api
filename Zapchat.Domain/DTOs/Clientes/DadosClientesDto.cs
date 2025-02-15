using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Zapchat.Domain.DTOs.Clientes
{
    public class DadosClientesDto
    {
        [JsonPropertyName("codigo_cliente_omie")]
        public long CodClienteOmie { get; set; }
        [JsonPropertyName("razao_social")]
        public string? RazaoSocial { get; set; }
        [JsonPropertyName("cnpj_cpf")]
        public string? CnpjCpf { get; set; }
        [JsonPropertyName("nome_fantasia")]
        public string? NomeFantasia { get; set; }
    }
}
