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

            if (filtro.Desde.HasValue)
            {
                var desde = filtro.Desde.Value.Date;
                query = query.Where(x => x.FechaPedido.Date >= desde);
            }

            if (filtro.Hasta.HasValue)
            {
                var hasta = filtro.Hasta.Value.Date;
                query = query.Where(x => x.FechaPedido.Date <= hasta);
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
    }
}