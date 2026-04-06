import React, { Component } from 'react';
import { useTranslation } from 'react-i18next';
import $ from 'jquery';

export default function withGridDataProvider(WrappedComponent) {
    return function WithGridDataProvider(props) {
        const { i18n } = useTranslation();

        const initialize = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                gridId: data.gridId,
                rowsToFetch: data.rowsToFetch,
                parameters: data.parameters
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.gridId,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Grid/Initialize", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);
        };
        const fetchRows = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                gridId: data.gridId,
                filters: data.filters,
                explicitFilter: data.explicitFilter,
                sorts: data.sorts,
                rowsToSkip: data.rowsToSkip,
                rowsToFetch: data.rowsToFetch,
                parameters: data.parameters
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.gridId,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Grid/FetchRows", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);
        };
        const fetchStats = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                gridId: data.gridId,
                filters: data.filters,
                explicitFilter: data.explicitFilter,
                sorts: data.sorts,
                parameters: data.parameters
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.gridId,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Grid/FetchStats", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);
        };
        const validateRow = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                gridId: data.gridId,
                row: data.row,
                parameters: data.parameters
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.gridId,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Grid/ValidateRow", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);
        };
        const commit = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                gridId: data.gridId,
                rows: data.rows,
                query: {
                    filters: data.filters,
                    explicitFilter: data.explicitFilter,
                    sorts: data.sorts,
                    rowsToFetch: data.rowsToFetch,
                    rowsToSkip: 0,
                    parameters: data.parameters
                }
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.gridId,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Grid/Commit", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);
        };
        const buttonAction = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                gridId: data.gridId,
                buttonId: data.buttonId,
                row: data.row,
                column: data.columnId,
                parameters: data.parameters
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.gridId,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Grid/ButtonAction", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);
        };
        const menuItemAction = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                gridId: data.gridId,
                buttonId: data.buttonId,
                filters: data.filters,
                explicitFilter: data.explicitFilter,
                selection: data.selection,
                parameters: data.parameters
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.gridId,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Grid/MenuItemAction", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);
        };

        const showLoadingOverlay = (id = "layout") => {
            $("." + id).LoadingOverlay("show", {
                image: "",
                fontawesome: "fa fa-cog fa-spin",
                background: "rgba(22, 25, 28, 0.2)"
            });
        }

        const hideLoadingOverlay = (id = "layout") => {
            $("." + id).LoadingOverlay("hide");
        }
        const updateConfig = (data) => {
            showLoadingOverlay();
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                gridId: data.gridId,
                columns: data.columns,
                parameters: data.parameters
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.gridId,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Grid/UpdateConfig", request)
                .then(response => {
                    hideLoadingOverlay();
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => {
                    hideLoadingOverlay();
                    return response ? response.json() : response
                });
        };
        const exportExcel = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                fileName: data.fileName,
                gridId: data.gridId,
                filters: data.filters,
                explicitFilter: data.explicitFilter,
                sorts: data.sorts,
                type: data.type,
                parameters: data.parameters,
                userLanguage: i18n.languages[0]
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.gridId,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Grid/ExportExcel", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);
        };
        const importExcel = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                fileName: data.fileName,
                gridId: data.gridId,
                payload: data.payload,
                query: {
                    filters: data.filters,
                    explicitFilter: data.explicitFilter,
                    sorts: data.sorts,
                    rowsToFetch: data.rowsToFetch,
                    rowsToSkip: 0,
                    parameters: data.parameters
                }
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.gridId,
                    application: application,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Grid/ImportExcel", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);
        };
        const generateExcelTemplate = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                fileName: application,
                gridId: data.gridId,
                payload: data.payload,
                query: {
                    parameters: data.parameters
                }
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.gridId,
                    application: application,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Grid/GenerateExcelTemplate", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);
        };
        const selectSearch = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                row: data.row,
                query: data.query
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.query.gridId,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Grid/SelectSearch", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);
        };
        const saveFilter = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                gridId: data.gridId,
                name: data.name,
                description: data.description,
                filters: data.filters,
                sorts: data.sorts,
                explicitFilter: data.explicitFilter,
                isGlobal: data.isGlobal,
                isDefault: data.isDefault
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.gridId,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Grid/SaveFilter", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);
        };
        const removeFilter = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                gridId: data.gridId,
                filterId: data.filterId
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.gridId,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Grid/RemoveFilter", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);
        };
        const getFilterList = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                gridId: data.gridId
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.gridId,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Grid/GetFilterList", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);
        };
        const gridNotifySelection = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                gridId: data.gridId,
                rowId: data.rowId,
                filters: data.filters,
                selection: data.selection,
                parameters: data.parameters
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.gridId,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Grid/NotifySelection", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);
        }
        const gridNotifyInvertSelection = (data) => {
            const path = window.location.pathname;

            const application = data.application ? data.application : path.substring(path.lastIndexOf('/') + 1);

            const requestData = {
                gridId: data.gridId,
                selection: data.selection,
                filters: data.filters,
                isAllSelected: data.isAllSelected,
                parameters: data.parameters
            };

            const request = {
                method: "POST",
                cache: "no-cache",
                headers: {
                    "Content-Type": "application/json",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: JSON.stringify({
                    componentId: data.gridId,
                    application: application,
                    pageToken: data.pageToken,
                    data: JSON.stringify(requestData)
                })
            };

            return fetch("api/Grid/NotifyInvertSelection", request)
                .then(response => {
                    if (response.status === 401) {
                        window.location = "/api/Security/Logout";

                        return null;
                    }

                    return response;
                })
                .then((response) => response ? response.json() : response);
        }

        return (
            <WrappedComponent
                gridInitialize={initialize}
                gridFetchRows={fetchRows}
                gridFetchStats={fetchStats}
                gridValidateRow={validateRow}
                gridCommit={commit}
                gridButtonAction={buttonAction}
                gridMenuItemAction={menuItemAction}
                gridUpdateConfig={updateConfig}
                gridExportExcel={exportExcel}
                gridImportExcel={importExcel}
                gridGenerateExcelTemplate={generateExcelTemplate}
                gridSelectSearch={selectSearch}
                gridSaveFilter={saveFilter}
                gridGetFilterList={getFilterList}
                gridRemoveFilter={removeFilter}
                gridNotifySelection={gridNotifySelection}
                gridNotifyInvertSelection={gridNotifyInvertSelection}
                {...props}
            />
        );
    };
}