import React, { useState } from 'react';
import { Modal, Col, Row, Button } from 'react-bootstrap';
import { StatusMessage, Field, FieldCheckbox, SubmitButton, Form, FieldSelect } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';

export function COF110CreateOrUpdate(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const TP_AUTH_OAUTH2 = "TPAUTHSRVOAUTH2";

    const [mostrarOAuthFields, setMostrarOAuthFields] = useState(false);

    const initialValues = {
        isUpdatePassword: false,
        isEnabled: true,
        tipoAutorizacion: "",
        user: "",
        scope: "",
        authServer: "",
        urlIntegracion: "",
        tipoComunicacion: "",
        descripcion: "",
        codigo: "",
    };

    const validationSchema = {
        tipoAutorizacion: Yup.string(),
        user: Yup.string(),
        password: Yup.string(),
        scope: Yup.string(),
        authServer: Yup.string(),
        urlIntegracion: Yup.string(),
        tipoComunicacion: Yup.string(),
        descripcion: Yup.string().required(),
        codigo: Yup.string().required(),
        isEnabled: Yup.boolean(),
        isUpdatePassword: Yup.boolean(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const onBeforeInitialize = (context, form, data, nexus) => {
        if (props.isUpdate) {
            data.parameters.push({ id: "isUpdate", value: "S" });
            data.parameters.push({ id: "nroIntegracion", value: props.nroIntegracion });
        }
    }

    const onAfterInitialize = (context, form, query, nexus) => {

        if (query.parameters.some(x => x.id == "SHOW_OAUTH2_FIELDS")) {
            var value = query.parameters.find(x => x.id == "SHOW_OAUTH2_FIELDS").value;

            if (value === "S") {
                setMostrarOAuthFields(true);
            }
        }

        if (props.isUpdate) {
            document.getElementById("passwordField").readOnly = true;
        }
    };

    const onBeforeValidateField = (context, form, query, nexus) => {

        if (query.fieldId === "isUpdatePassword") {

            context.abortServerCall = true;

            var updatePassword = nexus.getForm("COF110_form_1").getFieldValue("isUpdatePassword");

            document.getElementById("passwordField").readOnly = !updatePassword;
        }
    };

    const handleFormAfterValidateField = (context, form, query, nexus) => {

        if (query.fieldId == "tipoAutorizacion") {

            var tipoAuthSelected = nexus.getForm("COF110_form_1").getFieldValue("tipoAutorizacion");

            setMostrarOAuthFields(tipoAuthSelected == TP_AUTH_OAUTH2);

            if (!mostrarOAuthFields) {

                var form = nexus.getForm("COF110_form_1");

                form.setFieldValue("authServer", "");

                form.setFieldValue("scope", "");
            }

        }
    }

    const onBeforeSubmit = (context, form, data, nexus) => {
        if (props.isUpdate) {
            data.parameters.push({ id: "isUpdate", value: "S" });
            data.parameters.push({ id: "nroIntegracion", value: props.nroIntegracion });
        }
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "ERROR")
            return;

        nexus.getGrid("COF110_grid_1").refresh();

        props.onHide();
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Form
                id="COF110_form_1"
                application="COF110CreateOrUpdate"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={onBeforeInitialize}
                onAfterInitialize={onAfterInitialize}
                onBeforeValidateField={onBeforeValidateField}
                onAfterValidateField={handleFormAfterValidateField}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{props.isUpdate ? t("COF110_Sec0_Title_Update") : t("COF110_Sec0_Title_Create")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>

                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="codigo">{t("COF110_frm1_lbl_codigo")}</label>
                                <Field name="codigo" />
                                <StatusMessage for="codigo" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="isEnabled">{t("COF110_frm1_lbl_isEnabled")}</label>
                                <FieldCheckbox name="isEnabled" />
                                <StatusMessage for="isEnabled" />
                            </div>
                        </Col>
                    </Row>

                    <Row>
                        <Col md={8}>
                            <div className="form-group">
                                <label htmlFor="descripcion">{t("COF110_frm1_lbl_descripcion")}</label>
                                <Field name="descripcion" />
                                <StatusMessage for="descripcion" />
                            </div>
                        </Col>
                    </Row>

                    <Row>
                        <Col md={8}>
                            <div className="form-group">
                                <label htmlFor="urlIntegracion">{t("COF110_frm1_lbl_urlIntegracion")}</label>
                                <Field name="urlIntegracion" />
                                <StatusMessage for="urlIntegracion" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="tipoComunicacion">{t("COF110_frm1_lbl_tipoComunicacion")}</label>
                                <FieldSelect name="tipoComunicacion" />
                                <StatusMessage for="tipoComunicacion" />
                            </div>
                        </Col>
                    </Row>

                    <Row>
                        <Col md={3}>
                            <div className="form-group">
                                <label htmlFor="tipoAutorizacion">{t("COF110_frm1_lbl_tipoAutorizacion")}</label>
                                <FieldSelect name="tipoAutorizacion" />
                                <StatusMessage for="tipoAutorizacion" />
                            </div>
                        </Col>
                        <Col style={{ display: mostrarOAuthFields ? 'block' : 'none' }}>
                            <div className="form-group">
                                <label htmlFor="authServer">{t("COF110_frm1_lbl_authServer")}</label>
                                <Field name="authServer" />
                                <StatusMessage for="authServer" />
                            </div>
                        </Col>
                        <Col style={{ display: mostrarOAuthFields ? 'block' : 'none' }}>
                            <div className="form-group">
                                <label htmlFor="scope">{t("COF110_frm1_lbl_scope")}</label>
                                <Field name="scope" />
                                <StatusMessage for="scope" />
                            </div>
                        </Col>
                    </Row>

                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="user">{t("COF110_frm1_lbl_user")}</label>
                                <Field name="user" />
                                <StatusMessage for="user" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="password">{t("COF110_frm1_lbl_password")}</label>
                                <Field type="password" name="password" id="passwordField" />
                                <StatusMessage for="password" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="isUpdatePassword">{t("COF110_frm1_lbl_isUpdatePassword")}</label>
                                <FieldCheckbox name="isUpdatePassword" disabled={!props.isUpdate} />
                                <StatusMessage for="isUpdatePassword" />
                            </div>
                        </Col>
                    </Row>

                </Modal.Body>
                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleClose}>
                        {t("General_Sec0_btn_Cerrar")}
                    </Button>
                    <SubmitButton id="btnSubmit" variant="primary" label="General_Sec0_btn_Confirmar" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}