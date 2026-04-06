import React from 'react';

export default function ColumnResizeMarker(props) {
    const style = {
        left: props.left,
        top: props.top,
        height: props.height,
        display: props.isResizing ? "block" : "none"
    };

    return (
        <div className="gr-col-resize-marker" style={style} />
    );
}