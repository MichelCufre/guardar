using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Picking;

namespace WIS.Domain.Facturacion
{
    public interface IConfiguracionValidacionFacturacion
    {
        void CargarValidaciones();
        List<IFacturacionValidacion> GetValidacionesEvaluar(Pedido pedido);
        List<IFacturacionValidacion> GetValidaciones();
        bool IsUnicoGrupoExpedicionRequerido(); //TODO: Quitar de aca
    }
}
