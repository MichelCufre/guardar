import React, { Component } from 'react';
import { useTranslation } from "react-i18next";

export function useCustomTranslation(props) {

    const { t } = useTranslation(props);

    const translate = (message, args) => {
        const messageParts = message.split(" ");

        const translatedGroup = messageParts.map(d => t(d));

        let translatedMessage = translatedGroup.join(" ");

        if (args && args.length > 0) {
            args.forEach((arg, index) => {
                translatedMessage = translatedMessage.replace(/\{\d+\}/, arg);
            });
        }

        return translatedMessage;
    }

    return { t: translate }

}