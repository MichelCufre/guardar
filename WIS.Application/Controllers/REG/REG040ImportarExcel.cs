using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.General;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.REG
{
    public class REG040ImportarExcel : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG040ImportarExcel> _logger;
        protected readonly IFormValidationService _formValidationService;

        public REG040ImportarExcel(IIdentityService identity, IUnitOfWorkFactory uowFactory, ILogger<REG040ImportarExcel> logger, IFormValidationService formValidationService)
        {
            _identity = identity;
            _uowFactory = uowFactory;
            _logger = logger;
            _formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            this.InicializarSelects(form, query, uow);

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            return form;
        }

        #region AUX METHODS
        public virtual void InicializarSelects(Form form, FormInitializeContext query, IUnitOfWork uow)
        {
            FormField selectPredio = form.GetField("predio");

            selectPredio.Options = [];

            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            List<Predio> predios = dbQuery.GetPrediosUsuario(_identity.UserId);
            foreach (var predio in predios)
            {
                selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}")); ;
            }

            if (_identity.Predio != GeneralDb.PredioSinDefinir)
            {
                selectPredio.Value = _identity.Predio;
                selectPredio.Disabled = true;
            }
            else if (predios.Count == 1)
            {
                selectPredio.Value = predios.FirstOrDefault().Numero;
                selectPredio.Disabled = true;
            }
            query.AddParameter("REG040_PREDIO_UNICO", selectPredio.Value);
        }

        #endregion
    }
}
