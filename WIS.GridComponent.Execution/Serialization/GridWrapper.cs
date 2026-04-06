using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;
using WIS.Components.Common.Redirection;
using WIS.Components.Common.Select;
using WIS.Filtering;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Excel.Responses;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Execution.Responses;
using WIS.GridComponent.Items;
using WIS.Serialization;
using WIS.Serialization.Binders;
using WIS.Sorting;

namespace WIS.GridComponent.Execution.Serialization
{
    public class GridWrapper : TransferWrapper, ITransferWrapper, IGridWrapper
    {
        public GridAction Action { get; set; }
        public string GridId { get; set; }

        public GridWrapper() : base()
        {
        }
        public GridWrapper(string gridId) : base()
        {
            this.GridId = gridId;
        }

        public GridWrapper(IGridWrapper wrapper) : base(wrapper)
        {
            this.GridId = wrapper.GridId;
        }

        public override ISerializationBinder GetSerializationBinder()
        {
            //Esto se define por seguridad, no se permite pasar tipos no esperados
            return new CustomSerializationBinder(new List<Type> {
                typeof(Grid),
                typeof(GridRow),
                typeof(GridColumn),
                typeof(GridColumnButton),
                typeof(GridColumnItemList),
                typeof(GridColumnText),
                typeof(GridColumnSelect),
                typeof(GridColumnSelectAsync),
                typeof(GridColumnToggle),
                typeof(GridCell),
                typeof(GridStats),
                typeof(GridColumnLink),
                typeof(GridColumnLinkMapping),

                typeof(GridItem),
                typeof(GridItemHeader),
                typeof(GridItemDivider),

                typeof(GridButton),
                typeof(GridSelection),

                typeof(GridCommitContext),
                typeof(GridInitializeResponse),
                typeof(GridFetchContext),
                typeof(GridFetchResponse),
                typeof(GridValidationContext),
                typeof(GridValidationResponse),
                typeof(GridButtonActionContext),
                typeof(GridMenuItemActionContext),
                typeof(GridExportExcelContext),
                typeof(GridExportExcelResponse),
                typeof(GridImportExcelContext),
                typeof(GridImportExcelResponse),
                typeof(GridSelectSearchContext),
                typeof(GridSelectSearchPayload),
                typeof(GridSelectSearchResponse),
                typeof(GridFilterData),
                typeof(GridFilterRemoveContext),
                typeof(GridFetchStatsContext),
                typeof(GridFetchStatsResponse),
                typeof(GridNotifySelectionContext),
                typeof(GridNotifyInvertSelectionContext),

                typeof(FilterCommand),
                typeof(SortCommand),

                typeof(ApplicationNotification),
                typeof(ComponentParameter),
                typeof(ComponentContext),
                typeof(SelectOption),
                typeof(ComponentError),
                typeof(ConfirmMessage),
                typeof(PageRedirection)
            });
        }
    }
}
