using Abstracciones.Interfaces.DA;
using DA.Contexto;
using Microsoft.Data.SqlClient;
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
            // Output parameter
            var facturaIdParam = new SqlParameter("@FacturaId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            // Input parameter
            var pedidoIdParam = new SqlParameter("@PedidoVentaId", SqlDbType.Int)
            {
                Value = pedidoVentaId
            };

            // Ejecuta el SP (IMPORTANTE: @FacturaId OUTPUT)
            await _db.Database.ExecuteSqlRawAsync(
                "EXEC dbo.sp_Factura_CrearDesdePedido @PedidoVentaId, @FacturaId OUTPUT",
                pedidoIdParam,
                facturaIdParam
            );

            // Si el SP hace THROW, esto lanza excepción y no llega aquí
            return (int)(facturaIdParam.Value ?? 0);
        }
    }
}

