import 'gasparesganga-jquery-loading-overlay';
import $ from 'jquery';
import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { withTranslation } from 'react-i18next';
import { ComponentError } from '../ComponentError';
import { columnFixed, componentType, filterStatus, gridStatus } from '../Enums';
import { ScrollSync } from '../ScrollSyncComponent';
import withCustomTranslation from '../WithCustomTranslation';
import { withPageContext } from '../WithPageContext';
import withToaster from '../WithToaster';
import { BodyPane } from './GridBodyPane';
import ColumnResizeMarker from './GridColumnResizeMarker';
import { GridContainer } from './GridContainer';
import { GridContentContainer } from './GridContentContainer';
import { GridDropdown } from './GridDropdown';
import { GridGuideFilterModal } from './GridGuideFilterModal';
import { HeaderPane } from './GridHeaderPane';
import { GridImportExcelModal } from './GridImportExcelModal';
import { GridImportExcelModalAPI } from './GridImportExcelModalAPI';
import { GridLoadFilterModal } from './GridLoadFilterModal';
import { LoadingSkeleton } from './GridLoadingSkeleton';
import { GridPanelContainer } from './GridPanelContainer';
import { GridSaveFilterModal } from './GridSaveFilterModal';
import { GridScrollPane } from './GridScrollPane';
import { GridStatsPanel } from './GridStatsPanel';
import { GridSuperFilterModal } from './GridSuperFilterModal';
import { Toolbar } from './GridToolbar';
import withGridDataProvider from './WithGridDataProvider';
import withScrollContext from './WithScrollContext';

const showLoadingOverlay = (id) => {
    let existe = $(".loadingoverlay").length > 0;
    if (!existe) {
        $("#" + id).LoadingOverlay("show", {
            image: "",
            fontawesome: "fa fa-cog fa-spin",
            background: "rgba(22, 25, 28, 0.2)"
        });
    }
}


const hideLoadingOverlay = (id) => {
    $("#" + id).LoadingOverlay("hide");
}
export class InternalGrid extends Component {
    static propTypes = {
        application: PropTypes.string,
        editable: PropTypes.bool,
        enableSelection: PropTypes.bool,
        notifySelection: PropTypes.bool,
        validateAllRows: PropTypes.bool,
        enableExcelExport: PropTypes.bool,
        enableExcelImport: PropTypes.bool,
        onBeforeInitialize: PropTypes.func,
        onAfterInitialize: PropTypes.func,
        onBeforeFetch: PropTypes.func,
        onAfterFetch: PropTypes.func,
        onBeforeCommit: PropTypes.func,
        onAfterCommit: PropTypes.func,
        onBeforeValidateRow: PropTypes.func,
        onAfterValidateRow: PropTypes.func,
        onBeforeButtonAction: PropTypes.func,
        onAfterButtonAction: PropTypes.func,
        onBeforeMenuItemAction: PropTypes.func,
        onAfterMenuItemAction: PropTypes.func,
        onBeforeApplyFilter: PropTypes.func,
        onAfterApplyFilter: PropTypes.func,
        onBeforeApplySort: PropTypes.func,
        onAfterApplySort: PropTypes.func,
        onBeforeExportExcel: PropTypes.func,
        onAfterExportExcel: PropTypes.func,
        onBeforeUpdateConfig: PropTypes.func,
        onAfterUpdateConfig: PropTypes.func,
        onBeforeSaveFilter: PropTypes.func,
        onAfterSaveFilter: PropTypes.func,
        onBeforeGetFilterList: PropTypes.func,
        onAfterGetFilterList: PropTypes.func,
        onBeforeRemoveFilter: PropTypes.func,
        onAfterRemoveFilter: PropTypes.func,
        onBeforeSelectSearch: PropTypes.func,
        onAfterSelectSearch: PropTypes.func,
        onBeforeFetchStats: PropTypes.func,
        onAfterFetchStats: PropTypes.func,
        onBeforeNotifySelection: PropTypes.func,
        onAfterNotifySelection: PropTypes.func,
        onBeforeNotifyInvertSelection: PropTypes.func,
        onAfterNotifyInvertSelection: PropTypes.func,
        rowFetchOverscan: PropTypes.number,
        rowOverscan: PropTypes.number,
        rowsToDisplay: PropTypes.number,
        rowsToFetch: PropTypes.number
    }

    static defaultProps = {
        application: null,
        enableSelection: false,
        notifySelection: false,
        validateAllRows: false, //TODO: Ver si esto hace falta
        enableExcelExport: false,
        enableExcelImport: false,
        editable: false,
        rowFetchOverscan: 30,
        rowOverscan: 30,
        rowsToDisplay: 20,
        rowsToFetch: 20,
        lookupAmount: 100,
        autofocus: false
    }

    constructor(props) {
        super(props);

        this.state = {
            columns: [],
            rows: [],
            filters: [],
            sorts: [],
            selection: [],
            highlight: [],
            menuItems: [],
            links: [],
            explicitFilter: "",
            application: "",
            filterList: null,
            activeFilter: null,

            dropdownRowId: null,
            dropdownColumnId: null,
            dropdownShow: false,
            dropdownTop: 0,
            dropdownLeft: 0,
            dropdownSpaceAvailableRight: 0,
            dropdownButtonWidth: 0,
            dropdownItems: [],

            lookup: [],
            lookupMargin: [],

            rowLastNewId: 0,
            rowHeight: 28,
            rowDisplayStart: 0,
            rowDisplayEnd: props.rowsToDisplay || props.rowsToFetch,
            rowCurrentIndex: 0,

            filterStatus: filterStatus.closed,
            superFilterStatus: filterStatus.closed,

            resizeParentOffset: 0,
            resizeWidthBase: 0,
            resizeWidth: 0,
            resizePositionInitial: 0,
            resizeCurrentPosition: 0,

            displaySelectionOnly: false,
            displayModifiedOnly: false,

            highlightLast: null,

            editingRow: null,
            editingColumn: null,

            isEditingEnabled: false,
            isCommitEnabled: false,
            isRollbackEnabled: false,
            isAddEnabled: false,
            isRemoveEnabled: false,
            isCommitButtonUnavailable: false,

            isSaveFilterModalOpen: false,
            isLoadFilterModalOpen: false,
            isGuideFilterModalOpen: false,

            isImportExcelModalOpen: false,
            isImportExcelModalAPIOpen: false,

            isStatsPanelOpen: false,

            isAllSelected: false,
            isFetching: false,
            isResizing: false,
            isUpdatingStructure: false,
            isInitializing: true
        };

        this.highlight = [];
        this.highlightLast = false;

        this.toolbarHeight = 29;
        this.headerHeight = 30;

        this.shouldPerformDisplayUpdate = false;
        this.offsetToUpdate = 0;
        this.lastOffset = 0;
        this.scrollWatcher = null;

        this.isCommitInProgress = false;

        this.scrollPaneRef = React.createRef();
        this.containerRef = React.createRef();
        this.bodyRef = React.createRef();
    }

    componentDidMount() {
        this.mounted = true;

        this.initialize();

        this.watchScroll();

        this.props.nexus.registerComponent(this.props.id, componentType.grid, this.getApi());
    }
    shouldComponentUpdate(nextProps) {
        return !nextProps.scrollContext.isScrolling;
    }
    componentWillUnmount() {
        this.unWatchScroll();

        this.props.nexus.unregisterComponent(this.props.id);

        this.mounted = false;
    }

    hasFixedColumnsLeft = () => {
        return this.state.columns.some(col => col.fixed === columnFixed.left);
    }
    hasFixedColumnsRight = () => {
        return this.state.columns.some(col => col.fixed === columnFixed.right);
    }

    addRow = (callback) => {
        if (!this.state.isEditingEnabled || !this.state.isAddEnabled)
            return;

        const rowId = this.state.rowLastNewId + 1;

        const newRow = {
            id: "newRow_" + rowId,
            index: -rowId,
            isNew: true,
            cells: this.state.columns.map(col =>
            ({
                column: col.id,
                value: col.defaultValue ? col.defaultValue : "",
                old: "",
                editable: col.insertable
            })
            )
        };

        this.setState({
            rows: [...this.state.rows.filter(d => d.isNew), newRow, ...this.state.rows.filter(d => !d.isNew)], //Ver si es necesario optimizar
            lookup: this.getUpdatedLookup([newRow]),
            rowLastNewId: rowId,
            rowDisplayEnd: Math.max(this.state.rows.length + 1, this.props.rowsToDisplay || this.props.rowsToFecth),
        }, callback);
    }
    getModifiedRows = () => {
        return this.state.rows.filter(row => row.isDeleted || row.cells.some(cell => cell.modified || (cell.editable && cell.old !== cell.value)));
    }
    anyModifiedRows = () => {
        return this.state.rows.some(row => row.isDeleted || row.cells.some(cell => cell.modified || (cell.editable && cell.old !== cell.value)));
    }
    getAllRows = () => {
        return this.state.rows;
    }
    getRows = (keys) => {
        return this.state.rows.filter(row => keys.some(key => key === row.id));
    }
    deleteRow = () => {
        if (!this.state.isEditingEnabled || !this.state.isRemoveEnabled)
            return;

        const rowIds = this.highlight.map(s => s.rowId);

        let rows = [...this.state.rows];
        let result = [];

        //TODO: < O(N^2) ver si es posible otimizar mas
        for (let i = 0; i < rows.length; i++) {
            for (let j = 0; j < rowIds.length; j++) {
                if (rows[i].id === rowIds[j]) {
                    rows[i].isDeleted = true;
                }
            }

            if (!rows[i].isNew || !rows[i].isDeleted)
                result.push(rows[i]);
        }

        this.setState({
            rows: result,
            lookup: this.getUpdatedLookup(result, true),
            rowDisplayEnd: Math.max(result.length, this.props.rowsToDisplay || this.props.rowsToFecth),
        });
    }
    updateRows = (rows) => {
        
        let stateRows = [...this.state.rows];

        let currentIndex = 0;

        for (let i = 0; i < rows.length; i++) {
            currentIndex = stateRows.findIndex(d => d.id === rows[i].id);

            if (currentIndex >= 0) {
                stateRows.splice(currentIndex, 1, rows[i]);
            }
        }

        this.setState({
            rows: stateRows,
            lookup: this.getUpdatedLookup(stateRows, true)
        }, () => this.isCommitInProgress = false);
    }

    updateRow = (row) => {
        let stateRows = [...this.state.rows];
        let currentIndex = stateRows.findIndex(d => d.id === row.id);

        row.isModified = row.cells.some(c => c.modified);

        if (currentIndex >= 0) {
            stateRows.splice(currentIndex, 1, row);
        }

        this.setState({
            rows: stateRows,
            editingRow: row.id,
            lookup: this.getUpdatedLookup(stateRows, true)
        }, () => {
            this.isCommitInProgress = false;
            this.validateRow(row);
        });
    }

    getRowHighlights = (rowId) => {
        var rowIndex = this.highlight.findIndex(d => d.rowId === rowId);

        if (rowIndex > -1)
            return this.highlight[rowIndex];

        return null;
    }
    toggleHighlight = (highlight, clearPrevious) => {
        this.highlightLast = highlight;

        let cellHighlight = clearPrevious ? [] : [...this.highlight];

        //Performance
        var rowIndex = cellHighlight.findIndex(d => d.rowId === highlight.rowId);

        if (rowIndex >= 0 && cellHighlight[rowIndex].columns) {
            var colIndex = cellHighlight[rowIndex].columns.indexOf(highlight.columnId);

            if (colIndex >= 0) {
                cellHighlight[rowIndex].columns = cellHighlight[rowIndex].columns.splice(colIndex, 1);
            }
            else {
                cellHighlight[rowIndex].columns.push(highlight.columnId);
            }
        }
        else {
            cellHighlight.push({
                rowId: highlight.rowId,
                index: highlight.index,
                columns: [highlight.columnId]
            });
        }

        this.highlight = cellHighlight;
    }
    clearHighlighted = () => {
        const cells = document.querySelectorAll("#" + this.props.id + ".gr-grid .gr-cell.selected");
        const rows = document.querySelectorAll("#" + this.props.id + ".gr-grid .gr-row.selected")

        if (cells) {
            for (var i = 0; i < cells.length; i++) {
                cells[i].classList.toggle("selected", false);
            }
        }

        if (rows) {
            for (var i = 0; i < rows.length; i++) {
                rows[i].classList.toggle("selected", false);
            }
        }
    }
    isHighlighted = (rowId, columnId) => {
        var rowIndex = this.highlight.findIndex(d => d.rowId === rowId);

        return rowIndex > -1 && this.highlight[rowIndex].columns.findIndex(d => d === columnId) > -1;
    }
    sortCellHighlight(a, b) {
        if ((a.fixed === 2 && b.fixed === 1) || (a.fixed === 1 && b.fixed === 3) || (a.fixed === 2 && b.fixed === 3)) {
            return -1;
        }

        if ((a.fixed === 3 && b.fixed === 1) || (a.fixed === 1 && b.fixed === 2) || (a.fixed === 3 && b.fixed === 2)) {
            return 1;
        }

        return 0;
    }
    getCellHighlight = (evt) => {
        evt.preventDefault();

        const highlight = [...this.highlight].sort((prev, next) => prev.index - next.index);
        const columns = [...this.state.columns].sort((a, b) => this.sortCellHighlight(a, b)).map(d => d.id);

        const columnList = columns.filter(col => highlight.some(sel => sel.columns.indexOf(col) >= 0));

        let rowIndex;
        let colOutput;

        const output = highlight.reduce((rowOutput, high) => {
            rowIndex = this.state.rows.findIndex(d => d.id === high.rowId);

            colOutput = columnList.reduce((output, col) => {
                return output + (high.columns.indexOf(col) >= 0 ? this.state.rows[rowIndex].cells.find(c => c.column === col).value : "") + "\t";
            }, "");

            return rowOutput + colOutput + "\n";
        }, "");

        navigator.clipboard.writeText(output);
    }

    initialize = (parameters) => {
        try {
            this.state.application = this.props.application;
            const data = {
                application: this.props.application,
                gridId: this.props.id,
                rowsToFetch: this.props.rowsToFetch,
                pageToken: this.props.nexus.getPageToken(),
                parameters: this.props.nexus.getQueryParameters() || []
            };

            if (parameters)
                data.parameters = [...data.parameters, ...parameters];

            const context = {
                abortServerCall: false
            };

            if (this.props.onBeforeInitialize)
                this.props.onBeforeInitialize(context, data, this.props.nexus);

            if (!this.mounted || context.abortServerCall)
                return false;

            this.props.gridInitialize(data).then(this.initializeProcessResponse);
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }
    initializeProcessResponse = (response) => {
        try {
            if (!response)
                return false;

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const data = JSON.parse(response.Data);

            let index = 0;
            let rows = [];

            const context = {
                abortUpdate: false,
            };

            if (this.props.onAfterInitialize)
                this.props.onAfterInitialize(context, data.grid, data.parameters, this.props.nexus);

            this.props.toaster.toastNotifications(data.notifications);

            if (!this.mounted || context.abortUpdate)
                return false;

            if (data.grid.rows.length > 0) {
                rows = this.appendRowData(data.grid.rows, true);
                index = data.grid.rows[data.grid.rows.length - 1].index;
            }

            if (data.links.length > 0) {
                for (let link of data.links) {
                    let column = data.grid.columns.find(d => d.id === link.column);

                    if (column) {
                        column.hasLink = true;
                    }
                }
            }

            this.setState({
                filters: data.filterData ? data.filterData.filters : [],
                sorts: data.filterData ? data.filterData.sorts : [],
                explicitFilter: data.filterData ? data.filterData.explicitFilter : null,
                activeFilter: data.filterData,
                columns: data.grid.columns,
                rows: rows,
                lookup: this.getUpdatedLookup(rows, true),
                menuItems: data.grid.menuItems,
                rowLastNewId: 0,
                rowDisplayStart: 0,
                rowDisplayEnd: Math.max(rows.length, this.props.rowsToDisplay || this.props.rowsToFetch),
                rowCurrentIndex: index,
                isInitializing: false,
                isEditingEnabled: data.isEditingEnabled,
                isCommitEnabled: data.isCommitEnabled,
                isRollbackEnabled: data.isRollbackEnabled,
                isAddEnabled: data.isAddEnabled,
                isRemoveEnabled: data.isRemoveEnabled,
                isCommitButtonUnavailable: data.isCommitButtonUnavailable,
                links: data.links
            });
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }

    fetchData = (evt, dontSkip, clearPrevious, parameters) => {
        if (this.state.isFetching)
            return false;

        this.setState({
            isFetching: true
        });

        try {
            const data = {
                application: this.props.application,
                gridId: this.props.id,
                pageToken: this.props.nexus.getPageToken(),
                filters: [...this.state.filters],
                explicitFilter: this.state.explicitFilter,
                sorts: [...this.state.sorts],
                rowsToSkip: dontSkip ? 0 : this.state.rows.length,
                rowsToFetch: this.props.rowsToFetch,
                parameters: this.props.nexus.getQueryParameters() || [],
            };

            if (parameters)
                data.parameters = [...data.parameters, ...parameters];

            const context = {
                abortServerCall: false
            };

            showLoadingOverlay(this.props.id);

            if (this.props.onBeforeFetch)
                this.props.onBeforeFetch(context, data, this.props.nexus);

            if (!this.mounted || context.abortServerCall) {
                hideLoadingOverlay(this.props.id);
                return false;
            }
            return this.props.gridFetchRows(data).then(this.fetchDataProcessResponse.bind(this, clearPrevious));
        }
        catch (ex) {
            hideLoadingOverlay(this.props.id);
            this.props.toaster.toastException(ex);

            this.setState({
                isFetching: false
            });
        }
    }
    fetchDataProcessResponse = (clearPrevious, response) => {
        try {
            if (!response)
                return false;

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const data = JSON.parse(response.Data);

            let newRows = data.rows;

            const context = {
                abortUpdate: false,
            };

            if (this.props.onAfterFetch)
                this.props.onAfterFetch(context, newRows, data.parameters, this.props.nexus);

            this.props.toaster.toastNotifications(data.notifications);
            hideLoadingOverlay(this.props.id);
            if (!this.mounted)
                return false;

            if (!context.abortUpdate && newRows) {
                if (!clearPrevious)
                    newRows = this.removeDuplicates(newRows);

                newRows = this.appendRowData(newRows, clearPrevious);

                if (clearPrevious) {
                    this.setState(prevState => ({
                        rows: [...newRows],
                        lookup: this.getUpdatedLookup(newRows, true),
                        rowCurrentIndex: newRows.length > 0 ? newRows[newRows.length - 1].index : 0,
                        isFetching: false,
                        rowDisplayStart: 0,
                        rowDisplayEnd: Math.max(newRows.length, this.props.rowsToDisplay || this.props.rowsToFecth),
                    }));
                }
                else {
                    this.setState(prevState => ({
                        rows: [...prevState.rows || [], ...newRows],
                        lookup: this.getUpdatedLookup(newRows),
                        rowCurrentIndex: newRows.length > 0 ? newRows[newRows.length - 1].index : prevState.rowCurrentIndex,
                        isFetching: false,
                    }));
                }
            }
            else {
                this.setState({
                    isFetching: false
                });
            }
        }
        catch (ex) {
            hideLoadingOverlay(this.props.id);
            this.props.toaster.toastException(ex);

            this.setState({
                isFetching: false
            });
        }
    }

    fetchStats = () => {
        try {
            const data = {
                application: this.props.application,
                gridId: this.props.id,
                pageToken: this.props.nexus.getPageToken(),
                filters: [...this.state.filters],
                explicitFilter: this.state.explicitFilter,
                sorts: [...this.state.sorts],
                parameters: this.props.nexus.getQueryParameters() || [],
            };

            const context = {
                abortServerCall: false
            };

            if (this.props.onBeforeFetchStats)
                this.props.onBeforeFetchStats(context, data, this.props.nexus);

            if (!this.mounted || context.abortServerCall)
                return false;

            return this.props.gridFetchStats(data).then(this.fetchStatsProcessResponse);
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }
    fetchStatsProcessResponse = (response) => {
        try {
            if (!response)
                return false;

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const data = JSON.parse(response.Data);

            console.log(data);

            let stats = data.stats;

            const context = {

            };

            if (this.props.onAfterFetchStats)
                this.props.onAfterFetchStats(context, stats, data.parameters, this.props.nexus);

            this.props.toaster.toastNotifications(data.notifications);

            if (!this.mounted)
                return false;

            return stats;
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }

    appendRowData = (rows, resetCounter) => {
        let currentIndex = this.state.rowCurrentIndex;

        let result = [...rows];

        if (resetCounter)
            currentIndex = 0;

        for (let i = 0; i < result.length; i++) {
            result[i].index = currentIndex++;
        }

        return result;
    }

    validateRow = (row) => {
        if (!this.state.isEditingEnabled)
            return;

        try {
            const data = {
                application: this.props.application,
                gridId: this.props.id,
                pageToken: this.props.nexus.getPageToken(),
                row: row,
                parameters: this.props.nexus.getQueryParameters() || []
            };

            const context = {
                abortServerCall: false
            };

            if (this.props.onBeforeValidateRow)
                this.props.onBeforeValidateRow(context, data, this.props.nexus);

            if (!this.mounted || context.abortServerCall)
                return false;

            this.props.gridValidateRow(data).then(this.validateRowProcessResponse);
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }

    validateRowProcessResponse = (response) => {
        try {
            if (!response)
                return false;

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const data = JSON.parse(response.Data);

            const context = {
                abortUpdate: false
            };

            if (this.props.onAfterValidateRow)
                this.props.onAfterValidateRow(context, data.row, data.parameters, this.props.nexus);

            this.props.toaster.toastNotifications(data.notifications);

            if (!this.mounted || context.abortUpdate)
                return false;

            const rowIndex = this.state.rows.findIndex(r => r.id === data.row.id);

            if (rowIndex > -1) {
                const updatedRows = [
                    ...this.state.rows.slice(0, rowIndex),
                    {
                        ...data.row,
                        index: this.state.rows[rowIndex].index
                    },
                    ...this.state.rows.slice(rowIndex + 1)
                ];

                this.setState({
                    rows: updatedRows,
                    lookup: this.getUpdatedLookup(updatedRows, true)
                });
            }
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }

    commit = (allow, overrideCommitSetting) => {
        if (!this.state.isEditingEnabled || (!overrideCommitSetting && !this.state.isCommitEnabled) || this.isCommitInProgress)
            return;

        if (!allow) {
            this.props.nexus.showConfirmation({
                message: "General_Sec0_Info_DeseaGuardarCambios",
                onAccept: () => this.commit(true)
            });

            return false;
        }

        this.isCommitInProgress = true;

        try {
            var data = {
                application: this.props.application,
                gridId: this.props.id,
                pageToken: this.props.nexus.getPageToken(),
                rows: this.props.validateAllRows ? this.state.rows : this.getModifiedRows(),
                filters: this.state.filters,
                explicitFilter: this.state.explicitFilter,
                sorts: this.state.sorts,
                rowsToFetch: this.props.rowsToFetch,
                parameters: this.props.nexus.getQueryParameters() || []
            };

            const context = {
                abortServerCall: false
            };

            showLoadingOverlay(this.props.id);

            if (this.props.onBeforeCommit)
                this.props.onBeforeCommit(context, data, this.props.nexus);

            if (!this.mounted || context.abortServerCall) {
                this.isCommitInProgress = false;
                hideLoadingOverlay(this.props.id);
                return false;
            }

            this.props.gridCommit(data).then(this.commitProcessResponse);
        }
        catch (ex) {
            hideLoadingOverlay(this.props.id);
            this.isCommitInProgress = false;

            this.props.toaster.toastException(ex);
        }
    }
    commitProcessResponse = (response) => {
        try {
            if (!response)
                return false;

            const data = JSON.parse(response.Data);

            if (response.Status === "ERROR" && data === null)
                throw new ComponentError(response.MessageArguments, response.Message);

            let rows = data.rows;

            const context = {
                abortUpdate: false,
                status: response.Status,
                abortHideLoading: false
            };

            if (this.props.onAfterCommit)
                this.props.onAfterCommit(context, rows, data.parameters, this.props.nexus);

            this.props.toaster.toastNotifications(data.notifications);

            if (!context.abortHideLoading)
                hideLoadingOverlay(this.props.id);

            if (!this.mounted || context.abortUpdate) {
                this.isCommitInProgress = false;

                return false;
            }

            if (rows) {
                rows = this.appendRowData(rows, true);
            }

            const index = rows.length > 0 ? rows[rows.length - 1].index : 0;

            if (response.Status === "OK") {
                this.setState({
                    rows: rows,
                    lookup: this.getUpdatedLookup(rows, true),
                    rowCurrentIndex: index,
                    isAllSelected: false,
                    selection: []
                }, () => this.isCommitInProgress = false);
            }
            else if (response.Status === "ERROR") {
                this.props.toaster.toastError(response.Message);

                this.updateRows(rows);
            }
        }
        catch (ex) {
            hideLoadingOverlay(this.props.id);
            this.isCommitInProgress = false;

            this.props.toaster.toastException(ex);
        }
    }

    performButtonAction = (rowId, columnId, btnId, allow, ctrlKey, parameters) => {
        try {
            const row = { ...this.state.rows.find(row => row.id === rowId) };

            if (!allow) {
                const column = this.state.columns.find(d => d.id === columnId);
                const buttons = column.items || column.buttons;

                if (buttons) {
                    const button = buttons.find(d => d.id === btnId);

                    if (button && button.confirmMessage) {
                        this.props.nexus.showConfirmation({
                            message: button.confirmMessage.message,
                            acceptVariant: button.confirmMessage.acceptVariant,
                            acceptLabel: button.confirmMessage.acceptLabel,
                            cancelVariant: button.confirmMessage.cancelVariant,
                            cancelLabel: button.confirmMessage.cancelLabel,
                            onAccept: () => this.performButtonAction(rowId, columnId, btnId, true, ctrlKey)
                        });

                        //return false;
                        return Promise.resolve(); // FIX para los mensajes de confirmación - VER con Mauro si esto esta bien
                    }
                }
            }

            const data = {
                application: this.props.application,
                gridId: this.props.id,
                pageToken: this.props.nexus.getPageToken(),
                buttonId: btnId,
                columnId: columnId,
                row: row,
                parameters: this.props.nexus.getQueryParameters() || []
            };

            if (parameters != null) {
                data.parameters = data.parameters.concat(parameters);
            }

            const context = {
                abortServerCall: false,
                ctrlKey: ctrlKey
            };

            showLoadingOverlay(this.props.id);

            if (this.props.onBeforeButtonAction)
                this.props.onBeforeButtonAction(context, data, this.props.nexus);

            if (!this.mounted || context.abortServerCall) {
                hideLoadingOverlay(this.props.id);
                return Promise.resolve(false);
            }

            return this.props.gridButtonAction(data).then(this.performButtonActionProcessResponse.bind(this, ctrlKey));
        }
        catch (ex) {
            console.log(ex);
            hideLoadingOverlay(this.props.id);
            this.props.toaster.toastException(ex);
        }
    }
    performButtonActionProcessResponse = (ctrlKey, response) => {
        try {
            if (!response)
                return false;

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const data = JSON.parse(response.Data);


            if (data.redirection) {
                if (data.redirection.parameters == null) {
                    data.redirection.parameters = [];
                }

                this.props.nexus.redirect(data.redirection.url, data.redirection.newTab || ctrlKey, data.redirection.parameters);
            }

            if (this.props.onAfterButtonAction)
                this.props.onAfterButtonAction(data, this.props.nexus);

            hideLoadingOverlay(this.props.id);

            this.props.toaster.toastNotifications(data.notifications);
        }
        catch (ex) {
            hideLoadingOverlay(this.props.id);
            this.props.toaster.toastException(ex);
        }
    }

    performMenuItemAction = (btnId, allow, ctrlKey) => {
        try {
            if (!allow) {
                const menuItem = this.state.menuItems.find(d => d.id === btnId);

                if (menuItem && menuItem.confirmMessage) {
                    this.props.nexus.showConfirmation({
                        message: menuItem.confirmMessage.message,
                        acceptVariant: menuItem.confirmMessage.acceptVariant,
                        acceptLabel: menuItem.confirmMessage.acceptLabel,
                        cancelVariant: menuItem.confirmMessage.cancelVariant,
                        cancelLabel: menuItem.confirmMessage.cancelLabel,
                        onAccept: () => this.performMenuItemAction(btnId, true)
                    });

                    return false;
                }
            }

            const selection = {
                allSelected: this.state.isAllSelected,
                keys: this.state.selection
            };

            const data = {
                application: this.props.application,
                gridId: this.props.id,
                pageToken: this.props.nexus.getPageToken(),
                buttonId: btnId,
                filters: this.state.filters,
                explicitFilter: this.state.explicitFilter,
                selection: selection,
                parameters: this.props.nexus.getQueryParameters() || []
            };

            const context = {
                abortServerCall: false,
                ctrlKey: ctrlKey
            };
            showLoadingOverlay(this.props.id);

            if (this.props.onBeforeMenuItemAction)
                this.props.onBeforeMenuItemAction(context, data, this.props.nexus);

            if (!this.mounted || context.abortServerCall) {
                hideLoadingOverlay(this.props.id);
                return false;
            }
            this.props.gridMenuItemAction(data).then(this.performMenuItemActionProcessResponse.bind(this, ctrlKey));
        }
        catch (ex) {
            hideLoadingOverlay(this.props.id);
            this.props.toaster.toastException(ex);
        }
    }
    performMenuItemActionProcessResponse = (ctrlKey, response) => {
        try {
            if (!response)
                return false;

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const data = JSON.parse(response.Data);

            const context = {
                abortUpdate: false,
                forceRefresh: false,
                ctrlKey: ctrlKey,
                abortHideLoading : false
            };

            if (this.props.onAfterMenuItemAction)
                this.props.onAfterMenuItemAction(context, data, this.props.nexus);

            this.props.toaster.toastNotifications(data.notifications);

            if (!context.abortHideLoading)
                hideLoadingOverlay(this.props.id);

            if (!this.mounted || context.abortUpdate)
                return false;

            if (data.redirection) {
                this.props.nexus.redirect(data.redirection.url, data.redirection.newTab || ctrlKey, data.redirection.parameters);
            }

            this.setState({
                isAllSelected: false,
                selection: []
            });

            if (context.forceRefresh)
                this.refresh();

        }
        catch (ex) {
            hideLoadingOverlay(this.props.id);
            this.props.toaster.toastException(ex);
        }
    }

    openRelatedLink = (rowId, columnId) => {
        const link = this.state.links.find(d => d.column === columnId);

        if (link) {
            const row = this.state.rows.find(d => d.id === rowId);

            const parameters = link.propertyMapping.map(d => ({
                id: d.mappedColumn,
                value: row.cells.find(e => e.column === d.column).value
            }));

            console.log(row);

            this.props.nexus.redirect(link.url, true, parameters);
        }
    }

    updateFilter = (columnId, value, callback) => {
        const filters = [...this.state.filters];

        const index = filters.findIndex(filter => filter.columnId === columnId);

        if (index >= 0) {
            if (value) {
                this.setState({
                    filters: [
                        ...filters.slice(0, index),
                        {
                            ...filters[index],
                            value: value
                        },
                        ...filters.slice(index + 1)
                    ],
                    activeFilter: null
                }, callback);
            }
            else {
                this.setState({
                    filters: filters.filter(f => f.columnId !== columnId) || []
                }, callback);
            }
        }
        else {
            this.setState({
                filters: [
                    ...filters,
                    {
                        columnId: columnId,
                        value: value
                    }
                ],
                activeFilter: null
            }, callback);
        }
    }
    applyFilter = (allow) => {
        try {
            if (this.state.isFetching)
                return false;

            if (!allow && this.anyModifiedRows()) {
                this.props.nexus.showConfirmation({
                    message: "General_Sec0_Info_DeseaAnularCambios",
                    acceptLabel: "General_Sec0_btn_DeshacerCambios",
                    onAccept: () => this.refresh([], true)
                });

                return false;
            }

            this.setState({
                isFetching: true
            });

            const data = {
                application: this.props.application,
                gridId: this.props.id,
                pageToken: this.props.nexus.getPageToken(),
                filters: this.state.filters,
                explicitFilter: this.state.explicitFilter,
                sorts: this.state.sorts,
                rowsToSkip: 0,
                rowsToFetch: this.props.rowsToFetch,
                parameters: this.props.nexus.getQueryParameters() || []
            };

            const context = {
                abortServerCall: false
            };
            showLoadingOverlay(this.props.id);

            if (this.props.onBeforeApplyFilter)
                this.props.onBeforeApplyFilter(context, data, this.props.nexus);

            if (!this.mounted && context.abortServerCall) {
                hideLoadingOverlay(this.props.id);
                return false;
            }
            this.props.gridFetchRows(data).then(this.applyFilterProcessResponse);
        }
        catch (ex) {
            hideLoadingOverlay(this.props.id);
            this.props.toaster.toastException(ex);

            this.setState({
                isFetching: false
            });
        }
    }
    applyFilterProcessResponse = (response) => {
        try {
            if (!response)
                return false;

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const data = JSON.parse(response.Data);

            let newRows = data.rows;

            const context = {
                abortUpdate: false
            };

            if (this.props.onAfterApplyFilter)
                this.props.onAfterApplyFilter(context, newRows, data, this.props.nexus);

            this.props.toaster.toastNotifications(data.notifications);
            hideLoadingOverlay(this.props.id);

            if (!this.mounted)
                return false;

            if (!context.abortUpdate && newRows !== undefined && newRows !== null) {
                this.updateRowsFilter(newRows);
            }
            else {
                this.setState({
                    isFetching: false
                });
            }
        }
        catch (ex) {
            hideLoadingOverlay(this.props.id);
            this.props.toaster.toastException(ex);

            this.setState({
                isFetching: false
            });
        }
    }

    applySort = (columnId, allow) => {
        try {
            if (this.state.isFetching)
                return false;

            if (!allow && this.anyModifiedRows()) {
                this.props.nexus.showConfirmation({
                    message: "General_Sec0_Info_DeseaAnularCambios",
                    acceptLabel: "General_Sec0_btn_DeshacerCambios",
                    onAccept: () => this.applySort(columnId, true)
                });

                return false;
            }

            this.setState({
                isFetching: true
            });

            const sorts = this.getNewSorts(columnId);

            this.applySortRequest(sorts);
        }
        catch (ex) {
            this.props.toaster.toastException(ex);

            this.setState({
                isFetching: false
            });
        }
    };
    applySortAscending = (columnId, allow) => {
        try {
            if (this.state.isFetching)
                return false;

            if (!allow && this.anyModifiedRows()) {
                this.props.nexus.showConfirmation({
                    message: "General_Sec0_Info_DeseaAnularCambios",
                    acceptLabel: "General_Sec0_btn_DeshacerCambios",
                    onAccept: () => this.applySortAscending(columnId, true)
                });

                return false;
            }

            this.setState({
                isFetching: true
            });

            let sorts = [...this.state.sorts];

            let currSort = sorts.find(d => d.columnId === columnId);

            if (currSort) {
                currSort.direction = 1;
            }
            else {
                sorts = [...sorts, {
                    columnId: columnId,
                    direction: 1
                }];
            }

            this.applySortRequest(sorts);
        }
        catch (ex) {
            this.props.toaster.toastException(ex);

            this.setState({
                isFetching: false
            });
        }
    };
    applySortDescending = (columnId, allow) => {
        try {
            if (this.state.isFetching)
                return false;

            if (!allow && this.anyModifiedRows()) {
                this.props.nexus.showConfirmation({
                    message: "General_Sec0_Info_DeseaAnularCambios",
                    acceptLabel: "General_Sec0_btn_DeshacerCambios",
                    onAccept: () => this.applyDescending(columnId, true)
                });

                return false;
            }

            this.setState({
                isFetching: true
            });

            let sorts = [...this.state.sorts];

            let currSort = sorts.find(d => d.columnId === columnId);

            if (currSort) {
                currSort.direction = 2;
            }
            else {
                sorts = [...sorts, {
                    columnId: columnId,
                    direction: 2
                }];
            }

            this.applySortRequest(sorts);
        }
        catch (ex) {
            this.props.toaster.toastException(ex);

            this.setState({
                isFetching: false
            });
        }
    };
    applySortReset = (columnId, allow) => {
        try {
            if (this.state.isFetching)
                return false;

            if (!allow && this.anyModifiedRows()) {
                this.props.nexus.showConfirmation({
                    message: "General_Sec0_Info_DeseaAnularCambios",
                    acceptLabel: "General_Sec0_btn_DeshacerCambios",
                    onAccept: () => this.applySortReset(columnId, true)
                });

                return false;
            }

            this.setState({
                isFetching: true
            });

            let sorts = [...this.state.sorts];

            sorts = sorts.filter(d => d.columnId !== columnId);

            this.applySortRequest(sorts);
        }
        catch (ex) {
            this.props.toaster.toastException(ex);

            this.setState({
                isFetching: false
            });
        }
    };
    applySortRequest = (sorts) => {
        const data = {
            application: this.props.application,
            gridId: this.props.id,
            pageToken: this.props.nexus.getPageToken(),
            filters: this.state.filters,
            explicitFilter: this.state.explicitFilter,
            sorts: sorts,
            rowsToSkip: 0,
            rowsToFetch: this.props.rowsToFetch,
            parameters: this.props.nexus.getQueryParameters() || []
        };

        const context = {
            abortServerCall: false
        };

        if (this.props.onBeforeApplySort)
            this.props.onBeforeApplySort(context, data, this.props.nexus);

        if (!this.mounted || context.abortServerCall)
            return false;

        this.props.gridFetchRows(data).then(this.applySortProcessResponse.bind(this, sorts));
    }
    applySortProcessResponse = (sorts, response) => {
        try {
            if (!response)
                return false;

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const data = JSON.parse(response.Data);

            let rows = data.rows;

            const context = {
                abortUpdate: false,
                gridId: this.props.id,
            };

            if (this.props.onAfterApplySort)
                this.props.onAfterApplySort(context, rows, data.parameters);

            this.props.toaster.toastNotifications(data.notifications);

            if (!this.mounted)
                return false;

            if (!context.abortUpdate && rows !== undefined && rows !== null) {
                this.updateRowsSort(rows, sorts);
            }
            else {
                this.setState({
                    isFetching: false
                });
            }
        }
        catch (ex) {
            this.props.toaster.toastException(ex);

            this.setState({
                isFetching: false
            });
        }
    };
    getNewSorts = (columnId) => {
        const sorts = [...this.state.sorts];
        const index = sorts.findIndex(sort => sort.columnId === columnId);
        let newSorts = [];

        if (index >= 0) {
            if (sorts[index].direction === 1) {
                newSorts = [
                    ...sorts.slice(0, index),
                    {
                        ...sorts[index],
                        direction: 2
                    },
                    ...sorts.slice(index + 1)
                ];
            }
            else {
                newSorts = sorts.filter(sort => sort.columnId !== columnId);
            }
        }
        else {
            newSorts = [
                ...sorts,
                {
                    columnId: columnId,
                    direction: 1
                }
            ];
        }

        return newSorts;
    }

    updateGridConfig = () => {
        try {
            const data = {
                application: this.props.application,
                gridId: this.props.id,
                pageToken: this.props.nexus.getPageToken(),
                columns: this.state.columns,
                parameters: this.props.nexus.getQueryParameters() || []
            };

            const context = {
                abortServerCall: false
            };

            if (this.props.onBeforeUpdateGridConfig)
                this.props.onBeforeUpdateGridConfig(context, data, this.props.nexus);

            if (!this.mounted || context.abortServerCall)
                return false;

            this.props.gridUpdateConfig(data).then(this.updateGridConfigResponse);
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }
    updateGridConfigResponse = (response) => {
        try {
            if (!response)
                return false;

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const context = {
            };

            if (this.props.onAfterUpdateGridConfig)
                this.props.onAfterUpdateGridConfig(context, this.props.nexus);

            if (!this.mounted)
                return false;
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }

    exportExcel = (fileName, type) => {
        if (this.state.isFetching)
            return false;

        this.setState({
            isFetching: true
        });

        try {
            const data = {
                application: this.props.application,
                gridId: this.props.id,
                pageToken: this.props.nexus.getPageToken(),
                fileName: fileName,
                type: type,
                filters: [...this.state.filters],
                explicitFilter: this.state.explicitFilter,
                sorts: [...this.state.sorts],
                parameters: this.props.nexus.getQueryParameters() || []
            };

            const context = {
                abortServerCall: false
            };
            showLoadingOverlay(this.props.id);

            if (this.props.onBeforeExportExcel)
                this.props.onBeforeExportExcel(context, data, this.props.nexus);

            if (!this.mounted || context.abortServerCall) {
                hideLoadingOverlay(this.props.id);
                return false;
            }
            return this.props.gridExportExcel(data).then(this.exportExcelProcessResponse);
        }
        catch (ex) {
            hideLoadingOverlay(this.props.id);
            this.props.toaster.toastException(ex);

            this.setState({
                isFetching: false
            });
        }
    }
    exportExcelProcessResponse = (response) => {
        try {
            if (!response)
                return false;

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const context = {
                abortDownload: false
            };

            if (this.props.onAfterExportExcel)
                this.props.onAfterExportExcel(context, this.props.nexus);
            hideLoadingOverlay(this.props.id);

            if (!context.abortDownload)
                window.location = "/api/Grid/DownloadExcel";
        }
        catch (ex) {
            hideLoadingOverlay(this.props.id);
            this.props.toaster.toastException(ex);
        }

        this.setState({
            isFetching: false
        });
    }

    updateRowsFilter = (rows) => {
        if (rows) {
            rows = this.appendRowData(rows, true);
        }

        const index = rows.length > 0 ? rows[rows.length - 1].index : 0;

        this.setState({
            rows: [...rows],
            lookup: this.getUpdatedLookup(rows, true),
            rowDisplayStart: 0,
            rowDisplayEnd: Math.max(rows.length, this.props.rowsToDisplay || this.props.rowsToFetch),
            rowCurrentIndex: index,
            isFetching: false
        }, () => {
            const body = document.querySelector("#" + this.props.id + " .gr-body-scroll");

            if (body) {
                body.scrollLeft = this.scrollPaneRef.scrollLeft;
            }
        });
    }
    updateRowsSort = (rows, sorts) => {
        if (rows) {
            rows = this.appendRowData(rows, true);
        }

        const index = rows.length > 0 ? rows[rows.length - 1].index : 0;

        this.setState({
            rows: [...rows],
            lookup: this.getUpdatedLookup(rows, true),
            rowDisplayStart: 0,
            rowDisplayEnd: Math.max(rows.length, this.props.rowsToDisplay || this.props.rowsToFetch),
            rowCurrentIndex: index,
            sorts: sorts,
            isFetching: false
        });
    }

    rollback = (allow) => {
        if (!this.state.isEditingEnabled || !this.state.isRollbackEnabled)
            return;

        if (!allow && this.state.rows.some(d => d.isNew || d.isDeleted || d.isModified)) {
            this.props.nexus.showConfirmation({
                message: "General_Sec0_Info_DeseaDeshacerCambios",
                onAccept: () => {
                    this.rollback(true)
                }
            });

            return false;
        }

        const rows = this.state.rows.filter(row => !row.isNew);

        const updatedRows = rows.map(row => ({
            ...row,
            isDeleted: false,
            isModified: false,
            cells: row.cells.map(cell => ({
                ...cell,
                value: cell.old,
                status: gridStatus.ok
            }))
        }));

        this.setState({
            rows: updatedRows,
            lookup: this.getUpdatedLookup(updatedRows, true),
            rowDisplayEnd: Math.max(updatedRows.length, this.props.rowsToDisplay || this.props.rowsToFetch),
        });
    }
    refresh = (parameters, allow) => {
        if (!allow && this.anyModifiedRows()) {
            this.props.nexus.showConfirmation({
                message: "General_Sec0_Info_DeseaAnularCambios",
                acceptLabel: "General_Sec0_btn_DeshacerCambios",
                onAccept: () => this.refresh(parameters, true)
            });

            return false;
        }

        this.fetchData(null, true, true, parameters);
    }
    forceRefresh = (parameters) => {
        this.refresh(parameters, true);
    }

    reset = (parameters) => {
        this.initialize(parameters);
    }

    updateCellValue = (rowId, columnId, value) => {
        if (!this.state.isEditingEnabled)
            return;

        const rows = [...this.state.rows];

        const rowIndex = rows.findIndex(row => row.id === rowId);

        if (rowIndex > -1) {
            const cells = [...rows[rowIndex].cells];

            const cellIndex = cells.findIndex(cell => cell.column === columnId);

            const updatedRows = [
                ...rows.slice(0, rowIndex),
                {
                    ...rows[rowIndex],
                    cells: [
                        ...cells.slice(0, cellIndex),
                        {
                            ...cells[cellIndex],
                            value: value,
                            modified: true
                        },
                        ...cells.slice(cellIndex + 1)
                    ],
                    isModified: true //Por optimización. Uso local en js
                },
                ...rows.slice(rowIndex + 1)
            ];

            this.setState({
                rows: updatedRows,
                lookup: this.getUpdatedLookup(updatedRows, true)
            }, () => this.validateRow(updatedRows[rowIndex]));
        }
    }

    columnResizeChange = (mousePosition) => {
        if (!this.state.resizeInverted) {
            this.setState(prevState => ({
                resizeWidth: prevState.resizeWidthBase + (mousePosition - prevState.resizePositionInitial)
            }));
        }
        else {
            this.setState(prevState => ({
                resizeWidth: prevState.resizeWidthBase - (mousePosition - prevState.resizePositionInitial)
            }));
        }
    }
    columnResizeBegin = (columnId, parentOffset, initialOffset, mousePositionInitial, inverted) => {
        this.setState({
            isResizing: true,
            resizeColumnId: columnId,
            resizeParentOffset: parentOffset,
            resizeWidthBase: initialOffset,
            resizeWidth: initialOffset,
            resizePositionInitial: mousePositionInitial,
            resizeInverted: inverted
        });
    }
    columnResizeEnd = () => {
        const index = this.state.columns.findIndex(col => col.id === this.state.resizeColumnId);

        this.setState(prevState => ({
            isResizing: false,
            columns: [
                ...prevState.columns.slice(0, index),
                {
                    ...prevState.columns[index],
                    width: prevState.resizeWidth
                },
                ...prevState.columns.slice(index + 1)
            ]
        }), () => this.updateGridConfig());
    }
    updateColumnOrder = (columnId, targetColumn, previousIndex, nextIndex, rightHalf) => {
        if (previousIndex === nextIndex)
            return false;

        const targetIndex = this.state.columns.findIndex(col => col.id === targetColumn);
        let target;

        let columns = this.state.columns.map(column => {
            if (rightHalf) {
                if (previousIndex > nextIndex) {
                    if (column.id === columnId) {
                        target = this.state.columns[targetIndex + 1];

                        return { ...column, order: nextIndex + 1, fixed: target ? target.fixed : column.fixed };
                    }

                    if (column.order < previousIndex && column.order > nextIndex) {
                        return { ...column, order: column.order + 1 };
                    }
                }
                else {
                    if (column.id === columnId) {
                        target = this.state.columns[targetIndex];

                        return { ...column, order: nextIndex, fixed: target ? target.fixed : column.fixed };
                    }

                    if (column.order > previousIndex && column.order <= nextIndex) {
                        return { ...column, order: column.order - 1 };
                    }
                }
            }
            else {
                if (previousIndex > nextIndex) {
                    if (column.id === columnId) {
                        target = this.state.columns[targetIndex];

                        return { ...column, order: nextIndex, fixed: target ? target.fixed : column.fixed };
                    }

                    if (column.order < previousIndex && column.order >= nextIndex) {
                        return { ...column, order: column.order + 1 };
                    }
                }
                else {
                    if (column.id === columnId) {
                        target = this.state.columns[targetIndex];

                        return { ...column, order: nextIndex - 1, fixed: target ? target.fixed : column.fixed };
                    }

                    if (column.order > previousIndex && column.order < nextIndex) {
                        return { ...column, order: column.order - 1 };
                    }
                }
            }

            return { ...column };
        });

        columns.sort((a, b) => a.order - b.order);

        this.setState({
            columns: columns
        }, () => this.updateGridConfig());
    }
    fixColumn = (columnId, position) => {
        const count = this.state.columns.reduce((prev, col) => !col.hidden && col.fixed === columnFixed.none ? prev + 1 : prev, 0);

        if (count < 2) {
            this.props.toaster.toastWarning(this.props.t("General_Sec0_Error_ImposibleFijarColumna"));
            return;
        }

        const columnIndex = this.state.columns.findIndex(col => col.id === columnId);

        const columns = this.state.columns.map((column, index) => index === columnIndex ? { ...column, fixed: position } : { ...column });

        this.setState({
            columns: columns
        }, () => this.updateGridConfig());
    }
    hideColumn = (columnId) => {
        const visibleNonFixedColumns = this.state.columns.reduce((prev, col) => (!col.hidden && col.fixed === columnFixed.none) ? prev + 1 : prev, 0);

        if (visibleNonFixedColumns < 2) {
            this.props.toaster.toastWarning(this.props.t("General_Sec0_Error_ImposibleOcultarColumna"));
            return;
        }

        const columnIndex = this.state.columns.findIndex(col => col.id === columnId);

        const columns = this.state.columns.map((column, index) => index === columnIndex ? { ...column, hidden: true, fixed: columnFixed.none } : { ...column });

        this.setState({
            columns: columns
        }, () => this.updateGridConfig());
    }
    showColumn = (columnId) => {
        const columnIndex = this.state.columns.findIndex(col => col.id === columnId);

        const columns = this.state.columns.map((column, index) => index === columnIndex ? { ...column, hidden: false } : { ...column });

        this.setState({
            columns: columns
        }, () => this.updateGridConfig());
    }

    removeDuplicates = (rows) => {
        return rows.filter(r => this.state.lookup.indexOf(r.id) === -1);
    }
    getUpdatedLookup = (rows, reset) => {
        const rowIds = rows.map(r => r.id);

        if (!reset) {
            if (rowIds.length > 0) {
                return [...this.state.lookup, ...rowIds];
            } else {
                return this.state.lookup;
            }
        }

        return rowIds;
    }

    getResizeMarkerPosition() {
        return this.state.resizeInverted ? this.state.resizeParentOffset + (this.state.resizeWidthBase - this.state.resizeWidth)
            : this.state.resizeParentOffset + this.state.resizeWidth;
    }
    getResizeMarkerHeight(scrollbarHeight) {
        if (this.containerRef.current) {
            return this.containerRef.current.getBoundingClientRect().height - scrollbarHeight;
        }

        return 0;
    }

    updateSelection = (rowId) => {
        if (this.state.selection.indexOf(rowId) >= 0) {
            this.setState(prevState => ({
                selection: prevState.selection.filter(s => s !== rowId),
            }), this.notifySelection.bind(this, rowId));
        }
        else {
            this.setState(prevState => ({
                selection: [
                    ...prevState.selection,
                    rowId
                ]
            }), this.notifySelection.bind(this, rowId));
        }
    }
    notifySelection = (rowId) => {
        if (!this.props.notifySelection)
            return false;

        try {
            const selection = {
                allSelected: this.state.isAllSelected,
                keys: this.state.selection
            };

            const data = {
                application: this.props.application,
                gridId: this.props.id,
                pageToken: this.props.nexus.getPageToken(),
                filters: this.state.filters,
                explicitFilter: this.state.explicitFilter,
                sorts: this.state.sorts,
                rowId: rowId,
                selection: selection,
                parameters: this.props.nexus.getQueryParameters() || []
            };

            const row = { ...this.state.rows.find(d => d.id === rowId) };

            const context = {
                abortServerCall: false,
                row: row
            };

            if (this.props.onBeforeNotifySelection)
                this.props.onBeforeNotifySelection(context, data, this.props.nexus);

            if (!this.mounted || context.abortServerCall)
                return false;

            this.props.gridNotifySelection(data).then(this.notifySelectionProcessResponse.bind(this, rowId));
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }
    notifySelectionProcessResponse = (rowId, response) => {
        try {
            if (!response)
                return false;

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const data = JSON.parse(response.Data);

            const row = { ...this.state.rows.find(d => d.id === rowId) };

            const context = {
                row: row
            };

            if (this.props.onAfterNotifySelection)
                this.props.onAfterNotifySelection(context, data, this.props.nexus);
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }

    invertSelection = () => {
        if (this.state.isAllSelected && this.state.selection.length > 0) {
            this.setState({
                selection: []
            }, this.notifyInvertSelection);
        }
        else {
            this.setState(prevState => ({
                selection: [],
                isAllSelected: !prevState.isAllSelected
            }), this.notifyInvertSelection);
        }
    }
    notifyInvertSelection = () => {
        if (!this.props.notifySelection)
            return false;

        try {
            const selection = {
                allSelected: this.state.isAllSelected,
                keys: this.state.selection
            };

            const data = {
                application: this.props.application,
                gridId: this.props.id,
                pageToken: this.props.nexus.getPageToken(),
                selection: selection,
                filters: this.state.filters,
                explicitFilter: this.state.explicitFilter,
                sorts: this.state.sorts,
                parameters: this.props.nexus.getQueryParameters() || []
            };

            const context = {
                abortServerCall: false
            };

            if (this.props.onBeforeNotifyInvertSelection)
                this.props.onBeforeNotifyInvertSelection(context, data, this.props.nexus);

            if (!this.mounted || context.abortServerCall)
                return false;

            this.props.gridNotifyInvertSelection(data).then(this.notifyInvertSelectionProcessResponse);
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }
    notifyInvertSelectionProcessResponse = (response) => {
        try {
            if (!response)
                return false;

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const data = JSON.parse(response.Data);

            const context = {
            };

            if (this.props.onAfterNotifyInvertSelection)
                this.props.onAfterNotifyInvertSelection(context, data, this.props.nexus);
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }

    setSelection = (allSelected, selection) => {
        this.setState({
            isAllSelected: allSelected,
            selection: [...selection]
        });
    }

    openFilterBar = () => {
        this.setState({
            filterStatus: filterStatus.open
        });
    }
    closeFilterBar = () => {
        this.containerRef.current.focus();

        this.setState({
            filterStatus: filterStatus.closed
        });
    }
    toggleFilterBar = () => {
        if (this.state.filterStatus === filterStatus.closed) { //TODO: pasar a enums
            this.openFilterBar();
        }
        else if (this.state.filterStatus === filterStatus.open) {
            this.closeFilterBar();
        }
    }

    openSaveFilterModal = () => {
        this.setState({
            isSaveFilterModalOpen: true
        });
    }
    closeSaveFilterModal = () => {
        this.setState({
            isSaveFilterModalOpen: false
        });
    }

    openLoadFilterModal = () => {
        this.getFilterList();

        this.setState({
            isLoadFilterModalOpen: true
        });
    }
    closeLoadFilterModal = () => {
        this.setState({
            isLoadFilterModalOpen: false
        });
    }

    openGuideFilterModal = () => {
        this.setState({
            isGuideFilterModalOpen: true
        });
    }
    closeGuideFilterModal = () => {
        this.setState({
            isGuideFilterModalOpen: false
        });
    }

    openImportExcelModal = () => {
        var aux1 = this.state.application;
        if (aux1 === 'INT050') {
            this.setState({ isImportExcelModalAPIOpen: true });
        }
        else
            this.setState({ isImportExcelModalOpen: true });
    };

    closeImportExcelModal = () => {
        this.setState({ isImportExcelModalOpen: false });
        this.setState({ isImportExcelModalAPIOpen: false });
    };

    openStatsPanel = () => {
        this.setState({
            isStatsPanelOpen: true,
            isUpdatingStructure: true
        });
    }
    closeStatsPanel = () => {
        this.setState({
            isStatsPanelOpen: false,
            isUpdatingStructure: true
        });
    }
    toggleStatsPanel = () => {
        this.setState(prevState => ({
            isStatsPanelOpen: !prevState.isStatsPanelOpen,
            isUpdatingStructure: true
        }));
    }
    panelResizeEnds = () => {
        this.setState({
            isUpdatingStructure: false
        });
    }

    importExcel = (filename, payloadData, api, empresa, referencia, parametersInitial) => {
        this.setState({
            isCommitInProgress: true
        });

        try {
            const data = {
                gridId: this.props.id,
                application: this.props.application,
                rows: this.state.rows.filter(row => row.isNew || row.isDeleted || row.cells.some(cell => cell.modified || (cell.editable && cell.old !== cell.value))),
                filters: this.state.filters,
                explicitFilter: this.state.explicitFilter,
                sorts: this.state.sorts,
                rowsToFetch: this.props.rowsToFetch,
                fileName: filename,
                payload: payloadData,
                parameters: this.props.nexus.getQueryParameters() || []
            };

            if (parametersInitial) {
                data.parameters = [...data.parameters, ...parametersInitial];
            }

            const context = {
                abortServerCall: false
            };
            showLoadingOverlay(this.props.id);

            if (this.props.onBeforeImportExcel)
                this.props.onBeforeImportExcel(context, data, this.props.nexus, api, empresa, referencia);

            if (!this.mounted || context.abortServerCall) {
                hideLoadingOverlay(this.props.id);
                return false;
            }
            return this.props.gridImportExcel(data).then(this.importExcelProcessResponse);
        }
        catch (ex) {
            hideLoadingOverlay(this.props.id);
            this.props.toaster.toastException(ex);

            this.isCommitInProgress = true;
        }
    };
    importExcelProcessResponse = (response) => {
        try {
            if (!response) {
                hideLoadingOverlay(this.props.id);
                return false;
            }

            if (response.Status === "OK") {
                const data = JSON.parse(response.Data);

                let rows = data.rows;

                const context = {
                    abortUpdate: false
                };

                if (this.props.onAfterImportExcelSuccess)
                    this.props.onAfterImportExcelSuccess(context, rows, data.parameters, this.props.nexus);

                this.props.toaster.toastNotifications(data.notifications);

                if (!this.mounted || context.abortUpdate) {
                    this.isCommitInProgress = false;

                    return false;
                }

                if (rows) {
                    rows = this.appendRowData(rows, true);
                }

                const index = rows.length > 0 ? rows[rows.length - 1].index : 0;

                this.setState({
                    rows: rows,
                    lookup: this.getUpdatedLookup(rows, true),
                    rowCurrentIndex: index,
                    rowDisplayEnd: Math.max(rows.length, this.props.rowsToDisplay || this.props.rowsToFetch),
                }, () => this.isCommitInProgress = false);

                hideLoadingOverlay(this.props.id);
                return response;
            }
            else if (response.Status === "ERROR") {
                this.props.toaster.toastError(response.Message);

                const context = {
                    abortDownload: false
                };

                if (this.props.onAfterImportExcelError)
                    this.props.onAfterImportExcelError(context, this.props.nexus);

                this.isCommitInProgress = false;

                if (!context.abortDownload)
                    window.location = "/api/Grid/DownloadExcel";

                hideLoadingOverlay(this.props.id);
                return response;
            }
        }
        catch (ex) {
            hideLoadingOverlay(this.props.id);
            this.isCommitInProgress = false;

            this.props.toaster.toastException(ex);
        }
    };

    generateExcelTemplate = (e) => {
        try {
            const data = {
                gridId: this.props.id,
                application: this.props.application,
                fileName: null,
                payload: null,
                parameters: this.props.nexus.getQueryParameters() || []
            };

            const context = {
                abortServerCall: false
            };

            if (this.props.onBeforeGenerateExcelTemplate)
                this.props.onBeforeGenerateExcelTemplate(context, data, this.props.nexus, e);

            if (!this.mounted || context.abortServerCall)
                return false;

            return this.props.gridGenerateExcelTemplate(data).then(this.generateExcelTemplateProcessResponse);
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    };
    generateExcelTemplateProcessResponse = (response) => {
        try {
            if (!response)
                return false;

            const data = JSON.parse(response.Data);

            if (response.Status === "ERROR" && data === null)
                throw new ComponentError(response.MessageArguments, response.Message);

            const context = {
                abortDownload: false
            };

            if (this.props.onAfterGenerateExcelTemplate)
                this.props.onAfterGenerateExcelTemplate(context, this.props.nexus);

            if (!context.abortDownload)
                window.location = "/api/Grid/DownloadExcel";

            return response;
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    };

    saveFilter = ({ name, description, isGlobal, isDefault, shouldIncludeSort }) => {
        try {
            const data = {
                application: this.props.application,
                gridId: this.props.id,
                pageToken: this.props.nexus.getPageToken(),
                name: name,
                description: description,
                filters: this.state.filters,
                sorts: shouldIncludeSort ? this.state.sorts : [],
                explicitFilter: this.state.explicitFilter,
                isGlobal: isGlobal,
                isDefault: isDefault
            };

            const context = {
                abortServerCall: false
            };

            if (this.props.onBeforeSaveFilter)
                this.props.onBeforeSaveFilter(context, data, this.props.nexus);

            if (!this.mounted || context.abortServerCall)
                return false;

            return this.props.gridSaveFilter(data).then(this.saveFilterProcessResponse.bind(this, data));
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }
    saveFilterProcessResponse = (filterData, response) => {
        try {
            if (!response)
                return false;

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const context = {
            };

            if (this.props.onAfterSaveFilter)
                this.props.onAfterSavefilter(context, this.props.nexus);

            if (!this.mounted)
                return false;

            this.setState({
                activeFilter: filterData,
                isSaveFilterModalOpen: false
            });
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }

        return response;
    }

    getFilterList = () => {
        try {
            const data = {
                application: this.props.application,
                gridId: this.props.id,
                pageToken: this.props.nexus.getPageToken()
            };

            const context = {
                abortServerCall: false
            };

            if (this.props.onBeforeGetFilterList)
                this.props.onBeforeGetFilterList(context, data, this.props.nexus);

            if (!this.mounted || context.abortServerCall)
                return false;

            return this.props.gridGetFilterList(data).then(this.getFilterListProcessResponse);
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }
    getFilterListProcessResponse = (response) => {
        try {
            if (!response)
                return false;

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const data = JSON.parse(response.Data);

            const context = {
                abortFilterListLoad: false
            };

            if (this.props.onAfterGetFilterList)
                this.props.onAfterGetFilterList(context, data, this.props.nexus);

            if (!this.mounted || context.abortFilterListLoad)
                return false;

            this.setState({
                filterList: data || []
            });
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }

    loadFilter = (filterId, allow) => {
        try {
            if (!allow && this.anyModifiedRows()) {
                this.props.nexus.showConfirmation({
                    message: "General_Sec0_Info_DeseaAnularCambios",
                    acceptLabel: "General_Sec0_btn_DeshacerCambios",
                    onAccept: () => this.loadFilter(filterId, true)
                });

                return false;
            }

            const filter = this.state.filterList.find(d => d.id === filterId);

            this.closeFilterBar();

            this.setState({
                filters: filter.filters,
                sorts: filter.sorts,
                explicitFilter: filter.explicitFilter,
                activeFilter: filter
            }, d => this.applyFilter());
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }
    clearFilter = () => {
        try {
            this.setState({
                filters: [],
                sorts: [],
                activeFilter: null,
                explicitFilter: null
            }, d => this.applyFilter());
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }

    removeFilter = (filterId, allow) => {
        try {
            if (!allow) {
                this.props.nexus.showConfirmation({
                    message: "¿Estas seguro de borrar el filtro?",
                    onAccept: () => this.removeFilter(filterId, true)
                });

                return false;
            }

            const data = {
                application: this.props.application,
                gridId: this.props.id,
                pageToken: this.props.nexus.getPageToken(),
                filterId: filterId
            };

            const context = {
                abortServerCall: false
            };

            if (this.props.onBeforeRemoveFilter)
                this.props.onBeforeRemoveFilter(context, data, this.props.nexus);

            if (!this.mounted || context.abortServerCall)
                return false;

            return this.props.gridRemoveFilter(data).then(this.removeFilterProcessResponse.bind(this, filterId));
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }
    removeFilterProcessResponse = (filterId, response) => {
        try {
            if (!response)
                return false;

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const context = {
            };

            if (this.props.onAfterRemoveFilter)
                this.props.onAfterRemovefilter(context, this.props.nexus);

            this.setState({
                filterList: this.state.filterList.filter(d => d.id !== filterId)
            });

            if (!this.mounted)
                return false;
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }

    openSuperFilterBar = () => {
        if (this.state.superFilterStatus === filterStatus.closed) {
            this.setState({
                superFilterStatus: filterStatus.open
            });
        }
    }
    closeSuperFilterBar = () => {
        this.containerRef.current.focus();

        this.setState({
            superFilterStatus: filterStatus.closed
        });
    }
    applySuperFilter = (value) => {
        this.setState({
            explicitFilter: value
        }, () => this.applyFilter());
    }

    searchSelectValue = async (rowId, columnId, value) => {
        try {
            const data = {
                application: this.props.application,
                pageToken: this.props.nexus.getPageToken(),
                row: this.state.rows.filter(d => d.id === rowId)[0],
                query: {
                    gridId: this.props.id,
                    columnId: columnId,
                    searchValue: value,
                    parameters: this.props.nexus.getQueryParameters() || []
                }
            };

            const context = {
                abortServerCall: false
            };

            if (this.props.onBeforeSelectSearch)
                this.props.onBeforeSelectSearch(context, data.row, data.query, this.props.nexus);

            if (!this.mounted)
                return false;

            if (context.abortServerCall)
                return false;

            const response = await this.props.gridSelectSearch(data).then((res) => res);

            if (response.Status === "ERROR")
                throw new ComponentError(response.MessageArguments, response.Message);

            const result = JSON.parse(response.Data);

            const responseContext = {
                abortRowUpdate: false
            };

            if (this.props.onAfterSelectSearch)
                this.props.onAfterSelectSearch(responseContext, result.options, result.query, this.props.nexus);

            this.props.toaster.toastNotifications(result.notifications);

            if (!this.mounted)
                return true;

            return result;
        }
        catch (ex) {
            this.props.toaster.toastException(ex);
        }
    }

    toggleDisplayModifiedOnly = () => {
        this.setState(prevState => ({
            displayModifiedOnly: !prevState.displayModifiedOnly
        }));
    };
    toggleDisplaySelectionOnly = () => {
        this.setState(prevState => ({
            displaySelectionOnly: !prevState.displaySelectionOnly
        }));
    };

    openDropdown = (columnId, rowId, left, top, spaceAvailableRight, buttonWidth) => {
        const column = this.state.columns.find(d => d.id === columnId);

        if (!column)
            return;

        const offsetTopBody = this.bodyRef.current.getBoundingClientRect().top;
        const offsetLeftBody = this.bodyRef.current.getBoundingClientRect().left;

        const disabledButtons = this.state.rows.find(d => d.id === rowId).disabledButtons;

        this.setState({
            dropdownShow: true,
            dropdownColumnId: columnId,
            dropdownRowId: rowId,
            dropdownItems: column.items.filter(i => disabledButtons.indexOf(i.id) === -1),
            dropdownLeft: left - offsetLeftBody,
            dropdownTop: top - offsetTopBody,
            dropdownButtonWidth: buttonWidth,
            dropdownSpaceAvailableRight: spaceAvailableRight
        });
    }
    closeDropdown = () => {
        if (!this.mounted)
            return false;

        this.setState({
            dropdownShow: false
        });
    }

    updateScrollPosition = (rawOffset) => {
        window.requestAnimationFrame(() => {
            this.props.scrollContext.onScrollBegin();

            window.requestAnimationFrame(() => {
                if (rawOffset !== this.lastOffset) {
                    this.lastOffset = rawOffset;

                    const offset = Math.floor(rawOffset / this.state.rowHeight);

                    if (!this.shouldUpdateVisibleArea) {
                        if (offset - this.props.rowOverscan < this.state.rowDisplayStart || offset + this.props.rowsToDisplay + this.props.rowOverscan > this.state.rowDisplayEnd) {
                            this.shouldUpdateVisibleArea = true;
                            this.offsetToUpdate = offset;
                        }
                    }
                }
            });
        });
    }
    updateVisibleArea = () => {
        if (!this.state.isFetching && this.offsetToUpdate + this.props.rowsToDisplay + this.props.rowFetchOverscan > this.state.rows.length) {
            this.fetchData().then(() => {
                this.shouldUpdateVisibleArea = false; //Ver si hacer promesa
            });
        }
        else {
            this.setState({
                rowDisplayStart: this.offsetToUpdate - this.props.rowOverscan < 0 ? 0 : this.offsetToUpdate - this.props.rowOverscan,
                rowDisplayEnd: Math.min(this.offsetToUpdate + this.props.rowsToDisplay + this.props.rowOverscan, this.state.rows.length)
            }, () => this.shouldUpdateVisibleArea = false);
        }
    }
    watchScroll = () => {
        this.scrollWatcher = setInterval(() => {
            if (this.shouldUpdateVisibleArea) {
                this.updateVisibleArea();
            }
        }, 50);
    }
    unWatchScroll = () => {
        clearInterval(this.scrollWatcher);
    }

    getRows = () => {
        if (this.state.displaySelectionOnly || this.state.displayModifiedOnly) {
            return this.state.rows.filter(d => {
                if (this.state.displaySelectionOnly)
                    return this.state.selection.indexOf(d.id) > -1;

                if (this.state.displayModifiedOnly)
                    return d.isNew || d.isDeleted || d.isModified;

                return true;
            });
        }

        return this.state.rows;
    }
    getColumns = () => {
        return this.state.columns.filter(col => !col.hidden);
    }
    getMaxHeight = () => {
        return this.props.rowsToDisplay * this.state.rowHeight;
    }
    getEstimatedHeight = () => {
        return this.getMaxHeight() + this.toolbarHeight + this.props.scrollContext.scrollbarHeight + this.headerHeight;
    }

    setEditingCell = (rowId, colId, callback) => {
        this.setState({
            editingRow: rowId,
            editingColumn: colId
        }, callback);
    };
    clearEditingCell = (callback) => {
        this.setState({
            editingRow: null,
            editingColumn: null
        }, callback);
    };
    moveToPreviousEditableCell = (rows, row, column, callback) => {
        let rowIndex = rows.findIndex(d => d.id === row);

        if (rowIndex > -1) {
            let colIndex = rows[rowIndex].cells.findIndex(d => d.column === column);

            if (colIndex > -1) {
                let previousColumn = null;

                colIndex--;

                while (colIndex > -1) {
                    if (rows[rowIndex].cells[colIndex].editable) {
                        previousColumn = rows[rowIndex].cells[colIndex].column;
                        break;
                    }

                    colIndex--;
                }

                if (previousColumn) {
                    this.setState({
                        editingRow: row,
                        editingColumn: previousColumn
                    }, callback);
                }
            }
        }
    }
    moveToNextEditableCell = (rows, row, column, callback) => {
        let rowIndex = rows.findIndex(d => d.id === row);

        if (rowIndex > -1) {
            let colIndex = rows[rowIndex].cells.findIndex(d => d.column === column);

            if (colIndex > -1) {
                let nextColumn = null;

                colIndex++;

                while (colIndex < rows[rowIndex].cells.length - 1) {
                    if (rows[rowIndex].cells[colIndex].editable) {
                        nextColumn = rows[rowIndex].cells[colIndex].column;
                        break;
                    }

                    colIndex++;
                }

                if (nextColumn) {
                    this.setState({
                        editingRow: row,
                        editingColumn: nextColumn
                    }, callback);
                }
            }
        }
    }
    moveToFirstEditableCell = (rows, callback) => {
        let rowIndex = rows.findIndex(d => d.id === this.state.editingRow);

        if (rowIndex > -1) {
            const editableCells = rows[rowIndex].cells.filter(d => d.editable);

            if (editableCells) {
                const firstCell = editableCells[0];

                if (firstCell) {
                    this.setState({
                        editingColumn: firstCell.column
                    }, callback);
                }
            }
        }
    };
    moveToLastEditableCell = (rows, callback) => {
        let rowIndex = rows.findIndex(d => d.id === this.state.editingRow);

        if (rowIndex > -1) {
            const editableCells = rows[rowIndex].cells.filter(d => d.editable);

            if (editableCells) {
                const lastCell = editableCells[editableCells.length - 1];

                if (lastCell) {
                    this.setState({
                        editingColumn: lastCell.column
                    }, callback);
                }
            }
        }
    };
    moveToNewRowEditableCell = (callback) => {
        this.addRow(() => {
            const newRows = this.getRows().filter(d => d.isNew);

            if (newRows) {
                const lastNewRow = newRows[newRows.length - 1];

                const editableCells = lastNewRow.cells.filter(d => d.editable);

                if (editableCells) {
                    this.setState({
                        editingRow: lastNewRow.id,
                        editingColumn: editableCells[0].column
                    }, callback);
                }
            }
        }, true);
    };

    getTotalHeight = (rows) => {
        return rows.length * this.state.rowHeight;
    }

    hasError = () => {
        return this.state.rows.filter(row => row.cells.some(cell => cell.value !== cell.old)).some(row => row.cells.some(cell => cell.error));
    }
    hasChanges = () => {
        return this.state.rows.some(row => row.cells.some(cell => cell.value !== cell.old));
    }

    fixSelectGridHeight = (top, height) => {
        const bottomDistance = window.pageYOffset + this.props.nexus.getBottomFillerViewportPosition();

        if (this.bodyRef.current) {
            if (height) {
                const difference = this.bodyRef.current.getBoundingClientRect().bottom - top;

                const fixedHeight = height - difference;

                if ((window.pageYOffset + this.bodyRef.current.getBoundingClientRect().bottom + fixedHeight) > bottomDistance) {
                    this.props.nexus.setBottomFillerHeight(fixedHeight);
                }
            }
            else {
                this.props.nexus.setBottomFillerHeight(0);
            }
        }
    }

    getMeasures = () => {
        return this.state.columns.filter(d => !d.hidden).reduce((measures, col) => {
            if (col.fixed === columnFixed.left) {
                measures.widthLeft = measures.widthLeft + col.width;
            }
            else if (col.fixed === columnFixed.right) {
                measures.widthRight = measures.widthRight + col.width;
            }
            else {
                measures.widthCenter = measures.widthCenter + col.width;
            }

            return measures;
        }, {
            widthLeft: 0,
            widthRight: 0,
            widthCenter: 0
        });
    }

    isVScrollActive = () => {
        return this.getMaxHeight() < this.getTotalHeight(this.getRows());
    }

    getApi() {
        return {
            addRow: this.addRow,
            updateRows: this.updateRows,
            updateRow: this.updateRow,
            validateRow: this.validateRow,
            getModifiedRows: this.getModifiedRows,
            getAllRows: this.getAllRows,
            commit: this.commit,
            refresh: this.forceRefresh,
            reset: this.reset,
            rollback: this.rollback,
            hasError: this.hasError,
            triggerMenuAction: this.performMenuItemAction,
            hasChanges: this.hasChanges,
            setSelection: this.setSelection,
            clickMenuItem: this.performMenuItemAction,
            clickButtonAction: this.performButtonAction,
            clearRows: () => {
                this.state.selection = []
            }
        };
    }

    render() {
        const rows = this.getRows();
        const columns = this.getColumns();
        const measures = this.getMeasures();
        const totalHeight = this.getTotalHeight(rows);

        const gridClientLeft = this.bodyRef.current ? this.bodyRef.current.getBoundingClientRect().left : 0;

        return (
            <ScrollSync onScrollBegin={this.props.scrollContext.onScrollBegin}>
                <GridContainer
                    id={this.props.id}
                    isInitializing={this.state.isInitializing}
                    height={this.getEstimatedHeight()}
                    ref={this.bodyRef}
                >
                    <Toolbar
                        columns={this.state.columns}
                        menuItems={this.state.menuItems}
                        refresh={this.refresh}
                        commit={this.commit}
                        rollback={this.rollback}
                        deleteRow={this.deleteRow}
                        addRow={this.addRow}
                        enableExcelExport={this.props.enableExcelExport}
                        enableExcelImport={this.props.enableExcelImport}
                        application={this.props.application}
                        performMenuItemAction={this.performMenuItemAction}
                        exportExcel={this.exportExcel}
                        showColumn={this.showColumn}
                        hideColumn={this.hideColumn}
                        toggleFilterBar={this.toggleFilterBar}
                        applyFilter={this.applyFilter}

                        moveToNewRowEditableCell={this.moveToNewRowEditableCell}

                        openSaveFilterModal={this.openSaveFilterModal}
                        openLoadFilterModal={this.openLoadFilterModal}
                        openGuideFilterModal={this.openGuideFilterModal}
                        openImportExcelModal={this.openImportExcelModal}

                        openStatsPanel={this.openStatsPanel}
                        toggleStatsPanel={this.toggleStatsPanel}

                        activeFilter={this.state.activeFilter}
                        enableSelection={this.props.enableSelection}

                        displayModifiedOnly={this.state.displayModifiedOnly}
                        displaySelectionOnly={this.state.displaySelectionOnly}

                        toggleDisplayModifiedOnly={this.toggleDisplayModifiedOnly}
                        toggleDisplaySelectionOnly={this.toggleDisplaySelectionOnly}

                        isEditingEnabled={this.state.isEditingEnabled}
                        isCommitEnabled={this.state.isCommitEnabled}
                        isRollbackEnabled={this.state.isRollbackEnabled}
                        isAddEnabled={this.state.isAddEnabled}
                        isRemoveEnabled={this.state.isRemoveEnabled}
                        isCommitButtonUnavailable={this.state.isCommitButtonUnavailable}
                    />
                    <GridPanelContainer
                        isStatsPanelOpen={this.state.isStatsPanelOpen}
                        panelResizeEnds={this.panelResizeEnds}
                    >
                        <GridContentContainer
                            ref={this.containerRef}
                            isResizing={this.state.isResizing}
                            isUpdatingStructure={this.state.isUpdatingStructure}
                            columnResizeEnd={this.columnResizeEnd}
                            columnResizeChange={this.columnResizeChange}
                            toggleFilterBar={this.toggleFilterBar}
                            openSuperFilterBar={this.openSuperFilterBar}
                            closeFilterBar={this.closeFilterBar}
                            rows={rows}
                            moveToNewRowEditableCell={this.moveToNewRowEditableCell}
                            deleteRow={this.deleteRow}
                        >
                            <HeaderPane
                                gridId={this.props.id}
                                columns={columns}
                                rows={rows}
                                selection={this.state.selection}
                                sorts={this.state.sorts}
                                filters={this.state.filters}
                                filterStatus={this.state.filterStatus}
                                widthLeft={measures.widthLeft}
                                widthRight={measures.widthRight}
                                highlightLast={this.state.highlightLast}
                                hasFixedColumnsLeft={this.hasFixedColumnsLeft}
                                hasFixedColumnsRight={this.hasFixedColumnsRight}
                                enableSelection={this.props.enableSelection}
                                applySort={this.applySort}
                                applySortAscending={this.applySortAscending}
                                applySortDescending={this.applySortDescending}
                                applySortReset={this.applySortReset}
                                columnResizeBegin={this.columnResizeBegin}
                                applyFilter={this.applyFilter}
                                updateFilter={this.updateFilter}
                                updateColumnOrder={this.updateColumnOrder}
                                invertSelection={this.invertSelection}
                                fixColumn={this.fixColumn}
                                hideColumn={this.hideColumn}
                                totalHeight={totalHeight}
                                isSelectionInverted={this.state.isAllSelected}
                                isResizing={this.state.isResizing}
                                isVScrollActive={this.isVScrollActive}
                                isEditingEnabled={this.state.isEditingEnabled}
                            />
                            <BodyPane
                                columns={columns}
                                rows={rows}
                                selection={this.state.selection}
                                highlight={this.state.highlight}
                                enableSelection={this.props.enableSelection}
                                hasFixedColumnsLeft={this.hasFixedColumnsLeft}
                                hasFixedColumnsRight={this.hasFixedColumnsRight}
                                widthLeft={measures.widthLeft}
                                widthRight={measures.widthRight}

                                addRow={this.addRow}
                                autofocus={this.props.autofocus}

                                displaySelectionOnly={this.state.displaySelectionOnly}
                                displayModifiedOnly={this.state.displayModifiedOnly}

                                getRowHighlights={this.getRowHighlights}
                                toggleHighlight={this.toggleHighlight}
                                clearHighlighted={this.clearHighlighted}
                                isHighlighted={this.isHighlighted}

                                openRelatedLink={this.openRelatedLink}

                                fixSelectGridHeight={this.fixSelectGridHeight}

                                gridClientLeft={gridClientLeft}

                                rowHeight={this.state.rowHeight}
                                rowDisplayStart={this.state.rowDisplayStart}
                                rowDisplayEnd={this.state.rowDisplayEnd}
                                rowTotalRows={rows.length}
                                maxHeight={this.getMaxHeight()}
                                performButtonAction={this.performButtonAction}
                                updateScrollPosition={this.updateScrollPosition}
                                updateCellValue={this.updateCellValue}
                                updateSelection={this.updateSelection}
                                searchSelectValue={this.searchSelectValue}
                                openDropdown={this.openDropdown}
                                isInitializing={this.state.isInitializing}
                                isSelectionInverted={this.state.isAllSelected}
                                isFetching={this.state.isFetching}
                                isResizing={this.state.isResizing}
                                isEditingEnabled={this.state.isEditingEnabled}

                                editingRow={this.state.editingRow}
                                editingColumn={this.state.editingColumn}
                                setEditingCell={this.setEditingCell}
                                clearEditingCell={this.clearEditingCell}
                                moveToPreviousEditableCell={this.moveToPreviousEditableCell}
                                moveToNextEditableCell={this.moveToNextEditableCell}
                                moveToFirstEditableCell={this.moveToFirstEditableCell}
                                moveToLastEditableCell={this.moveToLastEditableCell}
                                moveToNewRowEditableCell={this.moveToNewRowEditableCell}

                                translator={this.props.t}
                            />
                            <GridScrollPane
                                ref={this.scrollPaneRef}
                                widthLeft={measures.widthLeft}
                                widthRight={measures.widthRight}
                                widthCenter={measures.widthCenter}
                                isVScrollActive={this.isVScrollActive}
                            />
                            <ColumnResizeMarker
                                left={this.getResizeMarkerPosition()}
                                height={this.getResizeMarkerHeight(this.props.scrollContext.scrollbarHeight)}
                                isResizing={this.state.isResizing}
                            />
                        </GridContentContainer>
                        <GridStatsPanel
                            isStatsPanelOpen={this.state.isStatsPanelOpen}
                            closeStatsPanel={this.closeStatsPanel}
                            fetchStats={this.fetchStats}
                            rowCount={this.state.rows.length}
                            visibleRows={rows.length}
                        />
                    </GridPanelContainer>
                    <GridSuperFilterModal
                        columns={this.state.columns}
                        superFilterStatus={this.state.superFilterStatus}
                        closeSuperFilterBar={this.closeSuperFilterBar}
                        applySuperFilter={this.applySuperFilter}
                        explicitFilter={this.state.explicitFilter}
                    />
                    <GridSaveFilterModal
                        isSaveFilterModalOpen={this.state.isSaveFilterModalOpen}
                        closeSaveFilterModal={this.closeSaveFilterModal}
                        saveFilter={this.saveFilter}
                    />
                    <GridLoadFilterModal
                        isLoadFilterModalOpen={this.state.isLoadFilterModalOpen}
                        closeLoadFilterModal={this.closeLoadFilterModal}
                        filterList={this.state.filterList}
                        loadFilter={this.loadFilter}
                        clearFilter={this.clearFilter}
                        removeFilter={this.removeFilter}
                    />
                    <GridGuideFilterModal
                        isGuideFilterModalOpen={this.state.isGuideFilterModalOpen}
                        closeGuideFilterModal={this.closeGuideFilterModal}
                    />
                    <GridImportExcelModal
                        isImportExcelModalOpen={this.state.isImportExcelModalOpen}
                        closeImportExcelModal={this.closeImportExcelModal}
                        importExcel={this.importExcel}
                        generateExcelTemplate={this.generateExcelTemplate}
                        importExcelCustom={this.props.importExcelCustom}
                    />
                    <GridImportExcelModalAPI
                        isImportExcelModalAPIOpen={this.state.isImportExcelModalAPIOpen}
                        closeImportExcelModal={this.closeImportExcelModal}
                        importExcel={this.importExcel}
                        generateExcelTemplate={this.generateExcelTemplate}
                    />
                    <LoadingSkeleton
                        height={this.getEstimatedHeight()}
                        isInitializing={this.state.isInitializing}
                    />
                    <GridDropdown
                        columnId={this.state.dropdownColumnId}
                        rowId={this.state.dropdownRowId}
                        show={this.state.dropdownShow}
                        items={this.state.dropdownItems}
                        left={this.state.dropdownLeft}
                        top={this.state.dropdownTop}
                        spaceAvailableRight={this.state.dropdownSpaceAvailableRight}
                        buttonWidth={this.state.dropdownButtonWidth}
                        closeDropdown={this.closeDropdown}
                        performButtonAction={this.performButtonAction}
                    />
                </GridContainer>
            </ScrollSync>
        );
    }
}

export const Grid = withTranslation()(withCustomTranslation(withToaster(withPageContext(withGridDataProvider(withScrollContext(InternalGrid))))));