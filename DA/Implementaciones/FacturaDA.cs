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
                    Estado = "Emitida"
                };

                _db.Facturas.Add(factura);
                await _db.SaveChangesAsync();

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
                    if (producto != null)
                    {
                        producto.Stock -= det.Cantidad;
                        _db.Productos.Update(producto);
                    }
                }

                pedido.Estado = "ENTREGADA";
                _db.PedidoVentas.Update(pedido);

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

