import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Page } from '../../components/Page';

import { Grid } from '../../components/GridComponents/Grid';

export default function POR040(props) {
    const { t } = useTranslation();

    return (

        <Page
            title={t("POR040_Sec0_pageTitle_Titulo")}
            {...props}
        >

            

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="POR040_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport />
                </div>
            </div>

        </Page>
    );
}
