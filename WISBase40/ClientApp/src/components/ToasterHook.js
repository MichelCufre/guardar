import React from "react";
import { toast } from 'react-toastify';
import { useCustomTranslation } from './TranslationHook';
import { ToastMessage } from './ToastMessage';
import { notificationType } from './Enums';

export function useToaster() {
    const { t } = useCustomTranslation();

    const toastMessage = (type, message, options, args) => {
        let translatedMessage = t(message);

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
    const toastSuccess = (message, options, args) => {
        toastMessage(notificationType.success, message, options, args);
    }
    const toastError = (message, options, args) => {
        toastMessage(notificationType.error, message, options, args);
    }
    const toastWarning = (message, options, args) => {
        toastMessage(notificationType.warning, message, options, args);
    }
    const toastInfo = (message, options, args) => {
        toastMessage(notificationType.info, message, options, args);
    }

    const toastException = (exception) => {
        const translatedMessage = t(exception.message);

        toastMessage(notificationType.error, exception.name + ": " + translatedMessage, null, exception.messageArguments);
    }

    const toastNotifications = (notifications) => {
        if (!notifications)
            return;

        notifications.map(d => {
            return toastMessage(d.type, d.message, null, d.arguments);
        });
    }

    return {
        toastMessage: toastMessage,
        toastSuccess: toastSuccess,
        toastError: toastError,
        toastWarning: toastWarning,
        toastInfo: toastInfo,
        toastException: toastException,
        toastNotifications: toastNotifications
    };
}