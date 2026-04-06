using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using WIS.AutomationManager.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Automatismo.Enums;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Integracion;
using WIS.Domain.Validation;
using WIS.Extension;
using WIS.Security;

namespace WIS.AutomationManager.Services
{
    public class AutomatismoValidationService : IAutomatismoValidationService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFormatProvider _culture;
        protected readonly IIdentityService _identity;

        private string _separador;

        public AutomatismoValidationService(IUnitOfWorkFactory uowFactory,
            IIdentityService identity)
        {
            _uowFactory = uowFactory;
            _identity = identity;
            _culture = identity.GetFormatProvider();
        }

        public virtual async Task<ValidationsResult> ValidateAutomatismo(IUnitOfWork uow, string zona)
        {
            var errors = new List<Error>();
            var result = new ValidationsResult();
            var context = new AutomatismoServiceContext(uow);

            await context.Load();

            ValidarAutomatismo(context, zona, errors);

            foreach (var e in errors)
            {
                var message = Translator.Translate(uow, e, _identity.UserId);
                result.AddError(message);
            }

            return result;
        }

        public virtual async Task<ValidationsResult> ValidateEnvioInterfazByCodigo(IUnitOfWork uow, string codigo, int cdInterfaz)
        {
            var errors = new List<Error>();
            var result = new ValidationsResult();
            var context = new AutomatismoServiceContext(uow);

            await context.Load();

            ValidarEnvioInterfazByCodigo(context, codigo, cdInterfaz, errors);

            foreach (var e in errors)
            {
                var message = Translator.Translate(uow, e, _identity.UserId);
                result.AddError(message);
            }

            return result;
        }

        public virtual async Task<ValidationsResult> ValidateEnvioInterfaz(IUnitOfWork uow, string zona, int cdInterfaz)
        {
            return (await ValidateEnvioInterfaz(uow, new List<string> { zona }, cdInterfaz)).Item1;
        }

        public virtual async Task<(ValidationsResult, Dictionary<string, IAutomatismo>)> ValidateEnvioInterfaz(IUnitOfWork uow, IEnumerable<string> zonas, int cdInterfaz)
        {
            var errors = new List<Error>();
            var result = new ValidationsResult();
            var context = new AutomatismoServiceContext(uow);

            await context.Load();

            ValidarEnvioInterfaz(context, zonas, cdInterfaz, errors);

            foreach (var e in errors)
            {
                var message = Translator.Translate(uow, e, _identity.UserId);
                result.AddError(message);
            }

            return (result, context.ZonaAutomatismo);
        }

        public virtual async Task<ValidationsResult> ValidarEnvioInterfazByPuesto(IUnitOfWork uow, string puesto, int cdInterfaz)
        {
            var errors = new List<Error>();
            var result = new ValidationsResult();
            var context = new AutomatismoServiceContext(uow);

            await context.Load();

            ValidarEnvioInterfazByPuesto(context, puesto, cdInterfaz, errors);

            foreach (var e in errors)
            {
                var message = Translator.Translate(uow, e, _identity.UserId);
                result.AddError(message);
            }

            return result;
        }

        public virtual List<Error> ValidateProductoAutomatismo(ProductoAutomatismoRequest request, AutomatismoProductoServiceContext context, out bool errorProcedimiento)
        {
            var errors = new List<Error>();

            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);

            errorProcedimiento = false;

            ProductoAutomatismoValidacionCarga(request, context, errors);

            if (errors.Any())
                return errors;

            ProductoAutomatismoValidacionProcedimiento(request, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return errors;
        }

        protected virtual void ProductoAutomatismoValidacionCarga(ProductoAutomatismoRequest request, AutomatismoProductoServiceContext context, List<Error> errors)
        {
            if (ValidarNoEsRequerido(request.Codigo, true, "Producto", errors))
                ValidarLargoMaximo(request.Codigo, 40, "Producto", errors);
        }

        protected virtual void ProductoAutomatismoValidacionProcedimiento(ProductoAutomatismoRequest request, AutomatismoProductoServiceContext context, List<Error> errors)
        {
            if (!context.ExisteProducto(request.Codigo))
                errors.Add(new Error("WMSAPI_msg_Error_AutomatismoProductoNoExiste", request.Codigo));
        }

        public virtual List<Error> ValidateCodigoBarrasAutomatismo(CodigoBarraAutomatismoRequest request, AutomatismoCodigoBarraServiceContext context, out bool errorProcedimiento)
        {
            var errors = new List<Error>();

            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);

            errorProcedimiento = false;

            CodigoBarrasAutomatismoValidacionCarga(request, context, errors);

            if (errors.Any())
                return errors;

            CodigoBarrasAutomatismoValidacionProcedimiento(request, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return errors;
        }

        protected virtual void CodigoBarrasAutomatismoValidacionCarga(CodigoBarraAutomatismoRequest request, AutomatismoCodigoBarraServiceContext context, List<Error> errors)
        {
            if (ValidarNoEsRequerido(request.Producto, true, "Producto", errors))
                ValidarLargoMaximo(request.Producto, 40, "Producto", errors);
        }

        protected virtual void CodigoBarrasAutomatismoValidacionProcedimiento(CodigoBarraAutomatismoRequest request, AutomatismoCodigoBarraServiceContext context, List<Error> errors)
        {
            if (!context.ExisteProducto(request.Producto))
                errors.Add(new Error("WMSAPI_msg_Error_AutomatismoProductoNoExiste", request.Producto));
        }

        public virtual List<Error> ValidateEntradaAutomatismo(EntradaStockAutomatismoRequest request, AutomatismoEntradaServiceContext context, out bool errorProcedimiento)
        {
            var errors = new List<Error>();

            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);

            errorProcedimiento = false;

            EntradaAutomatismoValidacionCarga(request, context, errors);

            if (errors.Any())
                return errors;

            EntradaAutomatismoValidacionProcedimiento(request, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return errors;
        }

        protected virtual void EntradaAutomatismoValidacionCarga(EntradaStockAutomatismoRequest request, AutomatismoEntradaServiceContext context, List<Error> errors)
        {
            foreach (var det in request.Detalles)
            {
                if (ValidarNoEsRequerido(det.Producto, true, "Producto", errors))
                    ValidarLargoMaximo(det.Producto, 40, "Producto", errors);
            }
        }

        protected virtual void EntradaAutomatismoValidacionProcedimiento(EntradaStockAutomatismoRequest request, AutomatismoEntradaServiceContext context, List<Error> errors)
        {
            foreach (var det in request.Detalles)
            {
                if (!context.ExisteProducto(det.Producto))
                    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoProductoNoExiste", det.Producto));
            }
        }

        public virtual List<Error> ValidateSalidaAutomatismo(SalidaStockLineaAutomatismoRequest request, AutomatismoSalidaServiceContext context, out bool errorProcedimiento)
        {
            var errors = new List<Error>();

            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);

            errorProcedimiento = false;

            SalidaAutomatismoValidacionCarga(request, context, errors);

            if (errors.Any())
                return errors;

            SalidaAutomatismoValidacionProcedimiento(request, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return errors;
        }

        protected virtual void SalidaAutomatismoValidacionCarga(SalidaStockLineaAutomatismoRequest request, AutomatismoSalidaServiceContext context, List<Error> errors)
        {
            if (ValidarNoEsRequerido(request.Producto, true, "Producto", errors))
                ValidarLargoMaximo(request.Producto, 40, "Producto", errors);
        }

        protected virtual void SalidaAutomatismoValidacionProcedimiento(SalidaStockLineaAutomatismoRequest request, AutomatismoSalidaServiceContext context, List<Error> errors)
        {
            if (!context.ExisteProducto(request.Producto))
                errors.Add(new Error("WMSAPI_msg_Error_AutomatismoProductoNoExiste", request.Producto));
        }

        public List<Error> ValidateUbicacionPickingAutomatismo(UbicacionPickingAutomatismoRequest request, AutomatismoUbicacionPickingServiceContext context, out bool errorProcedimiento)
        {
            var errors = new List<Error>();

            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);

            errorProcedimiento = false;

            UbicacionPickingAutomatismoValidacionCarga(request, context, errors);

            if (errors.Any())
                return errors;

            UbicacionPickingAutomatismoValidacionProcedimiento(request, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return errors;
        }

        protected virtual void UbicacionPickingAutomatismoValidacionCarga(UbicacionPickingAutomatismoRequest request, AutomatismoUbicacionPickingServiceContext context, List<Error> errors)
        {
            if (ValidarNoEsRequerido(request.Producto, true, "Producto", errors))
                ValidarLargoMaximo(request.Producto, 40, "Producto", errors);
        }

        protected virtual void UbicacionPickingAutomatismoValidacionProcedimiento(UbicacionPickingAutomatismoRequest request, AutomatismoUbicacionPickingServiceContext context, List<Error> errors)
        {
            if (!context.ExisteProducto(request.Producto))
                errors.Add(new Error("WMSAPI_msg_Error_AutomatismoProductoNoExiste", request.Producto));
        }

        public virtual List<Error> ValidateNotificacionAjustesAutomatismo(AjustesDeStockRequest request, AutomatismoNotificacionAjusteStockServiceContext context, out bool errorProcedimiento)
        {
            var errors = new List<Error>();

            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);

            errorProcedimiento = false;

            NotificacionAjusteAutomatismoValidacionCarga(request, context, errors);

            if (errors.Any())
                return errors;

            NotificacionAjusteAutomatismoValidacionProcedimiento(request, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return errors;
        }

        protected virtual void NotificacionAjusteAutomatismoValidacionCarga(AjustesDeStockRequest request, AutomatismoNotificacionAjusteStockServiceContext context, List<Error> errors)
        {
            ValidarInt(request.Empresa.ToString(), true, "Empresa", errors, true, true);

            foreach (var det in request.Ajustes)
            {
                if (ValidarNoEsRequerido(det.Producto, true, "Producto", errors))
                    ValidarLargoMaximo(det.Producto, 40, "Producto", errors);
            }
        }

        protected virtual void NotificacionAjusteAutomatismoValidacionProcedimiento(AjustesDeStockRequest request, AutomatismoNotificacionAjusteStockServiceContext context, List<Error> errors)
        {
            if (!context.ExisteUsuario())
                errors.Add(new Error("WMSAPI_msg_Error_AutomatismoUsuarioNoExiste", request.Usuario.LoginName));

            foreach (var det in request.Ajustes)
            {
                if (!context.ExisteProducto(request.Empresa, det.Producto))
                    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoProductoNoExiste", det.Producto));
            }
        }

        public virtual List<Error> ValidateConfirmacionEntradaAutomatismo(TransferenciaStockRequest request, AutomatismoConfirmacionEntradaServiceContext context, out bool errorProcedimiento)
        {
            var errors = new List<Error>();

            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);

            errorProcedimiento = false;

            ConfirmacionEntradaAutomatismoValidacionCarga(request, context, errors);

            if (errors.Any())
                return errors;

            ConfirmacionEntradaAutomatismoValidacionProcedimiento(request, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return errors;
        }

        protected virtual void ConfirmacionEntradaAutomatismoValidacionCarga(TransferenciaStockRequest request, AutomatismoConfirmacionEntradaServiceContext context, List<Error> errors)
        {
            ValidarInt(request.Empresa.ToString(), true, "Empresa", errors, true, true);

            foreach (var det in request.Transferencias)
            {
                if (ValidarNoEsRequerido(det.CodigoProducto, true, "Producto", errors))
                    ValidarLargoMaximo(det.CodigoProducto, 40, "Producto", errors);
            }
        }

        protected virtual void ConfirmacionEntradaAutomatismoValidacionProcedimiento(TransferenciaStockRequest request, AutomatismoConfirmacionEntradaServiceContext context, List<Error> errors)
        {
            if (!context.ExisteUsuario())
                errors.Add(new Error("WMSAPI_msg_Error_AutomatismoUsuarioNoExiste", request.Usuario.LoginName));

            //if (!request.UltimaOperacion)
            //{
            //    if (request.Transferencias == null || request.Transferencias.Count == 0)
            //    {
            //        errors.Add(new Error("WMSAPI_msg_Error_AutomatismoDetalleFaltanteCabezal"));
            //    }
            //}

            var entrada = context.GetAutomatismoEjecucionEntrada();
            if (entrada.InterfazExterna != CodigoInterfazAutomatismoDb.CD_INTERFAZ_ENTRADAS)
            {
                errors.Add(new Error("WMSAPI_msg_Error_AutomatismoEntradaInterfazIncorrecta", entrada.InterfazExterna));
            }
            if (entrada.Estado == EstadoEjecucion.PROCESADO_FIN)
            {
                errors.Add(new Error("WMSAPI_msg_Error_AutomatismoEntradaFinalizada", entrada.Id.ToString()));
            }
            else
            {
                foreach (var det in request.Transferencias)
                {
                    if (!context.ExisteProducto(request.Empresa, det.CodigoProducto))
                        errors.Add(new Error("WMSAPI_msg_Error_AutomatismoProductoNoExiste", det.CodigoProducto));

                    var loteConfirmado = string.IsNullOrEmpty(det.Identificador) ? ManejoIdentificadorDb.IdentificadorProducto : det.Identificador;
                    var productoOrden = context.GetProductoOrden(request.Empresa, det.CodigoProducto, loteConfirmado, det.IdLinea);

                    if (productoOrden == null)
                    {
                        errors.Add(new Error("WMSAPI_msg_Error_AutomatismoEntradaNoNotificada", det.IdLinea, det.CodigoProducto, loteConfirmado));
                    }
                    else
                    {
                        if (context.AnyConfirmadaFinalizada(request.IdEntrada, request.Empresa, det.CodigoProducto, loteConfirmado, det.IdLinea))
                        {
                            errors.Add(new Error("WMSAPI_msg_Error_AutomatismoEntradaLineaFinalizada", det.IdLinea, det.CodigoProducto, loteConfirmado));
                        }
                        //decimal cantidadConfirmada = context.GetCantidadConfirmada(request.IdEntrada, request.Empresa, det.CodigoProducto, loteConfirmado, det.IdLinea);
                        //if (det.Cantidad > (productoOrden.Cantidad - cantidadConfirmada))
                        //{
                        //    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoEntradaNotificadaCantidad", det.IdLinea, det.CodigoProducto, loteConfirmado));
                        //}
                    }
                }
            }
        }

        public virtual List<Error> ValidateConfirmacionSalidaAutomatismo(PickingRequest request, AutomatismoConfirmacionSalidaServiceContext context, out bool errorProcedimiento)
        {
            var errors = new List<Error>();

            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);

            errorProcedimiento = false;

            ConfirmacionSalidaAutomatismoValidacionCarga(request, context, errors);

            if (errors.Any())
                return errors;

            ConfirmacionSalidaAutomatismoValidacionProcedimiento(request, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return errors;
        }

        protected virtual void ConfirmacionSalidaAutomatismoValidacionCarga(PickingRequest request, AutomatismoConfirmacionSalidaServiceContext context, List<Error> errors)
        {
            ValidarInt(request.Empresa.ToString(), true, "Empresa", errors, true, true);

            foreach (var det in request.Detalles)
            {
                if (ValidarNoEsRequerido(det.CodigoProducto, true, "Producto", errors))
                    ValidarLargoMaximo(det.CodigoProducto, 40, "Producto", errors);
            }
        }

        protected virtual void ConfirmacionSalidaAutomatismoValidacionProcedimiento(PickingRequest request, AutomatismoConfirmacionSalidaServiceContext context, List<Error> errors)
        {
            if (!context.ExisteUsuario())
                errors.Add(new Error("WMSAPI_msg_Error_AutomatismoUsuarioNoExiste", request.Usuario.LoginName));

            if (request.EstadoSalida == AutomatismoEstadoSalidaGalys.CierreDeBulto)
            {
                if (request.Detalles == null || request.Detalles.Count == 0)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoDetalleFaltanteCabezal"));
                }
                foreach (var det in request.Detalles)
                {
                    if (!context.ExisteProducto(request.Empresa, det.CodigoProducto))
                        errors.Add(new Error("WMSAPI_msg_Error_AutomatismoProductoNoExiste", det.CodigoProducto));
                }
            }
            else if (request.EstadoSalida == AutomatismoEstadoSalidaGalys.OrdenFinalizada
                || request.EstadoSalida == AutomatismoEstadoSalidaGalys.OrdenCancelada)
            {
                if (request.DetallesFinalizados == null || request.DetallesFinalizados.Count == 0)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoDetalleFaltanteCabezal"));
                }
                foreach (var det in request.DetallesFinalizados)
                {
                    if (!context.ExisteProducto(request.Empresa, det.CodigoProducto))
                        errors.Add(new Error("WMSAPI_msg_Error_AutomatismoProductoNoExiste", det.CodigoProducto));
                }
            }
        }

        public virtual List<Error> ValidateConfirmacionMovimientoAutomatismo(TransferenciaStockRequest request, AutomatismoConfirmacionMovimientoServiceContext context, out bool errorProcedimiento)
        {
            var errors = new List<Error>();

            _separador = context.GetParametro(ParamManager.NUMBER_DECIMAL_SEPARATOR);

            errorProcedimiento = false;

            ConfirmacionMovimientoAutomatismoValidacionCarga(request, context, errors);

            if (errors.Any())
                return errors;

            ConfirmacionMovimientoAutomatismoValidacionProcedimiento(request, context, errors);

            if (errors.Any())
                errorProcedimiento = true;

            return errors;
        }

        protected virtual void ConfirmacionMovimientoAutomatismoValidacionCarga(TransferenciaStockRequest request, AutomatismoConfirmacionMovimientoServiceContext context, List<Error> errors)
        {
            ValidarInt(request.Empresa.ToString(), true, "Empresa", errors, true, true);

            foreach (var det in request.Transferencias)
            {
                if (ValidarNoEsRequerido(det.CodigoProducto, true, "Producto", errors))
                    ValidarLargoMaximo(det.CodigoProducto, 40, "Producto", errors);
            }
        }

        protected virtual void ConfirmacionMovimientoAutomatismoValidacionProcedimiento(TransferenciaStockRequest request, AutomatismoConfirmacionMovimientoServiceContext context, List<Error> errors)
        {
            if (!context.ExisteUsuario())
                errors.Add(new Error("WMSAPI_msg_Error_AutomatismoUsuarioNoExiste", request.Usuario.LoginName));

            //if (!request.UltimaOperacion)
            //{
            //    if (request.Transferencias == null || request.Transferencias.Count == 0)
            //    {
            //        errors.Add(new Error("WMSAPI_msg_Error_AutomatismoDetalleFaltanteCabezal"));
            //    }
            //}

            //var entrada = context.GetAutomatismoEjecucionMovimiento();
            //if (entrada.InterfazExterna != CodigoInterfazAutomatismoDb.CD_INTERFAZ_ENTRADAS)
            //{
            //    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoEntradaInterfazIncorrecta", entrada.InterfazExterna));
            //}
            //if (entrada.Estado == EstadoEjecucion.PROCESADO_FIN)
            //{
            //    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoEntradaFinalizada", entrada.Id.ToString()));
            //}
            //else
            //{
            foreach (var det in request.Transferencias)
            {
                if (!context.ExisteProducto(request.Empresa, det.CodigoProducto))
                    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoProductoNoExiste", det.CodigoProducto));

                //var loteConfirmado = string.IsNullOrEmpty(det.Identificador) ? ManejoIdentificadorDb.IdentificadorProducto : det.Identificador;
                //var productoOrden = context.GetProductoOrden(request.Empresa, det.CodigoProducto, loteConfirmado, det.IdLinea);

                //if (productoOrden == null)
                //{
                //    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoEntradaNoNotificada", det.IdLinea, det.CodigoProducto, loteConfirmado));
                //}
                //else
                //{
                //    if (context.AnyConfirmadaFinalizada(request.IdEntrada, request.Empresa, det.CodigoProducto, loteConfirmado, det.IdLinea))
                //    {
                //        errors.Add(new Error("WMSAPI_msg_Error_AutomatismoEntradaLineaFinalizada", det.IdLinea, det.CodigoProducto, loteConfirmado));
                //    }
                //    decimal cantidadConfirmada = context.GetCantidadConfirmada(request.IdEntrada, request.Empresa, det.CodigoProducto, loteConfirmado, det.IdLinea);
                //    if (det.Cantidad > (productoOrden.Cantidad - cantidadConfirmada))
                //    {
                //        errors.Add(new Error("WMSAPI_msg_Error_AutomatismoEntradaNotificadaCantidad", det.IdLinea, det.CodigoProducto, loteConfirmado));
                //    }
                //}
            }
            //}
        }

        protected virtual void ValidarAutomatismo(AutomatismoServiceContext context, string zona, List<Error> errors)
        {
            if (!context.ExisteAutomatismo(zona))
                errors.Add(new Error("WMSAPI_msg_Error_AutomatismoZonaNoExiste", zona));
        }

        protected virtual void ValidarEnvioInterfaz(AutomatismoServiceContext context, IEnumerable<string> zonas, int cdInterfaz, List<Error> errors)
        {
            foreach (var zona in zonas)
            {
                if (!context.ExisteAutomatismo(zona))
                    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoZonaNoExiste", zona));
                else
                {
                    var automatismo = context.GetAutomatismoZona(zona);
                    ValidarAutomatismo(context, automatismo, cdInterfaz, errors);
                }
            }
        }

        protected virtual void ValidarEnvioInterfazByCodigo(AutomatismoServiceContext context, string codigo, int cdInterfaz, List<Error> errors)
        {
            if (!context.ExisteAutomatismoByCodigo(codigo))
                errors.Add(new Error("WMSAPI_msg_Error_AutomatismoCodigoNoExiste", codigo));
            else
            {
                var automatismo = context.GetAutomatismoCodigo(codigo);
                ValidarAutomatismo(context, automatismo, cdInterfaz, errors);
                ValidarPosicionesAutomatismo(automatismo.Numero, automatismo.Posiciones, cdInterfaz, errors);
            }
        }

        protected virtual void ValidarEnvioInterfazByPuesto(AutomatismoServiceContext context, string puesto, int cdInterfaz, List<Error> errors)
        {
            if (!context.ExisteAutomatismoByPuesto(puesto))
                errors.Add(new Error("WMSAPI_msg_Error_AutomatismoCodigoPuestoNoExiste", puesto));
            else
            {
                var automatismo = context.GetAutomatismoPuesto(puesto);
                ValidarAutomatismo(context, automatismo, cdInterfaz, errors);
                ValidarPosicionesAutomatismo(automatismo.Numero, automatismo.Posiciones, cdInterfaz, errors);
            }
        }

        protected virtual void ValidarAutomatismo(AutomatismoServiceContext context, IAutomatismo automatismo, int cdInterfaz, List<Error> errors)
        {
            if (automatismo != null)
            {
                if (string.IsNullOrEmpty(automatismo.Tipo))
                    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoTipoNoDefinido", automatismo.Numero));

                var interfaz = context.GetAutomatismoInterfaz(automatismo, cdInterfaz);

                if (interfaz == null)
                    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoInterfazNoConfigurada", cdInterfaz.ToString(), automatismo.Numero.ToString()));
                else
                {
                    if (interfaz.ProtocoloComunicacion == ServiceHttpProtocol.UNKNOWN)
                        errors.Add(new Error("WMSAPI_msg_Error_AutomatismoInterfazProtocoloNoConfigurado", cdInterfaz.ToString()));
                    else
                    {
                        if (interfaz.IntegracionServicio == null)
                            errors.Add(new Error("WMSAPI_msg_Error_AutomatismoSinServicioIntegracion"));
                    }
                }
            }
        }

        protected virtual void ValidarPosicionesAutomatismo(int numAutomatismo, List<AutomatismoPosicion> colAutomatismoPosicionint, int cdInterfaz, List<Error> errors)
        {
            if (colAutomatismoPosicionint == null || !colAutomatismoPosicionint.Any())
                errors.Add(new Error("WMSAPI_msg_Error_AutomatismoNoTienePosicion", numAutomatismo));
            else
            {
                switch (cdInterfaz)
                {
                    case CodigoInterfazAutomatismoDb.CD_INTERFAZ_NOTIF_AJUSTES:

                        if (!colAutomatismoPosicionint.Any(a => a.TipoUbicacion == AutomatismoPosicionesTipoDb.POS_AJUSTE))
                            errors.Add(new Error("WMSAPI_msg_Error_AutomatismoNoTieneUbicacionAjuste", numAutomatismo));

                        break;
                }
            }
        }

        protected virtual bool ValidarNoEsRequerido(string value, bool isRequired, string campo, List<Error> errors)
        {
            if (string.IsNullOrEmpty(value) && isRequired)
            {
                errors.Add(new Error("WMSAPI_msg_Error_AutomatismoCampoRequerido", campo));
                return false;
            }

            return true;
        }

        protected virtual bool ValidarLargoMaximo(string value, int length, string campo, List<Error> errors)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > length)
            {
                errors.Add(new Error("WMSAPI_msg_Error_AutomatismoLargoMaximo", campo));
                return false;
            }

            return true;
        }

        protected virtual bool ValidarInt(string value, bool isRequired, string campo, List<Error> errors, bool distintoCero = false, bool noNegativo = false)
        {
            if (this.ValidarNoEsRequerido(value, isRequired, campo, errors))
            {
                if (!int.TryParse(value, out int parsedValue))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoCampoNoNumerico", campo));
                    return false;
                }
                else
                {
                    return this.ValidarNumero(parsedValue, isRequired, campo, errors, distintoCero, noNegativo);
                }
            }

            return false;
        }

        protected virtual bool ValidarShort(string value, bool isRequired, string campo, List<Error> errors, bool distintoCero = false, bool noNegativo = false)
        {
            if (this.ValidarNoEsRequerido(value, isRequired, campo, errors))
            {
                if (!short.TryParse(value, out short parsedValue))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoCampoNoNumerico", campo));
                    return false;
                }
                else
                {
                    return this.ValidarNumero(parsedValue, isRequired, campo, errors, distintoCero, noNegativo);
                }
            }

            return false;
        }

        protected virtual bool ValidarDecimal(string value, bool isRequired, string campo, List<Error> errors, int maxLength, int precision, bool distintoCero = false, bool noNegativo = false)
        {
            if (this.ValidarNoEsRequerido(value, isRequired, campo, errors))
            {
                if (!Validations.TryParse_Decimal(value.Replace("-", ""), maxLength, precision, _separador, _culture, out decimal parsedValue, out string msg))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoCampoNoNumerico", campo));
                    return false;
                }
                else
                {
                    return this.ValidarNumero((int)parsedValue, isRequired, campo, errors, distintoCero, noNegativo);
                }
            }

            return false;
        }

        protected virtual bool ValidarNumero(int parsedValue, bool isRequired, string campo, List<Error> errors, bool distintoCero = false, bool noNegativo = false)
        {

            if (noNegativo && parsedValue < 0)
            {
                errors.Add(new Error("WMSAPI_msg_Error_AutomatismoCampoNoNegativo", campo));
                return false;
            }

            return true;
        }

        protected virtual bool ValidarDateTime(string value, bool isRequired, string campo, List<Error> errors)
        {
            if (this.ValidarNoEsRequerido(value, isRequired, campo, errors))
            {
                if (!DateTimeExtension.IsValid_DDMMYYYY(value, _culture))
                {
                    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoIncumplimientoFormatoFecha", campo));
                    return false;
                }

                return true;

            }

            return false;
        }

        protected virtual bool ValidarFechaMenorA(string campo, string fecha, string fechaAux, string campoAux, List<Error> errors, bool hoy = false)
        {
            if (!string.IsNullOrEmpty(fecha))
            {
                try
                {
                    DateTime? date = DateTimeExtension.FromString_DDMMYYYY(fecha, _culture);
                    DateTime? fechaMin = DateTimeExtension.FromString_DDMMYYYY(fechaAux, _culture);

                    if (date < fechaMin)
                    {
                        if (hoy)
                            errors.Add(new Error("WMSAPI_msg_Error_AutomatismoFechaMenorActual", campo));
                        else
                            errors.Add(new Error("WMSAPI_msg_Error_AutomatismoFechaMenorARango", new string[] { campo, fecha, campoAux ?? fechaAux }));

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoErrorNoControlado", campo, ex.Message));
                    return false;
                }
            }
            return true;
        }

        protected virtual bool ValidarSoN(string value, bool isRequired, string campo, List<Error> errors)
        {
            if (this.ValidarNoEsRequerido(value, isRequired, campo, errors))
            {
                if (value != "N" && value != "S")
                {
                    errors.Add(new Error("WMSAPI_msg_Error_AutomatismoCampoSoN", campo));

                    return false;
                }

                return true;

            }

            return false;
        }
    }
}
