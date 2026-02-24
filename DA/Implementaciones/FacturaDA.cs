using Abstracciones.Interfaces.DA;
using DA.Contexto;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Implementaciones
{
    public class FacturaDA : IFacturaDA
    {
        private readonly AppDbContext _db;

        public FacturaDA(AppDbContext db) => _db = db;

        public async Task<int> CrearDesdePedidoAsync(int pedidoVentaId)
        {
            // Ejecuta el SP usando DbConnection nativo para que sea agnóstico (PostgreSQL)
            using var command = _db.Database.GetDbConnection().CreateCommand();
            command.CommandText = "SELECT * FROM sp_Factura_CrearDesdePedido(@PedidoVentaId)";
            
            var pedidoIdParam = command.CreateParameter();
            pedidoIdParam.ParameterName = "@PedidoVentaId";
            pedidoIdParam.Value = pedidoVentaId;
            command.Parameters.Add(pedidoIdParam);

            await _db.Database.OpenConnectionAsync();
            var result = await command.ExecuteScalarAsync();
            
            if (result != null && int.TryParse(result.ToString(), out int facturaId))
            {
                return facturaId;
            }

            return 0;
        }
    }
}

