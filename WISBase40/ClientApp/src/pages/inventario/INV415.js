import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';

export default function INV415(props) {
    const { t } = useTranslation();

    return (
        <Page
            title={t("INV415_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="INV415_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                    />
                </div>
            </div>

        </Page>
    );
}