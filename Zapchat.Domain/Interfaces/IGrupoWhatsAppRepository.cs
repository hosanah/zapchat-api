using Zapchat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zapchat.Domain.Interfaces
{
    public interface IGrupoWhatsAppRepository
    {
        Task<IEnumerable<GrupoWhatsApp>> GetAllAsync();
        Task<GrupoWhatsApp?> GetByIdAsync(Guid id);
        Task AddAsync(GrupoWhatsApp grupo);
        Task UpdateAsync(GrupoWhatsApp grupo);
        Task DeleteAsync(Guid id);
        Task<GrupoWhatsApp?> GetByIdentificadorAsync(string identificador);
    }
}
