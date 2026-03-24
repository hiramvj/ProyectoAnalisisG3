namespace ProyectoTachi.Models
{
    public class InformeFinancieroViewModel
    {
        public decimal ValorTotalCosto { get; set; }
        public decimal ValorTotalVenta { get; set; }
        public decimal MargenGananciaEstimado { get; set; }
        public List<DetalleFinancieroProducto> Productos { get; set; } = new();
    }

    public class DetalleFinancieroProducto
    {
        public string Nombre { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal SubtotalVenta { get; set; }
    }
}
