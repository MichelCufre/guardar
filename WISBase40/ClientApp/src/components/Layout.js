import React, { useState, useRef, useEffect } from 'react';
import { LayoutHeader } from './LayoutComponents/LayoutHeader';
import { LayoutMain } from './LayoutComponents/LayoutMain';
import { LayoutFooter } from './LayoutComponents/LayoutFooter';
import { LayoutMenu } from './LayoutComponents/LayoutMenu';
import { ToastContainer, Slide } from 'react-toastify';

export function Layout(props) {
    const [isMenuOpening, setMenuOpening] = useState(false);
    const layoutRef = useRef(null);

    useEffect(() => {
        document.addEventListener("keydown", handleKeydown);
        document.addEventListener("keyup", handleKeyup);

        return () => {
            document.removeEventListener("keydown", handleKeydown);
            document.removeEventListener("keyup", handleKeyup);
        }
    });

    const handleKeydown = (evt) => {
        if (evt.which === 17) {
            layoutRef.current.classList.toggle("holding-ctrl", true);
        }
    }

    const handleKeyup = (evt) => {
        if (evt.which === 17) {
            layoutRef.current.classList.toggle("holding-ctrl", false);
        }
    }

    return (
        <div
            className="layout"
            ref={layoutRef}
        >
            <LayoutHeader
                userName={props.userName}
            />
            <LayoutMenu setMenuOpening={setMenuOpening} />
            <LayoutMain isMenuOpening={isMenuOpening} doneLoading={props.doneLoading} >
                {props.children}
            </LayoutMain>
            <LayoutFooter
                userLanguage={props.userLanguage}
                changeLanguage={props.changeLanguage}
            />
            <ToastContainer
                position="bottom-center"
                autoClose={5000}
                closeButton={false}
                draggable
            />
        </div>
    );
}
