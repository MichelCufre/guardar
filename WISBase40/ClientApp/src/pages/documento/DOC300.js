import React from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';

export default function DOC300(props) {
    const { t } = useTranslation();

    return (

        <Page
            icon="fas fa-piggy-bank"
            title={t("DOC300_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="DOC300_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport />
                </div>
            </div>
        </Page>
    );
}