import React, { Component } from 'react';
import { Body } from './GridBody';
import withScrollContext from './WithScrollContext';
import { columnFixed } from '../Enums';

export class InternalViewPortBodyRight extends Component {
    shouldComponentUpdate(nextProps) {
        return !nextProps.scrollContext.isScrolling;
    }

    getColumns = () => {
        return this.props.columns.filter(d => d.fixed === columnFixed.right);
    }

    getStyle = () => {
        return {
            minWidth: this.props.widthRight,
            maxWidth: this.props.widthRight
        };
    }

    render() {
        const columns = this.getColumns();
        
        return (
            <div className="gr-vp-body-right" style={this.getStyle()}>
                <Body
                    rows={this.props.rows}
                    columns={columns}
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

                    gridClientLeft={this.props.gridClientLeft}

                    isScrolling={this.props.scrollContext.isScrolling}
                    updateCellValue={this.props.updateCellValue}
                    performButtonAction={this.props.performButtonAction}
                    openDropdown={this.props.openDropdown}
                    searchSelectValue={this.props.searchSelectValue}
                    totalHeight={this.props.totalHeight}
                    rowTopSpan={this.props.rowTopSpan}
                    rowBottomSpan={this.props.rowBottomSpan}

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

export const ViewPortBodyRight = withScrollContext(InternalViewPortBodyRight);