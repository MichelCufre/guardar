import React from 'react';

export function FormTabStepHeader(props) {
    const className = `form-tab-step-header ${props.active ? "active": ""}`;

    return (
        <div className={className}>
            <div className="form-tab-step-number">
                <span>{props.step}</span>
            </div>
            <div className="form-tab-step-label">
                <span>{props.label}</span>
            </div>
        </div>
    );
}