import React, { useState, useRef } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useCustomTranslation } from '../../components/TranslationHook';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { notificationType } from '../../components/Enums';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';

function InternalREC010CreateReferenciaModal(props) {

    const { t } = useCustomTranslation();

    const initialValues = {

        codigo: "",
        tipoReferencia: "",
        idEmpresa: "",
        codigoInternoAgente: "",
        numeroPredio: "",
        fechaVencimiento: "",
        fechaEmitida: "",
        fechaEntrega: "",
        memo: "",
        anexo1: "",
        anexo2: "",
        anexo3: "",
        moneda: ""
    };

    const validationSchema = {

        codigo: Yup.string().required(),
        tipoReferencia: Yup.string().required(),
        idEmpresa: Yup.string().required(),
        codigoInternoAgente: Yup.string().required(),
        numeroPredio: Yup.string().required(),
        fechaVencimiento: Yup.date().transform(value => (isNaN(value) ? undefined : value)),
        fechaEmitida: Yup.date().transform(value => (isNaN(value) ? undefined : value)).required(t("General_Sec0_Error_Error25")),
        fechaEntrega: Yup.date().transform(value => (isNaN(value) ? undefined : value)),//.transform(value => (isNaN(value) ? undefined : value)).required(t("General_Sec0_Error_Error25")),
        memo: Yup.string(),
        anexo1: Yup.string(),
        anexo2: Yup.string(),
        anexo3: Yup.string(),
        moneda: Yup.string(),

    };

    const handleClose = () => {
        props.onHide();
    };


    const handleFormBeforeValidateField = (context, form, query, nexus) => {

    }

    const handleFormAfterValidateField = (context, form, query, nexus) => {


    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {


    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {

            if (query.buttonId == "btnSubmitConfirmarIrDetalle") {

                props.onHide(query.parameters.find(a => a.id === "idReferencia").value);

            } else {

                nexus.getGrid("REC010_grid_1").refresh();
                props.onHide(null);

            }
        }
    }

    const handleFormAfterInitialize = (context, form, query, nexus) => {

    };


    return (
        // <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
        <Form
            application="REC010Create"
            id="REC010Create_form_1"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onBeforeSubmit={handleFormBeforeSubmit}
            onAfterSubmit={handleFormAfterSubmit}
            onBeforeValidateField={handleFormBeforeValidateField}
            onAfterValidateField={handleFormAfterValidateField}
            onAfterInitialize={handleFormAfterInitialize}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("REC010_Sec0_mdlCreate_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="idEmpresa">{t("REC010_frm1_lbl_idEmpresa")}</label>
                                        <FieldSelectAsync name="idEmpresa" />
                                        <StatusMessage for="idEmpresa" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="codigoInternoAgente">{t("REC010_frm1_lbl_codigoInternoAgente")}</label>
                                        <FieldSelectAsync name="codigoInternoAgente" />
                                        <StatusMessage for="codigoInternoAgente" />
                                    </div>
                                </Col>
                            </Row>
                            <Row >
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="numeroPredio">{t("REC010_frm1_lbl_numeroPredio")}</label>
                                        <FieldSelect name="numeroPredio" />
                                        <StatusMessage for="numeroPredio" />
                                    </div>
                                </Col>

                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="moneda">{t("REC010_frm1_lbl_moneda")}</label>
                                        <FieldSelect name="moneda" />
                                        <StatusMessage for="moneda" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col >
                                    <div className="form-group" >
                                        <label htmlFor="memo">{t("REC010_frm1_lbl_memo")}</label>
                                        <FieldTextArea name="memo" maxLength="200" />
                                        <StatusMessage for="memo" />
                                    </div>

                                    <div className="form-group" >
                                        <label htmlFor="anexo1">{t("REC010_frm1_lbl_anexo1")}</label>
                                        <FieldTextArea name="anexo1" maxLength="200" />
                                        <StatusMessage for="anexo1" />
                                    </div>
                                </Col>

                            </Row>

                        </Col>
                        <Col>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="tipoReferencia">{t("REC010_frm1_lbl_tipoReferencia")}</label>
                                        <FieldSelect name="tipoReferencia" />
                                        <StatusMessage for="tipoReferencia" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="codigo">{t("REC010_frm1_lbl_codigo")}</label>
                                        <Field name="codigo" maxLength="20" />
                                        <StatusMessage for="codigo" />
                                    </div>
                                </Col>
                            </Row>

                            <Row >
                                <Col >
                                    <div className="form-group" >
                                        <label htmlFor="fechaVencimiento">{t("REC010_frm1_lbl_fechaVencimiento")}</label>
                                        <FieldDate name="fechaVencimiento" />
                                        <StatusMessage for="fechaVencimiento" />
                                    </div>
                                </Col>
                                <Col >
                                    <div className="form-group" >
                                        <label htmlFor="fechaEmitida">{t("REC010_frm1_lbl_fechaEmitida")}</label>
                                        <FieldDate name="fechaEmitida" />
                                        <StatusMessage for="fechaEmitida" />
                                    </div>
                                </Col>
                                <Col >
                                    <div className="form-group" >
                                        <label htmlFor="fechaEntrega">{t("REC010_frm1_lbl_fechaEntrega")}</label>
                                        <FieldDate name="fechaEntrega" />
                                        <StatusMessage for="fechaEntrega" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="anexo2">{t("REC010_frm1_lbl_anexo2")}</label>
                                        <FieldTextArea name="anexo2" maxLength="200" />
                                        <StatusMessage for="anexo2" />
                                    </div>
                                    <div className="form-group" >
                                        <label htmlFor="anexo3">{t("REC010_frm1_lbl_anexo3")}</label>
                                        <FieldTextArea name="anexo3" maxLength="200" />
                                        <StatusMessage for="anexo3" />
                                    </div>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("REC010_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmar" variant="primary" label="REC010_frm1_btn_confirmar" />
                <SubmitButton id="btnSubmitConfirmarIrDetalle" variant="primary" label="REC010_frm1_btn_confirmarIrDetalle" />
            </Modal.Footer>
        </Form>
        //  </Modal>
    );

}

export const REC010CreateReferenciaModal = withPageContext(InternalREC010CreateReferenciaModal);