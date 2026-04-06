import React, { useState, useRef, useEffect } from 'react';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import { useCustomTranslation } from '../../components/TranslationHook';
import { Form, Field, FieldSelect, FieldToggle, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';

function InternalREG070CreateZonaModal(props) {
    const { t } = useCustomTranslation();

    const initialValues = {
        idZona: "",
        descripcionZona: "",
        tipoZona: "",
        idZonaPicking: "",
        estacion: "",
        estacionAlmacenaje: "",
        habilitada: ""
    };

    const validationSchema = {

        idZona: Yup.string().required().max(20,"El campo no puede superar 20 caracteres."),
        descripcionZona: Yup.string().required().max(100, "El campo no puede superar 100 caracteres."),
        tipoZona: Yup.string().required(),
        idZonaPicking: Yup.string(),
        estacion: Yup.string().max(20, "El campo no puede superar 20 caracteres."),
        estacionAlmacenaje: Yup.string().max(20, "El campo no puede superar 20 caracteres."),
        habilitada: Yup.boolean(),
    };

    const onBeforeValidateField = (context, form, query, nexus) => {
        context.abortServerCall = true;
    }


    const handleClose = () => {
        props.onHide();
    };

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "isSubmit", value: true }
        ];
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("REG070_grid_1").refresh();
            props.onHide();
        }
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                id="REG070_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeSubmit={handleFormBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}

            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("REG070_Sec0_pageTitle_CreateTitle")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Container fluid>
                        <Row>
                            <Col>
                                <Row>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="idZona">{t("REG070_frm1_lbl_idZona")}</label>
                                            <Field name="idZona" />
                                            <StatusMessage for="idZona" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="descripcionZona">{t("REG070_frm1_lbl_descripcionZona")}</label>
                                            <Field name="descripcionZona" />
                                            <StatusMessage for="descripcionZona" />
                                        </div>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="tipoZona">{t("REG070_frm1_lbl_tipoZona")}</label>
                                            <FieldSelect name="tipoZona" />
                                            <StatusMessage for="tipoZona" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="idZonaPicking">{t("REG070_frm1_lbl_idZonaPicking")}</label>
                                            <FieldSelect name="idZonaPicking" />
                                            <StatusMessage for="idZonaPicking" />
                                        </div>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="estacion">{t("REG070_frm1_lbl_estacion")}</label>
                                            <Field name="estacion" />
                                            <StatusMessage for="estacion" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="estacionAlmacenaje">{t("REG070_frm1_lbl_estacionAlmacenaje")}</label>
                                            <Field name="estacionAlmacenaje" />
                                            <StatusMessage for="estacionAlmacenaje" />
                                        </div>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col>
                                        <div className="form-group" >
                                            <FieldToggle style={{ float: "left" }} name="habilitada" label="Habilitada" />
                                        </div>
                                    </Col>
                                </Row>
                            </Col>
                        </Row>
                    </Container>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                        {t("REG070_frm1_btn_CANCELAR")}
                    </Button>
                    <SubmitButton id="btnSubmitCreateZona" variant="primary" label="REG070_frm1_btn_CREAR" />
                </Modal.Footer>
            </Form>
        </Modal >
    );
}

export const REG070CreateZonaModal = withPageContext(InternalREG070CreateZonaModal);