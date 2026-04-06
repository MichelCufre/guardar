import React, { Component } from 'react';
import { connect, getIn } from 'formik';
import { withFormContext } from './WithFormContext';

export class InternalField extends Component {
    constructor(props) {
        super(props);

        props.formProps.registerField(this);
    }

    componentWillUnmount() {
        this.props.formProps.unregisterField(this.props.name);
    }

    handleChange = async (event) => {
        await this.props.formik.setFieldValue(this.props.name, event.target.value, false);
    }

    handleBlur = async (event) => {
        await this.props.formik.setFieldValue(this.props.name, event.target.value, false);
        await this.props.formProps.validateField(this.props.name, event.target.value);
    }

    getProps = () => {
        const { formProps, ...result } = this.props;

        return result;
    }

    getOptions = (options) => {
        if (options) {
            return options.map(d => <option key={d.value} value={d.value}>{d.label}</option>);
        }
    }

    getStatusClass = (error, touch) => {
        const errorEmpty = this.isErrorEmptyObject(error);

        if (this.props.readOnly || this.props.disabled)
            return "";

        return (touch && error && !errorEmpty ? "is-invalid" : (errorEmpty ? '' : (touch ? "is-valid" : "")));
    }

    isErrorEmptyObject(error) {
        //Si error es vacío, por lo general significa que la validación con Yup tiró excepción
        return error && (typeof error === "object") && !Object.keys(error).length;
    }

    render() {
        const value = getIn(this.props.formik.values, this.props.name);
        const error = getIn(this.props.formik.errors, this.props.name);
        const touch = getIn(this.props.formik.touched, this.props.name);
        const className = this.props.className + " form-control " + this.getStatusClass(error, touch);
        const { options, ...fieldProps } = this.props.formProps.getFieldProps(this.props.name);

        return (
            <input
                {...this.getProps()}
                {...fieldProps}
                className={className}
                value={value}
                onChange={this.handleChange}
                onBlur={this.handleBlur}
            >  
                {this.getOptions(options)}
            </input>
        );
    }
}

export const FieldSelectLegacy = withFormContext(connect(InternalField));