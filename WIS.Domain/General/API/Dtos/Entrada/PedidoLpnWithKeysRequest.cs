using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class PedidoLpnWithKeysRequest: PedidoLpnRequest
    {
        /// <summary>
        /// Requerido - Valor alfanumérico que representa un pedido en el sistema, conforma un identificador único junto al código de agente, tipo de agente y la empresa a la que pertenece.
        /// </summary>
        /// <example>PED100</example>
        [ApiDtoExample("PED100")]
        public string NroPedido { get; set; }

        /// <summary>
        /// Requerido - Código del agente para el cual se realiza el pedido
        /// </summary>
        /// <example>AGE01</example>
        [ApiDtoExample("AGE01")]
        public string CodigoAgente { get; set; }

        /// <summary>
        /// Requerido - Tipo de agente 
        /// PRO (Proveedor) - CLI (Cliente)
        /// </summary>
        /// <example>PRO</example>
        [ApiDtoExample("PRO")]
        public string TipoAgente { get; set; }
    }
}
