import React, { useState, useEffect } from 'react';
import { LayoutMenuItem } from './LayoutMenuItem';
import { LayoutSubmenu } from './LayoutSubmenu';
import { useTranslation } from 'react-i18next';

export const LayoutMenuItemLabel = React.memo(function LayoutMenuItemLabelInternal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [submenuOpen, setSubmenuOpen] = useState(false);

    const translatedLabel = t(props.label);

    const handleClick = () => {
        if (props.menuOpen) {
            setSubmenuOpen(!submenuOpen);
        }
    };

    const submenu = props.items ? props.items.map(i => (
        <LayoutMenuItem
            key={i.id}
            id={i.id}
            label={i.label}
            icon={i.icon}
            url={i.url}
            items={i.submenuItems}
            visible={i.visible}
            isLocal={i.isLocal}
            menuOpen={props.menuOpen}
            submenuOpen={submenuOpen}
            setSubmenuOpen={setSubmenuOpen}
            searchValue={props.searchValue}
        />
    )) : null;

    const itemClass = "wis-menu-item" + (props.searchValue && !props.visible ? " hidden" : "");

    const arrowClass = (submenuOpen || (props.searchValue && props.visible)) ? "fa fa-chevron-down float-end submenu-arrow" : "fa fa-chevron-right float-end submenu-arrow";

    return (
        <div id={props.id} className={itemClass}>
            <a className="wis-item-label" onClick={handleClick}>
                <span>{translatedLabel}</span>
                <i className={arrowClass} />
            </a>
            <LayoutSubmenu open={submenuOpen || (props.searchValue && props.visible)}>
                {submenu}
            </LayoutSubmenu>
        </div>
    );
});