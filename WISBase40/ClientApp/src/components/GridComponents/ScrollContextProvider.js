import React, { useState } from 'react';
import useScrollbarSize from 'react-scrollbar-size';

const { Provider, Consumer } = React.createContext();

//TODO: Ver de separar en distintos componentes

const ScrollContextProvider = (props) => {

    const { height, width } = useScrollbarSize();

    const [timer, setTimer] = useState(0);
    const [isScrolling, setIsScrolling] = useState(false);

    const onScrollBegin = () => {
        if (timer) {
            clearTimeout(timer);
            setTimer(0);
        }

        if (!isScrolling) {
            setIsScrolling(true);
        }

        setTimer(setTimeout(() => setIsScrolling(false), 100));
    }

    return (
        <Provider
            value={{
                scrollbarWidth: width,
                scrollbarHeight: height,
                isScrolling: isScrolling,
                perfScrollbarUpdate: props.perfScrollbarUpdate,
                onScrollBegin: onScrollBegin
            }}
        >
            {props.children}
        </Provider>
    );
}

export { ScrollContextProvider };
export default Consumer;