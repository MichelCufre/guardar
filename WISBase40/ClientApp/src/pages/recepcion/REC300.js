import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';


export default function REC300(props) {

    const { t } = useTranslation();

    return (

        <Page
            title={t("WREC300_Sec0_pageTitle_Titulo")}
            application="REC300"
            {...props}
        >
            <Grid
                id="REC300_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                application="REC300"
            />
        </Page>
    );
}
