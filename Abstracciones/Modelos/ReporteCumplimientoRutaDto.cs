using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    public class ReporteCumplimientoRutaDto
    {
        public int RutaId { get; set; }

        public string CodigoRuta { get; set; } = string.Empty;

        public string? Transportista { get; set; }

        public DateTime? FechaProgramada { get; set; }

        public int TotalPedidos { get; set; }

        public int Entregados { get; set; }

        public int Pendientes { get; set; }

        public decimal PorcentajeCumplimiento { get; set; }
    }
}
