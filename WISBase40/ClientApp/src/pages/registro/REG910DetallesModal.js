import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldCheckbox, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import * as Yup from 'yup';

function InternalREG910DetallesModal(props) {

    const { t } = useTranslation();

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "codigoDominio", value: props.codigoDominio },
            { id: "dominioInterno", value: props.dominioInterno },
        ];
    };

    const onAfterSubmit = (context, form, query, nexus) => {
        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            props.onHide(nexus);
        }
    }

    const handleClose = () => {
        props.onHide();
    };

    return (

        <Form
            application="REG910Detalles"
            id="REG910_form_1"
            onAfterSubmit={onAfterSubmit}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("REG910_Sec0_mdlEdit_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <div className="row mb-4">
                        <div className="col-12">
                            <Grid id="REG910_grid_2" application="REG910Detalles" rowsToFetch={30} rowsToDisplay={15} enableExcelExport={true}
                                onBeforeFetch={applyParameters}
                                onBeforeInitialize={applyParameters}
                                onBeforeExportExcel={applyParameters}
                                onBeforeFetchStats={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
                                onBeforeCommit={applyParameters}
                            />
                        </div>
                    </div>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("FAC251_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmar" variant="primary" label="FAC251_frm1_btn_confirmar" />
            </Modal.Footer>
        </Form>
    );
}

export const REG910DetallesModal = withPageContext(InternalREG910DetallesModal);