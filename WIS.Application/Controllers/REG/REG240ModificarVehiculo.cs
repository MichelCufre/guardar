using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.REG
{
    public class REG240ModificarVehiculo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ITrackingService _trackingService;

        public REG240ModificarVehiculo(IIdentityService identity, IUnitOfWorkFactory uowFactory, IFormValidationService formValidationService, ITrackingService trackingService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._trackingService = trackingService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            FormField fieldEstado = form.GetField("estado");

            using var uow = this._uowFactory.GetUnitOfWork();

            List<DominioDetalle> dominios = uow.VehiculoRepository.GetEstadosEditables();

            var mapper = new VehiculoMapper(new TipoVehiculoMapper());

            foreach (var dominio in dominios)
            {
                fieldEstado.Options.Add(new SelectOption(dominio.Id, dominio.Descripcion));
            }

            int vehiculoId = int.Parse(context.GetParameter("vehiculo"));

            Vehiculo vehiculo = uow.VehiculoRepository.GetVehiculo(vehiculoId);

            fieldEstado.Value = vehiculo.Estado;
            form.GetField("descripcion").Value = vehiculo.Descripcion;
            form.GetField("placa").Value = vehiculo.Matricula;
            form.GetField("marca").Value = vehiculo.Marca;
            form.GetField("disponibilidadDesde").Value = vehiculo.HoraDisponibilidadDesde.ToString(@"hh\:mm", this._identity.GetFormatProvider());
            form.GetField("disponibilidadHasta").Value = vehiculo.HoraDisponibilidadHasta.ToString(@"hh\:mm", this._identity.GetFormatProvider());

            this.InicializarSelects(uow, form, vehiculo);

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                int vehiculoId = int.Parse(context.GetParameter("vehiculo"));

                Vehiculo vehiculo = uow.VehiculoRepository.GetVehiculo(vehiculoId);

                var mapper = new VehiculoMapper(new TipoVehiculoMapper());

                string transportista = form.GetField("transportista").Value;
                string tipoVehiculo = form.GetField("tipo").Value;

                Domain.Expedicion.VehiculoEspecificacion especificacion = null;

                if (!string.IsNullOrEmpty(tipoVehiculo))
                    especificacion = uow.TipoVehiculoRepository.GetTipo(int.Parse(tipoVehiculo));

                vehiculo.Descripcion = form.GetField("descripcion").Value;
                vehiculo.Predio = form.GetField("predio").Value;
                vehiculo.Estado = form.GetField("estado").Value;
                vehiculo.Marca = form.GetField("marca").Value;
                vehiculo.Matricula = form.GetField("placa").Value;
                vehiculo.Transportista = int.Parse(form.GetField("transportista").Value);
                vehiculo.HoraDisponibilidadDesde = TimeSpan.Parse(form.GetField("disponibilidadDesde").Value, this._identity.GetFormatProvider());
                vehiculo.HoraDisponibilidadHasta = TimeSpan.Parse(form.GetField("disponibilidadHasta").Value, this._identity.GetFormatProvider());
                vehiculo.Caracteristicas = especificacion;

                uow.VehiculoRepository.Update(vehiculo);
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

            return this._formValidationService.Validate(new UpdateVehiculoFormValidationModule(uow, this._identity.UserId, this._identity.Predio, this._identity.GetFormatProvider()), form, context);
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
        public virtual void InicializarSelects(IUnitOfWork uow, Form form, Vehiculo vehiculo)
        {
            FormField fieldTransportista = form.GetField("transportista");
            FormField fieldTipoVehiculo = form.GetField("tipo");

            fieldTransportista.Options = SearchTransportista(form, new FormSelectSearchContext
            {
                SearchValue = Convert.ToString(vehiculo.Transportista)
            });

            fieldTransportista.Value = Convert.ToString(vehiculo.Transportista);

            fieldTipoVehiculo.Options = SearchTipoVehiculo(form, new FormSelectSearchContext
            {
                SearchValue = Convert.ToString(vehiculo.Caracteristicas.Id)
            });

            fieldTipoVehiculo.Value = Convert.ToString(vehiculo.Caracteristicas.Id);

            FormField selectPredio = form.GetField("predio");

            selectPredio.Options = new List<SelectOption>();


            // Predios
            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            List<Predio> predios = dbQuery.GetPrediosUsuario(_identity.UserId);
            foreach (var predio in predios)
            {
                selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - { predio.Descripcion}")); ;
            }
            selectPredio.Value = vehiculo.Predio;

        }
    }
}
