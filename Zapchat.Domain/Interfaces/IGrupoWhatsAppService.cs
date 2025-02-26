using Zapchat.Domain.DTOs;
using Zapchat.Domain.Entities;

namespace Zapchat.Domain.Interfaces
{
    public interface IGrupoWhatsAppService
    {
        Task<IEnumerable<GrupoWhatsAppDto>> GetAllAsync();
        Task<GrupoWhatsAppDto?> GetByIdAsync(Guid id);
        Task<GrupoWhatsAppDto> AddAsync(GrupoWhatsAppDto usuarioDto);
        Task<GrupoWhatsAppDto?> UpdateAsync(Guid id, GrupoWhatsAppDto usuarioDto);
        Task<bool> DeleteAsync(Guid id);
        Task<AutoConfigurarGrupoDto> AutoConfigurarGrupo(AutoConfigurarGrupoDto usuarioDto);
        Task<IEnumerable<AutoConfigurarGrupoDto>> BuscarTodasConfigurações();
        Task<GrupoWhatsApp?> GetByIdentificadorAsync(string identificador);
    }
}
