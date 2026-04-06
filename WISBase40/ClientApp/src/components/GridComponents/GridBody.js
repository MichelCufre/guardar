import React, { Component } from 'react';
import { RowList } from './GridRowList';
import { RowSpanContainer } from './GridRowSpanContainer';
import withScrollContext from './WithScrollContext';

class InternalBody extends Component {
    getBodyStyle = () => {
        return {
            height: "calc(100% + " + (this.props.scrollContext.scrollbarWidth === 0 ? 60 : this.props.scrollContext.scrollbarWidth) + "px)"
        };
    }

    getEmptyBodyStyle = () => {
        return {
            height: "calc(40px + " + (this.props.scrollContext.scrollbarWidth === 0 ? 60 : this.props.scrollContext.scrollbarWidth) + "px)"
        };
    }

    render() {
        return (
            <div className="gr-body-scroll">
                <div className="gr-body" style={this.getBodyStyle()} >
                    <RowSpanContainer height={this.props.rowTopSpan} />
                    <RowList
                        rows={this.props.rows}
                        columns={this.props.columns}
                        selection={this.props.selection}

                        isControlActive={this.props.isControlActive}

                        displaySelectionOnly={this.props.displaySelectionOnly}
                        displayModifiedOnly={this.props.displayModifiedOnly}

                        getRowHighlights={this.props.getRowHighlights}
                        toggleHighlight={this.props.toggleHighlight}
                        clearHighlighted={this.props.clearHighlighted}
                        isHighlighted={this.props.isHighlighted}

                        openRelatedLink={this.props.openRelatedLink}

                        fixSelectGridHeight={this.props.fixSelectGridHeight}

                        gridClientLeft={this.props.gridClientLeft}

                        getEmptyBodyStyle={this.getEmptyBodyStyle}

                        cellDisplayBorderLeft={this.props.cellDisplayBorderLeft}
                        updateCellValue={this.props.updateCellValue}
                        performButtonAction={this.props.performButtonAction}
                        openDropdown={this.props.openDropdown}
                        searchSelectValue={this.props.searchSelectValue}

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
                    <RowSpanContainer height={this.props.rowBottomSpan} />
                </div>
            </div>
        );
    }
}

export const Body = withScrollContext(InternalBody);