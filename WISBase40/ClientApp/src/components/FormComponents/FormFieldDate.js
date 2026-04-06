import React, { useEffect, useLayoutEffect, useRef } from 'react';
import { connect, getIn } from 'formik';
import { withFormContext } from './WithFormContext';
import DatePicker from 'react-datepicker';
import debounce from "debounce-promise";
import { FieldDateInput } from './FormFieldDateInput';

function FieldDateInternal(props) {
    const fieldData = {
        props: {
            name: props.name,
            hidden: props.hidden,
            readOnly: props.readOnly,
            disabled: props.disabled
        }
    };

    useLayoutEffect(() => props.formProps.registerField(fieldData), []);

    useEffect(() => {
        return () => props.formProps.unregisterField(props.name);
    }, []);    

    const fieldProps = props.formProps.getFieldProps(props.name);

    const updateValue = async (date) => {
        const isoDate = date ? date.toISOString() : "";

        if (fieldProps.disabled)
            return;

        await props.formik.setFieldValue(props.name, isoDate, false);
        await props.formProps.validateField(props.name, isoDate);
    };

    const handleChange = debounce(async (value) => {
        return await updateValue(value);
    }, 1000, { leading: true });

    const handleKeyDown = (event) => {
        if (props.onKeyDown) {
            props.onKeyDown(event, fieldProps)
        }
    };

    const error = getIn(props.formik.errors, props.name);
    const touch = getIn(props.formik.touched, props.name);
    const value = getIn(props.formik.values, props.name);

    const errorEmpty = touch && error && !(error && (typeof error === "object") && !Object.keys(error).length);

    const classValid = (touch && error && errorEmpty ? " is-invalid" : (errorEmpty ? '' : (touch ? " is-valid" : "")));

    const classNameInput = "form-control " + classValid;
    const classNameContainer = "form-field-date " + classValid;

    const dateValue = value ? new Date(value) : null;

    return (
        <div className={classNameContainer}>
            <DatePicker
                name={props.name}
                selected={dateValue}
                className={classNameInput}
                dateFormat="dd/MM/yyyy"
                onChange={handleChange}
                onKeyDown={handleKeyDown}
                customInput={<FieldDateInput mask="99/99/9999" readOnly={fieldProps.readOnly} disabled={fieldProps.disabled} totallyNotAutofocus={props.autoFocus} />} //Por alguna razon no le gusta que se llame autoFocus la propiedad
                {...fieldProps}
            />
        </div>
    );
}

export const FieldDate = withFormContext(connect(FieldDateInternal));