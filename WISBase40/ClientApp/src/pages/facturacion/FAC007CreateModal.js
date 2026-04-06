import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import * as Yup from 'yup';

function InternalFAC007CreateModal(props) {

    const { t } = useTranslation();

    const validationSchema = {
        empresa: Yup.string().required(),
        facturacion: Yup.string().required(),
        nuComponente: Yup.string().required(),
        resultado: Yup.string().required(),
        dsAdicional: Yup.string().nullable()
    };

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        if (props.infoFacturacion) {

            let parameters =
                [
                    { id: "nuEjecucion", value: props.infoFacturacion.NuEjecucion },
                ];

            query.parameters = parameters;
        } 
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            nexus.getForm("FAC007CreateModal_form_1").reset();
            props.onHide(props.nexus);
        }
    }

    return (

        <Form
            application="FAC007CreateModal"
            id="FAC007CreateModal_form_1"
            validationSchema={validationSchema}
            onAfterSubmit={handleFormAfterSubmit}
            onBeforeInitialize={handleFormBeforeInitialize}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("FAC007_Sec0_mdlCreate_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="empresa">{t("FAC007_frm1_lbl_empresa")}</label>
                                <FieldSelect name="empresa" />
                                <StatusMessage for="empresa" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="facturacion">{t("FAC007_frm1_lbl_facturacion")}</label>
                                <FieldSelect name="facturacion" />
                                <StatusMessage for="facturacion" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="nuComponente">{t("FAC007_frm1_lbl_nuComponente")}</label>
                                <FieldSelect name="nuComponente" />
                                <StatusMessage for="nuComponente" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="resultado">{t("FAC007_frm1_lbl_resultado")}</label>
                                <Field name="resultado" />
                                <StatusMessage for="resultado" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="dsAdicional">{t("FAC007_frm1_lbl_dsAdicional")}</label>
                                <Field name="dsAdicional" />
                                <StatusMessage for="dsAdicional" />
                            </div>
                        </Col>
                    </Row>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("FAC007_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmar" variant="primary" label="FAC007_frm1_btn_confirmar" />
            </Modal.Footer>
        </Form>
    );
}

export const FAC007CreateModal = withPageContext(InternalFAC007CreateModal);