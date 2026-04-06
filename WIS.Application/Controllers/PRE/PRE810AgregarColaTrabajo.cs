using crypto;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office.PowerPoint.Y2021.M06.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Application.Validation.Modules.FormModules.Preparacion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.Expedicion;
using WIS.Domain.Picking;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Extension;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE810AgregarColaTrabajo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IParameterService _paramService;
        protected readonly IFormValidationService _formValidationService;
        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE810AgregarColaTrabajo(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IParameterService paramService)
        {
            this._formValidationService = formValidationService;
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._paramService = paramService;
        }

        #region FORM
        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                this.InicializarSelectPredio(uow, form);
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {

                string strNuColaTrabajo = form.GetField("nuColaTrabajo").Value;
                string nuPredio = form.GetField("nuPredio").Value;
                string descripcion = form.GetField("descripcion").Value;
                string flOrdenCalendario = form.GetField("flOrdenCalendario").Value;


                if (!int.TryParse(strNuColaTrabajo, out int nuColaTrabajo))
                {
                    throw new ValidationFailedException("PRE810_Sec0_Error_TipoDeDatos");
                }

                ColaDeTrabajo obj = new ColaDeTrabajo()
                {
                    Numero = nuColaTrabajo,
                    Predio = nuPredio,
                    Descripcion = descripcion,
                    flOrdenCalendario = flOrdenCalendario == "true" ? "S" : "N"
                };

                uow.ColaDeTrabajoRepository.AddColaDeTrabajo(obj);

                uow.SaveChanges();
                context.AddSuccessNotification("PRE810_Sec0_msg_ColaTrabajoAgregada");

                uow.SaveChanges();

                uow.Commit();

                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
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

            return this._formValidationService.Validate(new ColaDeTrabajoFormValidationModule(uow), form, context);
        }
        
        #endregion

        #region MetodosAxiliares
        public virtual void InicializarSelectColaDeTrabajo(IUnitOfWork uow, Form form)
        {
            FormField fieldTipoMotivo = form.GetField("colaDeTrabajo");
            fieldTipoMotivo.Options = new List<SelectOption>();

            List<ColaDeTrabajo> col = new List<ColaDeTrabajo>();

            if (_identity.Predio == "S/D")
                col = uow.ColaDeTrabajoRepository.GetColasDeTrabajo();
            else
                col = uow.ColaDeTrabajoRepository.GetColasDeTrabajo(_identity.Predio);

            foreach (var colaDeTrabajo in col)
            {
                fieldTipoMotivo.Options.Add(new SelectOption(colaDeTrabajo.Numero.ToString(), $"Nº {colaDeTrabajo.Numero} - Predio {colaDeTrabajo.Predio} - {colaDeTrabajo.Descripcion}"));
            }

            if (col.Count == 1)
                fieldTipoMotivo.Value = col.First().Numero.ToString();
        }
        public virtual void InicializarSelectPredio(IUnitOfWork uow, Form form)
        {
            FormField fieldTipoMotivo = form.GetField("nuPredio");
            fieldTipoMotivo.Options = new List<SelectOption>();

            List<string> col = uow.PredioRepository.GetPredioUser(_identity.UserId);

            foreach (var predio in col)
            {
                fieldTipoMotivo.Options.Add(new SelectOption(predio.ToString(), $"Predio {predio}"));
            }
        }
        #endregion
    }
}
