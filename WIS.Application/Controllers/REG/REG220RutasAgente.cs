using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.DataModel.Queries.Registro;
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

namespace WIS.Application.Controllers.REG
{
    public class REG220RutasAgente : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG220RutasAgente> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeysDetalle { get; }

        public REG220RutasAgente(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            ILogger<REG220RutasAgente> logger,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService, 
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeysDetalle = new List<string>
            {
                "CD_EMPRESA", "CD_CLIENTE", "CD_ROTA", "NU_PREDIO"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {

            context.IsEditingEnabled = true;
            context.IsAddEnabled = true;
            context.IsCommitEnabled = true;
            context.IsRemoveEnabled = true;
            // query.IsCommitButtonUnavailable = true;

            grid.AddOrUpdateColumn(new GridColumnSelect("NU_PREDIO", this.OptionSelectPredio()));
            // grid.AddOrUpdateColumn(new GridColumnSelect("CD_ROTA", this.OptionSelectRuta()));

            grid.SetInsertableColumns(new List<string>
            {
                "CD_ROTA",
                "NU_PREDIO",

            });

            return this.GridFetchRows(grid, context.FetchContext);

        }
        
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var codigoInternoAgente = context.GetParameter("keyCodigo");
            var codigoEmpresa = context.GetParameter("keyEmpresa");

            if (string.IsNullOrEmpty(codigoInternoAgente) || string.IsNullOrEmpty(codigoEmpresa))
                throw new MissingParameterException("REG220_Frm1_Error_AgenteNoExiste");

            var dbQuery = new AgentesRutasPredioQuery(codigoInternoAgente, int.Parse(codigoEmpresa), this._identity.UserId);

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("NU_PREDIO", SortDirection.Descending);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeysDetalle);

            foreach (var row in grid.Rows)
            {
                row.SetEditableCells(new List<string> {
                    // "NU_PREDIO",
                    "CD_ROTA",
                });
            }

            return grid;
        }
        
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var codigoInternoAgente = context.GetParameter("keyCodigo");
            var codigoEmpresa = context.GetParameter("keyEmpresa");

            if (string.IsNullOrEmpty(codigoInternoAgente) || string.IsNullOrEmpty(codigoEmpresa))
                throw new MissingParameterException("REG220_Frm1_Error_AgenteNoExiste");

            var dbQuery = new AgentesRutasPredioQuery(codigoInternoAgente, int.Parse(codigoEmpresa), this._identity.UserId);

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("NU_PREDIO", SortDirection.Descending);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }
        
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var codigoInternoAgente = context.GetParameter("keyCodigo");
            var codigoEmpresa = context.GetParameter("keyEmpresa");

            if (string.IsNullOrEmpty(codigoInternoAgente) || string.IsNullOrEmpty(codigoEmpresa))
                throw new MissingParameterException("REG220_Frm1_Error_AgenteNoExiste");

            var dbQuery = new AgentesRutasPredioQuery(codigoInternoAgente, int.Parse(codigoEmpresa), this._identity.UserId);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        
        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_ROTA": return this.SearchRuta(grid, row, context);

            }

            return new List<SelectOption>();
        }
        
        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {

                    if (grid.HasNewDuplicates(this.GridKeysDetalle))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(new List<string>() { "NU_PREDIO", "CD_ROTA" }))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    var codigoInternoAgente = context.GetParameter("keyCodigo");
                    var codigoEmpresa = context.GetParameter("keyEmpresa");

                    foreach (var row in grid.Rows)
                    {
                        if (row.IsNew)
                        {
                            this.NuevaAsignacion(uow, row, codigoInternoAgente, codigoEmpresa);
                        }
                        else
                        {

                            //Se comprueba que no existan pedidos pendientes con el predio y ruta anterior para no crear incoherencias
                            var queryPendientes = new PedidosPendientesQuery(int.Parse(codigoEmpresa), codigoInternoAgente, row.GetCell("NU_PREDIO").Value, short.Parse(row.GetCell("CD_ROTA").Old));
                            uow.HandleQuery(queryPendientes);

                            if (queryPendientes.AnyPedidosPendientes())
                                throw new ValidationFailedException("REG220_Grid_Error_AgenteRutaPredioInconsistente", new string[] { row.GetCell("DS_PREDIO").Value, row.GetCell("DS_ROTA").Value });

                            if (row.IsDeleted)
                            {
                                this.RemoveAsignacion(uow, row, codigoInternoAgente, codigoEmpresa);

                            }
                            else
                            {
                                // rows editadas
                                this.EditarAsignacion(uow, row, codigoInternoAgente, codigoEmpresa);

                            }

                        }

                    }

                }

                uow.SaveChanges();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ExpectedException ex)
            {
                this._logger.LogWarning(ex, "GridCommit");
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REG140GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }
        
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoRutasDeAgenteValidationModule(uow, this._identity.UserId), grid, row, context);
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> OptionSelectPredio()
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            List<Predio> predios = dbQuery.GetPrediosUsuario(_identity.UserId);
            foreach (var predio in predios)
            {
                opciones.Add(new SelectOption(predio.Numero, $"{predio.Numero} - { predio.Descripcion}")); ;
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchRuta(Grid grid, GridRow row, GridSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            if (!string.IsNullOrEmpty(row.GetCell("NU_PREDIO").Value))
            {

                var dbQuery = new RutaOndaQuery(this._identity.UserId);
                uow.HandleQuery(dbQuery);

                List<Ruta> rutas = uow.RutaRepository.GetByDescripcionOrCodePartial(context.SearchValue, row.GetCell("NU_PREDIO").Value);
                foreach (var ruta in rutas)
                {
                    string descRuta = ruta.Descripcion;
                    descRuta += " - " + (ruta.Onda == null ? "SIN ONDA" : ruta.Onda?.Descripcion);

                    if (!string.IsNullOrEmpty(ruta.Zona))
                        descRuta += " - " + ruta.Zona;

                    opciones.Add(new SelectOption(ruta.Id.ToString(), descRuta));
                }

            }
            else
            {
                row.GetCell("NU_PREDIO").SetError("General_Sec0_Error_Error25", new List<string>());
            }

            return opciones;
        }

        public virtual AgenteRutaPredio NuevaAsignacion(IUnitOfWork uow, GridRow row, string codigoInternoAgente, string codigoEmpresa)
        {
            var agenteRutaPredio = new AgenteRutaPredio()
            {
                Empresa = int.Parse(codigoEmpresa),
                CodigoInternoAgente = codigoInternoAgente,
                Ruta = short.Parse(row.GetCell("CD_ROTA").Value),
                Predio = row.GetCell("NU_PREDIO").Value
            };

            uow.AgenteRepository.AddClienteRutaPredio(agenteRutaPredio);

            return agenteRutaPredio;
        }
        
        public virtual AgenteRutaPredio EditarAsignacion(IUnitOfWork uow, GridRow row, string codigoInternoAgente, string codigoEmpresa)
        {
            AgenteRutaPredio ruta = null;

            ruta = uow.AgenteRepository.GetAgenteRutaPredio(int.Parse(codigoEmpresa), codigoInternoAgente, row.GetCell("NU_PREDIO").Value, short.Parse(row.GetCell("CD_ROTA").Old));

            if (ruta == null)
                throw new EntityNotFoundException("REG220_Grid_Error_AgenteRutaPredioNoExiste", new string[] { row.GetCell("CD_ROTA").Value });

            ruta.Ruta = short.Parse(row.GetCell("CD_ROTA").Value);

            uow.AgenteRepository.UpdateClienteRutaPredio(ruta);

            return ruta;
        }
        
        public virtual void RemoveAsignacion(IUnitOfWork uow, GridRow row, string codigoInternoAgente, string codigoEmpresa)
        {
            AgenteRutaPredio ruta = null;

            ruta = uow.AgenteRepository.GetAgenteRutaPredio(int.Parse(codigoEmpresa), codigoInternoAgente, row.GetCell("NU_PREDIO").Value, short.Parse(row.GetCell("CD_ROTA").Old));

            if (ruta == null)
                throw new EntityNotFoundException("REG220_Grid_Error_AgenteRutaPredioNoExiste", new string[] { row.GetCell("CD_ROTA").Value });

            uow.AgenteRepository.RemoveClienteRutaPredio(ruta);

        }
        
        #endregion

    }
}
