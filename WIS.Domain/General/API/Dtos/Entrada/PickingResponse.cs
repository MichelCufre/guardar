using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.Interfaces;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class PickingResponse : EntradaResponse
    {
        public PickingResponse(InterfazEjecucion interfaz) : base(interfaz)
        {
        }
    }
}
