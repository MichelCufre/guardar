
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.REC
{
    public class REC170ValidarFactura : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridExcelService _excelService;
        protected readonly IParameterService _parameterService;
        protected readonly IFactoryService _factoryService;

        protected List<string> GridKeys { get; }

        public REC170ValidarFactura(
            IIdentityService identity,
            IFactoryService factoryService,
            ITrafficOfficerService concurrencyControl,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            ISecurityService security,
            IFilterInterpreter filterInterpreter,
            IGridExcelService excelService,
            IParameterService parameterService)
        {

            this._identity = identity;
            this._factoryService = factoryService;
            this._concurrencyControl = concurrencyControl;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._security = security;
            this._filterInterpreter = filterInterpreter;
            this._excelService = excelService;
            this._parameterService = parameterService;

            this.GridKeys = new List<string>
            {
                "NU_AGENDA", "CD_PRODUTO"
            };

        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ValidarFacturaQuery dbQuery;

            if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "keyAgenda")?.Value, out int nuAgenda))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            dbQuery = new ValidarFacturaQuery(nuAgenda);

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("NU_AGENDA", SortDirection.Descending);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridKeys);
            foreach (var row in grid.Rows)
            {
                if (row.GetCell("DIFERENCIAS").Value != "COINCIDE")
                {
                    row.CssClass = "red";
                }
            }
            return grid;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "keyAgenda")?.Value, out int nuAgenda))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var agenda = uow.AgendaRepository.GetAgenda(nuAgenda);
            var tipoRec = uow.RecepcionTipoRepository.GetRecepcionTipoByInterno(agenda.IdEmpresa, agenda.TipoRecepcionInterno);

            if (agenda == null)
                throw new ValidationFailedException("REC170_Frm1_Error_AgendaNoExiste", new string[] { nuAgenda.ToString() });
            //var facturaValidada = uow.AgendaCustomRepository.IsAgendaFacturaValida(nuAgenda);

            if (!tipoRec.IngresaFactura || uow.AgendaRepository.IsAgendaFacturaValida(nuAgenda) || !agenda.EnEstadoAbierta())
            {
                form.GetButton("btnValidar").Disabled = true;
                form.GetButton("btnValidar").Hidden = true;
            }
            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext query)
        {
            if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "keyAgenda")?.Value, out int nuAgenda))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                uow.FacturaRepository.ValidarFacturas(nuAgenda);

                query.AddSuccessNotification("REC170_Sec0_Succes_FacturasValidadas");
            }
            catch (Exception ex)
            {
                string message = "";

                if (ex.Message.Contains("ORA-2000"))
                {
                    message = ex.Message.Split(".").First();
                }
                else
                    message = ex.Message;

                throw new ValidationFailedException(message);
            }

            return form;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ValidarFacturaQuery dbQuery;
            if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "keyAgenda")?.Value, out int nuAgenda))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            dbQuery = new ValidarFacturaQuery(nuAgenda);

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("NU_AGENDA", SortDirection.Descending);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ValidarFacturaQuery dbQuery;
            if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "keyAgenda")?.Value, out int nuAgenda))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            dbQuery = new ValidarFacturaQuery(nuAgenda);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}
