using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using DA.Contexto;
using DA.Entidades;
using Microsoft.Data.SqlClient;
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
                .FromSqlInterpolated($"EXEC dbo.sp_Cliente_ListarPorEstado @Activo={activo}")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ClienteDto?> ObtenerPorIdAsync(int clienteId)
        {
            var lista = await _db.Clientes
                .FromSqlInterpolated($"EXEC dbo.sp_Cliente_ObtenerPorId @ClienteId={clienteId}")
                .AsNoTracking()
                .ToListAsync();

            return lista.FirstOrDefault();
        }

        public async Task<int> InsertarAsync(ClienteDto c)
        {
            using var cmd = _db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "dbo.sp_Cliente_Insertar";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@NombreCompleto", c.NombreCompleto));
            cmd.Parameters.Add(new SqlParameter("@Identificacion", (object?)c.Identificacion ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Correo", (object?)c.Correo ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Telefono", (object?)c.Telefono ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Direccion", (object?)c.Direccion ?? DBNull.Value));

            if (cmd.Connection!.State != System.Data.ConnectionState.Open)
                await cmd.Connection.OpenAsync();

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task<int> ActualizarAsync(ClienteDto c)
        {
            using var cmd = _db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "dbo.sp_Cliente_Actualizar";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@ClienteId", c.ClienteId));
            cmd.Parameters.Add(new SqlParameter("@NombreCompleto", c.NombreCompleto));
            cmd.Parameters.Add(new SqlParameter("@Identificacion", (object?)c.Identificacion ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Correo", (object?)c.Correo ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Telefono", (object?)c.Telefono ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Direccion", (object?)c.Direccion ?? DBNull.Value));

            if (cmd.Connection!.State != System.Data.ConnectionState.Open)
                await cmd.Connection.OpenAsync();

            var result = await cmd.ExecuteScalarAsync(); 
            return Convert.ToInt32(result);
        }

        public async Task<int> CambiarEstadoAsync(int clienteId, bool activo)
        {
            using var cmd = _db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "dbo.sp_Cliente_CambiarEstado";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@ClienteId", clienteId));
            cmd.Parameters.Add(new SqlParameter("@Activo", activo));

            if (cmd.Connection!.State != System.Data.ConnectionState.Open)
                await cmd.Connection.OpenAsync();

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }
    }
}
