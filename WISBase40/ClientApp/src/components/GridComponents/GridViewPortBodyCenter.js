import React, { Component } from 'react';
import { BodyAdaptive } from './GridBodyAdaptive';
import withScrollContext from './WithScrollContext';
import { columnFixed } from '../Enums';

class InternalViewPortBodyCenter extends Component {
    shouldComponentUpdate(nextProps) {
        return !nextProps.scrollContext.isScrolling;
    }

    getColumns = () => {
        return this.props.columns.filter(d => d.fixed === columnFixed.none);
    }

    render() {
        return (
            <div className="gr-vp-body-center">
                <BodyAdaptive
                    rows={this.props.rows}
                    columns={this.getColumns()}
                    selection={this.props.selection}

                    isControlActive={this.props.isControlActive}

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

                    perfScrollbarUpdate={this.props.scrollContext.perfScrollbarUpdate}
                    rowDisplayStart={this.props.rowDisplayStart}
                    rowDisplayEnd={this.props.rowDisplayEnd}
                    totalHeight={this.props.totalHeight}
                    rowTopSpan={this.props.rowTopSpan}
                    rowBottomSpan={this.props.rowBottomSpan}
                    rowTotalRows={this.props.rowTotalRows}
                    updateScrollPosition={this.props.updateScrollPosition}
                    updateCellValue={this.props.updateCellValue}
                    performButtonAction={this.props.performButtonAction}
                    openDropdown={this.props.openDropdown}
                    searchSelectValue={this.props.searchSelectValue}
                    isScrolling={this.props.scrollContext.isScrolling}
                    isFetching={this.props.isFetching}

                    isEditingEnabled={this.props.isEditingEnabled}
                    editingRow={this.props.editingRow}
                    editingColumn={this.props.editingColumn}
                    setEditingCell={this.props.setEditingCell}
                    clearEditingCell={this.props.clearEditingCell}
                    moveToPreviousEditableCell={this.props.moveToPreviousEditableCell}
                    moveToNextEditableCell={this.props.moveToNextEditableCell}
                    moveToFirstEditableCell={this.props.moveToFirstEditableCell}
                    moveToLastEditableCell={this.props.moveToLastEditableCell}
                    moveToNewRowEditableCell={this.props.moveToNewRowEditableCell}

                    translator={this.props.translator}
                />
            </div>
        );
    }
}

export const ViewPortBodyCenter = withScrollContext(InternalViewPortBodyCenter);