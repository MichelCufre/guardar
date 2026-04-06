import React from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';

export default function PRD130(props) {
    const { t } = useTranslation();

    return (
        <Page
            icon="fas fa-file"
            title={t("KIT130_sec0_lbl_PageTitle")}
            {...props}
        >
            <Grid
                id="KIT130_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                enableExcelImport={false}
            />
        </Page>
    );
}