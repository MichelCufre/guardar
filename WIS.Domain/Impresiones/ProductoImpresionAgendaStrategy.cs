using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Domain.Impresiones.Utils;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Impresiones
{
	public class ProductoImpresionAgendaStrategy : ProductoImpresionStrategy
	{
		private readonly Dictionary <Producto, int> _productosCantidad;

		public ProductoImpresionAgendaStrategy (
			IEstiloTemplate estilo,
			string tipoCodigoBarras,
			Dictionary <Producto, int> productosCantidad,
			IUnitOfWork uow,
			IPrintingService printingService)
			: base (estilo, uow, printingService, tipoCodigoBarras)
		{
			this._productosCantidad = productosCantidad;
		}

		public override List <DetalleImpresion> Generar (Impresora impresora)
		{
			var template = this._estilo.GetTemplate (impresora);

			var detalles = new List <DetalleImpresion> ();

			foreach (var producto in this._productosCantidad.OrderBy (s => s.Key.Codigo))
			{
				Producto prd = producto.Key;

				var codigoBarrasProducto = _uow.CodigoBarrasRepository.GetCodigoBarras (
					prd.Codigo,
					prd.Empresa.Id,
					int.Parse (_tipoCodigoBarra));
				var claves = GetDiccionarioInformacion (prd, codigoBarrasProducto);

				for (int i = 0; i < producto.Value; i ++)
					detalles.Add (
						new DetalleImpresion
						{
							Contenido      = template.Parse (claves),
							Estado         = _printingService.GetEstadoInicial (),
							FechaProcesado = DateTime.Now,
						});
			}

			return detalles;
		}
	}
}
