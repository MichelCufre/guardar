using ClosedXML.Excel;
using WIS.Extension;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class AgenteServiceContext : ServiceContext, IAgenteServiceContext
    {
        protected List<Agente> _agentes = new List<Agente>();

        public HashSet<short> Rutas { get; set; } = new HashSet<short>();
        public HashSet<string> TiposAgentes { get; set; } = new HashSet<string>();
        public HashSet<string> GruposConsulta { get; set; } = new HashSet<string>();
        public HashSet<string> Paises { get; set; } = new HashSet<string>();
        public Dictionary<string, string> SubdivisionesPaises { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, PaisSubdivisionLocalidad> LocalidadesSubdivisiones { get; set; } = new Dictionary<string, PaisSubdivisionLocalidad>();
        public Dictionary<string, Agente> Agentes { get; set; } = new Dictionary<string, Agente>();

        public List<ClienteDiasValidezVentana> ClienteDiasValidezVentanas { get; set; } = new List<ClienteDiasValidezVentana>();

        public AgenteServiceContext(IUnitOfWork uow, List<Agente> agentes, int userId, int empresa) : base(uow, userId, empresa)
        {
            _agentes = agentes;
        }

        public async override Task Load()
        {
            string sinEspecificar = "S/E";
            bool addSinEspecificar = false;

            await base.Load();

            var keysRutas = new Dictionary<string, Ruta>();
            var keysSubdivisiones = new Dictionary<string, PaisSubdivision>();
            var keysLocalidades = new Dictionary<string, PaisSubdivisionLocalidad>();
            var keysAgentes = new Dictionary<string, Agente>();

            foreach (var ta in _uow.AgenteRepository.GetTiposAgentes())
            {
                TiposAgentes.Add(ta);
            }

            foreach (var p in _uow.PaisRepository.GetPaises())
            {
                Paises.Add(p.Id);
            }

            foreach (var gc in _uow.GrupoConsultaRepository.GetGruposConsulta())
            {
                GruposConsulta.Add(gc.Id);
            }

            foreach (var a in _agentes)
            {
                if (a.RutaId.HasValue)
                {
                    var keyRuta = $"{a.RutaId.Value}";
                    keysRutas[keyRuta] = new Ruta() { Id = a.RutaId.Value };
                }

                if (!string.IsNullOrEmpty(a.SubdivisionId))
                {
                    var keysSubdivision = $"{a.SubdivisionId}";
                    keysSubdivisiones[keysSubdivision] = new PaisSubdivision() { Id = a.SubdivisionId.Truncate(15) };
                }
                else if (!string.IsNullOrEmpty(a.PaisId))
                {
                    var keysSubdivision = $"{a.PaisId.Truncate(2)}-{sinEspecificar}";
                    keysSubdivisiones[keysSubdivision] = new PaisSubdivision() { Id = keysSubdivision };
                    addSinEspecificar = true;
                }

                if (!string.IsNullOrEmpty(a.MunicipioId))
                {
                    var keyLocalidad = $"{a.MunicipioId}.{a.SubdivisionId}";
                    keysLocalidades[keyLocalidad] = new PaisSubdivisionLocalidad() { Codigo = a.MunicipioId.Truncate(20), CodigoSubDivicion = a.SubdivisionId.Truncate(15) };
                }

                if (addSinEspecificar)
                {
                    var keysSubdivision = $"{a.PaisId.Truncate(2)}-{sinEspecificar}";
                    var keyLocalidad = $"{sinEspecificar}.{keysSubdivision}";
                    keysLocalidades[keyLocalidad] = new PaisSubdivisionLocalidad() { Codigo = sinEspecificar, CodigoSubDivicion = keysSubdivision };
                }

                var keyAgente = $"{a.Codigo}.{a.Tipo}.{a.Empresa}";
                keysAgentes[keyAgente] = new Agente() { Codigo = a.Codigo.Truncate(40), Tipo = a.Tipo.Truncate(3), Empresa = a.Empresa };
            }

            var rutaParam = GetParametro(ParamManager.IE_507_CD_ROTA);
            if (short.TryParse(rutaParam, out short ruta) && !keysRutas.ContainsKey(rutaParam))
                keysRutas.Add(rutaParam, new Ruta() { Id = ruta });

            foreach (var r in _uow.RutaRepository.GetRutas(keysRutas.Values))
            {
                Rutas.Add(r);
            }

            SubdivisionesPaises = _uow.PaisSubdivisionRepository.GetSubdivisionesPaises(keysSubdivisiones.Values);
            LocalidadesSubdivisiones = _uow.PaisSubdivisionLocalidadRepository.GetLocalidadesSubdivisiones(keysLocalidades.Values);

            var agentes = _uow.AgenteRepository.GetAgentes(keysAgentes.Values);

            foreach (var empresa in _agentes.Select(x => x.Empresa).Distinct())
            {
                ClienteDiasValidezVentanas.AddRange(_uow.ProduccionRepository.GetAllVentanasConfiguracionLiberacionByEmpresa(empresa));
            }

            foreach (var a in agentes)
            {
                var key = $"{a.Tipo}.{a.Codigo}";
                Agentes[key] = a;
            }
        }

        public virtual bool ExisteTipoAgente(string tipoAgente)
        {
            return TiposAgentes.Contains(tipoAgente);
        }

        public virtual bool ExisteRuta(short ruta)
        {
            return Rutas.Contains(ruta);
        }

        public virtual bool ExisteGrupoConsulta(string grupoConsulta)
        {
            return GruposConsulta.Contains(grupoConsulta);
        }

        public virtual bool ExistePais(string pais)
        {
            return Paises.Contains(pais);
        }

        public virtual PaisSubdivision GetSubdivision(string subdivision)
        {
            if (SubdivisionesPaises.ContainsKey(subdivision))
            {
                return new PaisSubdivision()
                {
                    Id = subdivision,
                    IdPais = SubdivisionesPaises[subdivision]
                };
            }

            return null;
        }

        public virtual PaisSubdivisionLocalidad GetLocalidadId(string localidad, string subdivision)
        {
            if (ExisteLocalidad(localidad, subdivision))
            {
                var keyLocalidad = $"{localidad}.{subdivision}";
                return LocalidadesSubdivisiones[keyLocalidad];

            }

            return null;
        }

        public virtual Agente GetAgente(string tipo, string codigo)
        {
            var key = $"{tipo}.{codigo}";

            return Agentes.GetValueOrDefault(key, null);
        }

        public virtual bool ExisteLocalidad(string localidad, string subdivision)
        {
            var keyLocalidad = $"{localidad}.{subdivision}";
            return LocalidadesSubdivisiones.ContainsKey(keyLocalidad);
        }
    }
}
