using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Picking;

namespace WIS.Domain.Services.Interfaces
{
    public interface IPuntoEntregaServiceContext : IServiceContext
    {
        Dictionary<string, Agente> Agentes { get; set; }
        List<Pedido> PedidosPendientes { get; set; }
        HashSet<string> TiposAgente { get; set; }
        HashSet<int> Empresas { get; set; }
        Ruta RutaZona { get; set; } 

        Task Load();

        bool ExisteEmpresa(int empresa);
        bool ExisteTipoAgente(string tipoAgente);
        Agente GetAgente(string codigo, int empresa, string tipo);
    }
}