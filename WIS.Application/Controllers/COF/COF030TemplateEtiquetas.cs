using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.Configuracion;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Configuracion;
using WIS.Domain.General.Configuracion;
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
    public class COF030TemplateEtiquetas : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public COF030TemplateEtiquetas(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_LABEL_ESTILO", "CD_LENGUAJE_IMPRESION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_LABEL_ESTILO", SortDirection.Ascending),
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
                new GridButton("btnEditar", "COF030_grid1_btn_Editar", "fas fa-edit"),
                new GridButton("btnEliminarTemplate", "COF030_Sec0_btn_EliminarTemplate", "fas fa-trash", new ConfirmMessage(){
                    AcceptLabel = "COF030_grid1_btn_ConfirmarEliminarTemplate",
                    CancelLabel = "COF030_grid1_btn_CancelarEliminarTemplate",
                    Message = "COF030_grid1_msg_ConfirmarEliminarTemplate",
                    AcceptVariant = ButtonVariant.Success,
                    CancelVariant = ButtonVariant.Danger
                 }),
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new TemplateEtiquetasQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new TemplateEtiquetasQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new TemplateEtiquetasQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            var estilo = context.Row.GetCell("CD_LABEL_ESTILO").Value;
            var lenguaje = context.Row.GetCell("CD_LENGUAJE_IMPRESION").Value;

            switch (context.ButtonId)
            {
                case "btnEliminarTemplate":
                    this.EliminarTemplate(context, estilo, lenguaje);
                    break;
            }

            return context;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            this.InicializarSelectores(form, uow);

            if (context.Parameters.Any(x => x.Id == "idTemplate"))
            {
                string idTemplate = context.Parameters.FirstOrDefault(s => s.Id == "idTemplate").Value;
                string lenguaje = context.Parameters.FirstOrDefault(s => s.Id == "lenguaje").Value;

                var templateEtiqueta = uow.TemplateEtiquetaRepository.GetEtiquetaEstilo(idTemplate, lenguaje);

                form.GetField("estilo").Value = templateEtiqueta.estilo;
                form.GetField("estilo").ReadOnly = true;

                form.GetField("lenguaje").Value = templateEtiqueta.lenguaje;
                form.GetField("lenguaje").ReadOnly = true;

                form.GetField("cuerpo").Value = templateEtiqueta.cuerpo;
                form.GetField("altura").Value = templateEtiqueta.altura.ToString();
                form.GetField("largura").Value = templateEtiqueta.largura.ToString();
            }

            return form;

        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            TemplateEtiqueta etiqueta = new TemplateEtiqueta();

            etiqueta.estilo = form.GetField("estilo").Value;
            etiqueta.lenguaje = form.GetField("lenguaje").Value;

            etiqueta.cuerpo = form.GetField("cuerpo").Value;

            string altura = form.GetField("altura").Value;
            if (!string.IsNullOrEmpty(altura))
                etiqueta.altura = decimal.Parse(altura, this._identity.GetFormatProvider());

            string largura = form.GetField("largura").Value;
            if (!string.IsNullOrEmpty(largura))
                etiqueta.largura = decimal.Parse(largura, this._identity.GetFormatProvider());

            if (uow.TemplateEtiquetaRepository.ExisteTemplateEtiqueta(etiqueta.estilo, etiqueta.lenguaje))
            {
                //Si estos campos no estan readonly quiere decir que no es un update
                if (form.GetField("estilo").ReadOnly == false && form.GetField("lenguaje").ReadOnly == false)
                    throw new ValidationFailedException("COF030_Sec0_error_TemplateExiste");

                uow.TemplateEtiquetaRepository.UpdateTemplateEtiqueta(etiqueta);

                uow.SaveChanges();
                context.AddSuccessNotification("COF030_Sec0_Success_TemplateEditado");
            }
            else
            {
                uow.TemplateEtiquetaRepository.AddTemplateEtiqueta(etiqueta);

                uow.SaveChanges();
                context.AddSuccessNotification("COF030_Sec0_Success_TemplateAgregado");
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new MantenimientoTemplateEtiquetaValidationModule(uow, this._identity.GetFormatProvider()), form, context);
        }

        #region Metodos Auxiliares

        public virtual void InicializarSelectores(Form form, IUnitOfWork uow)
        {
            //Carga de estilo
            FormField selectorEstilo = form.GetField("estilo");
            selectorEstilo.Options = new List<SelectOption>();

            List<EtiquetaEstilo> etiquetasEstilo = uow.EstiloEtiquetaRepository.GetEtiquetaEstilos();

            foreach (var etiqueta in etiquetasEstilo)
            {
                selectorEstilo.Options.Add(new SelectOption(etiqueta.Id, $"{etiqueta.Id} - {etiqueta.Descripcion} - {etiqueta.Tipo}"));
            }

            //Carga de lenguaje
            FormField selectorLenguaje = form.GetField("lenguaje");
            selectorLenguaje.Options = new List<SelectOption>();

            List<LenguajeImpresion> lenguajes = uow.ImpresionRepository.GetLenguajesImpresion();

            foreach (var lenguaje in lenguajes)
            {
                selectorLenguaje.Options.Add(new SelectOption(lenguaje.Id, $"{lenguaje.Id} - {lenguaje.Descripcion}"));
            }
        }

        public virtual void EliminarTemplate(GridButtonActionContext context, string estilo, string lenguaje)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.TemplateEtiquetaRepository.DeleteTemplateEtiqueta(estilo, lenguaje);

            uow.SaveChanges();
            context.AddSuccessNotification("COF030_grid1_Success_TemplateEliminado");
        }

        #endregion
    }
}
