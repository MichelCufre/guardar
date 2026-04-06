using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Recepcion;
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

namespace WIS.Application.Controllers.REC
{
    public class REC150Etiquetas : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REC150Etiquetas> _logger;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC150Etiquetas(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<REC150Etiquetas> logger)
        {
            this.GridKeys = new List<string>
            {
               "NU_ETIQUETA_LOTE", "NU_AGENDA", "NU_EXTERNO_ETIQUETA", "TP_ETIQUETA", "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR", "CD_EMPRESA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_AGENDA", SortDirection.Descending),
                new SortCommand ("NU_ETIQUETA_LOTE",SortDirection.Descending)
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._gridValidationService = gridValidationService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsAddEnabled = false;
            context.IsRemoveEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem> {
                new GridButton("btnLogEtiquetas", "REC150_Sec0_btn_btnLogEtiquetas", "fas fa-list"),
                new GridButton("btnUTs", "REC150_Sec0_btn_btnUTs", "fas fa-list")
            }));

            context.AddLink("CD_EMPRESA", "registro/REG100", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa") });
            context.AddLink("CD_AGENTE", "registro/REG220", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_CLIENTE", "cliente"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });
            context.AddLink("CD_ENDERECO", "registro/REG040", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_ENDERECO", "ubicacion") });

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ConsultaEtiquetasQuery dbQuery;

            Agenda agenda = null;

            if (context.Parameters.Count > 3)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "agenda")?.Value, out int idAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!decimal.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "embalaje")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal embalaje))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "identificador")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = context.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value;
                string identificador = context.Parameters.FirstOrDefault(s => s.Id == "identificador").Value;

                dbQuery = new ConsultaEtiquetasQuery(idAgenda, idProducto, identificador, embalaje);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

                context.AddParameter("REC150_NU_AGENDA", idAgenda.ToString());
                context.AddParameter("REC150_NU_IDENTIFICADOR", identificador);
                context.AddParameter("REC150_CD_FAIXA", embalaje.ToString());

                grid.GetColumn("CD_FAIXA").Hidden = true;
                grid.GetColumn("NU_IDENTIFICADOR").Hidden = true;
                grid.GetColumn("NU_AGENDA").Hidden = true;
                grid.GetColumn("CD_PRODUTO").Hidden = true;

                string ds_Prod = " - ";

                if (grid.Rows.Count > 0)
                {
                    string idEmpresa = grid.Rows.FirstOrDefault().GetCell("CD_EMPRESA").Value;
                    string descProducto = uow.ProductoRepository.GetDescripcion(int.Parse(idEmpresa), idProducto);

                    ds_Prod = ds_Prod + descProducto;

                    grid.GetColumn("DS_PRODUTO").Hidden = true;

                    agenda = uow.AgendaRepository.GetAgendaSinDetalles(idAgenda);
                }

                context.AddParameter("REC150_CD_PRODUCTO", idProducto + ds_Prod);

            }
            else if (context.Parameters.Count > 0)
            {

                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "agenda")?.Value, out int idAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ConsultaEtiquetasQuery(idAgenda);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

                context.AddParameter("REC150_NU_AGENDA", idAgenda.ToString());
            }
            else
            {
                dbQuery = new ConsultaEtiquetasQuery();

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }


            foreach (var row in grid.Rows)
            {
                if (new AjusteEtiquetaLote(uow
                                        , short.Parse(row.GetCell("CD_SITUACAO")?.Value)
                                        , short.Parse(row.GetCell("CD_SITUACAO_DET_AGENDA")?.Value)
                                        , decimal.Parse(row.GetCell("QT_PRODUTO_RECIBIDO").Value, this._identity.GetFormatProvider())
                                        , decimal.Parse(row.GetCell("QT_PRODUTO").Value, this._identity.GetFormatProvider())
                                        , int.Parse(row.GetCell("NU_AGENDA").Value)).EsEtiquetaAjustable())
                {

                    if (agenda == null)
                        agenda = uow.AgendaRepository.GetAgendaSinDetalles(int.Parse(row.GetCell("NU_AGENDA")?.Value));

                    EtiquetaLote etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLote(int.Parse(row.GetCell("NU_ETIQUETA_LOTE")?.Value));
                    if (!(!agenda.EnEstadoConferidaConDiferencias() && !agenda.EnEstadoConferidaSinDiferencias()) 
                            && !uow.ManejoLpnRepository.IsEtiquetaLpnRecepcion(etiqueta.CodigoBarras, etiqueta.TipoEtiqueta) 
                            && string.IsNullOrEmpty(etiqueta.IdUbicacionSugerida)
                            && !uow.AlmacenamientoRepository.AnySugerenciaAlmacenajePendienteFraccionado(etiqueta.CodigoBarras)
                            && !uow.AlmacenamientoRepository.AnySugerenciaAlmacenajePendienteClasificacion(etiqueta.CodigoBarras)
                            && !uow.AlmacenamientoRepository.AnySugerenciaAlmacenajeReabastecimintoPendienteClasificacion(etiqueta.Numero))
                    {
                        row.SetEditableCells(new List<string>
                        {
                            "QT_PRODUTO_RECIBIDO"
                        });
                    }

                }

                var manejaUT = !string.IsNullOrEmpty(row.GetCell("NU_UNIDAD_TRANSPORTE_UT")?.Value);

                if (!manejaUT)
                    row.DisabledButtons.Add("btnUTs");
            }

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ConsultaEtiquetasQuery dbQuery;

            if (context.Parameters.Count > 3)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "agenda")?.Value, out int idAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!decimal.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "embalaje")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal embalaje))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "identificador")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = context.Parameters.FirstOrDefault(s => s.Id == "producto").Value;
                string identificador = context.Parameters.FirstOrDefault(s => s.Id == "identificador").Value;

                dbQuery = new ConsultaEtiquetasQuery(idAgenda, idProducto, identificador, embalaje);

            }
            else if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "agenda")?.Value, out int idAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ConsultaEtiquetasQuery(idAgenda);
            }
            else
            {
                dbQuery = new ConsultaEtiquetasQuery();

            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ConsultaEtiquetasQuery dbQuery;

            if (context.Parameters.Count > 3)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "agenda")?.Value, out int idAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (!decimal.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "embalaje")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal embalaje))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "identificador")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idProducto = context.Parameters.FirstOrDefault(s => s.Id == "producto").Value;
                string identificador = context.Parameters.FirstOrDefault(s => s.Id == "identificador").Value;

                dbQuery = new ConsultaEtiquetasQuery(idAgenda, idProducto, identificador, embalaje);

            }
            else if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "agenda")?.Value, out int idAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ConsultaEtiquetasQuery(idAgenda);
            }
            else
            {
                dbQuery = new ConsultaEtiquetasQuery();
            }

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("NU_ETIQUETA_LOTE", SortDirection.Ascending);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            if (context.ButtonId == "btnLogEtiquetas")
            {
                context.Redirect("/recepcion/REC180", new List<ComponentParameter>() {
                      new ComponentParameter(){ Id = "producto", Value = context.Row.GetCell("CD_PRODUTO").Value},
                      new ComponentParameter(){ Id = "etiqueta", Value = context.Row.GetCell("NU_ETIQUETA_LOTE").Value},
                      new ComponentParameter(){ Id = "identificador", Value = context.Row.GetCell("NU_IDENTIFICADOR").Value},
                      new ComponentParameter(){ Id = "embalaje", Value = context.Row.GetCell("CD_FAIXA").Value},
                      new ComponentParameter(){ Id = "numeroExterno", Value = context.Row.GetCell("NU_EXTERNO_ETIQUETA").Value},
                });
            }
            else if (context.ButtonId == "btnUTs")
            {
                context.Redirect("/stock/STO750", true, new List<ComponentParameter>() {
                      new ComponentParameter(){ Id = "nroUT", Value = context.Row.GetCell("NU_UNIDAD_TRANSPORTE_UT").Value}
                });
            }

            return context;
        }
        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                uow.CreateTransactionNumber($"{this._identity.Application} - Etiquetas");
                uow.BeginTransaction(uow.GetSnapshotIsolationLevel());

                if (grid.Rows.Any())
                {
                    foreach (var row in grid.Rows)
                    {
                        if (row.IsNew)
                        {
                            throw new OperationNotAllowedException("General_Sec0_msg_DeleteNotImplemented");
                        }
                        else if (row.IsDeleted)
                        {
                            throw new OperationNotAllowedException("General_Sec0_msg_DeleteNotImplemented");
                        }
                        else
                        {
                            // rows editadas
                            this.EditarEtiqueta(uow, row);
                        }
                    }
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ExpectedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                uow.Rollback();
                this._logger.LogError(ex, "REG603GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion_Reintente");
            }
            finally
            {
                uow.EndTransaction();
            }

            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new AjusteEtiquetaRecepcionGridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public virtual void EditarEtiqueta(IUnitOfWork uow, GridRow row)
        {
            new AjusteEtiquetaLote(uow
                                , this._identity.UserId
                                , this._identity.Application
                                , int.Parse(row.GetCell("NU_AGENDA").Value)
                                , row.GetCell("CD_ENDERECO").Value
                                , row.GetCell("CD_ENDERECO_SUGERIDO").Value
                                , row.GetCell("CD_PRODUTO").Value
                                , decimal.Parse(row.GetCell("CD_FAIXA").Value ?? "1", _identity.GetFormatProvider())
                                , int.Parse(row.GetCell("CD_EMPRESA").Value)
                                , row.GetCell("NU_IDENTIFICADOR").Value
                                , int.Parse(row.GetCell("NU_ETIQUETA_LOTE").Value)
                                , row.GetCell("TP_ETIQUETA").Value
                                , row.GetCell("NU_EXTERNO_ETIQUETA").Value
                                , short.Parse(row.GetCell("CD_SITUACAO")?.Value)
                                , short.Parse(row.GetCell("CD_SITUACAO_DET_AGENDA")?.Value)
                                , decimal.Parse(row.GetCell("QT_PRODUTO_RECIBIDO").Value, this._identity.GetFormatProvider())
                                , decimal.Parse(row.GetCell("QT_PRODUTO").Value, this._identity.GetFormatProvider()))
                .AjustarEtiqueta();

            uow.SaveChanges();
        }
    }
}