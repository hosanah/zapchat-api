using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapchat.Domain.DTOs.Categoria;

namespace Zapchat.Domain.Interfaces.Categoria
{
    public interface ICategoriaService
    {
        Task<DadosCategoriaDto> ListarDadosCategoriaPorCod(string codCategoria);
    }
}
