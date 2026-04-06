import React, { Component } from 'react';

export class ColumnSortButton extends Component {
    shouldComponentUpdate(nextProps) {
        return this.props.order !== nextProps.order
            || this.props.direction !== nextProps.direction;
    }

    handleClick = (evt) => {
        this.props.applySort(this.props.columnId);
    }

    render() {
        const className = "gr-col-sort " + (this.props.direction === 0 ? "" : this.props.direction === 1 ? "ascending" : "descending");

        if (this.props.direction === 0) {
            return (
                <div className={className}></div>
            );
        }

        if (this.props.direction === 1) {
            return (
                <div className={className}>                    
                    <div className="gr-col-sort-order">
                        {this.props.order}
                    </div>
                    <div className="gr-col-sort-marker-up">
                        <i className="fas fa-caret-up" />
                    </div>
                </div>
            );
        }

        return (
            <div className={className}>                
                <div className="gr-col-sort-order">
                    {this.props.order}
                </div>
                <div className="gr-col-sort-marker-down">
                    <i className="fas fa-caret-down" />
                </div>
            </div>
        );
    }
}