using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SecureIdentity.Password;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Zapchat.Domain.DTOs;
using Zapchat.Domain.Entities;
using Zapchat.Domain.Interfaces;
using Zapchat.Domain.Interfaces.Messages;

namespace Zapchat.Service.Services
{
    public sealed class AuthService : BaseService, IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUsuarioRepository _usuarioRepository;
        public AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration, INotificator notificator) : base(notificator)
        {
            _configuration = configuration;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<dynamic> LoginUserAutenticate(LoginUserRequestDto userRequest)
        {
            try
            {
                var tempoSessao = _configuration.GetSection("Auth").GetValue<int>("tempoSessao");
                var secretEncoded = _configuration.GetSection("Auth").GetValue<string>("secret")!;
                byte[] base64Bytes = Convert.FromBase64String(secretEncoded);
                string decodedSecrectc = Encoding.UTF8.GetString(base64Bytes);

                var usuario = await _usuarioRepository.Search(e => e.Nome!.Equals(userRequest.Login));

                if (!usuario.Any())
                {
                    Notify("Usuário ou senha inválidos");
                    return false!;
                }


                var usuarioAutenticado = usuario.First();

                if (!PasswordHasher.Verify(usuarioAutenticado.Senha, userRequest.Senha))
                {
                    Notify("Usuário ou senha inválidos");
                    return false!;
                }

                try
                {
                    var token = GenerateToken(usuarioAutenticado, decodedSecrectc);

                    return new
                    {
                        Usuario = usuarioAutenticado,
                        Token = token,
                        tempoDeSessao = tempoSessao
                    };

                }
                catch
                {
                    Notify("01200X - Falha interna no servidor");
                    return false!;
                }
            }
            catch (Exception ex)
            {
                Notify(ex.Message);
                return false!;
            }

        }

        protected string GenerateToken(Usuario usuario, string secret)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, usuario.Id.ToString()),
                    new(ClaimTypes.GivenName, usuario.Nome),
                    new(ClaimTypes.Email, usuario.Email)
                }),

                    Expires = DateTime.UtcNow.AddHours(6),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        protected static IEnumerable<Claim> GetClaims(Usuario usuario)
        {
            var result = new List<Claim>
            {
            new(ClaimTypes.Name,usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.NameIdentifier, usuario.Nome)
            };

            return result;
        }

        public string RefreshToken(Usuario usuario)
        {
            return "'test'";
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            //_authRepository?.Dispose();
        }

    }

}
