import React, { Component } from 'react';
import { ActionToolbar } from './GridActionToolbar';
import { FilterToolbar } from './GridFilterToolbar';

export class Toolbar extends Component {
    render() {
        return (
            <div className="gr-toolbar">
                <ActionToolbar
                    gridIsEditable={this.props.gridIsEditable}
                    menuItems={this.props.menuItems}
                    refresh={this.props.refresh}
                    commit={this.props.commit}
                    rollback={this.props.rollback}
                    deleteRow={this.props.deleteRow}
                    addRow={this.props.addRow}
                    performMenuItemAction={this.props.performMenuItemAction}
                    toggleFilterBar={this.props.toggleFilterBar}
                    applyFilter={this.props.applyFilter}

                    moveToNewRowEditableCell={this.props.moveToNewRowEditableCell}

                    isEditingEnabled={this.props.isEditingEnabled}
                    isCommitEnabled={this.props.isCommitEnabled}
                    isRollbackEnabled={this.props.isRollbackEnabled}
                    isAddEnabled={this.props.isAddEnabled}
                    isRemoveEnabled={this.props.isRemoveEnabled}
                    isCommitButtonUnavailable={this.props.isCommitButtonUnavailable}
                />
                <FilterToolbar
                    columns={this.props.columns}
                    showColumn={this.props.showColumn}
                    hideColumn={this.props.hideColumn}
                    enableExcelExport={this.props.enableExcelExport}
                    enableExcelImport={this.props.enableExcelImport}
                    exportExcel={this.props.exportExcel}
                    application={this.props.application}
                    openSaveFilterModal={this.props.openSaveFilterModal}
                    openLoadFilterModal={this.props.openLoadFilterModal}
                    openGuideFilterModal={this.props.openGuideFilterModal}
                    openImportExcelModal={this.props.openImportExcelModal}

                    openStatsPanel={this.props.openStatsPanel}
                    toggleStatsPanel={this.props.toggleStatsPanel}

                    isEditingEnabled={this.props.isEditingEnabled}
                    isCommitEnabled={this.props.isCommitEnabled}
                    isAddEnabled={this.props.isAddEnabled}

                    enableSelection={this.props.enableSelection}

                    displayModifiedOnly={this.props.displayModifiedOnly}
                    displaySelectionOnly={this.props.displaySelectionOnly}

                    toggleDisplayModifiedOnly={this.props.toggleDisplayModifiedOnly}
                    toggleDisplaySelectionOnly={this.props.toggleDisplaySelectionOnly}

                    activeFilter={this.props.activeFilter}
                />
            </div>
        );
    }
}