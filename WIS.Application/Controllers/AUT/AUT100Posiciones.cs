using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WIS.Components.Common.Select;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.Automatismo.Logic;
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
    public class AUT100Posiciones : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ISecurityService _security;
        protected readonly IAutomatismoFactory _automatismoFactory;
        protected readonly ILogger<AUT100Posiciones> _logger;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public AUT100Posiciones(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ISecurityService security,
            IAutomatismoFactory automatismoFactory,
            ILogger<AUT100Posiciones> logger)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._security = security;
            this._automatismoFactory = automatismoFactory;
            this._logger = logger;

            this.GridKeys = new List<string>
            {
                "NU_AUTOMATISMO_POSICION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("ND_TIPO_ENDERECO", SortDirection.Ascending),
                new SortCommand("NU_ORDEN", SortDirection.Ascending)
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

            if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            context.IsEditingEnabled = true;
            context.IsAddEnabled = true;
            context.IsRemoveEnabled = true;
            context.IsCommitEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnSelect("ND_TIPO_ENDERECO", this.SearchTipoUbicacion(numeroAutomatismo)));

            grid.SetInsertableColumns(new List<string>
            {
                "ND_TIPO_ENDERECO",
                "VL_POSICION_EXTERNA",
                "NU_ORDEN"
            });

            grid.SetColumnDefaultValues(new Dictionary<string, string>
            {
                ["NU_AUTOMATISMO"] = nuAutomatismo,
            });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

            if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var query = new PosicionesAutomatismoQuery(numeroAutomatismo);

            uow.HandleQuery(query);

            grid.Rows = _gridService.GetRows(query, grid.Columns, context, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string>
            {
                "VL_POSICION_EXTERNA",
                "NU_ORDEN"
            });

            return grid;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                uow.BeginTransaction();
                uow.CreateTransactionNumber("AUT100Posiciones - GridCommit");

                var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

                if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
                {
                    context.AddErrorNotification("AUT100Posiciones_Sec0_Error_FormatoNumeroAutomatismoIncorrecto");

                    return null;
                }

                grid.Rows.ForEach(row =>
                {
                    if (row.IsNew)
                        InsertUbicacion(uow, row, numeroAutomatismo);
                    else if (row.IsModified)
                        UpdatePosicionAutomatismo(uow, row);
                    else if (row.IsDeleted)
                        DeletePosicionAutomatismo(uow, row, numeroAutomatismo);
                });

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("AUT100Posiciones_Sec0_Success_AutomatismoAuthInsertado");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                context.AddErrorNotification("General_Sec0_Error_Operacion");
                this._logger.LogError(ex, "General_Sec0_Error_Operacion");
            }

            return grid;
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

            var query = new PosicionesAutomatismoQuery(numeroAutomatismo);

            uow.HandleQuery(query);

            query.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats()
            {
                Count = query.GetCount()
            };
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

            var dbQuery = new PosicionesAutomatismoQuery(numeroAutomatismo);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        #region Metodos auxiliares
        public virtual List<SelectOption> SearchTipoUbicacion(int nroAutomatismo)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var automatismo = uow.AutomatismoRepository.GetAutomatismoById(nroAutomatismo);
            var isAutoStore = automatismo.Tipo == AutomatismoTipo.AutoStore;

            var tiposUbicaciones = uow.DominioRepository.GetDominios(AutomatismoDb.TP_UBIC_AUTOMATISMO_DOMAIN);

            foreach (var tipo in tiposUbicaciones)
            {
                if (tipo.Id != AutomatismoDb.TP_UBIC_PICKING || (tipo.Id == AutomatismoDb.TP_UBIC_PICKING && !isAutoStore))
                    opciones.Add(new SelectOption(tipo.Id, tipo.Descripcion));
            }


            return opciones;
        }

        public virtual void InsertUbicacion(IUnitOfWork uow, GridRow row, int numeroAutomatismo)
        {
            var automatismo = uow.AutomatismoRepository.GetAutomatismoById(numeroAutomatismo);
            var logic = new AutomatismoLogic(uow, _automatismoFactory);
            var orden = short.TryParse(row.GetCell("NU_ORDEN").Value, out short nuOrden) ? nuOrden : 0;
            var tipoPosicion = row.GetCell("ND_TIPO_ENDERECO").Value;
            var ubic = automatismo.GetUbicacionPorDefault(tipoPosicion, out int? tipoAgrupacion);

            var recorridoPorDefecto = uow.RecorridoRepository.GetRecorridoPorDefectoParaPredio(automatismo.Predio);

            logic.CrearUbicacionesAutomatismo(automatismo, new AutomatismoPosicion
            {
                IdAutomatismo = automatismo.Numero,
                TipoUbicacion = tipoPosicion,
                PosicionExterna = row.GetCell("VL_POSICION_EXTERNA").Value,
                Orden = (short)orden,
                Ubicacion = ubic,
                TipoAgrupacion = tipoAgrupacion,

            }, recorridoPorDefecto);
        }

        public virtual void UpdatePosicionAutomatismo(IUnitOfWork uow, GridRow row)
        {
            var nuAutomatismoPosicion = int.Parse(row.GetCell("NU_AUTOMATISMO_POSICION").Value);

            var posicionExterna = row.GetCell("VL_POSICION_EXTERNA").Value;

            var posicion = uow.AutomatismoPosicionRepository.GetAutomatismoPosicionById(nuAutomatismoPosicion);

            posicion.PosicionExterna = posicionExterna;

            if (short.TryParse(row.GetCell("NU_ORDEN").Value, out short nuOrden)) 
                posicion.Orden = nuOrden;

            uow.AutomatismoPosicionRepository.Update(posicion);
        }

        public virtual void DeletePosicionAutomatismo(IUnitOfWork uow, GridRow row, int numeroAutomatismo)
        {
            var nuAutomatismoPosicion = int.Parse(row.GetCell("NU_AUTOMATISMO_POSICION").Value);
            var cdUbicacion = row.GetCell("CD_ENDERECO").Value;
            var tipoPosicion = row.GetCell("ND_TIPO_ENDERECO").Value;

            var automatismo = uow.AutomatismoRepository.GetAutomatismoById(numeroAutomatismo);
            ValidatePreviousDeletion(uow, automatismo, cdUbicacion, tipoPosicion);

            uow.AutomatismoPosicionRepository.Remove(nuAutomatismoPosicion);

            var ubicacion = uow.UbicacionRepository.GetUbicacion(cdUbicacion);

            uow.RecorridoRepository.EliminarUbicacionDeRecorridos(ubicacion);
            uow.UbicacionRepository.RemoveUbicacion(ubicacion);
        }

        public virtual void ValidatePreviousDeletion(IUnitOfWork uow, IAutomatismo automatismo, string ubicacion, string tipoPosicion)
        {
            if (uow.StockRepository.AnyStockUbicacion(ubicacion))
                throw new ValidationFailedException("AUT100_Sec0_msg_UbicacionTieneStock");

            if (uow.StockTraceRepository.AnyStockTrace(ubicacion))
                throw new ValidationFailedException("AUT100_Sec0_msg_UbicacionHaOperado", new string[] { ubicacion });

            if (automatismo.Tipo == AutomatismoTipo.AutoStore && tipoPosicion == AutomatismoDb.TP_UBIC_PICKING)
                throw new ValidationFailedException("AUT100_Sec0_msg_UbicacionPickingNoEliminable");
        }

        #endregion
    }
}
