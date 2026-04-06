using Microsoft.Extensions.Logging;
using System.Linq;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent.Build;
using WIS.GridComponent.Excel;
using WIS.Security;
using WIS.Session;

namespace WIS.Application.Controllers.EVT
{
    public class EVT040DestinatariosInstancia : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<EVT040DestinatariosInstancia> _logger;

        public EVT040DestinatariosInstancia(
            ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ILogger<EVT040DestinatariosInstancia> logger)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._gridValidationService = gridValidationService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._formValidationService = formValidationService;
            this._logger = logger;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var numeroInstancia = int.Parse(context.GetParameter("instancia"));
            var instancia = uow.EventoRepository.GetInstancia(numeroInstancia);

            if (instancia != null)
            {
                form.GetField("evento").Value = instancia.NumeroEvento.ToString();
                form.GetField("instancia").Value = numeroInstancia.ToString();
                form.GetField("descripcion").Value = instancia.Descripcion;

                var parametrosInstancia = uow.EventoRepository.GetParametrosInstancia(instancia.Id);

                form.GetField("empresa").Value = parametrosInstancia.Find(x => x.Codigo == EventoParametroDb.CD_EMPRESA)?.Valor;
                form.GetField("tipoAgente").Value = parametrosInstancia.Find(x => x.Codigo == EventoParametroDb.TP_AGENTE)?.Valor;
                form.GetField("codigoAgente").Value = parametrosInstancia.Find(x => x.Codigo == EventoParametroDb.CD_AGENTE)?.Valor;

                context.Parameters.Add(new ComponentParameter { Id = "manejaEmpresa", Value = parametrosInstancia.Any(x => x.Codigo == EventoParametroDb.CD_EMPRESA).ToString() });
                context.Parameters.Add(new ComponentParameter { Id = "manejaTipoAgente", Value = parametrosInstancia.Any(x => x.Codigo == EventoParametroDb.TP_AGENTE).ToString() });
                context.Parameters.Add(new ComponentParameter { Id = "manejaCodigoAgente", Value = parametrosInstancia.Any(x => x.Codigo == EventoParametroDb.CD_AGENTE).ToString() });
            }

            return form;
        }
    }
}

