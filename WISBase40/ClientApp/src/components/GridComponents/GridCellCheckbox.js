import React, { Component } from 'react';
import { withKeyNavigation } from './WithKeyNavigation';
import { GridCellEditControl } from './GridCellEditControl';

export class InternalCellCheckbox extends Component {
    constructor(props) {
        super(props);

        this.cellRef = React.createRef();
    }

    shouldComponentUpdate(nextProps) {
        return this.props.content.value !== nextProps.content.value
            || this.props.content.old !== nextProps.content.old
            || this.props.content.old !== this.props.content.value
            || nextProps.content.old !== nextProps.content.value;
    }

    handleChange = (event) => {
        if (this.props.isEditingEnabled && this.props.content.editable) {
            const value = event.target.checked ? "S" : "N";

            this.props.updateCellValue(this.props.rowId, this.props.column.id, value);
        }
    }
    handleKeydown = (evt) => {
        switch (evt.which) {
            case 35:
                this.props.navigation.handleEnd(evt.target);
                evt.preventDefault();
                break;
            case 36:
                this.props.navigation.handleHome(evt.target);
                evt.preventDefault();
                break;
            case 37:
                this.props.navigation.handleLeft(evt.target);
                evt.preventDefault();
                break;
            case 39:
                this.props.navigation.handleRight(evt.target);
                evt.preventDefault();
                break;
            case 38:
                this.props.navigation.handleUp(evt.target);
                evt.preventDefault();
                break;
            case 40:
                this.props.navigation.handleDown(evt.target);
                evt.preventDefault();
                break;
            case 13:
                this.props.navigation.handleDown(evt.target);
                evt.preventDefault();
                break;
            case 9:
                this.handleTab(evt);
                evt.preventDefault();
                break;
        }
    }

    handleTab = (evt) => {
        const shiftKey = evt.shiftKey;

        if (shiftKey)
            this.props.moveToPreviousEditableCell(this.props.rowId, this.props.column.id, () => this.cellRef.current.parentElement.classList.toggle("selected", false));
        else
            this.props.moveToNextEditableCell(this.props.rowId, this.props.column.id, () => this.cellRef.current.parentElement.classList.toggle("selected", false));
    }

    addHighlight = (evt) => {
        this.props.toggleCellHighlight(evt.shiftKey);
    }

    isChecked = () => {
        if (this.props.content.value)
            return this.props.content.value.toUpperCase() === "S";

        return false;
    }

    render() {
        if (this.props.isEditingEnabled) {
            return (
                <div className="gr-cell-checkbox-container">
                    <input
                        ref={this.cellRef}
                        type="checkbox"
                        checked={this.isChecked()}
                        onFocus={this.addHighlight}
                        onChange={this.handleChange}
                        onKeyDown={this.handleKeydown}
                    />
                </div>
            );
        }
        else {
            return (
                <div className="gr-cell-checkbox-container">
                    <input
                        ref={this.cellRef}
                        type="checkbox"
                        checked={this.isChecked()}
                        onFocus={this.addHighlight}
                        onKeyDown={this.handleKeydown}
                        readOnly
                    />
                </div>
            );
        }
    }
}

export const CellCheckbox = withKeyNavigation(InternalCellCheckbox);