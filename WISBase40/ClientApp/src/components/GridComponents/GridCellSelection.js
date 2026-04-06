import React, { Component } from 'react';

export class CellSelection extends Component {
    getClassName = () => {
        const checked = this.props.checked ? "checked" : "";

        return "gr-selection-checkbox " + checked;
    }
    getIcon = () => {
        if (this.props.checked) {
            return "fas fa-check-square";
        }

        return "fas fa-square";
    }

    render() {
        if (this.props.rowIsNew) {
            return <div className="gr-selection-placeholder" />;
        }

        if (this.props.readOnly) {
            return (<div> <i className="gr-selection-checkbox disabledSelection fas fa-times" /></div>);
        }
        else {
            return (
                <div className={this.getClassName()} onClick={this.props.handleClick}>
                    <i className={this.getIcon()} />
                </div>
            );
        }
    }
}