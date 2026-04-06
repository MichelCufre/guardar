using Microsoft.VisualStudio.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Security;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Recorridos;
using WIS.Domain.Services.Interfaces;
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
using WIS.Persistence.Database;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.REG
{
    public class REG705PanelPredios : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrackingService _trackingService;
        protected readonly ISecurityService _security;

        protected List<string> GridKeys { get; }

        public REG705PanelPredios(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter,
            ITrackingService trackingService,
            ISecurityService security)
        {
            this.GridKeys = new List<string>
            {
                "NU_PREDIO"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._gridValidationService = gridValidationService;
            this._filterInterpreter = filterInterpreter;
            this._trackingService = trackingService;
            this._security = security;
        }

        #region GRID
        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = true;
            query.IsCommitEnabled = true;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
            {
                new GridButton("btnEnviarTracking", "REG705_Sec0_btn_EnviarTracking", "fas fa-share-square")
            }));

            var insertableColumns = new List<string>
            {
                "NU_PREDIO",
                "DS_PREDIO",
                "DS_ENDERECO"
            };

            if (!_trackingService.TrackingHabilitado())
                insertableColumns.Add("ID_EXTERNO"); // Si tracking esta habilitado el idExterno correspondera al punto de entrega.

            grid.SetInsertableColumns(insertableColumns);

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            SortCommand defaultSort = new SortCommand("NU_PREDIO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PrediosQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

            var editableColumns = new List<string>
            {
                "DS_PREDIO",
                "DS_ENDERECO"
            };

            if (!_trackingService.TrackingHabilitado())
                editableColumns.Add("ID_EXTERNO");

            grid.SetEditableCells(editableColumns);

            foreach (var row in grid.Rows)
            {
                if (!_trackingService.TrackingHabilitado() || !this._security.IsUserAllowed(SecurityResources.REG705_Sec0_btn_EnviarTracking)
                || row.GetCell("FL_SYNC_REALIZADA")?.Value == "S")
                    row.DisabledButtons.Add("btnEnviarTracking");
            }


            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PrediosQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            SortCommand defaultSort = new SortCommand("NU_PREDIO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            PrediosQuery dbQuery = null;

            dbQuery = new PrediosQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();
            uow.CreateTransactionNumber("GridCommit", _identity.Application, _identity.UserId);

            try
            {
                if (grid.Rows.Any())
                {

                    if (grid.HasNewDuplicates(new List<string> { "NU_PREDIO" }))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {
                        var codigo = row.GetCell("NU_PREDIO").Value;
                        var descripcion = row.GetCell("DS_PREDIO").Value;
                        var direccion = row.GetCell("DS_ENDERECO").Value;
                        var idExterno = row.GetCell("ID_EXTERNO").Value;

                        Predio predio = uow.PredioRepository.GetPredio(codigo);

                        if (predio != null)
                        {
                            if (row.IsDeleted)
                            {
                                if (uow.UbicacionRepository.AnyUbicacionConPredio(predio.Numero))
                                    throw new ValidationFailedException("REG705_Sec0_Error_UbicacionConPredio");

                                if (uow.PredioRepository.AnyPredioConAsignacionUsuario(predio.Numero, _identity.UserId))
                                    throw new ValidationFailedException("REG705_Sec0_Error_PredioAsignacionUser");
                                if (uow.RecorridoRepository.AnyRecorridoPredioPersonalizado(predio.Numero))
                                    throw new ValidationFailedException("REG705_Sec0_Error_PredioAsignacionRecorrido");

                                var recorrido = uow.RecorridoRepository.GetRecorridoPorDefectoParaPredio(predio.Numero);
                                recorrido.Transaccion = uow.GetTransactionNumber();
                                recorrido.TransaccionDelete = uow.GetTransactionNumber();

                                uow.RecorridoRepository.UpdateRecorrido(recorrido);
                                uow.SaveChanges();

                                uow.RecorridoRepository.DeleteRecorrido(recorrido);
                                uow.SaveChanges();

                                uow.PredioRepository.RemoverPredioUsuarios(predio.Numero, new List<int> { _identity.UserId });

                                uow.PredioRepository.RemovePredio(predio);
                                uow.DominioRepository.RemoveDominioPredio(predio.Numero);

                            }
                            else
                            {
                                predio.Descripcion = descripcion;
                                predio.Direccion = direccion;
                                predio.IdExterno = idExterno;
                                predio.Modificacion = DateTime.Now;

                                if (predio.Direccion != direccion)
                                {
                                    predio.PuntoEntrega = null;
                                    predio.SincronizacionRealizada = false;

                                    if (_trackingService.TrackingHabilitado())
                                        _trackingService.SincronizarPredio(predio);
                                }

                                uow.PredioRepository.UpdatePredio(predio);
                            }
                        }
                        else
                        {
                            predio = new Predio();
                            predio.Numero = codigo;
                            predio.Descripcion = descripcion;
                            predio.Direccion = direccion;
                            predio.IdExterno = idExterno;
                            predio.Alta = DateTime.Now;

                            uow.PredioRepository.AddPredio(predio);
                            uow.DominioRepository.AddDetalleDominioPredio(predio.Numero, predio.Descripcion);

                            uow.PredioRepository.AsignarPredioUsuarios(predio, new List<int> { _identity.UserId });

                            if (_identity.UserId != 0)
                                uow.PredioRepository.AsignarPredioUsuarios(predio, new List<int> { 0 });

                            string cliente = $"DEP-{predio.Numero}";

                            if (!uow.AgenteRepository.AnyCliente(cliente, 1))
                            {
                                Agente agente = new Agente();
                                agente.Tipo = TipoAgenteDb.Deposito;
                                agente.Codigo = cliente;
                                agente.Descripcion = predio.Descripcion;
                                agente.Empresa = int.Parse(uow.ParametroRepository.GetParameter("EMP_DEFAULT_ENVASE") ?? "1");
                                agente.Ruta = uow.RutaRepository.GetRuta(0); // Ruta interna de WIS
                                agente.Estado = EstadoAgente.Activo;

                                uow.AgenteRepository.AddAgente(agente);
                            }

                            uow.SaveChanges();

                            var recorrido = InsertarRecorrido(uow, predio);
                            uow.SaveChanges();

                            uow.RecorridoRepository.AsociarRecorridoPorDefectoAplicaciones(recorrido);

                            if (_trackingService.TrackingHabilitado())
                            {
                                _trackingService.SincronizarPredio(predio);
                                uow.PredioRepository.UpdatePredio(predio);
                            }
                        }
                        uow.SaveChanges();
                    }
                }

                uow.SaveChanges();
                uow.Commit();
                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                query.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                query.AddErrorNotification(ex.Message);
                uow.Rollback();
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoPrediosGridValidationModule(uow), grid, row, context);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext data)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                if (data.Parameters.Any(x => x.Id == "predio"))
                {
                    string nuPredio = data.Parameters.FirstOrDefault(x => x.Id == "predio").Value;
                    if (string.IsNullOrEmpty(nuPredio))
                        throw new ValidationFailedException("REG705_Sec0_Error_NoSePudoObtenerPredio");

                    Predio predio = uow.PredioRepository.GetPredio(nuPredio);
                    if (predio == null)
                        throw new ValidationFailedException("REG705_Sec0_Error_NoSePudoObtenerPredio");

                    if (data.ButtonId == "btnEnviarTracking")
                    {
                        _trackingService.SincronizarPredio(predio);
                        uow.PredioRepository.UpdatePredio(predio);
                        uow.SaveChanges();
                        data.AddSuccessNotification("REG705_Sec0_msg_UsuarioEnviado");
                    }

                    uow.Commit();
                }
            }
            catch (ValidationFailedException ex)
            {
                data.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                data.AddErrorNotification(ex.Message);
                uow.Rollback();
            }

            return data;
        }
        #endregion


        #region AUX
        public virtual Recorrido InsertarRecorrido(IUnitOfWork uow, Predio predio)
        {
            var recorrido = new Recorrido
            {
                Nombre = $"{RecorridosConstants.DEFAULT}_{predio.Numero}",
                Descripcion = $"Por defecto para {predio.Descripcion}",
                EsDefault = true,
                Predio = predio.Numero,
                Transaccion = uow.GetTransactionNumber(),
                EsHabilitado = true
            };

            return uow.RecorridoRepository.AddRecorrido(recorrido);
        }
        #endregion
    }
}
