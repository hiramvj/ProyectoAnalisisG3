using Abstracciones.Interfaces.DA;
using DA.Contexto;
using DA.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Implementaciones
{
    public class PedidoVentaDA : IPedidoVentaDA
    {
        private readonly AppDbContext _db;
        public PedidoVentaDA(AppDbContext db) => _db = db;

        public async Task<int> CrearPedidoAsync(int clienteId, string? observaciones, int? metodoPagoId)
        {
            var max = await _db.PedidoVentas.MaxAsync(p => (int?)p.NumeroPedido) ?? 0;

            var pedido = new PedidoVenta
            {
                NumeroPedido = max + 1,
                ClienteId = clienteId,
                Estado = "CREADA",
                FechaPedido = DateTime.Now,
                Observaciones = observaciones,
                MetodoPagoId = metodoPagoId
            };

            _db.PedidoVentas.Add(pedido);
            await _db.SaveChangesAsync();
            return pedido.PedidoVentaId;
        }

        public async Task AgregarLineaAsync(int pedidoVentaId, int productoId, int cantidad, decimal precioUnitario)
        {
            var detalle = new PedidoVentaDetalle
            {
                PedidoVentaId = pedidoVentaId,
                ProductoId = productoId,
                Cantidad = cantidad,
                PrecioUnitario = precioUnitario
            };

            _db.PedidoVentaDetalles.Add(detalle);
            await _db.SaveChangesAsync();
        }
    }
}
