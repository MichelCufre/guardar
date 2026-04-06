using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.Enums;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.REC
{
    public class REC220LiberacionCrossDocking : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IIdentityService _identity;
        protected readonly IGridExcelService _excelService;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> GridSorts { get; }

        public REC220LiberacionCrossDocking(IUnitOfWorkFactory uowFactory, IGridService gridService, IIdentityService identity, IGridExcelService excelService, ISecurityService security, IFormValidationService formValidationService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_CARGA"
            };

            this.GridSorts = new List<SortCommand>
            {
            new SortCommand("NU_CARGA", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._identity = identity;
            this._excelService = excelService;
            this._security = security;
            this._formValidationService = formValidationService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = this.GetDbQuery(uow, context);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.GridSorts, this.GridKeys);
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = this.GetDbQuery(uow, context);

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.GridSorts);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = this.GetDbQuery(uow, context);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }


        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new REC220FormValidationModule(uow), form, context);
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            FormField fieldEmmpresa = form.GetField("CD_EMPRESA");
            if (int.TryParse(fieldEmmpresa.Value, out int empresa))
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                uow.CreateTransactionNumber($"REC220 - Liberación Cross-Docking");

                uow.BeginTransaction();

                var cargasMostrar = new List<Carga>();
                var agendas = uow.AgendaRepository.GetAgendasByEmpresa(empresa);

                foreach (int ag in agendas)
                {
                    var agenda = uow.AgendaRepository.GetAgenda(ag);
                    var crossDocking = CrossDockingLogic.GetOrCreateCrossDocking(uow, agenda, _identity.UserId, TipoCrossDockingDb.UnaFase);

                    crossDocking.Liberar(uow, agenda, cargasMostrar);
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Db_Success_Update");

                form.GetField("listCargas").Value = JsonConvert.SerializeObject(cargasMostrar.Select(s => s.Id).ToList());
            }

            return form;
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "CD_EMPRESA": return this.SearchEmpresa(context.SearchValue, null);

                default:
                    return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares

        public virtual CargaREC220Query GetDbQuery(IUnitOfWork uow, ComponentContext context)
        {
            List<long> listaCargas = new List<long>();

            if (context.Parameters.Any(w => w.Id == "listCargas"))
            {
                string listaCargasString = context.GetParameter("listCargas");

                if (!string.IsNullOrEmpty(listaCargasString))
                    listaCargas = JsonConvert.DeserializeObject<List<long>>(listaCargasString);
            }

            return new CargaREC220Query(listaCargas);
        }

        public virtual List<SelectOption> SearchEmpresa(string searchValue, string userId)
        {
            int user = string.IsNullOrEmpty(userId) ? _identity.UserId : int.Parse(userId);

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return uow.EmpresaRepository.GetByNombreOrCodePartial(searchValue)
                    .Select(w => new SelectOption(w.Id.ToString(), w.Nombre))
                    .ToList();
            }
        }
        #endregion
    }
}
