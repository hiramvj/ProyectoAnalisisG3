using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoTachi.Controllers
{
    [Authorize(Roles = "Admin,Gerencia")]
    public class ReportesController : Controller
    {
        private readonly IReporteFlujo _reporteFlujo;
        private readonly IClienteFlujo _clienteFlujo;
        private readonly IProductoFlujo _productoFlujo;
        private readonly DA.Contexto.AppDbContext _db;

        public ReportesController(
            IReporteFlujo reporteFlujo,
            IClienteFlujo clienteFlujo,
            IProductoFlujo productoFlujo,
            DA.Contexto.AppDbContext db)
        {
            _reporteFlujo = reporteFlujo;
            _clienteFlujo = clienteFlujo;
            _productoFlujo = productoFlujo;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ReporteVentasFiltroDto filtro, int page = 1)
        {
            int pageSize = 10;

            await CargarCombos(filtro);

            var lista = await _reporteFlujo.ObtenerReporteVentasAsync(filtro);

            lista = lista
                .OrderByDescending(x => x.FechaPedido)
                .ThenByDescending(x => x.NumeroPedido)
                .ToList();

            var totalRegistros = lista.Count();
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);

            if (page < 1)
            {
                page = 1;
            }

            if (totalPaginas > 0 && page > totalPaginas)
            {
                page = totalPaginas;
            }

            var listaPaginada = lista
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Filtro = filtro;
            ViewBag.PaginaActual = page;
            ViewBag.TotalPaginas = totalPaginas;

            return View(listaPaginada);
        }

        [HttpGet]
        public async Task<IActionResult> ExportarExcel(ReporteVentasFiltroDto filtro)
        {
            var lista = await _reporteFlujo.ObtenerReporteVentasAsync(filtro);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("ReporteVentas");

            worksheet.Cell(1, 1).Value = "Pedido";
            worksheet.Cell(1, 2).Value = "Fecha";
            worksheet.Cell(1, 3).Value = "Cliente";
            worksheet.Cell(1, 4).Value = "Producto";
            worksheet.Cell(1, 5).Value = "Cantidad";
            worksheet.Cell(1, 6).Value = "Precio Unitario";
            worksheet.Cell(1, 7).Value = "Total Línea";
            worksheet.Cell(1, 8).Value = "Estado";

            int fila = 2;

            foreach (var item in lista)
            {
                worksheet.Cell(fila, 1).Value = item.NumeroPedido;
                worksheet.Cell(fila, 2).Value = item.FechaPedido.ToString("dd/MM/yyyy");
                worksheet.Cell(fila, 3).Value = item.ClienteNombre;
                worksheet.Cell(fila, 4).Value = item.ProductoNombre;
                worksheet.Cell(fila, 5).Value = item.Cantidad;
                worksheet.Cell(fila, 6).Value = item.PrecioUnitario;
                worksheet.Cell(fila, 7).Value = item.TotalLinea;
                worksheet.Cell(fila, 8).Value = item.Estado;
                fila++;
            }

            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"ReporteVentas_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var clientes = await _clienteFlujo.ObtenerTodosAsync(true);
            var categorias = _db.CategoriasProducto.AsEnumerable();

            ViewBag.Clientes = new SelectList(clientes, "ClienteId", "NombreCompleto");
            ViewBag.Categorias = new SelectList(categorias, "CategoriaProductoId", "Nombre");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMetricas([FromQuery] MetricasFiltroDto filtro)
        {
            var metricas = await _reporteFlujo.ObtenerMetricasDashboardAsync(filtro);
            return Json(metricas);
        }

        private async Task CargarCombos(ReporteVentasFiltroDto filtro)
        {
            var clientes = await _clienteFlujo.ObtenerTodosAsync(true);
            var productos = await _productoFlujo.ObtenerTodosAsync(true);

            ViewBag.Clientes  = clientes.OrderBy(c => c.NombreCompleto).ToList();
            ViewBag.Productos = productos.OrderBy(p => p.Nombre).ToList();
        }
    }
}
