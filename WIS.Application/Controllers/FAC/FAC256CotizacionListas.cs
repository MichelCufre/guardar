using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Facturacion;
using WIS.Domain.Facturacion;
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

namespace WIS.Application.Controllers.FAC
{
    public class FAC256CotizacionListas : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC256CotizacionListas> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC256CotizacionListas(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC256CotizacionListas> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_FACTURACION",
                "NU_COMPONENTE",
                "CD_LISTA_PRECIO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_FACTURACION", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = false;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = false;

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CotizacionListasQuery dbQuery;

            string idListaPrecio = query.Parameters.FirstOrDefault(s => s.Id == "idListaPrecio")?.Value;
            string cdFacturacion = query.Parameters.FirstOrDefault(s => s.Id == "cdFacturacion")?.Value;
            string nuComponente = query.Parameters.FirstOrDefault(s => s.Id == "nuComponente")?.Value;

            if (!string.IsNullOrEmpty(idListaPrecio) && !string.IsNullOrEmpty(nuComponente))
                AddParametersComponente(nuComponente, query);

            if (string.IsNullOrEmpty(idListaPrecio))
                idListaPrecio = "-1";

            dbQuery = new CotizacionListasQuery(int.Parse(idListaPrecio), nuComponente, cdFacturacion);

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> {
                "QT_IMPORTE",
                "QT_IMPORTE_MINIMO"
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
                    RegistroModificacionCotizacionListas registroModificacionCL = new RegistroModificacionCotizacionListas(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {
                        string codigoFacturacion = row.GetCell("CD_FACTURACION").Value;
                        string numeroComponente = row.GetCell("NU_COMPONENTE").Value;
                        string codigoListaPrecio = row.GetCell("CD_LISTA_PRECIO").Value;

                        if (!uow.CotizacionListasRepository.AnyCotizacionListas(codigoFacturacion, numeroComponente, int.Parse(codigoListaPrecio)))
                        {
                            CotizacionListas cotizacionListas = this.CrearCotizacionListas(uow, row, query);
                            registroModificacionCL.RegistrarCotizacionListas(cotizacionListas);
                        }
                        else
                        {
                            // rows editadas
                            CotizacionListas cotizacionListas = this.UpdateCotizacionListas(uow, row, query);
                            registroModificacionCL.ModificarCotizacionListas(cotizacionListas);
                        }
                    }
                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "FAC256GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            return this._gridValidationService.Validate(new MantenimientoCotizacionListasGridValidationModule(this._identity.GetFormatProvider()), grid, row, context);
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CotizacionListasQuery dbQuery;

            string idListaPrecio = query.Parameters.FirstOrDefault(s => s.Id == "idListaPrecio")?.Value;
            string cdFacturacion = query.Parameters.FirstOrDefault(s => s.Id == "cdFacturacion")?.Value;
            string nuComponente = query.Parameters.FirstOrDefault(s => s.Id == "nuComponente")?.Value;

            if (string.IsNullOrEmpty(idListaPrecio))
                idListaPrecio = "-1";

            dbQuery = new CotizacionListasQuery(int.Parse(idListaPrecio), nuComponente, cdFacturacion);

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            CotizacionListasQuery dbQuery;

            string idListaPrecio = query.Parameters.FirstOrDefault(s => s.Id == "idListaPrecio")?.Value;
            string cdFacturacion = query.Parameters.FirstOrDefault(s => s.Id == "cdFacturacion")?.Value;
            string nuComponente = query.Parameters.FirstOrDefault(s => s.Id == "nuComponente")?.Value;

            if (string.IsNullOrEmpty(idListaPrecio))
                idListaPrecio = "-1";

            dbQuery = new CotizacionListasQuery(int.Parse(idListaPrecio), nuComponente, cdFacturacion);

            using var uow = this._uowFactory.GetUnitOfWork();

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual CotizacionListas CrearCotizacionListas(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            CotizacionListas nuevaCotizacionListas = new CotizacionListas();

            nuevaCotizacionListas.CodigoFacturacion = row.GetCell("CD_FACTURACION").Value;
            nuevaCotizacionListas.NumeroComponente = row.GetCell("NU_COMPONENTE").Value;
            nuevaCotizacionListas.CodigoListaPrecio = int.Parse(row.GetCell("CD_LISTA_PRECIO").Value);

            string importe = row.GetCell("QT_IMPORTE").Value;
            string importeMinimo = row.GetCell("QT_IMPORTE_MINIMO").Value;

            if (!string.IsNullOrEmpty(importe))
                nuevaCotizacionListas.CantidadImporte = decimal.Parse(importe, this._identity.GetFormatProvider());

            if (!string.IsNullOrEmpty(importeMinimo))
                nuevaCotizacionListas.CantidadImporteMinimo = decimal.Parse(importeMinimo, this._identity.GetFormatProvider());

            nuevaCotizacionListas.FechaAlta = DateTime.Now;
            nuevaCotizacionListas.Funcionario = this._identity.UserId;

            return nuevaCotizacionListas;
        }

        public virtual CotizacionListas UpdateCotizacionListas(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string CodigoFacturacion = row.GetCell("CD_FACTURACION").Value;
            string NumeroComponente = row.GetCell("NU_COMPONENTE").Value;
            string CodigoListaPrecio = row.GetCell("CD_LISTA_PRECIO").Value;

            CotizacionListas cotizacionListas = new CotizacionListas();
            cotizacionListas = uow.CotizacionListasRepository.GetCotizacionListas(CodigoFacturacion, NumeroComponente, int.Parse(CodigoListaPrecio));

            string importe = row.GetCell("QT_IMPORTE").Value;
            string importeMinimo = row.GetCell("QT_IMPORTE_MINIMO").Value;

            if (!string.IsNullOrEmpty(importe))
                cotizacionListas.CantidadImporte = decimal.Parse(importe, this._identity.GetFormatProvider());

            if (!string.IsNullOrEmpty(importeMinimo))
                cotizacionListas.CantidadImporteMinimo = decimal.Parse(importeMinimo, this._identity.GetFormatProvider());

            cotizacionListas.FechaModificacion = DateTime.Now;
            cotizacionListas.Funcionario = this._identity.UserId;

            return cotizacionListas;
        }

        public virtual void AddParametersComponente(string nuComponente, GridFetchContext query)
        {
            string cdFacturacion = query.GetParameter("cdFacturacion");
            string dsSignificado = cdFacturacion + "_" + nuComponente;

            query.AddParameter("descripcionListaPrecio", dsSignificado);
        }
    }
}
