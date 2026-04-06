using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.General.Enums;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Extension;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
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

namespace WIS.Application.Controllers.STO
{
    public class STO210PanelEnvases : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public STO210PanelEnvases(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IFormValidationService formValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "ID_ENVASE","ND_TP_ENVASE"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._formValidationService = formValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"),
                new GridButton("btnLog", "General_Sec0_btn_Log", "fas fa-list"),
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            SortCommand defaultSort = new SortCommand("DT_UDPROW", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new STO210Query();

            uow.HandleQuery(dbQuery, true);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new STO210Query();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            SortCommand defaultSort = new SortCommand("DT_UDPROW", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            STO210Query dbQuery = null;

            dbQuery = new STO210Query();

            uow.HandleQuery(dbQuery, true);

            context.FileName = $"{this._identity.Application}{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }
        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            if (context.ButtonId == "btnLog")
            {
                context.Redirect("/stock/STO211", new List<ComponentParameter>() {
                    new ComponentParameter("envase",context.Row.GetCell("ID_ENVASE").Value ),
                    new ComponentParameter("tipo", context.Row.GetCell("ND_TP_ENVASE").Value)
                });
            }

            return context;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var rowkey = query.GetParameter("ROW_KEY");
            FormField FieldID_ENVASE = form.GetField("ID_ENVASE");
            FieldID_ENVASE.ReadOnly = false;
            FormField FieldND_TP_ENVASE = form.GetField("ND_TP_ENVASE");
            FieldND_TP_ENVASE.ReadOnly = false;

            FormField FieldTP_AGENTE = form.GetField("TP_AGENTE");
            FormField FieldND_ESTADO_ENVASE = form.GetField("ND_ESTADO_ENVASE");

            FieldTP_AGENTE.Options = this.SelectTP_AGENTE(uow);
            FieldND_ESTADO_ENVASE.Options = this.SelectND_ESTADO_ENVASE(uow);
            FieldND_TP_ENVASE.Options = this.SelectND_TP_ENVASE(uow);

            if (!string.IsNullOrEmpty(rowkey))
            {

                string[] splitKey = rowkey.Split('$');

                Envase obj = uow.EnvaseRepository.GetEnvase(splitKey[0], splitKey[1]);

                if (obj == null) throw new EntityNotFoundException("STO210_Db_Error_EnvaseNoExiste");

                var mapper = new StockMapper();

                FieldID_ENVASE.ReadOnly = true;
                FieldID_ENVASE.Value = obj.Id;
                FieldND_TP_ENVASE.ReadOnly = true;
                FieldND_TP_ENVASE.Value = obj.TipoEnvase;
                form.GetField("CD_BARRAS").Value = obj.CodigoBarras;
                FieldND_ESTADO_ENVASE.Value = obj.Estado;
                form.GetField("DS_OBSERVACIONES").Value = obj.Observaciones;
                FieldTP_AGENTE.Value = obj.TipoAgente;

                FormField FieldCD_EMPRESA = form.GetField("CD_EMPRESA");
                FieldCD_EMPRESA.Value = obj.Empresa?.ToString() ?? uow.ParametroRepository.GetParameter("EMP_DEFAULT_ENVASE") ?? "1";
                FieldCD_EMPRESA.Options = this.SearchCD_EMPRESA(form, new FormSelectSearchContext { SearchValue = FieldCD_EMPRESA.Value.ToString() });

                FormField FieldCD_AGENTE = form.GetField("CD_AGENTE");
                FieldCD_AGENTE.Value = obj.CodigoAgente;
                FieldCD_AGENTE.Options = this.SearchCD_AGENTE(form, new FormSelectSearchContext { SearchValue = obj.CodigoAgente });


            }

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            FormField FieldID_ENVASE = form.GetField("ID_ENVASE");
            FormField FieldND_TP_ENVASE = form.GetField("ND_TP_ENVASE");

            bool IsUpdate = (FieldID_ENVASE.ReadOnly && FieldND_TP_ENVASE.ReadOnly);

            Envase obj = IsUpdate ? uow.EnvaseRepository.GetEnvase(FieldID_ENVASE.Value, FieldND_TP_ENVASE.Value) : new Envase();
            if (obj == null) throw new EntityNotFoundException("STO210_Db_Error_EnvaseNoExiste");

            var mapper = new StockMapper();

            obj.CodigoAgente = form.GetField("CD_AGENTE").Value;
            obj.Estado = form.GetField("ND_ESTADO_ENVASE").Value;
            obj.Observaciones = form.GetField("DS_OBSERVACIONES").Value;

            obj.TipoAgente = form.GetField("TP_AGENTE").Value;
            obj.Empresa = (obj.TipoAgente == TipoAgenteDb.Deposito) ? default(int?) : form.GetField("CD_EMPRESA").Value.ToNumber<int>();

            if (IsUpdate)
            {
                uow.CreateTransactionNumber("Edición de envase");
                obj.NumeroTransaccion = uow.GetTransactionNumber();
                uow.EnvaseRepository.UpdateEnvase(obj);
            }
            else
            {
                uow.CreateTransactionNumber("Creación de envase");

                obj.NumeroTransaccion = uow.GetTransactionNumber();
                obj.Id = FieldID_ENVASE.Value;
                obj.TipoEnvase = FieldND_TP_ENVASE.Value;

                uow.EnvaseRepository.AddEnvase(obj);
            }

            uow.SaveChanges();

            context.AddSuccessNotification(IsUpdate ? "General_Db_Success_Update" : "General_Db_Success_Insert");

            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new STO210FormValidationModule(uow, this._identity.GetFormatProvider()), form, context);
        }
        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "CD_EMPRESA": return this.SearchCD_EMPRESA(form, context);
                case "CD_AGENTE": return this.SearchCD_AGENTE(form, context);
                default: return new List<SelectOption>();
            }
        }

        public virtual List<SelectOption> SearchCD_EMPRESA(Form form, FormSelectSearchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue)
                       .Select(w => new SelectOption(w.Id.ToString(), w.Nombre))
                       .ToList();
        }
        public virtual List<SelectOption> SearchCD_AGENTE(Form form, FormSelectSearchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (int.TryParse(form.GetField("CD_EMPRESA").Value, out int emp))
            {
                return uow.AgenteRepository.GetAgenteByKeysPartial(context.SearchValue, form.GetField("TP_AGENTE").Value, emp)
                .Select(w => new SelectOption(w.Codigo.ToString(), w.Descripcion))
                .ToList();
            }
            else
            {
                context.AddErrorNotification("General_Sec0_Error_EmpresaNecesaria");
            }

            return new List<SelectOption>();
        }
        public virtual List<SelectOption> SelectND_ESTADO_ENVASE(UnitOfWorkCore uow)
        {
            return uow.EnvaseRepository.GetEstadosEnvases()
                    .Select(w => new SelectOption(w.Id, w.Descripcion))
                    .ToList();
        }
        public virtual List<SelectOption> SelectTP_AGENTE(UnitOfWorkCore uow)
        {
            return uow.AgenteRepository.GetTiposAgente()
                    .Select(w => new SelectOption(w.Valor, w.Descripcion))
                    .ToList();
        }
        public virtual List<SelectOption> SelectND_TP_ENVASE(UnitOfWorkCore uow)
        {
            return uow.EnvaseRepository.GetTiposEnvases()
                    .Select(w => new SelectOption(w.Valor, w.Descripcion))
                    .ToList();
        }
    }
}
