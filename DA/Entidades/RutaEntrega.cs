using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Entidades
{
    [Table("rutaentrega")]
    public class RutaEntrega
    {
        [Key]
        [Column("rutaid")]
        public int RutaId { get; set; }

        [Required]
        [Column("codigoruta")]
        public string CodigoRuta { get; set; } = string.Empty;

        [Column("fechaprogramada")]
        public DateTime? FechaProgramada { get; set; }

        [Required]
        [Column("estado")]
        public string Estado { get; set; } = string.Empty;

        [Column("transportistaid")]
        public int? TransportistaId { get; set; }

        [Column("vehiculoid")]
        public int? VehiculoId { get; set; }

        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; }

        [ForeignKey("TransportistaId")]
        public Transportista? Transportista { get; set; }
    }
}
