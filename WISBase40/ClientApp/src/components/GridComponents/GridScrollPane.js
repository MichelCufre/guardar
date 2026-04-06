import React from 'react';
import withScrollContext from './WithScrollContext';
import { ScrollSyncPane } from '../ScrollSyncComponent';

function InternalGridScrollPane(props) {
    const widthRight = props.widthRight + (props.isVScrollActive() ? props.scrollContext.scrollbarWidth : 0);

    const styleLeft = {
        height: props.scrollContext.scrollbarWidth,
        minWidth: props.widthLeft
    };

    const styleContainer = {
        height: props.scrollContext.scrollbarWidth
    };

    const styleCenter = {
        minHeight: props.scrollContext.scrollbarWidth,
        minWidth: props.widthCenter
    };

    const styleRight = {
        height: props.scrollContext.scrollbarWidth,
        minWidth: widthRight
    };

    return (
        <div className="gr-scroll-pane">
            <div className="gr-scroll-space" style={styleLeft} />
            <ScrollSyncPane group="horizontal">
                <div className="gr-scroll-container" style={styleContainer} ref={props.ref}>
                    <div style={styleCenter} />
                </div>
            </ScrollSyncPane>
            <div className="gr-scroll-space" style={styleRight} />
        </div>
    );
}

const ForwardedGridScrollPane = React.forwardRef((props, ref) => <InternalGridScrollPane forwardedRef={ref} {...props} />);

export const GridScrollPane = withScrollContext(ForwardedGridScrollPane);