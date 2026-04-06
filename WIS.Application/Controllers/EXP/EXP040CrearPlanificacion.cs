using System;
using System.Collections.Generic;
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
using WIS.Domain.Tracking;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.EXP
{
    public class EXP040CrearPlanificacion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IParameterService _parameterService;

        public EXP040CrearPlanificacion(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            ISecurityService security,
            IAPITrackingService apiTrackingService,
            IParameterService parameterService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
            this._parameterService = parameterService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            form.GetField("descripcion").Value = string.Empty;
            form.GetField("codigoEmpresa").Value = string.Empty;

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

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return this._formValidationService.Validate(new CreatePlanificacionFormValidationModule(uow, this._identity, this._security), form, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            ExpedicionConfiguracionService expedicionService = new ExpedicionConfiguracionService(uow, this._parameterService, new ParametroMapper());

            int? empresaValue = null;
            if (int.TryParse(form.GetField("codigoEmpresa").Value, out int parsedValue))
                empresaValue = parsedValue;

            uow.CreateTransactionNumber("EXP040 Crear Planificación");

            Camion camion = new Camion
            {
                Descripcion = form.GetField("descripcion").Value,
                Empresa = empresaValue,
                Predio = form.GetField("predio").Value,
                Estado = CamionEstado.AguardandoCarga,
                TipoArmadoEgreso = TipoArmadoEgreso.Planificacion,
                ArmadoEgreso = TipoArmadoEgreso.ArmadoPlanificacion,
                IsCierreParcialHabilitado = expedicionService.IsCierreParcialHabilitado(empresaValue ?? -1),
                RespetaOrdenCarga = expedicionService.RespetaOrdenCargarDefault(empresaValue ?? -1),
                IsRuteoHabilitado = expedicionService.RuteoHabilitadoDefault(empresaValue ?? -1),
                Transportista = expedicionService.TransportistaDefault(empresaValue ?? -1),

                Puerta = null,
                Ruta = null,
                Vehiculo = null,
                FechaCreacion = DateTime.Now,
                Matricula = null,
                IsTrackingHabilitado = true,
                IsCargaAutomaticaHabilitada = false,
                IsCierreAutomaticoHabilitado = false,
                IsControlContenedoresHabilitado = false,
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
                _ => new List<SelectOption>(),
            };
        }

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
    }
}
