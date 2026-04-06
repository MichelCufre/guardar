import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import * as Yup from 'yup';

function InternalImpresionHerramientasPosicionModal(props) {

    const { t } = useTranslation();

    const validationSchema = {

        predio: Yup.string().required(),
        impresora: Yup.string().required(),
        estilo: Yup.string().required(),
        lenguaje: Yup.string().required(),
        descripcionLenguaje: Yup.string(),
        minimo: Yup.string().required(),
        maximo: Yup.string().required()
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        if (context.responseStatus === "OK") {
            props.onHide(props.nexus);
        }
    }
    const onBeforeSubmit = (context, form, query, nexus) => {
        if (props.equipo) {
            const value = props.equipo.find(a => a.id === "equipo").value;
            query.parameters = [{ id: "equipo", value: value }, { id: "isSubmit", value: true}];
        } else {
            context.abortServerCall = true;
        }
    }

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                application="IMP050HerramientasPosicion"
                id="IMP050HerramientasPosicion_form_1"
                validationSchema={validationSchema}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
            >

                <Modal.Header closeButton>
                    <Modal.Title>{t("IMP050_Sec0_mdl_herramientasPosicion_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Container fluid>
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="predio">{t("IMP050_frm1_lbl_predio")}</label>
                                    <FieldSelect name="predio" />
                                    <StatusMessage for="predio" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="impresora">{t("IMP050_frm1_lbl_impresora")}</label>
                                    <FieldSelect name="impresora" />
                                    <StatusMessage for="impresora" />
                                </div>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="estilo">{t("IMP050_frm1_lbl_estilo")}</label>
                                    <FieldSelect name="estilo" />
                                    <StatusMessage for="estilo" />
                                </div>
                            </Col>
                            <Col>
                                <Row>
                                    <Col sm={4}>
                                        <div className="form-group" >
                                            <label htmlFor="lenguaje">{t("IMP050_frm1_lbl_lenguaje")}</label>
                                            <Field name="lenguaje" />
                                            <StatusMessage for="lenguaje" />
                                        </div>
                                    </Col>
                                    <Col sm={8}>
                                        <div className="form-group" >
                                            <label htmlFor="descripcionLenguaje">{t("IMP050_frm1_lbl_descripcionLenguaje")}</label>
                                            <Field name="descripcionLenguaje" />
                                            <StatusMessage for="descripcionLenguaje" />
                                        </div>
                                    </Col>
                                </Row>
                            </Col>

                        </Row>
                        <Row>
                            <Col>
                                <Row>
                                    <Col sm={6}>
                                        <div className="form-group" >
                                            <label htmlFor="minimo">{t("IMP050_frm1_lbl_minimo")}</label>
                                            <Field name="minimo" />
                                            <StatusMessage for="minimo" />
                                        </div>
                                    </Col>
                                    <Col sm={6}>
                                        <div className="form-group" >
                                            <label htmlFor="maximo">{t("IMP050_frm1_lbl_maximo")}</label>
                                            <Field name="maximo" />
                                            <StatusMessage for="maximo" />
                                        </div>
                                    </Col>
                                </Row>
                            </Col>
                        </Row>
                    </Container>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("IMP050_frm1_btn_cerrar")} </Button>
                    <SubmitButton id="btnSubmitConfirmar" variant="primary" label="IMP050_frm1_btn_confirmar" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}

export const IMP050ImpresionHerramientaPosicionModal = withPageContext(InternalImpresionHerramientasPosicionModal);
