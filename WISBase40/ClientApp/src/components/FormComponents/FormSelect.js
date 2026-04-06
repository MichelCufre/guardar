import React, { useEffect, useLayoutEffect, useRef } from 'react';
import { connect, getIn } from 'formik';
import { withFormContext } from './WithFormContext';
import Select from 'react-select';
import { useTranslation } from 'react-i18next';

function FieldSelectInternal(props) {
    const { t } = useTranslation();

    const fieldData = {
        props: {
            name: props.name,
            hidden: props.hidden,
            readOnly: props.readOnly,
            disabled: props.disabled
        }
    };

    const inputRef = useRef(null);

    useLayoutEffect(() => props.formProps.registerField(fieldData), []);

    useEffect(() => {
        if (props.autoFocus && inputRef.current) {
            inputRef.current.focus();
        }

        return () => props.formProps.unregisterField(props.name);
    }, []);

    const valueRef = useRef(null);

    const handleChange = async (option) => {
            
        if (!option) {
            await props.formik.setFieldValue(props.name, "", false);
            await props.formProps.validateField(props.name, "");

            return;
        }

        await props.formik.setFieldValue(props.name, option.value, false);
        await props.formProps.validateField(props.name, option.value);
    };

    const handleBlur = (event) => {
        const fieldValue = props.formik.values[props.name];
        handleChange(fieldValue ? { value: fieldValue } : null);
    };

    const getNoOptionMessage = () => {
        return t("General_Sec0_lbl_SELECT_NO_OPTIONS");
    };

    const { options, ...fieldProps } = props.formProps.getFieldProps(props.name);
    let { className, ...elementProps } = props;

    const error = getIn(props.formik.errors, props.name);
    const touch = getIn(props.formik.touched, props.name);
    const value = getIn(props.formik.values, props.name);

    if (!props.readOnly && !props.disabled) {
        const errorEmpty = touch && error && !(error && (typeof error === "object") && !Object.keys(error).length);

        className = (className || "") + " wis-form-select " + (touch && error && errorEmpty ? " is-invalid" : (errorEmpty ? '' : (touch ? " is-valid" : "")));
    }

    const selectedOption = options ? options.find(d => d.value === value) : null;
    const translatedOptions = options ? options.map(d => ({ ...d, label: t(d.label) })) : [];

    if (selectedOption) {
        selectedOption.label = t(selectedOption.label);
        valueRef.current = selectedOption;
    }
    else
        valueRef.current = null;

    if (fieldProps.readOnly) {
        return (
            <input
                className={"form-control " + className }
                readOnly
                value={selectedOption ? t(selectedOption.label) : ""}
            />
        );
    }
    else {
        return (
            <Select
                ref={inputRef}
                className={className}
                options={translatedOptions}
                onChange={handleChange}
                onBlur={handleBlur}
                value={valueRef.current}
                captureMenuScroll
                isDisabled={fieldProps.disabled}
                placeholder={t("General_Sec0_lbl_SELECT_MSG")}
                noOptionsMessage={getNoOptionMessage}
                {...fieldProps}
                {...elementProps}
            />
        );
    }

}

export const FieldSelect = withFormContext(connect(FieldSelectInternal));