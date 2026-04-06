using System;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Domain.DataModel;
using WIS.Domain.Expedicion;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.REG
{
    public class REG250ModificarTipoVehiculo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ITrackingService _trackingService;

        public REG250ModificarTipoVehiculo(IUnitOfWorkFactory uowFactory, IIdentityService identity, IFormValidationService formValidationService, ITrackingService trackingService)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._formValidationService = formValidationService;
            this._trackingService = trackingService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var idTipoVehiculo = Convert.ToInt32(context.GetParameter("idTipoVehiculo"));

            VehiculoEspecificacion especificacion = uow.TipoVehiculoRepository.GetTipo(idTipoVehiculo);

            form.GetField("descripcion").Value = especificacion.Tipo;
            form.GetField("volumen").Value = especificacion.CapacidadVolumen.HasValue ? Convert.ToString(especificacion.CapacidadVolumen) : string.Empty;
            form.GetField("peso").Value = especificacion.CapacidadPeso.HasValue ? Convert.ToString(especificacion.CapacidadPeso) : string.Empty;
            form.GetField("pallets").Value = especificacion.CapacidadPallet.HasValue ? Convert.ToString(especificacion.CapacidadPallet) : string.Empty;
            form.GetField("porcentajeOcupacionVolumen").Value = especificacion.PorcentajeOcupacionVolumen.HasValue ? Convert.ToString(especificacion.PorcentajeOcupacionVolumen) : "0";
            form.GetField("porcentajeOcupacionPeso").Value = especificacion.PorcentajeOcupacionPeso.HasValue ? Convert.ToString(especificacion.PorcentajeOcupacionPeso) : "0";
            form.GetField("porcentajeOcupacionPallet").Value = especificacion.PorcentajeOcupacionPallet.HasValue ? Convert.ToString(especificacion.PorcentajeOcupacionPallet) : "0";
            form.GetField("refrigerado").Value = especificacion.TieneRefrigeracion.ToString();
            form.GetField("cargaLateral").Value = especificacion.AdmiteCargaLateral.ToString();
            form.GetField("admiteZorra").Value = especificacion.AdmiteZorra.ToString();
            form.GetField("soloCabina").Value = especificacion.TieneSoloCabina.ToString();

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var idTipoVehiculo = Convert.ToInt32(context.GetParameter("idTipoVehiculo"));

                string capacidadPallet = form.GetField("pallets").Value;
                string capacidadVolumen = form.GetField("volumen").Value;
                string capacidadPeso = form.GetField("peso").Value;
                string porcentajeOcupacionPeso = form.GetField("porcentajeOcupacionPeso").Value;
                string porcentajeOcupacionVolumen = form.GetField("porcentajeOcupacionVolumen").Value;
                string porcentajeOcupacionPallet = form.GetField("porcentajeOcupacionPallet").Value;

                VehiculoEspecificacion especificacion = uow.TipoVehiculoRepository.GetTipo(idTipoVehiculo);

                especificacion.Tipo = form.GetField("descripcion").Value;
                especificacion.AdmiteCargaLateral = bool.Parse(form.GetField("cargaLateral").Value);
                especificacion.AdmiteZorra = bool.Parse(form.GetField("admiteZorra").Value);
                especificacion.CapacidadPallet = string.IsNullOrEmpty(capacidadPallet) ? null : (decimal?)decimal.Parse(capacidadPallet, _identity.GetFormatProvider());
                especificacion.CapacidadPeso = string.IsNullOrEmpty(capacidadPeso) ? null : (decimal?)decimal.Parse(capacidadPeso, _identity.GetFormatProvider());
                especificacion.CapacidadVolumen = string.IsNullOrEmpty(capacidadVolumen) ? null : (decimal?)decimal.Parse(capacidadVolumen, _identity.GetFormatProvider());
                especificacion.PorcentajeOcupacionPeso = string.IsNullOrEmpty(porcentajeOcupacionPeso) ? null : (short?)short.Parse(porcentajeOcupacionPeso);
                especificacion.PorcentajeOcupacionVolumen = string.IsNullOrEmpty(porcentajeOcupacionVolumen) ? null : (short?)short.Parse(porcentajeOcupacionVolumen);
                especificacion.PorcentajeOcupacionPallet = string.IsNullOrEmpty(porcentajeOcupacionPallet) ? null : (short?)short.Parse(porcentajeOcupacionPallet);
                especificacion.TieneRefrigeracion = bool.Parse(form.GetField("refrigerado").Value);
                especificacion.TieneSoloCabina = bool.Parse(form.GetField("soloCabina").Value);

                uow.TipoVehiculoRepository.Update(especificacion);
                uow.SaveChanges();

                _trackingService.SincronizarTipoVehiculo(especificacion, true);
                uow.TipoVehiculoRepository.Update(especificacion);
                uow.SaveChanges();

                uow.Commit();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw ex;
            }
            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new CreateTipoVehiculoValidationModule(this._identity.GetFormatProvider()), form, context);
        }        
    }
}
