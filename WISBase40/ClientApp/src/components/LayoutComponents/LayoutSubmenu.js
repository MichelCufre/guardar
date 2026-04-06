import React from 'react';

export function LayoutSubmenu(props) {
    const submenuClass = "wis-item-submenu" + (props.open ? " open" : "");

    return (
        <div className={submenuClass}>
            {props.children}
        </div>
    );
}