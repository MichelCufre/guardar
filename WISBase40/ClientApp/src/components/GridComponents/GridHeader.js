import React, { Component } from 'react';
import { Column } from './GridColumn';
import { Filter } from './GridFilter';
import { filterStatus } from '../Enums';

export class Header extends Component {
    constructor(props) {
        super(props);

        this.headerRef = React.createRef();
    }

    shouldComponentUpdate(nextProps) {
        return this.props.columns !== nextProps.columns
            || this.props.filterStatus !== nextProps.filterStatus
            || this.props.filters !== nextProps.filters
            || this.props.sorts !== nextProps.sorts
            || this.props.highlightLast !== nextProps.highlightLast
            || this.props.shouldAddFilterFiller !== nextProps.shouldAddFilterFiller;
    }
    
    getScrollLeft = () => {
        if (this.headerRef.current)
            return this.headerRef.current.scrollLeft;

        return 0;
    }

    getColumns = () => {
        let columns = this.props.columns.map((col, index) => (
            <Column
                key={col.id}
                columnId={col.id}
                name={col.name}
                width={col.width}
                order={col.order}
                sorts={this.props.sorts}
                fixed={col.fixed}
                editable={col.editable}
                insertable={col.insertable}
                allowsSorting={col.allowsSorting}
                displayBorderLeft={this.props.displayBorderLeft}
                applySort={this.props.applySort}
                applySortAscending={this.props.applySortAscending}
                applySortDescending={this.props.applySortDescending}
                applySortReset={this.props.applySortReset}
                getScrollLeft={this.getScrollLeft}
                resizeLeft={this.props.resizeLeft}
                columnResizeBegin={this.props.columnResizeBegin}
                updateColumnOrder={this.props.updateColumnOrder}
                fixColumn={this.props.fixColumn}
                hideColumn={this.props.hideColumn}
                isFirst={index === 0}
                isResizing={this.props.isResizing}

                setDraggedColumn={this.props.setDraggedColumn}
                getDraggedColumn={this.props.getDraggedColumn}
            />
        ));

        if (this.props.shouldAddFillers) {
            columns = [...columns, <div key="headerFiller" className="gr-header-filler"></div>];
        }

        return columns;
    }
    getFilters = () => {
        if (this.props.filterStatus === filterStatus.closed)
            return null;

        let currentIndex = -1;

        let filters = this.props.columns.map((col, index) => (
            <Filter
                key={col.id}
                columnId={col.id}
                type={col.type}
                allowsFiltering={col.allowsFiltering}
                width={col.width}
                filters={this.props.filters}
                filterStatus={this.props.filterStatus}
                isFirst={index === 0}
                shouldFocus={currentIndex === index}
                displayBorderLeft={this.props.displayBorderLeft}
                highlightLast={this.props.highlightLast}
                updateFilter={this.props.updateFilter}
                applyFilter={this.props.applyFilter}
            />
        ));

        if (this.props.shouldAddFillers) {
            filters = [...filters, <div key="filterFiller" className="gr-filter-filler"></div>]
        }

        return filters;
    }

    render() {
        return (
            <div className="gr-header" ref={this.headerRef}>
                <div className="gr-header-row">
                    {this.getColumns()}
                </div>
                <div className="gr-filter-row">
                    {this.getFilters()}
                </div>
            </div>
        );       
    }
}