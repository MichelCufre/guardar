import React from 'react';

export default function InputCheckbox(props) {
    return (
        <input type="checkbox" checked={props.isChecked} onClick={props.onClick} onChange={props.onChange} />
    );
}