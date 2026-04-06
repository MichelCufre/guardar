import React from 'react';

export function GridCellEditControl(props) {
    if (!props.isEditingEnabled || !props.editable) {
        return (
            <React.Fragment>
                <div className="gr-cell-content">
                    {props.children}
                </div>
                <div className="gr-cell-edit-blank" />
            </React.Fragment>
        );
    }

    return (
        <React.Fragment>
            <div className="gr-cell-edit-content">
                {props.children}
            </div>
            <div className="gr-cell-edit-icon">
                <i className="fas fa-pencil-alt" />
            </div>
        </React.Fragment>
    );
}