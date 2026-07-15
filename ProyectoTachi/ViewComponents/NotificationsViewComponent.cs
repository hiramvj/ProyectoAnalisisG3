using DA.Contexto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoTachi.Models;

namespace ProyectoTachi.ViewComponents
{
    public class NotificationsViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public NotificationsViewComponent(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new NotificationsViewModel();
            var user = HttpContext.User;

            if (user.Identity?.IsAuthenticated != true)
                return View(model);

            if (user.IsInRole("Admin") || user.IsInRole("Ventas"))
            {
                var productos = await _db.Productos.AsNoTracking()
                    .Where(p => p.Activo && p.Stock <= p.StockMinimo)
                    .OrderBy(p => p.Stock)
                    .Take(5)
                    .Select(p => new { p.Nombre, p.Stock, p.StockMinimo })
                    .ToListAsync();

                model.Items.AddRange(productos.Select(p => new NotificationItemViewModel
                {
                    Title = $"Stock bajo: {p.Nombre}",
                    Detail = $"Disponible: {p.Stock:N0} · Mínimo: {p.StockMinimo:N0}",
                    Icon = "fa-solid fa-box-open",
                    ColorClass = "text-warning",
                    Controller = "Productos",
                    Action = "Index"
                }));
            }

            if (user.IsInRole("Admin") || user.IsInRole("Operaciones"))
            {
                var ordenes = await _db.OrdenesCompra.AsNoTracking()
                    .Where(o => o.Estado != "COMPLETADA" && o.Estado != "CANCELADA")
                    .OrderBy(o => o.FechaEsperada)
                    .Take(5)
                    .Select(o => new { o.NumeroOrden, o.Estado })
                    .ToListAsync();

                model.Items.AddRange(ordenes.Select(o => new NotificationItemViewModel
                {
                    Title = $"Orden pendiente: {o.NumeroOrden ?? "Sin número"}",
                    Detail = $"Estado: {o.Estado}",
                    Icon = "fa-solid fa-cart-flatbed",
                    ColorClass = "text-primary",
                    Controller = "Compras",
                    Action = "Index"
                }));
            }

            if (user.IsInRole("Admin") || user.IsInRole("Gerencia"))
            {
                var hoy = DateTime.UtcNow.Date;
                var cuentas = await _db.CuentasPorPagar.AsNoTracking()
                    .Where(c => c.SaldoPendiente > 0 && c.FechaVencimiento < hoy)
                    .OrderBy(c => c.FechaVencimiento)
                    .Take(5)
                    .Select(c => new { c.NumeroFactura, c.SaldoPendiente, c.FechaVencimiento })
                    .ToListAsync();

                model.Items.AddRange(cuentas.Select(c => new NotificationItemViewModel
                {
                    Title = $"Cuenta vencida: {c.NumeroFactura}",
                    Detail = $"Saldo: ₡{c.SaldoPendiente:N2} · Venció {c.FechaVencimiento:dd/MM/yyyy}",
                    Icon = "fa-solid fa-file-circle-exclamation",
                    ColorClass = "text-danger",
                    Controller = "CuentasPorPagar",
                    Action = "Index"
                }));
            }

            return View(model);
        }
    }
}
