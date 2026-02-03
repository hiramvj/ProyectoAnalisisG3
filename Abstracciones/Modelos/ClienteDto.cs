using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    [Table("Cliente")]
    public class ClienteDto
    {
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres.")]
        public string NombreCompleto { get; set; } = default!;

        [StringLength(30, ErrorMessage = "La identificación no puede exceder 30 caracteres.")]
        public string? Identificacion { get; set; }

        [StringLength(120, ErrorMessage = "El correo no puede exceder 120 caracteres.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string? Correo { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres.")]
        public string? Telefono { get; set; }

        [StringLength(250, ErrorMessage = "La dirección no puede exceder 250 caracteres.")]
        public string? Direccion { get; set; }

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
    }
}
