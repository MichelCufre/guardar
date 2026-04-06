import React, { useState,useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';

export default function PRD150(props) {
    const { t } = useTranslation();
    return (

        <Page
            icon="fas fa-file"
            title={t("KIT150_sec0_lbl_PageTitle")}
            {...props}
        >
            <Grid id="PRD150_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport />

        </Page>
    );
}