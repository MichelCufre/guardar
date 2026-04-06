using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class StockRequest
    {
        /// <summary>
        /// Código del producto
        /// </summary>
        /// <example>PROD-1</example>
        [ApiDtoExample("PROD-1")]
        [Required]
        public string Producto { get; set; } // CD_PRODUTO

        /// <summary>
        /// Stock disponible en el local (elegible)
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        [Required]
        public int StockDisponible { get; set; } // STOCK_DISPONIBLE

        /// <summary>
        /// Stock disponible + stock en camiones, bloqueados y otros.
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        [Required]
        public int StockGeneral { get; set; } // STOCK_GENERAL
    }
}