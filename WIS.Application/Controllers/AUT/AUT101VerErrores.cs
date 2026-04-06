using System.Collections.Generic;
using WIS.Components.Common;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.Automatismo.Logic;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Exceptions;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;
using WIS.Validation;

namespace WIS.Application.Controllers.AUT
{
    public class AUT101VerErrores : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IAutomatismoFactory _automatismoFactory;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public AUT101VerErrores(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IAutomatismoFactory automatismoFactory)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._automatismoFactory = automatismoFactory;

            this.GridKeys = new List<string>
            {
                 "Message"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("Message", SortDirection.Descending)
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }
        
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            GridInMemoryUtils.LoadGrid(_gridService, uow, grid, context, GetListOfApiValidationErrors(uow, context), DefaultSort, GridKeys);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return GridInMemoryUtils.FetchStats(context, uow, GetListOfApiValidationErrors(uow, context));
        }
        
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return GridInMemoryUtils.CreateExcel(_excelService, uow, grid, context, GetListOfApiValidationErrors(uow, context), DefaultSort, _identity.Application);
        }

        #region Metodos Auxiliares

        public virtual int ValidateNumeroAutomatismoEjecucion(string nuAutomatismoEjecucion)
        {
            if (string.IsNullOrEmpty(nuAutomatismoEjecucion) || !int.TryParse(nuAutomatismoEjecucion, out int numeroAutomatismoEjecucion))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            return numeroAutomatismoEjecucion;
        }
        
        public virtual List<ValidationError> GetListOfApiValidationErrors(IUnitOfWork uow, ComponentContext context)
        {
            var nuAutomatismoEjecucion = ValidateNumeroAutomatismoEjecucion(context.GetParameter("NU_AUT_EJECUCION"));
            var logic = new AutomatismoLogic(uow, _automatismoFactory);
            var responseMsg = logic.GetJsonResponseFromAutomatismoEjecucion(nuAutomatismoEjecucion);
            var errors = new List<ValidationError>();

            responseMsg.ForEach(ve =>
            {
                ve.Messages.ForEach(m => errors.Add(new ValidationError(m)));
            });

            return errors;
        }
        
        #endregion
    }
}
