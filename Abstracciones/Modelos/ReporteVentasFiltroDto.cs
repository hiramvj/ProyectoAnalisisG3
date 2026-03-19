using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    public class ReporteVentasFiltroDto
    {
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public int? ClienteId { get; set; }
        public int? ProductoId { get; set; }
        public string? Estado { get; set; }
    }
}
