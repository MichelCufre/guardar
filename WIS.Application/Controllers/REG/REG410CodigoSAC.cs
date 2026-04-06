using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
using WIS.Domain.Registro;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.REG
{
    public class REG410CodigoSAC : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REG410CodigoSAC> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG410CodigoSAC(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<REG410CodigoSAC> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_NAM",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_NAM", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            _filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = true;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;

            grid.SetInsertableColumns(new List<string> {
                "CD_NAM",
                "DS_NAM"
            });

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CodigoSACQuery dbQuery = new CodigoSACQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> {
                "CD_NAM",
                "DS_NAM"
            });

            return grid;
        }
        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    RegistroModificacionCodigoSAC registroModificacionCSAC = new RegistroModificacionCodigoSAC(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {

                        if (row.IsNew)
                        {
                            CodigoNomenclaturaComunMercosur codigoSAC = this.CrearCodigoSAC(uow, row, query);
                            registroModificacionCSAC.RegistrarCodigoSAC(codigoSAC);
                        }
                        else if (row.IsDeleted)
                        {
                            // rows delete
                            string idCodigoSAC = row.GetCell("CD_NAM").Value;
                            this.DeleteCodigoSAC(uow, row, query, idCodigoSAC);
                        }
                        else
                        {
                            // rows editadas
                            //En caso de ID diferente borro el anterior y creo uno nuevo
                            //De lo contrario actualizo el valor existente
                            CodigoNomenclaturaComunMercosur codigoSAC = new CodigoNomenclaturaComunMercosur();
                            string idCodigoSAC = row.GetCell("CD_NAM").Old;
                            if (idCodigoSAC != row.GetCell("CD_NAM").Value)
                            {
                                this.DeleteCodigoSAC(uow, row, query, idCodigoSAC);

                                codigoSAC = this.CrearCodigoSAC(uow, row, query);
                                registroModificacionCSAC.RegistrarCodigoSAC(codigoSAC);
                            }
                            else
                            { 
                                codigoSAC = this.UpdateCodigoSAC(uow, row, query);
                                registroModificacionCSAC.ModificarCodigoSAC(codigoSAC);
                            }
                        }
                    }
                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REG410GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoCodigoSACGridValidationModule(uow), grid, row, context);
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CodigoSACQuery dbQuery = new CodigoSACQuery();
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CodigoSACQuery dbQuery = new CodigoSACQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual CodigoNomenclaturaComunMercosur CrearCodigoSAC(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            CodigoNomenclaturaComunMercosur CodigoSAC = new CodigoNomenclaturaComunMercosur();

            CodigoSAC.Id = row.GetCell("CD_NAM").Value;
            CodigoSAC.Descripcion = row.GetCell("DS_NAM").Value;

            return CodigoSAC;
        }
        public virtual CodigoNomenclaturaComunMercosur UpdateCodigoSAC(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string idCodigoSAC = row.GetCell("CD_NAM").Value;

            CodigoNomenclaturaComunMercosur CodigoSAC = new CodigoNomenclaturaComunMercosur();
            CodigoSAC = uow.NcmRepository.GetNCM(idCodigoSAC);

            CodigoSAC.Descripcion = row.GetCell("DS_NAM").Value;

            return CodigoSAC;
        }
        public virtual void DeleteCodigoSAC(IUnitOfWork uow, GridRow row, GridFetchContext query, string idCodigoSAC)
        {
            if (uow.NcmRepository.ExisteNCM(idCodigoSAC))
            {
                uow.NcmRepository.DeleteNCM(idCodigoSAC);
            }
            else
            {
                throw new EntityNotFoundException("REG310_Sec0_Error_Er001_CodigoSACNoExisteEliminar");
            }
        }
    }
}
