import React from 'react';
import { Form } from 'react-bootstrap';
import { connect } from 'formik';
import { withFormContext } from './WithFormContext';

function FieldCheckboxListItemInternal(props) {

    const handleChange = (evt) => {
        props.onChange(evt, props.id);
    }
    return (
        <Form.Check id={"option-" + props.id} key={"option-" + props.id} type="checkbox" label={props.label} checked={props.selected} onChange={handleChange} custom />
    );
}

export const FieldCheckboxListItem = withFormContext(connect(FieldCheckboxListItemInternal));
