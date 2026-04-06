using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Impresion;
using WIS.Domain.Impresiones;
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
    public class COF040PanelImpresoras : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public COF040PanelImpresoras(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_IMPRESORA","NU_PREDIO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_IMPRESORA", SortDirection.Ascending),
                new SortCommand("NU_PREDIO", SortDirection.Ascending),
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
                new GridButton("btnEditar", "COF040_grid1_btn_Detalle", "fas fa-edit")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ImpresorasQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ImpresorasQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ImpresorasQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            this.InicializarSelectores(form, uow);

            if (context.Parameters.Any(x => x.Id == "idImpresora") && context.Parameters.Any(x => x.Id == "predio"))
            {
                var codigoImpresora = context.Parameters.FirstOrDefault(s => s.Id == "idImpresora").Value;
                var predio = context.Parameters.FirstOrDefault(s => s.Id == "predio").Value;

                var impre = uow.ImpresoraRepository.GetImpresora(codigoImpresora, predio);

                form.GetField("codigo").Value = impre.Id;
                form.GetField("codigo").ReadOnly = true;

                form.GetField("predio").Value = impre.Predio;
                form.GetField("predio").ReadOnly = true;

                form.GetField("descripcion").Value = impre.Descripcion;
                form.GetField("lenguaje").Value = impre.CodigoLenguajeImpresion;
                form.GetField("servidor").Value = impre.Servidor.ToString();
            }

            return form;

        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var codigoImpresora = form.GetField("codigo").Value;
            var predio = form.GetField("predio").Value;

            if (uow.ImpresoraRepository.ExisteImpresoraPredio(codigoImpresora, predio))
            {
                var impre = uow.ImpresoraRepository.GetImpresora(codigoImpresora, predio);

                impre.Descripcion = form.GetField("descripcion").Value;
                impre.CodigoLenguajeImpresion = form.GetField("lenguaje").Value;
                impre.Servidor = int.Parse(form.GetField("servidor").Value);

                uow.ImpresoraRepository.UpdateImpresora(impre);
                uow.SaveChanges();
                context.AddSuccessNotification("COF040_Sec0_error_ImpresoraActualizada");
            }
            else
            {
                var impresora = new Impresora()
                {
                    Id = codigoImpresora,
                    Predio = predio,
                    Descripcion = form.GetField("descripcion").Value,
                    CodigoLenguajeImpresion = form.GetField("lenguaje").Value,
                    Servidor = int.Parse(form.GetField("servidor").Value),
                };

                uow.ImpresoraRepository.AddImpresora(impresora);
                uow.SaveChanges();
                context.AddSuccessNotification("COF040_Sec0_error_ImpresoraAgregada");
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new MantenimientoImpresorasValidationModule(uow, this._identity.UserId, this._identity.Predio), form, context);
        }

        public virtual void InicializarSelectores(Form form, IUnitOfWork uow)
        {
            #region Predios

            var selectorPredio = form.GetField("predio");
            selectorPredio.Options = new List<SelectOption>();

            var prediosUsuario = uow.PredioRepository.GetPrediosUsuario(this._identity.UserId);

            foreach (var predio in prediosUsuario)
            {
                selectorPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}"));
            }

            if (!this._identity.Predio.Equals(GeneralDb.PredioSinDefinir))
                selectorPredio.Value = this._identity.Predio;

            #endregion

            #region Lenguajes

            var selectorLenguaje = form.GetField("lenguaje");
            selectorLenguaje.Options = new List<SelectOption>();

            var lenguajes = uow.ImpresionRepository.GetLenguajesImpresion();

            foreach (var lenguaje in lenguajes)
            {
                selectorLenguaje.Options.Add(new SelectOption(lenguaje.Id, $"{lenguaje.Id} - {lenguaje.Descripcion}"));
            }

            if (lenguajes != null && lenguajes.Count == 1)
                form.GetField("lenguaje").Value = lenguajes.First().Id;

            #endregion

            #region Servidores

            var selectorServidor = form.GetField("servidor");
            selectorServidor.Options = new List<SelectOption>();

            var servidores = uow.ImpresionRepository.GetServidoresImpresion();

            foreach (var servidor in servidores)
            {
                selectorServidor.Options.Add(new SelectOption(servidor.Id.ToString(), $"{servidor.Id} - {servidor.Descripcion}"));
            }

            if (servidores != null && servidores.Count == 1)
                form.GetField("servidor").Value = servidores.First().Id.ToString();

            #endregion
        }
    }
}