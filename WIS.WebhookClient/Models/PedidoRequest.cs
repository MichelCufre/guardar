using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class PedidoRequest
    {
        /// <summary>
        /// Código de la empresa
        /// </summary>
        /// <example>10</example>
        [ApiDtoExample("10")]
        [Required]
        public int Empresa { get; set; } // CD_EMPRESA

        /// <summary>
        /// Tipo de agente 
        /// PRO (Proveedor) - CLI (Cliente)
        /// </summary>
        /// <example>CLI</example>
        [ApiDtoExample("CLI")]
        [Required]
        public string TipoAgente { get; set; } // TP_AGENTE

        /// <summary>
        /// Código del agente
        /// </summary>
        /// <example>LOCAL11</example>
        [ApiDtoExample("LOCAL11")]
        [Required]
        public string CodigoAgente { get; set; } // CD_AGENTE

        /// <summary>
        /// Número de pedido
        /// </summary>
        /// <example>ORDEN-100</example>
        [Required]
        [ApiDtoExample("ORDEN-100")]
        public string Pedido { get; set; } // NU_PEDIDO

        /// <summary>
        /// Código de origen
        /// </summary>
        /// <example>WPRE100</example>
        [ApiDtoExample("WPRE100")]
        public string CodigoOrigen { get; set; } // CD_ORIGEN

        /// <summary>
        /// Información adicional
        /// </summary>
        /// <example></example>
        public string Memo { get; set; } // DS_MEMO

        /// <summary>
        /// Información adicional
        /// </summary>
        /// <example></example>
        public string Memo1 { get; set; } // DS_MEMO_1

        /// <summary>
        /// Código de condición de liberación
        /// </summary>
        /// <example></example>
        public string CondicionLiberacion { get; set; } // CD_CONDICION_LIBERACION

        /// <summary>
        /// Número de predio.
        /// Este valor se notifica en el cabezal del camión y del pedido. 
        /// Como referencia debe tomarse siempre el del camión.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        public string Predio { get; set; } // NU_PREDIO

        /// <summary>
        /// Tipo de pedido
        /// </summary>
        /// <example>URG</example>
        [ApiDtoExample("URG")]
        public string TipoPedido { get; set; } // TP_PEDIDO

        /// <summary>
        /// Tipo de expedición
        /// </summary>
        /// <example>NORMAL</example>
        [ApiDtoExample("NORMAL")]
        public string TipoExpedicion { get; set; } // TP_EXPEDICION

        /// <summary>
        /// Destino de la expedición
        /// </summary>
        /// <example></example>
        public string Direccion { get; set; } // DS_ENDERECO

        /// <summary>
        /// Código de punto de entrega
        /// </summary>
        /// <example></example>
        public string PuntoEntrega { get; set; } // CD_PUNTO_ENTREGA

        /// <summary>
        /// Serializado
        /// </summary>
        /// <example></example>
        public string Serializado { get; set; } // VL_SERIALIZADO_1
        public List<PedidoDetalleRequest> Detalles { get; set; }

        public PedidoRequest()
        {
            Detalles = new List<PedidoDetalleRequest>();
        }
    }
}
