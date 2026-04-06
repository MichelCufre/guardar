using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Security;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Items;
using WIS.GridComponent.Build.Configuration;
using WIS.Domain.DataModel;
using WIS.Sorting;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Excel;
using WIS.Exceptions;
using WIS.Components.Common;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.PageComponent.Execution;
using WIS.Domain.Recepcion.Enums;
using WIS.Domain.Recepcion;
using WIS.Filtering;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Application.Controllers.REC
{
    public class REC010ReferenciasRecepcion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public REC010ReferenciasRecepcion(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_RECEPCION_REFERENCIA"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override PageContext PageLoad(PageContext data)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (uow.ReferenciaRecepcionRepository.GetReferenciaConfiguracion().PermiteDigitacion)
            {
                data.AddParameter("PermiteDigitacion", "true");
            }

            return data;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {

            query.IsAddEnabled = false;
            query.IsEditingEnabled = false;
            query.IsRemoveEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnDetalle", "General_Sec0_btn_Detalles", "fas fa-list"),
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"),
                new GridButton("btnCancelar", "REC010_grid1_btn_Cancelar", "fas fa-ban", new ConfirmMessage("Seguro que desea cancelar la referencia?")),
                new GridButton("btnAgenda", "REC010_grid1_btn_IrAgendas", "fas fa-calendar-week"),
            }));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ReferenciaRecepcionQuery dbQuery;

            if (query.Parameters.Count > 0)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "agenda").Value, out int numeroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ReferenciaRecepcionQuery(numeroAgenda);
            }
            else
            {
                dbQuery = new ReferenciaRecepcionQuery();
            }

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("NU_RECEPCION_REFERENCIA", SortDirection.Descending);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridKeys);

            bool permiteEditar = uow.ReferenciaRecepcionRepository.GetReferenciaConfiguracion().PermiteDigitacion;

            if (!permiteEditar)
            {
                foreach (var row in grid.Rows)
                {
                    row.DisabledButtons = new List<string>() { "btnEditar" };
                }
            }
            else
            {
                foreach (var row in grid.Rows)
                {
                    var estadoReferencia = uow.ReferenciaRecepcionRepository.GetEstadoReferencia(int.Parse(row.GetCell("NU_RECEPCION_REFERENCIA").Value));

                    #region btnEditar

                    if (estadoReferencia != EstadoReferenciaRecepcionDb.Abierta)
                        row.DisabledButtons.Add("btnEditar");
                    #endregion

                    #region btnAgenda
                    if (!uow.ReferenciaRecepcionRepository.AnyReferenciaAsociada(int.Parse(row.GetCell("NU_RECEPCION_REFERENCIA").Value)))
                        row.DisabledButtons.Add("btnAgenda");
                    #endregion

                    #region btnCancelar
                    if (estadoReferencia == EstadoReferenciaRecepcionDb.Cancelada || estadoReferencia == EstadoReferenciaRecepcionDb.Finalizada ||
                        !uow.ReferenciaRecepcionRepository.PuedeCancelarReferencia(int.Parse(row.GetCell("NU_RECEPCION_REFERENCIA").Value)))
                        row.DisabledButtons.Add("btnCancelar");
                    #endregion
                }
            }

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ReferenciaRecepcionQuery dbQuery;

            if (query.Parameters.Count > 0)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "agenda").Value, out int numeroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ReferenciaRecepcionQuery(numeroAgenda);
            }
            else
            {
                dbQuery = new ReferenciaRecepcionQuery();
            }

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("DT_UPDROW", SortDirection.Descending);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, defaultSort);
        }
        public override GridButtonActionContext GridButtonAction(GridButtonActionContext data)
        {

            if (data.ButtonId == "btnDetalle")
            {
                data.Redirect("/recepcion/REC011", new List<ComponentParameter>() {
                      new ComponentParameter(){ Id = "referencia", Value = data.Row.GetCell("NU_RECEPCION_REFERENCIA").Value},
                });
            }

            if (data.ButtonId == "btnAgenda")
            {
                data.Redirect("/recepcion/REC170", new List<ComponentParameter>() {
                      new ComponentParameter(){ Id = "referencia", Value = data.Row.GetCell("NU_RECEPCION_REFERENCIA").Value},
                });
            }

            if (data.ButtonId == "btnCancelar")
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                uow.CreateTransactionNumber("Cancelar");

                AnularReferencia(uow, data.Row);

                uow.SaveChanges();

                data.AddSuccessNotification("REC010_grid1_Succes_Cancelar", new List<string> { data.Row.GetCell("NU_REFERENCIA").Value });

            }

            return data;
        }
        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Rows.Any())
            {

                if (grid.HasNewDuplicates(this.GridKeys))
                    throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                foreach (var row in grid.Rows)
                {

                    if (row.IsNew)
                    {
                    }
                    else if (row.IsDeleted)
                    {
                        // rows delete

                    }
                    else
                    {
                        // rows editadas

                    }
                }

            }

            uow.SaveChanges();

            query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");

            return grid;

        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ReferenciaRecepcionQuery dbQuery;

            if (query.Parameters.Count > 0)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "agenda").Value, out int numeroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ReferenciaRecepcionQuery(numeroAgenda);
            }
            else
            {
                dbQuery = new ReferenciaRecepcionQuery();
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual void AnularReferencia(IUnitOfWork uow, GridRow row)
        {
            if (!uow.ReferenciaRecepcionRepository.GetReferenciaConfiguracion().PermiteDigitacion)
                throw new ValidationFailedException("REC010_frm1_error_DigitacionLineasNoPermitido");

            var referencia = uow.ReferenciaRecepcionRepository.GetReferenciaConDetalle(int.Parse(row.GetCell("NU_RECEPCION_REFERENCIA").Value));

            if (referencia == null)
                throw new ValidationFailedException("REC010_Frm1_Error_ReferenciaNoExiste");

            // Verifico que el estado sea abierta
            if (referencia.Estado == EstadoReferenciaRecepcionDb.Abierta)
            {
                // Se excluyen referencias sobrantes
                var detallesAAnular = referencia.Detalles.Where(s => s.CantidadReferencia > 0);
                var nuTransaccion = uow.GetTransactionNumber();

                foreach (var detalle in detallesAAnular)
                {
                    detalle.CantidadAnulada = (detalle.CantidadReferencia ?? 0) - (detalle.CantidadRecibida ?? 0);
                    detalle.NumeroTransaccion = nuTransaccion;
                    uow.ReferenciaRecepcionRepository.UpdateReferenciaDetalle(detalle);
                }

                referencia.Estado = EstadoReferenciaRecepcionDb.Cancelada;
                referencia.NumeroTransaccion = nuTransaccion;

                uow.ReferenciaRecepcionRepository.UpdateReferencia(referencia);
            }
        }
    }
}