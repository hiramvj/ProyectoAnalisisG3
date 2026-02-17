using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IProductoFlujo
    {
        Task<List<ProductoDto>> ObtenerTodosAsync(bool soloActivos);
        Task<ProductoDto?> ObtenerPorIdAsync(int productoId);

        Task<int> AgregarAsync(ProductoDto producto);
        Task<bool> EditarAsync(ProductoDto producto);

        Task<bool> CambiarEstadoAsync(int productoId, bool activo);
    }
}
