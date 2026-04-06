import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';


export default function PAR050(props) {

    const { t } = useTranslation();

    return (
        <Page
            title={t("PAR050_Sec0_pageTitle_Titulo")}            
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="PAR050_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                    />
                </div>
            </div>

        </Page>
    );
}