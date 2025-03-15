using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapchat.Domain.Entities
{
    public class Usuario : Entity
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public DateTime DataExpiracao { get; set; }
        public int TrocarSenha { get; set; }
    }
}
