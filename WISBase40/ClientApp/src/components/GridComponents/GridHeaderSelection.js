import React, { Component } from 'react';
import { CellSelection } from './GridCellSelection';
import { filterStatus } from '../Enums';

export class HeaderSelection extends Component {
    constructor(props) {
        super(props);

        this.headerRef = React.createRef();
    }

    handleClick = (evt) => {
        this.props.invertSelection();
    }

    getScrollLeft = () => {
        if (this.headerRef.current)
            return this.headerRef.current.scrollLeft;

        return 0;
    }

    getFilterContent = () => {
        if (this.props.filterStatus === filterStatus.closed)
            return null;

        return <div className="gr-filter-placeholder" />;
    }

    isChecked = () => {
        return this.props.isSelectionInverted && this.props.selection.length === 0;
    }

    render() {
        return (
            <div className="gr-header" ref={this.headerRef}>
                <div className="gr-header-row">
                    <CellSelection
                        isEditingEnabled={this.props.isEditingEnabled}
                        handleClick={this.handleClick}
                        checked={this.isChecked()}
                    />
                </div>
                <div className="gr-filter-row-selection">
                    {this.getFilterContent()}
                </div>
            </div>
        );
    }
}