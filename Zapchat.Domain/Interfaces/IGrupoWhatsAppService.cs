using Zapchat.Domain.DTOs;

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
    }
}
