using Zapchat.Domain.Entities;
using Zapchat.Domain.Interfaces;
using Zapchat.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace Zapchat.Repository.Repositories
{
    public class GrupoWhatsAppRepository : IGrupoWhatsAppRepository
    {
        private readonly AppDbContext _context;

        public GrupoWhatsAppRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GrupoWhatsApp>> GetAllAsync() =>
            await _context.GruposWhatsApp.ToListAsync();

        public async Task<GrupoWhatsApp?> GetByIdAsync(Guid id) =>
            await _context.GruposWhatsApp.FindAsync(id);

        public async Task AddAsync(GrupoWhatsApp grupo)
        {
            await _context.GruposWhatsApp.AddAsync(grupo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GrupoWhatsApp grupo)
        {
            _context.GruposWhatsApp.Update(grupo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var grupo = await GetByIdAsync(id);
            if (grupo != null)
            {
                _context.GruposWhatsApp.Remove(grupo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<GrupoWhatsApp?> GetByIdentificadorAsync(string identificador) =>
            await _context.GruposWhatsApp.FirstOrDefaultAsync(param => param.Identificador == identificador);
    }
}
