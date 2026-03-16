using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Entidades
{
    [Table("transportista")]
    public class Transportista
    {
        [Key]
        [Column("transportistaid")]
        public int TransportistaId { get; set; }

        [Required]
        [Column("nombrecompleto")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Column("identificacion")]
        public string? Identificacion { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("activo")]
        public bool Activo { get; set; }
    }
}