import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';

export default function REG701(props) {

    const { t } = useTranslation();

    const gridOnAfterButtonAction = (data, nexus) => {
        if (data.buttonId === "btnPredeterminado") {
            nexus.getGrid("REG701_grid_1").refresh();
        };
    }

    return (

        <Page
            title={t("REG701_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Grid
                application="REG701"
                id="REG701_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onAfterButtonAction={gridOnAfterButtonAction}
            />
        </Page>
    );
}
