using Azure;
using MigraDoc.DocumentObjectModel.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.AutomationManager.Interfaces;
using WIS.AutomationManager.Models;
using WIS.Common.Extensions;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Impresiones;
using WIS.Domain.ManejoStock.Constants;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Extension;
using WIS.Security;

namespace WIS.AutomationManager.Services
{
    public class PtlCrossDockingEnUnaFaseService : PtlService
    {
        public PtlCrossDockingEnUnaFaseService(
           IUnitOfWorkFactory uowFactory,
           IUnitOfWorkInMemoryFactory uowFactoryMemory,
           IPtlInterpreterClientService interpretService,
           IAutomatismoWmsApiClientService wmsApiClientService,
           IPtl ptl,
           IIdentityService _identity) : base(uowFactory, uowFactoryMemory, interpretService, wmsApiClientService, ptl, _identity)
        {

        }

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

                    if (cantidadIngresada != 0)
                    {
                        uow.BeginTransaction();
                        uow.CreateTransactionNumber("ConfirmarLuzConCantidad PTL");

                        AutomatismoPosicion automatismoPosicion = _ptl.GetPosicion(luz.Ubicacion);
                        PtlDetailCrossDockingUnaFase datosCrossDocking = luz.GetDetail<PtlDetailCrossDockingUnaFase>();

                        ValidationsResult result = _wmsApiClientService.SepararCrossDocking(ptlUso.UserId, datosCrossDocking.NuPreparacion, datosCrossDocking.NuAgenda, datosCrossDocking.CodigoAgente, datosCrossDocking.TipoAgente, datosCrossDocking.Contenedor, datosCrossDocking.Ubicacion, luz.Empresa, luz.Producto, datosCrossDocking.NuIdentificador, cantidadIngresada, BarcodeDb.TIPO_CONTENEDOR_V);

                        if (result.HasError())
                        {
                            validationResult.Errors.AddRange(result.Errors);
                            this.NotificarLuzError(validationResult, luz);
                        }

                        uow.SaveChanges();
                        uow.Commit();
                    }

                    _uowMemory.Commit();
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
            var luz = _uowMemory.PosicionRepository.GetUbicacionesPrendidasByPosicion(accion.Ubicacion).FirstOrDefault(w => w.Estado == PtlEstadoPosicion.Cerrando);
            var validaciones = new List<Error>();

            AutomatismoPosicion posicionOrigen = _ptl.GetPosicion(accion.Ubicacion);
            AutomatismoPosicion posicionSalida = _ptl.GetPosiciones(AutomatismoPosicionesTipoDb.POS_SALIDA).FirstOrDefault();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (posicionSalida == null)
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_PtlUbicacionSalidaNoConfigurada"));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    validationResult.Errors.Add(new ValidationsError(0, true, messages));
                }
                else
                {
                    _uowMemory.BeginTransaction();
                    int numeroContenedorPosicion = posicionOrigen.GetNumeroVirtualEtiqueta();

                    try
                    {
                        PtlDetailCrossDockingUnaFase datosContenedorDestino = luz.GetDetail<PtlDetailCrossDockingUnaFase>();

                        var result = _wmsApiClientService.CambiarContenedor(numeroContenedorPosicion, luz.UserId, posicionSalida.IdUbicacion, datosContenedorDestino.Contenedor, TipoOperacion.CrossDockingUnaFase, posicionOrigen.TipoAgrupacion, posicionOrigen.GetComparteEntrega(), datosContenedorDestino.TipoEtiqueta);

                        if (result.HasError())
                            validationResult.Errors.AddRange(result.Errors);

                        if (validationResult.IsValid())
                        {

                            uow.CreateTransactionNumber("ConfirmarCierraUbicacion PTL");
                            uow.BeginTransaction();

                            posicionOrigen.LimpiarComparteAgrupacion();
                            posicionOrigen.Transaccion = uow.GetTransactionNumber();
                            uow.AutomatismoPosicionRepository.Update(posicionOrigen);

                            uow.SaveChanges();
                            uow.Commit();


                            _uowMemory.PosicionRepository.RemovePtlUbicacionPrendida(accion.Ubicacion, accion.Color, false);

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

        public override ValidationsResult ValidarOperacion(string color, int empresa, string producto)
        {
            var ubicPrendidas = _uowMemory.PosicionRepository.GetUbicacionesPrendidasByPtl(_ptl.Numero);
            var validaciones = new List<Error>();

            ValidationsResult result = new ValidationsResult();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (ubicPrendidas.Any(s => s.Empresa == empresa && s.Producto == producto))
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_PtlProductoAtendido"));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    result.Errors.Add(new ValidationsError(0, true, messages));
                }

                if (ubicPrendidas.Any(s => s.Color == color))
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_PtlOperativaSinFinalizar", ubicPrendidas.Where(s => s.Color == color).Count(), ubicPrendidas.FirstOrDefault().Producto));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    result.Errors.Add(new ValidationsError(0, true, messages));
                }
            }

            return result;
        }

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
                        var detail = accion.GetDetail<PtlDetailCrossDockingUnaFase>();
                        int tipoAgrupacion = _ptl.GetTipoAgrupacion();
                        var crossDockingList = this.GetDetalleCrossDockingAgrupadoCliente(detail.NuAgenda, detail.IdEspecificaIdentificador, detail.NuIdentificador, accion.Empresa, accion.Producto, tipoAgrupacion);


                        if (crossDockingList != null && crossDockingList.Count > 0)
                        {

                            uow.CreateTransactionNumber("Preder luces - Cross Docking en una fase");
                            uow.BeginTransaction();

                            List<AutomatismoPosicion> posicionesDisponibles = base.ReservarPosicionesByComparteAgrupaciones(crossDockingList.Select(w => w.Agrupacion).Distinct().ToList(), tipoAgrupacion);
                            string qtTotSepara = this.ProcesarAccion(uow, accion, detail, crossDockingList, posicionesDisponibles, result);
                            LiberarPosicionesNoUtilizadasEnAccion(posicionesDisponibles, accion);

                            uow.SaveChanges();
                            uow.Commit();

                            _uowMemory.Commit();

                            result.SuccessMessage = qtTotSepara;

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

        private string ProcesarAccion(IUnitOfWork uow, PtlPosicionEnUso accion, PtlDetailCrossDockingUnaFase detalle, List<LineaCrossDocking> crossDockingList, List<AutomatismoPosicion> posicionesDisponibles, ValidationsResult result)
        {
            decimal qtTotSepara = 0;
            decimal qtTotSepararDisponible = crossDockingList.Sum(w => w.Cantidad);
            decimal cantDisp = accion.Display.ToNumber<decimal>();
            decimal qtDisponible = cantDisp > qtTotSepararDisponible ? qtTotSepararDisponible : cantDisp;
            var validaciones = new List<Error>();

            if (posicionesDisponibles.Count() > 0)
            {
                List<PtlPosicionEnUso> colLucesPrender = new List<PtlPosicionEnUso>();


                foreach (var lineaAgrupadaCrossDocking in crossDockingList)
                {
                    if (qtDisponible == 0) break;

                    AutomatismoPosicion posicion = posicionesDisponibles.FirstOrDefault(w => w.ComparteAgrupacion == lineaAgrupadaCrossDocking.Agrupacion);

                    if (posicion != null)
                    {
                        PtlPosicionEnUso luz = new PtlPosicionEnUso();
                        PtlDetailCrossDockingUnaFase detalleLuz = new PtlDetailCrossDockingUnaFase();

                        luz.Clonar(accion);

                        detalleLuz.Clonar(detalle);
                        Agente agente = uow.AgenteRepository.GetAgente(lineaAgrupadaCrossDocking.Empresa, lineaAgrupadaCrossDocking.Cliente);
                        detalleLuz.Ubicacion = posicion.IdUbicacion;
                        detalleLuz.Contenedor = posicion.GetNumeroVirtualEtiqueta();
                        detalleLuz.CodigoAgente = agente.Codigo;
                        detalleLuz.TipoAgente = agente.Tipo;

                        luz.SetSerielizado(detalleLuz);

                        decimal auxQtSeparar = qtDisponible > lineaAgrupadaCrossDocking.Cantidad ? lineaAgrupadaCrossDocking.Cantidad : qtDisponible;
                        auxQtSeparar = auxQtSeparar < 0 ? 0 : auxQtSeparar;
                        luz.SetOperacion(PtlEstadoPosicion.Separando, auxQtSeparar.ToString(), posicion.Id);
                        colLucesPrender.Add(luz);
                        qtTotSepara += auxQtSeparar;
                        qtDisponible -= auxQtSeparar;
                    }
                }

                PrenderLuz(colLucesPrender, result);
            }
            else
            {
                validaciones.Add(new Error("WMSAPI_msg_Error_PtlSinPosicionesDisponibles"));
                var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                result.Errors.Add(new ValidationsError(0, true, messages));
                return string.Empty;
            }

            decimal pendienteXD = qtTotSepararDisponible - qtTotSepara;
            decimal dispAlmacenar = cantDisp - qtTotSepara - pendienteXD;
            dispAlmacenar = dispAlmacenar < 0 ? 0 : dispAlmacenar;


            if (qtTotSepara == 0)
            {
                validaciones.Add(new Error("WMSAPI_msg_Error_PtlNoSePuedenSepararCantidades"));
                var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                result.Errors.Add(new ValidationsError(0, true, messages));
                return string.Empty;
            }

            return new
            {
                PendienteCrossDocking = pendienteXD,
                Separar = qtTotSepara,
                Almacenar = dispAlmacenar,
            }.Serialize();
        }

        private List<LineaCrossDocking> GetDetalleCrossDockingAgrupadoCliente(int nuAgenda, string idEspecificaIdentificador, string lote, int cdEmpresa, string cdProduto, int tipoAgrupacion)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                return uow.PtlRepository.GetCrossDockingAgrupadoCliente(nuAgenda, idEspecificaIdentificador, lote, cdEmpresa, cdProduto, tipoAgrupacion);
            }
        }


        protected override bool IsPosicionLlena(PtlPosicionEnUso accion)
        {
            AutomatismoPosicion automatismoPosicion = _ptl.GetPosicion(accion.Ubicacion);

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                return !uow.PtlRepository.AnyPendientesSeperar(automatismoPosicion.TipoAgrupacion, automatismoPosicion.ComparteAgrupacion, TipoOperacion.CrossDockingUnaFase);
            }
        }
    }
}
