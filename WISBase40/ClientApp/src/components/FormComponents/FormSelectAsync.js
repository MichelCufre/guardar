import React, { useEffect, useLayoutEffect, useRef } from 'react';
import { connect, getIn } from 'formik';
import { withFormContext } from './WithFormContext';
import Select from 'react-select/async';
import debounce from "debounce-promise";
import { useTranslation } from 'react-i18next';
import { Spinner } from 'react-bootstrap';
import { FormDropdownIndicator } from './FormSelectAsyncDropdownIndicator';

function FieldSelectInternal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

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
        if (props.autoFocus && inputRef.current) {
            inputRef.current.focus();
        }

        return () => props.formProps.unregisterField(props.name);
    }, []);

    const inputRef = useRef(null);
    const valueRef = useRef(null);

    const handleChange = async (option) => {

        if (!option) {
            props.formProps.updateOptions(props.name, []);
            await props.formik.setFieldValue(props.name, "", false);
            await props.formProps.validateField(props.name, "");
            return;
        }

        props.formProps.updateOptions(props.name, [option]);
        await props.formik.setFieldValue(props.name, option.value, false);
        await props.formProps.validateField(props.name, option.value);
    };

    const handleBlur = (event) => {
        const fieldValue = props.formik.values[props.name];
        const options = props.formProps.getFieldProps(props.name).options;
        handleChange(fieldValue ? options.find(o => o.value === fieldValue) : null);
    };

    const getNoOptionMessage = () => {
        return t("General_Sec0_lbl_SELECT_NO_OPTIONS_ASYNC");
    };

    const getLoadingMessage = () => {
        return (
            <Spinner animation="border" role="status" size="sm">
                <span className="sr-only">Loading...</span>
            </Spinner>
        );
    };

    const loadOptions = debounce(async (value) => {
        const result = await props.formProps.searchSelectValue(props.name, value);

        if (result.moreResultsAvailable)
            return [...result.options, { label: t("General_Sec0_lbl_SELECT_MSG_MORE_RESULTS"), value: "NonSelectableItem", isDisabled: true, isInfoOption: true }];

        return result.options;
    }, 1000, { leading: true });

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

    if (selectedOption) {
        selectedOption.label = t(selectedOption.label);
        valueRef.current = selectedOption;
    }
    else
        valueRef.current = null;

    if (fieldProps.readOnly) {
        return (
            <input
                className="form-control"
                readOnly
                value={selectedOption ? t(selectedOption.label) : ""}
            />
        );
    }
    else {
        const selectStyles = {
            option: (provided, op) => {
                if (op.data.isInfoOption) {

                    return {
                        ...provided,
                        whiteSpace: "initial",
                        textAlign: "center"
                    };
                }

                return provided;
            }
        };

        return (
            <Select
                ref={inputRef}
                styles={selectStyles}
                defaultOptions={options}
                value={valueRef.current || null}
                components={{ DropdownIndicator: FormDropdownIndicator }}
                onChange={handleChange}
                onBlur={handleBlur}
                loadOptions={loadOptions}
                className={className}
                captureMenuScroll
                isDisabled={fieldProps.disabled || fieldProps.readOnly}
                placeholder={t("General_Sec0_lbl_SELECT_MSG_ASYNC")}
                noOptionsMessage={getNoOptionMessage}
                loadingMessage={getLoadingMessage}
                {...fieldProps}
                {...elementProps}
            />
        );
    }
}

export const FieldSelectAsync = withFormContext(connect(FieldSelectInternal));