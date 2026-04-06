import React from 'react';
import InputMask from 'react-input-mask';

export const MaskedInput = React.forwardRef(({ mask, field, form, ...props }, forwardedRef) => {
    return (
        <InputMask ref={forwardedRef} mask={mask} {...props} />
    );
});