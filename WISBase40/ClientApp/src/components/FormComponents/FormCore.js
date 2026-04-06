import React, { Component } from 'react';
import { Form as FormikForm, yupToFormErrors } from 'formik';
import { connect, getIn } from 'formik';
import { FormContextProvider } from './WithFormContext';
import { withPageContext } from '../WithPageContext';
import { formTouchValue, formStatus, componentType, notificationType, formButtonType, fieldType } from '../Enums';
import { mapButtonVariantToEnum } from '../Mapper';
import * as Yup from 'yup';
import withToaster from '../WithToaster';
import { withTranslation } from 'react-i18next';
import withCustomTranslation from '../WithCustomTranslation';
import { ComponentError } from '../ComponentError';
import { each, entries } from 'lodash';
import $ from 'jquery';

class InternalFormCore extends Component {
    constructor(props) {
        super(props);

        this.fields = [];
        this.buttons = [];
        this.resetParameters = [];
        this.buttonsInProcess = [];
        this.initialFormValues = this.props.initialValues;
        this.idSubmitClick = "";
        this.validationSchema = Yup.object().shape(this.props.validationSchema);

        this.props.nexus.registerComponent(this.props.id, componentType.form, this.getApi());

        this.props.registerValidateHandler(this.validateFields);
        this.props.registerSubmitHandler(this.handleSubmit);
        this.props.registerResetHandler(this.handleReset);
    }

    componentDidMount = async () => {
        this.initialize();
        this.mounted = true;
    }

    componentWillUnmount() {
        this.props.nexus.unregisterComponent(this.props.id);
        this.mounted = false;
    }

    showLoadingOverlay = (id = "layout") => {
        let divLayout = $("." + id);
        if (this.props.loadingOverlay === undefined || this.props.loadingOverlay) {
            let existe = $(".loadingoverlay").length > 0;
            if (!existe && $("." + id).length > 0 && divLayout.LoadingOverlay !== undefined) {
                $("." + id).LoadingOverlay("show", {
                    image: "",
                    fontawesome: "fa fa-cog fa-spin",
                    background: "rgba(22, 25, 28, 0.2)"
                });
            }
        }
    }

    hideLoadingOverlay = (id = "layout") => {
        if ((this.props.loadingOverlay === undefined || this.props.loadingOverlay) && $("." + id).LoadingOverlay !== undefined) {
            $("." + id).LoadingOverlay("hide");
        }
    }


    initialize = async (parameters) => {
        this.showLoadingOverlay();

        console.log("initializing " + this.props.id);

        try {
            this.props.formik.setSubmitting(true);

            const data = {
                application: this.props.application,
                pageToken: this.props.nexus.getPageToken(),
                form: {
                    id: this.props.id,
                    fields: this.getFields(),
                    buttons: this.getButtons()
                },
                context: {
                    parameters: this.props.nexus.getQueryParameters() || []
                }
            };

            if (parameters)
                data.context.parameters = [...data.context.parameters, ...parameters];

            const localContext = {
                abortServerCall: false,
                forceUpdateFields: false
            };

            if (this.props.onBeforeInitialize)
                this.props.onBeforeInitialize(localContext, data.form, data.context, this.props.nexus);

            if (localContext.forceUpdateFields) {
                await this.updateFields(data.form.fields);
                this.updateButtons(data.form.buttons);
            }

            if (localContext.abortServerCall) {
                this.hideLoadingOverlay();
                return Promise.resolve(false);
            }

            return this.props.formInitialize(data)
                .then(this.initializeProcessResponse);
        }
        catch (ex) {
            this.hideLoadingOverlay();
            this.props.formik.setSubmitting(false);
            this.props.toaster.toastException(ex);
        }

        return Promise.resolve();
    }
    initializeProcessResponse = async (response) => {
        try {
            if (!response) {
                this.hideLoadingOverlay();
                return Promise.resolve(false);
            }

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const data = JSON.parse(response.Data);

            if (data) {
                const localContext = {
                    abortFieldUpdate: false,
                    abortHideLoading: false
                };

                if (this.props.onAfterInitialize)
                    this.props.onAfterInitialize(localContext, data.form, data.context, this.props.nexus);

                this.props.toaster.toastNotifications(data.context.notifications);

                if (!localContext.abortHideLoading)
                    this.hideLoadingOverlay();

                if (!localContext.abortFieldUpdate) {
                    this.loadInitialFormValues(data.form.fields);
                    await this.updateFields(data.form.fields, formTouchValue.clean, true);
                    this.updateButtons(data.form.buttons);
                }
            } else {
                this.hideLoadingOverlay();
            }
        }
        catch (ex) {
            this.hideLoadingOverlay();
            this.props.toaster.toastException(ex);
        }

        this.props.formik.setSubmitting(false);

        return Promise.resolve();
    }

    loadInitialFormValues = (fields) => {
        this.initialFormValues = fields.reduce((acc, curr) => {

            if (curr.type === fieldType.checkbox)
                acc[curr.id] = curr.value === "S" || curr.value === "True" || curr.value === "true" || curr.value === true;
            else
                acc[curr.id] = curr.value || ""

            return acc;
        }, {});
    }

    reset = (parameters) => {
        this.resetParameters = parameters;
        this.props.formik.resetForm({
            values: this.props.formik.initialValues,
            errors: this.props.formik.initialErrors,
            touched: this.props.formik.initialTouched,
        });
    }

    validateFields = async (values, props) => {
        let errors = {};
        let touched = {};

        each(entries(values), ([k, v]) => {
            var field = this.fields.find(f => f.id === k);

            if (field) {
                field.error = null;
                field.status = formStatus.ok;
            }

            errors[k] = null;
        });

        this.props.formik.setErrors(errors)

        errors = {};

        return await this.validationSchema.validate(values, { abortEarly: false })
            .catch(err => {
                each(err.inner, e => {
                    var field = this.fields.find(f => f.id === e.path);

                    if (field) {
                        field.error = field.error || e.message;
                        field.status = formStatus.error;
                    }

                    errors[e.path] = e.message;
                    touched[e.path] = true;
                });

                return Promise.resolve();
            })
            .then(res => this.handleValidate(values, props, null, true))
            .then(err => {
                if (err && err !== {}) {
                    each(entries(err), ([k, v]) => {
                        var field = this.fields.find(f => f.id === k);

                        if (field) {
                            field.error = field.error || v;
                            field.status = formStatus.error;
                        }

                        touched[k] = true;
                        errors[k] = errors[k] || v;
                    });
                }

                return Promise.resolve();
            })
            .then(res => {
                return this.props.formik.setAllProperties({}, errors, touched)
                    .then(res => Promise.resolve(errors));
            });
    }

    validateField = (name, value) => {
        if (!this.props.validationSchema.hasOwnProperty(name)) {
            return Promise.resolve();
        }

        const field = this.fields.find(f => f.id === name);
        const values = this.props.formik.values;
        const touched = {};
        const errors = {};

        return this.validationSchema.validate(values, { abortEarly: false })
            .then(res => { })
            .catch(err => {
                each(err.inner, e => {
                    var field = this.fields.find(f => f.id === e.path);

                    if (field) {
                        field.error = field.error || e.message;
                        field.status = formStatus.error;
                    }

                    errors[e.path] = e.message;
                    touched[e.path] = true;
                });

                return Promise.resolve();
            })
            .then(res => this.handleValidate(values, null, name))
            .then(err => {
                if (err && err !== {}) {
                    each(entries(err), ([k, v]) => {
                        var field = this.fields.find(f => f.id === k);

                        if (field) {
                            field.error = field.error || v;
                            field.status = formStatus.error;
                        }

                        touched[k] = true;
                        errors[k] = errors[k] || v;
                    });
                } else {
                    field.error = null;
                    field.status = formStatus.ok;

                    this.props.formik.setFieldError(name, null);
                }

                return Promise.resolve();
            })
            .then(res => {
                return this.props.formik.setAllProperties({}, errors, touched)
                    .then(res => Promise.resolve(errors));
            });
    }

    handleValidate = async (values, props, currentField, isSubmit = false) => {
        const context = {
            application: this.props.application,
            formId: this.props.id,
            updateFields: this.updateFields,
            updateButtons: this.updateButtons,
            values: values,
            currentField: currentField,
            getFields: this.getFields,
            getButtons: this.getButtons,
            formValidateField: this.props.formValidateField,
            onBeforeValidateField: this.props.onBeforeValidateField,
            onAfterValidateField: this.props.onAfterValidateField,
            nexus: this.props.nexus,
            mounted: this.mounted,
            pageToken: this.props.nexus.getPageToken(),
            queryParameters: this.props.nexus.getQueryParameters() || []
        };

        if (isSubmit) {
            context.queryParameters.push({ "id": "isSubmit", "value": "true" });
        }

        return await this.serverValidation(context);
    }

    handleSubmit = async (values, actions) => {
        return await this.validationSchema.validate(values, { abortEarly: false })
            .then(res => this.submit(this.getIdSubmitClick()))
            .catch(errors => {
                if (errors.inner) {
                    errors.inner.map(async (error) => {
                        actions.setFieldError(error.path, error.message);
                        await actions.setFieldTouched(error.path, true, false);
                        return error;
                    });
                }

                actions.setSubmitting(false);

                return Promise.resolve();
            });
    }

    handleReset = async (values, actions) => {
        each(this.fields, field => {
            var initialValue = this.props.initialValues[field.id];

            if (initialValue === undefined) {
                if (field.type === fieldType.checkbox)
                    initialValue = false;
                else
                    initialValue = '';
            }

            field.error = null;
            field.status = formStatus.ok;
            field.value = initialValue;
            values[field.id] = initialValue;
            this.initialFormValues[field.id] = initialValue;
        });

        await this.initialize(this.resetParameters);

        return Promise.resolve();
    }

    submit = async (buttonId, ctrlKey) => {
        this.showLoadingOverlay();

        console.log("submitting: " + this.props.id);

        try {
            const data = {
                application: this.props.application,
                pageToken: this.props.nexus.getPageToken(),
                form: {
                    id: this.props.id,
                    fields: this.getFields(),
                    buttons: this.getButtons()
                },
                context: {
                    parameters: this.props.nexus.getQueryParameters() || [],
                    buttonId: buttonId
                }
            };

            const localContext = {
                abortServerCall: false,
                forceUpdateFields: false,
                ctrlKey: ctrlKey
            };

            if (this.props.onBeforeSubmit)
                this.props.onBeforeSubmit(localContext, data.form, data.context, this.props.nexus);

            if (!this.mounted) {
                this.hideLoadingOverlay();
                return Promise.resolve(false);
            }

            if (localContext.forceUpdateFields) {
                await this.updateFields(data.form.fields);
                this.updateButtons(data.form.buttons);
            }

            if (localContext.abortServerCall) {
                this.hideLoadingOverlay();
                return Promise.resolve(false);
            }

            return await this.props.formSubmit(data)
                .then(this.submitProcessResponse.bind(this, ctrlKey));
        }
        catch (ex) {
            this.hideLoadingOverlay();
            this.props.toaster.toastException(ex);
        }
        finally {
            this.props.formik.setSubmitting(false);
        }

        return Promise.resolve();
    }

    submitProcessResponse = async (ctrlKey, response) => {
        try {
            if (!response) {
                this.hideLoadingOverlay();
                return false;
            }

            const data = JSON.parse(response.Data);

            console.log(response);

            if (response.Status === "ERROR" && data === null)
                throw new ComponentError(response.MessageArguments, response.Message);

            const localContext = {
                abortFieldUpdate: false,
                responseStatus: response.Status,
                showErrorMessage: false,
                abortHideLoading: false
            };

            if (this.props.onAfterSubmit)
                this.props.onAfterSubmit(localContext, data.form, data.context, this.props.nexus);

            localContext.showErrorMessage = false;
            if (localContext.showErrorMessage && response.Status === "ERROR" && response.Message)
                throw new ComponentError(response.MessageArguments, response.Message);

            this.props.toaster.toastNotifications(data.context.notifications);

            if (!localContext.abortHideLoading)
                this.hideLoadingOverlay();

            if (!this.mounted) {
                this.hideLoadingOverlay();
                return false;
            }

            if (data.context.redirection) {
                this.hideLoadingOverlay();
                return this.props.nexus.redirect(data.context.redirection.url, data.context.redirection.newTab || ctrlKey, data.context.redirection.parameters);
            }

            if (data.form.fields.some(f => f.status === formStatus.error)) {
                if (response.Status === "ERROR")
                    this.props.toaster.toastError(response.Message);

                if (!localContext.abortFieldUpdate) {
                    await this.updateFields(data.form.fields, formTouchValue.touch);
                    this.updateButtons(data.form.buttons);
                }
            } else if (data.context.resetForm) {
                this.reset();
            } else {
                await this.updateFields(data.form.fields, formTouchValue.touch);
                this.updateButtons(data.form.buttons);
            }

            this.props.formik.setSubmitting(false);
        }
        catch (ex) {
            this.hideLoadingOverlay();
            this.props.formik.setSubmitting(false);

            this.props.toaster.toastException(ex);
        }

        return Promise.resolve();
    }

    performButtonAction = async (btnId, ctrlKey) => {
        this.showLoadingOverlay();
        console.log("performing button action " + this.props.id);

        if (this.buttonsInProcess.indexOf(btnId) > -1)
            return;

        this.buttonsInProcess.push({ id: btnId, ctrlKey });
        if (document.getElementById(btnId))
            document.getElementById(btnId).disabled = true;

        try {
            const data = {
                application: this.props.application,
                pageToken: this.props.nexus.getPageToken(),
                form: {
                    id: this.props.id,
                    fields: this.getFields(),
                    buttons: this.getButtons()
                },
                context: {
                    buttonId: btnId,
                    parameters: this.props.nexus.getQueryParameters() || []
                }
            };

            const context = {
                abortServerCall: false,
                forceUpdateFields: false,
                ctrlKey: ctrlKey
            };

            if (this.props.onBeforeButtonAction)
                this.props.onBeforeButtonAction(context, data.form, data.context, this.props.nexus);

            if (!this.mounted) {
                this.hideLoadingOverlay();
                return Promise.resolve(false);
            }

            if (context.forceUpdateFields) {
                await this.updateFields(data.form.fields);
                this.updateButtons(data.form.buttons);
            }

            if (context.abortServerCall) {
                this.hideLoadingOverlay();
                document.getElementById(btnId).disabled = false;
                return Promise.resolve(false);
            }

            return this.props.formPerformButtonAction(data)
                .then(this.performButtonActionProcessResponse.bind(this, ctrlKey, btnId));
        }
        catch (ex) {
            this.hideLoadingOverlay();
            this.props.toaster.toastException(ex);
        }

        return Promise.resolve();
    }

    performButtonActionProcessResponse = async (ctrlKey, btnId, response) => {
        try {

            this.buttonsInProcess.splice(this.buttonsInProcess.indexOf(btnId), 1);

            if (!response) {
                this.hideLoadingOverlay();
                return false;
            }

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const data = JSON.parse(response.Data);

            const context = {
                abortFieldUpdate: false,
                abortHideLoading: false
            };

            if (this.props.onAfterButtonAction)
                this.props.onAfterButtonAction(context, data.form, data.context, this.props.nexus);

            this.props.toaster.toastNotifications(data.context.notifications);

            if (!context.abortHideLoading)
                this.hideLoadingOverlay();

            if (!this.mounted) {
                this.hideLoadingOverlay();
                return false;
            }

            if (data.context.redirection) {
                this.hideLoadingOverlay();
                return this.props.nexus.redirect(data.context.redirection.url, data.context.redirection.newTab || ctrlKey, data.context.redirection.parameters);
            }

            if (!context.abortFieldUpdate) {
                await this.updateFields(data.form.fields);
                this.updateButtons(data.form.buttons);
            }
        }
        catch (ex) {
            this.hideLoadingOverlay();
            this.props.toaster.toastException(ex);
        }
        finally {

            if (document.getElementById(btnId))
                document.getElementById(btnId).disabled = false;
        }

        return Promise.resolve();
    }

    serverValidation = async (context) => {
        const errors = {};

        try {
            const data = {
                application: context.application,
                pageToken: context.pageToken,
                form: {
                    id: context.formId,
                    fields: context.getFields(context.values),
                    buttons: context.getButtons()
                },
                context: {
                    fieldId: context.currentField,
                    parameters: context.queryParameters || [],
                }
            };

            const localContext = {
                abortServerCall: false,
                forceUpdateFields: false
            };

            if (context.onBeforeValidateField)
                context.onBeforeValidateField(localContext, data.form, data.context, context.nexus);

            if (!context.mounted)
                return Promise.resolve();

            if (localContext.forceUpdateFields) {
                await context.updateFields(data.form.fields);
                context.updateButtons(data.form.buttons);
            }

            if (localContext.abortServerCall)
                return Promise.resolve();

            const result = await context.formValidateField(data)
                .then((res) => {
                    if (!res)
                        return res;

                    if (res.Status === "ERROR")
                        throw new ComponentError(res.MessageArguments, res.Message);

                    return JSON.parse(res.Data);
                });

            if (!result)
                return Promise.resolve();

            if (result.Status === "ERROR")
                throw new ComponentError(result.MessageArguments, result.Message);

            const responseContext = {
                abortFieldUpdate: false,
                abortValidation: false
            };

            if (context.onAfterValidateField)
                context.onAfterValidateField(responseContext, result.form, result.context, context.nexus);

            context.nexus.toastNotifications(result.context.notifications);

            if (!context.mounted || responseContext.abortValidation)
                return Promise.resolve();

            if (result && result.form && result.context) {

                if (!responseContext.abortFieldUpdate) {
                    if (result.form.fields)
                        await context.updateFields(result.form.fields);
                    if (result.form.buttons)
                        context.updateButtons(result.form.buttons);
                }

                each(result.form.fields, field => {
                    if (field.status === formStatus.error) {
                        var objectConstructor = ({}).constructor;
                        if (field.error && field.error.constructor === objectConstructor) {
                            field.error.isMultidataCodeReading = field.isMultidataCodeReading;
                        }
                        errors[field.id] = field.error;
                    }
                });
            }
        }
        catch (ex) {
            context.nexus.toastException(ex);
        }

        return Promise.resolve(errors);
    }

    searchSelectValue = async (fieldId, value) => {
        try {
            const data = {
                application: this.props.application,
                pageToken: this.props.nexus.getPageToken(),
                form: {
                    id: this.props.id,
                    fields: this.getFields(),
                    buttons: this.getButtons()
                },
                context: {
                    fieldId: fieldId,
                    searchValue: value,
                    parameters: this.props.nexus.getQueryParameters() || []
                }
            };

            const context = {
                abortServerCall: false,
                forceUpdateFields: false
            };

            if (this.props.onBeforeSelectSearch)
                this.props.onBeforeSelectSearch(context, data.form, data.context, this.props.nexus);

            if (!this.mounted)
                return Promise.resolve(false);

            if (context.forceUpdateFields) {
                await this.updateFields(data.form.fields);
                this.updateButtons(data.form.buttons);
            }

            if (context.abortServerCall)
                return Promise.resolve(false);

            const response = await this.props.formSelectSearch(data).then((res) => res);

            if (!response || !response.Data)
                return Promise.resolve(false);

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const result = JSON.parse(response.Data);

            const responseContext = {
                abortFieldUpdate: false
            };

            if (this.props.onAfterSelectSearch)
                this.props.onAfterSelectSearch(responseContext, result.options, result.context, this.props.nexus);

            if (!this.mounted)
                return Promise.resolve(true);

            if (!responseContext.abortFieldUpdate) {
                await this.updateFields(data.form.fields);
                this.updateButtons(data.form.buttons);
            }

            this.props.toaster.toastNotifications(result.notifications);

            return Promise.resolve(result);
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }

        return Promise.resolve();
    }

    updateFields = (fields, forceTouch, forceValue) => {
        let values = {};
        let errors = {};
        let touched = {};

        fields.forEach(field => {
            if (this.fields && this.fields.some(d => d.id === field.id && (forceValue || d.value !== field.value))) {
                if (field.type === fieldType.checkbox)
                    values[field.id] = field.value === "S" || field.value === "True" || field.value === "true" || field.value === true;
                else
                    values[field.id] = field.value || "";
            }

            if (field.status === formStatus.error) {
                var objectConstructor = ({}).constructor;
                if (field.error && field.error.constructor === objectConstructor) {
                    field.error.isMultidataCodeReading = field.isMultidataCodeReading;
                }
                errors[field.id] = field.error;
                touched[field.id] = true;
            }
            else {
                errors[field.id] = null;
            }

            if (forceTouch === formTouchValue.touch) {
                if (!field.readOnly && !field.disabled)
                    touched[field.id] = true;
            }
            else if (forceTouch === formTouchValue.clean || field.forceCleanTouched) {
                touched[field.id] = false;
            }
        });

        this.fields = fields;

        return this.props.formik.setAllProperties(values, errors, touched);
    }

    registerField = (field, type) => {
        const data = {
            id: field.props.name,
            hidden: field.props.hidden || false,
            readOnly: field.props.readOnly || false,
            disabled: field.props.disabled,
            label: field.label,
            type: type
        };

        this.addOrUpdateField(data);
    };
    unregisterField = (name) => {
        const fieldIndex = this.fields.findIndex(f => f.id === name);

        if (fieldIndex <= 0) {
            this.fields = [
                ...this.fields.slice(0, fieldIndex),
                ...this.fields.slice(fieldIndex + 1)
            ];
        }
    };
    addOrUpdateField = (data) => {
        const fieldIndex = this.fields.findIndex(f => f.id === data.id);

        if (fieldIndex < 0) {
            this.fields = [
                ...this.fields,
                data
            ];
        }
        else {
            this.fields = [
                ...this.fields.slice(0, fieldIndex),
                data,
                ...this.fields.slice(fieldIndex + 1)
            ];
        }
    };
    getFields = (values) => {
        if (this.fields) {
            return this.fields.map(field => {
                return {
                    id: field.id,
                    hidden: field.hidden,
                    readOnly: field.readOnly,
                    disabled: field.disabled,
                    type: field.type,
                    value: values ? values[field.id] : this.props.formik.values[field.id],
                    options: field.options || [],
                    error: field.error || this.props.formik.errors[field.id],
                    status: field.status,
                    forceCleanTouched: field.forceCleanTouched,
                };
            });
        }

        return [];
    }

    updateButtons = (buttons) => {
        this.buttons = [...buttons];
    }
    registerButton = (button, type) => {
        const data = {
            id: button.props.id,
            hidden: button.props.hidden || false,
            disabled: button.props.disabled || false,
            label: button.props.label || button.props.value,
            variant: mapButtonVariantToEnum(button.props.variant),
            buttonType: type || formButtonType.submit
        };

        this.addOrUpdateButton(data);
    };
    unregisterButton = (id) => {
        const buttonIndex = this.buttons.findIndex(f => f.id === id);

        if (buttonIndex <= 0) {
            this.buttons = [
                ...this.buttons.slice(0, buttonIndex),
                ...this.buttons.slice(buttonIndex + 1)
            ];
        }
    };
    addOrUpdateButton = (data) => {
        const buttonIndex = this.buttons.findIndex(f => f.id === data.id);

        if (buttonIndex < 0) {
            this.buttons = [
                ...this.buttons,
                data
            ];
        }
        else {
            this.buttons = [
                ...this.buttons.slice(0, buttonIndex),
                data,
                ...this.buttons.slice(buttonIndex + 1)
            ];
        }
    };
    getButtons = () => {
        if (this.buttons) {
            return this.buttons.map(button => {
                return { ...button };
            });
        }

        return [];
    }

    updateOptions = (fieldId, options) => {
        const field = this.fields.find(d => d.id === fieldId);

        if (field) {
            field.options = options;
        }
    }

    hasChanges = (fieldsToCheck) => {
        const keys = fieldsToCheck || Object.keys(this.props.formik.values)

        for (let i = 0; i < keys.length; i++) {
            if (this.props.formik.values[keys[i]] !== this.initialFormValues[keys[i]])
                return true;
        }

        return false;
    };

    parseError = (error, name) => {
        error.path = name;

        if (error.inner && error.inner.length > 0) {
            for (let i = 0; i < error.inner.length; i++) {
                error.inner[i].path = name;
            }
        }

        return yupToFormErrors(error)[name];
    }

    getInitialValues = () => {
        return Object.keys(this.fields).reduce((values, current) => {
            values[current.name] = "";

            return values;
        }, {});
    }

    getFieldProps = (name) => {
        const field = this.fields.find(field => field.id === name);

        if (field) {
            return {
                hidden: field.hidden,
                readOnly: field.readOnly,
                disabled: field.disabled,
                options: field.options
            };
        }

        return {};
    }
    getButtonProps = (id) => {
        const button = this.buttons.find(button => button.id === id);

        if (button) {
            return { ...button };
        }

        return {};
    }
    getFormProps = () => {
        return {
            validateField: this.validateField,
            registerField: this.registerField,
            unregisterField: this.unregisterField,
            registerButton: this.registerButton,
            unregisterButton: this.unregisterButton,
            getFieldProps: this.getFieldProps,
            getButtonProps: this.getButtonProps,
            setIdSubmitClick: this.setIdSubmitClick,
            performButtonAction: this.performButtonAction,
            searchSelectValue: this.searchSelectValue,
            updateOptions: this.updateOptions
        };
    }

    getIdSubmitClick = () => {
        let result = this.idSubmitClick;
        this.idSubmitClick = "";
        return result;
    }

    setFieldValue = (fieldId, value) => {
        this.props.formik.setFieldValue(fieldId, value, false);
    }

    getFieldValue = (fieldId) => {
        return this.props.formik.values[fieldId];
    }

    getFieldOptions = (fieldId) => {
        var fieldProps = this.getFieldProps(fieldId);
        var fieldOptions = [];

        if (fieldProps && fieldProps.options)
            fieldOptions = fieldProps.options;

        return fieldOptions;
    }

    setIdSubmitClick = (idButton) => {
        this.idSubmitClick = idButton;
    }

    getApi() {
        return {
            reset: this.reset,
            submit: this.submit,
            hasChanges: this.hasChanges,
            setFieldValue: this.setFieldValue,
            getFieldValue: this.getFieldValue,
            getFieldOptions: this.getFieldOptions,
            addOrUpdateField: this.addOrUpdateField,
            clickButton: this.performButtonAction,
            showLoadingOverlay: this.showLoadingOverlay,
            hideLoadingOverlay: this.hideLoadingOverlay
        };
    }

    render() {
        return (
            <FormContextProvider value={this.getFormProps()}>
                <FormikForm onSubmit={this.props.formik.handleSubmit}>
                    {this.props.children}
                </FormikForm>
            </FormContextProvider>
        );
    }
}

export const FormCore = withTranslation()(withCustomTranslation(withToaster(withPageContext(connect(InternalFormCore)))));
