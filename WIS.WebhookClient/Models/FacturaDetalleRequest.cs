using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class FacturaDetalleRequest
    {
        /// <summary>
        /// Código del producto
        /// </summary>
        /// <example>PRO-1</example>
        [ApiDtoExample("PRO-1")]
        public string Producto { get; set; } // CD_PRODUTO

        /// <summary>
        /// Serie o Lote del producto en caso que lo maneje.
        /// Es un valor opcional y WIS lo manejará en modalidad automática si no viene especificado
        /// </summary>
        /// <example>AAA</example>
        [ApiDtoExample("AAAA")]
        public string Identificador { get; set; } // NU_IDENTIFICADOR

        /// <summary>
        /// Cantidad facturada
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        public decimal CantidadFacturada { get; set; } // QT_FACTURADA

        /// <summary>
        /// Cantidad validada
        /// </summary>
        [ApiDtoExample("100")]
        public decimal CantidadValidada { get; set; } // QT_VALIDADA

        /// <summary>
        /// Cantidad recibida
        /// </summary>
        [ApiDtoExample("100")]
        public decimal CantidadRecibida { get; set; } // QT_RECIBIDA

        /// <summary>
        /// Fecha de vencimiento
        /// </summary>
        /// <example>12/06/2016 13:19</example>
        [ApiDtoExample("12/06/2016 13:19")]
        public string FechaVencimiento { get; set; } // DT_VENCIMIENTO
        /// <summary>
        /// Importe unitario
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        public string ImporteUnitario { get; set; } // IM_UNITARIO_DIGITADO
    }
}
