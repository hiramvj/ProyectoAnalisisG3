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
    public class ClienteDA : IClienteDA
    {
        private readonly AppDbContext _db;

        public ClienteDA(AppDbContext db) => _db = db;

        public async Task<List<ClienteDto>> ListarPorEstadoAsync(bool activo)
        {
            return await _db.Clientes
                .Where(c => c.Activo == activo) // LINQ instead of EXEC
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ClienteDto?> ObtenerPorIdAsync(int clienteId)
        {
            return await _db.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClienteId == clienteId);
        }

        public async Task<int> InsertarAsync(ClienteDto c)
        {
            c.FechaCreacion = DateTime.UtcNow;
            _db.Clientes.Add(c);
            await _db.SaveChangesAsync();
            return c.ClienteId;
        }

        public async Task<int> ActualizarAsync(ClienteDto c)
        {
            var existing = await _db.Clientes.FindAsync(c.ClienteId);
            if (existing == null) return 0;

            existing.NombreCompleto = c.NombreCompleto;
            existing.Identificacion = c.Identificacion;
            existing.Correo = c.Correo;
            existing.Telefono = c.Telefono;
            existing.Direccion = c.Direccion;
            // Activo and FechaCreacion usually not updated here

            _db.Clientes.Update(existing);
            return await _db.SaveChangesAsync();
        }

        public async Task<int> CambiarEstadoAsync(int clienteId, bool activo)
        {
            var existing = await _db.Clientes.FindAsync(clienteId);
            if (existing == null) return 0;

            existing.Activo = activo;
            return await _db.SaveChangesAsync();
        }
    }
}
