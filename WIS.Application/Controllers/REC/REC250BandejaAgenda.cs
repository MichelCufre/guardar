using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Recepcion;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;
using static WIS.Domain.DataModel.Queries.Recepcion.ResumenesAgendaQuery;

namespace WIS.Application.Controllers.REC
{
    public class REC250BandejaAgenda : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeysAgenda { get; }

        public REC250BandejaAgenda(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory, 
            IGridService gridService, 
            IGridExcelService excelService, 
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeysAgenda = new List<string>
            {
                "NU_AGENDA"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        
        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem> {
                    new GridButton("btnGuardar", "General_Sec0_btn_Guardar"),
                };
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            FiltrosAgendaResumen filtros = JsonConvert.DeserializeObject<FiltrosAgendaResumen>(context.GetParameter("FILTROS"));
            
            if (string.IsNullOrEmpty(filtros.USERID))
            {
                filtros.USERID = _identity.UserId.ToString();
            }

            var dbQuery = new ResumenesAgendaQuery(filtros);

            uow.HandleQuery(dbQuery);

            SortCommand defaultSort = new SortCommand("NU_AGENDA", SortDirection.Descending);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeysAgenda);

            var selection = new List<string>();

            grid.Rows.ForEach(w =>
            {
                string nuColor = w.GetCell("NU_COLOR").Value;
                if (!string.IsNullOrEmpty(w.GetCell("DT_LIBERACIO").Value))
                    w.GetCell("DT_LIBERACIO").Value = w.GetCell("DT_LIBERACIO").Value.Replace('+', ' ').Replace('-', ' ').Substring(6, 13);
                if (!string.IsNullOrEmpty(w.GetCell("DT_INI_RECEPCION").Value))
                    w.GetCell("DT_INI_RECEPCION").Value = w.GetCell("DT_INI_RECEPCION").Value.Replace('+', ' ').Replace('-', ' ').Substring(6, 13);
                if (!string.IsNullOrEmpty(w.GetCell("DT_FIN_RECEPCION").Value))
                    w.GetCell("DT_FIN_RECEPCION").Value = w.GetCell("DT_FIN_RECEPCION").Value.Replace('+', ' ').Replace('-', ' ').Substring(6, 13);
                if (w.GetCell("CD_FUN_RESP").Value != "" && nuColor == "5")
                {
                    selection.Add(w.Id);
                }
                switch (w.GetCell("NU_COLOR").Value)
                {
                    //case "SINVEDFINOK":
                    //    w.CssClass = "orange";
                    //    break;
                    //case "SINVEDCON":
                    //    w.CssClass = "gray";
                    //    break;
                    case "1":
                        w.CssClass = "redBlack";

                        break;
                    case "2":
                        w.CssClass = "orange";
                        w.DisabledSelected = true;
                        break;
                    case "3":
                        w.CssClass = "yellow";
                        w.DisabledSelected = true;
                        break;
                    case "4":
                        w.CssClass = "green";
                        w.DisabledSelected = true;
                        break;
                    case "5":
                        w.CssClass = "red";
                        break;
                    default:
                        w.DisabledSelected = true;
                        break;
                }

            });

            context.AddParameter("preSeleccion", JsonConvert.SerializeObject(selection));

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            FiltrosAgendaResumen filtros = JsonConvert.DeserializeObject<FiltrosAgendaResumen>(context.GetParameter("FILTROS"));

            if (string.IsNullOrEmpty(filtros.USERID))
            {
                filtros.USERID = _identity.UserId.ToString();
            }

            var dbQuery = new ResumenesAgendaQuery(filtros);

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

            SortCommand defaultSort = new SortCommand("NU_AGENDA", SortDirection.Descending);

            FiltrosAgendaResumen filtros = JsonConvert.DeserializeObject<FiltrosAgendaResumen>(context.GetParameter("FILTROS"));

            if (string.IsNullOrEmpty(filtros.USERID))
            {
                filtros.USERID = _identity.UserId.ToString();
            }

            var dbQuery = new ResumenesAgendaQuery(filtros);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber($"{this._identity.Application} - Bandeja Agenda");

            FiltrosAgendaResumen filtros = JsonConvert.DeserializeObject<FiltrosAgendaResumen>(context.GetParameter("FILTROS"));

            var queryDb = new ResumenesAgendaQuery(filtros);

            uow.HandleQuery(queryDb);

            queryDb.ApplyFilter(this._filterInterpreter, context.Filters);

            List<string[]> keysRowSelected = this.GetSelectedPlanificaciones(uow, filtros, context);

            List<Agenda> oldAgenda = queryDb.GetAgendas();

            oldAgenda.ForEach(r =>
            {

                if (keysRowSelected.Any(a => r.Id == int.Parse(a[0])))
                {

                    if (r.FuncionarioResponsable == null)
                    {
                        r.FuncionarioResponsable = _identity.UserId;
                        r.FechaFuncionarioResponsable = DateTime.Now;
                        r.NumeroTransaccion  = uow.GetTransactionNumber();
                        uow.AgendaRepository.UpdateAgendaFuncionarioResponsable(r);
                    }

                }
                else
                {
                    if (r.FuncionarioResponsable != null)
                    {
                        r.FuncionarioResponsable = null;
                        r.FechaFuncionarioResponsable = null;
                        r.NumeroTransaccion = uow.GetTransactionNumber();
                        uow.AgendaRepository.UpdateAgendaFuncionarioResponsable(r);
                    }
                }

            });

            uow.SaveChanges();

            return context;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {

            var FieldUSERID = form.GetField("USERID");
            FieldUSERID.Options = SearchUsuario(_identity.UserId.ToString());
            FieldUSERID.Value = _identity.UserId.ToString();

            using var uow = this._uowFactory.GetUnitOfWork();

            form.GetField("NU_PREDIO").Options = this.SelectPredio(uow);
            form.GetField("FL_CERRADO").Options = this.SelectCerrado(uow);

            return form;
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "USERID": return this.SearchUsuario(context.SearchValue);
                case "CD_EMPRESA": return this.SearchEmpresa(context.SearchValue, form.GetField("USERID").Value);

                default:
                    return new List<SelectOption>();
            }
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            FiltrosAgendaResumen filtros = JsonConvert.DeserializeObject<FiltrosAgendaResumen>(context.GetParameter("FILTROS"));

            if (string.IsNullOrEmpty(filtros.USERID))
            {
                filtros.USERID = _identity.UserId.ToString();
            }

            var dbQuery = new ResumenesAgendaQuery(filtros);

            using var uow = this._uowFactory.GetUnitOfWork();

            uow.HandleQuery(dbQuery, false, false, false);

            List<CantidadResumenAgenda> objetoConCantidades = dbQuery.GetCantidad(uow);

            form.GetField("QT_ABIERTAS").Value = objetoConCantidades.FirstOrDefault(w => w.NU_COLOR == 1)?.CANTIDAD.ToString();
            form.GetField("QT_LIBERADAS").Value = objetoConCantidades.FirstOrDefault(w => w.NU_COLOR == 2)?.CANTIDAD.ToString();
            form.GetField("QT_RECEPCION").Value = objetoConCantidades.FirstOrDefault(w => w.NU_COLOR == 3)?.CANTIDAD.ToString();
            form.GetField("QT_RECIBIDAS").Value = objetoConCantidades.FirstOrDefault(w => w.NU_COLOR == 4)?.CANTIDAD.ToString();
            form.GetField("QT_CON_RESPON").Value = objetoConCantidades.FirstOrDefault(w => w.NU_COLOR == 5)?.CANTIDAD.ToString();
            uow.HandleQuery(dbQuery);

            return form;
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SearchUsuario(string searchValue)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.SecurityRepository.GetUsuariosByNombreOrCodePartial(searchValue)
                       .Select(w => new SelectOption(w.UserId.ToString(), w.Username))
                       .ToList();
        }

        public virtual List<SelectOption> SearchEmpresa(string searchValue, string userId)
        {
            int user = string.IsNullOrEmpty(userId) ? _identity.UserId : int.Parse(userId);

            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.EmpresaRepository.GetByNombreOrCodePartialForUsuario(searchValue, user)
                       .Select(w => new SelectOption(w.Id.ToString(), w.Nombre))
                       .ToList();
        }

        public virtual List<SelectOption> SelectPredio(IUnitOfWork uow)
        {
            return uow.PredioRepository.GetPredios()
                  .Select(w => new SelectOption(w.Numero, w.Descripcion))
                  .ToList();
        }

        public virtual List<SelectOption> SelectCerrado(IUnitOfWork uow)
        {
            return new List<SelectOption>
            {
                new SelectOption("S", "SI"),
                new SelectOption("N", "NO"),
            };
        }

        public virtual List<string[]> GetSelectedPlanificaciones(IUnitOfWork uow, FiltrosAgendaResumen filtros, GridMenuItemActionContext context)
        {
            var dbQuery = new ResumenesAgendaQuery(filtros);

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys);

            return dbQuery.GetSelectedKeys(context.Selection.Keys);
        }

        #endregion
    }
}
