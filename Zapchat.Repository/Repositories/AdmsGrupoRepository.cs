using Zapchat.Domain.Entities;
using Zapchat.Domain.Interfaces;
using Zapchat.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Zapchat.Domain.DTOs;
using System.Runtime.Intrinsics.Arm;

namespace Zapchat.Repository.Repositories
{
    public class AdmsGrupoRepository : IAdmsGrupoRepository
    {
        private readonly AppDbContext _context;

        public AdmsGrupoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdmGrupoWhatsApp>> GetAllAsync() =>
            await _context.AdmsGrupoWhatsApp.ToListAsync();

        public async Task<AdmGrupoWhatsApp?> GetByIdAsync(Guid id) =>
            await _context.AdmsGrupoWhatsApp.FindAsync(id);

        public async Task AddAsync(AdmGrupoWhatsApp adm)
        {
            await _context.AdmsGrupoWhatsApp.AddAsync(adm);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AdmGrupoWhatsApp adm)
        {
            _context.AdmsGrupoWhatsApp.Update(adm);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var adm = await GetByIdAsync(id);
            if (adm != null)
            {
                _context.AdmsGrupoWhatsApp.Remove(adm);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<AdmGrupoWhatsApp>> GetByGrupoIdAsync(Guid grupoId) =>
        await _context.AdmsGrupoWhatsApp
        .Where(adm => adm.GrupoId == grupoId)
        .ToListAsync();

    }
}
