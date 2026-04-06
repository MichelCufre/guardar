import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import * as Yup from 'yup';

function InternalREC275CrearEstrategiaModal(props) {

    const { t } = useTranslation();

    const initialValues = {
        nombreEstrategia: "",
        codigoEstrategia: "",
    };

    const validationSchema = {
        nombreEstrategia: Yup.string(),
    };

    const handleClose = () => {
        props.onHide(null, props.nexus);
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            var numeroEstrategia = query.buttonId === "btnSubmitConfirmarEstrategia" ? null : props.codigoEstrategia;

            if (!props.editMode) {
                numeroEstrategia = query.parameters.find(p => p.id === "numeroEstrategia").value;
            }

            nexus.getForm("REC275_form_1").reset();
            props.onHide(numeroEstrategia, nexus);
        }
    }

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            { id: "modoEditar", value: false },
            { id: "numeroEstrategia", value: props.codigoEstrategia },
        ];
    };

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "modoEditar", value: false },
            { id: "numeroEstrategia", value: props.codigoEstrategia },
        ];
    }

    return (

        <Form
            application="REC275"
            id="REC275_form_1"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onAfterSubmit={handleFormAfterSubmit}
            onBeforeSubmit={handleFormBeforeSubmit}
            onBeforeInitialize={handleFormBeforeInitialize}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("REC275_Sec0_mdlCreate_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="codigoEstrategia">{t("REC275_grid1_colname_CodigoEstrategia")}</label>
                                <Field name="codigoEstrategia" readOnly />
                                <StatusMessage for="codigoEstrategia" />
                            </div>
                        </Col>

                        <Col>
                            <div className="form-group" >
                                <label htmlFor="nombreEstrategia">{t("REC275_grid1_colname_NombreEstrategia")}</label>
                                <Field name="nombreEstrategia" />
                                <StatusMessage for="nombreEstrategia" />
                            </div>
                        </Col>
                    </Row>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("REC275_frm1_btn_Cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmarEstrategia" variant="primary" label="REC275_frm1_btn_Confirmar" />
            </Modal.Footer>
        </Form>
    );
}

export const REC275CrearEstrategiaModal = withPageContext(InternalREC275CrearEstrategiaModal);