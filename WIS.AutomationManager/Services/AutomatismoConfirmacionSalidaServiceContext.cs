using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Security;

namespace WIS.AutomationManager.Services
{
	public class AutomatismoConfirmacionSalidaServiceContext : AutomatismoServiceContext
	{
		protected int _empresa = 0;
		protected int _userId = 0;
		protected Usuario _usuario;

		protected readonly PickingRequest _request;

		public readonly Dictionary<string, Producto> Productos = new Dictionary<string, Producto>();

		public AutomatismoConfirmacionSalidaServiceContext(IUnitOfWork uow,
			PickingRequest salidasStockAutomatismo,
			int userId) : base(uow)
		{
			_request = salidasStockAutomatismo;
			_userId = userId;
			_empresa = _request.Empresa;
		}

		public async override Task Load()
		{
			await base.Load();

			var productos = _request.Detalles
				.Select(p => new Producto
				{
					CodigoEmpresa = _request.Empresa,
					Codigo = p.CodigoProducto,
				});

			foreach (var p in _uow.ProductoRepository.GetProductos(productos, out HashSet<string> noEditables))
			{
				Productos[p.Codigo] = p;
			}

			if (_request.Usuario != null)
				_usuario = _uow.SecurityRepository.GetUser(_request.Usuario.LoginName);
			else
				_usuario = _uow.SecurityRepository.GetUsuario(_userId);
		}

		public bool ExisteProducto(int empresa, string producto)
		{
			return Productos.ContainsKey(producto);
		}

		public bool ExisteUsuario()
		{
			return _usuario != null && _usuario.IsEnabled;
		}
	}
}
