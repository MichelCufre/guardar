import React from 'react';

export function FormTabTitle(props) {
    if (props.hasChanges) {
        return (
            <div>{props.value} <span className="form-tab-title-changes">*</span></div>
        );
    }

    return (
        <div>{props.value}</div>
    );
}