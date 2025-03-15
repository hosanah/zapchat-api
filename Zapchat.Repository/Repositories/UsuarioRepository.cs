using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using Zapchat.Domain.Entities;
using Zapchat.Domain.Interfaces;
using Zapchat.Repository.Data;

namespace Zapchat.Repository.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync() => await _context.Usuarios.ToListAsync();

        public async Task<Usuario?> GetByIdAsync(int id) => await _context.Usuarios.FindAsync(id);

        public async Task AddAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Usuario>> Search(Expression<Func<Usuario, bool>> predicate)
        {
            return await _context.Usuarios.Where(predicate).ToListAsync();
        }
    }
}
