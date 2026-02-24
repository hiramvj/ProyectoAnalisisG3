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
    public class ProveedorDA : IProveedorDA
    {
        private readonly AppDbContext _db;
        public ProveedorDA(AppDbContext db) => _db = db;

        public async Task<List<ProveedorDto>> ListarPorEstadoAsync(bool activo)
        {
            return await _db.Proveedores
                .Where(p => p.Activo == activo)
                .Select(p => new ProveedorDto
                {
                    ProveedorId = p.ProveedorId,
                    NombreLegal = p.NombreLegal,
                    CedulaJuridica = p.CedulaJuridica,
                    Correo = p.Correo,
                    Telefono = p.Telefono,
                    Direccion = p.Direccion,
                    Activo = p.Activo,
                    FechaCreacion = p.FechaCreacion
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ProveedorDto?> ObtenerPorIdAsync(int proveedorId)
        {
            return await _db.Proveedores
                .Where(p => p.ProveedorId == proveedorId)
                .Select(p => new ProveedorDto
                {
                    ProveedorId = p.ProveedorId,
                    NombreLegal = p.NombreLegal,
                    CedulaJuridica = p.CedulaJuridica,
                    Correo = p.Correo,
                    Telefono = p.Telefono,
                    Direccion = p.Direccion,
                    Activo = p.Activo,
                    FechaCreacion = p.FechaCreacion
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<int> InsertarAsync(ProveedorDto proveedor)
        {
            proveedor.FechaCreacion = DateTime.UtcNow;
            proveedor.Activo = true;

            _db.Proveedores.Add(proveedor);
            return await _db.SaveChangesAsync();
        }

        public async Task<int> ActualizarAsync(ProveedorDto proveedor)
        {
            var existing = await _db.Proveedores.FindAsync(proveedor.ProveedorId);
            if (existing == null) return 0;

            existing.NombreLegal = proveedor.NombreLegal;
            existing.CedulaJuridica = proveedor.CedulaJuridica;
            existing.Correo = proveedor.Correo;
            existing.Telefono = proveedor.Telefono;
            existing.Direccion = proveedor.Direccion;

            return await _db.SaveChangesAsync();
        }

        public async Task<int> CambiarEstadoAsync(int proveedorId, bool activo)
        {
            var existing = await _db.Proveedores.FindAsync(proveedorId);
            if (existing == null) return 0;

            existing.Activo = activo;
            return await _db.SaveChangesAsync();
        }
    }
}