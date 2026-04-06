import React, { useContext } from 'react';
import { FormTabStepHeader } from './FormTabStepHeader';
import { FormTabStepArrow } from './FormTabStepArrow';
import { FormTabContext } from './WithFormTabContext';

export function FormTabStepList(props) {
    const context = useContext(FormTabContext);

    if (props.steps.length < 2) {
        return null;
    }

    const steps = props.steps.map((d, index) => {
        if (index > 0) {
            return (
                <React.Fragment key={d.id}>
                    <FormTabStepArrow/>
                    <FormTabStepHeader
                        step={index + 1}
                        label={d.label}
                        active={d.id === context.currentTab}
                    />
                </React.Fragment>
            );
        }
        else {
            return (
                <FormTabStepHeader
                    key={d.id}
                    step={index + 1}
                    label={d.label}
                    active={d.id === context.currentTab}
                />
            );
        }
    });

    return (
        <div className="form-tab-step-list">
            {steps}
        </div>
    );
}