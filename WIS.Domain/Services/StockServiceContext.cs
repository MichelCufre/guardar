using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services
{
    public class StockServiceContext : ServiceContext, IStockServiceContext
    {
        protected FiltrosStock _filtros = new FiltrosStock();

        public HashSet<int> Familias { get; set; } = new HashSet<int>();
        public HashSet<short> Ramos { get; set; } = new HashSet<short>();
        public HashSet<string> Clases { get; set; } = new HashSet<string>();
        public HashSet<string> Predios { get; set; } = new HashSet<string>();
        public HashSet<string> Ubicaciones { get; set; } = new HashSet<string>();
        public HashSet<string> GruposConsulta { get; set; } = new HashSet<string>();

        public StockServiceContext(IUnitOfWork uow, FiltrosStock filtros, int userId, int empresa) : base(uow, userId, empresa)
        {
            _filtros = filtros;
        }

        public async override Task Load()
        {
            await base.Load();

            foreach (var u in _uow.ClaseRepository.GetClases())
            {
                Clases.Add(u.Id);
            }

            foreach (var f in _uow.ProductoFamiliaRepository.GetProductoFamilias())
            {
                Familias.Add(f.Id);
            }

            foreach (var r in _uow.ProductoRamoRepository.GetProductoRamos())
            {
                Ramos.Add(r.Id);
            }

            foreach (var gc in _uow.GrupoConsultaRepository.GetGruposConsulta())
            {
                GruposConsulta.Add(gc.Id);
            }

            foreach (var p in _uow.PredioRepository.GetPrediosUsuario(UserId))
            {
                Predios.Add(p.Numero);
            }

            foreach (var p in _uow.UbicacionRepository.GetUbicaciones())
            {
                Ubicaciones.Add(p.Id);
            }
        }

        public virtual bool ExistePredio(string predio)
        {
            return Predios.Contains(predio);
        }
        public virtual bool ExisteClase(string clase)
        {
            return Clases.Contains(clase);
        }
        public virtual bool ExisteFamilia(int familia)
        {
            return Familias.Contains(familia);
        }
        public virtual bool ExisteRamo(short ramo)
        {
            return Ramos.Contains(ramo);
        }
        public virtual bool ExisteUbicacion(string ubicacion)
        {
            return Ubicaciones.Contains(ubicacion);
        }
        public virtual bool ExisteGrupoConsulta(string grupoConsulta)
        {
            return GruposConsulta.Contains(grupoConsulta);
        }
    }
}
