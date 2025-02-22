using System;

namespace Zapchat.Domain.Entities
{
    public class AdmGrupoWhatsApp : Entity
    {
        public Guid GrupoId { get; set; }
        public string NumeroAdm { get; set; } = string.Empty;

        public AdmGrupoWhatsApp()
        {
            
        }
        public AdmGrupoWhatsApp(string numeroAdm)
        {
            Id = new Guid();
            NumeroAdm = numeroAdm;
        }
    }
}
