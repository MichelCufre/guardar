import React, { Component } from 'react';
import { textAlign } from '../Enums';
import { withKeyNavigation } from './WithKeyNavigation';

export class InternalCellToggle extends Component {
    constructor(props) {
        super(props);

        this.cellRef = React.createRef();
    }

    shouldComponentUpdate(nextProps) {
        return this.props.content.value !== nextProps.content.value
            || this.props.content.old !== nextProps.content.old
            || this.props.editing !== nextProps.editing
            || this.props.content.old !== this.props.content.value
            || nextProps.content.old !== nextProps.content.value;
    }

    componentDidUpdate(prevProps) {
        if (this.props.editing && prevProps.editing != this.props.editing && this.cellRef.current)
            this.cellRef.current.focus();
    }

    updateValue = (event) => {
        event.preventDefault();

        if (this.props.isEditingEnabled && this.props.content.editable) {
            this.props.updateCellValue(this.props.rowId, this.props.column.id, this.props.content.value === "S" ? "N" : "S");
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
            case 37: this.props.navigation.handleLeft(evt.target);
                evt.preventDefault();
                break;
            case 39: this.props.navigation.handleRight(evt.target);
                evt.preventDefault();
                break;
            case 38: this.props.navigation.handleUp(evt.target);
                evt.preventDefault();
                break;
            case 40: this.props.navigation.handleDown(evt.target);
                evt.preventDefault();
                break;
            case 13: this.props.navigation.handleDown(evt.target);
                evt.preventDefault();
                break;
            case 9: this.handleTab(evt);
                evt.preventDefault();
                break;
            case 32: this.updateValue(evt);
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

    getStyle = () => {
        let align;

        switch (this.props.column.textAlign) {
            case textAlign.center:
                align = "center";
                break;
            case textAlign.right:
                align = "right";
                break;
            default:
                align = "left";
        }

        return {
            textAlign: align
        };
    }

    isChecked = () => {
        if (this.props.content.value)
            return this.props.content.value.toUpperCase() === "S";

        return false;
    }

    render() {
        const containerClassName = `gr-cell-toggle ${ this.isChecked() ? "" : "toggle-off" }`;
        const className = `fas ${this.isChecked() ? "fa-toggle-on" : "fa-toggle-off"}`;
        const style = this.getStyle();

        if (this.props.isEditingEnabled) {
            return (
                <div
                    ref={this.cellRef}
                    className={containerClassName}
                    onFocus={this.addHighlight}
                    style={style}
                    onKeyDown={this.handleKeydown}
                    tabIndex="-1"
                >
                    <a onClick={this.updateValue}>
                        <i className={className} />
                    </a>
                </div>
            );
        }
        else {
            return (
                <div
                    ref={this.cellRef}
                    className={containerClassName}
                    onFocus={this.addHighlight} 
                    style={style}
                    onKeyDown={this.handleKeydown}
                >
                    <a>
                        <i className={className} />
                    </a>
                </div>
            );
        }
    }
}

export const CellToggle = withKeyNavigation(InternalCellToggle);