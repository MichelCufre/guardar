import React from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';

export default function STO151(props) {
    const { t } = useTranslation();

    return (
        <Page
            icon="fas fa-cubes"
            title={t("STO151_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col">
                    <h2>{t("STO151_grid1_title_PICKING")}</h2>
                    <hr />
                    <Grid
                        id="STO151_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                    />
                </div>
            </div>
            <div className="row mb-4">
                <div className="col">
                    <h2>{t("STO151_grid2_title_CONT")}</h2>
                    <hr />
                    <Grid
                        id="STO151_grid_2"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                    />
                </div>
            </div>
            <div className="row mb-4">
                <div className="col">
                    <h2>{t("STO151_grid3_title_TRANSFERENCIA")}</h2>
                    <hr />
                    <Grid
                        id="STO151_grid_3"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                    />
                </div>
            </div>
            <div className="row mb-4">
                <div className="col">
                    <h2>{t("STO151_grid4_title_RECEPCION")}</h2>
                    <hr />
                    <Grid
                        id="STO151_grid_4"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                    />
                </div>
            </div>
            <div className="row mb-4">
                <div className="col">
                    <h2>{t("STO151_grid5_title_LPN")}</h2>
                    <hr />
                    <Grid
                        id="STO151_grid_5"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                    />
                </div>
            </div>
        </Page>
    );
}