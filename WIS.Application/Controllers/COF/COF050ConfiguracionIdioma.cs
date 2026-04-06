using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.Configuracion;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Configuracion;
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
using WIS.Translation;

namespace WIS.Application.Controllers.COF
{
    public class COF050ConfiguracionIdioma : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<COF050ConfiguracionIdioma> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public COF050ConfiguracionIdioma(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<COF050ConfiguracionIdioma> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_APLICACION", "CD_BLOQUE", "CD_TIPO", "CD_CLAVE", "CD_IDIOMA",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_APLICACION", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = false;
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = true;

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var lenguajeModificarFiltro = context.Parameters.FirstOrDefault(s => s.Id == "lenguajeModificarFiltro")?.Value;

            //Cargo el filtro default por idioma de usuario
            if (string.IsNullOrEmpty(lenguajeModificarFiltro))
                lenguajeModificarFiltro = this._identity.GetFormatProvider().ToString();

            var dbQuery = new ConfiguracionIdiomaQuery(lenguajeModificarFiltro);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> {
                "DS_VALOR_NUEVO",
            });

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var lenguajeModificarFiltro = context.Parameters.FirstOrDefault(s => s.Id == "lenguajeModificarFiltro")?.Value;
            
            //Cargo el filtro default por idioma de usuario
            if (string.IsNullOrEmpty(lenguajeModificarFiltro))
                lenguajeModificarFiltro = this._identity.GetFormatProvider().ToString();

            var dbQuery = new ConfiguracionIdiomaQuery(lenguajeModificarFiltro);
            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string lenguajeModificarFiltro = context.Parameters.FirstOrDefault(s => s.Id == "lenguajeModificarFiltro")?.Value;

            //Cargo el filtro default por idioma de usuario
            if (string.IsNullOrEmpty(lenguajeModificarFiltro))
                lenguajeModificarFiltro = this._identity.GetFormatProvider().ToString();

            var dbQuery = new ConfiguracionIdiomaQuery(lenguajeModificarFiltro);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    var idiomas = new List<ConfiguracionIdioma>();

                    foreach (var row in grid.Rows)
                    {
                        var configuracionIdioma = this.MapRowToObject(row);

                        bool existeConfiguracionIdioma = uow.ConfiguracionIdiomaRepository
                            .ExisteConfiguracionIdioma(configuracionIdioma.Aplicacion, configuracionIdioma.Bloque,configuracionIdioma.Tipo, configuracionIdioma.Clave, configuracionIdioma.Idioma);

                        //Agrego idioma a lista para luego hacer group by y actualizar version
                        idiomas.Add(configuracionIdioma);

                        if (row.IsDeleted)
                        {
                            uow.ConfiguracionIdiomaRepository.DeleteLenguajeImpresion(configuracionIdioma.Aplicacion, configuracionIdioma.Bloque,
                                                configuracionIdioma.Tipo, configuracionIdioma.Clave, configuracionIdioma.Idioma);
                        }
                        else if (!existeConfiguracionIdioma)
                            uow.ConfiguracionIdiomaRepository.AddConfiguracionIdioma(configuracionIdioma);
                        else
                            uow.ConfiguracionIdiomaRepository.UpdateConfiguracionIdioma(configuracionIdioma);
                    }

                    //Group by para conseguir idiomas
                    var idiomaGRP = from idioma in idiomas
                                    group idiomas by idioma.Idioma into idiomasGRP
                                    select new { idiomasGRP.Key };

                    foreach (var idioma in idiomaGRP)
                    {
                        var version = uow.LocalizationRepository.GetVersionByLanguage(idioma.Key);

                        //Actualizo version traduccion. Si no existe se crea.
                        if (version == null)
                        {
                            var nuevaVersion = new TranslationVersion
                            {
                                Language = idioma.Key,
                                Version = 1,
                                LastEdited = DateTime.Now
                            };
                            uow.LocalizationRepository.AddTranslationVersion(nuevaVersion);
                        }
                        else
                        {
                            version.IncreaseVersion();
                            uow.LocalizationRepository.UpdateTranslationVersion(version);
                        }
                    }
                }

                uow.SaveChanges();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "COF010GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoConfiguracionIdiomaGridValidationModule(uow), grid, row, context);
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            form.GetField("lenguajeModificarFiltro").Options = this.SelectIdioma(uow);

            //Cargo el filtro default por idioma de usuario
            form.GetField("lenguajeModificarFiltro").Value = this._identity.GetFormatProvider().ToString();

            return form;
        }
        
        #region Metodos Auxiliares

        public virtual List<SelectOption> SelectIdioma(IUnitOfWork uow)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            CultureInfo[] culturas = CultureInfo.GetCultures(CultureTypes.NeutralCultures);

            foreach (CultureInfo cultura in culturas)
            {
                opciones.Add(new SelectOption(cultura.TwoLetterISOLanguageName, cultura.NativeName));
            }

            return opciones;
        }

        public virtual ConfiguracionIdioma MapRowToObject(GridRow row)
        {
            return new ConfiguracionIdioma
            {
                Aplicacion = row.GetCell("CD_APLICACION").Value,
                Bloque = row.GetCell("CD_BLOQUE").Value,
                Tipo = row.GetCell("CD_TIPO").Value,
                Clave = row.GetCell("CD_CLAVE").Value,
                Idioma = row.GetCell("CD_IDIOMA_NUEVO").Value,
                Valor = row.GetCell("DS_VALOR_NUEVO").Value
            };
        }

        #endregion
    }
}
