using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Interfaces;

namespace WIS.Domain.Services.Interfaces
{
	public interface IControlCalidadService
	{
		Task <ValidationsResult> AsignarControlCalidad (List <ControlCalidadAPI> criterios,int userId, int empresa);
		ControlCalidadResponse GetResponse (InterfazEjecucion ejecucion);
	}
}
