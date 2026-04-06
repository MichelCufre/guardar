using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Extension;

namespace WIS.Domain.Services
{
    public class AgendaServiceContext : ServiceContext, IAgendaServiceContext
    {
        protected List<Agenda> _agendas = new List<Agenda>();

        public HashSet<string> TiposAgente { get; set; } = new HashSet<string>();
        public HashSet<string> TiposReferencia { get; set; } = new HashSet<string>();
        public HashSet<string> TiposReferenciaRecepcion { get; set; } = new HashSet<string>();
        public HashSet<string> TiposReferenciaAgenteRecepcion { get; set; } = new HashSet<string>();
        public HashSet<string> Predios { get; set; } = new HashSet<string>();
        public Dictionary<string, Dictionary<int, string>> PuertasPorPredio { get; set; } = new Dictionary<string, Dictionary<int, string>>();
        public HashSet<int> ReferenciasConSaldo { get; set; } = new HashSet<int>();
        public HashSet<string> TiposRecepcionReferencia { get; set; } = new HashSet<string>();
        public Dictionary<string, string> Agentes { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, EmpresaRecepcionTipo> TiposRecepcionExternos { get; set; } = new Dictionary<string, EmpresaRecepcionTipo>();
        public Dictionary<string, ReferenciaRecepcion> Referencias { get; set; } = new Dictionary<string, ReferenciaRecepcion>();
        public Dictionary<int, string> ReferenciaIds { get; set; } = new Dictionary<int, string>();


        public AgendaServiceContext(IUnitOfWork uow, List<Agenda> agendas, int userId, int empresa) : base(uow, userId, empresa)
        {
            _agendas = agendas;
        }

        public async override Task Load()
        {
            await base.Load();

            var keysAgentes = new Dictionary<string, Agente>();
            var keysReferencias = new Dictionary<string, ReferenciaRecepcion>();
            var idsReferencias = new HashSet<int>();

            foreach (var ta in _uow.AgenteRepository.GetTiposAgentes())
            {
                TiposAgente.Add(ta);
            }

            foreach (var tr in _uow.ReferenciaRecepcionRepository.GetReferenciaRecepcionTipos())
            {
                TiposReferencia.Add(tr.Tipo);
            }

            foreach (var ert in _uow.RecepcionTipoRepository.GetRecepcionTiposHabilitadosByEmpresa(Empresa))
            {
                var tipoRecepcion = ert.RecepcionTipoInterno;
                var tipoReferencia = tipoRecepcion.TipoReferencia;
                TiposRecepcionExternos[tipoRecepcion.Tipo] = ert;
                TiposRecepcionReferencia.Add($"{tipoRecepcion.Tipo}.{tipoReferencia}");
                TiposReferenciaRecepcion.Add(tipoReferencia);
                TiposReferenciaAgenteRecepcion.Add($"{tipoReferencia}.{tipoRecepcion.TipoAgente}");
            }

            foreach (var p in _uow.PredioRepository.GetPrediosUsuario(UserId))
            {
                Predios.Add(p.Numero);
                PuertasPorPredio[p.Numero] = new Dictionary<int, string>();
            }

            foreach (var p in _uow.PuertaEmbarqueRepository.GetPuertasPredio(Predios))
            {
                PuertasPorPredio[p.NumPredio][p.Id] = p.Tipo;
            }

            foreach (var a in _agendas)
            {
                var keyAgente = $"{a.TipoAgente}.{a.CodigoAgente}.{Empresa}";
                keysAgentes[keyAgente] = new Agente() { Tipo = a.TipoAgente.Truncate(3), Codigo = a.CodigoAgente.Truncate(40), Empresa = Empresa };
            }

            foreach (var a in _uow.AgenteRepository.GetAgentes(keysAgentes.Values))
            {
                Agentes[($"{a.Tipo}.{a.Codigo}")] = a.CodigoInterno;
            }

            foreach (var a in _agendas)
            {
                var cliente = Agentes.GetValueOrDefault($"{a.TipoAgente}.{a.CodigoAgente}", string.Empty);

                if (!string.IsNullOrEmpty(cliente))
                {
                    var keyReferencia = $"{a.NumeroDocumento}.{Empresa}.{a.TipoReferenciaId}.{cliente}";
                    keysReferencias[keyReferencia] = new ReferenciaRecepcion()
                    {
                        Numero = a.NumeroDocumento.Truncate(20),
                        IdEmpresa = Empresa,
                        TipoReferencia = a.TipoReferenciaId.Truncate(6),
                        CodigoCliente = cliente
                    };
                }
            }

            var referencias = _uow.ReferenciaRecepcionRepository.GetReferencias(keysReferencias.Values);

            foreach (var r in referencias)
            {
                var key = $"{r.Numero}.{r.IdEmpresa}.{r.TipoReferencia}.{r.CodigoCliente}";
                idsReferencias.Add(r.Id);
                Referencias[key] = r;
                ReferenciaIds[r.Id] = key;
            }

            foreach (var r in _uow.ReferenciaRecepcionRepository.GetReferenciasConSaldo(idsReferencias))
            {
                ReferenciasConSaldo.Add(r);
            }
        }

        public virtual bool ExisteTipoAgente(string tipoAgente)
        {
            return TiposAgente.Contains(tipoAgente);
        }

        public virtual Agente GetAgente(string codigo, int empresa, string tipo)
        {
            var cliente = Agentes.GetValueOrDefault($"{tipo}.{codigo}", string.Empty);

            if (string.IsNullOrEmpty(cliente))
            {
                return null;
            }

            return new Agente()
            {
                Codigo = codigo,
                CodigoInterno = cliente,
                Empresa = empresa,
                Tipo = tipo
            };
        }

        public virtual ReferenciaRecepcion GetReferencia(string referencia, int empresa, string tipoReferencia, string cliente)
        {
            var key = $"{referencia}.{empresa}.{tipoReferencia}.{cliente}";
            return Referencias.GetValueOrDefault(key, null);
        }

        public virtual EmpresaRecepcionTipo GetRecepcionTipoExternoByInterno(int empresa, string tipoRecepcion)
        {
            return TiposRecepcionExternos.GetValueOrDefault(tipoRecepcion, null);
        }

        public virtual bool TipoRecCompatibleTpReferencia(string tipoRecepcion, string tipoReferencia)
        {
            return TiposRecepcionReferencia.Contains($"{tipoRecepcion}.{tipoReferencia}");
        }

        public virtual bool ReferenciaSaldoDisponible(ReferenciaRecepcion referencia)
        {
            return ReferenciasConSaldo.Contains(referencia.Id);
        }

        public virtual bool ExisteTipoReferencia(string tipo)
        {
            return TiposReferencia.Contains(tipo);
        }

        public virtual bool ExisteTpRefTpRecepcion(string tipoReferencia)
        {
            return TiposReferenciaRecepcion.Contains(tipoReferencia);
        }

        public virtual bool ExisteTpRefTpAgente(string tipoReferencia, string tipoAgente)
        {
            return TiposReferenciaAgenteRecepcion.Contains($"{tipoReferencia}.{tipoAgente}");
        }

        public virtual bool ExistePredio(string predio)
        {
            return Predios.Contains(predio);
        }

        public bool ExistePuertaIn(string predio, short puerta)
        {
            if (!PuertasPorPredio.TryGetValue(predio, out var puertas))
                return false;

            return new[]
            {
                TipoPuertaEmbarqueDb.Entrada,
                TipoPuertaEmbarqueDb.EntradaSalida
            }
            .Contains(puertas.GetValueOrDefault(puerta));
        }
    }
}