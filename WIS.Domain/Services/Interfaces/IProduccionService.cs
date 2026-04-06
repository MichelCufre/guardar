using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.DTOs;

namespace WIS.Domain.Services.Interfaces
{
    public interface IProduccionService
	{
        Task<IngresoProduccion> GetProduccion(string nroIngresoProduccion);
        
        Task<ValidationsResult> AgregarIngresos(List<IngresoProduccion> ingresos, int userId, List<IngresosGeneradosApiProduccion> ingresosGenerados);
	}
}
