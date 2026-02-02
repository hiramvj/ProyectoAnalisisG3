using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.DA
{
    public interface IProductoDA
    {
        Task<List<ProductoDto>> ListarPorEstadoAsync(bool activo);
        Task<ProductoDto?> ObtenerPorIdAsync(int productoId);

        Task<int> InsertarAsync(ProductoDto producto);
        Task<int> ActualizarAsync(ProductoDto producto);

        Task<int> CambiarEstadoAsync(int productoId, bool activo);
    }
}
