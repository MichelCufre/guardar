using NLog;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.Security;
using WIS.Session;

namespace WIS.Application.Controllers.DOC
{
    public class DOC021 : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; }

        public DOC021(ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
            this._filterInterpreter = filterInterpreter;

            this.GridKeys = new List<string>
            {
               "TP_DOCUMENTO", "NU_DOCUMENTO", "CD_EMPRESA"
            };
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            decimal? disponible = _session.GetValue<decimal?>("DOC021_QT_DISPONIBLE");
            decimal? reservada = _session.GetValue<decimal?>("DOC021_QT_RESERVADA");
            decimal? mercaderia = _session.GetValue<decimal?>("DOC021_QT_MERCADERIA");

            decimal? ingresado = _session.GetValue<decimal?>("DOC021_QT_INGRESADA");
            decimal? desafectada = _session.GetValue<decimal?>("DOC021_QT_DESAFECTADA");
            decimal? existencia = _session.GetValue<decimal?>("DOC021_QT_EXISTENCIA");

            form.GetField("QT_DISPONIBLE").Value = (disponible ?? 0).ToString("#.##");
            form.GetField("QT_RESERVADA").Value = (reservada ?? 0).ToString("#.##");
            form.GetField("QT_MERCADERIA").Value = (mercaderia ?? 0).ToString("#.##");

            form.GetField("QT_INGRESADA").Value = (ingresado ?? 0).ToString("#.##");
            form.GetField("QT_DESAFECTADA").Value = (desafectada ?? 0).ToString("#.##");
            form.GetField("QT_EXISTENCIA").Value = (existencia ?? 0).ToString("#.##");


            return form;
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            return grid;
        }
    }
}
