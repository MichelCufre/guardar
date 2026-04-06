import React, { useRef, useEffect } from 'react';

export function GridPanelContainer(props) {
    const ref = useRef(null);

    const handleTransitionEnd = () => {
        props.panelResizeEnds();
    };

    useEffect(() => {
        if (ref && ref.current) {
            ref.current.addEventListener("transitionend", handleTransitionEnd);

            return () => {
                if (ref && ref.current) {
                    ref.current.removeEventListener("transitionend", handleTransitionEnd);
                }
            };
        }
    }, []);

    const className = `gr-panel-container ${props.isStatsPanelOpen ? "open" : ""}`;    

    return (
        <div ref={ref} className={className}>
            {props.children}
        </div>
    );
}