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
        public DbSet<FacturaDto> Facturas { get; set; }
        public DbSet<ProductoDto> Productos { get; set; }
        public DbSet<ClienteDto> Clientes { get; set; }
        public DbSet<CategoriaProductoDto> CategoriasProducto { get; set; }
        public DbSet<UnidadMedidaDto> UnidadesMedida { get; set; }
        public DbSet<PedidoVenta> PedidoVentas { get; set; }
        public DbSet<PedidoVentaDetalle> PedidoVentaDetalles { get; set; }
        public DbSet<ProveedorDto> Proveedores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<FacturaDto>(entity =>
            {
                entity.HasKey(f => f.FacturaId);

    
                entity.ToTable("Factura");

                entity.Property(f => f.Subtotal).HasColumnType("decimal(18,2)");
                entity.Property(f => f.Impuesto).HasColumnType("decimal(18,2)");
                entity.Property(f => f.Total).HasColumnType("decimal(18,2)");

                entity.Property(f => f.Estado).HasMaxLength(50);
            });




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
                // Si tu tabla real de clientes también es singular, podrías hacer:
                // entity.ToTable("Cliente");
            });
            modelBuilder.Entity<PedidoVentaDetalle>()
    .HasOne(d => d.PedidoVenta)
    .WithMany(p => p.Detalles)
    .HasForeignKey(d => d.PedidoVentaId);
            modelBuilder.Entity<ProveedorDto>(entity =>
            {
                entity.HasKey(p => p.ProveedorId);
                entity.ToTable("Proveedor");

                entity.Property(p => p.NombreLegal).HasMaxLength(200);
                entity.Property(p => p.CedulaJuridica).HasMaxLength(50);
                entity.Property(p => p.Correo).HasMaxLength(150);
                entity.Property(p => p.Telefono).HasMaxLength(50);
                entity.Property(p => p.Direccion).HasMaxLength(250);
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