using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flujo
{
    public class ProveedorFlujo : IProveedorFlujo
    {
        private readonly IProveedorDA _proveedorDA;

        public ProveedorFlujo(IProveedorDA proveedorDA)
        {
            _proveedorDA = proveedorDA;
        }

        public Task<List<ProveedorDto>> ObtenerTodosAsync(bool activos)
            => _proveedorDA.ListarPorEstadoAsync(activos);

        public Task<ProveedorDto?> ObtenerPorIdAsync(int id)
            => _proveedorDA.ObtenerPorIdAsync(id);

        public Task<int> AgregarAsync(ProveedorDto dto)
            => _proveedorDA.InsertarAsync(dto);

        public Task<int> EditarAsync(ProveedorDto dto)
            => _proveedorDA.ActualizarAsync(dto);

        public Task<int> CambiarEstadoAsync(int id, bool activo)
            => _proveedorDA.CambiarEstadoAsync(id, activo);
    }
}
