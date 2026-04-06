
import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';


export default function REG060(props) {

    const { t } = useTranslation();

    return (
        <Page
            title={t("REG060_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="REG060_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport={true}
                    />
                </div>
            </div>

        </Page>
    );
}