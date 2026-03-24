using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using DA.Contexto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Implementaciones
{
    public class ReporteDA : IReporteDA
    {
        private readonly AppDbContext _context;

        public ReporteDA(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReporteVentaDto>> ObtenerReporteVentasAsync(ReporteVentasFiltroDto filtro)
        {
            DateTime? desdeUtc = null;
            DateTime? hastaUtc = null;

            if (filtro.Desde.HasValue)
            {
                desdeUtc = DateTime.SpecifyKind(filtro.Desde.Value.Date, DateTimeKind.Utc);
            }

            if (filtro.Hasta.HasValue)
            {
                hastaUtc = DateTime.SpecifyKind(
                    filtro.Hasta.Value.Date.AddDays(1).AddTicks(-1),
                    DateTimeKind.Utc
                );
            }

            var query =
                from pv in _context.PedidoVentas.AsNoTracking()
                join c in _context.Clientes.AsNoTracking()
                    on pv.ClienteId equals c.ClienteId
                join pvd in _context.PedidoVentaDetalles.AsNoTracking()
                    on pv.PedidoVentaId equals pvd.PedidoVentaId
                join p in _context.Productos.AsNoTracking()
                    on pvd.ProductoId equals p.ProductoId
                select new
                {
                    pv.PedidoVentaId,
                    pv.NumeroPedido,
                    pv.FechaPedido,
                    pv.ClienteId,
                    ClienteNombre = c.NombreCompleto,
                    p.ProductoId,
                    ProductoNombre = p.Nombre,
                    pvd.Cantidad,
                    pvd.PrecioUnitario,
                    TotalLinea = pvd.Cantidad * pvd.PrecioUnitario,
                    pv.Estado
                };

            if (desdeUtc.HasValue)
            {
                query = query.Where(x => x.FechaPedido >= desdeUtc.Value);
            }

            if (hastaUtc.HasValue)
            {
                query = query.Where(x => x.FechaPedido <= hastaUtc.Value);
            }

            if (filtro.ClienteId.HasValue)
            {
                query = query.Where(x => x.ClienteId == filtro.ClienteId.Value);
            }

            if (filtro.ProductoId.HasValue)
            {
                query = query.Where(x => x.ProductoId == filtro.ProductoId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtro.Estado))
            {
                query = query.Where(x => x.Estado == filtro.Estado);
            }

            var resultado = await query
                .OrderByDescending(x => x.FechaPedido)
                .ThenByDescending(x => x.NumeroPedido)
                .Select(x => new ReporteVentaDto
                {
                    PedidoVentaId = x.PedidoVentaId,
                    NumeroPedido = x.NumeroPedido,
                    FechaPedido = x.FechaPedido,
                    ClienteNombre = x.ClienteNombre,
                    ProductoNombre = x.ProductoNombre,
                    Cantidad = x.Cantidad,
                    PrecioUnitario = x.PrecioUnitario,
                    TotalLinea = x.TotalLinea,
                    Estado = x.Estado
                })
                .ToListAsync();

            return resultado;
        }

        public async Task<DashboardAgrupadoDto> ObtenerMetricasDashboardAsync(MetricasFiltroDto filtro)
        {
            var query = from pv in _context.PedidoVentas.AsNoTracking()
                        join pvd in _context.PedidoVentaDetalles.AsNoTracking() on pv.PedidoVentaId equals pvd.PedidoVentaId
                        join p in _context.Productos.AsNoTracking() on pvd.ProductoId equals p.ProductoId
                        join c in _context.CategoriasProducto.AsNoTracking() on p.CategoriaProductoId equals c.CategoriaProductoId
                        where pv.Estado != "CANCELADA"
                        select new { pv, pvd, p, c };

            if (filtro.Desde.HasValue)
            {
                var d = DateTime.SpecifyKind(filtro.Desde.Value.Date, DateTimeKind.Utc);
                query = query.Where(x => x.pv.FechaPedido >= d);
            }
            if (filtro.Hasta.HasValue)
            {
                var h = DateTime.SpecifyKind(filtro.Hasta.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
                query = query.Where(x => x.pv.FechaPedido <= h);
            }
            if (filtro.CategoriaId.HasValue)
            {
                query = query.Where(x => x.p.CategoriaProductoId == filtro.CategoriaId.Value);
            }
            if (filtro.ClienteId.HasValue)
            {
                query = query.Where(x => x.pv.ClienteId == filtro.ClienteId.Value);
            }

            var rawData = await query.ToListAsync();

            var dto = new DashboardAgrupadoDto();
            
            dto.TotalVentas = rawData.Sum(x => x.pvd.Cantidad * x.pvd.PrecioUnitario);
            dto.CantidadPedidos = rawData.Select(x => x.pv.PedidoVentaId).Distinct().Count();

            dto.VentasPorMes = rawData
                .GroupBy(x => x.pv.FechaPedido.ToString("yyyy-MM"))
                .Select(g => new VentasPorMesDto { Mes = g.Key, Total = g.Sum(x => x.pvd.Cantidad * x.pvd.PrecioUnitario) })
                .OrderBy(x => x.Mes)
                .ToList();

            dto.VentasPorCategoria = rawData
                .GroupBy(x => x.c.Nombre)
                .Select(g => new VentasPorCategoriaDto { Categoria = g.Key, Total = g.Sum(x => x.pvd.Cantidad * x.pvd.PrecioUnitario) })
                .OrderByDescending(x => x.Total)
                .ToList();

            dto.TopProductos = rawData
                .GroupBy(x => x.p.Nombre)
                .Select(g => new TopProductoDto 
                { 
                    Producto = g.Key, 
                    CantidadVendida = g.Sum(x => x.pvd.Cantidad),
                    TotalRecaudado = g.Sum(x => x.pvd.Cantidad * x.pvd.PrecioUnitario)
                })
                .OrderByDescending(x => x.CantidadVendida)
                .Take(5)
                .ToList();

            return dto;
        }
    }
}