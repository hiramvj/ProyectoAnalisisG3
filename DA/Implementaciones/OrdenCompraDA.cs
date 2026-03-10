using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using DA.Contexto;
using DA.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DA.Implementaciones
{
    public class OrdenCompraDA : IOrdenCompraDA
    {
        private readonly AppDbContext _db;

        public OrdenCompraDA(AppDbContext db)
        {
            _db = db;
        }

        public async Task<int> CrearOrdenAsync(OrdenCompraCrearDto dto)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                int maxId = await _db.OrdenesCompra.MaxAsync(o => (int?)o.OrdenCompraId) ?? 0;
                string numeroOrden = "OC-" + (maxId + 1).ToString("D6");

                var orden = new OrdenCompra
                {
                    ProveedorId = dto.ProveedorId,
                    Observaciones = dto.Observaciones,
                    FechaOrden = DateTime.UtcNow,
                    Estado = "CREADA",
                    NumeroOrden = numeroOrden,

                    // OJO con esto:
                    // si BodegaId es obligatorio en la BD, hay que mandarlo
                    BodegaId = 1
                };

                _db.OrdenesCompra.Add(orden);
                await _db.SaveChangesAsync();

                foreach (var linea in dto.Lineas)
                {
                    var detalle = new OrdenCompraDetalle
                    {
                        OrdenCompraId = orden.OrdenCompraId,
                        ProductoId = linea.ProductoId,
                        Cantidad = linea.Cantidad,
                        CostoUnitario = linea.CostoUnitario
                    };

                    _db.OrdenCompraDetalles.Add(detalle);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return orden.OrdenCompraId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
