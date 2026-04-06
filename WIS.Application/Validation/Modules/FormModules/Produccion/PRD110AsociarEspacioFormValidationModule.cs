using WIS.Application.Validation.Produccion.Rules;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Produccion
{
	public class PRD110AsociarEspacioFormValidationModule : FormValidationModule
	{
		protected readonly IUnitOfWork _uow;
		protected readonly IIdentityService _securityService;

		public PRD110AsociarEspacioFormValidationModule(IUnitOfWork uow, IIdentityService securityService)
		{
			_uow = uow;
			_securityService = securityService;

			Schema = new FormValidationSchema
			{
				["espacio"] = ValidateEspacioProduccion,
			};
		}

        public virtual FormValidationGroup ValidateEspacioProduccion(FormField field, Form form, List<ComponentParameter> parameters)
		{
			return new FormValidationGroup
			{
				BreakValidationChain = true,
				Rules = new List<IValidationRule>
				{
					new NonNullValidationRule(field.Value),
					new EspacioProduccionExisteValidationRule(_uow, field.Value)
				}
			};
		}
	}
}
