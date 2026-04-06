using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class PedidoDetalleRequest
    {
        /// <summary>
        /// Código del producto
        /// </summary>
        /// <example>PR1</example>
        [ApiDtoExample("PR1")]
        [Required]
        public string Producto { get; set; } // CD_PRODUTO

        /// <summary>
        /// Identificador del producto (serie o lote)
        /// </summary>
        /// <example>LOTEAAA</example>
        [ApiDtoExample("LOTEAAA")]
        public string Identificador { get; set; } // NU_IDENTIFICADOR

        /// <summary>
        /// Especifica identificador
        /// S (Sí) - N (No) 
        /// </summary>
        /// <example>S</example>
        [ApiDtoExample("S")]
        public string EspecificaIdentificador { get; set; } // ID_ESPECIFICA_IDENTIFICADOR

        /// <summary>
        /// Cantidad de producto para el pedido. En caso de confirmación de mercadería preparada, refiere a la cantidad preparada. En caso de cierre de egreso, refiere a la cantidad expedida en el camión.
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        [Required]
        public decimal CantidadProducto { get; set; } // QT_PRODUTO

        /// <summary>
        /// Información adicional
        /// </summary>
        /// <example></example>
        public string Memo { get; set; } // DS_MEMO

        /// <summary>
        /// Serializado
        /// </summary>
        /// <example></example>
        public string Serializado { get; set; } // VL_SERIALIZADO_1

        public List<PedidoDetalleDuplicadoRequest> Duplicados { get; set; }

        public PedidoDetalleRequest()
        {
            Duplicados = new List<PedidoDetalleDuplicadoRequest>();
        }
    }
}