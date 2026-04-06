import React, { Component } from 'react';
import { withKeyNavigation } from './WithKeyNavigation';
import { navigationRestrictedKeys } from '../Constants';

class InternalCellProgress extends Component {
    constructor(props) {
        super(props);

        this.valueOnHold = false;
        this.ignoreNextFocus = false;
        this.ignoreNextUpdate = false;

        this.inputRef = React.createRef();
        this.cellRef = React.createRef();
    }

    shouldComponentUpdate(nextProps, nextState) {
        return this.props.content.value !== nextProps.content.value
            || this.props.content.old !== nextProps.content.old
            || this.props.editing !== this.props.editing
            || this.props.content.old !== this.props.content.value
            || nextProps.content.old !== nextProps.content.value;
    }
    componentDidUpdate() {
        if (this.props.editing) {
            setTimeout(() => this.inputRef.current.focus(), 100); //Bug de firefox
        }
        else {
            if (!this.ignoreNextFocus)
                setTimeout(() =>this.cellRef.current.focus(), 100);
        }

        this.ignoreNextFocus = false;
    }

    handleDoubleClick = (evt) => {
        this.toggleEditable(evt);
    }    
        
    handleBlur = (event) => {
        this.props.updateCellValue(this.props.rowId, this.props.column.id, event.target.value);

        this.props.clearEditingCell();
    }
    handleInputKeydown = (evt) => {
        if (evt.which === 40) {
            this.ignoreNextFocus = this.props.navigation.handleDown(evt);
        }
        else if (evt.which === 38) {
            this.ignoreNextFocus = this.props.navigation.handleUp(evt);
        }
        else if (evt.which === 9) {
            this.ignoreNextFocus = true; //Testear esto
            this.handleInputTab(evt);
        }
        else if (evt.which === 27 && this.inputRef) {
            this.ignoreNextUpdate = true;

            evt.preventDefault();

            this.inputRef.current.blur();
        }
        else if (evt.which === 13 && this.inputRef) {
            evt.preventDefault();

            this.inputRef.current.blur();
        }
        else if (evt.which === 36) {
            this.props.clearEditingCell(() => this.props.navigation.handleHome(this.cellRef.current));
        }
        else if (evt.which === 37) {
            this.props.clearEditingCell(() => this.props.navigation.handleEnd(this.cellRef.current));
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
            default: this.handleDefaultKey(evt);
                break;
        }
    }

    handleDefaultKey = (evt) => {
        if (this.props.isEditingEnabled && navigationRestrictedKeys.indexOf(evt.which) === -1 && evt.key !== "Dead") { //Quitar teclas que no son letras ni simbolos
            this.valueOnHold = evt.key;

            this.toggleEditable(evt);
        }
    }
    handleTab = (evt) => {
        const shiftKey = evt.shiftKey;

        if (shiftKey)
            this.props.moveToPreviousEditableCell(this.props.rowId, this.props.column.id, () => this.cellRef.current.parentElement.classList.toggle("selected", false));
        else
            this.props.moveToNextEditableCell(this.props.rowId, this.props.column.id, () => this.cellRef.current.parentElement.classList.toggle("selected", false));
    }
    handleTabInput = (evt) => {
        const shiftKey = evt.shiftKey;

        this.props.updateCellValue(this.props.rowId, this.props.column.id, evt.target.value);

        if (shiftKey)
            this.props.moveToPreviousEditableCell(this.props.rowId, this.props.column.id);
        else
            this.props.moveToNextEditableCell(this.props.rowId, this.props.column.id);
    }

    addHighlight = (evt) => {
        this.props.toggleCellHighlight(evt.shiftKey);
    }

    toggleEditable = (evt) => {
        if (this.props.isEditingEnabled && this.props.content.editable) {
            evt.preventDefault();

            this.props.setEditingCell(this.props.rowId, this.props.column.id);
        }
    }

    getStyle(value) {
        return {
            width: value + "%"
        };
    }
    getClassName() {
        const status = +this.props.content.value < 30 ? "low"
            : +this.props.content.value < 80 ? "mid"
            : "high";

        return "gr-cell-progress " + status;
    }

    render() {
        if (!this.props.isEditingEnabled || !this.props.editing || !this.props.content.editable) {
            return (
                <div
                    ref={this.cellRef}
                    onDoubleClick={this.handleDoubleClick}
                    onKeyDown={this.handleKeydown}
                    className={this.getClassName()}
                    onFocus={this.addHighlight}
                    tabIndex={0}
                >
                    <div style={this.getStyle(this.props.content.value)} />
                </div>
            );
        }

        const value = this.valueOnHold ? this.valueOnHold : this.props.content.value;

        this.valueOnHold = false;

        return (
            <input
                ref={this.inputRef}
                defaultValue={value}
                onBlur={this.handleBlur}
                onKeyDown={this.handleInputKeydown}
                type="number"
            />
        );
    }
}

export const CellProgress = withKeyNavigation(InternalCellProgress);