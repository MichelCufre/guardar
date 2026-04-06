using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Automatizacion;
using WIS.Domain.Impresiones;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.AUT
{
    public class AUT100Puestos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public AUT100Puestos(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IGridValidationService gridValidationService)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._gridValidationService = gridValidationService;

            this.GridKeys = new List<string>
            {
                "NU_AUTOMATISMO_PUESTO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_AUTOMATISMO_PUESTO", SortDirection.Descending)
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

            if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            context.IsEditingEnabled = true;
            context.IsAddEnabled = true;
            context.IsRemoveEnabled = false;
            context.IsCommitEnabled = false;

            grid.SetColumnDefaultValues(new Dictionary<string, string>
            {
                ["NU_AUTOMATISMO"] = nuAutomatismo,
            });
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_IMPRESORA", this.OptionSelectImpresoras(numeroAutomatismo)));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

            if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var query = new PuestosAutomatismoQuery(numeroAutomatismo);

            uow.HandleQuery(query);

            grid.Rows = _gridService.GetRows(query, grid.Columns, context, this.DefaultSort, this.GridKeys);

            grid.SetInsertableColumns(new List<string>
            {
                "ID_PUESTO","CD_IMPRESORA"
            });

            grid.SetEditableCells(new List<string>
            {
                "ID_PUESTO","CD_IMPRESORA"
            });

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

            if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
            {
                context.AddErrorNotification("AUT100Posiciones_Sec0_Error_FormatoNumeroAutomatismoIncorrecto");

                return null;
            }

            var dbQuery = new PuestosAutomatismoQuery(numeroAutomatismo);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
        
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

            if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
            {
                context.AddErrorNotification("AUT100Posiciones_Sec0_Error_FormatoNumeroAutomatismoIncorrecto");

                return null;
            }

            var query = new PuestosAutomatismoQuery(numeroAutomatismo);

            uow.HandleQuery(query);

            query.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats()
            {
                Count = query.GetCount()
            };
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber($"GridCommit - {_identity.Application}");

            try
            {
                uow.BeginTransaction();

                grid.Rows.ForEach(row =>
                {
                    if (row.IsNew) CreatePuesto(uow, row);

                    else if (row.IsModified) UpdatePuesto(uow, row);

                });

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("AUT100Puestos_Sec0_Success_OperacionExitosa");
            }
            catch (Exception e)
            {
                uow.Rollback();
                context.AddErrorNotification(e.Message);
            }

            return grid;
        }
        
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new AutomatismoPuestosGridValidationModule(uow, this._identity), grid, row, context);
        }

        #region Metodos Auxiliares

        public virtual void CreatePuesto(IUnitOfWork uow, GridRow row)
        {
            var puesto = new AutomatismoPuesto
            {
                IdAutomatismo = int.Parse(row.GetCell("NU_AUTOMATISMO").Value),
                Transaccion = uow.GetTransactionNumber(),
                Puesto = row.GetCell("ID_PUESTO").Value,
                Impresora = row.GetCell("CD_IMPRESORA").Value
            };

            uow.AutomatismoPuestoRepository.Add(puesto);
            uow.SaveChanges();
        }
        
        public virtual void UpdatePuesto(IUnitOfWork uow, GridRow row)
        {
            var id = int.Parse(row.GetCell("NU_AUTOMATISMO_PUESTO").Value);

            var puesto = uow.AutomatismoPuestoRepository.GetAutomatismoPuestoById(id);

            puesto.Puesto = row.GetCell("ID_PUESTO").Value;
            puesto.Impresora = row.GetCell("CD_IMPRESORA").Value;
            puesto.Transaccion = uow.GetTransactionNumber();

            uow.AutomatismoPuestoRepository.Update(puesto);

            uow.SaveChanges();
        }
        
        public virtual List<SelectOption> OptionSelectImpresoras(int numeroAutomatismo)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var automatismo = uow.AutomatismoRepository.GetAutomatismoById(numeroAutomatismo);
            List<Impresora> impresoras = uow.ImpresoraRepository.GetListaImpresorasPredio(automatismo.Predio);

            foreach (Impresora impresora in impresoras)
            {
                opciones.Add(new SelectOption(impresora.Id, impresora.Descripcion));
            }

            return opciones;
        }

        #endregion
    }
}
