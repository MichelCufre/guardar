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
using WIS.Domain.Produccion.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.PRD
{
	public class PRD111CrearEspacioProduccion : AppController
	{
		protected readonly IUnitOfWorkFactory _uowFactory;
		protected readonly IIdentityService _identity;
		protected readonly IEspacioProduccionLogic _espacioProduccionLogic;
		protected readonly IFormValidationService _formValidationService;

		protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public PRD111CrearEspacioProduccion(IUnitOfWorkFactory uowFactory,
			IIdentityService identity,
			IFormValidationService formValidationService,
			IEspacioProduccionLogic espacioProduccionLogic)
		{
			_uowFactory = uowFactory;
			_identity = identity;
			_formValidationService = formValidationService;
			_espacioProduccionLogic = espacioProduccionLogic;
		}

		public override Form FormInitialize(Form form, FormInitializeContext context)
		{
			using var uow = _uowFactory.GetUnitOfWork();

			InicializarSelect(uow, form);

			return form;
		}

		public override Form FormSubmit(Form form, FormSubmitContext context)
		{
			using var uow = _uowFactory.GetUnitOfWork();

			try
			{
				string descripcion = form.GetField("descripcion").Value;
				string flStockConsumible = "N";
				string flConfMan = form.GetField("flConfMan").IsChecked() ? "S" : "N";
				string predio = form.GetField("predio").Value;

				// Creación de la linea de producción
				var espacio = _espacioProduccionLogic.CrearEspacioProduccion(descripcion, CEspacioProduccion.TipoEspacioaBlackBox, flConfMan, flStockConsumible, predio);

				uow.BeginTransaction();

				uow.EspacioProduccionRepository.AddEspacioProduccion(espacio);

				uow.SaveChanges();
				uow.Commit();

				context.AddSuccessNotification("PRD111_Sec0_Succes_Succes01", new List<string> { espacio.Id });

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

			var defaultTipoIngreso = uow.ParametroRepository.GetParameter("PRD_TP_ING_DEFAULT");
			var selectTipoIngresoHabilidado = uow.ParametroRepository.GetParameter("PRD_TP_ING_HABILITADO");

			if (!string.IsNullOrEmpty(defaultTipoIngreso))
			{
				selectorTipo.Value = defaultTipoIngreso;

				if (!string.IsNullOrEmpty(selectTipoIngresoHabilidado))
				{
					selectorTipo.Disabled = selectTipoIngresoHabilidado != "S";
				}
			}
		}

        #endregion
    }
}
