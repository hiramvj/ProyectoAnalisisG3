using System;
using System.Collections.Generic;

namespace Abstracciones.Modelos
{
    public class OrdenCompraCrearDto
    {
        public int ProveedorId { get; set; }
        public string? Observaciones { get; set; }
        public List<OrdenCompraLineaDto> Lineas { get; set; } = new();
    }

    public class OrdenCompraLineaDto
    {
        public int ProductoId { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
    }
}
