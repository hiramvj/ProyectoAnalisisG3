using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    public class RutaEntregaDto
    {
        public int RutaId { get; set; }

        [Required(ErrorMessage = "El código de ruta es obligatorio.")]
        [Display(Name = "Código de ruta")]
        public string CodigoRuta { get; set; } = string.Empty;

        [Display(Name = "Fecha programada")]
        [DataType(DataType.Date)]
        public DateTime? FechaProgramada { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public string Estado { get; set; } = "PLANIFICADA";

        [Display(Name = "Transportista")]
        public int? TransportistaId { get; set; }

        [Display(Name = "Vehículo")]
        public int? VehiculoId { get; set; }

        public string? Observaciones { get; set; }

        public DateTime FechaCreacion { get; set; }

        // Para mostrar en tabla
        public string? NombreTransportista { get; set; }
    }
}