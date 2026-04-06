import React, { useState } from 'react';
import { LayoutMenuItem } from './LayoutMenuItem';
import { LayoutSubmenu } from './LayoutSubmenu';
import { useTranslation } from 'react-i18next';

export function LayoutMenuSection(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [submenuOpen, setSubmenuOpen] = useState(false);

    const handleClick = () => {
        if (props.menuOpen) {
            setSubmenuOpen(!submenuOpen);
        }
    };

    const submenu = !props.items ? null : props.items.map(i => (
        <LayoutMenuItem
            key={i.id}
            id={i.id}
            label={i.label}
            icon={i.icon}
            url={i.url}
            items={i.submenuItems}
            visible={i.visible}
            isLocal={i.isLocal}
            searchValue={props.searchValue}
            menuOpen={props.menuOpen}
            submenuOpen={submenuOpen}
            setSubmenuOpen={setSubmenuOpen}
        />
    ));

    const sectionClass = "wis-menu-section" + (props.searchValue && !props.visible ? " hidden" : "");

    return (
        <div id={props.id} className={sectionClass}>
            <a className="wis-item-label" onClick={handleClick}>
                <i className={props.icon} />
                <span>{t(props.label)}</span>
            </a>
            <LayoutSubmenu open={submenuOpen || (props.searchValue && props.visible)}>
                {submenu}
            </LayoutSubmenu>
        </div>
    );
}