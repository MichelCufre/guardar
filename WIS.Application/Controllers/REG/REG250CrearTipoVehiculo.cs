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
    public class REG250CrearTipoVehiculo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ITrackingService _trackingService;

        public REG250CrearTipoVehiculo(IUnitOfWorkFactory uowFactory, IIdentityService identity, IFormValidationService formValidationService, ITrackingService trackingService)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._formValidationService = formValidationService;
            this._trackingService = trackingService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            form.GetField("descripcion").Value = string.Empty;
            form.GetField("volumen").Value = string.Empty;
            form.GetField("peso").Value = string.Empty;
            form.GetField("pallets").Value = string.Empty;
            form.GetField("porcentajeOcupacionVolumen").Value = "0";
            form.GetField("porcentajeOcupacionPeso").Value = "0";
            form.GetField("porcentajeOcupacionPallet").Value = "0";
            form.GetField("refrigerado").Value = false.ToString();
            form.GetField("cargaLateral").Value = false.ToString();
            form.GetField("admiteZorra").Value = false.ToString();
            form.GetField("soloCabina").Value = false.ToString();

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                string capacidadPallet = form.GetField("pallets").Value;
                string capacidadVolumen = form.GetField("volumen").Value;
                string capacidadPeso = form.GetField("peso").Value;
                string porcentajeOcupacionPeso = form.GetField("porcentajeOcupacionPeso").Value;
                string porcentajeOcupacionVolumen = form.GetField("porcentajeOcupacionVolumen").Value;
                string porcentajeOcupacionPallet = form.GetField("porcentajeOcupacionPallet").Value;

                var tipoVehiculo = new VehiculoEspecificacion
                {
                    Tipo = form.GetField("descripcion").Value,
                    AdmiteCargaLateral = bool.Parse(form.GetField("cargaLateral").Value),
                    AdmiteZorra = bool.Parse(form.GetField("admiteZorra").Value),
                    CapacidadPallet = string.IsNullOrEmpty(capacidadPallet) ? null : (decimal?)decimal.Parse(capacidadPallet, _identity.GetFormatProvider()),
                    CapacidadPeso = string.IsNullOrEmpty(capacidadPeso) ? null : (decimal?)decimal.Parse(capacidadPeso, _identity.GetFormatProvider()),
                    CapacidadVolumen = string.IsNullOrEmpty(capacidadVolumen) ? null : (decimal?)decimal.Parse(capacidadVolumen, _identity.GetFormatProvider()),
                    PorcentajeOcupacionPeso = string.IsNullOrEmpty(porcentajeOcupacionPeso) ? null : (short?)short.Parse(porcentajeOcupacionPeso),
                    PorcentajeOcupacionVolumen = string.IsNullOrEmpty(porcentajeOcupacionVolumen) ? null : (short?)short.Parse(porcentajeOcupacionVolumen),
                    PorcentajeOcupacionPallet = string.IsNullOrEmpty(porcentajeOcupacionPallet) ? null : (short?)short.Parse(porcentajeOcupacionPallet),
                    TieneRefrigeracion = bool.Parse(form.GetField("refrigerado").Value),
                    TieneSoloCabina = bool.Parse(form.GetField("soloCabina").Value)
                };

                uow.TipoVehiculoRepository.Add(tipoVehiculo);
                uow.SaveChanges();

                _trackingService.SincronizarTipoVehiculo(tipoVehiculo, true);
                uow.TipoVehiculoRepository.Update(tipoVehiculo);
                uow.SaveChanges();

                uow.Commit();
                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
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
