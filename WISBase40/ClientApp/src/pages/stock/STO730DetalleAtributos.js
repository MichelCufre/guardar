import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Modal } from 'react-bootstrap';

export function STO730DetalleAtributos(props) {

    const { t } = useTranslation();

    const addParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "numeroAuditoria", value: props.numeroAuditoria }
        ];
    }

    return (
        <Page
            {...props}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("STO730DetalleAtributos_Sec0_modalTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <div className="row mb-4">
                    <div className="col-12">
                        <Grid
                            application="STO730DetalleAtributos"
                            id="STO730DetalleAtributos_grid_1"
                            rowsToFetch={30}
                            rowsToDisplay={15}
                            enableExcelExport
                            onBeforeFetch={addParameters}
                            onBeforeApplyFilter={addParameters}
                            onBeforeApplySort={addParameters}
                            onBeforeFetchStats={addParameters}
                            onBeforeExportExcel={addParameters}
                            onBeforeInitialize={addParameters}
                        />
                    </div>
                </div>
            </Modal.Body>

        </Page>
    );
}