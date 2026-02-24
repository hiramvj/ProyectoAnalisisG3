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
    public class ProductoDA : IProductoDA
    {
        private readonly AppDbContext _db;

        public ProductoDA(AppDbContext db) => _db = db;

        public async Task<List<ProductoDto>> ListarPorEstadoAsync(bool activo)
        {
            return await _db.Productos
                .Where(p => p.Activo == activo)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ProductoDto?> ObtenerPorIdAsync(int productoId)
        {
            return await _db.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductoId == productoId);
        }

        public async Task<int> InsertarAsync(ProductoDto p)
        {
            p.FechaCreacion = DateTime.UtcNow; // Ensure creation date is set
            _db.Productos.Add(p);
            await _db.SaveChangesAsync();
            return p.ProductoId;
        }

        public async Task<int> ActualizarAsync(ProductoDto p)
        {
            var existing = await _db.Productos.FindAsync(p.ProductoId);
            if (existing == null) return 0;

            existing.SKU = p.SKU;
            existing.Nombre = p.Nombre;
            existing.CategoriaProductoId = p.CategoriaProductoId;
            existing.UnidadMedidaId = p.UnidadMedidaId;
            existing.Costo = p.Costo;
            existing.Precio = p.Precio;
            existing.Stock = p.Stock;
            existing.StockMinimo = p.StockMinimo;
            // FechaCreacion and Activo might strictly not change here or depend on logic, keeping unsafe updates minimal
            
            _db.Productos.Update(existing);
            return await _db.SaveChangesAsync(); 
        }

        public async Task<int> CambiarEstadoAsync(int productoId, bool activo)
        {
            var existing = await _db.Productos.FindAsync(productoId);
            if (existing == null) return 0;

            existing.Activo = activo;
            return await _db.SaveChangesAsync();
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