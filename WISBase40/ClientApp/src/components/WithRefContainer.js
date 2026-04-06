import React, { Component } from 'react';

export default function withRefContainer(WrappedComponent) {
    class WithRefContainer extends Component {
        render() {
            const { forwardedRef, ...rest } = this.props;

            return (
                <div ref={forwardedRef}>
                    <WrappedComponent {...rest} />
                </div>
            );
        }
    }

    return React.forwardRef((props, ref) => {
        return <WithRefContainer forwardedRef={ref} {...props} />;
    });
}