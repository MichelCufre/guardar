using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.Interfaces;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class TransferenciaStockResponse : EntradaResponse
    {
        public TransferenciaStockResponse(InterfazEjecucion interfaz) : base(interfaz)
        {
        }
    }
}
