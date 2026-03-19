using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flujo
{
    public class ReporteFlujo : IReporteFlujo
    {
        private readonly IReporteDA _reporteDA;

        public ReporteFlujo(IReporteDA reporteDA)
        {
            _reporteDA = reporteDA;
        }

        public async Task<IEnumerable<ReporteVentaDto>> ObtenerReporteVentasAsync(ReporteVentasFiltroDto filtro)
        {
            return await _reporteDA.ObtenerReporteVentasAsync(filtro);
        }
    }
}
