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
    public class AsistenciaDA : IAsistenciaDA
    {
        private readonly AppDbContext _context;

        public AsistenciaDA(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AsistenciaDto>> ObtenerTodasAsync()
        {
            return await _context.Asistencias
                .Select(a => new AsistenciaDto
                {
                    AsistenciaId = a.AsistenciaId,
                    EmpleadoId = a.EmpleadoId,
                    Fecha = a.Fecha,
                    Tipo = a.Tipo,
                    HoraEntrada = a.HoraEntrada,
                    HoraSalida = a.HoraSalida
                    
                })
                .ToListAsync();
        }

        public async Task<List<AsistenciaDto>> ObtenerPorEmpleadoAsync(int empleadoId)
        {
            return await _context.Asistencias
                .Where(a => a.EmpleadoId == empleadoId)
                .Select(a => new AsistenciaDto
                {
                    AsistenciaId = a.AsistenciaId,
                    EmpleadoId = a.EmpleadoId,
                    Fecha = a.Fecha,
                    Tipo = a.Tipo,
                    HoraEntrada = a.HoraEntrada,
                    HoraSalida = a.HoraSalida
                })
                .ToListAsync();
        }

        public async Task AgregarAsync(AsistenciaDto dto)
        {
            var entidad = new Asistencia
            {
                EmpleadoId = dto.EmpleadoId,
                Fecha = dto.Fecha,
                Tipo = dto.Tipo,
                HoraEntrada = dto.HoraEntrada,
                HoraSalida = dto.HoraSalida
            };

            _context.Asistencias.Add(entidad);
            await _context.SaveChangesAsync();
        }

        public async Task RegistrarSalidaAsync(long asistenciaId, TimeSpan horaSalida)
        {
            var entidad = await _context.Asistencias
                .FirstAsync(a => a.AsistenciaId == asistenciaId);

            entidad.HoraSalida = horaSalida;

            await _context.SaveChangesAsync();
        }
    }
}