using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Porteria;
using WIS.Domain.Porteria;
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

namespace WIS.Application.Controllers.POR
{
    public class POR030VehiculosDeposito : AppController
    {
        protected readonly IIdentityService _security;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService service;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public POR030VehiculosDeposito(
            IIdentityService security,
            IUnitOfWorkFactory uowFactory,
            IGridService service,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_PORTERIA_VEHICULO"
            };

            this._security = security;
            this._uowFactory = uowFactory;
            this.service = service;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_PORTERIA_VEHICULO", SortDirection.Descending);

            FiltrosDispositivosVehiculos filtros = JsonConvert.DeserializeObject<FiltrosDispositivosVehiculos>(query.GetParameter("FILTROS"));

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaVehiculosDepositoQuery(filtros);

            uow.HandleQuery(dbQuery, true);

            grid.Rows = service.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridKeys);

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            FiltrosDispositivosVehiculos filtros = JsonConvert.DeserializeObject<FiltrosDispositivosVehiculos>(query.GetParameter("FILTROS"));
            var dbQuery = new PorteriaVehiculosDepositoQuery(filtros);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_PORTERIA_VEHICULO", SortDirection.Descending);

            FiltrosDispositivosVehiculos filtros = JsonConvert.DeserializeObject<FiltrosDispositivosVehiculos>(query.GetParameter("FILTROS"));

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaVehiculosDepositoQuery(filtros);

            uow.HandleQuery(dbQuery, true);

            query.FileName = $"{this._security.Application}{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, defaultSort);
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            //form.GetField("TP_REGISTRO").Options = this.SelectTipoRegistro(uow);
            form.GetField("CD_SECTOR").Options = this.SelectSector(uow, _security.Predio == GeneralDb.PredioSinDefinir ? "" : _security.Predio);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext query)
        {

            return form;
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext query)
        {
            switch (query.FieldId)
            {
                case "NU_VEHICULO": return this.SearchVehiculo(query.SearchValue);
                case "CD_EMPRESA": return this.SearchEmpresa(query.SearchValue);
            }

            return new List<SelectOption>();
        }


        public virtual List<SelectOption> SearchVehiculo(string searchValue)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.PorteriaRepository.GetRegistroVehiculoByKeysPartial(searchValue)
                    .Select(w => new SelectOption(w.NU_PORTERIA_VEHICULO.ToString(), w.VL_MATRICULA_1))
                    .ToList();
        }

        public virtual List<SelectOption> SearchContacto(string searchValue)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.PorteriaRepository.GetPersonaByKeysPartial(searchValue)
                    .Select(w => new SelectOption(w.NU_POTERIA_PERSONA.ToString(), w.NM_PERSONA + " " + w.AP_PERSONA))
                    .ToList();
        }

        public virtual List<SelectOption> SearchEmpresa(string searchValue)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.EmpresaRepository.GetByNombreOrCodePartial(searchValue)
                    .Select(w => new SelectOption(w.Id.ToString(), w.Nombre))
                    .ToList();
        }

        public virtual List<SelectOption> SelectTipoRegistro(IUnitOfWork uow)
        {
            return new List<SelectOption> {
                new SelectOption("TOD","WPOR030_frm1_lbl_TIPO_REGISTRO_TODOS"),
                new SelectOption("ENT","WPOR030_frm1_lbl_TIPO_REGISTRO_DENTRODEPO"),
                new SelectOption("SAL","WPOR030_frm1_lbl_TIPO_REGISTRO_FUERADEPO"),
                new SelectOption("TRA","WPOR030_frm1_lbl_TIPO_REGISTRO_TRANSITO"),
            };
        }
        public virtual List<SelectOption> SelectSector(IUnitOfWork uow, string predio)
        {
            return uow.PorteriaRepository.GetSectoresByPredio(predio)
                   .Select(w => new SelectOption(w.CD_SECTOR, w.DS_SECTOR))
                   .ToList();
        }
    }
}
