import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Page } from '../../components/Page';

import { Grid } from '../../components/GridComponents/Grid';

export default function STO211(props) {
    const { t } = useTranslation();

    

    

    return (

        <Page
            title={t("STO211_Sec0_pageTitle_Titulo")}
            {...props}
        >

            

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="STO211_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport 
                        
                    />
                </div>
            </div>

        </Page>
    );
}
