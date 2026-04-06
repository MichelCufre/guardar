using Microsoft.Extensions.Configuration;
using System.Linq;
using WIS.AutomationInterpreter.Models.Mappers.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Security;
using WIS.Persistence.Database;

namespace WIS.AutomationInterpreter.Models.Mappers
{
	public class AutomatismoMapper : IAutomatismoMapper
	{
		protected string _secret;

		public AutomatismoMapper(IConfiguration config)
		{
			_secret = config.GetSection("IntegrationSettings:Secret")?.Value;
		}

		public virtual NotificacionAjustesStockRequest Map(NotificacionAjustesStockAutomatismoRequest request)
		{
			return new NotificacionAjustesStockRequest
			{
				Ajustes = request.Ajustes.Select(a => Map(a)).ToList(),
				Archivo = request.Archivo,
				DsReferencia = request.DsReferencia,
				Empresa = request.Empresa,
				Puesto = request.Puesto,
				Usuario = MapUsuario(request.Usuario),
			};
		}

		public virtual AutomatismoAjusteStockRequest Map(AjusteStockAutomatismoRequest request)
		{
			return new AutomatismoAjusteStockRequest
			{
				Cantidad = request.Cantidad,
				Causa = request.Causa,
				FechaVencimiento = request.FechaVencimiento,
				Identificador = request.Identificador,
				Predio = request.Predio,
				Producto = request.Producto,
			};
		}

		public virtual ConfirmacionEntradaStockRequest Map(ConfirmacionEntradaStockAutomatismoRequest request)
		{
			return new ConfirmacionEntradaStockRequest
			{
				Archivo = request.Archivo,
				Detalles = request.Detalles.Select(d => Map(d)).ToList(),
				DsReferencia = request.DsReferencia,
				Empresa = request.Empresa,
				Puesto = request.Puesto,
				IdEntrada = request.IdEntrada,
				Predio = request.Predio,
				Usuario = MapUsuario(request.Usuario),
			};
		}

		public virtual ConfirmacionEntradaStockLineaRequest Map(ConfirmacionEntradaStockLineaAutomatismoRequest request)
		{
			return new ConfirmacionEntradaStockLineaRequest
			{
				Cantidad = request.Cantidad,
				FechaVencimiento = request.FechaVencimiento,
				Identificador = request.Identificador,
				IdLinea = request.IdLinea,
				Producto = request.Producto
			};
		}

		public virtual ConfirmacionSalidaStockRequest Map(ConfirmacionSalidaStockAutomatismoRequest request)
		{
			return new ConfirmacionSalidaStockRequest
			{
				Archivo = request.Archivo,
				CodigoAgente = request.CodigoAgente,
				Detalles = request.Detalles.Select(d => Map(d)).ToList(),
				DsReferencia = request.DsReferencia,
				Empresa = request.Empresa,
				Predio = request.Predio,
				IdSalida = request.IdSalida,
				Pedido = request.Pedido,
				Preparacion = request.Preparacion,
				Puesto = request.Puesto,
				TipoAgente = request.TipoAgente,
				Usuario = MapUsuario(request.Usuario),
			};
		}

		public virtual ConfirmacionSalidaStockLineaRequest Map(ConfirmacionSalidaStockLineaAutomatismoRequest request)
		{
			return new ConfirmacionSalidaStockLineaRequest
			{
				IdLinea = request.IdLinea,
				Producto = request.Producto,
			};
		}

		protected virtual UsuarioRequest MapUsuario(string loginName)
		{
			if (string.IsNullOrEmpty(loginName))
				return null;

			return new UsuarioRequest
			{
				Hash = Signer.ComputeHash(_secret, loginName),
				LoginName = loginName
			};
		}
    }
}
