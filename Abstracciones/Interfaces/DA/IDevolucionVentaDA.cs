using Abstracciones.Modelos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.DA
{
    public interface IDevolucionVentaDA
    {
        Task<int> ProcesarDevolucionAsync(int facturaId, List<DevolucionLineaDto> lineas, string motivo);
        Task<DevolucionResultadoDto?> ObtenerPorIdAsync(int devolucionVentaId);
        Task<List<DevolucionListadoDto>> ObtenerTodosAsync();
        Task<List<DevolucionLineaDto>> ObtenerLineasFacturaAsync(int facturaId);
    }
}
