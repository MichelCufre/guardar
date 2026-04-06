using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Parametrizacion;
using WIS.Domain.General;
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

namespace WIS.Application.Controllers.PAR
{
    public class PAR050TiposDeUbicaciones : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<PAR050TiposDeUbicaciones> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PAR050TiposDeUbicaciones(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<PAR050TiposDeUbicaciones> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_TIPO_ENDERECO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_TIPO_ENDERECO", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = true;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = false;
            query.IsCommitEnabled = true;

            using var uow = this._uowFactory.GetUnitOfWork();

            grid.SetInsertableColumns(new List<string> {
                "CD_TIPO_ENDERECO",
                "DS_TIPO_ENDERECO",
                "VL_ALTURA",
                "VL_LARGURA",
                "VL_COMPRIMENTO",
                "VL_PESO_MAXIMO",
                "CD_TIPO_ESTRUTURA",
                "ID_VARIOS_LOTES",
                "ID_VARIOS_PRODUTOS",
                "QT_VOLUMEN_UNIDAD_FACTURACION",
                "QT_CAPAC_PALETES",
                "FL_RESPETA_CLASE"
            });

            //Cargo los selects
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_TIPO_ESTRUTURA", this.SelectTipoEstructura(uow)));
            grid.AddOrUpdateColumn(new GridColumnSelect("ID_VARIOS_LOTES", this.SelectSorN()));
            grid.AddOrUpdateColumn(new GridColumnSelect("ID_VARIOS_PRODUTOS", this.SelectSorN()));
            grid.AddOrUpdateColumn(new GridColumnSelect("FL_RESPETA_CLASE", this.SelectSorN()));

            var defaultColumns = new Dictionary<string, string>
            {
                { "QT_CAPAC_PALETES", "1" }
            };

            grid.SetColumnDefaultValues(defaultColumns);

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new TiposDeUbicacionesQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string>
            {
                "DS_TIPO_ENDERECO",
                "VL_ALTURA",
                "VL_LARGURA",
                "VL_COMPRIMENTO",
                "VL_PESO_MAXIMO",
                "CD_TIPO_ESTRUTURA",
                "ID_VARIOS_LOTES",
                "ID_VARIOS_PRODUTOS",
                "QT_VOLUMEN_UNIDAD_FACTURACION",
                "QT_CAPAC_PALETES",
                "FL_RESPETA_CLASE"
            });

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new TiposDeUbicacionesQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new TiposDeUbicacionesQuery();
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoTiposDeUbicacionesValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {
                        if (row.IsNew)
                        {
                            var tipoUbicacion = this.CrearTipoUbicacion(uow, row, query);
                            uow.UbicacionTipoRepository.AddTipoUbicacion(tipoUbicacion);
                        }
                        else
                        {
                            var tipoUbicacion = this.UpdateTipoUbicacion(uow, row, query);
                            uow.UbicacionTipoRepository.UpdateTipoUbicacion(tipoUbicacion);
                        }
                    }
                }

                uow.SaveChanges();
                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "PAR110GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        #region Metodos Auxiliares

        public virtual UbicacionTipo CrearTipoUbicacion(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            return new UbicacionTipo
            {
                Id = short.Parse(row.GetCell("CD_TIPO_ENDERECO").Value),
                Descripcion = row.GetCell("DS_TIPO_ENDERECO").Value,
                Altura = decimal.Parse(row.GetCell("VL_ALTURA").Value, this._identity.GetFormatProvider()),
                Largo = decimal.Parse(row.GetCell("VL_LARGURA").Value, this._identity.GetFormatProvider()),
                Ancho = decimal.Parse(row.GetCell("VL_COMPRIMENTO").Value, this._identity.GetFormatProvider()),
                PesoMaximo = decimal.Parse(row.GetCell("VL_PESO_MAXIMO").Value, this._identity.GetFormatProvider()),
                VolumenUnidadFacturacion = decimal.Parse(row.GetCell("QT_VOLUMEN_UNIDAD_FACTURACION").Value, this._identity.GetFormatProvider()),
                IdTipoEstatura = short.Parse(row.GetCell("CD_TIPO_ESTRUTURA").Value),
                FechaInsercion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                CapacidadPallets = int.Parse(row.GetCell("QT_CAPAC_PALETES").Value),
                PermiteVariosProductos = (row.GetCell("ID_VARIOS_PRODUTOS").Value == "S"),
                PermiteVariosLotes = (row.GetCell("ID_VARIOS_LOTES").Value == "S"),
                RespetaClase = (row.GetCell("FL_RESPETA_CLASE").Value == "S")
            };
        }

        public virtual UbicacionTipo UpdateTipoUbicacion(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            var codigoTipoUbicacion = short.Parse(row.GetCell("CD_TIPO_ENDERECO").Value);
            var tipoUbicacion = uow.UbicacionTipoRepository.GetUbicacionTipo(codigoTipoUbicacion);

            tipoUbicacion.Descripcion = row.GetCell("DS_TIPO_ENDERECO").Value;
            tipoUbicacion.Altura = decimal.Parse(row.GetCell("VL_ALTURA").Value, this._identity.GetFormatProvider());
            tipoUbicacion.Largo = decimal.Parse(row.GetCell("VL_LARGURA").Value, this._identity.GetFormatProvider());
            tipoUbicacion.Ancho = decimal.Parse(row.GetCell("VL_COMPRIMENTO").Value, this._identity.GetFormatProvider());
            tipoUbicacion.PesoMaximo = decimal.Parse(row.GetCell("VL_PESO_MAXIMO").Value, this._identity.GetFormatProvider());
            tipoUbicacion.VolumenUnidadFacturacion = decimal.Parse(row.GetCell("QT_VOLUMEN_UNIDAD_FACTURACION").Value, this._identity.GetFormatProvider());
            tipoUbicacion.IdTipoEstatura = short.Parse(row.GetCell("CD_TIPO_ESTRUTURA").Value);
            tipoUbicacion.FechaModificacion = DateTime.Now;
            tipoUbicacion.CapacidadPallets = int.Parse(row.GetCell("QT_CAPAC_PALETES").Value);
            tipoUbicacion.PermiteVariosProductos = (row.GetCell("ID_VARIOS_PRODUTOS").Value == "S");
            tipoUbicacion.PermiteVariosLotes = (row.GetCell("ID_VARIOS_LOTES").Value == "S");
            tipoUbicacion.RespetaClase = (row.GetCell("FL_RESPETA_CLASE").Value == "S");

            return tipoUbicacion;
        }

        public virtual List<SelectOption> SelectTipoEstructura(IUnitOfWork uow)
        {
            return uow.UbicacionTipoRepository.GetTiposEstructuras()
                .Select(w => new SelectOption(w.Codigo.ToString(), $"{w.Codigo} - {w.Tipo}"))
                .ToList();
        }

        public virtual List<SelectOption> SelectSorN()
        {
            return new List<SelectOption>()
            {
                new SelectOption("N","N"),
                new SelectOption("S","S"),
            };
        }

        #endregion
    }
}
