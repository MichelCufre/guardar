import { Formik } from 'formik';
import { each, entries, set } from 'lodash';
import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { Button as FormButton } from './FormButton';
import { FormCore } from './FormCore';
import { Field } from './FormField';
import { FieldCheckbox } from './FormFieldCheckbox';
import { FieldCheckboxList } from './FormFieldCheckboxList';
import { FieldDate } from './FormFieldDate';
import { FieldDateTime } from './FormFieldDateTime';
import { FieldNumber } from './FormFieldNumber';
import { FieldRange } from './FormFieldRange';
import { FieldRangePercentage } from './FormFieldRangePercentage';
import { FieldText } from './FormFieldText';
import { FieldTextArea } from './FormFieldTextArea';
import { FieldToggle } from './FormFieldToggle';
import { FieldSelect } from './FormSelect';
import { FieldSelectAsync } from './FormSelectAsync';
import { FieldSelectLegacy } from './FormSelectLegacy';
import { StatusMessage } from './FormStatusMessage';
import { SubmitButton } from './FormSubmitButton';
import withFormDataProvider from './WithFormDataProvider';

export class InternalForm extends Component {
    static propTypes = {
        application: PropTypes.string,
        initialValues: PropTypes.object,
        onBeforeInitialize: PropTypes.func,
        onAfterInitialize: PropTypes.func,
        onBeforeValidateField: PropTypes.func,
        onAfterValidateField: PropTypes.func,
        onBeforeSubmit: PropTypes.func,
        onAfterSubmit: PropTypes.func,
        onBeforeButtonAction: PropTypes.func,
        onAfterButtonAction: PropTypes.func,
        onBeforeSelectSearch: PropTypes.func,
        onAfterSelectSearch: PropTypes.func
    }

    static defaultProps = {
        application: null,
        initialValues: {},
        validationSchema: {},
    }

    constructor(props) {
        super(props);

        this.validateHandler = null;
        this.submitHandler = null;
    }

    registerValidateHandler = (handler) => {
        this.validateHandler = handler;
    }

    registerSubmitHandler = (handler) => {
        this.submitHandler = handler;
    }

    registerResetHandler = (handler) => {
        this.resetHandler = handler;
    }

    validateCaller = async (values, props) => {
        return await this.validateHandler(values, props);
    }

    submitCaller = async (values, actions) => {
        return await this.submitHandler(values, actions);
    }

    resetCaller = async (values, actions) => {
        return await this.resetHandler(values, actions);
    }

    withFormikCustom = (formik) => {
        if (!formik.hasOwnProperty('setAllProperties')) {
            formik['setAllProperties'] = function (values, errors, touched) {
                const formikValues = this.values || {};
                const formikErrors = this.errors || {};
                const formikTouched = this.touched || {};

                each(entries(values), ([k, v]) => {
                    set(formikValues, k, v)
                });

                each(entries(errors), ([k, v]) => {
                    if (v) {
                        set(formikErrors, k, v)
                    } else {
                        delete formikErrors[k]
                        delete this.errors[k];
                    }
                });

                each(entries(touched), ([k, v]) => {
                    set(formikTouched, k, v)
                });

                this.setErrors(formikErrors);

                return this.setValues(formikValues, false)
                    .then(res => this.setTouched(formikTouched, false));
            }
        }

        return formik;
    }

    render() {
        return (
            <Formik
                initialValues={this.props.initialValues}
                validateOnChange={false}
                validate={this.validateCaller}
                onSubmit={this.submitCaller}
                onReset={this.resetCaller}
                render={
                    formik => (
                        <FormCore
                            application={this.props.application}
                            id={this.props.id}
                            loadingOverlay={this.props.loadingOverlay}
                            formik={this.withFormikCustom(formik)}
                            initialValues={this.props.initialValues}
                            validationSchema={this.props.validationSchema}
                            registerValidateHandler={this.registerValidateHandler}
                            registerSubmitHandler={this.registerSubmitHandler}
                            registerResetHandler={this.registerResetHandler}
                            formInitialize={this.props.formInitialize}
                            formValidateField={this.props.formValidateField}
                            formSubmit={this.props.formSubmit}
                            formPerformButtonAction={this.props.formPerformButtonAction}
                            formSelectSearch={this.props.formSelectSearch}
                            onBeforeInitialize={this.props.onBeforeInitialize}
                            onAfterInitialize={this.props.onAfterInitialize}
                            onBeforeSubmit={this.props.onBeforeSubmit}
                            onAfterSubmit={this.props.onAfterSubmit}
                            onBeforeValidateField={this.props.onBeforeValidateField}
                            onAfterValidateField={this.props.onAfterValidateField}
                            onBeforeButtonAction={this.props.onBeforeButtonAction}
                            onAfterButtonAction={this.props.onAfterButtonAction}
                            onBeforeSelectSearch={this.props.onBeforeSelectSearch}
                            onAfterSelectSearch={this.props.onAfterSelectSearch}
                        >
                            {this.props.children}
                        </FormCore>
                    )
                }
            />
        );
    }
}

export const Form = withFormDataProvider(InternalForm);

export {
    Field, FieldCheckbox, FieldCheckboxList, FieldDate,
    FieldDateTime, FieldNumber, FieldRange,
    FieldRangePercentage, FieldSelect,
    FieldSelectAsync,
    FieldSelectLegacy, FieldText, FieldTextArea, FieldToggle, FormButton,
    StatusMessage, SubmitButton
};
