using WIS.Common.API.Attributes;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class StockResponse
    {
        /// <summary>
        /// Código del producto
        /// </summary>
        /// <example>PROD-1</example>
        [ApiDtoExample("PROD-1")]
        public string Producto { get; set; } // CD_PRODUTO

        /// <summary>
        /// Stock disponible + stock en camiones, bloqueados y otros.
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        public decimal StockGeneral { get; set; } // STOCK_GENERAL

        /// <summary>
        /// Stock disponible
        /// </summary>
        /// <example>100</example>
        [ApiDtoExample("100")]
        public decimal StockDisponible { get; set; } // STOCK_DISPONIBLE


    }
}