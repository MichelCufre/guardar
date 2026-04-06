using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Interfaces;
using WIS.Domain.Produccion.DTOs;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ProduccionesResponse : EntradaResponse
    {
        public List<IngresosGeneradosApiProduccion> Ingresos { get; set; } = new List<IngresosGeneradosApiProduccion>();

        public ProduccionesResponse(InterfazEjecucion interfaz, List<IngresosGeneradosApiProduccion> ingresos) : base(interfaz)
        {
            Ingresos = ingresos;
        }
    }
}
