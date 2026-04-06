import React, { Component } from 'react';
import { Cell } from './GridCell';

export class Row extends Component {
    constructor(props) {
        super(props);

        this.rowRef = React.createRef();
    }

    componentDidMount() {
        if (this.props.highlighted && this.rowRef.current)
            this.rowRef.current.classList.toggle("selected", true); //Performance
    }

    shouldComponentUpdate(nextProps, nextState) {
        return this.props.isDeleted !== nextProps.isDeleted
            || this.props.cells !== nextProps.cells
            || this.props.editingRow !== nextProps.editingRow
            || this.props.editingColumn !== nextProps.editingColumn
            || this.isColumnDiff(this.props.columns, nextProps.columns)
            || this.props.isModified === true;
    }
    
    getCells = (highlightedCells) => {
        return this.props.columns.map((col, index) => {
            return (
                <Cell
                    key={"cell" + col.id}
                    rowIsNew={this.props.isNew}
                    rowIsDeleted={this.props.isDeleted}
                    rowId={this.props.id}
                    rowIndex={this.props.index}

                    addExtraPixel={this.props.isBodyAdaptive && index === this.props.columns.length - 1}

                    editing={this.props.editingRow === this.props.id && this.props.editingColumn === col.id}
                    setEditingCell={this.props.setEditingCell}
                    clearEditingCell={this.props.clearEditingCell}
                    moveToPreviousEditableCell={this.props.moveToPreviousEditableCell}
                    moveToNextEditableCell={this.props.moveToNextEditableCell}
                    moveToFirstEditableCell={this.props.moveToFirstEditableCell}
                    moveToLastEditableCell={this.props.moveToLastEditableCell}
                    moveToNewRowEditableCell={this.props.moveToNewRowEditableCell}

                    disabledButtons={this.props.disabledButtons}
                    column={col}
                    content={this.props.cells.find(cell => cell.column === col.id)}
                    highlighted={highlightedCells && highlightedCells.columns.findIndex(d => d === col.id) > -1}

                    toggleHighlight={this.props.toggleHighlight}
                    clearHighlighted={this.props.clearHighlighted}
                    isHighlighted={this.props.isHighlighted}

                    openRelatedLink={this.props.openRelatedLink}

                    toggleRowHighlight={this.toggleRowHighlight}

                    isControlActive={this.props.isControlActive}

                    fixSelectGridHeight={this.props.fixSelectGridHeight}

                    gridClientLeft={this.props.gridClientLeft}

                    cellDisplayBorderLeft={this.props.cellDisplayBorderLeft}
                    updateCellValue={this.props.updateCellValue}
                    performButtonAction={this.props.performButtonAction}
                    openDropdown={this.props.openDropdown}
                    searchSelectValue={this.props.searchSelectValue}

                    isEditingEnabled={this.props.isEditingEnabled}

                    translator={this.props.translator}
                />
            );
        });
    }
    getClassName = (highlightedCells) => {
        let rowClass = "gr-row";

        if (this.props.isNew) {
            rowClass = rowClass + " new";
        }

        if (this.props.isDeleted) {
            rowClass = rowClass + " deleted";
        }

        if (highlightedCells) {
            rowClass = rowClass + " selected";
        }

        /*if (this.props.cells.some(d => d.value !== d.old)) {
            rowClass = rowClass + " modified";
        }*/

        return `${rowClass} ${this.props.cssClass ? this.props.cssClass : ""}`;
    }

    toggleRowHighlight = (highlightData, clear) => {
        if (this.rowRef.current) {
            this.rowRef.current.classList.toggle("selected");

            this.props.toggleHighlight(highlightData, clear);
        }
    };

    isColumnDiff = (columnsA, columnsB) => {
        if ((!columnsA && columnsB) || (!columnsB && columnsA) || columnsA.length !== columnsB.length) {
            return true;
        }

        for (var i = 0; i < columnsA.length; i++) {
            if (columnsA[i].width !== columnsB[i].width || columnsA[i].id !== columnsB[i].id) {
                return true;
            }
        }

        return false;
    }

    render() {
        const highlightedCells = this.props.getRowHighlights(this.props.id);

        return (
            <div className={this.getClassName(highlightedCells)} id={this.props.id} ref={this.rowRef}>
                {this.getCells(highlightedCells)}
            </div>
        );
    }
}