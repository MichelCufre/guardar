using System.Collections.Generic;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;

namespace WIS.WMS_API.Models.Mappers.Interfaces
{
    public interface IControlCalidadMapper
    {
        public List<ControlCalidadAPI> Map(ControlCalidadRequest request);
    }
}
