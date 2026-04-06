import React, { Component } from 'react';
import Consumer from './ScrollContextProvider';

export default function withScrollContext(WrappedComponent) {
    class WithScrollContext extends Component {
        render() {
            const { forwardedRef, ...props } = this.props;

            return (
                <Consumer>
                    {scrollContext => (
                        <WrappedComponent scrollContext={scrollContext} {...props} ref={forwardedRef} />
                    )}
                </Consumer>
            );
        }
    };

    return React.forwardRef((props, ref) => {
        return <WithScrollContext {...props} forwardedRef={ref} />;
    });
}