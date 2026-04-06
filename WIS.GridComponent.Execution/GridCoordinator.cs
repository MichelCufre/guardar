using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using WIS.GridComponent;
using WIS.GridComponent.Execution.Serialization;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using System.Linq;
using WIS.GridComponent.Excel.Configuration;
using WIS.Components.Common.Select;
using WIS.GridComponent.Execution.Responses;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Responses;
using WIS.Translation;
using WIS.Exceptions;
using WIS.Filtering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WIS.GridComponent.Execution
{
    public class GridCoordinator : IGridCoordinator
    {
        private readonly IGridConfigProvider _configProvider;
        private readonly ITranslator _translator;

        public Dictionary<GridAction, Func<IGridWrapper, IGridController, IGridWrapper>> Actions { get; }

        public GridCoordinator(IGridConfigProvider configProvider, ITranslator translator)
        {
            this._configProvider = configProvider;
            this._translator = translator;

            this.Actions = new Dictionary<GridAction, Func<IGridWrapper, IGridController, IGridWrapper>>
            {
                [GridAction.ButtonAction] = this.ButtonAction,
                [GridAction.Commit] = this.Commit,
                [GridAction.ExportExcel] = this.ExportExcel,
                [GridAction.FetchRows] = this.FetchRows,
                [GridAction.FetchStats] = this.FetchStats,
                [GridAction.GenerateExcelTemplate] = this.GenerateExcelTemplate,
                [GridAction.ImportExcel] = this.ImportExcel,
                [GridAction.Initialize] = this.Initialize,
                [GridAction.MenuItemAction] = this.MenuItemAction,
                [GridAction.NotifyInvertSelection] = this.NotifyInvertSelection,
                [GridAction.NotifySelection] = this.NotifySelection,
                [GridAction.SelectSearch] = this.SelectSearch,
                [GridAction.ValidateRow] = this.ValidateRow
            };
        }

        public bool IsActionAvailable(GridAction action)
        {
            return this.Actions.ContainsKey(action);
        }

        private IGridWrapper Initialize(IGridWrapper wrapper, IGridController controller)
        {
            var query = wrapper.GetData<GridFetchContext>();
            query.IsGridInitialize = true;
            IGridWrapper response = new GridWrapper(wrapper);

            var grid = new Grid(wrapper.GridId);

            GridFilterData filterData = this._configProvider.GetFilterData(wrapper.Application, wrapper.User, wrapper.GridId);

            grid.Columns = this._configProvider.GetColumns(wrapper.Application, wrapper.User, wrapper.GridId);

            if (filterData != null)
            {
                query.Filters = filterData.Filters;
                query.Sorts = filterData.Sorts;
            }

            var initQuery = new GridInitializeContext
            {
                FetchContext = query,
                Parameters = query.Parameters

            };

            grid = controller.GridInitialize(grid, initQuery);

            this._configProvider.SaveColumns(grid, wrapper.Application, wrapper.User);

            var queryResponse = new GridInitializeResponse
            {
                Grid = grid,
                Parameters = query.Parameters,
                Notifications = query.Notifications,
                FilterData = filterData,
                IsEditingEnabled = initQuery.IsEditingEnabled,
                IsCommitEnabled = initQuery.IsCommitEnabled,
                IsAddEnabled = initQuery.IsAddEnabled,
                IsRemoveEnabled = initQuery.IsRemoveEnabled,
                IsRollbackEnabled = initQuery.IsRollbackEnabled,
                IsCommitButtonUnavailable = initQuery.IsCommitButtonUnavailable,
                Links = initQuery.Links
            };

            response.SetData(queryResponse);

            return response;
        }
        private IGridWrapper FetchRows(IGridWrapper wrapper, IGridController controller)
        {
            var query = wrapper.GetData<GridFetchContext>();

            IGridWrapper response = new GridWrapper(wrapper);

            var grid = new Grid(wrapper.GridId);

            grid.Columns = this._configProvider.GetColumns(wrapper.Application, wrapper.User, wrapper.GridId);

            grid = controller.GridFetchRows(grid, query);

            var queryResponse = new GridFetchResponse
            {
                Rows = grid.Rows,
                Parameters = query.Parameters,
                Notifications = query.Notifications
            };

            response.SetData(queryResponse);

            return response;
        }
        private IGridWrapper FetchStats(IGridWrapper wrapper, IGridController controller)
        {
            var query = wrapper.GetData<GridFetchStatsContext>();

            IGridWrapper response = new GridWrapper(wrapper);

            var grid = new Grid(wrapper.GridId)
            {
                Columns = this._configProvider.GetColumns(wrapper.Application, wrapper.User, wrapper.GridId)
            };

            GridStats stats = controller.GridFetchStats(grid, query);

            var queryResponse = new GridFetchStatsResponse
            {
                Stats = stats,
                Parameters = query.Parameters,
                Notifications = query.Notifications
            };

            response.SetData(queryResponse);

            return response;
        }
        private IGridWrapper ValidateRow(IGridWrapper wrapper, IGridController controller)
        {
            var data = wrapper.GetData<GridValidationContext>();

            IGridWrapper response = new GridWrapper(wrapper);

            var grid = new Grid(data.GridId);

            grid.Columns = this._configProvider.GetColumns(wrapper.Application, wrapper.User, wrapper.GridId);

            GridRow row = data.Row;

            row.SetCellColumn(grid.Columns);

            grid = controller.GridValidateRow(row, grid, data);

            var validationResponse = new GridValidationResponse
            {
                Row = row,
                Parameters = data.Parameters,
                Notifications = data.Notifications
            };

            response.SetData(validationResponse);

            return response;
        }
        private IGridWrapper Commit(IGridWrapper wrapper, IGridController controller)
        {
            var data = wrapper.GetData<GridCommitContext>();

            IGridWrapper response = new GridWrapper(wrapper);

            var grid = new Grid(wrapper.GridId)
            {
                Columns = this._configProvider.GetColumns(wrapper.Application, wrapper.User, wrapper.GridId),
                Rows = data.Rows
            };

            try
            {
                foreach (var row in grid.Rows)
                {
                    if (row.IsDeleted)
                        continue;

                    var validatedRow = row;

                    validatedRow.SetCellColumn(grid.Columns);

                    foreach (var cell in validatedRow.Cells)
                    {
                        cell.Modified = true;
                    }

                    var validationContext = new GridValidationContext
                    {
                        Parameters = data.Query.Parameters
                    };

                    grid = controller.GridValidateRow(validatedRow, grid, validationContext);
                }

                if (grid.Rows.Any(r => !r.IsDeleted && !r.IsValid()))
                    throw new ValidationFailedException("General_Sec0_Error_Error07");

                grid = controller.GridCommit(grid, data.Query);

                if (grid.Rows.Any(r => !r.IsValid()))
                {
                    var msg = "General_Sec0_Error_Error07";
                    var args = new string[] { };

                    if (data.Query.Notifications != null && data.Query.Notifications.Count > 0)
                    {
                        msg = data.Query.Notifications.FirstOrDefault().Message;
                        args = data.Query.Notifications.FirstOrDefault()?.Arguments?.ToArray() ?? new string[] { };
                    }

                    throw new ValidationFailedException(msg, args);
                }

                grid = controller.GridFetchRows(grid, data.Query);

                var queryResponse = new GridFetchResponse
                {
                    Rows = grid.Rows,
                    Parameters = data.Query.Parameters,
                    Notifications = data.Query.Notifications
                };

                response.SetData(queryResponse);
            }
            catch (ExpectedException ex)
            {
                response.SetData(new GridFetchResponse { Rows = grid.Rows, Parameters = data.Query.Parameters });
                response.SetError(ex.Message, ex.StrArguments);
            }

            return response;
        }
        private IGridWrapper ButtonAction(IGridWrapper wrapper, IGridController controller)
        {
            var data = wrapper.GetData<GridButtonActionContext>();

            IGridWrapper response = new GridWrapper(wrapper);

            var columns = this._configProvider.GetColumns(wrapper.Application, wrapper.User, wrapper.GridId);

            data.Row.SetCellColumn(columns);

            data = controller.GridButtonAction(data);

            response.SetData(data);

            return response;
        }
        private IGridWrapper MenuItemAction(IGridWrapper wrapper, IGridController controller)
        {
            var data = wrapper.GetData<GridMenuItemActionContext>();

            IGridWrapper response = new GridWrapper(wrapper);

            data = controller.GridMenuItemAction(data);

            response.SetData(data);

            return response;
        }
        private IGridWrapper ExportExcel(IGridWrapper wrapper, IGridController controller)
        {
            var query = wrapper.GetData<GridExportExcelContext>();

            IGridWrapper response = new GridWrapper(wrapper);

            var grid = new Grid(query.GridId);

            grid.Columns = this._configProvider.GetColumns(wrapper.Application, wrapper.User, wrapper.GridId);

            byte[] excel = controller.GridExportExcel(grid, query);

            var queryResponse = new GridExportExcelResponse()
            {
                FileName = query.FileName,
                Notifications = query.Notifications,
                Parameters = query.Parameters
            };

            queryResponse.SetExcelContent(excel);

            response.SetData(queryResponse);

            return response;
        }
        private IGridWrapper ImportExcel(IGridWrapper wrapper, IGridController controller)
        {
            var request = wrapper.GetData<GridImportExcelContext>();

            IGridWrapper response = new GridWrapper(wrapper);

            var grid = new Grid(request.GridId);

            grid.Columns = this._configProvider.GetColumns(wrapper.Application, wrapper.User, wrapper.GridId);

            try
            {
                request.Translator = this._translator;
                grid = controller.GridImportExcel(grid, request);

                var queryResponse = new GridFetchResponse
                {
                    Rows = grid.Rows,
                    Parameters = request.FetchContext.Parameters,
                    Notifications = request.Notifications ?? request.FetchContext.Notifications
                };

                response.SetData(queryResponse);
            }
            catch (GridExcelImporterException ex)
            {
                var queryResponse = new GridImportExcelResponse
                {
                    FileName = request.FileName,
                    ExcelContent = ex.Payload ?? request.Payload
                };

                response.SetData(queryResponse);
                response.SetError(ex.Message, ex.StrArguments);
            }
            catch (ExpectedException ex)
            {
                //Logger.Log(LogLevel.BAJO, wrapper.User.ToString(), LogType.BACKEND, "ImportExcel", ex);

                var queryResponse = new GridImportExcelResponse
                {
                    FileName = request.FileName,
                    ExcelContent = request.Payload
                };

                response.SetData(queryResponse);
                response.SetError(ex.Message, ex.StrArguments);
            }
            catch (Exception ex)
            {
                var queryResponse = new GridImportExcelResponse
                {
                    FileName = request.FileName,
                    ExcelContent = request.Payload
                };

                response.SetData(queryResponse);
                response.SetError(ex.Message);
            }


            return response;
        }
        private IGridWrapper GenerateExcelTemplate(IGridWrapper wrapper, IGridController controller)
        {
            var request = wrapper.GetData<GridImportExcelContext>();
            IGridWrapper response = new GridWrapper(wrapper);

            var grid = new Grid(request.GridId);


            var initQuery = new GridInitializeContext
            {
                Parameters = request.FetchContext.Parameters,
                FetchContext = new GridFetchContext
                {
                    RowsToFetch = 1,
                    Parameters = request.FetchContext.Parameters,
                    IsGridInitialize = false,
                }
            };

            request.Translator = this._translator;

            byte[] excelData = null;
            if (wrapper.Application == "INT050")
            {
                int interfazExterna = int.Parse(request.FetchContext.Parameters.FirstOrDefault(s => s.Id == "api").Value);
                var sheetColumns = this._configProvider.GetApiColumns(interfazExterna, out string filename);
                request.FileName = filename;
                excelData = controller.GridGenerateExcelTemplate(sheetColumns, request, interfazExterna);
            }
            else
            {
                grid.Columns = this._configProvider.GetColumns(wrapper.Application, wrapper.User, wrapper.GridId);
                grid = controller.GridInitialize(grid, initQuery);
                excelData = controller.GridGenerateExcelTemplate(grid, request);
            }

            var queryResponse = new GridImportExcelResponse
            {
                FileName = request.FileName + ".xlsx",
                ExcelContent = Convert.ToBase64String(excelData)
            };

            response.SetData(queryResponse);

            return response;
        }
        private IGridWrapper SelectSearch(IGridWrapper wrapper, IGridController controller)
        {
            var data = wrapper.GetData<GridSelectSearchPayload>();

            IGridWrapper response = new GridWrapper(wrapper);

            var grid = new Grid(wrapper.GridId);

            grid.Columns = this._configProvider.GetColumns(wrapper.Application, wrapper.User, wrapper.GridId);

            var result = new SelectResult(this._configProvider.GetSelectResultLimit());

            data.Query.ResultLimit = result.GetUsableResultLimit();

            data.Row.SetCellColumn(grid.Columns);

            result.SetOptions(controller.GridSelectSearch(data.Row, grid, data.Query));

            var queryResponse = new GridSelectSearchResponse()
            {
                Options = result.Options,
                MoreResultsAvailable = result.MoreResultsAvailable,
                Notifications = data.Query.Notifications,
                Parameters = data.Query.Parameters
            };

            response.SetData(queryResponse);

            return response;
        }
        private IGridWrapper NotifySelection(IGridWrapper wrapper, IGridController controller)
        {
            var data = wrapper.GetData<GridNotifySelectionContext>();

            IGridWrapper response = new GridWrapper(wrapper);

            data = controller.GridNotifySelection(data);

            response.SetData(data);

            return response;
        }
        private IGridWrapper NotifyInvertSelection(IGridWrapper wrapper, IGridController controller)
        {
            var data = wrapper.GetData<GridNotifyInvertSelectionContext>();

            IGridWrapper response = new GridWrapper(wrapper);

            data = controller.GridNotifyInvertSelection(data);

            response.SetData(data);

            return response;
        }

    }
}
