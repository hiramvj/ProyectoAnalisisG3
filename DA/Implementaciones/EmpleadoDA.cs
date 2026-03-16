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
    public class EmpleadoDA : IEmpleadoDA
    {
        private readonly AppDbContext _context;

        public EmpleadoDA(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EmpleadoDto>> ObtenerTodosAsync(bool activo)
        {
            return await _context.Empleados
                .Where(e => e.Activo == activo)
                .Select(e => new EmpleadoDto
                {
                    EmpleadoId = e.EmpleadoId,
                    NombreCompleto = e.NombreCompleto,
                    Identificacion = e.Identificacion,
                    Correo = e.Correo,
                    Telefono = e.Telefono,
                    Puesto = e.Puesto,
                    Activo = e.Activo,
                    FechaIngreso = e.FechaIngreso
                })
                .ToListAsync();
        }

        public async Task<EmpleadoDto?> ObtenerPorIdAsync(int id)
        {
            return await _context.Empleados
                .Where(e => e.EmpleadoId == id)
                .Select(e => new EmpleadoDto
                {
                    EmpleadoId = e.EmpleadoId,
                    NombreCompleto = e.NombreCompleto,
                    Identificacion = e.Identificacion,
                    Correo = e.Correo,
                    Telefono = e.Telefono,
                    Puesto = e.Puesto,
                    Activo = e.Activo,
                    FechaIngreso = e.FechaIngreso
                })
                .FirstOrDefaultAsync();
        }

        public async Task AgregarAsync(EmpleadoDto dto)
        {
            var entidad = new Empleado
            {
                NombreCompleto = dto.NombreCompleto,
                Identificacion = dto.Identificacion,
                Correo = dto.Correo,
                Telefono = dto.Telefono,
                Puesto = dto.Puesto,
                Activo = true,
                FechaIngreso = dto.FechaIngreso
            };

            _context.Empleados.Add(entidad);
            await _context.SaveChangesAsync();
        }

        public async Task EditarAsync(EmpleadoDto dto)
        {
            var entidad = await _context.Empleados
                .FirstAsync(e => e.EmpleadoId == dto.EmpleadoId);

            entidad.NombreCompleto = dto.NombreCompleto;
            entidad.Identificacion = dto.Identificacion;
            entidad.Correo = dto.Correo;
            entidad.Telefono = dto.Telefono;
            entidad.Puesto = dto.Puesto;
            entidad.FechaIngreso = dto.FechaIngreso;

            await _context.SaveChangesAsync();
        }

        public async Task CambiarEstadoAsync(int id, bool activo)
        {
            var entidad = await _context.Empleados
                .FirstAsync(e => e.EmpleadoId == id);

            entidad.Activo = activo;

            await _context.SaveChangesAsync();
        }
    }
}