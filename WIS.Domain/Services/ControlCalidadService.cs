using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.General.Enums;
using WIS.Domain.Interfaces;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Domain.Validation;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class ControlCalidadService : IControlCalidadService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly IValidationService _validationService;
        protected readonly IIdentityService _identity;
        protected readonly ControlCalidadResponse _response;

        public ControlCalidadService(
            IUnitOfWorkFactory uowFactory,
            IOptions<MaxItemsSettings> configuration,
            IValidationService validationService,
            IIdentityService identity)
        {
            _configuration = configuration;
            _uowFactory = uowFactory;
            _validationService = validationService;
            _identity = identity;
            _response = new ControlCalidadResponse();
        }

        #region Asignacion

        public virtual async Task<ValidationsResult> AsignarControlCalidad(List<ControlCalidadAPI> controles, int userId, int empresa)
        {
            var result = new ValidationsResult();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (controles.Count > 0)
                {
                    uow.CreateTransactionNumber(_identity.Application, CInterfazExterna.ControlDeCalidad.ToString(), _identity.UserId);

                    int nroRegistro = 1;
                    var keys = new HashSet<string>();
                    int maxItems = _configuration.Value.ControlDeCalidad;

                    if (!_validationService.ValidateMaxItems(result, nroRegistro, controles.Count, maxItems))
                        return result;

                    var context = await this.GetNewServiceContext(uow, controles, userId, empresa);

                    foreach (var control in controles)
                    {
                        context.AsignarTipoCriterioControl(control);

                        var validaciones = await _validationService.ValidateControlCalidad(control, context, out bool errorProcedimiento);

                        if (validaciones.Count > 0)
                        {
                            var messages = Translator.Translate(uow, validaciones, _identity.UserId);
                            result.Errors.Add(new ValidationsError(nroRegistro, errorProcedimiento, messages));
                        }

                        nroRegistro++;
                    }

                    if (result.HasError())
                        return result;

                    await this.ControlCalidadProcess(uow, controles, context, empresa);
                }
            }

            return result;
        }

        public virtual async Task ControlCalidadProcess(IUnitOfWork uow, List<ControlCalidadAPI> controles, IControlCalidadServiceContext context, int empresa)
        {
            List<ControlCalidadAPI> paraAsociar = controles.Where(x => x.Estado == ControlCalidadOperacion.Asociar).ToList();
            List<ControlCalidadAPI> paraAprobar = controles.Where(x => x.Estado == ControlCalidadOperacion.Aprobar).ToList();

            List<ControlDeCalidadPendiente> toAddControlesPendientes = new List<ControlDeCalidadPendiente>();
            List<ControlDeCalidadPendiente> toUpdateControlesPendientes = new List<ControlDeCalidadPendiente>();
            List<LpnDetalle> toUpdateLpnDetalle = new List<LpnDetalle>();
            List<Stock> toUpdateStock = new List<Stock>();

            if (paraAsociar.Any())
                this.AsignarControlesDeCalidad(uow, context, paraAsociar, toAddControlesPendientes, toUpdateLpnDetalle, toUpdateStock);

            if (paraAprobar.Any())
                this.AprobarControlesDeCalidad(uow, context, paraAprobar, toAddControlesPendientes, toUpdateControlesPendientes, toUpdateLpnDetalle, toUpdateStock);

            await uow.ControlDeCalidadRepository.AddOrUpdateControlDeCalidadPendiente(
                toAddControlesPendientes,
                toUpdateControlesPendientes,
                toUpdateLpnDetalle,
                toUpdateStock);

            _response.ProcesadoOk = true;
        }

        public virtual void AsignarIdsNuevosControlesPendientes(List<ControlDeCalidadPendiente> toAddControlesPendientes, IControlCalidadServiceContext context)
        {
            context.LoadNewIds(toAddControlesPendientes.Count);
            foreach (var nuevoControl in toAddControlesPendientes)
            {
                nuevoControl.Id = context.NextNuevaIdValue;
            }
        }

        public virtual bool ControlPendienteYaIngresado(List<ControlDeCalidadPendiente> toAddControlesPendientes, int control, string predio, string producto, int empresa, string lote, decimal faixa, int? etiqueta, long? nuLpn)
        {
            return toAddControlesPendientes.Any(w => w.Codigo == control &&
                    w.NroLPN == nuLpn &&
                    w.Etiqueta == etiqueta &&
                    w.Predio == predio &&
                    w.Ubicacion == null &&
                    w.Producto == producto &&
                    w.Faixa == faixa &&
                    w.Identificador == lote &&
                    w.Empresa == empresa &&
                    !w.Aceptado);
        }

        public virtual bool ControlPendienteYaIngresado(List<ControlDeCalidadPendiente> toAddControlesPendientes, int control, string predio, string ubicacion, string producto, int empresa, string lote, decimal faixa, long? nuLpn)
        {
            return toAddControlesPendientes.Any(w => w.Codigo == control &&
                    w.NroLPN == nuLpn &&
                    w.Etiqueta == null &&
                    w.Predio == predio &&
                    w.Ubicacion == ubicacion &&
                    w.Producto == producto &&
                    w.Faixa == faixa &&
                    w.Identificador == lote &&
                    w.Empresa == empresa &&
                    !w.Aceptado);
        }

        public virtual bool ControlPendienteYaIngresado(List<ControlDeCalidadPendiente> toAddControlesPendientes, int control, string predio, string producto, int empresa, string lote, decimal faixa, int? etiqueta)
        {
            return toAddControlesPendientes.Any(w => w.Codigo == control &&
                    w.Etiqueta == etiqueta &&
                    w.NroLPN == null &&
                    w.Predio == predio &&
                    w.Ubicacion == null &&
                    w.Producto == producto &&
                    w.Faixa == faixa &&
                    w.Identificador == lote &&
                    w.Empresa == empresa &&
                    !w.Aceptado);
        }

        public virtual bool ControlPendienteYaIngresado(List<ControlDeCalidadPendiente> toAddControlesPendientes, int control, string predio, string ubicacion, string producto, int empresa, string lote, decimal faixa)
        {
            return toAddControlesPendientes.Any(w => w.Codigo == control &&
                    w.NroLPN == null &&
                    w.Etiqueta == null &&
                    w.Predio == predio &&
                    w.Ubicacion == ubicacion &&
                    w.Producto == producto &&
                    w.Faixa == faixa &&
                    w.Identificador == lote &&
                    w.Empresa == empresa &&
                    !w.Aceptado);
        }

        public virtual ControlDeCalidadPendiente GenerarControlesPendientesEtiqueta(
            CriterioControlCalidadAPI criterio,
            ControlCalidadAPI control,
            IControlCalidadServiceContext context,
            EtiquetaLote etiqueta,
            long instancia)
        {
            if (!context.ExisteControlPendiente(
                criterio.Predio,
                control.CodigoControlCalidad,
                criterio.Producto,
                criterio.Lote,
                criterio.Faixa,
                criterio.Empresa,
                etiqueta.Numero))
                return context.GenerarControlCalidadEtiquetaProcess(
                        criterio,
                        control.CodigoControlCalidad,
                        control.Descripcion,
                        etiqueta, instancia);

            return null;
        }

        public virtual List<ControlDeCalidadPendiente> GenerarControlesPendientesEtiqueta(
           CriterioControlCalidadAPI criterio,
           ControlCalidadAPI control,
           IControlCalidadServiceContext context,
           EtiquetaLote etiqueta,
           Lpn lpn,
           long instancia)
        {
            List<ControlDeCalidadPendiente> toReturn = new List<ControlDeCalidadPendiente>();

            foreach (LpnDetalle detalle in lpn.Detalles.Where(x =>
                x.CodigoProducto == criterio.Producto &&
                x.Lote == criterio.Lote &&
                x.Empresa == criterio.Empresa))
                if (!context.ExisteControlPendiente(criterio.Predio, control.CodigoControlCalidad, criterio.Producto, criterio.Lote, criterio.Faixa, criterio.Empresa, lpn.NumeroLPN, detalle.Id))
                {
                    LpnDetalle lpnDetalle = detalle;
                    ControlDeCalidadPendiente controlPendiente = context.GenerarControlCalidadEtiquetaProcess(criterio, control.CodigoControlCalidad, control.Descripcion, etiqueta, lpn, instancia, lpnDetalle);

                    if (controlPendiente != null)
                        toReturn.Add(controlPendiente);
                }

            return toReturn;
        }

        public virtual List<ControlDeCalidadPendiente> GenerarControlesPendientesLpn(
            CriterioControlCalidadAPI criterio,
            ControlCalidadAPI control,
            IControlCalidadServiceContext context,
            Lpn lpn,
            long instancia,
            out List<LpnDetalle> toUpdateLpnDetalle)
        {
            List<ControlDeCalidadPendiente> toReturn = new List<ControlDeCalidadPendiente>();
            toUpdateLpnDetalle = new List<LpnDetalle>();

            foreach (LpnDetalle detalle in lpn.Detalles.Where(x =>
                x.CodigoProducto == criterio.Producto &&
                x.Lote == criterio.Lote &&
                x.Empresa == criterio.Empresa))
                if (!context.ExisteControlPendiente(criterio.Predio, control.CodigoControlCalidad, criterio.Producto, criterio.Lote, criterio.Faixa, criterio.Empresa, lpn.NumeroLPN, detalle.Id))
                {
                    LpnDetalle lpnDetalle = detalle;

                    ControlDeCalidadPendiente controlPendiente = context.GenerarControlCalidadLpnProcess(criterio, control.CodigoControlCalidad, control.Descripcion, lpn, instancia, ref lpnDetalle);

                    if (controlPendiente != null)
                    {
                        toReturn.Add(controlPendiente);
                        toUpdateLpnDetalle.Add(lpnDetalle);
                    }
                }

            return toReturn;
        }

        public virtual ControlDeCalidadPendiente GenerarControlesPendientesUbicacion(
            CriterioControlCalidadAPI criterio,
            ControlCalidadAPI control,
            IControlCalidadServiceContext context,
            long instancia,
            ref Stock stock)
        {
            if (!context.ExisteControlPendiente(
                criterio.Predio,
                control.CodigoControlCalidad,
                criterio.Producto,
                criterio.Lote,
                criterio.Faixa,
                criterio.Empresa,
                stock.Ubicacion))
                return context.GenerarControlCalidadUbicacionProcess(
                        criterio,
                        control.CodigoControlCalidad,
                        control.Descripcion,
                        instancia,
                        ref stock);

            return null;
        }

        #endregion

        #region Aprobacion

        public virtual void AprobarControlesDeCalidad(
            IUnitOfWork uow,
            IControlCalidadServiceContext context,
            List<ControlCalidadAPI> paraAprobar,
            List<ControlDeCalidadPendiente> controlesNuevos,
            List<ControlDeCalidadPendiente> toUpdateControlesPendientes,
            List<LpnDetalle> toUpdateLpnDetalle,
            List<Stock> toUpdateStock)
        {
            AceptacionControlesCalidad aceptacion = new AceptacionControlesCalidad(uow, _identity.UserId);

            List<ControlDeCalidadPendiente> toApproveEtiquetaDisponibles = context.GetPorAprobarEtiqueta();
            List<ControlDeCalidadPendiente> toApproveUbicacionDisponibles = context.GetPorAprobarUbicacion();

            List<ControlDeCalidadPendiente> toApproveEtiqueta = new List<ControlDeCalidadPendiente>();
            List<ControlDeCalidadPendiente> toApproveUbicacion = new List<ControlDeCalidadPendiente>();

            foreach (ControlCalidadAPI controlParaAprobar in paraAprobar)
            {
                foreach (CriterioControlCalidadAPI criterio in controlParaAprobar.Criterios)
                {
                    switch (criterio.Operacion)
                    {
                        case ControlCalidadCriterio.Ubicacion:

                            List<ControlDeCalidadPendiente> controlesAprobarUbicacion = toApproveUbicacionDisponibles.Where(w => w.Codigo == controlParaAprobar.CodigoControlCalidad && w.Ubicacion == criterio.Ubicacion && w.Producto == criterio.Producto && w.Empresa == criterio.Empresa && w.Identificador == criterio.Lote && w.Faixa == criterio.Faixa).ToList();
                            if (controlesAprobarUbicacion.Count > 0)
                            {
                                toApproveUbicacion.AddRange(controlesAprobarUbicacion);
                            }
                            break;

                        case ControlCalidadCriterio.LPN:

                            Lpn etiquetaLpnAprobar = null;
                            if (string.IsNullOrEmpty(criterio.TipoEtiquetaExterna))
                            {
                                etiquetaLpnAprobar = context.GetEtiquetaLpnExterno(criterio.Predio, criterio.EtiquetaExterna);
                            }
                            else
                            {
                                etiquetaLpnAprobar = context.GetEtiquetaLpnExterno(criterio.Predio, criterio.EtiquetaExterna, criterio.TipoEtiquetaExterna);
                            }

                            List<ControlDeCalidadPendiente> conntrolesAprobarLpn = toApproveUbicacionDisponibles.Where(w => w.Codigo == controlParaAprobar.CodigoControlCalidad && w.NroLPN == etiquetaLpnAprobar.NumeroLPN && w.Producto == criterio.Producto && w.Empresa == criterio.Empresa && w.Identificador == criterio.Lote && w.Faixa == criterio.Faixa).ToList();
                            if (conntrolesAprobarLpn.Count > 0)
                            {
                                toApproveUbicacion.AddRange(conntrolesAprobarLpn);
                            }
                            break;

                        case ControlCalidadCriterio.Etiqueta:

                            EtiquetaLote etiqueta = null;
                            if (string.IsNullOrEmpty(criterio.TipoEtiquetaExterna))
                            {
                                etiqueta = context.GetEtiquetaRecepcionExterno(criterio.Predio, criterio.EtiquetaExterna);
                            }
                            else
                            {
                                etiqueta = context.GetEtiquetaRecepcionExterno(criterio.Predio, criterio.EtiquetaExterna, criterio.TipoEtiquetaExterna);
                            }

                            List<ControlDeCalidadPendiente> conntrolesAprobarEtiqueta = toApproveEtiquetaDisponibles.Where(w => w.Codigo == controlParaAprobar.CodigoControlCalidad && w.Etiqueta == etiqueta.Numero && w.Producto == criterio.Producto && w.Empresa == criterio.Empresa && w.Identificador == criterio.Lote && w.Faixa == criterio.Faixa).ToList();
                            if (conntrolesAprobarEtiqueta.Count > 0)
                            {
                                toApproveEtiqueta.AddRange(conntrolesAprobarEtiqueta);
                            }
                            break;

                        case ControlCalidadCriterio.Producto:

                            List<ControlDeCalidadPendiente> conntrolesAprobarProductoEtiqueta = toApproveEtiquetaDisponibles.Where(w => w.Codigo == controlParaAprobar.CodigoControlCalidad && w.Producto == criterio.Producto && w.Empresa == criterio.Empresa && w.Identificador == criterio.Lote && w.Faixa == criterio.Faixa).ToList();
                            if (conntrolesAprobarProductoEtiqueta.Count > 0)
                            {
                                toApproveEtiqueta.AddRange(conntrolesAprobarProductoEtiqueta);
                            }

                            List<ControlDeCalidadPendiente> conntrolesAprobarUbicacionProducto = toApproveUbicacionDisponibles.Where(w => w.Codigo == controlParaAprobar.CodigoControlCalidad && w.Producto == criterio.Producto && w.Empresa == criterio.Empresa && w.Identificador == criterio.Lote && w.Faixa == criterio.Faixa).ToList();
                            if (conntrolesAprobarUbicacionProducto.Count > 0)
                            {
                                toApproveUbicacion.AddRange(conntrolesAprobarUbicacionProducto);
                            }

                            break;
                    }
                }

                _response.AddControl(controlParaAprobar);
            }

            this.AprobarControlesDeCalidadEtiqueta(uow, aceptacion, toApproveEtiqueta);
            this.AprobarControlesDeCalidadUbicacion(uow, context, aceptacion, controlesNuevos, toApproveUbicacion, toApproveUbicacionDisponibles, toUpdateLpnDetalle, toUpdateStock);

            if (toApproveEtiqueta.Any())
                _response.AddAprobados(toApproveEtiqueta);
            if (toApproveUbicacion.Any())
                _response.AddAprobados(toApproveUbicacion);

            toUpdateControlesPendientes.AddRange(toApproveEtiqueta);
            toUpdateControlesPendientes.AddRange(toApproveUbicacion);
        }

        public virtual void AsignarControlesDeCalidad(
            IUnitOfWork uow,
            IControlCalidadServiceContext context,
            List<ControlCalidadAPI> paraAsociar,
            List<ControlDeCalidadPendiente> toAddControlesPendientes,
            List<LpnDetalle> toUpdateLpnDetalle,
            List<Stock> toUpdateStock)
        {
            context.LoadNewInstancias(paraAsociar.Count);

            foreach (ControlCalidadAPI control in paraAsociar)
            {
                if (context.NextNuevaInstancia)
                {
                    long instancia = context.NextNuevaInstanciaValue;
                    foreach (CriterioControlCalidadAPI criterio in control.Criterios)
                        switch (criterio.Operacion)
                        {
                            case ControlCalidadCriterio.Etiqueta:
                                EtiquetaLote etiquetaCriterio;
                                etiquetaCriterio =
                                    criterio.TipoEtiquetaExterna != null
                                        ? context.GetEtiquetaRecepcionExterno(
                                            criterio.Predio,
                                            criterio.EtiquetaExterna,
                                            criterio.TipoEtiquetaExterna)
                                        : context.GetEtiquetaRecepcionExterno(
                                            criterio.Predio,
                                            criterio.EtiquetaExterna);

                                if (!this.ControlPendienteYaIngresado(
                                    toAddControlesPendientes,
                                    control.CodigoControlCalidad,
                                    criterio.Predio,
                                    criterio.Producto,
                                    criterio.Empresa,
                                    criterio.Lote,
                                    criterio.Faixa,
                                    etiquetaCriterio.Numero))
                                {
                                    Lpn etiquetaRecepcionLpnCriterio =
                                        criterio.TipoEtiquetaExterna != null
                                            ? context.GetEtiquetaLpnExterno(
                                                criterio.Predio,
                                                criterio.EtiquetaExterna,
                                                criterio.TipoEtiquetaExterna)
                                            : context.GetEtiquetaLpnExterno(
                                                criterio.Predio,
                                                criterio.EtiquetaExterna);

                                    if (etiquetaRecepcionLpnCriterio == null)
                                    {
                                        ControlDeCalidadPendiente nuevoControlEtiqueta = this.GenerarControlesPendientesEtiqueta(
                                        criterio,
                                        control,
                                        context,
                                        etiquetaCriterio,
                                        instancia);

                                        if (nuevoControlEtiqueta != null)
                                            toAddControlesPendientes.Add(nuevoControlEtiqueta);
                                    }
                                    else
                                    {
                                        if (!this.ControlPendienteYaIngresado(
                                            toAddControlesPendientes,
                                            control.CodigoControlCalidad,
                                            criterio.Predio,
                                            criterio.Producto,
                                            criterio.Empresa,
                                            criterio.Lote,
                                            criterio.Faixa,
                                            etiquetaCriterio.Numero,
                                            etiquetaRecepcionLpnCriterio.NumeroLPN))
                                        {

                                            List<ControlDeCalidadPendiente> nuevosControlesEtiquetaLpn = this.GenerarControlesPendientesEtiqueta(
                                            criterio,
                                            control,
                                            context,
                                            etiquetaCriterio,
                                            etiquetaRecepcionLpnCriterio,
                                            instancia);

                                            if (nuevosControlesEtiquetaLpn != null && nuevosControlesEtiquetaLpn.Count > 0)
                                                toAddControlesPendientes.AddRange(nuevosControlesEtiquetaLpn);
                                        }

                                    }

                                }

                                break;
                            case ControlCalidadCriterio.LPN:
                                Lpn lpnCriterio;
                                lpnCriterio =
                                    criterio.TipoEtiquetaExterna != null
                                        ? context.GetEtiquetaLpnExterno(
                                            criterio.Predio,
                                            criterio.EtiquetaExterna,
                                            criterio.TipoEtiquetaExterna)
                                        : context.GetEtiquetaLpnExterno(
                                            criterio.Predio,
                                            criterio.EtiquetaExterna);

                                if (!this.ControlPendienteYaIngresado(toAddControlesPendientes, control.CodigoControlCalidad, criterio.Predio, lpnCriterio.Ubicacion, criterio.Producto, criterio.Empresa, criterio.Lote, criterio.Faixa, lpnCriterio.NumeroLPN))
                                {
                                    List<int> idsDetallesConControlPendiente = lpnCriterio.Detalles.Where(x =>
                                                                                                   x.CodigoProducto == criterio.Producto &&
                                                                                                   x.Lote == criterio.Lote &&
                                                                                                   x.Empresa == criterio.Empresa && x.IdCtrlCalidad == EstadoControlCalidad.Pendiente).Select(s => s.Id).ToList();



                                    List<ControlDeCalidadPendiente> controlesPendientesLpn = this.GenerarControlesPendientesLpn(criterio, control, context, lpnCriterio, instancia, out List<LpnDetalle> toUpdate);
                                    if (controlesPendientesLpn != null && controlesPendientesLpn.Count > 0)
                                    {
                                        toAddControlesPendientes.AddRange(controlesPendientesLpn);
                                        toUpdate = toUpdate.Where(w => !idsDetallesConControlPendiente.Contains(w.Id)).ToList();

                                        if (toUpdate.Count > 0)
                                        {
                                            toUpdateLpnDetalle.AddRange(toUpdate);
                                        }

                                    }
                                }

                                break;
                            case ControlCalidadCriterio.Ubicacion:

                                if (!this.ControlPendienteYaIngresado(
                                    toAddControlesPendientes,
                                    control.CodigoControlCalidad,
                                    criterio.Predio,
                                    criterio.Ubicacion,
                                    criterio.Producto,
                                    criterio.Empresa,
                                    criterio.Lote,
                                    criterio.Faixa))
                                {
                                    Stock stockCriterio =
                                context.GetStock(
                                    criterio.Predio,
                                    criterio.Producto,
                                    criterio.Lote,
                                    criterio.Empresa,
                                    criterio.Ubicacion,
                                    criterio.Faixa);

                                    bool stockConControlPendiente = (stockCriterio.ControlCalidad == EstadoControlCalidad.Pendiente);
                                    ControlDeCalidadPendiente controlUbicacion = this.GenerarControlesPendientesUbicacion(criterio, control, context, instancia, ref stockCriterio);

                                    if (controlUbicacion != null)
                                    {
                                        toAddControlesPendientes.Add(controlUbicacion);

                                        if (!stockConControlPendiente)
                                            toUpdateStock.Add(stockCriterio);
                                    }
                                }

                                break;
                            case ControlCalidadCriterio.Producto:
                                List<Lpn> lpns =
                                    context.GetEtiquetaLpnExterno(
                                        criterio.Predio,
                                        criterio.Producto,
                                        criterio.Empresa,
                                        criterio.Lote,
                                        criterio.Faixa);

                                List<EtiquetaLote> etiquetas =
                                    context.GetEtiquetaRecepcionExterno(
                                        criterio.Predio,
                                        criterio.Producto,
                                        criterio.Empresa,
                                        criterio.Lote,
                                        criterio.Faixa);

                                List<Stock> stocks =
                                    context.GetStock(
                                        criterio.Predio,
                                        criterio.Producto,
                                        criterio.Lote,
                                        criterio.Empresa,
                                        criterio.Faixa);

                                foreach (Lpn lpn in lpns)
                                {
                                    if (!this.ControlPendienteYaIngresado(
                                        toAddControlesPendientes,
                                        control.CodigoControlCalidad,
                                        criterio.Predio,
                                        lpn.Ubicacion,
                                        criterio.Producto,
                                        criterio.Empresa,
                                        criterio.Lote,
                                        criterio.Faixa,
                                        lpn.NumeroLPN))
                                    {

                                        if (!etiquetas.Any(a => a.NroLpn == lpn.NumeroLPN))
                                        {
                                            List<int> idsDetallesConControlPendiente = lpn.Detalles.Where(x =>
                                                                                                    x.CodigoProducto == criterio.Producto &&
                                                                                                    x.Lote == criterio.Lote &&
                                                                                                    x.Empresa == criterio.Empresa && x.IdCtrlCalidad == EstadoControlCalidad.Pendiente).Select(s => s.Id).ToList();

                                            List<ControlDeCalidadPendiente> controlesPendientesLpn = this.GenerarControlesPendientesLpn(
                                                criterio,
                                                control,
                                                context,
                                                lpn, instancia,
                                                out List<LpnDetalle> toUpdateDetalle);

                                            if (controlesPendientesLpn != null && controlesPendientesLpn.Count > 0)
                                            {
                                                toAddControlesPendientes.AddRange(controlesPendientesLpn);
                                                toUpdateDetalle = toUpdateDetalle.Where(w => !idsDetallesConControlPendiente.Contains(w.Id)).ToList();

                                                if (toUpdateDetalle.Count > 0)
                                                {
                                                    toUpdateLpnDetalle.AddRange(toUpdateDetalle);
                                                }
                                            }
                                        }
                                    }
                                }

                                foreach (EtiquetaLote etiqueta in etiquetas)
                                {
                                    if (!this.ControlPendienteYaIngresado(
                                        toAddControlesPendientes,
                                        control.CodigoControlCalidad,
                                        criterio.Predio,
                                        criterio.Producto,
                                        criterio.Empresa,
                                        criterio.Lote,
                                        criterio.Faixa,
                                        etiqueta.Numero))
                                    {
                                        Lpn etiquetaRecepcionLpnCriterio = lpns.FirstOrDefault(f => f.NumeroLPN == etiqueta.NroLpn);

                                        if (etiquetaRecepcionLpnCriterio == null)
                                        {
                                            ControlDeCalidadPendiente nuevoControlEtiqueta = this.GenerarControlesPendientesEtiqueta(
                                            criterio,
                                            control,
                                            context,
                                            etiqueta,
                                            instancia);

                                            if (nuevoControlEtiqueta != null)
                                                toAddControlesPendientes.Add(nuevoControlEtiqueta);
                                        }
                                        else
                                        {
                                            if (!this.ControlPendienteYaIngresado(
                                           toAddControlesPendientes,
                                           control.CodigoControlCalidad,
                                           criterio.Predio,
                                           criterio.Producto,
                                           criterio.Empresa,
                                           criterio.Lote,
                                           criterio.Faixa,
                                           etiqueta.Numero,
                                           etiquetaRecepcionLpnCriterio.NumeroLPN))
                                            {

                                                List<ControlDeCalidadPendiente> nuevosControlesEtiquetaLpn = this.GenerarControlesPendientesEtiqueta(
                                                criterio,
                                                control,
                                                context,
                                                etiqueta,
                                                etiquetaRecepcionLpnCriterio,
                                                instancia);

                                                if (nuevosControlesEtiquetaLpn != null && nuevosControlesEtiquetaLpn.Count > 0)
                                                    toAddControlesPendientes.AddRange(nuevosControlesEtiquetaLpn);
                                            }

                                        }
                                    }
                                }

                                foreach (Stock stock in stocks)
                                {
                                    Stock toUpdateItem = stock;
                                    if (!this.ControlPendienteYaIngresado(
                                        toAddControlesPendientes,
                                        control.CodigoControlCalidad,
                                        criterio.Predio,
                                        stock.Ubicacion,
                                        criterio.Producto,
                                        criterio.Empresa,
                                        criterio.Lote,
                                        criterio.Faixa))
                                    {
                                        List<EtiquetaLote> etiquetasUbicacionStock = etiquetas.Where(w => w.IdUbicacion == stock.Ubicacion).ToList();
                                        if (context.TieneStockLibre(stock, etiquetasUbicacionStock))
                                        {
                                            bool stockConControlPendiente = (stock.ControlCalidad == EstadoControlCalidad.Pendiente);
                                            ControlDeCalidadPendiente controlUbicacion = this.GenerarControlesPendientesUbicacion(
                                                criterio,
                                                control,
                                                context,
                                                instancia,
                                                ref toUpdateItem);

                                            if (controlUbicacion != null)
                                            {
                                                toAddControlesPendientes.Add(controlUbicacion);
                                                if (!stockConControlPendiente)
                                                    toUpdateStock.Add(toUpdateItem);
                                            }
                                        }
                                    }
                                }

                                break;
                        }

                    _response.AddControl(control, instancia);
                }
            }

            AsignarIdsNuevosControlesPendientes(toAddControlesPendientes, context);
        }

        public virtual void AprobarControlesDeCalidadEtiqueta(
            IUnitOfWork uow,
            AceptacionControlesCalidad aceptacion,
            List<ControlDeCalidadPendiente> toApprove)
        {
            aceptacion.CargarObjetosBulkAceptarControlesEtiqueta(toApprove);
        }

        public virtual void AprobarControlesDeCalidadUbicacion(
            IUnitOfWork uow,
            IControlCalidadServiceContext context,
            AceptacionControlesCalidad aceptacion,
            List<ControlDeCalidadPendiente> controlesNuevos,
            List<ControlDeCalidadPendiente> toApprove,
            List<ControlDeCalidadPendiente> toApproveDisponibles,
            List<LpnDetalle> toUpdateDetallesLpn,
            List<Stock> toUpdateStock)
        {
            aceptacion.CargarObjetosBulkAceptarControlesUbicacion(
                controlesNuevos,
                toApprove,
                toApproveDisponibles,
                context,
                toUpdateDetallesLpn,
                toUpdateStock);
        }

        #endregion

        #region Get

        public virtual async Task<IControlCalidadServiceContext> GetNewServiceContext(IUnitOfWork uow, List<ControlCalidadAPI> controles, int userId, int empresa)
        {
            var context = new ControlCalidadServiceContext(uow, controles, userId, empresa);

            AddParametros(context, empresa);

            await context.Load();

            return context;
        }

        public virtual void AddParametros(IServiceContext context, int empresa)
        {
        }

        public virtual ControlCalidadResponse GetResponse(InterfazEjecucion ejecucion)
        {
            _response.CodigoEmpresa = ejecucion.Empresa ?? 1;
            _response.NumeroInterfaz = ejecucion.Id;
            _response.CodigoInterfaz = ejecucion.CdInterfazExterna ?? 0;
            return _response;
        }

        #endregion
    }
}
