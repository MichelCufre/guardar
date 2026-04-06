using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class ReferenciaDetalleRequest
    {
        /// <summary>
        /// Identificación en el ERP o sistema externo para la línea de la referencia.  
        /// Si está definido después en la confirmación, se retorna para un matcheo en el sistema externo.
        /// </summary>
        /// <example>LIN1</example>
        [ApiDtoExample("L1N1")]
        public string IdLineaSistemaExterno { get; set; } // ID_LINEA_SISTEMA_EXTERNO

        /// <summary>
        /// Código del producto
        /// </summary>
        /// <example>PRO-1</example>
        [ApiDtoExample("PRO-1")]
        [Required]
        public string Producto { get; set; } // CD_PRODUTO

        /// <summary>
        /// Serie o Lote del producto en caso que lo maneje.
        /// Es un valor opcional y WIS lo manejará en modalidad automática si no viene especificado
        /// </summary>
        [ApiDtoExample("AAA")]
        public string Identificador { get; set; } // NU_IDENTIFICADOR

        /// <summary>
        /// Cantidad de la Referencia
        /// </summary>
        [ApiDtoExample("100")]
        [Required]
        public decimal CantidadReferencia { get; set; } // QT_REFERENCIA

        /// <summary>
        /// Cantidad consumida
        /// </summary>
        [ApiDtoExample("100")]
        public decimal? CantidadConsumida { get; set; } // QT_CONSUMIDA

        /// <summary>
        /// Información adicional
        /// </summary>
        /// <example></example>
        public string Anexo { get; set; } // DS_ANEXO1

        /// <summary>
        /// Cantidad consumida agendada
        /// </summary>
        /// <example></example>
        [ApiDtoExample("200")]
        public decimal? CantidadConsumidaAgenda { get; set; } // QT_CONSUMIDA_AGENDA

        /// <summary>
        /// Fecha de vencimiento
        /// </summary>
        /// <example>10/10/2022</example>
        [ApiDtoExample("10/10/2022")]
        public string FechaVencimiento { get; set; } // DT_VENCIMIENTO
    }
}
