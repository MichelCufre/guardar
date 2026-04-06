import React from 'react';

export function FormWarningMessage(props) {
    if (props.show) {
        return (
            <div className="input-warning text-muted">
                <i className="fas fa-exclamation-triangle"></i> {props.message}
            </div>
        );
    }

    return null;    
}