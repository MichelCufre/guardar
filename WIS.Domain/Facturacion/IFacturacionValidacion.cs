using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.Expedicion;
using WIS.Domain.Picking;

namespace WIS.Domain.Facturacion
{
    public interface IFacturacionValidacion
    {
        void Validate(Camion camion, Pedido pedido);
        bool IsValid(); //TODO: Comprobar que validacion se ejecuto
        ValidacionCamionResultado GetResult();
    }
}
