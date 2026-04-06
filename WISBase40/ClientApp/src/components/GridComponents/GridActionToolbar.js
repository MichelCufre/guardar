import React, { Component } from 'react';
import { ToolbarDivider } from './GridToolbarDivider';
import { ToolbarButton } from './GridToolbarButton';
import { ToolbarMenu } from './GridToolbarMenu';

export class ActionToolbar extends Component {
    handleCommitClick = (evt) => {
        this.props.commit();
        evt.preventDefault();
    }
    handleRollbackClick = (evt) => {
        this.props.rollback(false);
        evt.preventDefault();
    }
    handleDeleteRowClick = (evt) => {
        this.props.deleteRow();
        evt.preventDefault();
    }
    handleAddRowClick = (evt) => {
        this.props.moveToNewRowEditableCell();
        evt.preventDefault();
    }
    handleApplyFilterClick = (evt) => {
        this.props.applyFilter();
        evt.preventDefault();
    }
    handleToggleFilterBarClick = (evt) => {
        this.props.toggleFilterBar();
        evt.preventDefault();
    }
    handleRefreshClick = (evt) => {
        this.props.refresh();
        evt.preventDefault();
    }
    handleVoidEvent = (evt) => {
        evt.preventDefault();
    }

    getCommitButton = () => {
        if (this.props.isEditingEnabled && this.props.isCommitEnabled && !this.props.isCommitButtonUnavailable)
            return <ToolbarButton key="commitButton" className="gr-toolbar-btn commit" onClick={this.handleCommitClick} label="General_Sec0_btn_ToolTip_commitButton" icon="fas fa-check" />;

        return <ToolbarButton key="commitButton" className="gr-toolbar-btn disabled" onClick={this.handleVoidEvent} label="General_Sec0_btn_ToolTip_commitButton" icon="fas fa-check" />;
    }
    getRollbackButton = () => {
        if (this.props.isEditingEnabled && this.props.isRollbackEnabled)
            return <ToolbarButton key="rollbackButton" className="gr-toolbar-btn rollback" onClick={this.handleRollbackClick} label="General_Sec0_btn_ToolTip_rollbackButton" icon="fas fa-undo-alt" />;

        return <ToolbarButton key="rollbackButton" className="gr-toolbar-btn disabled" onClick={this.handleVoidEvent} label="General_Sec0_btn_ToolTip_rollbackButton" icon="fas fa-undo-alt" />;
    }
    getDeleteButton = () => {
        if (this.props.isEditingEnabled && this.props.isRemoveEnabled)
            return <ToolbarButton key="deleteButton" className="gr-toolbar-btn delete" onClick={this.handleDeleteRowClick} label="General_Sec0_btn_ToolTip_deleteButton" icon="fas fa-trash-alt" />;

        return <ToolbarButton key="deleteButton" className="gr-toolbar-btn disabled" onClick={this.handleVoidEvent} label="General_Sec0_btn_ToolTip_deleteButton" icon="fas fa-trash-alt" />;
    }
    getAddButton = () => {
        if (this.props.isEditingEnabled && this.props.isAddEnabled)
            return <ToolbarButton key="addButton" className="gr-toolbar-btn add" onClick={this.handleAddRowClick} label="General_Sec0_btn_ToolTip_addButton" icon="fas fa-plus" />;

        return <ToolbarButton key="addButton" className="gr-toolbar-btn disabled" onClick={this.handleVoidEvent} label="General_Sec0_btn_ToolTip_addButton" icon="fas fa-plus" />;
    }
    getSearchButton = () => {
        return <ToolbarButton key="searchButton" className="gr-toolbar-btn filter" onClick={this.handleToggleFilterBarClick} label="General_Sec0_btn_ToolTip_searchButton" icon="fas fa-search" />;
    }
    getExecuteFilterButton = () => {
        return <ToolbarButton key="executeFilterButton" className="gr-toolbar-btn filter" onClick={this.handleApplyFilterClick} label="General_Sec0_btn_ToolTip_executeFilterButton" icon="fas fa-bolt" />;
    }

    render() {
        return (
            <div className="gr-action-toolbar">
                <ToolbarButton key="refreshButton" className="gr-toolbar-btn refresh" onClick={this.handleRefreshClick} label="General_Sec0_btn_ToolTip_refreshButton" icon="fas fa-sync-alt" />
                <ToolbarDivider />
                {this.getCommitButton()}
                {this.getRollbackButton()}
                <ToolbarDivider />
                {this.getDeleteButton()}
                {this.getAddButton()}
                <ToolbarDivider />
                {this.getSearchButton()}
                {this.getExecuteFilterButton()}
                <ToolbarDivider />
                <ToolbarMenu key="selectionMenu" title="Acciones sobre selección" icon="fas fa-bars" items={this.props.menuItems} performMenuItemAction={this.props.performMenuItemAction}/>
            </div>
        );
    }
}