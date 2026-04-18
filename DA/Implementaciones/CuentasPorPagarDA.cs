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
    public class CuentasPorPagarDA : ICuentasPorPagarDA
    {
        private readonly AppDbContext _db;

        public CuentasPorPagarDA(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<CuentaPorPagarDto>> ListarCuentasAsync()
        {
            var cuentasDto = new List<CuentaPorPagarDto>();

            var query = from c in _db.CuentasPorPagar
                        join p in _db.Proveedores on c.ProveedorId equals p.ProveedorId
                        orderby c.FechaVencimiento ascending
                        select new { c, NombreLegal = p.NombreLegal };

            var cuentas = await query.ToListAsync();

            foreach (var item in cuentas)
            {
                cuentasDto.Add(new CuentaPorPagarDto
                {
                    CuentaPorPagarId = item.c.CuentaPorPagarId,
                    ProveedorId = item.c.ProveedorId,
                    ProveedorNombre = item.NombreLegal,
                    NumeroFactura = item.c.NumeroFactura,
                    FechaEmision = item.c.FechaEmision,
                    FechaVencimiento = item.c.FechaVencimiento,
                    MontoOriginal = item.c.MontoOriginal,
                    SaldoPendiente = item.c.SaldoPendiente,
                    Estado = item.c.Estado
                });
            }

            return cuentasDto;
        }

        public async Task<CuentaPorPagarDto> ObtenerCuentaAsync(int cuentaPorPagarId)
        {
            var item = await (from c in _db.CuentasPorPagar
                              join p in _db.Proveedores on c.ProveedorId equals p.ProveedorId
                              where c.CuentaPorPagarId == cuentaPorPagarId
                              select new { c, p.NombreLegal }).FirstOrDefaultAsync();

            if (item == null) return null;

            return new CuentaPorPagarDto
            {
                CuentaPorPagarId = item.c.CuentaPorPagarId,
                ProveedorId = item.c.ProveedorId,
                ProveedorNombre = item.NombreLegal,
                NumeroFactura = item.c.NumeroFactura,
                FechaEmision = item.c.FechaEmision,
                FechaVencimiento = item.c.FechaVencimiento,
                MontoOriginal = item.c.MontoOriginal,
                SaldoPendiente = item.c.SaldoPendiente,
                Estado = item.c.Estado
            };
        }

        public async Task<int> CrearCuentaAsync(CuentaPorPagarDto dto)
        {
            var factura = await _db.Facturas
                .FirstOrDefaultAsync(f => f.FacturaId == dto.FacturaId);

            if (factura == null)
                throw new Exception("La factura seleccionada no existe.");

            var nuevaCuenta = new CuentaPorPagar
            {
                ProveedorId = dto.ProveedorId,

                NumeroFactura = factura.NumeroFactura.ToString(),

                FechaEmision = DateTime.SpecifyKind(factura.FechaEmision, DateTimeKind.Utc),
                FechaVencimiento = DateTime.SpecifyKind(dto.FechaVencimiento, DateTimeKind.Utc),

                MontoOriginal = factura.Total,
                SaldoPendiente = factura.Total,

                Estado = "PENDIENTE"
            };

            await _db.CuentasPorPagar.AddAsync(nuevaCuenta);
            await _db.SaveChangesAsync();

            return nuevaCuenta.CuentaPorPagarId;
        }

        public async Task<IEnumerable<PagoProveedorDto>> ListarPagosPorCuentaAsync(int cuentaPorPagarId)
        {
            return await _db.PagosProveedor
                .Where(p => p.CuentaPorPagarId == cuentaPorPagarId)
                .OrderByDescending(p => p.FechaPago)
                .Select(p => new PagoProveedorDto
                {
                    PagoProveedorId = p.PagoProveedorId,
                    CuentaPorPagarId = p.CuentaPorPagarId,
                    FechaPago = p.FechaPago,
                    Monto = p.Monto,
                    MetodoPago = p.MetodoPago,
                    TipoTransaccion = p.TipoTransaccion,
                    Estado = p.Estado,
                    Notas = p.Notas
                }).ToListAsync();
        }

        public async Task<int> RegistrarPagoAsync(PagoProveedorDto dto)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var cuenta = await _db.CuentasPorPagar.FindAsync(dto.CuentaPorPagarId);
                if (cuenta == null)
                    throw new Exception("La cuenta por pagar no existe.");

                // 🔥 VALIDACIONES PRO
                if (cuenta.SaldoPendiente <= 0)
                    throw new Exception("Esta cuenta ya está pagada.");

                if (dto.Monto <= 0)
                    throw new Exception("El monto debe ser mayor a 0.");

                if (dto.Estado == "COMPLETADO" && dto.Monto > cuenta.SaldoPendiente)
                    throw new Exception($"El monto no puede ser mayor al saldo pendiente (₡{cuenta.SaldoPendiente:N2}).");

                var nuevoPago = new PagoProveedor
                {
                    CuentaPorPagarId = dto.CuentaPorPagarId,
                    FechaPago = DateTime.SpecifyKind(dto.FechaPago, DateTimeKind.Utc), // 🔥 FIX UTC
                    Monto = dto.Monto,
                    MetodoPago = dto.MetodoPago,
                    TipoTransaccion = dto.TipoTransaccion,
                    Estado = dto.Estado,
                    Notas = dto.Notas
                };

                await _db.PagosProveedor.AddAsync(nuevoPago);

                // 🔥 SOLO SI ES COMPLETADO AFECTA SALDO
                if (nuevoPago.Estado == "COMPLETADO")
                {
                    cuenta.SaldoPendiente -= nuevoPago.Monto;

                    if (cuenta.SaldoPendiente <= 0)
                    {
                        cuenta.SaldoPendiente = 0;
                        cuenta.Estado = "PAGADA";
                    }
                    else
                    {
                        cuenta.Estado = "PENDIENTE";
                    }

                    _db.CuentasPorPagar.Update(cuenta);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return nuevoPago.PagoProveedorId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task ActualizarEstadoPagoAsync(int pagoProveedorId, string nuevoEstado)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var pago = await _db.PagosProveedor.FindAsync(pagoProveedorId);
                if (pago == null)
                    throw new Exception("El pago no existe.");

                if (pago.Estado != "COMPLETADO" && nuevoEstado == "COMPLETADO")
                {
                    var cuenta = await _db.CuentasPorPagar.FindAsync(pago.CuentaPorPagarId);
                    if (cuenta != null)
                    {
                        cuenta.SaldoPendiente -= pago.Monto;
                        if (cuenta.SaldoPendiente <= 0)
                        {
                            cuenta.SaldoPendiente = 0;
                            cuenta.Estado = "PAGADA";
                        }
                        _db.CuentasPorPagar.Update(cuenta);
                    }
                }

                pago.Estado = nuevoEstado;
                _db.PagosProveedor.Update(pago);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<FacturaDto>> ListarFacturasAsync()
        {
            return await _db.Facturas
                .Select(f => new FacturaDto
                {
                    FacturaId = f.FacturaId,
                    NumeroFactura = f.NumeroFactura,
                    PedidoVentaId = f.PedidoVentaId,
                    FechaEmision = f.FechaEmision,
                    Subtotal = f.Subtotal,
                    Impuesto = f.Impuesto,
                    Total = f.Total,
                    Estado = f.Estado
                })
                .ToListAsync();
        }
    }
}
