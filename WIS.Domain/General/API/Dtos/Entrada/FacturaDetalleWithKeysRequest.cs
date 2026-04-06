using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class FacturaDetalleWithKeysRequest : FacturaDetalleRequest
    {
        /// <summary>
        /// Número de factura
        /// </summary>
        /// <example>REF01</example>
        [ApiDtoExample("REF01")]
        public string Factura { get; set; }

        /// <summary>
        /// Numero de serie
        /// </summary>
        /// <example>OC</example>
        [ApiDtoExample("OC")]
        public string Serie { get; set; }

        /// <summary>
        /// Código de agente
        /// </summary>
        /// <example>AGE01</example>
        [ApiDtoExample("AGE01")]
        public string CodigoAgente { get; set; }

    }
}
