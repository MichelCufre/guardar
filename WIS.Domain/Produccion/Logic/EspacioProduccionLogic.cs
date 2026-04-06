using System;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Produccion.Models;

namespace WIS.Domain.Produccion.Logic
{
	public class EspacioProduccionLogic : IEspacioProduccionLogic
	{
		protected readonly IUnitOfWorkFactory _uowFactory;
		protected readonly IEspacioProduccionFactory _espacioProduccionFactory;

		public EspacioProduccionLogic(IUnitOfWorkFactory uowFactory, IEspacioProduccionFactory espacioProduccionFactory)
		{
			_uowFactory = uowFactory;
			_espacioProduccionFactory = espacioProduccionFactory;
		}

		public virtual EspacioProduccion CrearEspacioProduccion(string descripcion, string tipoEspacio, string flConfirmacionManual, string flStockConsumible, string predio)
		{
			using var uow = _uowFactory.GetUnitOfWork();

			var espacio = _espacioProduccionFactory.Create(tipoEspacio);

			espacio.Id = uow.EspacioProduccionRepository.GetNumeroEspacio();
			espacio.Descripcion = descripcion;
			espacio.Predio = predio;
			espacio.FechaAlta = DateTime.Now;
			espacio.NumeroIngreso = null;
			espacio.FlConfirmacionManual = flConfirmacionManual;
			espacio.FlStockConsumible = flStockConsumible;

			this.AgregarUbicacionesEspacio(espacio);

			return espacio;
		}

		public virtual void AgregarUbicacionesEspacio(EspacioProduccion espacio)
		{
            /*
             * Formato Ubicaciones: predio+PRD+IDEspacio-E+3digitos 
             * Entrada 1PRD1-E01
             * Produccion 1PRD1-P01
             * Salida 1PRD1-S01
             * * SalidaTran 1PRD1-ST01
             */
            using var uow = _uowFactory.GetUnitOfWork();
            string prefijo = uow.ParametroRepository.GetParameter(ParamManager.PREFIJO_PRODUCCION) ?? BarcodeDb.PREFIX_UBIC_PRODUCCION;

            string ubicacionEntrada = $"{espacio.Predio}{prefijo}{espacio.Id}-E01";
			string ubicacionProduccion = $"{espacio.Predio}{prefijo}{espacio.Id}-P01";
			string ubicacionSalida = $"{espacio.Predio}{prefijo}{espacio.Id}-S01";
			string ubicacionSalidaTran = $"{espacio.Predio}{prefijo}{espacio.Id}-ST01";

			var entrada = new Ubicacion()
			{
				Id = ubicacionEntrada,
				IdEmpresa = CEspacioProduccion.Empresa,
				Bloque = null,
				NumeroPredio = espacio.Predio,
				CodigoClase = CEspacioProduccion.ProductoClase,
				IdProductoRotatividad = CEspacioProduccion.ProductoRotatividad,
				IdProductoFamilia = CEspacioProduccion.ProductoFamilia,
				IdUbicacionTipo = TipoUbicacionDb.WIS,
				CodigoSituacion = SituacionDb.Activo,
				EsUbicacionBaja = true,
				NecesitaReabastecer = false,
				FechaInsercion = DateTime.Now,
				FechaModificacion = DateTime.Now,
				IdUbicacionArea = AreaUbicacionDb.BlackBox,
				IdUbicacionZona = TipoUbicacionZonaDb.Normal,
                Profundidad = 1,
                CodigoBarras = $"{BarcodeDb.PREFIX_UBICACION}{ubicacionEntrada}"
            };

			espacio.IdUbicacionEntrada = ubicacionEntrada;
			espacio.UbicacionEntrada = entrada;

			var produccion = new Ubicacion()
			{
				Id = ubicacionProduccion,
				IdEmpresa = CEspacioProduccion.Empresa,
				Bloque = null,
				NumeroPredio = espacio.Predio,
				CodigoClase = CEspacioProduccion.ProductoClase,
				IdProductoRotatividad = CEspacioProduccion.ProductoRotatividad,
				IdProductoFamilia = CEspacioProduccion.ProductoFamilia,
				IdUbicacionTipo = TipoUbicacionDb.WIS,
				CodigoSituacion = SituacionDb.Activo,
				EsUbicacionBaja = true,
				NecesitaReabastecer = false,
				FechaInsercion = DateTime.Now,
				FechaModificacion = DateTime.Now,
				IdUbicacionArea = AreaUbicacionDb.BlackBox,
				IdUbicacionZona = TipoUbicacionZonaDb.Normal,
                Profundidad = 1,
                CodigoBarras = $"{BarcodeDb.PREFIX_UBICACION}{ubicacionProduccion}"
            };

			espacio.IdUbicacionProduccion = ubicacionProduccion;
			espacio.UbicacionProduccion = produccion;

			var salida = new Ubicacion()
			{
				Id = ubicacionSalida,
				IdEmpresa = CEspacioProduccion.Empresa,
				Bloque = null,
				NumeroPredio = espacio.Predio,
				CodigoClase = CEspacioProduccion.ProductoClase,
				IdProductoRotatividad = CEspacioProduccion.ProductoRotatividad,
				IdProductoFamilia = CEspacioProduccion.ProductoFamilia,
				IdUbicacionTipo = TipoUbicacionDb.WIS,
				CodigoSituacion = SituacionDb.Activo,
				EsUbicacionBaja = true,
				NecesitaReabastecer = false,
				FechaInsercion = DateTime.Now,
				FechaModificacion = DateTime.Now,
				IdUbicacionArea = AreaUbicacionDb.ProduccionSalida,
				IdUbicacionZona = TipoUbicacionZonaDb.Normal,
                Profundidad = 1,
                CodigoBarras = $"{BarcodeDb.PREFIX_UBICACION}{ubicacionSalida}"
            };

			espacio.IdUbicacionSalida = ubicacionSalida;
			espacio.UbicacionSalida = salida;

			var salidaTran = new Ubicacion()
			{
				Id = ubicacionSalidaTran,
				IdEmpresa = CEspacioProduccion.Empresa,
				Bloque = null,
				NumeroPredio = espacio.Predio,
				CodigoClase = CEspacioProduccion.ProductoClase,
				IdProductoRotatividad = CEspacioProduccion.ProductoRotatividad,
				IdProductoFamilia = CEspacioProduccion.ProductoFamilia,
				IdUbicacionTipo = TipoUbicacionDb.WIS,
				CodigoSituacion = SituacionDb.Activo,
				EsUbicacionBaja = true,
				NecesitaReabastecer = false,
				FechaInsercion = DateTime.Now,
				FechaModificacion = DateTime.Now,
				IdUbicacionArea = AreaUbicacionDb.BlackBox,
				IdUbicacionZona = TipoUbicacionZonaDb.Normal,
                Profundidad = 1,
                CodigoBarras = $"{BarcodeDb.PREFIX_UBICACION}{ubicacionSalidaTran}"
            };

            espacio.IdUbicacionSalidaTran = ubicacionSalidaTran;
			espacio.UbicacionSalidaTran = salidaTran;
		}
	}
}
