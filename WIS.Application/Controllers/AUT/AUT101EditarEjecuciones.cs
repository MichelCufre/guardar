using System;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.Automatismo.Logic;
using WIS.Domain.DataModel;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.AUT
{
    public class AUT101EditarEjecuciones : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IAutomatismoAutoStoreClientService _automatismoAutoStoreClientService;
        protected readonly IAutomatismoFactory _automatismoFactory;
        protected IAutomatismoRequestStategy _automatismoRequestStategy;

        public AUT101EditarEjecuciones(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IAutomatismoAutoStoreClientService wmsAutomatismoAutoStoreClientService,
            IGridValidationService gridValidationService,
            IAutomatismoFactory automatismoFactory)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._automatismoAutoStoreClientService = wmsAutomatismoAutoStoreClientService;
            this._gridValidationService = gridValidationService;
            this._automatismoFactory = automatismoFactory;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsAddEnabled = false;
            context.IsRemoveEnabled = false;
            context.IsCommitEnabled = false;

            return this.GridFetchRows(grid, context.FetchContext);
        }
        
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            LoadStrategy(uow, context);

            _automatismoRequestStategy.GenerateRowsAndLoadGrid(grid, context);

            return grid;
        }
        
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            LoadStrategy(uow, context);

            return _automatismoRequestStategy.CreateExcel(grid, context);
        }
        
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            LoadStrategy(uow, context);

            return _automatismoRequestStategy.FetchStats(context);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            LoadStrategy(uow, context);

            try
            {
                if (grid.Rows != null && grid.Rows.Count > 0)
                {
                    grid.Rows.ForEach(row =>
                    {
                        if (row.IsModified) _automatismoRequestStategy.UpdateRequest(row, context);
                    });

                    uow.SaveChanges();

                    context.AddSuccessNotification("AUT101EditarEjecuciones_Sec0_Success_AutomatismoEjecRequestModificado");
                }
            }
            catch (Exception e)
            {
                context.AddErrorNotification(e.Message);
            }

            return grid;
        }
        
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new AutomatismoEditarEjecucionGridValidationModule(uow, this._identity), grid, row, context);
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            try
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                var logic = new AutomatismoLogic(uow, _automatismoFactory);
                var ejec = context.GetParameter("NU_AUT_EJECUCION").ToNumber<int>();

                logic.Reprocesar(ejec, _automatismoAutoStoreClientService);

                context.AddInfoNotification("AUT100Ejecuciones_form1_Success_AutomatismoReprocesado");
            }
            catch (Exception e)
            {
                context.AddErrorNotification(e.Message);
            }

            return form;
        }

        #region Metodos Auxiliares
        
        public virtual void LoadStrategy(IUnitOfWork uow, ComponentContext context)
        {
            var nuAutomatismoEjecucion = ValidateNumeroAutomatismoEjecucion(context.GetParameter("NU_AUT_EJECUCION"));
            var ejecucion = uow.AutomatismoEjecucionRepository.GetAutomatismoEjecucionWithData(nuAutomatismoEjecucion);
            var logic = new AutomatismoLogic(uow, _automatismoFactory);

            _automatismoRequestStategy = logic.CreateAutomatismoRequestStategy(ejecucion, _gridService, _identity, _excelService, uow);
        }
        
        public virtual int ValidateNumeroAutomatismoEjecucion(string nuAutomatismoEjecucion)
        {
            if (string.IsNullOrEmpty(nuAutomatismoEjecucion) || !int.TryParse(nuAutomatismoEjecucion, out int numeroAutomatismoEjecucion))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            return numeroAutomatismoEjecucion;
        }
        
        #endregion

    }
}
