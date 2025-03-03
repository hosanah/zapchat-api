using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapchat.Domain.Entities
{
    public enum TipoPlataforma
    {
        [Display(Name = "Plataforma Omie")]
        Omie = 10,
        [Display(Name = "Plataforma Conta Azul")]
        ContaAzul = 20
    }
}
