using DA.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Contexto
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos => Set<Producto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>(entity =>
            {
                // Tabla
                entity.ToTable("Producto", "dbo");

                // Primary Key
                entity.HasKey(p => p.ProductoId);

                // Campos
                entity.Property(p => p.SKU)
                      .IsRequired()
                      .HasMaxLength(40);

                entity.Property(p => p.Nombre)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(p => p.CategoriaProductoId)
                      .IsRequired(false);

                entity.Property(p => p.UnidadMedidaId)
                      .IsRequired();

                entity.Property(p => p.Costo)
                      .HasColumnType("decimal(18,2)");

                entity.Property(p => p.Precio)
                      .HasColumnType("decimal(18,2)");

                entity.Property(p => p.StockMinimo)
                      .HasColumnType("decimal(18,2)");

                entity.Property(p => p.Activo)
                      .IsRequired();

                entity.Property(p => p.FechaCreacion)
                      .IsRequired();
            });
        }
    }
}