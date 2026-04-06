using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.Expedicion;
using WIS.Domain.Expedicion.Enums;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.EXP
{
    public class EXP040CrearEgreso : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ITrackingService _trackingService;
        protected readonly IParameterService _parameterService;

        public EXP040CrearEgreso(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            ISecurityService security,
            ITrackingService trackingService,
            IParameterService parameterService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
            this._trackingService = trackingService;
            this._parameterService = parameterService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {

            form.GetField("descripcion").Value = string.Empty;
            form.GetField("codigoRuta").Value = string.Empty;
            form.GetField("codigoEmpresa").Value = string.Empty;
            form.GetField("respetaOrdenCarga").Value = "false";
            form.GetField("habilitarRuteo").Value = "false";
            form.GetField("controlContenedores").Value = "false";
            form.GetField("codigoPuerta").Value = string.Empty;
            form.GetField("codigoVehiculo").Value = string.Empty;
            form.GetField("matricula").Value = string.Empty;
            form.GetField("matricula").ReadOnly = false;

            form.GetField("codigoTransportista").Value = string.Empty;
            form.GetField("codigoTransportista").ReadOnly = false;

            using var uow = this._uowFactory.GetUnitOfWork();
            // Predios
            FormField selectPredio = form.GetField("predio");
            selectPredio.Options = new List<SelectOption>();
            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            var predios = dbQuery.GetPrediosUsuario(_identity.UserId).OrderBy(x => x.Numero);
            foreach (var predio in predios)
            {
                selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}")); ;
            }

            if (predios.Count() == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;

            if (!this._identity.Predio.Equals(GeneralDb.PredioSinDefinir))
                selectPredio.Value = this._identity.Predio;

            form.GetField("habilitarTracking").Value = "false";
            form.GetField("habilitarTracking").Disabled = true;

            if (_trackingService.TrackingHabilitado())
            {
                form.GetField("habilitarTracking").Value = "true";
                form.GetField("habilitarTracking").Disabled = false;
            }
            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new CreateEgresoFormValidationModule(uow, this._identity, this._security), form, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            string empresa = form.GetField("codigoEmpresa").Value;
            string puerta = form.GetField("codigoPuerta").Value;
            string ruta = form.GetField("codigoRuta").Value;
            string vehiculo = form.GetField("codigoVehiculo").Value;

            using var uow = this._uowFactory.GetUnitOfWork();

            ExpedicionConfiguracionService expedicionService = new ExpedicionConfiguracionService(uow, this._parameterService, new ParametroMapper());

            int? empresaValue = string.IsNullOrEmpty(empresa) ? null : (int?)int.Parse(empresa);

            uow.CreateTransactionNumber("EXP040 Alta Camion");

            Camion camion = new Camion
            {
                Empresa = empresaValue,
                Puerta = string.IsNullOrEmpty(puerta) ? null : (short?)short.Parse(puerta),
                Ruta = string.IsNullOrEmpty(ruta) ? null : (short?)short.Parse(ruta),
                Transportista = int.Parse(form.GetField("codigoTransportista").Value),
                Vehiculo = string.IsNullOrEmpty(vehiculo) ? null : (short?)short.Parse(vehiculo),
                Descripcion = form.GetField("descripcion").Value,
                Estado = CamionEstado.AguardandoCarga,
                FechaCreacion = DateTime.Now,
                IsCierreParcialHabilitado = expedicionService.IsCierreParcialHabilitado(empresaValue ?? -1),
                Matricula = form.GetField("matricula").Value,
                Predio = form.GetField("predio").Value,
                IsTrackingHabilitado = bool.Parse(form.GetField("habilitarTracking").Value),
                RespetaOrdenCarga = bool.Parse(form.GetField("respetaOrdenCarga").Value),
                IsRuteoHabilitado = bool.Parse(form.GetField("habilitarRuteo").Value),
                TipoArmadoEgreso = TipoArmadoEgreso.Manual,
                ArmadoEgreso = TipoArmadoEgreso.ArmadoManual,
                IsCargaAutomaticaHabilitada = false,
                IsCierreAutomaticoHabilitado = false,
                IsControlContenedoresHabilitado = bool.Parse(form.GetField("controlContenedores").Value),
                NumeroTransaccion = uow.GetTransactionNumber(),
                ArmadoHabilitado = true,
            };

            uow.CamionRepository.AddCamion(camion);

            uow.SaveChanges();

            context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");

            return form;
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            return context.FieldId switch
            {
                "codigoEmpresa" => this.SearchEmpresa(form, context),
                "codigoRuta" => this.SearchRuta(form, context),
                "codigoPuerta" => this.SearchPuerta(form, context),
                "codigoTransportista" => this.SearchTransportista(form, context),
                "codigoVehiculo" => this.SearchVehiculo(form, context),
                _ => new List<SelectOption>(),
            };
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchRuta(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            string predio = form.GetField("predio").Value;

            if (string.IsNullOrEmpty(predio))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Ruta> rutas = uow.RutaRepository.GetByDescripcionOrCodePartial(context.SearchValue, predio);

            foreach (var ruta in rutas)
            {
                string rutaDesc = $"{ruta.Id} - {ruta.Descripcion}";
                if (!string.IsNullOrEmpty(ruta.Zona))
                    rutaDesc += $"- {ruta.Zona}";

                opciones.Add(new SelectOption(ruta.Id.ToString(), rutaDesc));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchPuerta(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            string predio = form.GetField("predio").Value;

            if (string.IsNullOrEmpty(predio))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            List<PuertaEmbarque> puertas = uow.PuertaEmbarqueRepository.GetByDescripcionOrCodePartial(context.SearchValue, predio);
            puertas = puertas.FindAll(p => p.Tipo != TipoPuertaEmbarqueDb.Entrada);

            foreach (var puerta in puertas)
            {
                opciones.Add(new SelectOption(puerta.Id.ToString(), $"{puerta.Id} - {puerta.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchTransportista(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Transportista> transportistas = uow.TransportistaRepository.GetByDescripcionOrCodePartial(context.SearchValue);

            foreach (var transportista in transportistas)
            {
                opciones.Add(new SelectOption(transportista.Id.ToString(), $"{transportista.Id} - {transportista.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchVehiculo(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            string predio = form.GetField("predio").Value;

            if (string.IsNullOrEmpty(predio))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Vehiculo> vehiculos = uow.VehiculoRepository.GetByDescripcionOrCodePartial(context.SearchValue, predio);

            foreach (var vehiculo in vehiculos)
            {
                opciones.Add(new SelectOption(vehiculo.Id.ToString(), $"{vehiculo.Id} - {vehiculo.Descripcion}"));
            }

            return opciones;
        }

        #endregion
    }
}
