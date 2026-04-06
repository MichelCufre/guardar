using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.EXP
{
    public class EXP040ModificarPlanificacion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IParameterService _parameterService;

        public EXP040ModificarPlanificacion(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            ISecurityService security,
            IParameterService parameterService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
            this._parameterService = parameterService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            if (!int.TryParse(query.GetParameter("camion"), out int camionId))
                throw new ValidationFailedException("EXP040_Sec0_Error_PlanificacionNoValida");

            using var uow = this._uowFactory.GetUnitOfWork();

            var camion = uow.CamionRepository.GetCamion(camionId);
            this.InicializarSelects(uow, form, camion);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("EXP040 Modificar Planificación");

            int camionId = int.Parse(context.GetParameter("camion"));
            Camion camion = uow.CamionRepository.GetCamion(camionId);

            int? empresaValue = null;
            if (int.TryParse(form.GetField("codigoEmpresa").Value, out int parsedValue))
                empresaValue = parsedValue;

            camion.Empresa = empresaValue;
            camion.Predio = form.GetField("predio").Value;
            camion.Descripcion = form.GetField("descripcion").Value;
            camion.FechaModificacion = DateTime.Now;
            camion.NumeroTransaccion = uow.GetTransactionNumber();

            uow.CamionRepository.UpdateCamion(camion);
            uow.SaveChanges();

            context.AddSuccessNotification("General_Sec0_Error_Er022_EditSuccess");

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new CreatePlanificacionFormValidationModule(uow, this._identity, this._security), form, context);
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

        public virtual void InicializarSelects(IUnitOfWork uow, Form form, Camion camion)
        {
            FormField fieldEmpresa = form.GetField("codigoEmpresa");

            string empresa = camion.Empresa.ToString();

            form.GetField("descripcion").Value = camion.Descripcion;
            form.GetField("predio").Value = camion.Predio;

            if (uow.CamionRepository.AnyClienteAsociadoCamion(camion.Id))
            {
                form.GetField("predio").ReadOnly = true;
                form.GetField("codigoEmpresa").ReadOnly = true;
            }

            fieldEmpresa.Options = SearchEmpresa(form, new FormSelectSearchContext()
            {
                SearchValue = empresa
            });

            fieldEmpresa.Value = empresa;

            // Predios
            FormField selectPredio = form.GetField("predio");
            selectPredio.Options = new List<SelectOption>();
            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            var predios = dbQuery.GetPrediosUsuario(_identity.UserId).OrderBy(x => x.Numero);
            foreach (var pred in predios)
            {
                selectPredio.Options.Add(new SelectOption(pred.Numero, $"{pred.Numero} - {pred.Descripcion}"));
            }
        }
    }
}
