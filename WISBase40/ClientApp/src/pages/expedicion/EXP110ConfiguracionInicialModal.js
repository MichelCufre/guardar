import React, { useState } from 'react';
import { Form, Field, FieldSelect, SubmitButton, FormButton, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import * as Yup from 'yup';


export function EXP110ConfiguracionInicialModal(props) {
    const { t } = useTranslation();
    const initialValues = {

        predio: "",
        impresora: "",
        estilo: "",
        lenguaje: "",
        descripcionLenguaje: "",
        ubicacion: "",
    };
    const validationSchema = {

        predio: Yup.string().required(),
        impresora: Yup.string().required(),
        estilo: Yup.string().required(),
        lenguaje: Yup.string(),
        descripcionLenguaje: Yup.string(),
        ubicacion: Yup.string().required(),
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        props.onHide(nexus, query);
    }
    const handleClose = () => {
        props.onHide(props.nexus);
    };
    const handleonBeforeInitialize = (context, form, data, nexus) => {

        data.parameters = [
            {
                id: "CONF_INICIAL",
                value: props.conf_inicial
            }
        ];
    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        query.parameters.push({ id: "isSubmit", value: true });
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                application="EXP110ConfInicialModal"
                id="EXP110ConfInicial_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeInitialize={handleonBeforeInitialize}
            >

                <Modal.Header closeButton>
                    <Modal.Title>{t("EXP110ConfInicial_Sec0_mdl_Configuracion_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Container>
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="predio">{t("EXP110ConfInicial_frm1_lbl_predio")}</label>
                                    <FieldSelect name="predio" />
                                    <StatusMessage for="predio" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="ubicacion">{t("EXP110ConfInicial_frm1_lbl_ubicacion")}</label>
                                    <FieldSelect name="ubicacion" />
                                    <StatusMessage for="ubicacion" />
                                </div>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="impresora">{t("EXP110ConfInicial_frm1_lbl_impresora")}</label>
                                    <FieldSelect name="impresora" />
                                    <StatusMessage for="impresora" />
                                </div>
                            </Col>
                            <Col>
                                <Row>
                                    <Col sm={4}>
                                        <div className="form-group" >
                                            <label htmlFor="lenguaje">{t("EXP110ConfInicial_frm1_lbl_lenguaje")}</label>
                                            <Field name="lenguaje" readOnly />
                                            <StatusMessage for="lenguaje" />
                                        </div>
                                    </Col>
                                    <Col sm={8}>
                                        <div className="form-group" >
                                            <label htmlFor="descripcionLenguaje">{t("EXP110ConfInicial_frm1_lbl_descripcionLenguaje")}</label>
                                            <Field name="descripcionLenguaje" readOnly />
                                            <StatusMessage for="descripcionLenguaje" />
                                        </div>
                                    </Col>
                                </Row>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="estilo">{t("EXP110ConfInicial_frm1_lbl_estilo")}</label>
                                    <FieldSelect name="estilo" />
                                    <StatusMessage for="estilo" />
                                </div>
                            </Col>
                        </Row>

                    </Container>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("EXP110ConfInicial_frm1_btn_cerrar")} </Button>
                    <SubmitButton id="btnSubmitConfirmar" variant="primary" label="EXP110ConfInicial_frm1_btn_confirmar" />
                </Modal.Footer>
            </Form>

        </Modal>
    );
}

