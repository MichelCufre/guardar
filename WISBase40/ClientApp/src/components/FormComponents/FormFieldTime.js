import React, { useLayoutEffect, useEffect, useState, useRef } from 'react';
import { connect, getIn } from 'formik';
import { withFormContext } from './WithFormContext';
import { MaskedInput } from './FormMaskedInput';

function InternalFieldTime(props) {
    const [newValue, setNewValue] = useState("");
    const inputRef = useRef(null);

    const fieldData = {
        props: {
            name: props.name,
            hidden: props.hidden,
            readOnly: props.readOnly,
            disabled: props.disabled
        }
    };

    const error = getIn(props.formik.errors, props.name);
    const touch = getIn(props.formik.touched, props.name);
    const value = getIn(props.formik.values, props.name) || "";

    useLayoutEffect(() => {
        props.formProps.registerField(fieldData)
    }, []);

    useEffect(() => {
        if (props.autoFocus && inputRef.current) {
            inputRef.current.focus();
        }

        return () => props.formProps.unregisterField(props.name);
    }, []);

    const fieldProps = props.formProps.getFieldProps(props.name);

    const { formProps, formik, autoFocus, ...result } = props;

    const updateValue = async (value) => {
        await props.formik.setFieldValue(props.name, value, false);
        await props.formProps.validateField(props.name, value);
    };

    const handleChange = (evt) => {
        //evt.preventDefault();

        setNewValue(evt.target.value);
    };
    const handleBlur = async (evt) => {
        //evt.preventDefault();

        await updateValue(newValue);
    };

    const getStatusClass = (error, touch) => {
        const errorEmpty = isErrorEmptyObject(error);

        if (props.readOnly || props.disabled)
            return "";

        return (touch && error && !errorEmpty ? "is-invalid" : (errorEmpty ? '' : (touch ? "is-valid" : "")));
    };
    const isErrorEmptyObject = (error) => {
        //Si error es vacío, por lo general significa que la validación con Yup tiró excepción
        return error && (typeof error === "object") && !Object.keys(error).length;
    }

    const className = props.className + " form-control " + getStatusClass(error, touch);
    const mask = props.includeSeconds ? "99:99:99" : "99:99";
    const internalValue = newValue || value;

    return (
        <MaskedInput ref={inputRef} mask={mask} className={className} {...result} {...fieldProps} value={internalValue} onChange={handleChange} onBlur={handleBlur} />
    );
}

export const FieldTime = withFormContext(connect(InternalFieldTime));