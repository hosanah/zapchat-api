using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapchat.Domain.DTOs;
using Zapchat.Domain.Entities;

namespace Zapchat.Domain.Interfaces
{
    public interface IParametroSistemaRepository
    {
        Task<IEnumerable<ParamGrupoWhatsApp>> GetAllAsync();
        Task<ParamGrupoWhatsApp?> GetByIdAsync(Guid id);
        Task AddAsync(ParamGrupoWhatsApp param);
        Task UpdateAsync(ParamGrupoWhatsApp param);
        Task DeleteAsync(Guid id);
        Task<ParamGrupoWhatsApp> GetByGrupoIdAsync(Guid grupoId);
    }
}
