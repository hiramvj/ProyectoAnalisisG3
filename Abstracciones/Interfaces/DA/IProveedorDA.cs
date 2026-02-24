using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.DA
{
    public interface IProveedorDA
    {
        Task<List<ProveedorDto>> ListarPorEstadoAsync(bool activo);
        Task<ProveedorDto?> ObtenerPorIdAsync(int proveedorId);
        Task<int> InsertarAsync(ProveedorDto proveedor);
        Task<int> ActualizarAsync(ProveedorDto proveedor);
        Task<int> CambiarEstadoAsync(int proveedorId, bool activo);
    }
}
