using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using WIS.Application.Security;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.Recorridos;
using WIS.Exceptions;
using WIS.Extension;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.PageComponent.Execution;
using WIS.Persistence.Database;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.REG
{
    public class REG702AplicacionesUsuarioRecorridos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG702AplicacionesUsuarioRecorridos(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            ISecurityService security,
            IFilterInterpreter filterInterpreter)
        {
            GridKeys =
            [
                "NU_RECORRIDO","CD_APLICACION","USERID"
            ];

            DefaultSort =
            [
                new SortCommand("NU_RECORRIDO", SortDirection.Descending),
                new SortCommand("CD_APLICACION", SortDirection.Descending),
                new SortCommand("USERID", SortDirection.Ascending)
            ];

            _uowFactory = uowFactory;
            _identity = identity;
            _gridService = gridService;
            _excelService = excelService;
            _security = security;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsCommitEnabled = false;
            query.IsAddEnabled = false;
            query.IsEditingEnabled = false;
            query.IsRemoveEnabled = false;

            var columnButton = new GridColumnButton("BTN_ARRAY_0",
            [
                new GridButton("btnPredeterminado", "REG702_Sec0_btn_Predeterminado", "fas fa-check",new ConfirmMessage("REG702_Sec0_msg_AsignarPredeterminado")),
            ]);

            grid.AddOrUpdateColumn(columnButton);

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new AplicacionesRecorridoUsuarioQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            DeshabilitarBotones(grid, uow);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var dbQuery = new AplicacionesRecorridoUsuarioQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{_identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return _excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new AplicacionesRecorridoUsuarioQuery();

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            if (context.ButtonId == "btnPredeterminado")
            {
                var numeroRecorrido = int.Parse(context.Row.GetCell("NU_RECORRIDO").Value);
                var aplicacion = context.Row.GetCell("CD_APLICACION").Value;
                var predio = context.Row.GetCell("NU_PREDIO").Value;
                var userId = int.Parse(context.Row.GetCell("USERID").Value);
                using var uow = this._uowFactory.GetUnitOfWork();
                uow.CreateTransactionNumber("AplicacionRecorrido: Predeterminado");
                uow.BeginTransaction();

                try
                {
                    var aplicacionRecorridoUsuarioPredeterminada = uow.RecorridoRepository.GetAplicacionUsuarioPredeterminado(aplicacion, predio, userId);
                    if (aplicacionRecorridoUsuarioPredeterminada != null)
                    {
                        aplicacionRecorridoUsuarioPredeterminada.NuTransaccion = uow.GetTransactionNumber();
                        aplicacionRecorridoUsuarioPredeterminada.FechaModificacion = DateTime.Now;
                        aplicacionRecorridoUsuarioPredeterminada.EsPredeterminado = false;
                        uow.RecorridoRepository.UpdateAplicacionRecorridoUsuario(aplicacionRecorridoUsuarioPredeterminada);
                    }

                    var aplicacionRecorridoUsuario = uow.RecorridoRepository.GetAplicacionUsuarioRecorrido(numeroRecorrido, aplicacion, userId);
                    aplicacionRecorridoUsuario.NuTransaccion = uow.GetTransactionNumber();
                    aplicacionRecorridoUsuario.FechaModificacion = DateTime.Now;
                    aplicacionRecorridoUsuario.EsPredeterminado = true;
                    uow.RecorridoRepository.UpdateAplicacionRecorridoUsuario(aplicacionRecorridoUsuario);

                    uow.SaveChanges();
                    uow.Commit();
                    context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
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
            }
            return context;
        }

        protected virtual void DeshabilitarBotones(Grid grid, IUnitOfWork uow)
        {
            foreach (var row in grid.Rows)
            {
                if (row.GetCell("FL_PREDETERMINADO").Value == "S") row.DisabledButtons.Add("btnPredeterminado");
            }
        }

        #region MetodosGenerales

        #endregion

    }
}