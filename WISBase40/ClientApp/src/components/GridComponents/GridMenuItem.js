import React from 'react';
import { MenuItemButton } from './GridMenuItemButton';
import { MenuItemDivider } from './GridMenuItemDivider';
import { MenuItemHeader } from './GridMenuItemHeader';
import { menuItemType } from '../Enums';

export function MenuItem(props) {
    switch (props.type) {
        case menuItemType.button:
            return <MenuItemButton {...props} />;
        case menuItemType.divider:
            return <MenuItemDivider {...props} />;
        case menuItemType.header:
            return <MenuItemHeader {...props} />;
        default:
            return null;
    }
}