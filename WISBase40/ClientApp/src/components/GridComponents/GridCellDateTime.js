import React, { Component } from 'react';
import { textAlign } from '../Enums';
import DatePicker from 'react-datepicker';
import { getDateTimeString } from '../DateTimeUtil';
import { withKeyNavigation } from './WithKeyNavigation';
import { navigationRestrictedKeys } from '../Constants';
import { GridCellEditControl } from './GridCellEditControl';
import { CellDateInput } from './GridCellDateInput';

class InternalCellDateTime extends Component {
    constructor(props) {
        super(props);

        this.cellRef = React.createRef();
        this.dateRef = React.createRef();
        this.inputRef = React.createRef();
        this.ignoreNextUpdate = false;

        this.state = {
            focusCell: false
        };
    }

    shouldComponentUpdate(nextProps, nextState) {
        return this.props.isEditingEnabled
            || this.props.editing !== nextProps.editing
            || this.props.content.value !== nextProps.content.value
            || this.props.content.old !== nextProps.content.old
            || this.props.content.metadata !== nextProps.content.metadata
            || this.props.content.old !== this.props.content.value
            || nextProps.content.old !== nextProps.content.value;
    }

    componentDidUpdate() {
        if (this.state.focusCell) {
            this.cellRef.current.focus();

            this.setState({
                focusCell: false
            });
        }
    }

    handleDblClick = (evt) => {
        this.toggleEditable(evt);
    }
    handleChange = (date) => {
        if (!this.ignoreNextUpdate) {
            this.ignoreNextUpdate = false;

            const isoDate = date ? date.toISOString() : "";

            this.props.updateCellValue(this.props.rowId, this.props.column.id, isoDate);
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
    handleInputKeydown = (evt) => {
        if (evt.which === 9) {
            if (evt.target.value === "")
                this.props.updateCellValue(this.props.rowId, this.props.column.id, evt.target.value);

            if (evt.shiftKey) {
                this.props.moveToPreviousEditableCell(this.props.rowId, this.props.column.id);
            }
            else {
                this.props.moveToNextEditableCell(this.props.rowId, this.props.column.id);
            }

            evt.preventDefault();
        }
        else if (evt.which === 13) {
            if (this.props.rowIsNew) {
                this.props.moveToNewRowEditableCell();
            }
            else {
                this.props.clearEditingCell();

                if (!this.props.navigation.handleDown(evt.target.parentElement.parentElement)) {
                    this.setFocusCell();
                }
            }

            evt.preventDefault();
        }
        else if (evt.which === 40 || evt.which === 38) {
            evt.preventDefault();
        }
        else if (evt.which === 36) {
            this.props.clearEditingCell(() => this.props.navigation.handleHome(this.cellRef.current));

            evt.preventDefault();
        }
        else if (evt.which === 35) {
            this.props.clearEditingCell(() => this.props.navigation.handleEnd(this.cellRef.current));

            evt.preventDefault();
        }
    }

    handleDefaultKey = (evt) => {
        if (this.props.isEditingEnabled && navigationRestrictedKeys.indexOf(evt.which) === -1 && evt.key !== "Dead") { //Quitar teclas que no son letras ni simbolos
            this.toggleEditable(evt);
        }
    }
    handleTab = (evt) => {
        const shiftKey = evt.shiftKey;

        if (shiftKey)
            this.props.moveToPreviousEditableCell(this.props.rowId, this.props.column.id, () => {
                if (!this.props.editing) {
                    this.cellRef.current.parentElement.classList.toggle("selected", false);
                }
            });
        else
            this.props.moveToNextEditableCell(this.props.rowId, this.props.column.id, () => {
                if (!this.props.editing) {
                    this.cellRef.current.parentElement.classList.toggle("selected", false);
                }
            });
    }
    handleBlur = (evt) => {
        if (evt.target && !evt.target.classList.contains("gr-date-masked-input"))
            this.props.clearEditingCell();
    }

    clearCell = () => {
        if (!this.dateRef.current || !this.dateRef.current.isCalendarOpen()) {
            this.props.clearEditingCell();

            this.setState({
                focusCell: true
            });
        }
    } 

    setIgnoreNextUpdate = () => {
        this.ignoreNextUpdate = true;
    }

    addHighlight = (evt) => {
        this.props.toggleCellHighlight(evt.shiftKey);
    }

    toggleEditable = (evt) => {
        evt.preventDefault();

        if (!this.props.editing) {
            this.props.setEditingCell(this.props.rowId, this.props.column.id);
        }
        else {
            this.props.clearEditingCell();
        }
    }

    getCellClassName = () => {
        if (this.props.content.value !== this.props.content.old) {
            return "gr-cell-content-text edited";
        }

        return "gr-cell-content-text";
    }

    render() {
        const date = this.props.content.value ? new Date(this.props.content.value) : null;
        const value = date ? getDateTimeString(date) : "";
        const className = this.getCellClassName();

        if (!this.props.isEditingEnabled || !this.props.content.editable) {
            return (
                <div
                    ref={this.cellRef}
                    className={className}
                    onKeyDown={this.handleKeydown}
                    onFocus={this.addHighlight}
                    tabIndex={0}
                >
                    <GridCellEditControl
                        isEditingEnabled={this.props.isEditingEnabled}
                        editable={this.props.content.editable}
                    >
                        {value}
                    </GridCellEditControl>
                </div>
            );
        }
        else {
            if (this.props.editing) {
                return (
                    //Si se quiere aplicar al seleccionar, usar onSelect con handleBlur
                    <DatePicker
                        ref={this.dateRef}
                        selected={date}
                        showTimeInput
                        timeInputLabel="Hora: "
                        dateFormat="dd/MM/yyyy HH:mm:ss"
                        timeFormat="HH:mm"
                        onChange={this.handleChange}
                        onKeyDown={this.handleInputKeydown}
                        shouldCloseOnSelect={false}
                        onClickOutside={this.handleBlur}
                        customInput={<CellDateInput ref={this.inputRef} mask="99/99/9999 99:99:99" clearCell={this.clearCell} setIgnoreNextUpdate={this.setIgnoreNextUpdate} totallyNotAutofocus={true} />}
                    />
                );
            }
            else {
                return (
                    <div
                        ref={this.cellRef}
                        className={className}
                        onDoubleClick={this.handleDblClick}
                        onKeyDown={this.handleKeydown}
                        onFocus={this.addHighlight}
                        tabIndex={0}
                    >
                        <GridCellEditControl
                            isEditingEnabled={this.props.isEditingEnabled}
                            editable={this.props.content.editable}
                        >
                            {value}
                        </GridCellEditControl>
                    </div>
                );
            }
        }
    }
}

export const CellDateTime = withKeyNavigation(InternalCellDateTime);