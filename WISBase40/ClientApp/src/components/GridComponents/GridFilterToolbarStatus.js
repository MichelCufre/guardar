import React from 'react';
import { useTranslation } from 'react-i18next';

export function FilterToolbarStatus(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const filterName = props.activeFilter ? props.activeFilter.name : t("General_Sec0_lbl_FILTER_INACTIVE");
    const helpText = props.activeFilter ? props.activeFilter.description : undefined;

    return (
        <div className="gr-filter-status">
            <div>{t("General_Sec0_lbl_FILTER_INDICATOR")}</div>
            <div title={helpText}>{filterName}</div>
        </div>
    );
}