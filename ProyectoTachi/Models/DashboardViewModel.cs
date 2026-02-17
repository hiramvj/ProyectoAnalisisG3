
using System;

namespace ProyectoTachi.Models
{
    public class DashboardViewModel
    {
        public int TotalProductos { get; set; }
        public int ProductosBajoStock { get; set; }
        public int TotalClientes { get; set; }
        public int ClientesActivos { get; set; }
    }
}
