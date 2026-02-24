using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.DA
{
    public interface IFacturaDA
    {
        Task<List<FacturaDto>> ListarAsync(string? estado, DateTime? fechaEmision);
        Task<FacturaDto?> ObtenerPorIdAsync(int facturaId);
        Task<int> InsertarAsync(FacturaDto factura);
        Task<int> ActualizarAsync(FacturaDto factura);
        Task<int> CambiarEstadoAsync(int facturaId, string nuevoEstado);
    }
}