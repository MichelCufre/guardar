using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Automatizacion;
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
    public class AUT100Caracteristicas : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public AUT100Caracteristicas(
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
                "NU_AUTOMATISMO_CARACTERISTICA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_AUTOMATISMO_CARACTERISTICA", SortDirection.Descending)
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsCommitEnabled = false;
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = grid.Id == "AUT100Modal_grid_3";

            if (grid.Id == "AUT100Modal_grid_3")
                grid.AddOrUpdateColumn(new GridColumnSelect("CD_AUTOMATISMO_CARACTERISTICA", this.InitializeSelectCodigoAutomatismo()));

            return GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

            if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (grid.Id == "AUT100Modal_grid_3")
                ConfigurarGrillaListaCaracteristicas(uow, grid, context, numeroAutomatismo);

            else ConfigurarGrillaCaracteristicasValorUnico(uow, grid, context, numeroAutomatismo);

            return grid;

        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

            if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
            {
                context.AddErrorNotification(string.Format("AUT100Posiciones_Sec0_Error_FormatoNumeroAutomatismoIncorrecto"));

                return null;
            }

            context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            if (grid.Id == "AUT100Modal_grid_3") return GetExcelGridLista(uow, numeroAutomatismo, context, grid);

            return GetExcelGridSingleValue(uow, numeroAutomatismo, context, grid);
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

            if (grid.Id == "AUT100Modal_grid_3")
                return FetchStatsGridLista(uow, numeroAutomatismo, context);

            return FetchStatsGridSingleValue(uow, numeroAutomatismo, context);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

                if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                uow.BeginTransaction();

                foreach (var row in grid.Rows)
                {
                    if (row.IsNew) 
                        InsertCaracteristica(uow, row, numeroAutomatismo, context);
                    else 
                        UpdateCaracteristica(uow, row, context, grid, numeroAutomatismo);
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("AUT100Caracteristicas_Sec0_Success_CaracteristicaModificada");
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

            var rowIsInserting = string.IsNullOrEmpty(row.GetCell("VL_OPCIONES").Value);

            return this._gridValidationService.Validate(new AutomatismoCaracteristicasGridValidationModule(uow, _identity.GetFormatProvider(), rowIsInserting), grid, row, context);
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            if (grid.Id == "AUT100Modal_grid_4")
            {
                switch (context.ColumnId)
                {
                    case "VL_AUTOMATISMO_CARACTERISTICA": return this.SearchOpciones(row, grid, context);
                }
            }

            return new List<SelectOption>();
        }

        #region Metodos Auxiliares

        public virtual void ConfigurarGrillaListaCaracteristicas(IUnitOfWork uow, Grid grid, GridFetchContext context, int numeroAutomatismo)
        {
            var query = new CaracteristicasAutomatismoQuery(numeroAutomatismo);

            uow.HandleQuery(query);

            grid.Rows = _gridService.GetRows(query, grid.Columns, context, this.DefaultSort, this.GridKeys);

            grid.SetInsertableColumns(new List<string>
            {
                "CD_AUTOMATISMO_CARACTERISTICA",
                "VL_AUTOMATISMO_CARACTERISTICA",
                "DS_AUTOMATISMO_CARACTERISTICA",
                "VL_AUX1",
                "NU_AUX1",
                "QT_AUX1",
                "FL_AUX1",
            });

            grid.SetEditableCells(new List<string>
            {
                "CD_AUTOMATISMO_CARACTERISTICA",
                "VL_AUTOMATISMO_CARACTERISTICA",
                "DS_AUTOMATISMO_CARACTERISTICA",
                "VL_AUX1",
                "NU_AUX1",
                "QT_AUX1",
                "FL_AUX1",
            });

            grid.Columns.FirstOrDefault(i => i.Id == "VL_OPCIONES").Hidden = true;
            grid.Columns.FirstOrDefault(i => i.Id == "NU_AUTOMATISMO").Hidden = true;
            grid.Columns.FirstOrDefault(i => i.Id == "NU_AUTOMATISMO_CARACTERISTICA").Hidden = true;
        }

        public virtual void ConfigurarGrillaCaracteristicasValorUnico(IUnitOfWork uow, Grid grid, GridFetchContext context, int numeroAutomatismo)
        {
            var query = new CaracteristicasValorUnicoQuery(numeroAutomatismo);

            uow.HandleQuery(query);

            grid.Rows = _gridService.GetRows(query, grid.Columns, context, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> { "VL_AUTOMATISMO_CARACTERISTICA" });

            grid.Columns.Where(i => i.Id != "DS_AUTOMATISMO_CARACTERISTICA" && i.Id != "VL_AUTOMATISMO_CARACTERISTICA").ToList().ForEach(i => i.Hidden = true);
        }

        public virtual GridStats FetchStatsGridLista(IUnitOfWork uow, int numeroAutomatismo, GridFetchStatsContext context)
        {
            var query = new CaracteristicasAutomatismoQuery(numeroAutomatismo);

            uow.HandleQuery(query);

            query.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats()
            {
                Count = query.GetCount()
            };
        }

        public virtual GridStats FetchStatsGridSingleValue(IUnitOfWork uow, int numeroAutomatismo, GridFetchStatsContext context)
        {
            var query = new CaracteristicasValorUnicoQuery(numeroAutomatismo);

            uow.HandleQuery(query);

            query.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats()
            {
                Count = query.GetCount()
            };
        }

        public virtual byte[] GetExcelGridLista(IUnitOfWork uow, int numeroAutomatismo, GridExportExcelContext context, Grid grid)
        {
            var dbQuery = new CaracteristicasAutomatismoQuery(numeroAutomatismo);

            uow.HandleQuery(dbQuery);

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public virtual byte[] GetExcelGridSingleValue(IUnitOfWork uow, int numeroAutomatismo, GridExportExcelContext context, Grid grid)
        {
            var dbQuery = new CaracteristicasValorUnicoQuery(numeroAutomatismo);

            uow.HandleQuery(dbQuery);

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public virtual List<SelectOption> SearchOpciones(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var codigoCaracteristica = row.GetCell("CD_AUTOMATISMO_CARACTERISTICA").Value;

            var automatismo = uow.AutomatismoRepository.GetAutomatismoById(int.Parse(context.GetParameter("AUT100_NU_AUTOMATISMO")));

            var sql = uow.AutomatismoCaracteristicaRepository.GetOpcionesDinamicas(automatismo.Tipo, codigoCaracteristica);

            DynamicParameters parameters = new DynamicParameters();

            if (codigoCaracteristica == AutomatismoDb.CARACTERISTICA_COLOR_ERROR || codigoCaracteristica == AutomatismoDb.CARACTERISTICA_COLOR_CERRADO)
            {
                sql += " AND NU_AUTOMATISMO = :P_NU_AUTOMATISMO";

                parameters.Add("P_NU_AUTOMATISMO", automatismo.Numero);
            }

            parameters.Add("LIKE_CLAUSE", context.SearchValue);

            return uow.AutomatismoCaracteristicaRepository.GetOpcionesDeCaracteristica(sql, parameters);
        }

        public virtual List<SelectOption> InitializeSelectCodigoAutomatismo()
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var dominios = uow.DominioRepository.GetDominios(AutomatismoDb.TP_CARACTERISTICA_DOMAIN);

            foreach (var dominio in dominios)
                opciones.Add(new SelectOption(dominio.Id, dominio.Descripcion));

            return opciones;
        }

        public virtual void InsertCaracteristica(IUnitOfWork uow, GridRow row, int numeroAutomatismo, GridFetchContext context)
        {
            uow.CreateTransactionNumber($"InsertCaracteristica - {_identity.Application}");

            var caracteristica = new AutomatismoCaracteristica
            {
                Codigo = row.GetCell("CD_AUTOMATISMO_CARACTERISTICA").Value,
                IdAutomatismo = numeroAutomatismo,
                Descripcion = row.GetCell("DS_AUTOMATISMO_CARACTERISTICA").Value,
                FlagAuxiliar = row.GetCell("FL_AUX1").Value == "S",
                Valor = row.GetCell("VL_AUTOMATISMO_CARACTERISTICA").Value,
                ValorAuxiliar = row.GetCell("VL_AUX1").Value,
            };

            if (decimal.TryParse(row.GetCell("QT_AUX1").Value, out decimal cantidadAux)) caracteristica.CantidadAuxiliar = cantidadAux;

            if (long.TryParse(row.GetCell("QT_AUX1").Value, out long numeroAux)) caracteristica.NumeroAuxiliar = numeroAux;

            uow.AutomatismoCaracteristicaRepository.Add(caracteristica);

            uow.SaveChanges();

            ActualizarValoresSegunCaracteristica(uow, caracteristica, numeroAutomatismo, context);

            uow.SaveChanges();
        }

        public virtual void UpdateCaracteristica(IUnitOfWork uow, GridRow row, GridFetchContext context, Grid grid, int numeroAutomatismo)
        {
            uow.CreateTransactionNumber($"UpdateCaracteristica - {_identity.Application}");

            var caracteristica = uow.AutomatismoCaracteristicaRepository.GetAutomatismoCaracteristicaById(long.Parse(row.GetCell("NU_AUTOMATISMO_CARACTERISTICA").Value));

            if (grid.Id == "AUT100Modal_grid_3")
            {
                caracteristica.Codigo = row.GetCell("CD_AUTOMATISMO_CARACTERISTICA").Value;
                caracteristica.Descripcion = row.GetCell("DS_AUTOMATISMO_CARACTERISTICA").Value;
                caracteristica.FlagAuxiliar = row.GetCell("FL_AUX1").Value == "S";
                caracteristica.ValorAuxiliar = row.GetCell("VL_AUX1").Value;
            }

            caracteristica.Valor = row.GetCell("VL_AUTOMATISMO_CARACTERISTICA").Value;
            caracteristica.FechaModificacion = DateTime.Now;
            caracteristica.Transaccion = uow.GetTransactionNumber();

            if (decimal.TryParse(row.GetCell("QT_AUX1").Value, out decimal cantidadAux)) caracteristica.CantidadAuxiliar = cantidadAux;

            if (long.TryParse(row.GetCell("QT_AUX1").Value, out long numeroAux)) caracteristica.NumeroAuxiliar = numeroAux;

            uow.AutomatismoCaracteristicaRepository.Update(caracteristica);

            uow.SaveChanges();

            ActualizarValoresSegunCaracteristica(uow, caracteristica, numeroAutomatismo, context);

            uow.SaveChanges();
        }

        public virtual void ActualizarValoresSegunCaracteristica(IUnitOfWork uow, AutomatismoCaracteristica caracteristica, int numeroAutomatismo, GridFetchContext context)
        {
            var automatismo = uow.AutomatismoRepository.GetAutomatismoById(numeroAutomatismo);

            var configDefault = uow.AutomatismoCaracteristicaRepository.GetCaracteristicasPorDefecto();

            automatismo.ActualizarValoresSegunCaracteristica(caracteristica, configDefault);

            uow.AutomatismoPosicionRepository.InsertOrUpdateOrDelete(automatismo.Posiciones.ToList());
            uow.AutomatismoCaracteristicaRepository.InsertOrUpdateOrDelete(automatismo.Caracteristicas.ToList());

            if (caracteristica.Codigo == AutomatismoDb.CARACTERISTICA_AGRUPACION_UBIC)
            {
                context.AddParameter("UPDATE_GRID_POSICIONES", "S");
            }
        }

        #endregion
    }
}
