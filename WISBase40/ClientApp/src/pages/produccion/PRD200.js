import React, { useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import '../../css/RowColors.css';

export default function PRD200(props) {
    const { t } = useTranslation();
    const ingreso = useRef("");

    const onAfterPageLoad = (data) => {
        let numeroIngreso = data.parameters.find(p => p.id === "Ingreso");

        if (numeroIngreso) {
            ingreso.current = numeroIngreso;
        }
    };
    const onBeforeInitialize = (context, data, nexus) => {
        if (ingreso.current) {
            let parameters = [
                {
                    id: "PRD_INGRESO",
                    value: ingreso.current.value
                }
            ];

            data.parameters = parameters;
        }
    }
    const onBeforeCommit = (context, data, nexus) => {
        if (ingreso.current) {
            let parameters = [
                {
                    id: "PRD_INGRESO",
                    value: ingreso.current.value
                }
            ];

            data.parameters = parameters;
        }
    }

    const onBeforeFetch = (context, data, nexus) => {
        if (ingreso.current) {
            let parameters = [
                {
                    id: "PRD_INGRESO",
                    value: ingreso.current.value
                }
            ];

            data.parameters = parameters;
        }
    }
    const onBeforeApplyFilter = (context, data, nexus) => {
        if (ingreso.current) {
            let parameters = [
                {
                    id: "PRD_INGRESO",
                    value: ingreso.current.value
                }
            ];

            data.parameters = parameters;
        }
    }
    const onBeforeExportExcel = (context, data, nexus) => {
        if (ingreso.current) {
            let parameters = [
                {
                    id: "PRD_INGRESO",
                    value: ingreso.current.value
                }
            ];

            data.parameters = parameters;
        }
    }
    const onBeforeApplySort = (context, data, nexus) => {
        if (ingreso.current) {
            let parameters = [
                {
                    id: "PRD_INGRESO",
                    value: ingreso.current.value
                }
            ];

            data.parameters = parameters;
        }
    };
    const onBeforeValidateRow = (context, data, nexus) => {
        if (ingreso.current) {
            let parameters = [
                {
                    id: "PRD_INGRESO",
                    value: ingreso.current.value
                }
            ];

            data.parameters = parameters;
        }
    }

    return (

        <Page
            load
            icon="fas fa-list"
            title={t("PRD200_Sec0_pageTitle_Titulo")}
            onAfterLoad={onAfterPageLoad}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="PRD200_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeInitialize={onBeforeInitialize}
                        onBeforeCommit={onBeforeCommit}
                        onBeforeFetch={onBeforeFetch}
                        onBeforeExportExcel={onBeforeExportExcel}
                        onBeforeApplyFilter={onBeforeApplyFilter}
                        onBeforeApplySort={onBeforeApplySort}
                        onBeforeValidateRow={onBeforeValidateRow}
                        enableExcelImport={false}
                    />
                </div>
            </div>
        </Page>
    );
}