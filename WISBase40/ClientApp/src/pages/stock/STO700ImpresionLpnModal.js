import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import * as Yup from 'yup';

function InternalSTO700ImpresionLpnModal(props) {

    const { t } = useTranslation();

    const validationSchema = {

        predio: Yup.string().required(),
        impresora: Yup.string().required(),
        lenguaje: Yup.string().required(),
        descripcionLenguaje: Yup.string(),
        numCopias: Yup.string().required(),
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {

            props.onHide(props.nexus);
        }
    }

    const onBeforeSubmit = (context, form, query, nexus) => {
        if (props.rowSeleccionadas) {
            query.parameters = [{ id: "ListaFilasSeleccionadas", value: props.rowSeleccionadas }];
        }
        query.parameters.push({ id: "isSubmit", value: true });
    }

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                application="STO700ImpresionLpn"
                id="STO700Impresion_form_1"
                validationSchema={validationSchema}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
            >

                <Modal.Header closeButton>
                    <Modal.Title>{t("STO700_Sec0_mdl_ImprimirTitulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Container fluid>
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="predio">{t("IMP060_frm1_lbl_predio")}</label>
                                    <FieldSelect name="predio" />
                                    <StatusMessage for="predio" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="impresora">{t("IMP060_frm1_lbl_impresora")}</label>
                                    <FieldSelect name="impresora" />
                                    <StatusMessage for="impresora" />
                                </div>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <div className="form-group" >
                                    <label htmlFor="lenguaje">{t("IMP060_frm1_lbl_lenguaje")}</label>
                                    <Field name="lenguaje" />
                                    <StatusMessage for="lenguaje" />
                                </div>
                            </Col>
                            <Col sm={4}>
                                <div className="form-group" >
                                    <label htmlFor="descripcionLenguaje">{t("IMP060_frm1_lbl_descripcionLenguaje")}</label>
                                    <Field name="descripcionLenguaje" />
                                    <StatusMessage for="descripcionLenguaje" />
                                </div>
                            </Col>
                            <Col sm={4}>
                                <div className="form-group" >
                                    <label htmlFor="numCopias">{t("IMP060_frm1_lbl_numCopias")}</label>
                                    <Field name="numCopias" />
                                    <StatusMessage for="numCopias" />
                                </div>
                            </Col>
                        </Row>
                    </Container>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("IMP060_frm1_btn_cerrar")} </Button>
                    <SubmitButton id="btnSubmitConfirmar" variant="primary" label="IMP060_frm1_btn_confirmar" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}

export const STO700ImpresionLpnModal = withPageContext(InternalSTO700ImpresionLpnModal);
