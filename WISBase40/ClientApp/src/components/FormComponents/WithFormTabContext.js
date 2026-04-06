import React from 'react';

const FormTabContext = React.createContext();

export { FormTabContext };

export function withFormTabContext(Component) {
    const wrappedComponent = (outerProps) => (
        <FormTabContext.Consumer>
            {value => <Component {...outerProps} value={value} />}            
        </FormTabContext.Consumer>
    );

    const componentDisplayName =
        Component.displayName ||
        Component.name ||
        (Component.constructor && Component.constructor.name) ||
        'Component';

    wrappedComponent.displayName = componentDisplayName;

    return wrappedComponent;
}