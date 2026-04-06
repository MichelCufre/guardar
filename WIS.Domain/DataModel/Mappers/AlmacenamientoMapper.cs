using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
	public class AlmacenamientoMapper : Mapper
	{
		public virtual AsociacionEstrategia Map(T_ALM_ASOCIACION asociacion)
		{
			if (asociacion == null)
				return null;

			return new AsociacionEstrategia
			{
				Clase = asociacion.CD_CLASSE,
				Empresa = asociacion.CD_EMPRESA,
				Grupo = asociacion.CD_GRUPO,
				Operativa = new OperativaAlmacenaje
				{
					Codigo = asociacion.CD_ALM_OPERATIVA_ASOCIABLE,
					Tipo = asociacion.TP_ALM_OPERATIVA_ASOCIABLE
				},
				Producto = asociacion.CD_PRODUTO,
				FechaRegistro = asociacion.DT_ADDROW,
				FechaModificacion = asociacion.DT_UPDROW,
				Id = asociacion.NU_ALM_ASOCIACION,
				Estrategia = asociacion.NU_ALM_ESTRATEGIA,
				Predio = asociacion.NU_PREDIO,
				Tipo = asociacion.TP_ALM_ASOCIACION,
			};
		}

		public virtual SugerenciaAlmacenamiento Map(T_ALM_SUGERENCIA sugerencia)
		{
			if (sugerencia == null)
				return null;

			var detalles = sugerencia.T_ALM_SUGERENCIA_DET
				.Select(d => Map(d))
				.ToList();

			return new SugerenciaAlmacenamiento
			{
                NuAlmSugerencia = sugerencia.NU_ALM_SUGERENCIA,
                Agrupador = sugerencia.CD_AGRUPADOR,
				Operativa = new OperativaAlmacenaje
				{
					Codigo = sugerencia.CD_ALM_OPERATIVA_ASOCIABLE,
					Tipo = sugerencia.TP_ALM_OPERATIVA_ASOCIABLE,
				},
				Clase = sugerencia.CD_CLASSE,
				Empresa = sugerencia.CD_EMPRESA,
				UbicacionSugerida = sugerencia.CD_ENDERECO_SUGERIDO,
				Estado = sugerencia.CD_ESTADO,
				Funcionario = sugerencia.CD_FUNCIONARIO,
				Grupo = sugerencia.CD_GRUPO,
				MotivoRechazo = sugerencia.CD_MOTVO_RECHAZO,
				Producto = sugerencia.CD_PRODUTO,
				Referencia = sugerencia.CD_REFERENCIA,
				FechaRegistro = sugerencia.DT_ADDROW,
				FechaModificacion = sugerencia.DT_UPDROW,
				Estrategia = sugerencia.NU_ALM_ESTRATEGIA,
				Predio = sugerencia.NU_PREDIO,
				Transaccion = sugerencia.NU_TRANSACCION,
				TiempoCalculo = sugerencia.VL_TIEMPO_CALCULO,
				Instancia = Map(sugerencia.T_ALM_LOGICA_INSTANCIA),
				Detalles = detalles,
            };
		}

		public virtual SugerenciaAlmacenamientoDetalle Map(T_ALM_SUGERENCIA_DET detalle)
		{
			return new SugerenciaAlmacenamientoDetalle
			{
                CantidadSeparar = detalle.QT_PRODUTO_AGRUPADOR,
				CantidadAuditada = detalle.QT_AUDITADA_AGRUPADOR??0,
				CantidadClasificada = detalle.QT_CLASIFICADA_AGRUPADOR ?? 0,
				Empresa = detalle.CD_EMPRESA_AGRUPADOR,
				Faixa = detalle.CD_FAIXA_AGRUPADOR,
				Lote = detalle.NU_IDENTIFICADOR_AGRUPADOR?.Trim()?.ToUpper(),
				Producto = detalle.CD_PRODUTO_AGRUPADOR,
				Vencimiento = detalle.DT_FABRICACAO_AGRUPADOR,
                UbicacionSugerida = detalle.CD_ENDERECO_SUGERIDO_AGRUPADOR,
                Estado = detalle.CD_ESTADO,
            };
		}

		public virtual InstanciaLogica Map(T_ALM_LOGICA_INSTANCIA logica)
		{
			if (logica == null)
				return null;

			return new InstanciaLogica
			{
				Descripcion = logica.DS_ALM_LOGICA_INSTANCIA,
				FechaRegistro = logica.DT_ADDROW,
				FechaModificacion = logica.DT_UPDROW,
				OrdenarAscendente = logica.FL_ORDEN_ASC,
				Estrategia = logica.NU_ALM_ESTRATEGIA,
				Logica = logica.NU_ALM_LOGICA,
				Id = logica.NU_ALM_LOGICA_INSTANCIA,
				Orden = logica.NU_ORDEN,
				Parametros = Map(logica.T_ALM_LOGICA_INSTANCIA_PARAM)
			};
		}

		public virtual SugerenciaAlmacenamiento Map(T_ALM_REABASTECIMIENTO sugerencia)
		{
			if (sugerencia == null)
				return null;

			return new SugerenciaAlmacenamiento
			{
				Empresa = sugerencia.CD_EMPRESA,
				UbicacionSugerida = sugerencia.CD_ENDERECO_SUGERIDO,
				Estado = sugerencia.CD_ESTADO,
				Funcionario = sugerencia.CD_FUNCIONARIO,
				Producto = sugerencia.CD_PRODUTO,
				Lote = sugerencia.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
				Referencia = sugerencia.CD_REFERENCIA,
				FechaRegistro = sugerencia.DT_ADDROW ?? DateTime.Now,
				FechaModificacion = sugerencia.DT_UPDROW,
				Predio = sugerencia.NU_PREDIO,
				Transaccion = sugerencia.NU_TRANSACCION,
				CantidadSeparar = sugerencia.QT_PRODUTO ?? 0,
				CantidadAuditada = sugerencia.QT_AUDITADA ?? 0,
				CantidadClasificada = sugerencia.QT_CLASIFICADA ?? 0,
				Vencimiento = sugerencia.DT_FABRICACAO,
				IgnorarStock = sugerencia.FL_IGNORAR_STOCK == "S",
			};
		}

		public virtual List<InstanciaLogicaParametro> Map(ICollection<T_ALM_LOGICA_INSTANCIA_PARAM> parametros)
		{
			var resultado = new List<InstanciaLogicaParametro>();

			foreach (var parametro in parametros)
			{
				resultado.Add(Map(parametro));
			}

			return resultado;
		}

		public virtual InstanciaLogicaParametro Map(T_ALM_LOGICA_INSTANCIA_PARAM parametro)
		{
			return new InstanciaLogicaParametro
			{
				FechaRegistro = parametro.DT_ADDROW,
				FechaModificacion = parametro.DT_UPDROW,
				Logica = parametro.NU_ALM_LOGICA,
				Instancia = parametro.NU_ALM_LOGICA_INSTANCIA,
				Id = parametro.NU_ALM_LOGICA_INSTANCIA_PARAM,
				Valor = parametro.VL_ALM_PARAMETRO,
				Parametro = Map(parametro.T_ALM_PARAMETRO)
			};
		}

		public virtual LogicaParametro Map(T_ALM_PARAMETRO parametro)
		{
			return new LogicaParametro
			{
				Nombre = parametro.NM_ALM_PARAMETRO,
				Id = parametro.NU_ALM_PARAMETRO
			};
		}
	}
}
