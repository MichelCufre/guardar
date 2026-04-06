import React, { useState } from 'react';
import { FormTabContext } from './WithFormTabContext';
import { FormTabStepList } from './FormTabStepList';
import { FormTabStep } from './FormTabStep';

const FormTab = function (props) {
    const [steps, setSteps] = useState([]);

    const addStep = ({ id, label }) => {
        setSteps((prev) => [
            ...prev,
            { id: id, label: label }
        ]);       
    };

    const values = {
        currentTab: props.current,
        addStep: addStep
    };

    return (
        <FormTabContext.Provider
            value={values}            
        >
            <div className="form-tab"> 
                <div className="form-tab-title">
                    <h2>{props.title}</h2>
                    <div className="form-tab-close-button">
                        <a onClick={props.onClose}><i className="fa fa-times" /></a>
                    </div>
                </div>
                <div className="form-tab-container">
                    <FormTabStepList
                        steps={steps}
                    />
                    {props.children}
                </div>
            </div>
        </FormTabContext.Provider>
    );
};

export {
    FormTab,
    FormTabStep
};