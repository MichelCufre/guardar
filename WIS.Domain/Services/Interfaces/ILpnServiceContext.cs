using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Parametrizacion;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services.Interfaces
{
    public interface ILpnServiceContext : IServiceContext
    {
        List<Lpn> _lpns { get; }

        List<Lpn> Lpns { get; set; }
        List<LpnBarras> LpnBarras { get; set; }
        HashSet<string> TipoBarrasLpn { get; set; }
        Dictionary<string, LpnTipo> TiposLpn { get; set; }
        Dictionary<string, Atributo> Atributos { get; set; }
        Dictionary<string, Producto> Productos { get; set; }
        Dictionary<short, AtributoValidacion> AtributosValidacion { get; set; }
        Dictionary<string, List<DominioDetalle>> Dominios { get; set; }
        Dictionary<string, LpnTipoAtributo> TipoLpnAtributos { get; set; }
        Dictionary<string, LpnTipoAtributoDet> TipoLpnAtributosDetalle { get; set; }
        Dictionary<string, List<LpnTipoAtributo>> TipoLpnAtributosAsociados { get; set; }
        Dictionary<string, List<LpnTipoAtributoDet>> TipoLpnAtributosDetalleAsociados { get; set; }
        Dictionary<int, List<AtributoValidacionAsociada>> AtributosValidacionesAsociadas { get; set; }
        Dictionary<long, List<LpnAtributo>> LpnAtributosCabezal { get; set; }

        Task Load();

        bool AnyTipoLpnAtributoDet(string tipo);
        bool ExisteDominio(string cdDominio, string nuDominio);
        bool ExisteLpnActivo(string idExterno, string tipo);
        bool ExisteLpnBarraActivo(string cdBarras);
        bool ExistenAtributosDetFaltantes(string tipo, List<string> atributos);
        bool ExistenAtributosFaltantes(string tipo, List<string> atributos);
        bool ExisteTipoBarra(string tipo);
        bool ExisteTipoLpnAtributo(string tipo, string nombreAtributo);
        bool ExisteTipoLpnAtributoDet(string tipo, string nombreAtributo);
        Atributo GetAtributo(string nombre);
        List<LpnAtributo> GetAtributosCabezal(long numeroLPN);
        AtributoValidacion GetAtributoValidacion(short idValidacion);
        Producto GetProducto(int empresa, string codigo);
        LpnTipo GetTipoLpn(string tipo);
        Lpn GetUltimoLpn(string idExterno, string tipo);
        List<AtributoValidacionAsociada> GetValidacionesAsociadas(int idAtributo);
    }
}