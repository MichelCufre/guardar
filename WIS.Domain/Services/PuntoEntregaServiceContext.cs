using WIS.Extension;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Tracking.Models;

namespace WIS.Domain.Services
{
    public class PuntoEntregaServiceContext : ServiceContext, IPuntoEntregaServiceContext
    {
        protected PuntoEntregaAgentes _puntoEntrega = new PuntoEntregaAgentes();

        public Dictionary<string, Agente> Agentes { get; set; } = new Dictionary<string, Agente>();
        public List<Pedido> PedidosPendientes { get; set; } = new List<Pedido>();
        public HashSet<string> TiposAgente { get; set; } = new HashSet<string>();
        public HashSet<int> Empresas { get; set; } = new HashSet<int>();
        public Ruta RutaZona { get; set; } = new Ruta();

        public PuntoEntregaServiceContext(IUnitOfWork uow, PuntoEntregaAgentes puntoEntrega, int userId) : base(uow, userId, 0)
        {
            _puntoEntrega = puntoEntrega;
        }

        public async override Task Load()
        {
            await base.Load();

            var keysAgentes = new Dictionary<string, Agente>();
            var keysEmpresas = new Dictionary<int, Empresa>();

            TiposAgente = _uow.AgenteRepository.GetTiposAgentes().ToHashSet();
            RutaZona = _uow.RutaRepository.GetRutaByZona(_puntoEntrega.Zona, false);

            foreach (var a in _puntoEntrega.Agentes)
            {
                if (!keysEmpresas.ContainsKey(a.CodigoEmpresa))
                    keysEmpresas[a.CodigoEmpresa] = new Empresa() { Id = a.CodigoEmpresa };

                var keyAgente = $"{a.Tipo}.{a.Codigo}.{a.CodigoEmpresa}";
                if (!keysAgentes.ContainsKey(keyAgente))
                    keysAgentes[keyAgente] = new Agente() { Tipo = a.Tipo.Truncate(3), Codigo = a.Codigo.Truncate(40), Empresa = a.CodigoEmpresa };
            }

            Empresas = _uow.EmpresaRepository.GetEmpresas(keysEmpresas.Values).Select(e => e.Id).ToHashSet();
            Agentes = _uow.AgenteRepository.GetAgentes(keysAgentes.Values).ToDictionary(c => $"{c.Tipo}.{c.Codigo}.{c.Empresa}", c => c);

            PedidosPendientes = _uow.PedidoRepository.GetPedidosPendientes(Agentes.Values, _puntoEntrega.CodigoPuntoEntrega).ToList();
        }

        public virtual bool ExisteEmpresa(int empresa)
        {
            return Empresas.Contains(empresa);
        }

        public virtual bool ExisteTipoAgente(string tipoAgente)
        {
            return TiposAgente.Contains(tipoAgente);
        }

        public virtual Agente GetAgente(string codigo, int empresa, string tipo)
        {
            return Agentes.GetValueOrDefault($"{tipo}.{codigo}.{empresa}", null);
        }
    }
}
