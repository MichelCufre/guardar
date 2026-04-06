import React from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';

export default function STO110(props) {
    const { t } = useTranslation();

    return (
        <Page
            icon="fas fa-cubes"
            title={t("STO110_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col">
                    <Grid
                        id="STO110_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                    />
                </div>
            </div>
        </Page>
    );
}