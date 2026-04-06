using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Recepcion;

namespace WIS.Domain.Services.Interfaces
{
    public interface IAgendaServiceContext : IServiceContext
    {
        HashSet<string> TiposAgente { get; set; }
        HashSet<string> TiposReferencia { get; set; }
        HashSet<string> TiposReferenciaRecepcion { get; set; }
        HashSet<string> TiposReferenciaAgenteRecepcion { get; set; }
        HashSet<string> Predios { get; set; }
        Dictionary<string, Dictionary<int, string>> PuertasPorPredio { get; set; }
        HashSet<int> ReferenciasConSaldo { get; set; }
        HashSet<string> TiposRecepcionReferencia { get; set; }
        Dictionary<string, string> Agentes { get; set; }
        Dictionary<string, EmpresaRecepcionTipo> TiposRecepcionExternos { get; set; }
        Dictionary<string, ReferenciaRecepcion> Referencias { get; set; }
        Dictionary<int, string> ReferenciaIds { get; set; }

        Task Load();

        bool ExistePredio(string predio);
        bool ExistePuertaIn(string predio, short puerta);
        bool ExisteTipoAgente(string tipoAgente);
        bool ExisteTipoReferencia(string tipo);
        bool ExisteTpRefTpAgente(string tipoReferencia, string tipoAgente);
        bool ExisteTpRefTpRecepcion(string tipoReferencia);
        Agente GetAgente(string codigo, int empresa, string tipo);
        EmpresaRecepcionTipo GetRecepcionTipoExternoByInterno(int empresa, string tipoRecepcion);
        ReferenciaRecepcion GetReferencia(string referencia, int empresa, string tipoReferencia, string cliente);
        bool ReferenciaSaldoDisponible(ReferenciaRecepcion referencia);
        bool TipoRecCompatibleTpReferencia(string tipoRecepcion, string tipoReferencia);
    }
}