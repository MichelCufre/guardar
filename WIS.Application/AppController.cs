using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common.Select;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution;
using WIS.GridComponent.Execution.Configuration;
using WIS.PageComponent.Execution;

namespace WIS.Application
{
    public abstract class AppController : IAppController, IGridController, IFormController, IPageController
    {
        public virtual PageContext PageLoad(PageContext data)
        {
            return data;
        }
        public virtual PageContext PageUnload(PageContext data)
        {
            return data;
        }

        public virtual Form FormInitialize(Form form, FormInitializeContext context)
        {
            return form;
        }
        public virtual Form FormValidateForm(Form form, FormValidationContext context)
        {
            return form;
        }
        public virtual Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            throw new NotImplementedException();
        }
        public virtual Form FormSubmit(Form form, FormSubmitContext context)
        {
            throw new NotImplementedException();
        }
        public virtual List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            throw new NotImplementedException();
        }

        public virtual Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }
        public virtual Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            throw new NotImplementedException();
        }
        public virtual GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            return null;
        }
        public virtual Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            return grid;
        }
        public virtual Grid GridCommit(Grid grid, GridFetchContext context)
        {
            throw new NotImplementedException();
        }
        public virtual GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext selection)
        {
            throw new NotImplementedException();
        }
        public virtual GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            throw new NotImplementedException();
        }
        public virtual byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            throw new NotImplementedException();
        }
        public virtual Grid GridImportExcel(Grid grid, GridImportExcelContext context)
        {
            if (context.Payload == null)
                throw new MissingParameterException("Datos nulos");

            using (var excelImporter = new GridExcelImporter(context.Translator, context.FileName, grid.Columns, context.Payload))
            {
                try
                {
                    var rowsExcel = excelImporter.BuildRows();
                    int rowId = 0;

                    foreach (var row in rowsExcel)
                    {
                        foreach (var column in grid.Columns)
                        {
                            if (!row.Cells.Any(c => c.Column.Id == column.Id))
                            {
                                row.AddCell(new GridCell()
                                {
                                    Column = column,
                                });
                            }
                        }

                        rowId--;
                        row.Id = rowId.ToString();

                        var validationContext = new GridValidationContext
                        {
                            Parameters = context.FetchContext.Parameters
                        };

                        grid = this.GridValidateRow(row, grid, validationContext);
                    }

                    if (grid.Rows.Any(r => !r.IsDeleted && !r.IsValid()))
                        throw new ValidationFailedException("General_Sec0_Error_Error07");

                    grid.Rows.AddRange(rowsExcel);
                    grid = this.GridCommit(grid, context.FetchContext);

                    grid = this.GridFetchRows(grid, context.FetchContext);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return grid;
        }
        public virtual byte[] GridGenerateExcelTemplate(Grid grid, GridImportExcelContext context, int? interfazExterna = null)
        {
            Dictionary<string, List<IGridColumn>> sheetColumns = new Dictionary<string, List<IGridColumn>>();

            sheetColumns["IExcel_Template_lbl_SheetData"] = grid.Columns;

            return GridGenerateExcelTemplate(sheetColumns, context, interfazExterna);
        }

        public virtual byte[] GridGenerateExcelTemplate(Dictionary<string, List<IGridColumn>> sheetColumns, GridImportExcelContext context, int? interfazExterna = null)
        {
            var template = new GridExcelImportTemplate(context.FileName, sheetColumns, context.Translator, interfazExterna);
            template.Generate();
            return template.GetAsByteArray();
        }

        public virtual List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            throw new NotImplementedException();
        }
        public virtual GridNotifySelectionContext GridNotifySelection(GridNotifySelectionContext context)
        {
            return context;
        }
        public virtual GridNotifyInvertSelectionContext GridNotifyInvertSelection(GridNotifyInvertSelectionContext context)
        {
            return context;
        }

    }
}
