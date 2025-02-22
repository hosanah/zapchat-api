using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapchat.Domain.DTOs
{
    public class AutoConfigurarGrupoDto
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecrect { get; set; } = string.Empty;
        public string GrupoIdentificador { get; set; } = string.Empty;
        public string GrupoNome { get; set; } = string.Empty;
        public List<AdmDto> AdmDto { get; set; } = new List<AdmDto>();
    }

    public class AdmDto
    {
        public string NumeroAdm { get; set; } = string.Empty;
    }
}
