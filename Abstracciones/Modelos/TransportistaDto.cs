using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Abstracciones.Modelos
{
    [Table("Transportista")]
    public class TransportistaDto
    {
        public int TransportistaId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres.")]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "La identificación no puede exceder 30 caracteres.")]
        [Display(Name = "Identificación")]
        public string? Identificacion { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres.")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        public bool Activo { get; set; } = true;
    }
}