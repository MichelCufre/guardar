import React from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';

export default function PRE130(props) {

    const { t } = useTranslation();

    return (

        <Page
            title={t("PRE130_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="PRE130_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport />
                </div>
            </div>
        </Page>
    );
}