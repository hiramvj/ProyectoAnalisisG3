using Abstracciones.Modelos;
using DA.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Contexto
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductoDto> Productos { get; set; }
        public DbSet<ClienteDto> Clientes { get; set; }
        public DbSet<CategoriaProductoDto> CategoriasProducto { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductoDto>(entity =>
            {
                entity.HasKey(p => p.ProductoId);

                // 👇 IMPORTANTE: esta es tu tabla real en SQL Server
                entity.ToTable("Producto");

                // (Opcional pero recomendado) Definir decimales
                entity.Property(p => p.Costo).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Precio).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Stock).HasColumnType("decimal(18,2)");
                entity.Property(p => p.StockMinimo).HasColumnType("decimal(18,2)");
            });
            modelBuilder.Entity<CategoriaProductoDto>(entity =>
            {
                entity.HasKey(c => c.CategoriaProductoId);
                entity.ToTable("CategoriaProducto");
            });

            modelBuilder.Entity<ClienteDto>(entity =>
            {
                entity.HasKey(c => c.ClienteId);
                // Si tu tabla real de clientes también es singular, podrías hacer:
                // entity.ToTable("Cliente");
            });
        }
    }
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(@"Server=DESKTOP-R7KSH3B\SQLEXPRESS;Database=DistribuidoraTachiDB;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

            return new AppDbContext(options);
        }
    }
    
}