using Zapchat.Domain.DTOs.ContasReceber;

namespace Zapchat.Domain.Interfaces.ContasReceber
{
    public interface IContasReceberService
    {
        Task<string> ListarContasReceberExcel(ListarContasReceberExcelDto listarContasReceberExcelDto);
    }
}
