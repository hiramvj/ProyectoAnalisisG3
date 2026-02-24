using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace DA.Implementaciones
{
    public class FacturaDA : IFacturaDA
    {
        private readonly AppDbContext _db;

        public FacturaDA(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<FacturaDto>> ListarAsync(string? estado, DateTime? fechaEmision)
        {
            var query = _db.Facturas.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(f => f.Estado == estado);

            if (fechaEmision.HasValue)
            {
                var fecha = fechaEmision.Value.Date;
                query = query.Where(f => f.FechaEmision.Date == fecha);
            }

            return await query
                .OrderByDescending(f => f.FechaEmision)
                .ToListAsync();
        }

        public async Task<FacturaDto?> ObtenerPorIdAsync(int facturaId)
        {
            return await _db.Facturas
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FacturaId == facturaId);
        }

        public async Task<int> InsertarAsync(FacturaDto factura)
        {
            _db.Facturas.Add(factura);
            await _db.SaveChangesAsync();
            return factura.FacturaId;
        }

        public async Task<int> ActualizarAsync(FacturaDto factura)
        {
            var existing = await _db.Facturas.FindAsync(factura.FacturaId);
            if (existing == null) return 0;

            existing.NumeroFactura = factura.NumeroFactura;
            existing.PedidoVentaId = factura.PedidoVentaId;
            existing.FechaEmision = factura.FechaEmision;
            existing.Subtotal = factura.Subtotal;
            existing.Impuesto = factura.Impuesto;
            existing.Total = factura.Total;
            existing.Estado = factura.Estado;

            return await _db.SaveChangesAsync();
        }

        public async Task<int> CambiarEstadoAsync(int facturaId, string nuevoEstado)
        {
            var existing = await _db.Facturas.FindAsync(facturaId);
            if (existing == null) return 0;

            existing.Estado = nuevoEstado;
            return await _db.SaveChangesAsync();
        }
    }
}