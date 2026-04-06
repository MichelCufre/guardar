using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Configuracion;
using WIS.Domain.Impresiones;
using WIS.Exceptions;
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

namespace WIS.Application.Controllers.COF
{
    public class COF060ServidoresImpresion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public COF060ServidoresImpresion(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_SERVIDOR",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_SERVIDOR", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._formValidationService = formValidationService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnEditar", "COF060_grid1_btn_Editar", "fas fa-edit"),
                new GridButton("btnEliminarServidor", "COF060_Sec0_btn_EliminarServidor", "fas fa-trash", new ConfirmMessage(){
                    AcceptLabel = "COF060_grid1_btn_ConfirmarEliminarServidor",
                    CancelLabel = "COF060_grid1_btn_CancelarEliminarServidor",
                    Message = "COF060_grid1_msg_ConfirmarEliminarServidor",
                    AcceptVariant = ButtonVariant.Success,
                    CancelVariant = ButtonVariant.Danger
                 }),
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ServidoresImpresionQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ServidoresImpresionQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ServidoresImpresionQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            string codigoServidor = context.Row.GetCell("CD_SERVIDOR").Value;

            switch (context.ButtonId)
            {
                case "btnEliminarServidor":
                    this.EliminarServidor(context, codigoServidor);
                    break;
            }

            return context;
        }


        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.Parameters.Any(x => x.Id == "codigoServidor"))
            {
                var codigoServidor = context.Parameters.FirstOrDefault(s => s.Id == "codigoServidor").Value;

                var servidor = uow.ImpresionRepository.GetServidorImpresion(int.Parse(codigoServidor));

                form.GetField("dsServidor").Value = servidor.Descripcion;
                form.GetField("dominioServidor").Value = servidor.DominioServidor;
                form.GetField("clientId").Value = servidor.ClientId.ToString();
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ServidorImpresion servidor = new ServidorImpresion();

            ComponentParameter codigoServidor = context.Parameters.FirstOrDefault(s => s.Id == "codigoServidor");

            servidor.Descripcion = form.GetField("dsServidor").Value;
            servidor.DominioServidor = form.GetField("dominioServidor").Value;

            string clientId = form.GetField("clientId").Value;
            if (string.IsNullOrEmpty(clientId))
                throw new Exception("COF060_Sec0_error_usuarioNoValido");

            servidor.ClientId = clientId;

            if (codigoServidor != null)
            {
                servidor.Id = int.Parse(codigoServidor.Value);

                if (!uow.ImpresionRepository.ExisteServidorImpresion(servidor.Id))
                    throw new ValidationFailedException("COF060_Sec0_error_ServidorNoExiste");

                uow.ImpresionRepository.UpdateServidorImpresion(servidor);

                uow.SaveChanges();
                context.AddSuccessNotification("COF060_Sec0_Success_ServidorEditado");
            }
            else
            {
                uow.ImpresionRepository.AddServidorImpresion(servidor);

                uow.SaveChanges();
                context.AddSuccessNotification("COF060_Sec0_Success_ServidorAgregado");
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new MantenimientoServidoresImpresionValidationModule(uow), form, context);
        }

        #region Metodos Auxiliares

        public virtual void EliminarServidor(GridButtonActionContext context, string codigoServidor)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.ImpresionRepository.DeleteServidorImpresion(int.Parse(codigoServidor));

            uow.SaveChanges();
            context.AddSuccessNotification("COF060_grid1_Success_ServidorEliminado");
        }

        #endregion
    }
}
