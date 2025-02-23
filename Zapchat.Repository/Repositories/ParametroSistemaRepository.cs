using Zapchat.Domain.Entities;
using Zapchat.Domain.Interfaces;
using Zapchat.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Zapchat.Domain.DTOs;

namespace Zapchat.Repository.Repositories
{
    public class ParametroSistemaRepository : IParametroSistemaRepository
    {
        private readonly AppDbContext _context;

        public ParametroSistemaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ParamGrupoWhatsApp>> GetAllAsync() =>
            await _context.ParamsGrupoWhatsApp.ToListAsync();

        public async Task<ParamGrupoWhatsApp?> GetByIdAsync(Guid id) =>
            await _context.ParamsGrupoWhatsApp.FindAsync(id);

        public async Task AddAsync(ParamGrupoWhatsApp grupo)
        {
            await _context.ParamsGrupoWhatsApp.AddAsync(grupo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ParamGrupoWhatsApp adm)
        {
            _context.ParamsGrupoWhatsApp.Update(adm);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var grupo = await GetByIdAsync(id);
            if (grupo != null)
            {
                _context.ParamsGrupoWhatsApp.Remove(grupo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ParamGrupoWhatsApp> GetByGrupoIdAsync(Guid grupoId)
        {
            return await _context.ParamsGrupoWhatsApp
            .FirstAsync(param => param.GrupoId == grupoId);
        }

        public async Task DeleteByGrupoIdAsync(Guid id)
        {
            var param = await GetByGrupoIdAsync(id);
            if (param != null)
            {
                _context.ParamsGrupoWhatsApp.Remove(param);
                await _context.SaveChangesAsync();
            }
        }
    }
}
