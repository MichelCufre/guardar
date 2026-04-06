import React from 'react';
import withScrollContext from './WithScrollContext';

function InternalViewPortHeaderRight(props) {
    const width = props.width + (props.isVScrollActive() ? props.scrollContext.scrollbarWidth : 0);
    
    const style = {
        minWidth: width,
        maxWidth: width
    };
    
    return (
        <div className="gr-vp-header-right" style={style}>
            {props.children}
        </div>
    );
}

export const ViewPortHeaderRight = withScrollContext(InternalViewPortHeaderRight);