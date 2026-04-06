using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class ContenedorDetalleRequest
    {
        /// <summary>
        /// Número de pedido
        /// </summary>
        /// <example>PED1</example>
        [ApiDtoExample("PED1")]
        [Required]
        public string Pedido { get; set; } // NU_PEDIDO

        /// <summary>
        /// Código del agente
        /// </summary>
        /// <example>1555AAA</example>
        [ApiDtoExample("155AAA")]
        [Required]
        public string CodigoAgente { get; set; } // CD_AGENTE

        /// <summary>
        /// Tipo de agente 
        /// PRO (Proveedor) - CLI (Cliente)
        /// </summary>
        /// <example>CLI</example>
        [ApiDtoExample("CLI")]
        [Required]
        public string TipoAgente { get; set; } // TP_AGENTE

        /// <summary>
        /// Código del producto
        /// </summary>
        /// <example>PR1AAA</example>
        [ApiDtoExample("PR1AAA")]
        [Required]
        public string Producto { get; set; } // CD_PRODUTO

        /// <summary>
        /// Identificador del producto (serie o lote)
        /// </summary>
        /// <example>LOTEAAAAA</example>
        [ApiDtoExample("LOTEAAAA")]
        public string Identificador { get; set; } // NU_IDENTIFICADOR

        /// <summary>
        /// Fecha de vencimiento del pickeo
        /// </summary>
        /// <example>10/5/2018</example>
        [ApiDtoExample("10/5/2018")]
        public string FechaVencimientoPickeo { get; set; } // DT_VENCIMIENTO_PICKEO

        /// <summary>
        /// Avería pickeo 
        /// S (Sí) - N (No) 
        /// </summary>
        /// <example>N</example>
        [ApiDtoExample("N")]
        public string AveriaPickeo { get; set; } // ID_AVERIA_PICKEO

        /// <summary>
        /// Cantidad preparada 
        /// </summary>
        /// <example>120</example>
        [Required]
        [ApiDtoExample("120")]
        public decimal CantidadPreparada { get; set; } // QT_PREPARADO
    }
}