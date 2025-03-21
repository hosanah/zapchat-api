﻿using System;
using Zapchat.Domain.DTOs;
namespace Zapchat.Domain.Entities
{
    public class GrupoWhatsApp : Entity
    {
        public string Nome { get; set; } = string.Empty;
        public string Identificador { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public TipoPlataforma Plataforma { get; set; }

        public GrupoWhatsApp()
        {
            
        }

        public GrupoWhatsApp(AutoConfigurarGrupoDto grupoWhatsAppDto, TipoPlataforma plataforma)
        {
            Id = new Guid();
            Nome = grupoWhatsAppDto.GrupoNome;
            Identificador = grupoWhatsAppDto.GrupoIdentificador;
            Plataforma = plataforma;
        }
    }

}
