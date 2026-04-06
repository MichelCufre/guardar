using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
	public class PRE100FormValidationModule : FormValidationModule
	{
		protected readonly IUnitOfWork _uow;
		protected readonly int _usuario;
		protected readonly string _predio;

		public PRE100FormValidationModule(IUnitOfWork uow, int usuarioLogueado, string predioLogueado)
		{
			this._uow = uow;
			this._usuario = usuarioLogueado;
			this._predio = predioLogueado;

			this.Schema = new FormValidationSchema
			{
				["ND_ACTIVIDAD"] = this.ValidatePedidosActivos
			};
		}

		public virtual FormValidationGroup ValidatePedidosActivos(FormField field, Form form, List<ComponentParameter> parameters)
		{
			return new FormValidationGroup
			{
				BreakValidationChain = true,
				Rules = new List<IValidationRule>
				{
					new StringToBooleanValidationRule(field.Value)
				}
			};
		}
		
	}
}
