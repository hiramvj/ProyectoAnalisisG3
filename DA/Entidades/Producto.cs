using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Entidades
{
    public class Producto
    {
        public int ProductoId { get; set; }
        public string SKU { get; set; } = default!;
        public string Nombre { get; set; } = default!;

        public int? CategoriaProductoId { get; set; }
        public int UnidadMedidaId { get; set; }

        public decimal Costo { get; set; }
        public decimal Precio { get; set; }
        public decimal StockMinimo { get; set; }

        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
