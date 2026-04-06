import React, { useRef, useEffect } from 'react';

export function GridColumnDropdown(props) {
    const timeout = useRef(false);
    const dropdownRef = useRef(null);

    useEffect(() => {
        if (props.show) {
            if (props.left + dropdownRef.current.clientWidth > document.documentElement.clientWidth) {
                dropdownRef.current.style.left = `${props.left - dropdownRef.current.clientWidth}px`;
            }

            dropdownRef.current.style.visibility = "initial";
        }
        else {
            dropdownRef.current.style.visibility = "hidden";
        }
    }, [props.show]);

    const style = {
        display: props.show ? "block" : "none",
        visibility: "hidden",
        position: "fixed",
        left: props.left,
        top: props.top
    };

    const handleMenuLeave = () => {
        if (props.show) {
            timeout.current = setTimeout(() => {
                props.onClose();
            }, 300);
        }
    };

    const handleMouseEnter = () => {
        if (timeout.current) {
            clearTimeout(timeout.current);
        }
    };

    return (
        <div
            className="dropdown-menu"
            style={style}
            ref={dropdownRef}
            onMouseLeave={handleMenuLeave}
            onMouseEnter={handleMouseEnter}
        >
            {props.children}
        </div>
    );
};