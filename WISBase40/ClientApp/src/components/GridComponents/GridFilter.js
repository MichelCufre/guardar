import React, { Component } from 'react';
import { FilterText } from './GridFilterText';
import { filterStatus } from '../Enums';
import { FilterContent } from './GridFilterContent';

export class Filter extends Component {
    constructor(props) {
        super(props);

        this.inputRef = React.createRef();
    }

    shouldComponentUpdate(nextProps) {
        return this.props.filterStatus !== nextProps.filterStatus
            || this.props.filters !== nextProps.filters
            || this.props.width !== nextProps.width
            || this.props.highlightLast !== nextProps.highlightLast;
    }

    getValue = () => {
        const filter = this.props.filters.find(f => f.columnId === this.props.columnId);
            
        return filter ? filter.value : null;
    }

    getClassName = () => {
        const displayBorder = this.props.displayBorderLeft && this.props.isFirst ? " display-border-left" : "";

        return "gr-filter" + displayBorder;
    }

    render() {
        return (
            <div className={this.getClassName()} style={{ minWidth: this.props.width }}>
                <FilterContent
                    columnId={this.props.columnId}
                    type={this.props.type}
                    allowsFiltering={this.props.allowsFiltering}
                    value={this.getValue()}
                    shouldFocus={this.props.shouldFocus}
                    highlightLast={this.props.highlightLast}
                    updateFilter={this.props.updateFilter}
                    applyFilter={this.props.applyFilter}
                />
            </div>
        );
    }
}