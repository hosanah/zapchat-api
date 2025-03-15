using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentValidation;
using System.Text.Json;
using System.Text;
using Zapchat.Domain.DTOs;
using Zapchat.Domain.Entities;
using Zapchat.Domain.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using Zapchat.Domain.Notifications;
using Zapchat.Domain.Interfaces.Messages;
using SecureIdentity.Password;

namespace Zapchat.Service.Services
{
    public class UsuarioService : BaseService, IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IValidator<UsuarioDto> _validator;
        private readonly IMapper _mapper;

        public UsuarioService(IUsuarioRepository usuarioRepository, IValidator<UsuarioDto> validator, IMapper mapper, INotificator notificator) : base(notificator)
        {
            _usuarioRepository = usuarioRepository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UsuarioDto>> GetAllAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UsuarioDto>>(usuarios);
        }

        public async Task<UsuarioDto?> GetByIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            return usuario != null ? _mapper.Map<UsuarioDto>(usuario) : null;
        }

        public async Task<UsuarioDto> AddAsync(UsuarioDto usuarioDto)
        {
            var validationResult = await _validator.ValidateAsync(usuarioDto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var verificacaoemail = await _usuarioRepository.Search(a => a.Email.Equals(usuarioDto.Email));
            var verificaNomeUsuario = await _usuarioRepository.Search(b => b.Nome.Equals(usuarioDto.Nome));

            if (verificacaoemail.Any())
            {
                Notify("Já existe um usuario cadastrado com esse e-mail");
                return null;
            }
            if (verificaNomeUsuario.Any())
            {
                Notify("Já existe um usuario cadastrado com esse nome de usuario");
                return null;
            }

            var usuario = new Usuario
            {
                Nome = usuarioDto.Nome,
                Email = usuarioDto.Email,
                DataExpiracao = DateTime.UtcNow.AddMonths(3),
                TrocarSenha = 0,
                Senha = PasswordHasher.Hash(usuarioDto.Senha)
            };

            usuario.Senha = PasswordHasher.Hash(usuarioDto.Senha);

            await _usuarioRepository.AddAsync(usuario);
            return usuarioDto;
        }

        public async Task<UsuarioDto?> UpdateAsync(int id, UsuarioDto usuarioDto)
        {
            var validationResult = await _validator.ValidateAsync(usuarioDto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var usuarioExistente = await _usuarioRepository.GetByIdAsync(id);
            if (usuarioExistente == null) return null;

            var usuarioAtualizado = _mapper.Map(usuarioDto, usuarioExistente);
            await _usuarioRepository.UpdateAsync(usuarioAtualizado);

            return _mapper.Map<UsuarioDto>(usuarioAtualizado);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) return false;

            await _usuarioRepository.DeleteAsync(id);
            return true;
        }

    }
}
