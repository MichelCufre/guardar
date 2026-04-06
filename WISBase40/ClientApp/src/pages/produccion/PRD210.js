import React, { useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';

export default function PRD210(props) {
    const { t } = useTranslation();
    const ingresoRef = useRef(null);

    const onAfterLoad = (data) => {
        ingresoRef.current = data.parameters.find(d => d.id === "NU_PRDC_INGRESO").value;
    };

    const addParameters = (context, data, nexus) => {
        data.parameters = [
            {
                id: "NU_PRDC_INGRESO",
                value: ingresoRef.current
            }
        ];
    };

    return (
        <Page
            load
            icon="fas fa-list"
            title={t("PRD210_Sec0_pageTitle_Titulo")}
            onAfterLoad={onAfterLoad}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="PRD210_grid_1"
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
                        enableExcelExport
                        enableExcelImport={false}
                    />
                </div>
            </div>
        </Page>
    );
}