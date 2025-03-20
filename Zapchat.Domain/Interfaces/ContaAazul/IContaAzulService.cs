using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapchat.Domain.DTOs.ContaAzul;
using Zapchat.Domain.DTOs.ContasReceber;

namespace Zapchat.Domain.Interfaces.ContaAazul
{
    public interface IContaAzulService
    {
        Task<ListarContaAzulDto> ListarInadiplentePorEmpresa(CapturaInadiplentelDto inadiplentelDto);
        Task<ListarClienteDto> ListarCliente(CapturaClienteDto inadiplentelDto);
        Task<ListarCadastroContaAzulDto> ListarTodosClientes();
    }
}
