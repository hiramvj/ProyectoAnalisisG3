namespace Abstracciones.Modelos
{
    public class DevolucionLineaDto
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = "";
        public decimal CantidadFacturada { get; set; }
        public decimal CantidadDevuelta { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
