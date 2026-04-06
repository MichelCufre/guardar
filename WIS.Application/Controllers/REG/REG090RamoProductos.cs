using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Eventos;
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
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.REG
{
    public class REG090RamoProductos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REG090RamoProductos> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG090RamoProductos(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<REG090RamoProductos> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_RAMO_PRODUTO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_RAMO_PRODUTO", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._security = security;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._formValidationService = formValidationService;
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
                "CD_RAMO_PRODUTO",
                "DS_RAMO_PRODUTO",

            });

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    RegistroModificacionRamoProductos registroModificacionRamoProductos = new RegistroModificacionRamoProductos(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {

                        if (row.IsNew)
                        {
                            ProductoRamo reamoProducto = CrearRamoProducto(uow, row, query);
                            registroModificacionRamoProductos.RegistrarRamoProducto(reamoProducto);
                        }
                        else if (row.IsDeleted)
                        {
                            // rows delete
                            short idRamoProducto = short.Parse(row.GetCell("CD_RAMO_PRODUTO").Value);
                            this.DeleteRamoProducto(uow, row, query, idRamoProducto);
                        }
                        else
                        {
                            // rows editadas
                            ProductoRamo reamoProducto = ModificarRamoProducto(uow, row, query);
                            registroModificacionRamoProductos.ModificarRamoProducto(reamoProducto);
                        }

                    }

                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "FAC249GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }
        public virtual void DeleteRamoProducto(IUnitOfWork uow, GridRow row, GridFetchContext query, short codigoRamo)
        {
            if (!uow.ProductoRepository.RamoEnUso(codigoRamo))
            {
                uow.ProductoRamoRepository.DeleteRamo(codigoRamo);
            }
            else
            {
                throw new EntityNotFoundException("REG310_Sec0_Error_Er001_RamoEnUso");
            }
        }
        public virtual ProductoRamo CrearRamoProducto(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            ProductoRamo nuevoProductoRamo = new ProductoRamo();

            nuevoProductoRamo.Id = short.Parse(row.GetCell("CD_RAMO_PRODUTO").Value);
            nuevoProductoRamo.Descripcion = row.GetCell("DS_RAMO_PRODUTO").Value;
            nuevoProductoRamo.Alta = DateTime.Now;
            nuevoProductoRamo.Modificacion = DateTime.Now;

            return nuevoProductoRamo;
        }
        public virtual ProductoRamo ModificarRamoProducto(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            ProductoRamo nuevoProductoRamo = new ProductoRamo();

            nuevoProductoRamo.Id = short.Parse(row.GetCell("CD_RAMO_PRODUTO").Value);
            nuevoProductoRamo.Descripcion = row.GetCell("DS_RAMO_PRODUTO").Value;
            nuevoProductoRamo.Modificacion = DateTime.Now;

            return nuevoProductoRamo;

        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new RegistroRamoDeProductosValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            RamoDeProductosQuery dbQuery = new RamoDeProductosQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> {

                "DS_RAMO_PRODUTO",

            });

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            RamoDeProductosQuery dbQuery = new RamoDeProductosQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            RamoDeProductosQuery dbQuery = new RamoDeProductosQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}
