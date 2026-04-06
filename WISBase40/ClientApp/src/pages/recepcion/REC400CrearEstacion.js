import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldCheckbox, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import * as Yup from 'yup';


function InternalREC400CrearEstacion(props) {

    const { t } = useTranslation();

    const initialValues = {

        descripcion: "",
        predio: "",

    };

    const validationSchema = {
        descripcion: Yup.string().required(),
        predio: Yup.string().required(),
    };

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            nexus.getForm("REC400_form_1").reset();
            props.onHide(props.nexus);
        }
    }

    return (

        <Form
            application="REC400CrearEstacion"
            id="REC400_form_1"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onAfterSubmit={handleFormAfterSubmit}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("REC400_Sec0_mdl_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="descripcion">{t("REC400_frm1_lbl_descripcion")}</label>
                                <Field name="descripcion" />
                                <StatusMessage for="descripcion" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="predio">{t("REC400_frm1_lbl_predio")}</label>
                                <FieldSelect name="predio" />
                                <StatusMessage for="predio" />
                            </div>
                        </Col>
                    </Row>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("REC400_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmar" variant="primary" label="REC400_frm1_btn_confirmar" />

            </Modal.Footer>
        </Form>
    );
}

export const REC400CrearEstacion = withPageContext(InternalREC400CrearEstacion);