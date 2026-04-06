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
    public class ProductoServiceContext : ServiceContext, IProductoServiceContext
    {
        protected List<Producto> _productos = new List<Producto>();

        public HashSet<string> Clases { get; set; } = new HashSet<string>();
        public HashSet<int> Familias { get; set; } = new HashSet<int>();
        public HashSet<short> Ramos { get; set; } = new HashSet<short>();
        public HashSet<string> UnidadesMedida { get; set; } = new HashSet<string>();
        public HashSet<short> Rotatividades { get; set; } = new HashSet<short>();
        public HashSet<string> NAMs { get; set; } = new HashSet<string>();
        public HashSet<string> GruposConsulta { get; set; } = new HashSet<string>();
        public Dictionary<string, Producto> Productos { get; set; } = new Dictionary<string, Producto>();
        public HashSet<string> ProductosNoEditables { get; set; } = new HashSet<string>();
        public Dictionary<string, string> ProductosEmpresa { get; set; } = new Dictionary<string, string>();
        public HashSet<string> VentanasLiberacion { get; set; } = new HashSet<string>();

        public ProductoServiceContext(IUnitOfWork uow, List<Producto> productos, int userId, int empresa) : base(uow, userId, empresa)
        {
            _productos = productos;
        }

        public async override Task Load()
        {
            var keysProductos = new Dictionary<string, Producto>();

            await base.Load();

            foreach (var p in _productos)
            {
                var keyProducto = $"{p.Codigo}.{p.CodigoEmpresa}";
                keysProductos[keyProducto] = new Producto() { Codigo = p.Codigo.Truncate(40), CodigoEmpresa = p.CodigoEmpresa, CodigoProductoEmpresa = p.CodigoProductoEmpresa.Truncate(40) };
            }

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

            foreach (var u in _uow.UnidadMedidaRepository.GetUnidadesMedida())
            {
                UnidadesMedida.Add(u.Id);
            }

            foreach (var r in _uow.ProductoRotatividadRepository.GetProductoRotatividades())
            {
                Rotatividades.Add(r.Id);
            }

            foreach (var nam in _uow.NcmRepository.GetNCMs())
            {
                NAMs.Add(nam.Id);
            }

            foreach (var gc in _uow.GrupoConsultaRepository.GetGruposConsulta())
            {
                GruposConsulta.Add(gc.Id);
            }

            var productos = _uow.ProductoRepository.GetProductos(keysProductos.Values, out HashSet<string> noEditables);

            foreach (var p in productos)
            {
                Productos[p.Codigo] = p;
            }

            foreach (var r in _uow.DominioRepository.GetDominios(CodigoDominioDb.VentanaLiberacion))
            {
                VentanasLiberacion.Add(r.Valor);
            }


            ProductosNoEditables = noEditables;
            ProductosEmpresa = _uow.ProductoRepository.GetCodigosByProductoEmpresa(keysProductos.Values.ToList());
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

        public virtual bool ExisteUnidadMedida(string unidadMedida)
        {
            return UnidadesMedida.Contains(unidadMedida);
        }

        public virtual bool ExisteRotatividad(short rotatividad)
        {
            return Rotatividades.Contains(rotatividad);
        }

        public virtual bool ExisteNAM(string nam)
        {
            return NAMs.Contains(nam);
        }

        public virtual bool ExisteGrupoConsulta(string grupoConsulta)
        {
            return GruposConsulta.Contains(grupoConsulta);
        }

        public virtual bool PermiteEdicion(string codigo)
        {
            return !ProductosNoEditables.Contains(codigo);
        }

        public virtual Producto GetProducto(string codigo)
        {
            return Productos.GetValueOrDefault(codigo, null);
        }

        public virtual bool ExisteProductoEmpresa(string codigoProductoEmpresa, string codigoProducto)
        {
            var codigo = ProductosEmpresa.GetValueOrDefault(codigoProductoEmpresa, null);
            return codigo != null && codigo != codigoProducto;
        }
        public virtual bool ExisteVentanaLiberacion(string valor)
        {
            return VentanasLiberacion.Contains(valor);
        }
    }
}
