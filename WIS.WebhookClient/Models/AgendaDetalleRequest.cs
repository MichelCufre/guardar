using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class AgendaDetalleRequest
    {
        /// <summary>
        /// Código del producto
        /// </summary>
        [ApiDtoExample("PRO01")]
        [Required]
        public string Producto { get; set; } // CD_PRODUTO

        /// <summary>
        /// Serie o Lote del producto en caso que lo maneje.
        /// Es un valor opcional y WIS lo manejará en modalidad automática si no viene especificado
        /// </summary>
        [ApiDtoExample("AAA")]
        public string Identificador { get; set; } // NU_IDENTIFICADOR

        /// <summary>
        /// Cantidad teórica 
        /// </summary>
        [ApiDtoExample("100")]
        [Required]
        public decimal CantidadTeorica { get; set; } // QT_AGENDADO_ORIGINAL

        /// <summary>
        /// Cantidad recibida 
        /// </summary>
        [ApiDtoExample("100")]
        public decimal? CantidadRecibida { get; set; } // QT_AGENDADO_ORIGINAL

        /// <summary>
        /// Fecha de vencimiento
        /// </summary>
        /// <example>10/10/2022</example>
        [ApiDtoExample("10/10/2022")]
        public string FechaVencimiento { get; set; } // DT_FABRICACAO
    }
}
