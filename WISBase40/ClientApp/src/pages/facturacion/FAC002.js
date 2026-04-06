import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';


export default function FAC002(props) {

    const { t } = useTranslation();

    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("FAC002_grid_1").refresh();
    }

    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("FAC002_grid_1").refresh();
    }

    const handleGridBeforeValidate = (context, data, nexus) => {
    }

    return (
        <Page
            title={t("FAC002_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="FAC002_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport enableSelection enableExcelImport={false}
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        onAfterCommit={onAfterCommit}
                        onBeforeValidateRow={handleGridBeforeValidate}
                    />
                </div>
            </div>

        </Page>
    );
}