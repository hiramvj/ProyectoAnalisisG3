
using System;

namespace ProyectoTachi.Models
{
    public class DashboardViewModel
    {
        public int TotalProductos { get; set; }
        public int ProductosBajoStock { get; set; }
        public int TotalClientes { get; set; }
        public int ClientesActivos { get; set; }
        public int PedidosHoy { get; set; }
        public int FacturasHoy { get; set; }
        public decimal VentasHoy { get; set; }
        public int OrdenesPendientes { get; set; }
        public int ProveedoresActivos { get; set; }
        public int RutasActivas { get; set; }
        public decimal VentasMes { get; set; }
        public decimal SaldoPorPagar { get; set; }
        public int CuentasVencidas { get; set; }
    }
}
