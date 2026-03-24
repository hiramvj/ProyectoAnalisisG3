using Abstracciones.Modelos;
using DA.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
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
        public DbSet<PedidoVenta> PedidoVentas { get; set; }
        public DbSet<PedidoVentaDetalle> PedidoVentaDetalles { get; set; }
        public DbSet<ProveedorDto> Proveedores { get; set; }
        public DbSet<MetodoPagoDto> MetodosPago { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<FacturaDetalle> FacturaDetalles { get; set; }
        public DbSet<OrdenCompra> OrdenesCompra { get; set; }
        public DbSet<OrdenCompraDetalle> OrdenCompraDetalles { get; set; }
        public DbSet<Transportista> Transportistas { get; set; }
        public DbSet<RutaEntrega> RutasEntrega { get; set; }
        public DbSet<RutaEntregaDetalle> RutasEntregaDetalle { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<DevolucionVenta> DevolucionesVenta { get; set; }
        public DbSet<DevolucionVentaDetalle> DevolucionesVentaDetalle { get; set; }
        public DbSet<NotaCredito> NotasCredito { get; set; }
        public DbSet<CuentaPorPagar> CuentasPorPagar { get; set; }
        public DbSet<PagoProveedor> PagosProveedor { get; set; }

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
            modelBuilder.Entity<PedidoVenta>(entity =>
            {
                entity.ToTable("PedidoVenta");
            });

            modelBuilder.Entity<PedidoVentaDetalle>(entity =>
            {
                entity.ToTable("PedidoVentaDetalle");
                entity.HasOne(d => d.PedidoVenta)
                      .WithMany(p => p.Detalles)
                      .HasForeignKey(d => d.PedidoVentaId);
            });
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
            modelBuilder.Entity<MetodoPagoDto>(entity =>
            {
                entity.HasKey(x => x.MetodoPagoId);

                // ✅ Tabla real en Supabase 
                entity.ToTable("MetodoPago");

                // ✅ Columnas reales 
                entity.Property(x => x.MetodoPagoId).HasColumnName("metodopagoid");
                entity.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(150);
            });

            modelBuilder.Entity<DevolucionVenta>(entity =>
            {
                entity.ToTable("DevolucionVenta");
                entity.HasKey(d => d.DevolucionVentaId);
            });

            modelBuilder.Entity<DevolucionVentaDetalle>(entity =>
            {
                entity.ToTable("DevolucionVentaDetalle");
                entity.HasKey(d => d.DevolucionVentaDetalleId);
                entity.HasOne(d => d.DevolucionVenta)
                      .WithMany(dv => dv.Detalles)
                      .HasForeignKey(d => d.DevolucionVentaId);
            });

            modelBuilder.Entity<NotaCredito>(entity =>
            {
                entity.ToTable("NotaCredito");
                entity.HasKey(n => n.NotaCreditoId);
                entity.HasOne(n => n.DevolucionVenta)
                      .WithOne(d => d.NotaCredito)
                      .HasForeignKey<NotaCredito>(n => n.DevolucionVentaId);
            });

            modelBuilder.Entity<Asistencia>(entity =>
            {
                entity.ToTable("asistencia", t => t.ExcludeFromMigrations());
            });

            modelBuilder.Entity<CuentaPorPagar>(entity =>
            {
                entity.ToTable("CuentaPorPagar");
                entity.HasKey(c => c.CuentaPorPagarId);
            });

            modelBuilder.Entity<PagoProveedor>(entity =>
            {
                entity.ToTable("PagoProveedor");
                entity.HasKey(p => p.PagoProveedorId);
                entity.HasOne(p => p.CuentaPorPagar)
                      .WithMany(c => c.Pagos)
                      .HasForeignKey(p => p.CuentaPorPagarId);
            });
        }
    }
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Build config to read connection strings
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString ?? "Host=localhost;Database=postgres;Username=postgres");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
    
}