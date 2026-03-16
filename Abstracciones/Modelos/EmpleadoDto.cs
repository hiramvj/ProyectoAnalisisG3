using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    public class EmpleadoDto
    {
        public int EmpleadoId { get; set; }

        [Required]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        public string Identificacion { get; set; } = string.Empty;

        public string? Correo { get; set; }

        public string? Telefono { get; set; }

        public string? Puesto { get; set; }

        public bool Activo { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaIngreso { get; set; }
    }
}
