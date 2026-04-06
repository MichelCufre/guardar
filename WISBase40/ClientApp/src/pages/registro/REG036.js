import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';

export default function REG036(props) {

    const { t } = useTranslation();

    return (
        <Page
            title={t("REG036_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Grid
                id="REG036_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
            />
        </Page>
    );
}