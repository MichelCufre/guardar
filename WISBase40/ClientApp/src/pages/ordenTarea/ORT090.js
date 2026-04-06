import React, { useState } from 'react';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldCheckbox, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FormButton, FieldDateTime } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container, Div } from 'react-bootstrap';
import * as Yup from 'yup';

export default function ORT090(props) {

    const { t } = useTranslation();

    const validationSchema = {

        usuario: Yup.string(),
        nombre: Yup.string(),
        password: Yup.string(),
        codigoEmpresa: Yup.string(),
        descripcionEmpresa: Yup.string(),
        codigoTarea: Yup.string(),
        descripcionTarea: Yup.string(),
        fechaInicio: Yup.string(),
        fechaFin: Yup.string(),
        numeroOrden: Yup.string(),
        descripcionOrden: Yup.string(),
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") 
            window.location.reload();
    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        query.parameters.push({ id: "isSubmit", value: true });
    }

    const onBeforeButtonAction = (context, form, query, nexus) => {
        if (query.buttonId === "Cancelar") {
            window.location.reload();
            context.abortServerCall = true;
        }
    }

    return (

        <Page
            application="ORT090"
            title={t("ORT090_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                application="ORT090"
                id="ORT090_form_1"
                loadingOverlay={false}
                validationSchema={validationSchema}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeSubmit={onBeforeSubmit}
                onBeforeButtonAction={onBeforeButtonAction}
            >
                <Container fluid className="d-flex justify-content-center" style={{ width: "50%" }}>
                    <Row>
                        <Col>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="usuario">{t("ORT090_frm1_lbl_usuario")}</label>
                                        <Field name="usuario" />
                                        <StatusMessage for="usuario" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="nombre">{t("ORT090_frm1_lbl_nombre")}</label>
                                        <Field name="nombre" />
                                        <StatusMessage for="nombre" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" style={{ width: "50%" }}>
                                        <label htmlFor="password">{t("ORT090_frm1_lbl_password")}</label>
                                        <Field type="password" name="password" />
                                        <StatusMessage for="password" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="numeroOrden">{t("ORT090_frm1_lbl_numeroOrden")}</label>
                                        <FieldSelect name="numeroOrden" />
                                        <StatusMessage for="numeroOrden" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="descripcionOrden">{t("ORT090_frm1_lbl_descripcionOrden")}</label>
                                        <Field name="descripcionOrden" />
                                        <StatusMessage for="descripcionOrden" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="codigoTarea">{t("ORT090_frm1_lbl_codigoTarea")}</label>
                                        <FieldSelect name="codigoTarea" />
                                        <StatusMessage for="codigoTarea" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="descripcionTarea">{t("ORT090_frm1_lbl_descripcionTarea")}</label>
                                        <Field name="descripcionTarea" />
                                        <StatusMessage for="descripcionTarea" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="codigoEmpresa">{t("ORT090_frm1_lbl_codigoEmpresa")}</label>
                                        <FieldSelect name="codigoEmpresa" />
                                        <StatusMessage for="codigoEmpresa" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="descripcionEmpresa">{t("ORT090_frm1_lbl_descripcionEmpresa")}</label>
                                        <Field name="descripcionEmpresa" />
                                        <StatusMessage for="descripcionEmpresa" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="fechaInicio">{t("ORT090_frm1_lbl_fechaInicio")}</label>
                                        <FieldDateTime name="fechaInicio" />
                                        <StatusMessage for="fechaInicio" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="fechaFin">{t("ORT090_frm1_lbl_fechaFin")}</label>
                                        <FieldDateTime name="fechaFin" />
                                        <StatusMessage for="fechaFin" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <SubmitButton id="Confirmar" variant="primary" label="ORT090_frm1_btn_Confirmar" style={{ margin: "auto" }} />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <SubmitButton id="Terminar" variant="primary" label="ORT090_frm1_btn_Terminar" style={{ margin: "auto" }} />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <FormButton id="Cancelar" variant="primary" label="ORT090_frm1_btn_Cancelar" style={{ margin: "auto" }} />
                                    </div>
                                </Col>
                            </Row>

                        </Col>
                    </Row>
                </Container>
            </Form>
        </Page>
    );
}