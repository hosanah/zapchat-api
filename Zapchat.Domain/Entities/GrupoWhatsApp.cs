using System;
using Zapchat.Domain.DTOs;
namespace Zapchat.Domain.Entities
{
    public class GrupoWhatsApp : Entity
    {
        public string Nome { get; set; } = string.Empty;
        public string Identificador { get; set; } = string.Empty;
        public string? Descricao { get; set; }

        public GrupoWhatsApp()
        {
            
        }
        public GrupoWhatsApp(GrupoWhatsAppDto grupoWhatsAppDto)
        {
            Nome = grupoWhatsAppDto.Nome;
            Identificador = grupoWhatsAppDto.Identificador;
            Descricao = grupoWhatsAppDto?.Descricao;
        }

        public GrupoWhatsApp(AutoConfigurarGrupoDto grupoWhatsAppDto)
        {
            Id = new Guid();
            Nome = grupoWhatsAppDto.GrupoNome;
            Identificador = grupoWhatsAppDto.GrupoIdentificador;
        }
    }

}
