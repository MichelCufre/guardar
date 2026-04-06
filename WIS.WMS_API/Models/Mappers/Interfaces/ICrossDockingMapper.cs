using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Interfaces;
using WIS.Domain.Picking.Dtos;

namespace WIS.WMS_API.Models.Mappers.Interfaces
{
    public interface ICrossDockingMapper
    {
        List<CrossDockingUnaFase> Map(CrossDockingUnaFaseRequest request);
    }
}
