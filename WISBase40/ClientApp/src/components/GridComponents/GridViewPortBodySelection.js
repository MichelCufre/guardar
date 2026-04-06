import React, { Component } from 'react';
import { BodySelection } from './GridBodySelection';
import withScrollContext from './WithScrollContext';

class InternalViewPortBodySelection extends Component {
    shouldComponentUpdate(nextProps) {
        return !nextProps.scrollContext.isScrolling;
    }

    getStyle = () => {
        const width = 28;

        return {
            minWidth: width,
            maxWidth: width
        };
    }
    getBodyStyle = (isHScrollActive, scrollbarWidth, scrollbarHeight) => {
        return {
            marginRight: -scrollbarWidth,
            marginBottom: isHScrollActive ? scrollbarHeight : 0
        };
    }

    render() {
        if (!this.props.enableSelection)
            return null;

        return (
            <div className="gr-vp-body-selection" style={this.getStyle()}>
                <BodySelection
                    rows={this.props.rows}
                    bodyStyle={this.getBodyStyle(this.props.scrollContext.isHScrollActive, this.props.scrollContext.scrollbarWidth, this.props.scrollContext.scrollbarHeight)}
                    selection={this.props.selection}
                    updateSelection={this.props.updateSelection}
                    totalHeight={this.props.totalHeight}
                    rowTopSpan={this.props.rowTopSpan}
                    rowBottomSpan={this.props.rowBottomSpan}
                    isSelectionInverted={this.props.isSelectionInverted}
                    isEditingEnabled={this.props.isEditingEnabled}
                />
            </div>
        );
    }
}

export const ViewPortBodySelection = withScrollContext(InternalViewPortBodySelection);