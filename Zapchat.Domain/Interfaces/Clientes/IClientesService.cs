using Zapchat.Domain.DTOs.Clientes;

namespace Zapchat.Domain.Interfaces.Clientes
{
    public interface IClientesService
    {
        Task<DadosClientesDto> ListarDadosClientesPorCod(string codCliente);
    }
}
