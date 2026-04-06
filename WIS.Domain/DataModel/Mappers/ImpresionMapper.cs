using System.Linq;
using WIS.Domain.Impresiones;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
	public class ImpresionMapper : Mapper
	{
		public ImpresionMapper()
		{
		}

		#region Impresion

		public virtual Impresion MapToObject(T_IMPRESION entity)
		{
			if (entity == null)
				return null;

			var impresion = new Impresiones.Impresion
			{
				Id = entity.NU_IMPRESION,
				Generado = entity.DT_GENERADO,
				Procesado = entity.DT_PROCESADO,
				Predio = entity.NU_PREDIO,
				CodigoImpresora = entity.CD_IMPRESORA,
				NombreImpresora = entity.NM_IMPRESORA,
				Estilo = entity.TP_LABEL,
				CantRegistros = entity.QT_REGISTROS,
				Error = entity.DS_ERROR,
				Referencia = entity.DS_REFERENCIA,
				Estado = entity.CD_ESTADO,
				Usuario = entity.CD_FUNCIONARIO
			};

			if ((entity.T_DET_IMPRESION?.Count ?? 0) > 0)
			{
				impresion.Detalles.AddRange(entity.T_DET_IMPRESION
					.Select(d => MapToEntityDetalleImpresion(d)));
			}

			return impresion;
		}

		public virtual T_IMPRESION MapToEntity(Impresion obj)
		{
			var referencia = obj.Referencia ?? string.Empty;
			var referenciaMaxLength = 150;

			if (referencia.Length > referenciaMaxLength)
			{
				referencia = obj.Referencia?.Substring(0, 146) + " ...";
			}

			return new T_IMPRESION
			{
				NU_IMPRESION = obj.Id,
				DT_GENERADO = obj.Generado,
				DT_PROCESADO = obj.Procesado,
				NU_PREDIO = obj.Predio,
				CD_IMPRESORA = obj.CodigoImpresora,
				NM_IMPRESORA = obj.NombreImpresora,
				TP_LABEL = obj.Estilo,
				QT_REGISTROS = obj.CantRegistros,
				DS_ERROR = obj.Error,
				DS_REFERENCIA = referencia,
				CD_ESTADO = obj.Estado,
				CD_FUNCIONARIO = obj.Usuario,
			};
		}

		public virtual T_DET_IMPRESION MapToEntityDetalleImpresion(DetalleImpresion detalle)
		{
			return new T_DET_IMPRESION
			{
				CD_ESTADO = detalle.Estado,
				DS_ERROR = detalle.Error,
				DT_PROCESADO = detalle.FechaProcesado,
				NU_IMPRESION = detalle.NumeroImpresion,
				NU_REGISTRO = detalle.Registro,
				VL_DATO = detalle.Contenido
			};
		}

		public virtual DetalleImpresion MapToEntityDetalleImpresion(T_DET_IMPRESION detalle)
		{
			return new DetalleImpresion
			{
				Registro = detalle.NU_REGISTRO,
				NumeroImpresion = detalle.NU_IMPRESION,
				Estado = detalle.CD_ESTADO,
				Contenido = detalle.VL_DATO,
				FechaProcesado = detalle.DT_PROCESADO,
				Error = detalle.DS_ERROR
			};
		}

		#endregion

		#region LenguajeImpresion

		public virtual LenguajeImpresion MapToObjectLenguajeImpresion(T_LENGUAJE_IMPRESION entity)
		{
			return new LenguajeImpresion
			{
				Id = entity.CD_LENGUAJE_IMPRESION,
				Descripcion = entity.DS_LENGUAJE_IMPRESION
			};
		}

		public virtual T_LENGUAJE_IMPRESION MapToEntityLenguajeImpresion(LenguajeImpresion entity)
		{
			return new T_LENGUAJE_IMPRESION
			{
				CD_LENGUAJE_IMPRESION = entity.Id,
				DS_LENGUAJE_IMPRESION = entity.Descripcion
			};
		}

		#endregion

		#region ServidorImpresion

		public virtual ServidorImpresion MapToObjectServidorImpresion(T_IMPRESORA_SERVIDOR entity)
		{
			return new ServidorImpresion
			{
				Id = entity.CD_SERVIDOR,
				Descripcion = entity.DS_SERVIDOR,
				ClientId = entity.CLIENTID,
				DominioServidor = entity.VL_DOMINIO_SERVIDOR,
				PassServidor = entity.VL_PASS_SERVIDOR,
				UrlServidor = entity.VL_URL_SERVIDOR,
				UserServidor = entity.VL_USER_SERVIDOR
			};
		}

		public virtual T_IMPRESORA_SERVIDOR MapToEntityServidorImpresion(ServidorImpresion entity)
		{
			return new T_IMPRESORA_SERVIDOR
			{
				CD_SERVIDOR = entity.Id,
				DS_SERVIDOR = entity.Descripcion,
				CLIENTID = entity.ClientId,
				VL_DOMINIO_SERVIDOR = entity.DominioServidor,
				VL_PASS_SERVIDOR = entity.PassServidor,
				VL_URL_SERVIDOR = entity.UrlServidor,
				VL_USER_SERVIDOR = entity.UserServidor
			};
		}

		#endregion

		#region Estilos
		public virtual EtiquetaEstiloLenguaje MapToObject(V_ESTILOS_LENGUAJES entity)
		{
			if (entity == null)
				return null;

			var estilo = new EtiquetaEstiloLenguaje
			{
				CodigoLabel = entity.CD_LABEL_ESTILO,
				Descripcion = entity.DS_LABEL_ESTILO,
				Tipo = entity.TP_LABEL,
				DescripcionDominio = entity.DS_DOMINIO_VALOR,
				CodigoLenguaje = entity.CD_LENGUAJE_IMPRESION,
				TipoContenedor = entity.TP_CONTENEDOR,
				Habilitado = entity.FL_HABILITADO,
				DescripcionTipo = entity.DS_TIPO_CONTENEDOR
			};

			return estilo;
		}

		#endregion

	}
}
