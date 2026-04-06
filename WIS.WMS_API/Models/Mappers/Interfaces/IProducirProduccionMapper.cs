using System.Collections.Generic;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Interfaces;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Models;

namespace WIS.WMS_API.Models.Mappers.Interfaces
{
    public interface IProducirProduccionMapper
    {
        ProducirProduccion Map(ProducirProduccionRequest request);
    }
}
