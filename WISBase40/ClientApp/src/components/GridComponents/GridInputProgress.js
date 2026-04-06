import React from 'react';

const InputProgress = React.forwardRef((props, ref) => {
    return (
        <input ref={ref} defaultValue={props.value} onBlur={props.onBlur} onKeyUp={props.onKeyUp} type="number" />
    );
});

export default InputProgress;