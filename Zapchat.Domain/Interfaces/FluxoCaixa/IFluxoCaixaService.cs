using System.Threading.Tasks;
using Zapchat.Domain.DTOs.FluxoCaixa;

namespace Zapchat.Domain.Interfaces.FluxoCaixa
{
    public interface IFluxoCaixaService
    {
        /// <summary>
        /// Consulta a API Omie (ListarResumoContasCorrentes + ListarExtrato para cada conta)
        /// e retorna o arquivo Excel em Base64.
        /// </summary>
        Task<string> ListarFluxoCaixaExcel(ListarFluxoCaixaDto listarFluxoCaixaDto);
    }
}
