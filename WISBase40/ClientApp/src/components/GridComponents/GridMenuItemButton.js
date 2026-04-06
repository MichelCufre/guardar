import React from 'react';
import { useTranslation } from 'react-i18next';

export const MenuItemButton = (props) => {
    const { t } = useTranslation();

    const className = "dropdown-item-icon " + props.className;

    return (
        <button id={props.id} className="dropdown-item" onClick={props.onClick} disabled={props.disabled}><i className={className} />{' '}{t(props.label)}</button>
    );
};