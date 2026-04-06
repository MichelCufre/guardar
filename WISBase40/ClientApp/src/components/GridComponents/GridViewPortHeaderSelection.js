import React from 'react';

function ViewPortHeaderSelection(props) {
    return (
        <div className="gr-vp-header-selection" style={props.style}>
            {props.children}
        </div>
    );
}

export { ViewPortHeaderSelection };