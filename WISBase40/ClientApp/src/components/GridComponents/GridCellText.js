import React, { Component } from 'react';
import { textAlign } from '../Enums';
import { withKeyNavigation } from './WithKeyNavigation';
import { navigationRestrictedKeys } from '../Constants';
import { GridCellEditControl } from './GridCellEditControl';
import { CellValue } from './GridCellValue';

class InternalCellText extends Component {
    constructor(props) {
        super(props);

        this.valueOnHold = false;
        this.ignoreNextUpdate = false;

        this.inputRef = React.createRef();
        this.cellRef = React.createRef();

        this.state = {
            shouldFocus: false
        };
    }

    shouldComponentUpdate(nextProps, nextState) {
        return this.props.content.value !== nextProps.content.value
            || this.props.content.old !== nextProps.content.old
            || this.props.content.editable !== nextProps.content.editable
            || this.props.editing !== nextProps.editing
            || (nextState.shouldFocus && this.state.shouldFocus !== nextState.shouldFocus)
            || this.props.content.old !== this.props.content.value
            || nextProps.content.old !== nextProps.content.value;
    }

    componentDidUpdate() {
        if (this.props.editing) {
            if (this.inputRef.current)
                this.inputRef.current.focus();
        }
        else {
            if (this.state.shouldFocus) {
                this.setState({
                    shouldFocus: false
                }, () => this.cellRef.current.focus());
            }
        }
    }

    handleDoubleClick = (evt) => {
        this.toggleEditable(evt);
    }
    handleClick = (evt) => {
        if (!this.props.rowIsNew && evt.ctrlKey)
            this.props.openRelatedLink(this.props.rowId, this.props.column.id);
    }
    handleInputKeydown = (evt) => {
        if (evt.which === 40) {
            this.handleDown(evt);

            evt.preventDefault();
        }                
        else if (evt.which === 38) {
            this.handleUp(evt);

            evt.preventDefault();
        }
        else if (evt.which === 9) {
            this.handleTab(evt);            

            evt.preventDefault();
        }
        else if (evt.which === 36) {
            this.handleHome(evt);

            evt.preventDefault();
        }
        else if (evt.which === 35) {
            this.handleEnd(evt);

            evt.preventDefault();
        }
        else if (evt.which === 27 && this.inputRef) {
            this.ignoreNextUpdate = true;

            evt.preventDefault();

            this.inputRef.current.blur();
        }
        else if (evt.which === 13 && this.inputRef) {
            this.handleEnter(evt);

            evt.preventDefault();
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
            case 13:
                this.props.navigation.handleDown(evt.target);
                evt.preventDefault();
                break;
            case 9:
                this.handleTabNonEditing(evt);
                evt.preventDefault();
                break;            
            case 8: this.handleBackspace(evt);
                break;
            default: this.handleDefaultKey(evt);
                break;
        }
    }

    handleBlur = (evt) => {
        if (!this.ignoreNextUpdate)
            this.props.updateCellValue(this.props.rowId, this.props.column.id, evt.target.value);

        this.ignoreNextUpdate = false;

        this.props.clearEditingCell(() => {
            this.setState({
                shouldFocus: true
            });
        });
    }

    handleDefaultKey = (evt) => {
        if (this.props.isEditingEnabled && !evt.ctrlKey && navigationRestrictedKeys.indexOf(evt.which) === -1 && evt.key !== "Dead") { //Quitar teclas que no son letras ni simbolos
            this.valueOnHold = evt.key;

            this.toggleEditable(evt);
        }
    }
    handleBackspace = (evt) => {
        if (this.props.isEditingEnabled && evt.key !== "Dead") { //Quitar teclas que no son letras ni simbolos
            this.valueOnHold = "";

            this.toggleEditable(evt);
        }
    }
    handleTab = (evt) => {
        const shiftKey = evt.shiftKey;

        this.props.updateCellValue(this.props.rowId, this.props.column.id, evt.target.value);

        if (shiftKey)
            this.props.moveToPreviousEditableCell(this.props.rowId, this.props.column.id);
        else
            this.props.moveToNextEditableCell(this.props.rowId, this.props.column.id);
    }
    handleTabNonEditing = (evt) => {
        const shiftKey = evt.shiftKey;

        if (shiftKey) {
            this.props.moveToPreviousEditableCell(this.props.rowId, this.props.column.id, () => this.cellRef.current.parentElement.classList.toggle("selected", false));
        }
        else
            this.props.moveToNextEditableCell(this.props.rowId, this.props.column.id, () => this.cellRef.current.parentElement.classList.toggle("selected", false));
    }
    handleUp = (evt) => {
        this.props.updateCellValue(this.props.rowId, this.props.column.id, evt.target.value);

        this.props.clearEditingCell(() => {
            this.props.navigation.handleUp(this.cellRef.current);
        });
    }
    handleDown = (evt) => {
        this.props.updateCellValue(this.props.rowId, this.props.column.id, evt.target.value);

        this.props.clearEditingCell(() => {
            this.props.navigation.handleDown(this.cellRef.current);
        });
    }
    handleHome = (evt) => {
        this.props.updateCellValue(this.props.rowId, this.props.column.id, evt.target.value);

        this.props.clearEditingCell(() => {
            this.props.navigation.handleHome(this.cellRef.current);
        });
        
    };
    handleEnd = (evt) => {
        this.props.updateCellValue(this.props.rowId, this.props.column.id, evt.target.value);

        this.props.clearEditingCell(() => {
            this.props.navigation.handleEnd(this.cellRef.current);
        });
    };
    handleEnter = (evt) => {
        this.props.updateCellValue(this.props.rowId, this.props.column.id, evt.target.value);

        if (this.props.rowIsNew) {
            this.props.moveToNewRowEditableCell();
        }
        else {
            this.props.clearEditingCell(() => {
                if (!this.props.navigation.handleDown(this.cellRef.current)) {
                    this.setState({
                        shouldFocus: true
                    });
                }
            });
        }
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
    getCellClassName = () => {
        if (this.props.content.value !== this.props.content.old) {
            return "gr-cell-content-text edited";
        }

        return "gr-cell-content-text";
    }

    render() {
        if (!this.props.isEditingEnabled || !this.props.editing || !this.props.content.editable) {
            return (
                <div
                    ref={this.cellRef}
                    onDoubleClick={this.handleDoubleClick}
                    onClick={this.handleClick}
                    className={this.getCellClassName()}
                    onKeyDown={this.handleKeydown}
                    onFocus={this.addHighlight}
                    tabIndex="0"
                >
                    <GridCellEditControl
                        isEditingEnabled={this.props.isEditingEnabled}
                        isEditing={this.props.editing}
                        editable={this.props.content.editable}
                        onClickEdit={this.toggleEditable}
                    >
                        <CellValue value={this.props.content.value} translator={this.props.translator} translate={this.props.column.translate} />
                    </GridCellEditControl>
                </div>
            );
        }

        const value = this.valueOnHold || this.valueOnHold === "" ? this.valueOnHold : this.props.content.value;

        this.valueOnHold = false;

        return (
            <input
                ref={this.inputRef}
                defaultValue={value}
                onBlur={this.handleBlur}
                onKeyDown={this.handleInputKeydown}
                className={"gr-cell-content-input"}
            />
        );
    }
}

export const CellText = withKeyNavigation(InternalCellText);