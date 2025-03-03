using System;

namespace Zapchat.Domain.DTOs.FluxoCaixa
{
    /// <summary>
    /// DTO de entrada para listar fluxo de caixa.
    /// Você pode ajustar as datas se preferir.
    /// </summary>
    public class ListarFluxoCaixaDto
    {
        public string GrupoIdentificador { get; set; } = string.Empty;
    }
}
