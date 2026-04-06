using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Preparacion
{
    public class UpdatePonderacionPedidoValidationModule : FormValidationModule
    {
        protected readonly int _userId;
        protected readonly string _userPredio;
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _formatProvider;

        public UpdatePonderacionPedidoValidationModule(IUnitOfWork uow, int userId, string userPredio, IFormatProvider culture)
        {
            this._uow = uow;
            this._formatProvider = culture;
            this._userId = userId;
            this._userPredio = userPredio;

            this.Schema = new FormValidationSchema
            {
                ["qtPonderacion"] = this.ValidateQtPonderacion,
            };
        }

        public virtual FormValidationGroup ValidateQtPonderacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new PositiveIntValidationRule(field.Value),
                }
            };
        }
    }
}
