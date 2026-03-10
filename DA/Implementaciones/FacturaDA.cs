using Abstracciones.Interfaces.DA;
using DA.Contexto;
using DA.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DA.Implementaciones
{
    public class FacturaDA : IFacturaDA
    {
        private readonly AppDbContext _db;

        public FacturaDA(AppDbContext db) => _db = db;

        public async Task<int> CrearDesdePedidoAsync(int pedidoVentaId)
        {
            if (await _db.Facturas.AnyAsync(f => f.PedidoVentaId == pedidoVentaId))
                throw new Exception("Ese pedido ya fue facturado.");
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var pedido = await _db.PedidoVentas
                    .Include(p => p.Detalles)
                    .FirstOrDefaultAsync(p => p.PedidoVentaId == pedidoVentaId);

                if (pedido == null) throw new Exception("Pedido no encontrado");

                int maxNumero = await _db.Facturas.MaxAsync(f => (int?)f.NumeroFactura) ?? 0;
                var nuevoNumero = maxNumero + 1;

                decimal subtotal = pedido.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
                decimal impuesto = Math.Round(subtotal * 0.13m, 2);
                decimal total = subtotal + impuesto;

                var factura = new Factura
                {
                    NumeroFactura = nuevoNumero,
                    PedidoVentaId = pedidoVentaId,
                    FechaEmision = DateTime.UtcNow,
                    Subtotal = subtotal,
                    Impuesto = impuesto,
                    Total = total,
                    Estado = "EMITIDA"
                };

                _db.Facturas.Add(factura);
                await _db.SaveChangesAsync();

                if (pedido.Detalles == null || !pedido.Detalles.Any())
                    throw new Exception("El pedido no tiene detalles.");

                foreach (var det in pedido.Detalles)
                {
                    var factDet = new FacturaDetalle
                    {
                        FacturaId = factura.FacturaId,
                        ProductoId = det.ProductoId,
                        Cantidad = det.Cantidad,
                        PrecioUnitario = det.PrecioUnitario
                    };
                    _db.FacturaDetalles.Add(factDet);

                    var producto = await _db.Productos.FindAsync(det.ProductoId);
                    if (producto == null)
                        throw new Exception($"No se encontró el producto con id {det.ProductoId}");

                    if (producto.Stock < det.Cantidad)
                        throw new Exception($"Stock insuficiente para el producto {producto.Nombre}");

                    producto.Stock -= det.Cantidad;
                }

                pedido.Estado = "ENTREGADA";
                pedido.FechaPedido = DateTime.SpecifyKind(pedido.FechaPedido, DateTimeKind.Utc);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return factura.FacturaId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

