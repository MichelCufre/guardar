import React, { useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';

export default function PRD220(props) {
    const { t } = useTranslation();
    const ingresoRef = useRef(null);

    const onAfterLoad = (data) => {
        ingresoRef.current = {
            ingreso: data.parameters.find(d => d.id === "NU_PRDC_INGRESO").value,
            empresa: data.parameters.find(d => d.id === "CD_EMPRESA").value
        };
    };

    const addParameters = (context, data, nexus) => {
        if (ingresoRef.current) {
            data.parameters = [
                {
                    id: "NU_PRDC_INGRESO",
                    value: ingresoRef.current.ingreso
                },
                {
                    id: "CD_EMPRESA",
                    value: ingresoRef.current.empresa
                }
            ];
        }
    };

    const onBeforeSelectSearch = (context, row, query, nexus) => {
        if (ingresoRef.current) {
            query.parameters = [
                {
                    id: "NU_PRDC_INGRESO",
                    value: ingresoRef.current.ingreso
                },
                {
                    id: "CD_EMPRESA",
                    value: ingresoRef.current.empresa
                }
            ];
        }
    }

    return (
        <Page
            load
            icon="fas fa-list"
            title={t("PRD220_Sec0_pageTitle_Titulo")}
            onAfterLoad={onAfterLoad}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="PRD220_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onBeforeInitialize={addParameters}
                        onBeforeFetch={addParameters}
                        onBeforeValidateRow={addParameters}
                        onBeforeCommit={addParameters}
                        onBeforeExportExcel={addParameters}
                        onBeforeApplyFilter={addParameters}
                        onBeforeApplySort={addParameters}
                        onBeforeValidateRow={addParameters}
                        onBeforeSelectSearch={onBeforeSelectSearch}
                        enableExcelExport
                        enableExcelImport={false}
                    />
                </div>
            </div>
        </Page>
    );
}