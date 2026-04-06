using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Produccion;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.General;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Models;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.PRD
{
    public class PRD111ModificarEspacioProduccion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;

        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PRD111ModificarEspacioProduccion(IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IFormValidationService formValidationService)
        {
            _uowFactory = uowFactory;
            _identity = identity;
            _formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            string codigoEspacio = context.GetParameter("codigo");
            EspacioProduccion espacio = uow.EspacioProduccionRepository.GetEspacioProduccion(codigoEspacio);

            form.GetField("descripcion").Value = espacio.Descripcion;
            form.GetField("tipoEspacio").Value = espacio.Tipo;
            form.GetField("predio").Value = espacio.Predio;
            form.GetField("predio").Disabled = true;
            form.GetField("flConfMan").Value = espacio.FlConfirmacionManual;

            InicializarSelect(uow, form);
            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            try
            {
				string codigoEspacio = context.GetParameter("codigo");
				
                var ingresoActivo = uow.EspacioProduccionRepository.AnyIngresoActivoEspacio(codigoEspacio);

                if(ingresoActivo)
					throw new ValidationFailedException("PRD111_Error_msg_IngresoActivoEspacio");

				EspacioProduccion espacio = uow.EspacioProduccionRepository.GetEspacioProduccion(codigoEspacio);

                espacio.Descripcion = form.GetField("descripcion").Value;
                espacio.FlConfirmacionManual = form.GetField("flConfMan").IsChecked() ? "S" : "N";
                espacio.Predio = form.GetField("predio").Value;

				uow.EspacioProduccionRepository.UpdateEspacio(espacio);
                uow.SaveChanges();
                context.AddSuccessNotification("PRD111_Sec0_Succes_Succes02", new List<string> { espacio.Id });

            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw;
            }


            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new PRD111FormValidationModule(uow, this._identity), form, context);
        }

        #region Metodos Auxiliares
        public virtual void InicializarSelect(IUnitOfWork uow, Form form)
        {
            FormField selectPredio = form.GetField("predio");
            FormField selectorTipo = form.GetField("tipoEspacio");
  
            selectPredio.Options = new List<SelectOption>();
            selectorTipo.Options = new List<SelectOption>();

            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            List<Predio> predios = dbQuery.GetPrediosUsuario(_identity.UserId);
            foreach (var predio in predios)
            {
                selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}")); ;
            }

            if (!_identity.Predio.Equals(GeneralDb.PredioSinDefinir))
            {
                selectPredio.Value = _identity.Predio;
                selectPredio.Disabled = true;
            }
            else if (predios.Count == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;

            List<DominioDetalle> dominios = uow.DominioRepository.GetDominios(CEspacioProduccion.DominioTipoEspacio);
            foreach (var dominio in dominios)
            {
                selectorTipo.Options.Add(new SelectOption(dominio.Id, dominio.Descripcion));
            }

            selectorTipo.Disabled = true;
        }
        #endregion
    }
}
