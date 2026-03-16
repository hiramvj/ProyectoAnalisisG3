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
    public class RutaEntregaDA : IRutaEntregaDA
    {
        private readonly AppDbContext _context;

        public RutaEntregaDA(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RutaEntregaDto>> ObtenerTodasAsync()
        {
            return await _context.RutasEntrega
                .Include(r => r.Transportista)
                .Select(r => new RutaEntregaDto
                {
                    RutaId = r.RutaId,
                    CodigoRuta = r.CodigoRuta,
                    FechaProgramada = r.FechaProgramada,
                    Estado = r.Estado,
                    TransportistaId = r.TransportistaId,
                    Observaciones = r.Observaciones,
                    FechaCreacion = r.FechaCreacion,
                    NombreTransportista = r.Transportista != null ? r.Transportista.NombreCompleto : null
                })
                .ToListAsync();
        }

        public async Task<RutaEntregaDto?> ObtenerPorIdAsync(int id)
        {
            return await _context.RutasEntrega
                .Where(r => r.RutaId == id)
                .Select(r => new RutaEntregaDto
                {
                    RutaId = r.RutaId,
                    CodigoRuta = r.CodigoRuta,
                    FechaProgramada = r.FechaProgramada,
                    Estado = r.Estado,
                    TransportistaId = r.TransportistaId,
                    Observaciones = r.Observaciones,
                    FechaCreacion = r.FechaCreacion
                })
                .FirstOrDefaultAsync();
        }

        public async Task AgregarAsync(RutaEntregaDto dto)
        {
            var entidad = new Entidades.RutaEntrega
            {
                CodigoRuta = dto.CodigoRuta,
                FechaProgramada = dto.FechaProgramada.HasValue
                    ? DateTime.SpecifyKind(dto.FechaProgramada.Value, DateTimeKind.Utc)
                    : null,
                Estado = dto.Estado,
                TransportistaId = dto.TransportistaId,
                Observaciones = dto.Observaciones,
                FechaCreacion = DateTime.UtcNow
            };

            _context.RutasEntrega.Add(entidad);
            await _context.SaveChangesAsync();
        }

        public async Task EditarAsync(RutaEntregaDto dto)
        {
            var entidad = await _context.RutasEntrega
                .FirstAsync(r => r.RutaId == dto.RutaId);

            entidad.CodigoRuta = dto.CodigoRuta;
            entidad.FechaProgramada = dto.FechaProgramada.HasValue
                ? DateTime.SpecifyKind(dto.FechaProgramada.Value, DateTimeKind.Utc)
                : null;
            entidad.TransportistaId = dto.TransportistaId;
            entidad.Observaciones = dto.Observaciones;
            entidad.Estado = dto.Estado;

            await _context.SaveChangesAsync();
        }

        public async Task CambiarEstadoAsync(int id, string estado)
        {
            var entidad = await _context.RutasEntrega
                .FirstAsync(r => r.RutaId == id);

            entidad.Estado = estado;

            await _context.SaveChangesAsync();
        }
    }
}