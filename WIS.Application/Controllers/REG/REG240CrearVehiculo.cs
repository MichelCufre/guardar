using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.Expedicion;
using WIS.Domain.Expedicion.Enums;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Tracking;
using WIS.Domain.Tracking.Models;
using WIS.Domain.Tracking.Validation;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.REG
{
    public class REG240CrearVehiculo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ITrackingService _trackingService;

        public REG240CrearVehiculo(IIdentityService identity, IUnitOfWorkFactory uowFactory, IFormValidationService formValidationService, ITrackingService trackingService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._trackingService = trackingService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            form.GetField("descripcion").Value = string.Empty;
            form.GetField("predio").Value = string.Empty;
            form.GetField("descripcion").Value = string.Empty;
            form.GetField("transportista").Value = string.Empty;
            form.GetField("tipo").Value = string.Empty;
            form.GetField("disponibilidadDesde").Value = string.Empty;
            form.GetField("disponibilidadHasta").Value = string.Empty;

            this.InicializarSelects(form);

            if (_identity.Predio != GeneralDb.PredioSinDefinir)
            {
                form.GetField("predio").Value = _identity.Predio;
                form.GetField("predio").ReadOnly = true;
            }
            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                string transportista = form.GetField("transportista").Value;
                string tipoVehiculo = form.GetField("tipo").Value;

                Domain.Expedicion.VehiculoEspecificacion especificacion = null;

                if (!string.IsNullOrEmpty(tipoVehiculo))
                    especificacion = uow.TipoVehiculoRepository.GetTipo(int.Parse(tipoVehiculo));

                var vehiculo = new Vehiculo
                {
                    Descripcion = form.GetField("descripcion").Value,
                    Predio = form.GetField("predio").Value,
                    Estado = EstadoVehiculoDb.Disponible,
                    Marca = form.GetField("marca").Value,
                    Matricula = form.GetField("placa").Value,
                    Transportista = int.Parse(form.GetField("transportista").Value),
                    HoraDisponibilidadDesde = TimeSpan.Parse(form.GetField("disponibilidadDesde").Value, this._identity.GetFormatProvider()),
                    HoraDisponibilidadHasta = TimeSpan.Parse(form.GetField("disponibilidadHasta").Value, this._identity.GetFormatProvider()),
                    Caracteristicas = especificacion
                };

                uow.VehiculoRepository.Add(vehiculo);
                uow.SaveChanges();

                _trackingService.SincronizarVehiculo(uow, vehiculo, true);
                uow.VehiculoRepository.Update(vehiculo);
                uow.TipoVehiculoRepository.Update(vehiculo.Caracteristicas);
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

            return this._formValidationService.Validate(new CreateVehiculoFormValidationModule(uow, this._identity.UserId, this._identity.Predio, this._identity.GetFormatProvider()), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "tipo":
                    return this.SearchTipoVehiculo(form, context);
                case "transportista":
                    return this.SearchTransportista(form, context);
                default:
                    return new List<SelectOption>();
            }
        }

        public virtual List<SelectOption> SearchTipoVehiculo(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Domain.Expedicion.VehiculoEspecificacion> especificaciones = uow.TipoVehiculoRepository.GetTipoByDescripcionOrCodePartial(context.SearchValue);

            foreach (Domain.Expedicion.VehiculoEspecificacion especificacion in especificaciones)
            {
                opciones.Add(new SelectOption(especificacion.Id.ToString(), especificacion.Tipo));
            }

            return opciones;
        }
        public virtual List<SelectOption> SearchTransportista(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Transportista> transportistas = uow.TransportistaRepository.GetByDescripcionOrCodePartial(context.SearchValue);

            foreach (Transportista transportista in transportistas)
            {
                opciones.Add(new SelectOption(transportista.Id.ToString(), transportista.Descripcion));
            }

            return opciones;
        }

        public virtual void InicializarSelects(Form form)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            FormField selectPredio = form.GetField("predio");
            selectPredio.Options = new List<SelectOption>();

            var dbQuery = new GetPrediosUsuarioQuery();

            uow.HandleQuery(dbQuery);

            List<Predio> predios = dbQuery.GetPrediosUsuario(_identity.UserId);
            foreach (var predio in predios)
            {
                selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - { predio.Descripcion}")); ;
            }

            if (_identity.Predio != GeneralDb.PredioSinDefinir)
                selectPredio.Value = _identity.Predio;
            else if (predios.Count == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;
        }
    }
}
