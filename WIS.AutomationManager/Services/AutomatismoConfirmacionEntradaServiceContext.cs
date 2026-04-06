using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Automatismo;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Security;

namespace WIS.AutomationManager.Services
{
	public class AutomatismoConfirmacionEntradaServiceContext : AutomatismoServiceContext
	{
		protected int _empresa = 0;
		protected int _userId = 0;
		protected Usuario _usuario;

		protected readonly TransferenciaStockRequest _request;
		protected readonly EntradaStockAutomatismoRequest _ordenEntrada;

		public List<AtomatismoConfirmacionEntrada> _confirmacionEntradaAutomatismo;
		protected readonly AutomatismoEjecucionMapper _mapper;
		protected AutomatismoEjecucion _ejecucionEntradaAutomatismo;
		public readonly Dictionary<string, Producto> Productos = new Dictionary<string, Producto>();

		public AutomatismoConfirmacionEntradaServiceContext(IUnitOfWork uow,
			TransferenciaStockRequest entradasStockAutomatismo,
			EntradaStockAutomatismoRequest ordenesEntrada,
			int userId) : base(uow)
		{
			_request = entradasStockAutomatismo;
			_ordenEntrada = ordenesEntrada;
			_userId = userId;
			_empresa = ordenesEntrada.Empresa;
			_mapper = new AutomatismoEjecucionMapper();
		}

		public async override Task Load()
		{
			await base.Load();

			var productos = _ordenEntrada.Detalles
				.Select(p => new Producto
				{
					CodigoEmpresa = _ordenEntrada.Empresa,
					Codigo = p.Producto,
				});

			foreach (var p in _uow.ProductoRepository.GetProductos(productos, out HashSet<string> noEditables))
			{
				Productos[p.Codigo] = p;
			}

			_confirmacionEntradaAutomatismo = _uow.AutomatismoEjecucionRepository.GetAutomatismoConfirmacionAutomatismo(_request.IdEntrada);
			_ejecucionEntradaAutomatismo = _uow.AutomatismoEjecucionRepository.GetAutomatismoEjecucionById(int.Parse(_request.IdEntrada));

			if (_request.Usuario != null)
				_usuario = _uow.SecurityRepository.GetUser(_request.Usuario.LoginName);
			else
				_usuario = _uow.SecurityRepository.GetUsuario(_userId);
		}

		public AutomatismoEjecucion GetAutomatismoEjecucionEntrada()
		{
			return _ejecucionEntradaAutomatismo;
		}

		public decimal GetCantidadConfirmada(string IdEntrada, int empresa, string producto, string identificador, int lineaEntrada)
		{
			return _confirmacionEntradaAutomatismo.Where(x => x.EjecucionEntrada == IdEntrada
				&& x.Identificador == identificador
				&& x.Empresa == empresa
				&& x.CodigoProducto == producto
				&& x.LineaEntrada == lineaEntrada)?.Sum(x => x.Cantidad).Value ?? 0;
		}
		public bool AnyConfirmadaFinalizada(string IdEntrada, int empresa, string producto, string identificador, int lineaEntrada)
		{
			return _confirmacionEntradaAutomatismo.Any(x => x.EjecucionEntrada == IdEntrada
				&& x.Identificador == identificador
				&& x.Empresa == empresa
				&& x.CodigoProducto == producto && x.UltimaOperacionDetalle == "S" && x.LineaEntrada == lineaEntrada);
		}
		public bool ExisteProducto(int empresa, string producto)
		{
			return Productos.ContainsKey(producto);
		}

		public bool ExisteUsuario()
		{
			return _usuario != null && _usuario.IsEnabled;
		}

		public EntradaStockLineaAutomatismoRequest GetProductoOrden(int empresa, string producto, string loteConfirmado, int idEntrada)
		{
            return _ordenEntrada.Detalles.FirstOrDefault(p => p.Producto == producto && p.Identificador == loteConfirmado/* && p.LineaEntrada == idEntrada*/);
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
