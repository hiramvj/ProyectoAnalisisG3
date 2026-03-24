using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using DA.Contexto;
using DA.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DA.Implementaciones
{
    public class DevolucionVentaDA : IDevolucionVentaDA
    {
        private readonly AppDbContext _db;

        public DevolucionVentaDA(AppDbContext db) => _db = db;

        public async Task<List<DevolucionLineaDto>> ObtenerLineasFacturaAsync(int facturaId)
        {
            var factura = await _db.Facturas
                .Include(f => f.Detalles)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FacturaId == facturaId);

            if (factura == null)
                throw new Exception("Factura no encontrada.");

            var lineas = new List<DevolucionLineaDto>();

            foreach (var det in factura.Detalles)
            {
                var producto = await _db.Productos.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ProductoId == det.ProductoId);

                lineas.Add(new DevolucionLineaDto
                {
                    ProductoId = det.ProductoId,
                    ProductoNombre = producto?.Nombre ?? $"Producto #{det.ProductoId}",
                    CantidadFacturada = det.Cantidad,
                    CantidadDevuelta = 0,
                    PrecioUnitario = det.PrecioUnitario
                });
            }

            return lineas;
        }

        public async Task<int> ProcesarDevolucionAsync(int facturaId, List<DevolucionLineaDto> lineas, string motivo)
        {
            var factura = await _db.Facturas
                .Include(f => f.Detalles)
                .FirstOrDefaultAsync(f => f.FacturaId == facturaId);

            if (factura == null)
                throw new Exception("Factura no encontrada.");

            if (factura.Estado == "DEVUELTA")
                throw new Exception("Esta factura ya fue devuelta totalmente.");

            // Filtrar solo líneas con cantidad > 0
            var lineasValidas = lineas.Where(l => l.CantidadDevuelta > 0).ToList();
            if (!lineasValidas.Any())
                throw new Exception("Debe indicar al menos un producto a devolver.");

            // Validar cantidades
            foreach (var linea in lineasValidas)
            {
                var detFactura = factura.Detalles.FirstOrDefault(d => d.ProductoId == linea.ProductoId);
                if (detFactura == null)
                    throw new Exception($"El producto #{linea.ProductoId} no pertenece a esta factura.");

                if (linea.CantidadDevuelta > detFactura.Cantidad)
                    throw new Exception($"La cantidad a devolver ({linea.CantidadDevuelta}) excede la facturada ({detFactura.Cantidad}) para el producto #{linea.ProductoId}.");
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Determinar si es total o parcial
                bool esTotal = true;
                foreach (var det in factura.Detalles)
                {
                    var linea = lineasValidas.FirstOrDefault(l => l.ProductoId == det.ProductoId);
                    if (linea == null || linea.CantidadDevuelta < det.Cantidad)
                    {
                        esTotal = false;
                        break;
                    }
                }

                // Calcular montos
                decimal subtotal = lineasValidas.Sum(l => l.CantidadDevuelta * l.PrecioUnitario);
                decimal impuesto = Math.Round(subtotal * 0.13m, 2);
                decimal total = subtotal + impuesto;

                // Crear devolución
                var devolucion = new DevolucionVenta
                {
                    FacturaId = facturaId,
                    FechaDevolucion = DateTime.UtcNow,
                    Motivo = motivo,
                    Tipo = esTotal ? "TOTAL" : "PARCIAL",
                    Estado = "PROCESADA",
                    MontoTotal = total
                };

                _db.DevolucionesVenta.Add(devolucion);
                await _db.SaveChangesAsync();

                // Crear detalles y reponer stock
                foreach (var linea in lineasValidas)
                {
                    var detalle = new DevolucionVentaDetalle
                    {
                        DevolucionVentaId = devolucion.DevolucionVentaId,
                        ProductoId = linea.ProductoId,
                        CantidadDevuelta = linea.CantidadDevuelta,
                        PrecioUnitario = linea.PrecioUnitario
                    };
                    _db.DevolucionesVentaDetalle.Add(detalle);

                    // Reponer stock
                    var producto = await _db.Productos.FindAsync(linea.ProductoId);
                    if (producto != null)
                    {
                        producto.Stock += linea.CantidadDevuelta;
                    }
                }

                // Crear nota de crédito
                int maxNumero = await _db.NotasCredito.MaxAsync(n => (int?)n.NumeroNotaCredito) ?? 0;

                var notaCredito = new NotaCredito
                {
                    DevolucionVentaId = devolucion.DevolucionVentaId,
                    NumeroNotaCredito = maxNumero + 1,
                    FechaEmision = DateTime.UtcNow,
                    Subtotal = subtotal,
                    Impuesto = impuesto,
                    Total = total,
                    Estado = "EMITIDA"
                };

                _db.NotasCredito.Add(notaCredito);

                // Actualizar estado de factura
                factura.Estado = esTotal ? "DEVUELTA" : "DEVOLUCION_PARCIAL";

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return devolucion.DevolucionVentaId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<DevolucionResultadoDto?> ObtenerPorIdAsync(int devolucionVentaId)
        {
            var dev = await _db.DevolucionesVenta
                .Include(d => d.Detalles)
                .Include(d => d.NotaCredito)
                .Include(d => d.Factura)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DevolucionVentaId == devolucionVentaId);

            if (dev == null) return null;

            var resultado = new DevolucionResultadoDto
            {
                DevolucionVentaId = dev.DevolucionVentaId,
                FacturaId = dev.FacturaId,
                NumeroFactura = dev.Factura?.NumeroFactura ?? 0,
                FechaDevolucion = dev.FechaDevolucion,
                Motivo = dev.Motivo,
                Tipo = dev.Tipo,
                Estado = dev.Estado,
                MontoTotal = dev.MontoTotal,
                NotaCreditoId = dev.NotaCredito?.NotaCreditoId ?? 0,
                NumeroNotaCredito = dev.NotaCredito?.NumeroNotaCredito ?? 0,
                NCSubtotal = dev.NotaCredito?.Subtotal ?? 0,
                NCImpuesto = dev.NotaCredito?.Impuesto ?? 0,
                NCTotal = dev.NotaCredito?.Total ?? 0,
                Lineas = new List<DevolucionLineaDto>()
            };

            foreach (var det in dev.Detalles)
            {
                var producto = await _db.Productos.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ProductoId == det.ProductoId);

                resultado.Lineas.Add(new DevolucionLineaDto
                {
                    ProductoId = det.ProductoId,
                    ProductoNombre = producto?.Nombre ?? $"Producto #{det.ProductoId}",
                    CantidadDevuelta = det.CantidadDevuelta,
                    PrecioUnitario = det.PrecioUnitario
                });
            }

            return resultado;
        }

        public async Task<List<DevolucionListadoDto>> ObtenerTodosAsync()
        {
            return await _db.DevolucionesVenta
                .Include(d => d.Factura)
                .Include(d => d.NotaCredito)
                .AsNoTracking()
                .OrderByDescending(d => d.FechaDevolucion)
                .Select(d => new DevolucionListadoDto
                {
                    DevolucionVentaId = d.DevolucionVentaId,
                    NumeroFactura = d.Factura != null ? d.Factura.NumeroFactura : 0,
                    FechaDevolucion = d.FechaDevolucion,
                    Tipo = d.Tipo,
                    Estado = d.Estado,
                    MontoTotal = d.MontoTotal,
                    NumeroNotaCredito = d.NotaCredito != null ? d.NotaCredito.NumeroNotaCredito : 0
                })
                .ToListAsync();
        }
    }
}
