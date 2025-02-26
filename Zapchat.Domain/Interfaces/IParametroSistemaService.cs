using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapchat.Domain.DTOs.ContasPagar;
using Zapchat.Domain.Entities;

namespace Zapchat.Domain.Interfaces
{
    public interface IParametroSistemaService
    {
        Task<ParamGrupoWhatsApp> BuscarParammetroPorGrupoIdentificador(string grupoIdentificador);
    }
}
