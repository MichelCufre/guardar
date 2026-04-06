import React from 'react';

export const GridCellSelectButton = React.forwardRef((props, ref) => {
    const buttonClassName = `gr-cell-select-button ${props.content.value !== props.content.old ? " edited" : ""} ${props.className || ""}`;

    return (
        <div
            ref={ref}
            onClick={props.onClick}
            className={buttonClassName}
            onFocus={props.onFocus}
            onKeyDown={props.onKeyDown}
            title={props.title}
            tabIndex="0"
        >
            <div className="gr-cell-select-content">
                {props.children}
            </div>
            <div className="gr-cell-select-icon">
                <i className="fas fa-chevron-down" />
            </div>
        </div>
    );
});