using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
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
            var max = await _db.PedidoVentas
    .MaxAsync(p => (int?)p.NumeroPedido) ?? 0;

            var pedido = new PedidoVenta
            {
                NumeroPedido = max + 1,
                ClienteId = clienteId,
                Estado = "CREADA",
                FechaPedido = DateTime.UtcNow.Date,
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
        public async Task<List<PedidoVentaListadoDto>> ListarAsync(
    string? q,
    DateTime? desde,
    DateTime? hasta,
    int? clienteId,
    string? estado,
    int? metodoPagoId)
        {
            var pv = _db.PedidoVentas.AsNoTracking().AsQueryable();

            if (clienteId.HasValue && clienteId.Value > 0)
                pv = pv.Where(p => p.ClienteId == clienteId.Value);

            if (!string.IsNullOrWhiteSpace(estado))
                pv = pv.Where(p => p.Estado == estado);

            if (metodoPagoId.HasValue && metodoPagoId.Value > 0)
                pv = pv.Where(p => p.MetodoPagoId == metodoPagoId.Value);

            if (desde.HasValue)
                pv = pv.Where(p => p.FechaPedido >= desde.Value.Date);

            if (hasta.HasValue)
                pv = pv.Where(p => p.FechaPedido <= hasta.Value.Date);

            var query =
                from p in pv
                join c in _db.Clientes.AsNoTracking() on p.ClienteId equals c.ClienteId
                join m in _db.MetodosPago.AsNoTracking()
                    on p.MetodoPagoId equals m.MetodoPagoId into mp
                from m in mp.DefaultIfEmpty()
                select new PedidoVentaListadoDto
                {
                    PedidoVentaId = p.PedidoVentaId,
                    NumeroPedido = p.NumeroPedido,
                    FechaPedido = p.FechaPedido,
                    Estado = p.Estado,
                    ClienteNombre = c.NombreCompleto,
                    MetodoPagoNombre = m != null ? m.Nombre : "-"
                };

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x =>
                    x.ClienteNombre.Contains(q) ||
                    x.Estado.Contains(q) ||
                    x.NumeroPedido.ToString().Contains(q) ||
                    (x.MetodoPagoNombre ?? "").Contains(q));
            }

            return await query
                .OrderByDescending(x => x.FechaPedido)
                .ThenByDescending(x => x.NumeroPedido)
                .ToListAsync();
        }
        public async Task<PedidoVentaDetalleDto?> ObtenerDetalleAsync(int pedidoVentaId)
        {
            var pedido = await _db.PedidoVentas.AsNoTracking()
                .FirstOrDefaultAsync(p => p.PedidoVentaId == pedidoVentaId);

            if (pedido == null) return null;

            var cliente = await _db.Clientes.AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClienteId == pedido.ClienteId);

            var metodo = pedido.MetodoPagoId.HasValue
                ? await _db.MetodosPago.AsNoTracking()
                    .FirstOrDefaultAsync(m => m.MetodoPagoId == pedido.MetodoPagoId.Value)
                : null;

            var lineas = await (
                from d in _db.PedidoVentaDetalles.AsNoTracking()
                join pr in _db.Productos.AsNoTracking() on d.ProductoId equals pr.ProductoId
                where d.PedidoVentaId == pedidoVentaId
                select new PedidoVentaDetalleLineaDto
                {
                    ProductoId = d.ProductoId,
                    ProductoNombre = pr.Nombre,
                    Cantidad = (decimal)d.Cantidad,          // si tu entidad es int, dejalo sin cast
                    PrecioUnitario = d.PrecioUnitario
                }
            ).ToListAsync();

            return new PedidoVentaDetalleDto
            {
                PedidoVentaId = pedido.PedidoVentaId,
                NumeroPedido = pedido.NumeroPedido,
                FechaPedido = pedido.FechaPedido,
                Estado = pedido.Estado,
                ClienteId = pedido.ClienteId,
                ClienteNombre = cliente?.NombreCompleto ?? "",
                MetodoPagoId = pedido.MetodoPagoId,
                MetodoPagoNombre = metodo?.Nombre ?? "-",
                Observaciones = pedido.Observaciones,
                Lineas = lineas
            };
        }

        public async Task<int> ActualizarEncabezadoAsync(PedidoVentaEditarDto dto)
        {
            var pedido = await _db.PedidoVentas.FirstOrDefaultAsync(p => p.PedidoVentaId == dto.PedidoVentaId);
            if (pedido == null) return 0;

            pedido.Estado = dto.Estado;
            pedido.MetodoPagoId = dto.MetodoPagoId;
            pedido.Observaciones = dto.Observaciones;

            return await _db.SaveChangesAsync();
        }
        public async Task<List<PedidoVentaListadoDto>> ObtenerHistorialClienteAsync(int clienteId)
        {
            var query =
                from p in _db.PedidoVentas.AsNoTracking()
                join c in _db.Clientes.AsNoTracking() on p.ClienteId equals c.ClienteId
                join m in _db.MetodosPago.AsNoTracking()
                    on p.MetodoPagoId equals m.MetodoPagoId into mp
                from m in mp.DefaultIfEmpty()
                where p.ClienteId == clienteId
                select new PedidoVentaListadoDto
                {
                    PedidoVentaId = p.PedidoVentaId,
                    NumeroPedido = p.NumeroPedido,
                    FechaPedido = p.FechaPedido,
                    Estado = p.Estado,
                    ClienteNombre = c.NombreCompleto,
                    MetodoPagoNombre = m != null ? m.Nombre : "-"
                };

            return await query
                .OrderByDescending(x => x.FechaPedido)
                .ToListAsync();
        }
        public async Task<List<ProductoMasVendidoDto>> ObtenerProductosMasVendidosAsync()
        {
            var query =
                from d in _db.PedidoVentaDetalles
                join p in _db.Productos on d.ProductoId equals p.ProductoId
                group d by new { p.ProductoId, p.Nombre } into g
                select new ProductoMasVendidoDto
                {
                    ProductoId = g.Key.ProductoId,
                    Nombre = g.Key.Nombre,
                    CantidadVendida = g.Sum(x => x.Cantidad)
                };

            return await query
                .OrderByDescending(x => x.CantidadVendida)
                .Take(5)
                .ToListAsync();
        }
    }
}
