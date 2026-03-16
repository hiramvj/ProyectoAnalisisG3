using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    public class ProductoMasVendidoDto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = "";
        public decimal CantidadVendida { get; set; }
    }
}