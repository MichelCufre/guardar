import React from 'react';
import { useTranslation } from 'react-i18next';

export const MenuItemHeader = (props) => {
    const { t } = useTranslation();

    return (
        <h6 className="dropdown-header">{t(props.label)}</h6>
    );
};