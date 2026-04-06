using System;
using System.Collections.Generic;
using System.Text;
using WIS.Common.API.Attributes;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ReferenciaRecepcionDetalleWithKeysRequest : ReferenciaRecepcionDetalleRequest
    {
        /// <summary>
        /// Número de referencia
        /// </summary>
        /// <example>REF01</example>
        [ApiDtoExample("REF01")]
        public string Referencia { get; set; }

        /// <summary>
        /// Tipo de referencia
        /// OC (Orden de Compra) - RR (Referencia de Recepción) - OD (Orden de Devolución) - ODC (Devolución Canje)
        /// OC y RR corresponden al tipo de agente PRO
        /// OD y ODC corresponden al tipo de agente CLI
        /// </summary>
        /// <example>OC</example>
        [ApiDtoExample("OC")]
        public string TipoReferencia { get; set; }

        /// <summary>
        /// Código de agente
        /// </summary>
        /// <example>AGE01</example>
        [ApiDtoExample("AGE01")]
        public string CodigoAgente { get; set; }

        /// <summary>
        /// El tipo de Agente aceptado depende del tipo de referencia.
        /// Tipos de referencia de devolución, tipo de agente CLI (Cliente), tipos de referencia normales PRO (Proveedor)
        /// </summary>
        /// <example>PRO</example>
        [ApiDtoExample("PRO")]
        public string TipoAgente { get; set; }
    }
}
