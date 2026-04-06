import React, { useEffect, useRef } from 'react';
import InputMask from 'react-input-mask';

export const CellDateInput = React.forwardRef(({ mask, wildcardChar, onClick, onChange, onKeyDown, onBlur, value, ...rest }, forwardedRef) => {
    const ref = useRef(null);
    const isDraggingRef = useRef(false);

    useEffect(() => {        
        if (rest.totallyNotAutofocus && ref.current)
            setTimeout(() => ref.current.getInputDOMNode().focus(), 100);
    }, []);

    const getVoidValue = () => {
        return mask.replaceAll("9", wildcardChar || "_");
    }

    const handleChange = (evt) => { //Hack de compatibilidad entre librerias
        if (evt.target.value === getVoidValue()) {
            evt.target.value = "";
        }

        onChange(evt);
    }    

    const handleClick = (evt) => {
        if (!isDraggingRef.current)
            onClick(evt);
    }

    const handleMouseDown = () => {
        isDraggingRef.current = true;
    };

    const handleMouseUp = () => {
        isDraggingRef.current = false;
    }

    const handleKeydown = (evt) => {
        if (evt.which === 27) {
            rest.setIgnoreNextUpdate();

            evt.preventDefault();

            rest.clearEditingCell();

            rest.setFocusCell();
        }
        else if (evt.which === 9) {
            if (evt.target.value === getVoidValue() || evt.target.value.indexOf("_") > -1) {
                evt.target.value = "";
            }

            onKeyDown(evt);
        }
        else {
            onKeyDown(evt);
        }
    }

    const handleBlur = (evt) => {
        rest.clearCell();

        evt.preventDefault();
    }

    return (
        <InputMask ref={ref} mask={mask} onClick={handleClick} onMouseDown={handleMouseDown} onMouseUp={handleMouseUp} onChange={handleChange} onKeyDown={handleKeydown} onBlur={handleBlur} value={value} className="gr-date-masked-input gr-cell-content-input" />
    );
});