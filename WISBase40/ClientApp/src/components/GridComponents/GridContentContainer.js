import React, { Component } from 'react';

class InternalGridContentContainer extends Component {
    shouldComponentUpdate() {
        return !this.props.isUpdatingStructure;
    }

    handleMouseUp = () => {
        if (this.props.isResizing)
            this.props.columnResizeEnd();
    }
    handleMouseMove = (evt) => {
        if (!this.props.isResizing)
            return false;

        this.props.columnResizeChange(evt.clientX);
        
    }
    handleMouseLeave = () => {
        if (this.props.isResizing)
            this.props.columnResizeEnd();
    }
    handleKeyDown = (evt) => {
        if (evt.ctrlKey && evt.which === 70) {
            this.props.toggleFilterBar();

            evt.preventDefault();
        }
        else if (evt.which === 27) {
            if (evt.target.closest(".gr-filter") && !evt.target.closest(".filter-toggle-select__value-container")) {
                this.props.closeFilterBar();

                evt.preventDefault();
            }
        }
        else if (evt.ctrlKey && evt.which === 13) {
            this.props.moveToNewRowEditableCell();
        }
        else if (evt.ctrlKey && evt.which === 46) {
            this.props.deleteRow();
        }
    }

    render() {
        return (
            <div
                className="gr-content-container"                
                ref={this.props.forwardedRef}
                onKeyDown={this.handleKeyDown}
                onMouseUp={this.handleMouseUp}
                onMouseMove={this.handleMouseMove}
                onMouseLeave={this.handleMouseLeave}
                tabIndex="0"
            >
                {this.props.children}
            </div>
        );
    };
}

export const GridContentContainer = React.forwardRef((props, forwardedRef) => {
    return (
        <InternalGridContentContainer forwardedRef={forwardedRef} {...props}>
            {props.children}
        </InternalGridContentContainer>
    );
});