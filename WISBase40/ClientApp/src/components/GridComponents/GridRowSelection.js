import React, { Component } from 'react';
import { CellSelection } from './GridCellSelection';

export class RowSelection extends Component {
    shouldComponentUpdate(nextProps) {
        return this.props.isSelectionInverted !== nextProps.isSelectionInverted
            || this.props.selection !== nextProps.selection
            || this.props.readOnly !== nextProps.readOnly;
    }

    handleClick = (evt) => {
        evt.preventDefault();

        this.props.updateSelection(this.props.id);
    }

    getContent = () => {
        return (
            <CellSelection
                isEditingEnabled={this.props.isEditingEnabled}
                rowIsNew={this.props.isNew}
                readOnly={this.props.readOnly}
                handleClick={this.handleClick}
                checked={this.isChecked()}
            />
        );
    }
    getClassName = () => {
        return `gr-row ${this.props.isNew ? "new" : ""} ${this.props.cssClass}`;
    }

    isChecked = () => {
        const index = this.props.selection.indexOf(this.props.id);

        return (this.props.isSelectionInverted && index < 0)
            || (!this.props.isSelectionInverted && index >= 0);
    }

    render() {
        return (
            <div className={this.getClassName()} >
                {this.getContent()}
            </div>
        );
    }
}