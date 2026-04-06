import React from 'react';

export const { Provider: PageContextProvider, Consumer: PageContextConsumer } = React.createContext();

export function withPageContext(Component) {
    const wrappedComponent = (outerProps) => (
        <PageContextConsumer>
            {nexus => <Component {...outerProps} nexus={nexus} />}
        </PageContextConsumer>
    );

    const componentDisplayName =
        Component.displayName ||
        Component.name ||
        (Component.constructor && Component.constructor.name) ||
        'Component';

    wrappedComponent.displayName = componentDisplayName;

    return wrappedComponent;
}