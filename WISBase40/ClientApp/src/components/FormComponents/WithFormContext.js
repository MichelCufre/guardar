import React from 'react';

export const { Provider: FormContextProvider, Consumer: FormContextConsumer } = React.createContext();

export function withFormContext(Component) {
    const wrappedComponent = (outerProps) => (
        <FormContextConsumer>
            {formProps => <Component {...outerProps} formProps={formProps} />}
        </FormContextConsumer>
    );

    const componentDisplayName =
        Component.displayName ||
        Component.name ||
        (Component.constructor && Component.constructor.name) ||
        'Component';

    wrappedComponent.displayName = componentDisplayName;

    return wrappedComponent;
}