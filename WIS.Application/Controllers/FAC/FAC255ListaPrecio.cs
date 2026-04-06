using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Facturacion;
using WIS.Domain.Facturacion;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.FAC
{
    public class FAC255ListaPrecio : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC255ListaPrecio> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC255ListaPrecio(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC255ListaPrecio> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_LISTA_PRECIO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_LISTA_PRECIO", SortDirection.Ascending),
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
            query.IsRemoveEnabled = false;

            grid.SetInsertableColumns(new List<string> {
                "CD_LISTA_PRECIO",
                "DS_LISTA_PRECIO",
                "CD_MONEDA"
            });

            //Botones
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnCotizaciones", "FAC255_grid1_btn_DetalleLista", "fas fa-bars"),
            }));

            //Select moneda
            using var uow = this._uowFactory.GetUnitOfWork();
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_MONEDA", this.SelectMoneda(uow)));

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ListaPrecioQuery dbQuery = new ListaPrecioQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> {
                "DS_LISTA_PRECIO",
                "CD_MONEDA"
            });

            return grid;
        }
        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            switch (context.ButtonId)
            {
                case "btnCotizaciones":
                    context.Redirect("/facturacion/FAC256", true, new List<ComponentParameter>() {
                      new ComponentParameter(){ Id = "idListaPrecio", Value = context.Row.GetCell("CD_LISTA_PRECIO").Value},
                      new ComponentParameter(){ Id = "descripcionListaPrecio", Value = context.Row.GetCell("DS_LISTA_PRECIO").Value},
                    });
                    break;
            }

            return context;
        }
        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    RegistroModificacionListaPrecio registroModificacionLP = new RegistroModificacionListaPrecio(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    uow.CreateTransactionNumber(this._identity.Application);

                    foreach (var row in grid.Rows)
                    {
                        if (row.IsNew)
                        {
                            ListaPrecio listaPrecio = this.CrearListaPrecio(uow, row, query);
                            registroModificacionLP.RegistrarListaPrecio(listaPrecio);
                        }
                        else
                        {
                            // rows editadas
                            ListaPrecio listaPrecio = this.UpdateListaPrecio(uow, row, query);
                            registroModificacionLP.ModificarListaPrecio(listaPrecio);
                        }
                    }
                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "FAC255GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoListaPrecioGridValidationModule(uow), grid, row, context);
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ListaPrecioQuery dbQuery = new ListaPrecioQuery();
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ListaPrecioQuery dbQuery = new ListaPrecioQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual List<SelectOption> SelectMoneda(IUnitOfWork uow)
        {
            return uow.MonedaRepository.GetMonedas()
                .Select(w => new SelectOption(w.Codigo, w.Codigo))
                .ToList();
        }

        public virtual ListaPrecio CrearListaPrecio(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            ListaPrecio nuevaListaPrecio = new ListaPrecio();

            nuevaListaPrecio.Id = int.Parse(row.GetCell("CD_LISTA_PRECIO").Value);
            nuevaListaPrecio.Descripcion = row.GetCell("DS_LISTA_PRECIO").Value;
            nuevaListaPrecio.IdMoneda = row.GetCell("CD_MONEDA").Value;
            nuevaListaPrecio.FechaIngresado = DateTime.Now;
            nuevaListaPrecio.NumeroTransaccion = uow.GetTransactionNumber();

            return nuevaListaPrecio;
        }

        public virtual ListaPrecio UpdateListaPrecio(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            int idListaPrecio = int.Parse(row.GetCell("CD_LISTA_PRECIO").Value);

            ListaPrecio listaPrecio = new ListaPrecio();
            listaPrecio = uow.ListaPrecioRepository.GetListaPrecio(idListaPrecio);

            listaPrecio.Descripcion = row.GetCell("DS_LISTA_PRECIO").Value;
            listaPrecio.IdMoneda = row.GetCell("CD_MONEDA").Value;
            listaPrecio.FechaActualizacion = DateTime.Now;
            listaPrecio.NumeroTransaccion = uow.GetTransactionNumber();

            return listaPrecio;
        }
    }
}
