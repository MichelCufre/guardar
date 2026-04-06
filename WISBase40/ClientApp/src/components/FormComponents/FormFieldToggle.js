import React, { useEffect, useLayoutEffect, useRef } from 'react';
import { connect, getIn } from 'formik';
import { withFormContext } from './WithFormContext';
import { Form as FormBootstrap } from 'react-bootstrap';
import { fieldType } from '../Enums';

function FieldToggleInternal(props) {
    const fieldData = {
        props: {
            name: props.name,
            hidden: props.hidden,
            readOnly: props.readOnly,
            disabled: props.disabled,
            size: props.size
        }
    };

    useLayoutEffect(() => props.formProps.registerField(fieldData, fieldType.checkbox), []);

    useEffect(() => {
        return () => props.formProps.unregisterField(props.name);
    }, []);

    const updateValue = async (value) => {
        await props.formik.setFieldValue(props.name, value, false);
        await props.formProps.validateField(props.name, value);
    };

    const handleChange = async (evt) => {
        if (evt.target.classList.contains("form-check-input")) {
            let newVal = evt.target.checked;
            await updateValue(newVal);
            if (props.onChange)
                props.onChange(newVal);
        }
        
    };

    const fieldProps = props.formProps.getFieldProps(props.name);

    const error = getIn(props.formik.errors, props.name);
    const touch = getIn(props.formik.touched, props.name);
    const value = getIn(props.formik.values, props.name) ? true : false;

    const errorEmpty = touch && error && !(error && (typeof error === "object") && !Object.keys(error).length);

    const classValid = (touch && error && errorEmpty ? " is-invalid" : (errorEmpty ? '' : (touch ? " is-valid" : "")));

    const className = `form-field-toggle ${classValid} ${props.className}`;

    return (
        <FormBootstrap.Check
            id={props.name}
            className={className}
            type="switch"
            checked={value}
            label={props.label}
            onChange={handleChange}
            disabled={fieldProps.readOnly || fieldProps.disabled}
            {...fieldProps}            
        />
    );
}

export const FieldToggle = withFormContext(connect(FieldToggleInternal));