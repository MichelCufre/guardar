import React, { useEffect, useLayoutEffect, useRef } from 'react';
import { connect, getIn } from 'formik';
import { withFormContext } from './WithFormContext';
import { Form as FormBootstrap } from 'react-bootstrap';
import { fieldType } from '../Enums';

function FieldCheckboxInternal(props) {
    const inputRef = useRef(null);

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
        if (props.autoFocus && inputRef) {
            setTimeout(() => inputRef.focus(), 100);
        }

        return () => props.formProps.unregisterField(props.name);
    }, []);

    const updateValue = async (value) => {
        await props.formik.setFieldValue(props.name, value, false);
        await props.formProps.validateField(props.name, value);
    };

    const handleChange = async (evt) => {
        if (evt.target.classList.contains("form-check-input"))
            await updateValue(evt.target.checked);
    };

    const fieldProps = props.formProps.getFieldProps(props.name);

    const error = getIn(props.formik.errors, props.name);
    const touch = getIn(props.formik.touched, props.name);

    const value = getIn(props.formik.values, props.name);

    const errorEmpty = touch && error && !(error && (typeof error === "object") && !Object.keys(error).length);

    const classValid = (touch && error && errorEmpty ? " is-invalid" : (errorEmpty ? '' : (touch ? " is-valid" : "")));

    const className = `form-field-checkbox ${classValid} ${props.className}`;

    return (
        <FormBootstrap.Check
            ref={inputRef}
            id={props.name}
            className={className}
            type="checkbox"
            checked={value}
            label={props.label}
            onChange={handleChange}
            {...fieldProps}
            custom
        />
    );
}

export const FieldCheckbox = withFormContext(connect(FieldCheckboxInternal));