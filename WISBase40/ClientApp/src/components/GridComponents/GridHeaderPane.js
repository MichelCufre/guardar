import React, { useRef } from 'react';
import { ViewPortHeaderLeft } from './GridViewPortHeaderLeft';
import { ViewPortHeaderCenter } from './GridViewPortHeaderCenter';
import { ViewPortHeaderRight } from './GridViewPortHeaderRight';
import { ViewPortHeaderSelection } from './GridViewPortHeaderSelection';
import { Header } from './GridHeader';
import { HeaderSelection } from './GridHeaderSelection';
import { columnFixed } from '../Enums';

function InternalHeaderPane(props) {
    const draggedColumnData = useRef(null);

    const getColumnsCenter = () => {
        return props.columns.filter(d => d.fixed === columnFixed.none);
    }
    const getColumnsFixedLeft = () => {
        return props.columns.filter(d => d.fixed === columnFixed.left);
    }
    const getColumnsFixedRight = () => {
        return props.columns.filter(d => d.fixed === columnFixed.right);
    }

    const getFilters = (columns) => {
        if (columns && props.filters) {
            return props.filters.filter(f => columns.some(col => col.id === f.columnId));
        }

        return null;
    }
       
    const getViewPortHeaderSelectionContent = () => {
        if (!props.enableSelection)
            return null;

        return (
            <HeaderSelection
                selection={props.selection}
                filterStatus={props.filterStatus}
                isSelectionInverted={props.isSelectionInverted}
                invertSelection={props.invertSelection}
                updateFilter={props.updateFilter}
                applyFilter={props.applyFilter}
                isEditingEnabled={props.isEditingEnabled}
            />
        );
    }

    const shouldDisplayBorderLeft = () => {
        return !props.enableSelection;
    }

    const setDraggedColumn = (columnData) => {
        draggedColumnData.current = columnData;
    }
    const getDraggedColumn = () => {
        return draggedColumnData.current;
    }

    const columnsCenter = getColumnsCenter();
    const columnsFixedLeft = getColumnsFixedLeft();
    const columnsFixedRight = getColumnsFixedRight();

    const filtersCenter = getFilters(columnsCenter);
    const filtersFixedLeft = getFilters(columnsFixedLeft);
    const filtersFixedRight = getFilters(columnsFixedRight);

    return(
        <div className="gr-header-pane">
            <ViewPortHeaderSelection>
                {getViewPortHeaderSelectionContent()}
            </ViewPortHeaderSelection>
            <ViewPortHeaderLeft width={props.widthLeft}>
                <Header
                    gridId={props.gridId}
                    columns={columnsFixedLeft}
                    filters={filtersFixedLeft}
                    filterStatus={props.filterStatus}
                    highlightLast={props.highlightLast}
                    sorts={props.sorts}
                    displayBorderLeft={shouldDisplayBorderLeft()}
                    columnResizeBegin={props.columnResizeBegin}
                    updateFilter={props.updateFilter}
                    updateColumnOrder={props.updateColumnOrder}
                    fixColumn={props.fixColumn}
                    hideColumn={props.hideColumn}
                    applyFilter={props.applyFilter}
                    applySort={props.applySort}
                    applySortAscending={props.applySortAscending}
                    applySortDescending={props.applySortDescending}
                    applySortReset={props.applySortReset}
                    isResizing={props.isResizing}

                    setDraggedColumn={setDraggedColumn}
                    getDraggedColumn={getDraggedColumn}
                />
            </ViewPortHeaderLeft>
            <ViewPortHeaderCenter>
                <Header
                    gridId={props.gridId}
                    columns={columnsCenter}
                    filters={filtersCenter}
                    filterStatus={props.filterStatus}
                    highlightLast={props.highlightLast}
                    sorts={props.sorts}
                    columnResizeBegin={props.columnResizeBegin}
                    updateFilter={props.updateFilter}
                    updateColumnOrder={props.updateColumnOrder}
                    fixColumn={props.fixColumn}
                    hideColumn={props.hideColumn}
                    applyFilter={props.applyFilter}
                    applySort={props.applySort}
                    applySortAscending={props.applySortAscending}
                    applySortDescending={props.applySortDescending}
                    applySortReset={props.applySortReset}
                    isResizing={props.isResizing}

                    setDraggedColumn={setDraggedColumn}
                    getDraggedColumn={getDraggedColumn}
                />
            </ViewPortHeaderCenter>
            <ViewPortHeaderRight
                width={props.widthRight}
                isVScrollActive={props.isVScrollActive}
            >
                <Header
                    gridId={props.gridId}
                    resizeLeft
                    columns={columnsFixedRight}
                    filters={filtersFixedRight}
                    filterStatus={props.filterStatus}
                    highlightLast={props.highlightLast}
                    sorts={props.sorts}
                    columnResizeBegin={props.columnResizeBegin}
                    updateFilter={props.updateFilter}
                    updateColumnOrder={props.updateColumnOrder}
                    fixColumn={props.fixColumn}
                    hideColumn={props.hideColumn}
                    applyFilter={props.applyFilter}
                    applySort={props.applySort}
                    applySortAscending={props.applySortAscending}
                    applySortDescending={props.applySortDescending}
                    applySortReset={props.applySortReset}
                    isResizing={props.isResizing}

                    setDraggedColumn={setDraggedColumn}
                    getDraggedColumn={getDraggedColumn}

                    shouldAddFillers={props.isVScrollActive}
                />
            </ViewPortHeaderRight>
        </div>
    );
}

export const HeaderPane = React.memo(InternalHeaderPane, (props, nextProps) => {
    return nextProps.isResizing;
});