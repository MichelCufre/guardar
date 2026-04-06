import React from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';

export default function DOC362(props) {
    const { t } = useTranslation();
    return (

        <Page
            icon="fas fa-file"
            title={t("DOC362_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-6">
                    <Grid id="DOC362_grid_ajustes"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport />
                </div>

                <div className="col-6">
                    <Grid id="DOC362_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport />
                </div>
            </div>
        </Page>
    );
}