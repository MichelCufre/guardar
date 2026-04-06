import React from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';

export default function PRD191(props) {
    const { t } = useTranslation();

    return (
        <Page
            icon="fas fa-file"
            title={t("PRD191_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Grid
                id="PRD191_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                enableExcelImport={false}
            />
        </Page>
    );
}