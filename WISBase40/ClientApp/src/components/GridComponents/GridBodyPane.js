import React, { Component } from 'react';
import { ViewPortBodyLeft } from './GridViewPortBodyLeft';
import { ViewPortBodyCenter } from './GridViewPortBodyCenter';
import { ViewPortBodyRight } from './GridViewPortBodyRight';
import { ViewPortBodySelection } from './GridViewPortBodySelection';
import withScrollContext from './WithScrollContext';

class InternalBodyPane extends Component {
    constructor(props) {
        super(props);

        this.bodyPaneRef = React.createRef();
        this.controlActive = false;
        this.shiftActive = false;
    }

    shouldComponentUpdate(nextProps) {
        return !nextProps.isResizing;
    }
    componentDidUpdate(prevProps) {
        //Volver al principio de la lista cuando se recrean las filas
        if (this.bodyPaneRef.current && prevProps.rowTotalRows > this.props.rowTotalRows) {
            this.bodyPaneRef.current.scrollTop = 0;
        }

        if (this.props.autofocus && prevProps.isInitializing && !this.props.isInitializing) {
            this.moveToNewRowEditableCell();
        }
    }

    shouldCellDisplayBorderLeft = () => {
        return !this.props.enableSelection;
    }

    handleScroll = (evt) => {
        if (evt.target.className.indexOf("gr-body-pane") > -1) {
            this.props.updateScrollPosition(evt.target.scrollTop);
        }
    }
    handleMouseOver(evt) {
        //Manual, por motivos de performance
        let row = evt.target.closest(".gr-row");

        if (row) {
            let pane = row.closest(".gr-body-pane");

            var index = Array.prototype.indexOf.call(row.parentNode.children, row) + 1;

            let current = pane.querySelectorAll(".highlight");

            if (current) {
                for (var i = 0; i < current.length; i++) {
                    current[i].classList.toggle("highlight", false);
                }
            }

            const rowSelection = pane.querySelector(".gr-vp-body-selection .gr-row:nth-child(" + index + ")");
            const rowLeft = pane.querySelector(".gr-vp-body-left .gr-row:nth-child(" + index + ")");
            const rowCenter = pane.querySelector(".gr-vp-body-center .gr-row:nth-child(" + index + ")");
            const rowRight = pane.querySelector(".gr-vp-body-right .gr-row:nth-child(" + index + ")");

            if (rowSelection)
                rowSelection.classList.toggle("highlight", true);

            if(rowCenter)
                rowCenter.classList.toggle("highlight", true);

            if(rowLeft)
                rowLeft.classList.toggle("highlight", true);

            if(rowRight)
                rowRight.classList.toggle("highlight", true);
        }
    }
    handleMouseLeave(evt) {
        let current = evt.target.closest(".gr-body-pane").querySelectorAll(".highlight");

        if (current) {
            for (var i = 0; i < current.length; i++) {
                current[i].classList.toggle("highlight", false);
            }
        }
    }

    moveToPreviousEditableCell = (row, column, callback) => {
        this.props.moveToPreviousEditableCell(this.props.rows, row, column, callback);
    };
    moveToNextEditableCell = (row, column, callback) => {
        this.props.moveToNextEditableCell(this.props.rows, row, column, callback);
    };
    moveToFirstEditableCell = (callback) => {
        this.props.moveToFirstEditableCell(this.props.rows, callback);
    };
    moveToLastEditableCell = (callback) => {
        this.props.moveToLastEditableCell(this.props.rows, callback);
    };
    moveToNewRowEditableCell = (callback) => {
        this.props.moveToNewRowEditableCell(callback);
    };

    handleKeydown = (evt) => {
        if (evt.which === 17) {
            this.controlActive = true;
        }
    }
    handleKeyup = (evt) => {
        if (evt.which === 17) {
            this.controlActive = false;
        }
    }

    isControlActive = () => {
        return this.controlActive;
    }

    getRows = () => {
        let rowsToShow = [];

        if (this.props.rows.length > 0) {
            for (var i = this.props.rowDisplayStart; i < this.props.rowDisplayEnd; i++) {
                if(this.props.rows[i])
                    rowsToShow.push(this.props.rows[i]);
            }
        }

        return rowsToShow;
    }

    getBodyHeight = () => {
        if (this.props.rows.length === 0)
            return 40;

        return this.props.rows.length * this.props.rowHeight;
    }
    getTopRowSpan = () => {
        const value = this.props.rowDisplayStart * this.props.rowHeight;
        return value > 0 ? value : 0;
    }
    getBottomRowSpan = () => {
        const value = (this.props.rows.length - this.props.rowDisplayEnd) * this.props.rowHeight;
        return value > 0 ? value : 0;
    }
    getStyle = () => {
        return {
            height: this.props.maxHeight
        };
    }
    
    render() {
        const rows = this.getRows();
        const topRowSpan = this.getTopRowSpan();
        const bottomRowSpan = this.getBottomRowSpan();
        const bodyHeight = this.getBodyHeight();

        return (
            <div className="gr-body-pane"
                onMouseOver={this.handleMouseOver}
                onMouseLeave={this.handleMouseLeave}
                onKeyDown={this.handleKeydown}
                onKeyUp={this.handleKeyup}
                onScroll={this.handleScroll}
                style={this.getStyle()}
                ref={this.bodyPaneRef}
            >
                <div className="gr-body-pane-container" style={{ height: this.getBodyHeight() }} >
                    <ViewPortBodySelection
                        rows={rows}
                        selection={this.props.selection}
                        enableSelection={this.props.enableSelection}
                        updateSelection={this.props.updateSelection}
                        totalHeight={bodyHeight}
                        rowTopSpan={topRowSpan}
                        rowBottomSpan={bottomRowSpan}
                        isSelectionInverted={this.props.isSelectionInverted}
                        isEditingEnabled={this.props.isEditingEnabled}
                    />
                    <ViewPortBodyLeft
                        columns={this.props.columns}
                        rows={rows}
                        selection={this.props.selection}

                        isControlActive={this.isControlActive}

                        hasFixedColumnsLeft={this.props.hasFixedColumnsLeft}
                        hasFixedColumnsRight={this.props.hasFixedColumnsRight}

                        displaySelectionOnly={this.props.displaySelectionOnly}
                        displayModifiedOnly={this.props.displayModifiedOnly}

                        getRowHighlights={this.props.getRowHighlights}
                        toggleHighlight={this.props.toggleHighlight}
                        clearHighlighted={this.props.clearHighlighted}
                        isHighlighted={this.props.isHighlighted}

                        openRelatedLink={this.props.openRelatedLink}

                        fixSelectGridHeight={this.props.fixSelectGridHeight}

                        gridClientLeft={this.props.gridClientLeft}

                        updateCellValue={this.props.updateCellValue}
                        performButtonAction={this.props.performButtonAction}
                        openDropdown={this.props.openDropdown}
                        searchSelectValue={this.props.searchSelectValue}
                        cellDisplayBorderLeft={this.shouldCellDisplayBorderLeft()}
                        totalHeight={bodyHeight}
                        rowTopSpan={topRowSpan}
                        rowBottomSpan={bottomRowSpan}
                        widthLeft={this.props.widthLeft}

                        isEditingEnabled={this.props.isEditingEnabled}
                        editingRow={this.props.editingRow}
                        editingColumn={this.props.editingColumn}
                        setEditingCell={this.props.setEditingCell}
                        clearEditingCell={this.props.clearEditingCell}
                        moveToPreviousEditableCell={this.moveToPreviousEditableCell}
                        moveToNextEditableCell={this.moveToNextEditableCell}
                        moveToFirstEditableCell={this.moveToFirstEditableCell}
                        moveToLastEditableCell={this.moveToLastEditableCell}
                        moveToNewRowEditableCell={this.moveToNewRowEditableCell}

                        translator={this.props.translator}
                    />
                    <ViewPortBodyCenter
                        columns={this.props.columns}
                        rows={rows}
                        selection={this.props.selection}

                        isControlActive={this.isControlActive}

                        hasFixedColumnsLeft={this.props.hasFixedColumnsLeft}
                        hasFixedColumnsRight={this.props.hasFixedColumnsRight}
                        perfScrollbarUpdate={this.props.perfScrollbarUpdate}

                        displaySelectionOnly={this.props.displaySelectionOnly}
                        displayModifiedOnly={this.props.displayModifiedOnly}

                        getRowHighlights={this.props.getRowHighlights}
                        toggleHighlight={this.props.toggleHighlight}
                        clearHighlighted={this.props.clearHighlighted}
                        isHighlighted={this.props.isHighlighted}

                        openRelatedLink={this.props.openRelatedLink}

                        fixSelectGridHeight={this.props.fixSelectGridHeight}

                        gridClientLeft={this.props.gridClientLeft}

                        totalHeight={bodyHeight}
                        rowTopSpan={topRowSpan}
                        rowBottomSpan={bottomRowSpan}
                        rowTotalRows={this.props.rowTotalRows}
                        updateScrollPosition={this.props.updateScrollPosition}
                        updateCellValue={this.props.updateCellValue}
                        openDropdown={this.props.openDropdown}
                        searchSelectValue={this.props.searchSelectValue}
                        performButtonAction={this.props.performButtonAction}
                        isFetching={this.props.isFetching}

                        isEditingEnabled={this.props.isEditingEnabled}
                        editingRow={this.props.editingRow}
                        editingColumn={this.props.editingColumn}
                        setEditingCell={this.props.setEditingCell}
                        clearEditingCell={this.props.clearEditingCell}
                        moveToPreviousEditableCell={this.moveToPreviousEditableCell}
                        moveToNextEditableCell={this.moveToNextEditableCell}
                        moveToFirstEditableCell={this.moveToFirstEditableCell}
                        moveToLastEditableCell={this.moveToLastEditableCell}
                        moveToNewRowEditableCell={this.moveToNewRowEditableCell}

                        translator={this.props.translator}
                    />
                    <ViewPortBodyRight
                        columns={this.props.columns}
                        rows={rows}
                        selection={this.props.selection}

                        isControlActive={this.isControlActive}

                        hasFixedColumnsLeft={this.props.hasFixedColumnsLeft}
                        hasFixedColumnsRight={this.props.hasFixedColumnsRight}

                        displaySelectionOnly={this.props.displaySelectionOnly}
                        displayModifiedOnly={this.props.displayModifiedOnly}

                        getRowHighlights={this.props.getRowHighlights}
                        toggleHighlight={this.props.toggleHighlight}
                        clearHighlighted={this.props.clearHighlighted}
                        isHighlighted={this.props.isHighlighted}

                        openRelatedLink={this.props.openRelatedLink}

                        fixSelectGridHeight={this.props.fixSelectGridHeight}

                        gridClientLeft={this.props.gridClientLeft}

                        updateCellValue={this.props.updateCellValue}
                        performButtonAction={this.props.performButtonAction}
                        openDropdown={this.props.openDropdown}
                        searchSelectValue={this.props.searchSelectValue}
                        totalHeight={bodyHeight}
                        rowTopSpan={topRowSpan}
                        rowBottomSpan={bottomRowSpan}
                        widthRight={this.props.widthRight}

                        isEditingEnabled={this.props.isEditingEnabled}
                        editingRow={this.props.editingRow}
                        editingColumn={this.props.editingColumn}
                        setEditingCell={this.props.setEditingCell}
                        clearEditingCell={this.props.clearEditingCell}
                        moveToPreviousEditableCell={this.moveToPreviousEditableCell}
                        moveToNextEditableCell={this.moveToNextEditableCell}
                        moveToFirstEditableCell={this.moveToFirstEditableCell}
                        moveToLastEditableCell={this.moveToLastEditableCell}
                        moveToNewRowEditableCell={this.moveToNewRowEditableCell}

                        translator={this.props.translator}
                    />
                </div>
            </div>            
        );
    }
}

export const BodyPane = withScrollContext(InternalBodyPane);