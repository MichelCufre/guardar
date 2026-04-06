import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';


export default function FAC005(props) {

    const { t } = useTranslation();

    return (
        <Page
            title={t("FAC005_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="FAC005_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport enableExcelImport={false}
                    />
                </div>
            </div>

        </Page>
    );
}