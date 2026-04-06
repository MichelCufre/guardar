import React from 'react';
import { Form } from 'react-bootstrap';

export const CheckboxListItem = (props) => {

    const handleChange = (evt) => {
        props.onChange(evt, props.id);
    }
    return (
        <Form.Check id={"option-" + props.id} key={"option-" + props.id} type="checkbox" label={props.label} checked={props.selected} onChange={handleChange} custom />
    );
}