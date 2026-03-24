using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IReporteFlujo
    {
        Task<IEnumerable<ReporteVentaDto>> ObtenerReporteVentasAsync(ReporteVentasFiltroDto filtro);
        Task<DashboardAgrupadoDto> ObtenerMetricasDashboardAsync(MetricasFiltroDto filtro);
    }
}
