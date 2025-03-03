using System;
using Zapchat.Domain.Entities;

namespace Zapchat.Domain.DTOs
{
    public class GrupoWhatsAppDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Identificador { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public TipoPlataforma Plataforma { get; set; }
    }
}
