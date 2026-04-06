import React from 'react';
import { LayoutMenuItemAction } from './LayoutMenuItemAction';
import { LayoutMenuItemLabel } from './LayoutMenuItemLabel';
import { useTranslation } from 'react-i18next';

export const LayoutMenuItem = React.memo(function LayoutMenuItemInternal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    if (props.url)
        return (
            <LayoutMenuItemAction
                id={props.id}
                label={t(props.label)}
                url={props.url}
                visible={props.visible}
                isLocal={props.isLocal}
                searchValue={props.searchValue}
                menuOpen={props.menuOpen}
                submenuOpen={props.submenuOpen}
                setSubmenuOpen={props.setSubmenuOpen}
            />
        );

    return (
        <LayoutMenuItemLabel
            id={props.id}
            label={t(props.label)}
            items={props.items}
            visible={props.visible}
            isLocal={props.isLocal}
            searchValue={props.searchValue}
            menuOpen={props.menuOpen}
            submenuOpen={props.submenuOpen}
            setSubmenuOpen={props.setSubmenuOpen}
        />
    );
});