import React, { useState, useRef, useEffect, useLayoutEffect } from 'react';

export function GridCellSelectMenu(props) {
    const elementRef = useRef(null);
    const [left, setLeft] = useState(0);
    const [top, setTop] = useState(0);

    let menuStyle = {};

    useLayoutEffect(d => {
        const menuWidth = elementRef.current.clientWidth;

        setLeft((props.siblingsWidth < menuWidth) ? props.positionLeft - (menuWidth - props.parentWidth) : props.positionLeft);
        setTop(props.positionTop);
    }, [elementRef]);

    menuStyle = {
        left: left,
        top: top
    };

    return (
        <div ref={elementRef} className="gr-cell-select-menu" style={menuStyle}>{props.children}</div>
    );
}