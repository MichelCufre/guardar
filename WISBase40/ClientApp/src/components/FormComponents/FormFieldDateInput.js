import React, { useEffect, useRef } from 'react';
import InputMask from 'react-input-mask';

export const FieldDateInput = React.forwardRef(({ name, mask, wildcardChar, onClick, onChange, onKeyDown, onBlur, value, readOnly, disabled, ...rest }, forwardedRef) => {
    const ref = useRef(null);

    useEffect(() => {
        if (rest.totallyNotAutofocus && ref.current)
            setTimeout(() => ref.current.focus(), 100);
    }, []);

    const handleChange = (evt) => { //Hack de compatibilidad entre librerias
        if (evt.target.value === mask.replaceAll("9", wildcardChar || "_")) {
            evt.target.value = "";
        }

        onChange(evt);
    }

    if (readOnly || disabled) {
        return (
            <input
                name={name}
                ref={ref}
                value={value}
                disabled={disabled}
                readOnly={readOnly}
                className="form-control"
            />
        );
    }

    return (
        <InputMask
            name={name}
            ref={ref}
            mask={mask}
            onClick={onClick}
            onChange={handleChange}
            onKeyDown={onKeyDown}
            onBlur={onBlur}
            value={value}
            className="form-control"
        />
    );
});