import React, { useEffect, useLayoutEffect, useState, useRef } from 'react';
import { connect, getIn } from 'formik';
import { withFormContext } from './WithFormContext';
import { FormSliderInput } from './FormSliderInput';
import { Row, Col } from 'react-bootstrap';

function FieldRangeInternal(props) {
    const [currentValue, setCurrentValue] = useState(props.initialValue || props.min);
    const [touching, setTouching] = useState(false);
    const [initializing, setInitializing] = useState(true);

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

    const value = getIn(props.formik.values, props.name);

    useEffect(() => {
        console.log(value);

        if (initializing && value !== "") {            
            setCurrentValue(+value);
            setInitializing(false);
        }
    }, [value]);

    useEffect(() => {
        if (!touching && !initializing)
            updateValue(currentValue);
    }, [currentValue, touching]);

    const updateValue = async (value) => {
        await props.formik.setFieldValue(props.name, value, false);
        await props.formProps.validateField(props.name, value);
    };

    const handleChange = (evt, newValue) => {
        setTouching(true);
        setCurrentValue(newValue);
    }

    const handleInputChange = (evt) => {
        let value = (evt.target.value || "0").replace("%", "");

        if (value > props.max)
            value = props.max;

        if (value < props.min)
            value = props.min;

        setCurrentValue(value);
    }

    const handleAfterChange = () => {
        setTouching(false);
    }

    const fieldProps = props.formProps.getFieldProps(props.name);

    const error = getIn(props.formik.errors, props.name);
    const touch = getIn(props.formik.touched, props.name);

    const errorEmpty = touch && error && !(error && (typeof error === "object") && !Object.keys(error).length);

    const classValid = (touch && error && errorEmpty ? " is-invalid" : (errorEmpty ? '' : (touch ? " is-valid" : "")));

    const classNameContainer = "form-field-range " + classValid;
    const classNameInput = "form-control " + classValid;

    const sliderValue = +currentValue;
    const inputValue = props.tooltipFormat ? props.tooltipFormat(currentValue) : currentValue;

    if (props.showInput) {
        return (
            <div className={classNameContainer}>
                <Row>
                    <Col xs={9}>
                        <FormSliderInput
                            value={sliderValue}
                            onChange={handleChange}
                            onChangeCommitted={handleAfterChange}
                            valueLabelFormat={props.tooltipFormat}
                            min={props.min}
                            max={props.max}
                            marks={props.marks}
                            step={props.step}
                            valueLabelDisplay="auto"
                            //variant={props.variant}
                            //tooltip={props.tooltip}
                            //tooltipPlacement={props.tooltipPlacement}
                            {...fieldProps}
                        />
                    </Col>
                    <Col xs={3}>
                        <input className={classNameInput} value={inputValue} onChange={handleInputChange} />
                    </Col>
                </Row>
            </div>
        );
    }

    return (
        <div className={classNameContainer}>
            <FormSliderInput
                value={sliderValue}
                onChange={handleChange}
                onChangeCommited={handleAfterChange}
                valueLabelFormat={props.tooltipFormat}
                min={props.min}
                max={props.max}
                marks={props.marks}
                step={props.step}
                valueLabelDisplay="auto"
                //variant={props.variant}
                //tooltip={props.tooltip}
                //tooltipPlacement={props.tooltipPlacement}
                {...fieldProps}
            />
        </div>
    );
}

export const FieldRange = withFormContext(connect(FieldRangeInternal));