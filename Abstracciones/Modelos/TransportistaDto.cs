using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    public class TransportistaDto
    {
        public int TransportistaId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Display(Name = "Identificación")]
        public string? Identificacion { get; set; }

        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        public bool Activo { get; set; } = true;
    }
}