using MigraDoc.DocumentObjectModel.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.AutomationManager.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Picking;
using WIS.Domain.Validation;
using WIS.Security;

namespace WIS.AutomationManager.Services
{
    public class PtlPickingService : PtlService
    {
        public PtlPickingService(
           IUnitOfWorkFactory uowFactory,
           IUnitOfWorkInMemoryFactory uowFactoryMemory,
           IPtlInterpreterClientService interpretService,
           IAutomatismoWmsApiClientService wmsApiClientService,
           IPtl ptl,
		   IIdentityService _identity) : base(uowFactory, uowFactoryMemory, interpretService, wmsApiClientService, ptl, _identity)
        {

        }

        public override ValidationsResult ConfirmarCerrarUbicacion(PtlPosicionEnUso accion)
        {
            throw new NotImplementedException();
        }

        public override ValidationsResult ValidarOperacion(string color, int empresa, string producto)
        {
            var ubicPrendidas = _uowMemory.PosicionRepository.GetUbicacionesPrendidasByPtl(_ptl.Numero);
            var result = new ValidationsResult();
			var validaciones = new List<Error>();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                if (ubicPrendidas.Any(s => s.Color == color))
                {
					validaciones.Add(new Error("WMSAPI_msg_Error_PtlOperativaSinFinalizarPtl"));
					var messages = Translator.Translate(uow, validaciones, _identity.UserId);
					result.Errors.Add(new ValidationsError(0, true, messages));
                }
                else if (ubicPrendidas.Any(s => s.Agrupacion == producto))
                {
					validaciones.Add(new Error("WMSAPI_msg_Error_PtlAgrupacionAtendida"));
					var messages = Translator.Translate(uow, validaciones, _identity.UserId);
					result.Errors.Add(new ValidationsError(0, true, messages));
                }
            }

            return result;
        }

        public override ValidationsResult PrenderLuces(PtlPosicionEnUso accion)
        {
            var result = this.ValidarOperacion(accion.Color, accion.Empresa, string.Empty);
			var validaciones = new List<Error>();

			if (result.IsValid())
            {
                try
                {
                    _uowMemory.BeginTransaction();

                    var detail = accion.GetDetail<PtlDetailPicking>();
                    var tipoAgrupacion = _ptl.GetTipoAgrupacion();
                    var detallesPreparacionPendientes = this.GetDetallesPreparacionDisponibles(detail.Preparacion, detail.Cliente, detail.ComparteContenedorPicking, detail.SubClase);
                    using (var uow = _uowFactory.GetUnitOfWork())
                    {
                        if (detallesPreparacionPendientes != null && detallesPreparacionPendientes.Count > 0)
                        {

                            uow.BeginTransaction();

                            ProcesarAccion(accion, detallesPreparacionPendientes, result);

                            uow.SaveChanges();
                            uow.Commit();


                            _uowMemory.Commit();

                            result.SuccessMessage = "Ok";
                        }
                        else
                        {
                            validaciones.Add(new Error("WMSAPI_msg_Error_PtlNoMercaderiaASeparar"));
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(0, true, messages));
                        }
                    }
                }
                catch (Exception ex)
                {
                    _uowMemory.Rollback();
                    result.AddError($"Error no controlado:{ex}");
                }
            }

            return result;
        }

        protected virtual List<DetallePreparacion> GetDetallesPreparacionDisponibles(int preparacion, string CdCliente, string vlComparteContenedorPicking, string subClase)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                return uow.PtlRepository.GetDetallesPreparacionDisponibles(preparacion, CdCliente, vlComparteContenedorPicking, subClase);
            }
        }

        protected virtual List<DetallePreparacion> GetDetallesPreparacion(IUnitOfWork uow, int preparacion, int empresa, string cliente, string ubicacion)
        {
            return uow.PtlRepository.GetDetallesPreparacion(preparacion, empresa, cliente, ubicacion);
        }

        protected virtual void ProcesarAccion(PtlPosicionEnUso accion, List<DetallePreparacion> detallesPreparacionPendientes, ValidationsResult result)
        {
            var lucesPrender = new List<PtlPosicionEnUso>();

            foreach (var detalle in detallesPreparacionPendientes)
            {
                var posicion = _ptl.GetPosiciones(AutomatismoPosicionesTipoDb.POS_PICKING)
                    .FirstOrDefault(w => w.IdUbicacion == detalle.Ubicacion);

                if (posicion != null)
                {
                    var luz = new PtlPosicionEnUso();
                    luz.Clonar(accion);

                    decimal auxQtSeparar = detalle.Cantidad;
                    auxQtSeparar = auxQtSeparar < 0 ? 0 : auxQtSeparar;
                    luz.SetOperacion(PtlEstadoPosicion.Picking, auxQtSeparar.ToString(), posicion.Id);
                    lucesPrender.Add(luz);
                }
            }

            PrenderLuz(lucesPrender, result);
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

                var ptlUso = _uowMemory.ColorRepository.GetColorEnUso(accion.Ptl, accion.Color);
                var cantidadOriginal = decimal.Parse(luz.Display);
                var cantidadIngresada = cantidadOriginal;

                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    if (!string.IsNullOrEmpty(accion.Display))
                    {
                        var intCantidadDisplay = decimal.Parse(accion.Display);

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

                        var automatismoPosicion = _ptl.GetPosicion(luz.Ubicacion);
                        var datosPicking = luz.GetDetail<PtlDetailPicking>();

                        var detallesPicking = this.GetDetallesPreparacion(uow, datosPicking.Preparacion, luz.Empresa, datosPicking.Cliente, automatismoPosicion.IdUbicacion)
                            .Select(d => new DetallePickingRequest
                            {
                                Preparacion = d.NumeroPreparacion,
                                Ubicacion = d.Ubicacion,
                                Pedido = d.Pedido,
                                Cantidad = d.Cantidad,
                                CodigoAgente = d.CodigoAgente,
                                TipoAgente = d.TipoAgente,
                                CodigoProducto = d.Producto,
                                Identificador = d.Lote
                            });

                        var nueva = new List<DetallePickingRequest>();
                        var cantDisp = cantidadIngresada;

                        foreach (var det in detallesPicking)
                        {
                            det.IdExternoContenedor = datosPicking.Contenedor.ToString();
                            det.TipoContenedor = datosPicking.TipoContenedor;
                            det.UbicacionContenedor = datosPicking.UbicacionEquipo;

                            if (det.Cantidad <= cantDisp)
                                nueva.Add(det);
                            else
                            {
                                det.Cantidad = cantDisp;
                                nueva.Add(det);
                            }

                            cantDisp -= det.Cantidad;

                            if (cantDisp == 0)
                                break;
                        }

                        var result = _wmsApiClientService.Picking(nueva, luz.Empresa);

                        if (result.HasError())
                        {
                            validationResult.Errors.AddRange(result.Errors);
                            this.NotificarLuzError(validationResult, luz);
                        }

                        uow.SaveChanges();
                        uow.Commit();


                        _uowMemory.Commit();
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

        protected override bool IsPosicionLlena(PtlPosicionEnUso posicion)
        {
            return false;
        }

        public override List<AutomatismoPtl> GetPtlByTipo()
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                return uow.PtlRepository.GetAutomatismoByTipoPtl(PtlTipoDb.PICKING);
            }
        }

        public override ValidationsResult UpdateLuzByPtlColor(PtlColorActivoRequest colorActivo)
        {
            var validationResult = new ValidationsResult();

            try
            {
                var lucesPrendidasByPtlColor = GetLightsOn().Where(i => i.Ptl == _ptl.Numero && i.Color == colorActivo.Color);

                foreach (var luz in lucesPrendidasByPtlColor)
                {
                    var detail = JsonConvert.DeserializeObject<PtlDetailPicking>(luz.Detalle);

                    detail.Contenedor = int.Parse(colorActivo.Contenedor);

                    luz.Detalle = JsonConvert.SerializeObject(detail);

                    _uowMemory.SaveChanges();
                }
            }
            catch (Exception e)
            {
                validationResult.AddError(e.Message);
            }

            return validationResult;
        }

        public override ValidationsResult ValidatePtlReferencia(string referencia)
        {
			var validaciones = new List<Error>();
			var result = new ValidationsResult();
            var referenciaEnUso = _uowMemory.PosicionRepository.AnyPtlReferenciaEnUso(_ptl.Numero, referencia);

            if (referenciaEnUso)
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    validaciones.Add(new Error("WMSAPI_msg_Error_PtlYaExisteReferencia"));
                    var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                    result.Errors.Add(new ValidationsError(0, true, messages));
                }
			}

			return result;
        }
    }
}
