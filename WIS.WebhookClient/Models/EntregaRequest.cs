using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class EntregaRequest
    {
        /// <summary>
        /// Número de entrega
        /// </summary>
        /// <example>1111</example>
        [ApiDtoExample("1111")]
        public int NumeroEntrega { get; set; } // NU_ENTREGA

        /// <summary>
        /// Tipo de entrega
        /// </summary>
        public string TipoEntrega { get; set; } // TP_ENTREGA

        /// <summary>
        /// Drecipción de la entrega
        /// </summary>
        /// <example></example>      
        public string DescripcionEntrega { get; set; } // DS_ENTREGA

        /// <summary>
        /// Código del agente
        /// </summary>
        /// <example>LOCAL11</example>
        [ApiDtoExample("LOCAL11")]
        public string CodigoAgente { get; set; } // CD_AGENTE

        /// <summary>
        /// Tipo de agente 
        /// PRO (Proveedor) - CLI (Cliente)
        /// </summary>
        /// <example>CLI</example>
        [ApiDtoExample("CLI")]
        public string TipoAgente { get; set; } // TP_AGENTE

        /// <summary>
        /// Código de la empresa
        /// </summary>
        /// <example>10</example>
        [ApiDtoExample("10")]
        public int? Empresa { get; set; } // CD_EMPRESA

        /// <summary>
        /// Código de barras del producto
        /// </summary>
        /// <example>1111</example>
        [ApiDtoExample("1111")]
        public string CodigoBarras { get; set; } // CD_BARRAS

        /// <summary>
        /// Número de contenedor
        /// </summary>
        /// <example>20000</example>
        [ApiDtoExample("20000")]
        [Required]
        public int NumeroContenedor { get; set; } // NU_CONTENEDOR

        /// <summary>
        /// Número de preparación
        /// </summary>
        /// <example>1111</example>
        [ApiDtoExample("1111")]
        [Required]
        public int? NumeroPreparacion { get; set; } // NU_PREPARACION

        /// <summary>
        /// Agrupación
        /// </summary>
        /// <example></example>
        public string Agrupacion { get; set; } // VL_AGRUPACION_ENTREGA

        /// <summary>
        /// Código de punto de entrega
        /// </summary>
        /// <example></example>
        public string PuntoEntrega { get; set; } // CD_PUNTO_ENTREGA

        /// <summary>
        /// Información adicional
        /// </summary>
        /// <example></example>
        public string Anexo { get; set; } // DS_ANEXO
    }
}
