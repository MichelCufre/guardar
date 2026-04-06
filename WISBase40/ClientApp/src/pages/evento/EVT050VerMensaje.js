import React, { useState } from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldText, Form } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
export function EVT050VerMensaje(props) {
    const { t } = useTranslation();

    const [isHtmlBody, setIsHtmlBody] = useState(false)

    const initialValues = {
        notificacion: "",
        destinatario: "",
        asunto: "",
        cuerpo: "",
    };

    const validationSchema = {
        notificacion: Yup.string(),
        destinatario: Yup.string(),
        asunto: Yup.string(),
        cuerpo: Yup.string(),
    };

    const handleClose = () => {
        props.onHide();
    }

    const addParameters = (context, form, data, nexus) => {
        data.parameters = [
            {
                id: "notificacion",
                value: props.notificacion
            }
        ];
    }

    const onAfterInitialize = (context, form, query, nexus) => {
        var isHtmlBody = query.parameters.find(w => w.id == "isHtmlBody").value == "true";
        setIsHtmlBody(isHtmlBody);
    };

    return (
        <Page
            {...props}
        >
            <Modal show={props.show} onHide={handleClose} dialogClassName="modal-70w" backdrop="static">
                <Modal.Header closeButton>
                    <Modal.Title>{t("EVT050_Sec0_btn_VerMensaje")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form
                        application="EVT050VerMensaje"
                        id="EVT050_form_1"
                        initialValues={initialValues}
                        onBeforeInitialize={addParameters}
                        onAfterInitialize={onAfterInitialize}
                    >
                        <Row>
                            <Col lg={12}>
                                <label htmlFor="notificacion">{t("EVT050VerMensaje_frm1_colname_notificacion")}</label>
                                <Field name="notificacion" readOnly />
                            </Col >
                        </Row>
                        <Row>
                            <Col lg={12}>
                                <label htmlFor="asunto">{t("EVT050VerMensaje_frm1_colname_asunto")}</label>
                                <Field name="asunto" readOnly />
                            </Col >
                        </Row>
                        <Row>
                            <Col lg={12}>
                                <label htmlFor="destinatario">{t("EVT050VerMensaje_frm1_colname_destinatario")}</label>
                                <Field name="destinatario" readOnly />
                            </Col >
                        </Row>
                        <Row>
                            <Col lg={12}>
                                <label htmlFor="cuerpo">{t("EVT050VerMensaje_frm1_colname_cuerpo")}</label>
                                <FieldText name="cuerpo" height={250} isHtml={isHtmlBody} readOnly />
                            </Col>
                        </Row>
                    </Form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("EVT050VerMensaje_frm1_btn_Cerrar")} </Button>
                </Modal.Footer>
            </Modal>
        </Page>
    );
}