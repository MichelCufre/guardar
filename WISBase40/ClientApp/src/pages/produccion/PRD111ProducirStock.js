import React, { useState } from 'react';
import { Modal, Col, Row, Button, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';

export default function PRD111ProducirStock(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const initialValues = {};
    const validationSchema = {};

    const onBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            { id: "codigoEspacio", value: props.codigoEspacio }
        ];
    }

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "codigoEspacio", value: props.codigoEspacio }
        ];
    };

    const onAfterCommit = (context, rows, parameters, nexus) => {

        if (context.status === "ERROR") return;

        nexus.getGrid("PRD111ProducirStock_grid_1").refresh();
    }
    
    const handleClose = () => {
        props.onHide();
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-70w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRD111ProducirStock_Sec0_Title_ProducirStock")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    application="PRD111ProducirStock"
                    {...props}
                >
                    <Form
                        application="PRD111ProducirStock"
                        id="PRD111ProducirStock_form_1"
                        initialValues={initialValues}
                        validationSchema={validationSchema}
                        onBeforeInitialize={onBeforeInitialize}
                    >
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="codigo">{t("PRD111ConsumirStock_form_lbl_codigo")}</label>
                                    <Field name="codigo" readOnly />
                                    <StatusMessage for="codigo" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="descripcion">{t("PRD111ConsumirStock_form_lbl_descripcion")}</label>
                                    <Field name="descripcion" readOnly />
                                    <StatusMessage for="descripcion" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="tipo">{t("PRD111ConsumirStock_form_lbl_tipo")}</label>
                                    <Field name="tipo" readOnly />
                                    <StatusMessage for="tipo" />
                                </div>
                            </Col>
                        </Row>
                    </Form>

                    <hr />

                    <Grid
                        id="PRD111ProducirStock_grid_1"
                        application="PRD111ProducirStock"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeInitialize={applyParameters}
                        onBeforeSelectSearch={applyParameters}
                        onBeforeValidateRow={applyParameters}
                        onBeforeCommit={applyParameters}
                        onAfterCommit={onAfterCommit}
                        onBeforeExportExcel={applyParameters}
                        onBeforeFetch={applyParameters}
                        onBeforeFetchStats={applyParameters}
                        onBeforeApplySort={applyParameters}
                        onBeforeApplyFilter={applyParameters}

                    />
                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="outline-secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_Cerrar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}