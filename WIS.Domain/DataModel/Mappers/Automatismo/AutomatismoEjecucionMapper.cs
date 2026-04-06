using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Enums;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Automatismo
{
	public class AutomatismoEjecucionMapper : Mapper
	{
		protected readonly AutomatismoDataMapper _dataMapper;

		public AutomatismoEjecucionMapper()
		{
			_dataMapper = new AutomatismoDataMapper();
		}

		public virtual AutomatismoEjecucion Map(T_AUTOMATISMO_EJECUCION entity, bool data = false)
		{
			if (entity == null) return null;

			var result = new AutomatismoEjecucion()
			{
				Id = entity.NU_AUTOMATISMO_EJECUCION,
				IdAutomatismo = entity.NU_AUTOMATISMO,
				AutomatismoInterfaz = entity.NU_AUTOMATISMO_INTERFAZ,
				InterfazExterna = entity.CD_INTERFAZ_EXTERNA,
				Estado = GetTipo(entity.ND_SITUACION),
				Referencia = entity.DS_REFERENCIA,
				IdentityUser = entity.VL_IDENTITY_USER,
				FechaRegistro = entity.DT_ADDROW,
				FechaModificacion = entity.DT_UDPROW,
				Transaccion = entity.NU_TRANSACCION,
			};

			if (data && (entity.T_AUTOMATISMO_DATA != null && entity.T_AUTOMATISMO_DATA.Count > 0))
				result.AutomatismoData = _dataMapper.Map(entity.T_AUTOMATISMO_DATA.ToList());

			return result;

		}
		public virtual T_AUTOMATISMO_EJECUCION Map(AutomatismoEjecucion obj)
		{
			if (obj == null) return null;

			return new T_AUTOMATISMO_EJECUCION()
			{
				NU_AUTOMATISMO_EJECUCION = obj.Id,
				NU_AUTOMATISMO = obj.IdAutomatismo,
				NU_AUTOMATISMO_INTERFAZ = obj.AutomatismoInterfaz,
				CD_INTERFAZ_EXTERNA = obj.InterfazExterna,
				ND_SITUACION = GetTipoDb(obj.Estado),
				DS_REFERENCIA = obj.Referencia,
				VL_IDENTITY_USER = obj.IdentityUser,
				DT_ADDROW = obj.FechaRegistro,
				DT_UDPROW = obj.FechaModificacion,
				NU_TRANSACCION = obj.Transaccion,
			};
		}

		public virtual EstadoEjecucion GetTipo(string estado)
		{
			switch (estado)
			{
				case EstadoAutomatismoEjecucionDb.PROCESADO_PENDIENTE: return EstadoEjecucion.PENDIENTE;
				case EstadoAutomatismoEjecucionDb.PROCESADO_OK: return EstadoEjecucion.PROCESADO_OK;
				case EstadoAutomatismoEjecucionDb.PROCESADO_CON_ERROR_API: return EstadoEjecucion.PROCESADO_ERROR_API;
				case EstadoAutomatismoEjecucionDb.PROCESADO_CON_ERROR_AUTOMATISMO: return EstadoEjecucion.PROCESADO_ERROR_AUTOMATISMO;
				case EstadoAutomatismoEjecucionDb.ESTPROCREP: return EstadoEjecucion.ESTPROCREP;
				case EstadoAutomatismoEjecucionDb.ESTPROCFIN: return EstadoEjecucion.PROCESADO_FIN;

			}
			return EstadoEjecucion.UNKNOWN;
		}
		public virtual string GetTipoDb(EstadoEjecucion estado)
		{
			switch (estado)
			{
				case EstadoEjecucion.PENDIENTE: return EstadoAutomatismoEjecucionDb.PROCESADO_PENDIENTE;
				case EstadoEjecucion.PROCESADO_OK: return EstadoAutomatismoEjecucionDb.PROCESADO_OK;
				case EstadoEjecucion.PROCESADO_ERROR_API: return EstadoAutomatismoEjecucionDb.PROCESADO_CON_ERROR_API;
				case EstadoEjecucion.PROCESADO_ERROR_AUTOMATISMO: return EstadoAutomatismoEjecucionDb.PROCESADO_CON_ERROR_AUTOMATISMO;
				case EstadoEjecucion.ESTPROCREP: return EstadoAutomatismoEjecucionDb.ESTPROCREP;
				case EstadoEjecucion.PROCESADO_FIN: return EstadoAutomatismoEjecucionDb.ESTPROCFIN;
			}
			return null;
		}

        public virtual List<T_AUTOMATISMO_CONF_ENTRADA> Map(long ejecucion, string loginName, TransferenciaStockRequest obj, bool confEntrada = false)
        {
			if (obj == null) return null;

			List<T_AUTOMATISMO_CONF_ENTRADA> listaDetalles = new List<T_AUTOMATISMO_CONF_ENTRADA>();

			foreach (var det in obj.Transferencias)
			{
				listaDetalles.Add(new T_AUTOMATISMO_CONF_ENTRADA()
				{
					NU_INTERFAZ_EJECUCION = ejecucion,
                    NU_INTERFAZ_EJECUCION_ENT = string.IsNullOrEmpty(obj.IdEntrada) ? $"{ejecucion}" : obj.IdEntrada,
                    FL_ULT_OPERACION = confEntrada ? "S" : "N",
                    CD_EMPRESA = obj.Empresa,
					DS_REFERENCIA = obj.DsReferencia,
					NM_ARCHIVO = obj.Archivo,
					CD_ENDERECO = det.Ubicacion,
					CD_ENDERECO_DEST = det.UbicacionDestino,
					CD_PRODUTO = det.CodigoProducto,
					NU_IDENTIFICADOR = det.Identificador?.Trim()?.ToUpper(),
					QT_PRODUTO = det.Cantidad,
                    FL_ULT_OPERACION_DET = confEntrada ? "S" : "N",
                    LOGINNAME = loginName,
					ID_LINEA_ENTRADA = det.IdLinea,
				});
			}
			return listaDetalles;
		}

		public virtual List<AtomatismoConfirmacionEntrada> MapObject(long ejecucion, string loginName, TransferenciaStockRequest obj)
		{
			if (obj == null) return null;

			List<AtomatismoConfirmacionEntrada> listaDetalles = new List<AtomatismoConfirmacionEntrada>();

			foreach (var det in obj.Transferencias)
			{
				listaDetalles.Add(new AtomatismoConfirmacionEntrada()
				{
					Ejecucion = ejecucion,
					EjecucionEntrada = obj.IdEntrada,
					//UltimaOperacion = obj.UltimaOperacion ? "S" : "N",
					Empresa = obj.Empresa,
					DsReferencia = obj.DsReferencia,
					Archivo = obj.Archivo,
					Ubicacion = det.Ubicacion,
					UbicacionDestino = det.UbicacionDestino,
					CodigoProducto = det.CodigoProducto,
					Identificador = det.Identificador?.Trim()?.ToUpper(),
					Cantidad = det.Cantidad,
					//UltimaOperacionDetalle = det.UltimaOperacion ? "S" : "N",
					LoginName = loginName,
					LineaEntrada = det.IdLinea
				});
			}

			return listaDetalles;
		}

		public virtual List<AtomatismoConfirmacionEntrada> Map(List<T_AUTOMATISMO_CONF_ENTRADA> obj)
		{
			if (obj == null) return null;

			List<AtomatismoConfirmacionEntrada> ListaDetalles = new List<AtomatismoConfirmacionEntrada>();

			foreach (var det in obj)
			{

				ListaDetalles.Add(new AtomatismoConfirmacionEntrada()
				{
					Ejecucion = det.NU_INTERFAZ_EJECUCION,
					EjecucionEntrada = det.NU_INTERFAZ_EJECUCION_ENT,
					UltimaOperacion = det.FL_ULT_OPERACION,
					Empresa = det.CD_EMPRESA,
					DsReferencia = det.DS_REFERENCIA,
					Archivo = det.NM_ARCHIVO,
					Ubicacion = det.CD_ENDERECO,
					UbicacionDestino = det.CD_ENDERECO_DEST,
					CodigoProducto = det.CD_PRODUTO,
					Identificador = det.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
					Cantidad = det.QT_PRODUTO,
					LineaEntrada = det.ID_LINEA_ENTRADA,
					UltimaOperacionDetalle = det.FL_ULT_OPERACION_DET,
					LoginName = det.LOGINNAME
				});
			}
			return ListaDetalles;
		}

	}
}
