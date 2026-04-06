import React, { Component } from 'react';
import { withFormContext } from './WithFormContext';
import { connect } from 'formik';
import { Spinner, Button as BootstrapButton } from 'react-bootstrap';
import { withTranslation } from 'react-i18next';
import { mapButtonVariant } from '../Mapper';
import { buttonVariant, formButtonType } from '../Enums';
import { withPageContext } from '../WithPageContext';

class InternalSubmitButton extends Component {
    constructor(props) {
        super(props);

        props.formProps.registerButton(this, formButtonType.submit);
    }

    componentWillUnmount() {
        this.props.formProps.unregisterButton(this.props.id);
    }

    handleClick = (evt) => {
        const buttonProps = this.props.formProps.getButtonProps(this.props.id);
        this.props.formProps.setIdSubmitClick(this.props.id);

        if (buttonProps && buttonProps.confirmMessage) {
            this.props.nexus.showConfirmation({
                message: buttonProps.confirmMessage.message,
                acceptLabel: buttonProps.confirmMessage.acceptLabel,
                cancelLabel: buttonProps.confirmMessage.cancelLabel,
                acceptVariant: buttonProps.confirmMessage.acceptVariant,
                cancelVariant: buttonProps.confirmMessage.cancelVariant,
                onAccept: () => this.props.formik.handleSubmit()
            });
        }
        else {
            this.props.formik.handleSubmit();
        }
    }

    getProps = () => {
        const { formProps, isLoading, label, ...result } = this.props;

        return result;
    }

    getLoadingSpinner = () => {
        if (this.props.formik.isSubmitting) {
            return (
                <Spinner animation="border" role="status" size="sm">
                    <span className="sr-only">Loading...</span>
                </Spinner>
            );
        }

        return null;
    }

    render() {
        const buttonProps = this.props.formProps.getButtonProps(this.props.id);

        const disabled = this.props.formik.isSubmitting || this.props.isLoading || buttonProps.disabled;

        const hidden = buttonProps.hidden ? "hidden" : "";

        const className = `${hidden} ${this.props.className || ""}`;

        return (
            <BootstrapButton
                disabled={disabled}
                variant={mapButtonVariant(buttonProps.variant || buttonVariant.primary)}
                onClick={this.handleClick}
                className={className}
                id={this.props.id}
            >
                {this.props.t(buttonProps.label)}&nbsp;{this.getLoadingSpinner()}
            </BootstrapButton>
        );
    }
}

export const SubmitButton = withTranslation()(withPageContext(withFormContext(connect(InternalSubmitButton))));