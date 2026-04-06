import React, { useRef, useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';

export function GridCellErrorMessage(props) {
    const [style, setStyle] = useState({
        left: 0,
        top: 0,
        visibility: "hidden"
    });
    const errorMsgRef = useRef(null);
    const { t } = useTranslation();

    const className = "gr-cell-error-message";

    useEffect(d => {
        if (errorMsgRef.current) {
            const diff = (props.cellRef.current.offsetWidth - errorMsgRef.current.offsetWidth) / 2;

            const left = props.cellRef.current.getBoundingClientRect().left + diff;
            const top = props.cellRef.current.getBoundingClientRect().top - errorMsgRef.current.offsetHeight - 2;

            setStyle({
                left: left,
                top: top,
                visibility: "visible"
            });
        }
    }, [errorMsgRef]);

    let message = props.content.error.message;

    if (message) {
        const messageParts = message.split(" ");

        const translatedGroup = messageParts.map(d => t(d));

        let translatedMessage = translatedGroup.join(" ");

        if (props.content.error.arguments && props.content.error.arguments.length > 0) {
            props.content.error.arguments.forEach((arg, index) => {
                translatedMessage = translatedMessage.replace(/\{\d+\}/, arg);
            });
        }

        message = translatedMessage;
    }

    return (
        <div
            className={className}
            style={style}
            ref={errorMsgRef}
        >
            {message}
        </div>
    );
}