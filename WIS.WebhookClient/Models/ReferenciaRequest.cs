using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class ReferenciaRequest
    {
        /// <summary>
        ///  Identificador de la referencia para recepción (número de OC, de devolución, etc.)
        /// </summary>
        /// <example>1999</example>
        [ApiDtoExample("1999")]
        [Required]
        public string NumeroReferencia { get; set; } // NU_REFERENCIA

        /// <summary>
        /// Tipo de referencia
        /// OC (Orden de Compra) - RR (Referencia de Recepción) - OD (Orden de Devolución) - ODC (Devolución Canje)
        /// OC y RR corresponden al tipo de agente PRO
        /// OD y ODC corresponden al tipo de agente CLI
        /// </summary>
        /// <example>OD</example>
        [ApiDtoExample("OD")]
        [Required]
        public string TipoReferencia { get; set; }  // TP_REFERENCIA

        /// <summary>
        /// Tipo de agente 
        /// PRO (Proveedor) - CLI (Cliente)
        /// </summary>
        [ApiDtoExample("CLI")]
        [Required]
        public string TipoAgente { get; set; } // TP_AGENTE

        /// <summary>
        /// Código del agente
        /// </summary>
        [ApiDtoExample("LOC1111")]
        [Required]
        public string CodigoAgente { get; set; } // CD_AGENTE

        /// <summary>
        /// Información adicional devuelta en la confirmación de la recepción
        /// </summary>
        /// <example></example>
        public string Memo { get; set; } // DS_MEMO

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        public string Anexo1 { get; set; } // DS_ANEXO1

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        public string Anexo2 { get; set; } // DS_ANEXO2

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        public string Anexo3 { get; set; } // DS_ANEXO3

        /// <summary>
        /// Serializado
        /// </summary>
        /// <example></example>
        public string Serializado { get; set; } // VL_SERIALIZADO

        /// <summary>
        /// Número de predio
        /// </summary>
        /// <example></example>
        [ApiDtoExample("1")]
        public string Predio { get; set; } // NU_PREDIO

        public List<ReferenciaDetalleRequest> Detalles { get; set; }

        public ReferenciaRequest() 
        {
            Detalles = new List<ReferenciaDetalleRequest>();
        }
    }
}
