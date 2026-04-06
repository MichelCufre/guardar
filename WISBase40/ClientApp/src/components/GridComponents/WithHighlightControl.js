import React, { Component } from 'react';

export function withHighlightControl(WrappedComponent) {
    return class WithHighlightControl extends Component {
        

        render() {
            return (
                <WrappedComponent
                    navigation={{
                        handleLeft: this.handleLeft,
                        handleRight: this.handleRight,
                        handleUp: this.handleUp,
                        handleDown: this.handleDown,
                        handleTab: this.handleTab
                    }}
                    {...this.props}
                />
            );
        }
    };
}