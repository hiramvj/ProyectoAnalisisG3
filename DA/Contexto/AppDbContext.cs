using Abstracciones.Modelos;
using DA.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
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
        public DbSet<UnidadMedidaDto> UnidadesMedida { get; set; }

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
            modelBuilder.Entity<UnidadMedidaDto>(entity =>
            {
                entity.HasKey(u => u.UnidadMedidaId);
                entity.ToTable("UnidadMedida");

                entity.Property(u => u.Nombre).HasMaxLength(150);
                entity.Property(u => u.Abreviatura).HasMaxLength(20);
            });
            modelBuilder.Entity<ClienteDto>(entity =>
            {
                entity.HasKey(c => c.ClienteId);
                entity.ToTable("Cliente");
            });
        }
    }
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            
            // Build config to read connection strings
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            if (isWindows)
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString ?? @"Server=DESKTOP-R7KSH3B\SQLEXPRESS;Database=DistribuidoraTachiDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
            else
            {
                var connectionString = configuration.GetConnectionString("SqliteConnection");
                optionsBuilder.UseSqlite(connectionString ?? "Data Source=app.db");
            }

            return new AppDbContext(optionsBuilder.Options);
        }
    }
    
}