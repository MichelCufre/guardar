using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Interfaces;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ConsumirProduccionResponse : EntradaResponse
    {
        public ConsumirProduccionResponse(InterfazEjecucion interfaz) : base(interfaz)
        {
        }
    }
}
