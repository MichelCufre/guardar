import React, { useEffect, useLayoutEffect, useRef } from 'react';
import { connect, getIn } from 'formik';
import { withFormContext } from './WithFormContext';
import DatePicker from 'react-datepicker';
import debounce from "debounce-promise";
import { FieldDateInput } from './FormFieldDateInput';
import customParseFormat from 'dayjs/plugin/customParseFormat';
import dayjs from 'dayjs';

function FieldDateTimeInternal(props) {
    dayjs.extend(customParseFormat);

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

    const dateFormat = "dd/MM/yyyy HH:mm:ss";

    const updateValue = async (date) => {
        const isoDate = date ? date.toISOString() : "";

        await props.formik.setFieldValue(props.name, isoDate, false);
        await props.formProps.validateField(props.name, isoDate);
    };

    const handleChange = debounce(async (value) => {
        return await updateValue(value);
    }, 1000, { leading: true });

    const fieldProps = props.formProps.getFieldProps(props.name);

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
                showTimeInput
                className={classNameInput}
                timeInputLabel="Hora: "
                dateFormat={dateFormat}
                timeFormat="HH:mm"
                onChange={handleChange} //TODO: Ver si optimizar, actualmente manda a validar cada vez que se cambia la hora
                shouldCloseOnSelect={false}
                customInput={<FieldDateInput mask="99/99/9999 99:99:99" totallyNotAutofocus={props.autoFocus} />}
                tabIndex={0}
                {...fieldProps}
            />
        </div>
    );
}

export const FieldDateTime = withFormContext(connect(FieldDateTimeInternal));