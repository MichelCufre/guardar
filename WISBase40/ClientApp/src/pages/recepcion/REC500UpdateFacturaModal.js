import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { useCustomTranslation } from '../../components/TranslationHook';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime, FieldCheckbox } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';

function InternalREC500UpdateFacturaModal(props) {
    const { t } = useCustomTranslation();

    const initialValues = {
        numeroPredio: "",
        numeroSerie: "",
        numeroFactura: "",
        tipoFactura: "",
        idEmpresa: "",
        emision: "",
        totalDigitado: "",
        codigoInternoAgente: "",
        vencimiento: "",
        moneda: "",
        anexo1: "",
        anexo2: "",
        anexo3: "",
        observacion: "",
        ivaBase: "",
        ivaMinimo: "",
    };

    const validationSchema = {
        numeroPredio: Yup.string().required(),
        numeroSerie: Yup.string(),
        numeroFactura: Yup.string().required(),
        tipoFactura: Yup.string().required(),
        idEmpresa: Yup.string().required(),
        emision: Yup.string().required(),
        totalDigitado: Yup.string(),
        codigoInternoAgente: Yup.string().required(),
        vencimiento: Yup.string(),
        moneda: Yup.string().required(),
        anexo1: Yup.string(),
        anexo2: Yup.string(),
        anexo3: Yup.string(),
        observacion: Yup.string(),
        ivaBase: Yup.string(),
        ivaMinimo: Yup.string()
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        context.showErrorMessage = true;
        if (context.responseStatus === "OK") {
            if (query.buttonId == "btnSubmitConfirmarIrDetalle") {
                props.nexus.getGrid("REC500_grid_1").refresh();
                props.onHide(query.parameters.find(a => a.id === "idFactura").value, "detalles");
            }
            else {
                props.nexus.getGrid("REC500_grid_1").refresh();
                props.onHide(null, null);
            }
        }
    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "keyFactura", value: props.factura.find(a => a.id === "idFactura").value }

        ];
    }


    const handleFormBeforeInitialize = (context, form, query, nexus) => {

        query.parameters = [

            { id: "keyFactura", value: props.factura.find(a => a.id === "idFactura").value }
        ];

    }

    const handleFormAfterInitialize = (context, form, query, nexus) => {

    };

    if (!props.show) {
        return null;
    }

    return (
        <Page
            //title={t("REC500_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                application="REC500Update"
                id="REC500Update_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={handleFormBeforeInitialize}
                onAfterInitialize={handleFormAfterInitialize}
                onBeforeSubmit={handleFormBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("REC500_Sec0_mdlUpdate_Titulo")} {`${props.factura.find(a => a.id === "idFactura").value}`}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Container fluid>
                        <Tabs defaultActiveKey="Datos" transition={false} id="noanim-tab-example">
                            <Tab eventKey="Datos" title={t("REC500_frm1_tab_datos")} >
                                <br></br>
                                <Row>
                                    <Col lg={6}>
                                        <Row className="mb-2">
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="idEmpresa">{t("REC500_frm1_lbl_idEmpresa")}</label>
                                                    <FieldSelectAsync name="idEmpresa" readOnly />
                                                    <StatusMessage for="idEmpresa" />
                                                </div>
                                            </Col>
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="codigoInternoAgente">{t("REC500_frm1_lbl_codigoInternoAgente")}</label>
                                                    <FieldSelectAsync name="codigoInternoAgente" readOnly />
                                                    <StatusMessage for="codigoInternoAgente" />
                                                </div>
                                            </Col>
                                        </Row>
                                        <Row className="mb-2">
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="numeroPredio">{t("REC500_frm1_lbl_numeroPredio")}</label>
                                                    <FieldSelect name="numeroPredio" readOnly />
                                                    <StatusMessage for="numeroPredio" />
                                                </div>
                                            </Col>
                                            <Col >
                                                <div className="form-group" >
                                                    <label htmlFor="vencimiento">{t("REC500_frm1_lbl_vencimiento")}</label>
                                                    <FieldDate name="vencimiento" />
                                                    <StatusMessage for="vencimiento" />
                                                </div>
                                            </Col>
                                        </Row>
                                        <Row className="mb-2">
                                            <Col >
                                                <div className="form-group" >
                                                    <label htmlFor="totalDigitado">{t("REC500_frm1_lbl_totalDigitado")}</label>
                                                    <Field name="totalDigitado" />
                                                    <StatusMessage for="totalDigitado" />
                                                </div>
                                            </Col>
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="ivaBase">{t("REC500_frm1_lbl_ivaBase")}</label>
                                                    <Field name="ivaBase" />
                                                    <StatusMessage for="ivaBase" />
                                                </div>
                                            </Col>
                                        </Row>
                                    </Col>
                                    <Col lg={6}>
                                        <Row className="mb-2">
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="numeroSerie">{t("REC500_frm1_lbl_numeroSerie")}</label>
                                                    <Field name="numeroSerie" maxLength="20" readOnly />
                                                    <StatusMessage for="numeroSerie" />
                                                </div>
                                            </Col>
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="numeroFactura">{t("REC500_frm1_lbl_numeroFactura")}</label>
                                                    <Field name="numeroFactura" maxLength="20" readOnly />
                                                    <StatusMessage for="numeroFactura" />
                                                </div>
                                            </Col>
                                        </Row>
                                        <Row className="mb-2">
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="tipoFactura">{t("REC500_frm1_lbl_tipoFactura")}</label>                                                    
                                                    <FieldSelect name="tipoFactura" isClearable />
                                                    <StatusMessage for="tipoFactura" />
                                                </div>
                                            </Col>
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="emision">{t("REC500_frm1_lbl_emision")}</label>
                                                    <FieldDate name="emision" />
                                                    <StatusMessage for="emision" />
                                                </div>
                                            </Col>
                                        </Row>
                                        <Row className="mb-2">
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="ivaMinimo">{t("REC500_frm1_lbl_ivaMinimo")}</label>
                                                    <Field name="ivaMinimo" />
                                                    <StatusMessage for="ivaMinimo" />
                                                </div>
                                            </Col>
                                            <Col>
                                                <div className="form-group" >
                                                    <label htmlFor="moneda">{t("REC500_frm1_lbl_moneda")}</label>
                                                    <FieldSelect name="moneda" />
                                                    <StatusMessage for="moneda" />
                                                </div>
                                            </Col>
                                        </Row>
                                    </Col>

                                </Row>
                                <Row>
                                    <Col>
                                        <hr></hr>
                                    </Col>
                                </Row>

                            </Tab>
                            <Tab eventKey="anexos" title={t("REC500_frm1_tab_anexos")}>
                                <br></br>
                                <Row className="mb-2">
                                    <Col >
                                        <div className="form-group" >
                                            <label htmlFor="anexo1">{t("REC500_frm1_lbl_anexo1")}</label>
                                            <FieldTextArea name="anexo1" maxLength="200" />
                                            <StatusMessage for="anexo1" />
                                        </div>
                                    </Col>
                                </Row>
                                <Row className="mb-2">
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="anexo2">{t("REC500_frm1_lbl_anexo2")}</label>
                                            <FieldTextArea name="anexo2" maxLength="200" />
                                            <StatusMessage for="anexo2" />
                                        </div>
                                    </Col>
                                </Row>
                                <Row className="mb-2">
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="anexo3">{t("REC500_frm1_lbl_anexo3")}</label>
                                            <FieldTextArea name="anexo3" maxLength="200" />
                                            <StatusMessage for="anexo3" />
                                        </div>
                                    </Col>
                                </Row>
                                <Row className="mb-2">
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="observacion">{t("REC500_frm1_lbl_observacion")}</label>
                                            <FieldTextArea name="observacion" maxLength="200" />
                                            <StatusMessage for="observacion" />
                                        </div>
                                    </Col>
                                </Row>
                            </Tab>
                        </Tabs>
                    </Container>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("REC500_frm1_btn_cerrar")} </Button>
                    <SubmitButton id="btnSubmitConfirmar" variant="primary" label="REC500_frm1_btn_confirmar" />
                    <SubmitButton id="btnSubmitConfirmarIrDetalle" variant="primary" label="REC500_frm1_btn_confirmarIrDetalle" />
                </Modal.Footer>
            </Form >
        </Page>
    );
}
export const REC500UpdateFacturaModal = withPageContext(InternalREC500UpdateFacturaModal);