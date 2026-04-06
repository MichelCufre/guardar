import React from 'react';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../WithPageContext';
import { GridDisplayMenu } from './GridDisplayMenu';
import { GridExcelMenu } from './GridExcelMenu';
import { FilterToolbarStatus } from './GridFilterToolbarStatus';
import { ToolbarDivider } from './GridToolbarDivider';

const FilterToolbarInternal = (props) => {
    const { t } = useTranslation("translation", { useSuspense: false });

    const handleOpenStatsPanel = (evt) => {
        evt.preventDefault();

        props.toggleStatsPanel();
    };

    const handleOpenSaveFilterModal = (evt) => {
        evt.preventDefault();

        props.openSaveFilterModal();
    };

    const handleOpenLoadFilterModal = (evt) => {
        evt.preventDefault();

        props.openLoadFilterModal();
    }

    const handleOpenGuideFilterModal = (evt) => {
        evt.preventDefault();

        props.openGuideFilterModal();
    }

    return (
        <div className="gr-filter-toolbar">
            <GridExcelMenu
                openImportExcelModal={props.openImportExcelModal}
                enableExcelExport={props.enableExcelExport}
                enableExcelImport={props.enableExcelImport}
                isEditingEnabled={props.isEditingEnabled}
                isCommitEnabled={props.isCommitEnabled}
                isAddEnabled={props.isAddEnabled}
                exportExcel={props.exportExcel}
                application={props.application}
            />
            <ToolbarDivider />
            <GridDisplayMenu
                columns={props.columns}
                enableSelection={props.enableSelection}
                displayModifiedOnly={props.displayModifiedOnly}
                displaySelectionOnly={props.displaySelectionOnly}
                toggleDisplayModifiedOnly={props.toggleDisplayModifiedOnly}
                toggleDisplaySelectionOnly={props.toggleDisplaySelectionOnly}
                showColumn={props.showColumn}
                hideColumn={props.hideColumn}
            />
            <ToolbarDivider />
            <button key="filterSaveButton" className="gr-toolbar-btn filter" onClick={handleOpenSaveFilterModal} title={t("General_Sec0_lbl_SAVE_FILTER_TITLE")}>
                <i className="fas fa-download" />
            </button>
            <button key="filterLoadButton" className="gr-toolbar-btn filter" onClick={handleOpenLoadFilterModal} title={t("General_Sec0_lbl_LOAD_FILTER_TITLE")}>
                <i className="fas fa-upload" />
            </button>
            <button key="filterGuideButton" className="gr-toolbar-btn filter" onClick={handleOpenGuideFilterModal} title={t("General_Sec0_btn_Tooltip_FilterTypes")}>
                <i className="fa-solid fa-filter" />
            </button>
            <ToolbarDivider />
            <FilterToolbarStatus
                activeFilter={props.activeFilter}
            />
            <ToolbarDivider />
            <button key="statsModalButton" className="gr-toolbar-btn filter" onClick={handleOpenStatsPanel} title={t("General_Sec0_btn_Tooltip_info")}>
                <i className="fas fa-info" />
            </button>
        </div>
    );
};

export const FilterToolbar = withPageContext(FilterToolbarInternal);