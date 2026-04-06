import React from 'react';
import { useTranslation } from 'react-i18next';

export const ToolbarButton = (props) => {
    const { t } = useTranslation("translation", { useSuspense: false });

    return (
        <button className={props.className} onClick={props.onClick} title={t(props.label)} >
            <i className={props.icon} />
        </button>
    );
};