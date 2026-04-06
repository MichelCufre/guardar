import React, { useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

export const LayoutMenuItemAction = React.memo(function LayoutMenuItemActionInternal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const translatedLabel = t(props.label);

    const itemClass = "wis-menu-item" + (props.searchValue && !props.visible ? " hidden" : "");

    if (!props.isLocal) {
        const handleClick = (evt) => {
            evt.preventDefault();

            window.location = props.url;
        };

        return (
            <div id={props.id} className={itemClass}>
                <a href={props.url} url={props.url} className="wis-item-action" /*onClick={handleClick}*/>
                    <span>{translatedLabel}</span>
                </a>
            </div>
        );
    }

    return (
        <div id={props.id} className={itemClass}>
            <Link to={props.url} className="wis-item-action">
                <span>{translatedLabel}</span>
            </Link>
        </div>
    );
});