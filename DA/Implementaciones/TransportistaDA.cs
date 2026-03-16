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
    public class TransportistaDA : ITransportistaDA
    {
        private readonly AppDbContext _context;

        public TransportistaDA(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TransportistaDto>> ObtenerTodosAsync(bool activo)
        {
            return await _context.Transportistas
                .Where(t => t.Activo == activo)
                .Select(t => new TransportistaDto
                {
                    TransportistaId = t.TransportistaId,
                    NombreCompleto = t.NombreCompleto,
                    Identificacion = t.Identificacion,
                    Telefono = t.Telefono,
                    Activo = t.Activo
                })
                .ToListAsync();
        }

        public async Task<TransportistaDto?> ObtenerPorIdAsync(int id)
        {
            return await _context.Transportistas
                .Where(t => t.TransportistaId == id)
                .Select(t => new TransportistaDto
                {
                    TransportistaId = t.TransportistaId,
                    NombreCompleto = t.NombreCompleto,
                    Identificacion = t.Identificacion,
                    Telefono = t.Telefono,
                    Activo = t.Activo
                })
                .FirstOrDefaultAsync();
        }

        public async Task AgregarAsync(TransportistaDto dto)
        {
            var entidad = new Entidades.Transportista
            {
                NombreCompleto = dto.NombreCompleto,
                Identificacion = dto.Identificacion,
                Telefono = dto.Telefono,
                Activo = true
            };

            _context.Transportistas.Add(entidad);
            await _context.SaveChangesAsync();
        }

        public async Task EditarAsync(TransportistaDto dto)
        {
            var entidad = await _context.Transportistas
                .FirstAsync(t => t.TransportistaId == dto.TransportistaId);

            entidad.NombreCompleto = dto.NombreCompleto;
            entidad.Identificacion = dto.Identificacion;
            entidad.Telefono = dto.Telefono;

            await _context.SaveChangesAsync();
        }

        public async Task CambiarEstadoAsync(int id, bool activo)
        {
            var entidad = await _context.Transportistas
                .FirstAsync(t => t.TransportistaId == id);

            entidad.Activo = activo;

            await _context.SaveChangesAsync();
        }
    }
}