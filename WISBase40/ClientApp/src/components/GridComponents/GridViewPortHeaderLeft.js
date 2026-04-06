import React from 'react';

export function ViewPortHeaderLeft(props) {
    const style = {
        minWidth: props.width,
        maxWidth: props.width
    };

    return (
        <div className="gr-vp-header-left" style={style}>
            {props.children}
        </div>
    );
}