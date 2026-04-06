using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class PedidoAnuladoRequest
    {
        /// <summary>
        /// Número de pedido
        /// </summary>
        /// <example></example>
        [ApiDtoExample("PED01")]
        public string Pedido { get; set; } // NU_PEDIDO

        /// <summary>
        /// Código de la empresa
        /// </summary>
        /// <example></example>
        [ApiDtoExample("1")]
        public int? Empresa { get; set; } // CD_EMPRESA

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

        public List<PedidoAnuladoDetalleRequest> Detalles { get; set; }

        public PedidoAnuladoRequest()
        {
            Detalles = new List<PedidoAnuladoDetalleRequest>();
        }
    }
}