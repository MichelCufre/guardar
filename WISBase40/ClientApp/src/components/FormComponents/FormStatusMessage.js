import React, { Component } from 'react';
import { connect, getIn } from 'formik';
import { withTranslation } from 'react-i18next';
import withCustomTranslation from '../WithCustomTranslation';

export class InternalStatusMessage extends Component {
    isErrorEmptyObject(error) {
        return (typeof error === "object") && !Object.keys(error).length;
    }

    getErrorMessage = () => {
        const error = getIn(this.props.formik.errors, this.props.for);
        const touch = getIn(this.props.formik.touched, this.props.for);

        if (error && !this.isErrorEmptyObject(error) && touch) {
            if (error.message) {
                var prependErrorMessage = "";

                if (error.isMultidataCodeReading === true) {
                    prependErrorMessage = this.props.t('General_Sec0_Error_CodigoMultidato');
                }

                return prependErrorMessage + this.props.t(error.message, error.arguments);
            }

            return this.props.t(error);
        }

        return "";
    }

    render() {
        return (
            <div className="invalid-feedback">
                {this.getErrorMessage()}
            </div>
        );
    }
}

export const StatusMessage = withTranslation()(withCustomTranslation(connect(InternalStatusMessage)));