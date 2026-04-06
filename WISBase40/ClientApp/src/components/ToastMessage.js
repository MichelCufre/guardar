import React, { useState } from 'react';
import { notificationType } from './Enums';

export function ToastMessage(props) {
    let iconClass = "";

    if (props.type === notificationType.error || props.type === notificationType.warning)
        iconClass = "fas fa-exclamation-triangle";

    if (props.type === notificationType.success)
        iconClass = "fas fa-check";

    if (props.type === notificationType.info)
        iconClass = "fas fa-info-circle";

    return (
        <div className="toast-message">
            <div className="toast-message-icon"><i className={iconClass}></i></div>
            <div className="toast-message-content">{props.message}</div>
            <div className="toast-close-button"><i className="fa fa-times"></i></div>
        </div>
    );
}
