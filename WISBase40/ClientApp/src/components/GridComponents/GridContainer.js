import React, { Component } from 'react';

export class InternalGridContainer extends Component {
    getStyle = () => {
        return {
            height: this.props.isInitializing ? this.props.height : "auto"
        };
    }

    render() {
        return (
            <div
                id={this.props.id}
                className="gr-grid"
                style={this.getStyle()}
                ref={this.props.forwardedRef}
            >
                {this.props.children}
            </div>
        );
    }
}

export const GridContainer = React.forwardRef((props, forwardedRef) => {
    return (
        <InternalGridContainer forwardedRef={forwardedRef} {...props}>
            {props.children}
        </InternalGridContainer>
    );
});