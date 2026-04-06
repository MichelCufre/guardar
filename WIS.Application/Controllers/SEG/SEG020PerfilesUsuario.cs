using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
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
using WIS.Domain.DataModel.Queries.Seguridad;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Domain.Security;
using WIS.Domain.General.Enums;
using WIS.Domain.DataModel.Mappers;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Application.Validation;
using WIS.Filtering;

namespace WIS.Application.Controllers.SEG
{
    public class SEG020PerfilesUsuario : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly SecurityMapper _mapper;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public SEG020PerfilesUsuario(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IFormValidationService formValidationService,
            IGridService gridService,
            IGridExcelService excelService, 
            IFilterInterpreter filterInterpreter)
        {
            this._mapper = new SecurityMapper();

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("PROFILEID",SortDirection.Ascending)
            };

            this.GridKeys = new List<string>
            {
                "PROFILEID"
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
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "fas fa-edit"),
                new GridButton("btnAsignarPerfiles", "SEC020_Sec0_btn_Asignar", "fas fa-plus-square"),

            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PerfilesSistemaQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PerfilesSistemaQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
        
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PerfilesSistemaQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            return context;
        }


        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!string.IsNullOrEmpty(form.GetField("idPerfil").Value))
            {
                int idPerfilUpdate = int.Parse(form.GetField("idPerfil").Value);

                Perfil perfilUpdate = uow.SecurityRepository.GetPerfil(idPerfilUpdate);

                perfilUpdate.Descripcion = form.GetField("descripcion").Value;

                uow.SecurityRepository.UpdatePerfil(perfilUpdate);

                context.AddParameter("idPerfil", idPerfilUpdate.ToString());

                uow.SaveChanges();

                context.AddSuccessNotification("SEG020_Sec0_error_PerfilUpdated", new List<string>() { perfilUpdate.Descripcion });
            }
            else
            {

                string description = form.GetField("descripcion").Value;

                int idPerfil = uow.SecurityRepository.AddPerfil(new Perfil { Descripcion = description, Tipo = this._mapper.MapToTipoPerfilDb(TipoPerfil.Interno) });

                context.AddParameter("idPerfil", idPerfil.ToString());
                uow.SaveChanges();

                context.AddSuccessNotification("SEG020_Sec0_error_PerfilAgregado", new List<string>() { description });
            }

            return form;
        }
        
        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.Parameters.Any(x => x.Id == "idPerfil"))
            {
                int idPerfil = int.Parse(context.Parameters.FirstOrDefault(x => x.Id == "idPerfil").Value);
                Perfil perfil = uow.SecurityRepository.GetPerfil(idPerfil);

                form.GetField("idPerfil").Value = perfil.Id.ToString();
                form.GetField("descripcion").Value = perfil.Descripcion;
            }

            return form;
        }
        
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new CreatePerfilValidationModule(), form, context);
        }
    }
}
