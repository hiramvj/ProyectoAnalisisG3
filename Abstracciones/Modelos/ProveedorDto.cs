using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Abstracciones.Modelos
{
    [Table("Proveedor")]
    public class ProveedorDto
    {
        public int ProveedorId { get; set; }

        [Required(ErrorMessage = "El nombre legal es requerido.")]
        [StringLength(150, ErrorMessage = "El nombre legal no puede exceder 150 caracteres.")]
        public string NombreLegal { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cédula jurídica es requerida.")]
        [StringLength(30, ErrorMessage = "La cédula jurídica no puede exceder 30 caracteres.")]
        public string CedulaJuridica { get; set; } = string.Empty;

        [StringLength(120, ErrorMessage = "El correo no puede exceder 120 caracteres.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string? Correo { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres.")]
        public string? Telefono { get; set; }

        [StringLength(250, ErrorMessage = "La dirección no puede exceder 250 caracteres.")]
        public string? Direccion { get; set; }

        public bool Activo { get; set; } = true;

        [Required(ErrorMessage = "La fecha de creación es requerida.")]
        public DateTime FechaCreacion { get; set; }
    }
}