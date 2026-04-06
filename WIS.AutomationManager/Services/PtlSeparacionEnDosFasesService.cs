using MigraDoc.DocumentObjectModel.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.AutomationManager.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Impresiones;
using WIS.Domain.ManejoStock.Constants;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Security;

namespace WIS.AutomationManager.Services
{
    public class PtlSeparacionEnDosFasesService : PtlService
    {
        public PtlSeparacionEnDosFasesService(
            IUnitOfWorkFactory uowFactory,
            IUnitOfWorkInMemoryFactory uowFactoryMemory,
            IPtlInterpreterClientService interpretService,
            IAutomatismoWmsApiClientService wmsApiClientService,
            IPtl ptl,
		   IIdentityService _identity): base(uowFactory, uowFactoryMemory, interpretService, wmsApiClientService, ptl, _identity)
        {

        }

        #region PrenderLuces

        public override ValidationsResult PrenderLuces(PtlPosicionEnUso accion)
        {
            ValidationsResult result = this.ValidarOperacion(accion.Color, accion.Empresa, accion.Producto);
			var validaciones = new List<Error>();

			if (result.IsValid())
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    try
                    {
                        _uowMemory.BeginTransaction();
                        var detail = accion.GetDetail<PtlDetailSeparacionPickingDosFases>();

                        int tipoAgrupacion = _ptl.GetTipoAgrupacion();

                        var pickingList = this.GetPickingAgrupadoComparteCont(detail.Preparacion, detail.Contenedor, accion.Empresa, accion.Producto, tipoAgrupacion);

                        if (pickingList != null && pickingList.Count() > 0)
                        {
                            uow.CreateTransactionNumber("Prender luces - Separacion de preparaciones en dos fases");
                            uow.BeginTransaction(uow.GetSnapshotIsolationLevel());

                            List<AutomatismoPosicion> posicionesDisponibles = base.ReservarPosicionesByComparteAgrupaciones(pickingList.Select(w => w.Agrupacion).Distinct().ToList(), tipoAgrupacion);

                            decimal qtTotSepara = ProcesarAccion(accion, pickingList, posicionesDisponibles, result);

                            LiberarPosicionesNoUtilizadasEnAccion(posicionesDisponibles, accion);

                            uow.SaveChanges();
                            uow.Commit();

                            _uowMemory.Commit();

                            result.SuccessMessage = qtTotSepara.ToString();
                        }
                        else
                        {
							validaciones.Add(new Error("WMSAPI_msg_Error_PtlNoMercaderiaASeparar"));
							var messages = Translator.Translate(uow, validaciones, _identity.UserId);
							result.Errors.Add(new ValidationsError(0, true, messages));
                        }
                    }
                    catch (Exception ex)
                    {
                        _uowMemory.Rollback();

                        if (ex is Microsoft.EntityFrameworkCore.DbUpdateException
                            && ex.InnerException != null
                            && uow.IsSnapshotException(ex.InnerException))
                        {
                            return PrenderLuces(accion);
                        }

						validaciones.Add(new Error("WMSAPI_msg_Error_PtlErrorNoControladoEx", ex));
						var messages = Translator.Translate(uow, validaciones, _identity.UserId);
						result.Errors.Add(new ValidationsError(0, true, messages));
                    }
                }
            }

            return result;
        }

        private decimal ProcesarAccion(PtlPosicionEnUso accion, List<DetallePedido> pickingList, List<AutomatismoPosicion> posicionesDisponibles, ValidationsResult result)
        {
            decimal qtSolicitada = decimal.Parse(accion.Display);
            decimal qtTotSepara = 0;
            if (posicionesDisponibles.Count() > 0)
            {
                List<PtlPosicionEnUso> colLucesPrender = new List<PtlPosicionEnUso>();
                foreach (var lineaPicking in pickingList)
                {
                    AutomatismoPosicion posicion = posicionesDisponibles.FirstOrDefault(w => w.ComparteAgrupacion == lineaPicking.Agrupacion);

                    if (posicion != null)
                    {
                        PtlPosicionEnUso luz = new PtlPosicionEnUso();
                        luz.Clonar(accion);

                        decimal auxQtSeparar = qtSolicitada > (lineaPicking.CantidadPreparada ?? 0) ? lineaPicking.CantidadPreparada ?? 0 : qtSolicitada;
                        qtSolicitada -= auxQtSeparar;

                        auxQtSeparar = auxQtSeparar < 0 ? 0 : auxQtSeparar;
                        luz.SetOperacion(PtlEstadoPosicion.Separando, auxQtSeparar.ToString(), posicion.Id);
                        colLucesPrender.Add(luz);
                        qtTotSepara += auxQtSeparar;

                    }

                    if (qtSolicitada <= 0)
                        break;
                }

                PrenderLuz(colLucesPrender, result);
            }
            return qtTotSepara;
        }

        private List<DetallePedido> GetPickingAgrupadoComparteCont(int preparacion, int contenedor, int cdEmpresa, string cdProduto, int tipoAgrupacion)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                return uow.PtlRepository.GetPickingAgrupadoComparteCont(preparacion, contenedor, cdEmpresa, cdProduto, tipoAgrupacion);
            }
        }

        #endregion

        protected override ValidationsResult ConfirmarLuzConCantidad(PtlPosicionEnUso accion)
        {
            var validationResult = new ValidationsResult();
			var validaciones = new List<Error>();

			var luz = _uowMemory.PosicionRepository.GetPtlUbicacionPrendida(accion.Ubicacion, accion.Color);

            try
            {
                _uowMemory.BeginTransaction();

                _uowMemory.PosicionRepository.RemovePtlUbicacionPrendida(luz.Ubicacion, luz.Color);
                PtlColorEnUso ptlUso = _uowMemory.ColorRepository.GetColorEnUso(accion.Ptl, accion.Color);

                decimal cantidadOriginal = decimal.Parse(luz.Display);
                decimal cantidadIngresada = cantidadOriginal;

                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    if (!string.IsNullOrEmpty(accion.Display))
                    {
                        decimal intCantidadDisplay = decimal.Parse(accion.Display);

                        if (intCantidadDisplay > cantidadIngresada)
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_PtlNoSePuedePikear"));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            validationResult.Errors.Add(new ValidationsError(0, true, messages));
                        }

                        cantidadIngresada = intCantidadDisplay;
                    }

                    if (validationResult.HasError())
                    {
                        this.NotificarLuzError(validationResult, luz);

                        return validationResult;
                    }

                    if (cantidadIngresada != 0)
                    {

                        uow.BeginTransaction();
                        uow.CreateTransactionNumber("ConfirmarLuzConCantidad PTL");

                        AutomatismoPosicion automatismoPosicion = _ptl.GetPosicion(luz.Ubicacion);
                        PtlDetailSeparacionPickingDosFases datosPicking = luz.GetDetail<PtlDetailSeparacionPickingDosFases>();

                        int contenedorVirtual = automatismoPosicion.GetNumeroVirtualEtiqueta();

                        var result = _wmsApiClientService.SepararProductoPreparacion(ptlUso.UserId, datosPicking.Preparacion, datosPicking.Contenedor, contenedorVirtual, automatismoPosicion.IdUbicacion, luz.Empresa, luz.Producto, cantidadIngresada, automatismoPosicion.TipoAgrupacion, automatismoPosicion.GetComparteEntrega(), BarcodeDb.TIPO_CONTENEDOR_V);

                        if (result.HasError())
                        {
                            validationResult.Errors.AddRange(result.Errors);
                            this.NotificarLuzError(validationResult, luz);
                        }

                        uow.SaveChanges();
                        uow.Commit();


                        _uowMemory.Commit();
                    }

                    if (!validationResult.HasError() && cantidadOriginal > cantidadIngresada)
                    {
                        luz.Display = (cantidadOriginal - cantidadIngresada).ToString();

                        this.EnviarAlPrincipioDeLaCola(luz, validationResult);
                    }

                }
            }
            catch (Exception ex)
            {
                _uowMemory.Rollback();

                this.NotificarLuzErrorNoControlado(validationResult, luz, ex);
            }

            return validationResult;

        }


        public override ValidationsResult ConfirmarCerrarUbicacion(PtlPosicionEnUso accion)
        {
            var validationResult = new ValidationsResult();
			var validaciones = new List<Error>();
			var luz = _uowMemory.PosicionRepository.GetUbicacionesPrendidasByPosicion(accion.Ubicacion).FirstOrDefault(w => w.Estado == PtlEstadoPosicion.Cerrando);

            AutomatismoPosicion posicionOrigen = _ptl.GetPosicion(accion.Ubicacion);
            AutomatismoPosicion posicionSalida = _ptl.GetPosiciones(AutomatismoPosicionesTipoDb.POS_SALIDA).FirstOrDefault();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                if (posicionSalida == null)
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_PtlUbicacionSalidaNoConfigurada"));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    validationResult.Errors.Add(new ValidationsError(0, true, messages));
                }
                else
                {
                    int numeroContenedorPosicion = posicionOrigen.GetNumeroVirtualEtiqueta();

                    try
                    {
                        _uowMemory.BeginTransaction();
                        _uowMemory.PosicionRepository.RemovePtlUbicacionPrendida(accion.Ubicacion, accion.Color, false);

                        PtlDetailSeparacionPickingDosFases datosContenedorDestino = luz.GetDetail<PtlDetailSeparacionPickingDosFases>();

                        var result = _wmsApiClientService.CambiarContenedor(numeroContenedorPosicion, luz.UserId, posicionSalida.IdUbicacion, datosContenedorDestino.Contenedor, TipoOperacion.Preparacion, posicionOrigen.TipoAgrupacion, posicionOrigen.GetComparteEntrega(), datosContenedorDestino.TipoEtiqueta);

                        if (result.HasError())
                        {
                            validationResult.Errors.AddRange(result.Errors);
                            this.NotificarLuzError(validationResult, luz);
                        }

                        if (validationResult.IsValid())
                        {

                            if (!_uowMemory.PosicionRepository.AnyUbicacionesPrendidasByUbicacion(luz.Ubicacion))
                            {
                                uow.CreateTransactionNumber("ConfirmarCierraUbicacion PTL");
                                uow.BeginTransaction();

                                posicionOrigen.LimpiarComparteAgrupacion();
                                posicionOrigen.Transaccion = uow.GetTransactionNumber();
                                uow.AutomatismoPosicionRepository.Update(posicionOrigen);

                                uow.SaveChanges();
                                uow.Commit(); 
                            }

                            if (accion.Detalle == null)
                            {
                                _uowMemory.Commit();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (accion.Detalle == null)
                        {
                            _uowMemory.Rollback();

                            this.NotificarLuzErrorNoControlado(validationResult, luz, ex);
                        }
                        else
                        {
                            NotificarLuzErrorNoControlado(validationResult, null, ex);
                        }
                    }

                }
            }
            return validationResult;

        }

        protected override bool IsPosicionLlena(PtlPosicionEnUso accion)
        {
            AutomatismoPosicion automatismoPosicion = _ptl.GetPosicion(accion.Ubicacion);

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                return !uow.PtlRepository.AnyPendientesSeperar(automatismoPosicion.TipoAgrupacion, automatismoPosicion.ComparteAgrupacion, TipoOperacion.Preparacion);
            }
        }
    }
}
