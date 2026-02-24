using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IProveedorFlujo
    {
        Task<List<ProveedorDto>> ObtenerTodosAsync(bool activos);

        Task<ProveedorDto?> ObtenerPorIdAsync(int proveedorId);

        Task<int> AgregarAsync(ProveedorDto proveedor);

        Task<int> EditarAsync(ProveedorDto proveedor);

        Task<int> CambiarEstadoAsync(int proveedorId, bool activo);
    }
}
