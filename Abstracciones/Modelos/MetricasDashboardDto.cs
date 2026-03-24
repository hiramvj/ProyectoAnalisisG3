using System;
using System.Collections.Generic;

namespace Abstracciones.Modelos
{
    public class MetricasFiltroDto
    {
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public int? CategoriaId { get; set; }
        public int? ClienteId { get; set; }
    }

    public class DashboardAgrupadoDto
    {
        public decimal TotalVentas { get; set; }
        public int CantidadPedidos { get; set; }
        
        public List<VentasPorMesDto> VentasPorMes { get; set; } = new List<VentasPorMesDto>();
        public List<VentasPorCategoriaDto> VentasPorCategoria { get; set; } = new List<VentasPorCategoriaDto>();
        public List<TopProductoDto> TopProductos { get; set; } = new List<TopProductoDto>();
    }

    public class VentasPorMesDto
    {
        public string Mes { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    public class VentasPorCategoriaDto
    {
        public string Categoria { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    public class TopProductoDto
    {
        public string Producto { get; set; } = string.Empty;
        public decimal CantidadVendida { get; set; }
        public decimal TotalRecaudado { get; set; }
    }
}
