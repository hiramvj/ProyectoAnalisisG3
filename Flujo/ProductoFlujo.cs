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
    public class ProductoFlujo : IProductoFlujo
    {
        private readonly IProductoDA _productoDA;

        public ProductoFlujo(IProductoDA productoDA)
        {
            _productoDA = productoDA;
        }

        public async Task<List<ProductoDto>> ObtenerTodosAsync(bool soloActivos)
        {
            // Si soloActivos = true -> Activos, si false -> Inactivos
            return await _productoDA.ListarPorEstadoAsync(soloActivos);
        }

        public Task<ProductoDto?> ObtenerPorIdAsync(int productoId)
        {
            return _productoDA.ObtenerPorIdAsync(productoId);
        }

        public async Task<int> AgregarAsync(ProductoDto producto)
        {
            ValidarProducto(producto, esNuevo: true);
            return await _productoDA.InsertarAsync(producto);
        }

        public async Task<bool> EditarAsync(ProductoDto producto)
        {
            if (producto.ProductoId <= 0)
                throw new Exception("ProductoId inválido para editar.");

            ValidarProducto(producto, esNuevo: false);

            var filas = await _productoDA.ActualizarAsync(producto);
            return filas > 0;
        }

        public async Task<bool> CambiarEstadoAsync(int productoId, bool activo)
        {
            if (productoId <= 0) throw new Exception("ProductoId inválido.");

            var filas = await _productoDA.CambiarEstadoAsync(productoId, activo);
            return filas > 0;
        }

        private static void ValidarProducto(ProductoDto producto, bool esNuevo)
        {
            if (string.IsNullOrWhiteSpace(producto.SKU))
                throw new Exception("SKU es requerido.");

            if (string.IsNullOrWhiteSpace(producto.Nombre))
                throw new Exception("Nombre es requerido.");

            if (producto.SKU.Length > 40)
                throw new Exception("SKU supera el máximo permitido (40).");

            if (producto.Nombre.Length > 150)
                throw new Exception("Nombre supera el máximo permitido (150).");

            if (producto.Costo < 0) throw new Exception("Costo no puede ser negativo.");
            if (producto.Precio < 0) throw new Exception("Precio no puede ser negativo.");
            if (producto.StockMinimo < 0) throw new Exception("Stock mínimo no puede ser negativo.");

            if (producto.UnidadMedidaId <= 0)
                throw new Exception("UnidadMedidaId inválido.");
        }
    }
}