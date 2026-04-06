using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Documento;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Domain.Documento.Constants;
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

namespace WIS.Application.Controllers.DOC
{
    public class DOC100 : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public DOC100(IUnitOfWorkFactory uowFactory, IIdentityService identity, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter, IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "NU_DOCUMENTO_PREPARACION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW",SortDirection.Descending)
            };

            _uowFactory = uowFactory;
            _identity = identity;
            _gridService = gridService;
            _excelService = excelService;
            _filterInterpreter = filterInterpreter;
            _gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = true;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_ARRAY", new List<IGridItem>(){
                    new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"),
                    new GridButton("btnCerrar", "DOC100_Sec0_btn_CerrarAsociacion", "fa fa-times-circle", new ConfirmMessage("DOC100_Sec0_msg_CerrarConfirm")),
                    new GridButton("btnFinalizarOperacion", "DOC100_Sec0_btn_FinalizarOperacion", "fa fa-check-square", new ConfirmMessage("DOC100_Sec0_msg_FinalizarOperacionConfirm")),
                    new GridButton("btnReabrir", "DOC100_Sec0_btn_ReabrirAsociacion", "fa fa-share", new ConfirmMessage("DOC100_Sec0_msg_ReabrirConfirm")),
                }));

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var query = new DOC100Query();

            uow.HandleQuery(query);

            grid.Rows = _gridService.GetRows(query, grid.Columns, context, this.DefaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                var activa = row.GetCell("FL_ACTIVE").Value == "S" ? true : false;
                var prep = int.Parse(row.GetCell("NU_PREPARACION").Value);

                var permiteReabrirIngreso = row.GetCell("FL_REABRIR_I").Value == "S" ? true : false;
                var permiteReabrirEgreso = row.GetCell("FL_REABRIR_E").Value == "S" ? true : false;

                var permiteFinalizarIngreso = row.GetCell("FL_FINOP_I").Value == "S" ? true : false;
                var permiteFinalizarEgreso = row.GetCell("FL_FINOP_E").Value == "S" ? true : false;

                var estadoInicialIngreso = row.GetCell("ESTADO_INICIAL_I").Value == "S" ? true : false;
                var estadoInicialEgreso = row.GetCell("ESTADO_INICIAL_E").Value == "S" ? true : false;

                var tpOperativa = row.GetCell("TP_OPERATIVA").Value;

                if (!activa)
                    row.DisabledButtons.Add("btnCerrar");

                if (!activa || !permiteFinalizarIngreso || !permiteFinalizarEgreso)
                    row.DisabledButtons.Add("btnFinalizarOperacion");

                if (activa || !permiteReabrirIngreso || !permiteReabrirEgreso || uow.DocumentoRepository.ExisteDocumentoPreparacionActivo(prep, tpOperativa))
                    row.DisabledButtons.Add("btnReabrir");

                if (!activa || !estadoInicialIngreso || !estadoInicialEgreso)
                    row.DisabledButtons.Add("btnEditar");
            }
            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber(this._identity.Application);
            uow.BeginTransaction();

            try
            {
                switch (context.ButtonId)
                {
                    case "btnCerrar":
                        EjecutarProcedimiento(uow, context, true);
                        break;
                    case "btnFinalizarOperacion":
                        EjecutarProcedimiento(uow, context, true, true);
                        break;
                    case "btnReabrir":
                        EjecutarProcedimiento(uow, context, false);
                        break;
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("DOC100_Sec0_msg_Success");
            }
            catch (ValidationFailedException ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message, new List<string>(ex.StrArguments ?? new string[0]));
                uow.Rollback();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }

            return context;
        }
        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                if (grid.HasNewDuplicates(this.GridKeys))
                    throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                foreach (var row in grid.Rows)
                {
                    if (row.IsDeleted)
                    {
                        EliminarDocumentoPreparacion(uow, row);
                    }
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("DOC100_Sec0_msg_Success");
            }
            catch (ValidationFailedException ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message, new List<string>(ex.StrArguments ?? new string[0]));
                uow.Rollback();
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }

            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return this._gridValidationService.Validate(new DOC100GridValidationModule(uow), grid, row, context);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var query = new DOC100Query();
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

            var dbQuery = new DOC100Query();

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }


        public virtual void EjecutarProcedimiento(IUnitOfWork uow, GridButtonActionContext context, bool cerrar, bool finalizaroperacion = false)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var nuDocPrep = int.Parse(context.Row.GetCell("NU_DOCUMENTO_PREPARACION").Value);
            var docPrep = uow.DocumentoRepository.GetDocumentoPreparacion(nuDocPrep);

            if (finalizaroperacion)
            {
                var nuDocIngreso = context.Row.GetCell("NU_DOCUMENTO_INGRESO").Value;
                var tpDocIngreso = context.Row.GetCell("TP_DOCUMENTO_INGRESO").Value;

                var nuDocEgreso = context.Row.GetCell("NU_DOCUMENTO_EGRESO").Value;
                var tpDocEgreso = context.Row.GetCell("TP_DOCUMENTO_EGRESO").Value;

                if (!uow.DocumentoRepository.ExistenDocumentosEnOtraAsociacion(nuDocPrep, nuDocIngreso, tpDocIngreso, nuDocEgreso, tpDocEgreso))
                {
                    var docIngreso = uow.DocumentoRepository.GetIngreso(nuDocIngreso, tpDocIngreso);
                    var docEgreso = uow.DocumentoRepository.GetEgreso(nuDocEgreso, tpDocEgreso);

                    docIngreso.Estado = uow.DocumentoRepository.GetEstadoDestino(tpDocIngreso, AccionDocumento.FinalizarOperacion);
                    docEgreso.Estado = uow.DocumentoRepository.GetEstadoDestino(tpDocEgreso, AccionDocumento.FinalizarOperacion);

                    uow.DocumentoRepository.UpdateIngreso(docIngreso, nuTransaccion);
                    uow.DocumentoRepository.UpdateEgreso(docEgreso, nuTransaccion);
                }
            }

            if (cerrar)
                docPrep.Disable();
            else
                docPrep.Enable();

            uow.DocumentoRepository.UpdateDocumentoPreparacion(docPrep);
        }
        public virtual void EliminarDocumentoPreparacion(IUnitOfWork uow, GridRow row)
        {
            int nroDocPrep = int.Parse(row.GetCell("NU_DOCUMENTO_PREPARACION").Value);
            var estadoInicialIngreso = row.GetCell("ESTADO_INICIAL_I").Value == "S" ? true : false;
            var estadoInicialEgreso = row.GetCell("ESTADO_INICIAL_E").Value == "S" ? true : false;
            var docPrep = uow.DocumentoRepository.GetDocumentoPreparacion(nroDocPrep);

            if (docPrep == null)
                throw new ValidationFailedException("DOC100_Sec0_Error_DocPrepNoEncontrado", new string[] { nroDocPrep.ToString() });

            if (!estadoInicialIngreso || !estadoInicialEgreso)
                throw new ValidationFailedException("DOC100_Sec0_Error_NoSePuedeEliminar");

            //if (uow.DocumentoRepository.AnyContenedorEmsablado(docPrep.Preparacion))
            //  throw new ValidationFailedException("DOC100_Sec0_Error_PrepConContendoresEnsamblados", new string[] { docPrep.Preparacion.ToString() });

            uow.DocumentoRepository.DeleteDocumentoPreparacion(docPrep);
        }
    }
}
