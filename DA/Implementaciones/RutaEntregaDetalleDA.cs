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
    public class RutaEntregaDetalleDA : IRutaEntregaDetalleDA
    {
        private readonly AppDbContext _context;

        public RutaEntregaDetalleDA(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RutaEntregaDetalleVistaDto>> ObtenerPorRutaAsync(int rutaId)
        {
            return await _context.RutasEntregaDetalle
                .Where(d => d.RutaId == rutaId)
                .Select(d => new RutaEntregaDetalleVistaDto
                {
                    RutaDetalleId = d.RutaDetalleId,
                    RutaId = d.RutaId,
                    PedidoVentaId = d.PedidoVentaId,
                    OrdenParada = d.OrdenParada,
                    EstadoParada = d.EstadoParada
                })
                .ToListAsync();
        }

        public async Task AgregarAsync(RutaEntregaDetalleDto dto)
        {
            var entidad = new Entidades.RutaEntregaDetalle
            {
                RutaId = dto.RutaId,
                PedidoVentaId = dto.PedidoVentaId,
                OrdenParada = dto.OrdenParada,
                EstadoParada = dto.EstadoParada
            };

            _context.RutasEntregaDetalle.Add(entidad);
            await _context.SaveChangesAsync();
        }

        public async Task CambiarEstadoParadaAsync(int rutaDetalleId, string estadoParada)
        {
            var entidad = await _context.RutasEntregaDetalle
                .FirstAsync(d => d.RutaDetalleId == rutaDetalleId);

            entidad.EstadoParada = estadoParada;

            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int rutaDetalleId)
        {
            var entidad = await _context.RutasEntregaDetalle
                .FirstAsync(d => d.RutaDetalleId == rutaDetalleId);

            _context.RutasEntregaDetalle.Remove(entidad);
            await _context.SaveChangesAsync();
        }
    }
}