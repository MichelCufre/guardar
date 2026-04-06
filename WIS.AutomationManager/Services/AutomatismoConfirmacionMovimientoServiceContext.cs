using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Automatismo;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Security;

namespace WIS.AutomationManager.Services
{
    public class AutomatismoConfirmacionMovimientoServiceContext : AutomatismoServiceContext
    {
        protected int _empresa = 0;
        protected int _userId = 0;
        protected Usuario _usuario;

        protected readonly TransferenciaStockRequest _request;
        protected readonly AjustesDeStockRequest _requestAjustes;
        protected readonly ConfirmacionMovimientoStockRequest _confirmacionMovimiento;

        public List<AtomatismoConfirmacionEntrada> _confirmacionEntradaAutomatismo;
        protected readonly AutomatismoEjecucionMapper _mapper;
        protected AutomatismoEjecucion _ejecucionMovimientoAutomatismo;
        public readonly Dictionary<string, Producto> Productos = new Dictionary<string, Producto>();

        public AutomatismoConfirmacionMovimientoServiceContext(IUnitOfWork uow,
            TransferenciaStockRequest entradasStockAutomatismo,
            ConfirmacionMovimientoStockRequest confirmacionMovimiento,
            int userId) : base(uow)
        {
            _request = entradasStockAutomatismo;
            _confirmacionMovimiento = confirmacionMovimiento;
            _userId = userId;
            _empresa = confirmacionMovimiento.Empresa;
            _mapper = new AutomatismoEjecucionMapper();
        }
        public AutomatismoConfirmacionMovimientoServiceContext(IUnitOfWork uow,
            AjustesDeStockRequest ajusteStockAutomatismo,
            ConfirmacionMovimientoStockRequest confirmacionMovimiento,
            int userId) : base(uow)
        {
            _requestAjustes = ajusteStockAutomatismo;
            _confirmacionMovimiento = confirmacionMovimiento;
            _userId = userId;
            _empresa = confirmacionMovimiento.Empresa;
            _mapper = new AutomatismoEjecucionMapper();
        }
        public async override Task Load()
        {
            await base.Load();

            var productos = _confirmacionMovimiento.Detalles
                .Select(p => new Producto
                {
                    CodigoEmpresa = _confirmacionMovimiento.Empresa,
                    Codigo = p.Producto,
                });

            foreach (var p in _uow.ProductoRepository.GetProductos(productos, out HashSet<string> noEditables))
            {
                Productos[p.Codigo] = p;
            }

            if (_request != null)
            {
                if (_request.Usuario != null)
                    _usuario = _uow.SecurityRepository.GetUser(_request.Usuario.LoginName);
                else
                    _usuario = _uow.SecurityRepository.GetUsuario(_userId);
            }
            else
            {
                if (_requestAjustes.Usuario != null)
                    _usuario = _uow.SecurityRepository.GetUser(_requestAjustes.Usuario.LoginName);
                else
                    _usuario = _uow.SecurityRepository.GetUsuario(_userId);
            }
        }

        public bool ExisteProducto(int empresa, string producto)
        {
            return Productos.ContainsKey(producto);
        }

        public bool ExisteUsuario()
        {
            return _usuario != null && _usuario.IsEnabled;
        }

        public Usuario GetUsuario()
        {
            return _usuario;
        }

        public void AddConfirmacionAutomatismoEntrada(long ejecucion, TransferenciaStockRequest request)
        {
            var det = _mapper.MapObject(ejecucion, _usuario.Username, request);
            _confirmacionEntradaAutomatismo.AddRange(det);
        }
    }
}
