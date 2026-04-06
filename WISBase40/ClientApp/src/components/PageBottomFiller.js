import React, { useRef, useImperativeHandle, forwardRef } from 'react';

function PageBottomFillerInternal(props, ref) {
    const divRef = useRef();

    useImperativeHandle(ref, () => ({
        getBoundingClientRect: () => divRef.current.getBoundingClientRect()
    }));

    const style = {
        height: props.height
    };

    return (
        <div ref={divRef} style={style} />
    );
}

export const PageBottomFiller = forwardRef(PageBottomFillerInternal);