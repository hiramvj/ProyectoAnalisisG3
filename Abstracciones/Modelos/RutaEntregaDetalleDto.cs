using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Modelos
{
    public class RutaEntregaDetalleDto
    {
        public int RutaDetalleId { get; set; }

        [Required]
        public int RutaId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un pedido.")]
        [Display(Name = "Pedido")]
        public int PedidoVentaId { get; set; }

        [Required(ErrorMessage = "Debe indicar el orden.")]
        [Display(Name = "Orden de parada")]
        public int OrdenParada { get; set; }

        [Required]
        [Display(Name = "Estado parada")]
        public string EstadoParada { get; set; } = "Pendiente";
    }
}