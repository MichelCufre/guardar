import React, { Component } from 'react';
import { RowSpanContainer } from './GridRowSpanContainer';
import ScrollSyncPane from '../ScrollSyncPane';
import withScrollContext from './WithScrollContext';
import { RowList } from './GridRowList';

class InternalBodyAdaptive extends Component {

    constructor(props) {
        super(props);
    }

    shouldComponentUpdate(nextProps) {
        return !nextProps.scrollContext.isScrolling;
    }

    getBodyStyle = () => {
        return {
            height: "calc(100% + " + (this.props.scrollContext.scrollbarWidth === 0 ? 60 : this.props.scrollContext.scrollbarWidth) + "px)"
        };
    }

    getEmptyBodyStyle = () => {
        var columns = this.props.columns.filter(column => !column.hidden);
        var totalWidth = columns.reduce((partialSum, column) => partialSum + column.width, 0);
        var scrollbarWidth = this.props.scrollContext.scrollbarWidth;
        var totalHeight = "calc(40px + " + (scrollbarWidth === 0 ? 60 : scrollbarWidth) + "px)";

        return {
            minWidth: "100%",
            width: totalWidth,
            height: totalHeight,
        };
    }

    render() {
        return (
            <div
                className="gr-body-scroll"
                ref={this.bodyRef}
            >
                <ScrollSyncPane  group="horizontal">
                    <div className="gr-body" style={this.getBodyStyle()}>
                        <RowSpanContainer height={this.props.rowTopSpan} />
                        <RowList
                            rows={this.props.rows}
                            columns={this.props.columns}
                            selection={this.props.selection}

                            isControlActive={this.props.isControlActive}

                            isBodyAdaptive

                            showEmptyMessage

                            displaySelectionOnly={this.props.displaySelectionOnly}
                            displayModifiedOnly={this.props.displayModifiedOnly}

                            getRowHighlights={this.props.getRowHighlights}
                            toggleHighlight={this.props.toggleHighlight}
                            clearHighlighted={this.props.clearHighlighted}
                            isHighlighted={this.props.isHighlighted}

                            openRelatedLink={this.props.openRelatedLink}

                            getEmptyBodyStyle={this.getEmptyBodyStyle}

                            fixSelectGridHeight={this.props.fixSelectGridHeight}

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
                </ScrollSyncPane>
            </div>
        );
    }
}

export const BodyAdaptive = withScrollContext(InternalBodyAdaptive);