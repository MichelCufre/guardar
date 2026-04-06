using System.Collections.Generic;

namespace WIS.Domain.General.API.Dtos.Salida
{
    public class ConsultaStockResponse
    {        
        public List<StockResponse> Stock { get; set; }

        public ConsultaStockResponse()
        {
            Stock = new List<StockResponse>();
        }
    }
}
