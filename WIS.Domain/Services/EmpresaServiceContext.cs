using WIS.Extension;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class EmpresaServiceContext : ServiceContext, IEmpresaServiceContext
    {
        protected List<Empresa> _empresas = new List<Empresa>();

        public List<int> UsuariosAsignables { get; set; } = new List<int>();
        public List<string> TiposRecepcion { get; set; } = new List<string>();
        public Dictionary<string, List<string>> ReportesTipoRecepcion { get; set; } = new Dictionary<string, List<string>>();
        public HashSet<string> GruposConsulta { get; set; } = new HashSet<string>();
        public HashSet<string> Paises { get; set; } = new HashSet<string>();
        public Dictionary<int, Empresa> Empresas { get; set; } = new Dictionary<int, Empresa>();
        public Dictionary<string, string> SubdivisionesPaises { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, PaisSubdivisionLocalidad> LocalidadesSubdivisiones { get; set; } = new Dictionary<string, PaisSubdivisionLocalidad>();

        public EmpresaServiceContext(IUnitOfWork uow, List<Empresa> empresas, int userId, int empresaEjecucion) : base(uow, userId, empresaEjecucion)
        {
            _empresas = empresas;
        }

        public async override Task Load()
        {
            string sinEspecificar = "S/E";
            bool addSinEspecificar = false;
            await base.Load();

            var keysSubdivisiones = new Dictionary<string, PaisSubdivision>();
            var keysLocalidades = new Dictionary<string, PaisSubdivisionLocalidad>();
            var paramTpRec = GetParametro(ParamManager.IE_522_TIPOS_RECEPCION) ?? "";

            UsuariosAsignables.AddRange(_uow.EmpresaRepository.GetUsuariosAsignables());
            ReportesTipoRecepcion = _uow.EmpresaRepository.GetReportesTiposRecepcion(paramTpRec);
            TiposRecepcion.AddRange(_uow.EmpresaRepository.GetTiposRecepcion(paramTpRec));

            foreach (var p in _uow.PaisRepository.GetPaises())
            {
                Paises.Add(p.Id);
            }

            foreach (var gc in _uow.GrupoConsultaRepository.GetGruposConsulta())
            {
                GruposConsulta.Add(gc.Id);
            }

            foreach (var e in _empresas)
            {
                if (!string.IsNullOrEmpty(e.SubdivisionId))
                {
                    var keysSubdivision = $"{e.SubdivisionId}";
                    keysSubdivisiones[keysSubdivision] = new PaisSubdivision() { Id = e.SubdivisionId.Truncate(20) };
                }
                else if (!string.IsNullOrEmpty(e.PaisId))
                {
                    var keysSubdivision = $"{e.PaisId.Truncate(2)}-{sinEspecificar}";
                    keysSubdivisiones[keysSubdivision] = new PaisSubdivision() { Id = keysSubdivision };
                    addSinEspecificar = true;
                }

                if (!string.IsNullOrEmpty(e.MunicipioId))
                {
                    var keyLocalidad = $"{e.MunicipioId}.{e.SubdivisionId}";
                    keysLocalidades[keyLocalidad] = new PaisSubdivisionLocalidad() { Codigo = e.MunicipioId.Truncate(10), CodigoSubDivicion = e.SubdivisionId.Truncate(20) };
                }

                if (addSinEspecificar)
                {
                    var keysSubdivision = $"{e.PaisId.Truncate(2)}-{sinEspecificar}";
                    var keyLocalidad = $"{sinEspecificar}.{keysSubdivision}";
                    keysLocalidades[keyLocalidad] = new PaisSubdivisionLocalidad() { Codigo = sinEspecificar, CodigoSubDivicion = keysSubdivision };
                }
            }

            SubdivisionesPaises = _uow.PaisSubdivisionRepository.GetSubdivisionesPaises(keysSubdivisiones.Values);
            LocalidadesSubdivisiones = _uow.PaisSubdivisionLocalidadRepository.GetLocalidadesSubdivisiones(keysLocalidades.Values);

            var empresas = _uow.EmpresaRepository.GetEmpresas(_empresas);

            foreach (var e in empresas)
            {
                Empresas[e.Id] = e;
            }
        }

        public virtual bool ExistePais(string pais)
        {
            return Paises.Contains(pais);
        }

        public virtual Empresa GetEmpresa(int id)
        {
            return Empresas.GetValueOrDefault(id, null);
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

        public virtual bool ExisteLocalidad(string localidad, string subdivision)
        {
            var keyLocalidad = $"{localidad}.{subdivision}";
            return LocalidadesSubdivisiones.ContainsKey(keyLocalidad);
        }

        public virtual List<string> GetReportesTipoRecepcion(string tpRecepcion)
        {
            if (!ReportesTipoRecepcion.ContainsKey(tpRecepcion))
                return new List<string>();

            return ReportesTipoRecepcion[tpRecepcion];
        }
    }
}
