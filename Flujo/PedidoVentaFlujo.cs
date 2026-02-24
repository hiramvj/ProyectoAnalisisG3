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
    public class PedidoVentaFlujo : IPedidoVentaFlujo
    {
        private readonly IPedidoVentaDA _pedidoDA;
        private readonly IProductoDA _productoDA;

        public PedidoVentaFlujo(IPedidoVentaDA pedidoDA, IProductoDA productoDA)
        {
            _pedidoDA = pedidoDA;
            _productoDA = productoDA;
        }

        public async Task<int> CrearPedidoAsync(PedidoVentaCrearDto dto)
        {
            // ✅ 0) Limpiar líneas vacías (si el usuario dejó una línea sin producto)
            dto.Lineas = dto.Lineas?
                .Where(l => l.ProductoId > 0 && l.Cantidad > 0)
                .ToList()
                ?? new List<PedidoVentaLineaDto>();

            // ✅ 1) Validaciones
            if (dto.ClienteId <= 0) throw new Exception("Cliente inválido.");
            if (dto.Lineas.Count == 0) throw new Exception("Agregá al menos un producto.");

            
            // if (dto.Lineas.Any(l => l.Cantidad <= 0)) throw new Exception("Cantidad inválida.");

            // 2) Crear encabezado
            var pedidoId = await _pedidoDA.CrearPedidoAsync(dto.ClienteId, dto.Observaciones, dto.MetodoPagoId);

            // 3) Agregar líneas
            foreach (var l in dto.Lineas)
            {
                var producto = await _productoDA.ObtenerPorIdAsync(l.ProductoId);
                if (producto == null) throw new Exception($"ProductoId {l.ProductoId} no existe.");
                if (!producto.Activo) throw new Exception($"El producto {producto.Nombre} está inactivo.");
                if (producto.Stock < l.Cantidad) throw new Exception($"Stock insuficiente para {producto.Nombre}.");

                await _pedidoDA.AgregarLineaAsync(pedidoId, l.ProductoId, l.Cantidad, producto.Precio);
            }

            return pedidoId;
        }
    }
}