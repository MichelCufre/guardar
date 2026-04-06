using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class PedidoDetalleDuplicadoRequest
    {
        /// <summary>
        /// Identificación en el ERP o sistema externo para la línea del pedido. 
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [Required]
        public string IdLineaSistemaExterno { get; set; }

        /// <summary>
        /// Tipo de línea. 
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string TipoLinea { get; set; }

        /// <summary>
        /// Cantidad de producto para el pedido. En caso de confirmación de mercadería preparada, refiere a la cantidad preparada. En caso de cierre de egreso, refiere a la cantidad expedida en el camión.
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        [Required]
        public decimal CantidadProducto { get; set; }

        /// <summary>
        /// Serializado
        /// </summary>
        /// <example>Valor de ejemplo</example>
        [ApiDtoExample("Valor de ejemplo")]
        public string Serializado { get; set; }
    }
}