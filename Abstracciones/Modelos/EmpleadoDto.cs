using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Abstracciones.Modelos
{
    [Table("Empleado")]
    public class EmpleadoDto
    {
        public int EmpleadoId { get; set; }

        [Required(ErrorMessage = "El nombre completo es requerido.")]
        [StringLength(150, ErrorMessage = "El nombre completo no puede exceder 150 caracteres.")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La identificación es requerida.")]
        [StringLength(30, ErrorMessage = "La identificación no puede exceder 30 caracteres.")]
        public string Identificacion { get; set; } = string.Empty;

        [StringLength(120, ErrorMessage = "El correo no puede exceder 120 caracteres.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string? Correo { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres.")]
        public string? Telefono { get; set; }

        [StringLength(100, ErrorMessage = "El puesto no puede exceder 100 caracteres.")]
        public string? Puesto { get; set; }

        public bool Activo { get; set; } = true;

        [DataType(DataType.Date)]
        public DateTime? FechaIngreso { get; set; }
    }
}