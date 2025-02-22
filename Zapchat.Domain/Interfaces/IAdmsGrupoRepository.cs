using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapchat.Domain.DTOs;
using Zapchat.Domain.Entities;

namespace Zapchat.Domain.Interfaces
{
    public interface IAdmsGrupoRepository
    {
        Task<IEnumerable<AdmGrupoWhatsApp>> GetAllAsync();
        Task<AdmGrupoWhatsApp?> GetByIdAsync(Guid id);
        Task AddAsync(AdmGrupoWhatsApp param);
        Task UpdateAsync(AdmGrupoWhatsApp param);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<AdmGrupoWhatsApp>> GetByGrupoIdAsync(Guid grupoId);
    }
}
