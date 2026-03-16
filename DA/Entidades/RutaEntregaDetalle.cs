using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Entidades
{
    [Table("rutaentregadetalle")]
    public class RutaEntregaDetalle
    {
        [Key]
        [Column("rutadetalleid")]
        public int RutaDetalleId { get; set; }

        [Column("rutaid")]
        public int RutaId { get; set; }

        [Column("pedidoventaid")]
        public int PedidoVentaId { get; set; }

        [Column("ordenparada")]
        public int OrdenParada { get; set; }

        [Required]
        [Column("estadoparada")]
        public string EstadoParada { get; set; } = string.Empty;

        [ForeignKey("RutaId")]
        public RutaEntrega? RutaEntrega { get; set; }
    }
}