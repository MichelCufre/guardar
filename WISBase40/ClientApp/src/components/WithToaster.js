import React, { Component } from "react";
import { toast } from 'react-toastify';
import { useCustomTranslation } from './TranslationHook';
import { ToastMessage } from './ToastMessage';
import { notificationType } from './Enums';
import withCustomTranslation from "./WithCustomTranslation";

export default function withToaster(WrappedComponent) {
    return class WithToaster extends Component {
        toastMessage = (type, message, options, args) => {
            let translatedMessage = this.props.t(message);

            if (args && args.length > 0) {
                args.forEach((arg, index) => {
                    translatedMessage = translatedMessage.replace(new RegExp("\\{" + index + "\\}"), arg);
                });
            }

            switch (type) {
                case notificationType.error:
                    toast.error(<ToastMessage message={translatedMessage} type={notificationType.error} />, options);
                    break;
                case notificationType.info:
                    toast.info(<ToastMessage message={translatedMessage} type={notificationType.info} />, options);
                    break;
                case notificationType.warning:
                    toast.warn(<ToastMessage message={translatedMessage} type={notificationType.warn} />, options);
                    break;
                case notificationType.success:
                    toast.success(<ToastMessage message={translatedMessage} type={notificationType.success} />, options);
                    break;
                default:
                    toast(<ToastMessage message={translatedMessage} />, options);
            }
        }
        toastSuccess = (message, options, args) => {
            this.toastMessage(notificationType.success, message, options, args);
        }
        toastError = (message, options, args) => {
            this.toastMessage(notificationType.error, message, options, args);
        }
        toastWarning = (message, options, args) => {
            this.toastMessage(notificationType.warning, message, options, args);
        }
        toastInfo = (message, options, args) => {
            this.toastMessage(notificationType.info, message, options, args);
        }

        toastException = (exception) => {
            const translatedMessage = this.props.t(exception.message);

            this.toastMessage(notificationType.error, exception.name + ": " + translatedMessage, null, exception.messageArguments);
        }

        toastNotifications = (notifications) => {
            if (!notifications)
                return;

            notifications.map(d => {
                return this.toastMessage(d.type, d.message, null, d.arguments);
            });
        }        

        render() {
            const toaster = {
                toastMessage: this.toastMessage,
                toastSuccess: this.toastSuccess,
                toastError: this.toastError,
                toastWarning: this.toastWarning,
                toastInfo: this.toastInfo,
                toastException: this.toastException,
                toastNotifications: this.toastNotifications
            };

            return (
                <WrappedComponent
                    {...{ ...this.props, toaster: toaster }}
                />
            );
        }
    };
}