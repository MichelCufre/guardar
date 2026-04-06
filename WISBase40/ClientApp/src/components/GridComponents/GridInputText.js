import React from 'react';

const InputText = React.forwardRef((props, ref) => {
    return (
        <input
            ref={ref}
            defaultValue={props.value}
            onBlur={props.onBlur}
            onChange={props.onChange}
            onKeyDown={props.onKeyDown}
            style={props.style}
            className={props.className}
        />
    );
});

export default InputText;