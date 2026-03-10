using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA.Entidades
{
    [Table("ordencompra")]
    public class OrdenCompra
    {
        [Key]
        [Column("ordencompraid")]
        public int OrdenCompraId { get; set; }

        [Column("bodegaid")]
        public int? BodegaId { get; set; }

        [Column("proveedorid")]
        public int ProveedorId { get; set; }

        [Column("numeroorden")]
        public string? NumeroOrden { get; set; }

        // ESTA ES LA COLUMNA REAL EN LA BD
        [Column("fechaorden")]
        public DateTime FechaOrden { get; set; }

        [Column("fechaesperada")]
        public DateTime? FechaEsperada { get; set; }

        [Column("estado")]
        public string Estado { get; set; } = "CREADA";

        [Column("observaciones")]
        public string? Observaciones { get; set; }

        public ICollection<OrdenCompraDetalle> Detalles { get; set; } = new List<OrdenCompraDetalle>();
    }
}