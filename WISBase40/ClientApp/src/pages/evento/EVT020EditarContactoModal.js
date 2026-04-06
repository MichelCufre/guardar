import React, { useState } from 'react';
import { Page } from '../../components/Page';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldToggle, FieldSelect, StatusMessage, FieldDate, FieldSelectAsync, SubmitButton } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
export function EVT020EditarContactoModal(props) {
    const { t } = useTranslation();

    const [_nexus, setNexus] = useState(null);

    const initialValues = {
        NM_CONTACTO: "",
        DS_CONTACTO: "",
        NU_TELEFONO: "",
        DS_EMAIL: "",
        CD_EMPRESA: "",
        CD_CLIENTE: "",
    };

    const validationSchema = {
        NM_CONTACTO: Yup.string().required().max(100),
        DS_CONTACTO: Yup.string().required().max(200),
        NU_TELEFONO: Yup.string(),
        DS_EMAIL: Yup.string().required().max(100),
        CD_EMPRESA: Yup.string().required(),
        CD_CLIENTE: Yup.string().nullable().max(200),
    };

    const handleClose = () => {
        props.onHide();
    }

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        setNexus(nexus);

        query.parameters = [
            { id: "numeroContacto", value: props.numeroContacto },
        ];
    }

    const handleSubmit = () => {
        _nexus.getForm("EVT020_form_1").submit();
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            props.onHide();
        }
    }

    const onBeforeSubmit = (context, form, query, nexus) => {
        query.parameters.push(
            { id: "numeroContacto", value: props.numeroContacto },
            { id: "isSubmit", value: true });
    }

    return (
        <Page
            {...props}
        >
            <Modal show={props.show} onHide={handleClose} dialogClassName="modal-70w" backdrop="static">
                <Modal.Header closeButton>
                    <Modal.Title>{t("EVT020_frm1_btn_EditarContacto")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form
                        application="EVT020EditarContacto"
                        id="EVT020_form_1"
                        initialValues={initialValues}
                        validationSchema={validationSchema}
                        onBeforeInitialize={handleFormBeforeInitialize}
                        onAfterSubmit={handleFormAfterSubmit}
                        onBeforeSubmit={onBeforeSubmit}
                    >
                        <Row>
                            <Col lg={2}>
                                <label htmlFor="NU_CONTACTO">{t("EVT020_grid1_colname_NU_CONTACTO")}</label>
                                <Field name="NU_CONTACTO" readOnly />
                                <StatusMessage for="NU_CONTACTO" readOnly />
                            </Col >
                            <Col lg={4}>
                                <label htmlFor="NM_CONTACTO">{t("EVT020_grid1_colname_NM_CONTACTO")}</label>
                                <Field name="NM_CONTACTO" />
                                <StatusMessage for="NM_CONTACTO" readOnly />
                            </Col >
                            <Col lg={6}>
                                <label htmlFor="DS_CONTACTO">{t("EVT020_grid1_colname_DS_CONTACTO")}</label>
                                <Field name="DS_CONTACTO" />
                                <StatusMessage for="DS_CONTACTO" />
                            </Col>
                        </Row>
                        <Row>
                            <Col lg={6}>
                                <label htmlFor="DS_EMAIL">{t("EVT020_grid1_colname_DS_EMAIL")}</label>
                                <Field name="DS_EMAIL" />
                                <StatusMessage for="DS_EMAIL" />
                            </Col>
                            <Col lg={6}>
                                <label htmlFor="NU_TELEFONO">{t("EVT020_grid1_colname_NU_TELEFONO")}</label>
                                <Field name="NU_TELEFONO" />
                                <StatusMessage for="NU_TELEFONO" />
                            </Col>
                        </Row>
                        <Row >
                            <Col lg={6}>
                                <label htmlFor="CD_EMPRESA">{t("EVT020_grid1_colname_NM_EMPRESA")}</label>
                                <FieldSelectAsync name="CD_EMPRESA" />
                                <StatusMessage for="CD_EMPRESA" />
                            </Col>
                            <Col lg={6}>
                                <label htmlFor="CD_CLIENTE">{t("EVT020_grid1_colname_AGENTE")}</label>
                                <FieldSelectAsync name="CD_CLIENTE" isClearable />
                                <StatusMessage for="CD_CLIENTE" />
                            </Col>
                        </Row>
                    </Form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("EVT020_frm1_btn_Cerrar")} </Button>
                    <Button variant="primary" onClick={handleSubmit}> {t("EVT020_frm1_btn_Confirmar")} </Button>
                </Modal.Footer>
            </Modal>
        </Page>
    );
}