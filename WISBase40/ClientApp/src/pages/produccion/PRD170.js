import React from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';


export default function PRD170(props) {
    const { t } = useTranslation();

    const handleGridBeforeButtonAction = (context, data, nexus) => {
        console.log(data);

        if (data.buttonId === "btnProduccion") {
            context.abortServerCall = true;

            const paramIngreso = data.row.cells.find(d => d.column === "NU_PRDC_INGRESO");

            localStorage.setItem("PRD171_nroIngreso", paramIngreso.value);

            nexus.redirect("/produccion/PRD171");
        }
    }

    return (
        <Page
            icon="fas fa-file"
            title={t("PRD170_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="PRD170_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onBeforeButtonAction={handleGridBeforeButtonAction}
                        enableExcelExport
                    />
                </div>
            </div>
        </Page>
    );
}