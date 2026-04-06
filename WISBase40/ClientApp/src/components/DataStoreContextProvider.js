import React, { Component } from 'react';

const { Provider, Consumer } = React.createContext();

class DataStoreContextProvider extends Component {
    constructor(props) {
        super(props);

        this.state = {

        };
    }

    render() {
        return (
            <Provider>
                {this.props.children}
            </Provider>
        );
    }
}

export { DataStoreContextProvider };
export default Consumer;