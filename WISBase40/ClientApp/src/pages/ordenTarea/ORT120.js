import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';


export default function ORT120(props) {

    const { t } = useTranslation();

    const GridOnAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("ORT120_grid_1").refresh();
    }

    return (
        <Page
            title={t("ORT120_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="ORT120_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport enableExcelImport={false}
                        onAfterMenuItemAction={GridOnAfterMenuItemAction}
                    />
                </div>
            </div>

        </Page>
    );
}