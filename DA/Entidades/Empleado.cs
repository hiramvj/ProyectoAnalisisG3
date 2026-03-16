using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Entidades
{
    [Table("Empleado")]
    public class Empleado
    {
        [Key]
        [Column("empleadoid")]
        public int EmpleadoId { get; set; }

        [Column("nombrecompleto")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Column("identificacion")]
        public string Identificacion { get; set; } = string.Empty;

        [Column("correo")]
        public string? Correo { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("puesto")]
        public string? Puesto { get; set; }

        [Column("activo")]
        public bool Activo { get; set; }

        [Column("fechaingreso", TypeName = "date")]
        public DateTime? FechaIngreso { get; set; }
    }
}