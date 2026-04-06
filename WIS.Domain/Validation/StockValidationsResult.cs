using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.General;

namespace WIS.Domain.Validation
{
    public class StockValidationsResult
    {
        public ValidationsResult ValidationsResult { get; set; }
        public ConsultaStockResponse StockResult { get; set; }

        public StockValidationsResult()
        {
            ValidationsResult = new ValidationsResult();
        }
    }
}
