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
    public class ProductoDA : IProductoDA
    {
        private readonly AppDbContext _db;

        public ProductoDA(AppDbContext db) => _db = db;

        public async Task<List<ProductoDto>> ListarPorEstadoAsync(bool activo)
        {
            return await _db.Productos
                .FromSqlInterpolated($"EXEC dbo.sp_Producto_ListarPorEstado @Activo={activo}")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ProductoDto?> ObtenerPorIdAsync(int productoId)
        {
            var lista = await _db.Productos
                .FromSqlInterpolated($"EXEC dbo.sp_Producto_ObtenerPorId @ProductoId={productoId}")
                .AsNoTracking()
                .ToListAsync();

            return lista.FirstOrDefault();
        }

        public async Task<int> InsertarAsync(ProductoDto p)
        {
            using var cmd = _db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "dbo.sp_Producto_Insertar";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@SKU", p.SKU));
            cmd.Parameters.Add(new SqlParameter("@Nombre", p.Nombre));
            cmd.Parameters.Add(new SqlParameter("@CategoriaProductoId", (object?)p.CategoriaProductoId ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@UnidadMedidaId", p.UnidadMedidaId));
            cmd.Parameters.Add(new SqlParameter("@Costo", p.Costo));
            cmd.Parameters.Add(new SqlParameter("@Precio", p.Precio));
            cmd.Parameters.Add(new SqlParameter("@StockMinimo", p.StockMinimo));

            if (cmd.Connection!.State != System.Data.ConnectionState.Open)
                await cmd.Connection.OpenAsync();

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task<int> ActualizarAsync(ProductoDto p)
        {
            using var cmd = _db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "dbo.sp_Producto_Actualizar";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@ProductoId", p.ProductoId));
            cmd.Parameters.Add(new SqlParameter("@SKU", p.SKU));
            cmd.Parameters.Add(new SqlParameter("@Nombre", p.Nombre));
            cmd.Parameters.Add(new SqlParameter("@CategoriaProductoId", (object?)p.CategoriaProductoId ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@UnidadMedidaId", p.UnidadMedidaId));
            cmd.Parameters.Add(new SqlParameter("@Costo", p.Costo));
            cmd.Parameters.Add(new SqlParameter("@Precio", p.Precio));
            cmd.Parameters.Add(new SqlParameter("@StockMinimo", p.StockMinimo));

            if (cmd.Connection!.State != System.Data.ConnectionState.Open)
                await cmd.Connection.OpenAsync();

            var result = await cmd.ExecuteScalarAsync(); 
            return Convert.ToInt32(result);
        }

        public async Task<int> CambiarEstadoAsync(int productoId, bool activo)
        {
            using var cmd = _db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "dbo.sp_Producto_CambiarEstado";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@ProductoId", productoId));
            cmd.Parameters.Add(new SqlParameter("@Activo", activo));

            if (cmd.Connection!.State != System.Data.ConnectionState.Open)
                await cmd.Connection.OpenAsync();

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        private static ProductoDto MapToDto(Producto e) => new ProductoDto
        {
            ProductoId = e.ProductoId,
            SKU = e.SKU,
            Nombre = e.Nombre,
            CategoriaProductoId = e.CategoriaProductoId,
            UnidadMedidaId = e.UnidadMedidaId,
            Costo = e.Costo,
            Precio = e.Precio,
            StockMinimo = e.StockMinimo,
            Activo = e.Activo,
            FechaCreacion = e.FechaCreacion
        };
    }
}