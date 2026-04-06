import React, { useState } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { useCustomTranslation } from '../../components/TranslationHook';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';
import $ from 'jquery';
import './REG100.css';

function InternalREG100CreateEmpresaModal(props) {
    const { t } = useCustomTranslation();

    const [mostrarCamposWebhook, setMostrarCamposWebhook] = useState(false);
    const [errorClassName, setErrorClassName] = useState("");

    const initialValues = {

        codigoEmpresa: "",
        nombreEmpresa: "",
        codigoAlmacenaje: "",
        minimoStock: "",
        tipoFiscal: "",
        numeroFiscal: "",
        telefono: "",
        direccion: "",
        pais: "",
        paisSubdivision: "",
        localidad: "",
        codigoPostal: "",
        anexo1: "",
        anexo2: "",
        anexo3: "",
        anexo4: "",
        tipoNotificacion: "",
        payloadUrl: "",
        secret: "",
        resecret: ""
    };

    const validationSchema = {

        codigoEmpresa: Yup.string().required(),
        nombreEmpresa: Yup.string().required(),
        codigoAlmacenaje: Yup.string().required(),
        minimoStock: Yup.string(),
        tipoFiscal: Yup.string(),
        numeroFiscal: Yup.string(),
        telefono: Yup.string(),
        direccion: Yup.string(),
        pais: Yup.string(),
        paisSubdivision: Yup.string(),
        localidad: Yup.string(),
        codigoPostal: Yup.string(),
        anexo1: Yup.string(),
        anexo2: Yup.string(),
        anexo3: Yup.string(),
        anexo4: Yup.string(),
        tipoNotificacion: Yup.string(),
        payloadUrl: Yup.string(),
        secret: Yup.string(),
        resecret: Yup.string()
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormBeforeSubmit = (context, form, query, nexus) => {

        query.parameters = [
            { id: "isSubmit", value: true }
        ];

    }
    const handleFormBeforeValidateField = (context, form, query, nexus) => {
        query.parameters.push(
            { id: "mostrarCamposWebhook", value: mostrarCamposWebhook ? "true" : "false" }
        );
    }

    const handleFormAfterValidateField = (context, form, query, nexus) => {
        if (query.parameters.find(x => x.id === "mostrarCamposWebhook") != null) {
            setMostrarCamposWebhook(query.parameters.find(x => x.id === "mostrarCamposWebhook").value == "true");
        }
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("REG100_grid_1").refresh();
            props.onHide();
        }
    }

    const handleFormBeforeInitialize = (context, form, query, nexus) => {

    }

    const testWebhook = () => {
        const payloadUrl = $("[name='payloadUrl']").val();
        const secret = $("[name='secret']").val();
        const request = {
            method: 'POST',
            cache: "no-cache",
            headers: {
                'Content-Type': 'application/json',
                "X-Requested-With": "XMLHttpRequest"
            },
            body: JSON.stringify({
                application: "REG100",
                payloadUrl: payloadUrl,
                secret: secret,
            })
        };

        setErrorClassName("");

        fetch("/api/Webhook/Test", request)
            .then(response => {
                if (response.status === 401) {
                    window.location = "/api/Security/Logout";
                    return null;
                }

                if (response.status !== 200) {
                    throw new Error(response.status);
                }

                return response;
            })
            .then(response => response ? response.json() : response)
            .then(response => {
                if (response.Status === "ERROR")
                    throw new Error(response.Message);

                $(".webhook-response").text(response.response);
            })
            .catch(err => {
                setErrorClassName("error");
                $(".webhook-response").text(err);
            });
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Form
                id="REG100Create_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeSubmit={handleFormBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeInitialize={handleFormBeforeInitialize}
                onBeforeValidateField={handleFormBeforeValidateField}
                onAfterValidateField={handleFormAfterValidateField}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("REG100_Sec0_mdlCreate_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col>
                            <Row>

                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="codigoEmpresa">{t("REG100_frm1_lbl_codigo")}</label>
                                        <Field name="codigoEmpresa" maxLength="10" />
                                        <StatusMessage for="codigoEmpresa" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="nombreEmpresa">{t("REG100_frm1_lbl_nombre")}</label>
                                        <Field name="nombreEmpresa" maxLength="55" />
                                        <StatusMessage for="nombreEmpresa" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="codigoAlmacenaje">{t("REG100_frm1_lbl_codigoAlmacenaje")}</label>
                                        <FieldSelect name="codigoAlmacenaje" />
                                        <StatusMessage for="codigoAlmacenaje" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="minimoStock">{t("REG100_frm1_lbl_minimoStock")}</label>
                                        <Field name="minimoStock" maxLength="21" />
                                        <StatusMessage for="minimoStock" />
                                    </div>
                                </Col>
                            </Row>
                            <Tabs defaultActiveKey="detail" transition={false} id="noanim-tab-example">
                                <Tab eventKey="detail" title={t("REG100_frm1_tab_detalles")}>
                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="tipoFiscal">{t("REG100_frm1_lbl_tipoFiscal")}</label>
                                                <FieldSelect name="tipoFiscal" />
                                                <StatusMessage for="tipoFiscal" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="numeroFiscal">{t("REG100_frm1_lbl_numeroFiscal")}</label>
                                                <Field name="numeroFiscal" maxLength="30" />
                                                <StatusMessage for="numeroFiscal" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="telefono">{t("REG100_frm1_lbl_telefono")}</label>
                                                <Field name="telefono" maxLength="30" />
                                                <StatusMessage for="telefono" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="direccion">{t("REG100_frm1_lbl_direccion")}</label>
                                                <Field name="direccion" maxLength="100" />
                                                <StatusMessage for="direccion" />
                                            </div>
                                        </Col>
                                    </Row>

                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="pais">{t("REG100_frm1_lbl_Pais")}</label>
                                                <FieldSelectAsync name="pais" />
                                                <StatusMessage for="pais" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="paisSubdivision">{t("REG100_frm1_lbl_PaisSubdivision")}</label>
                                                <FieldSelectAsync name="paisSubdivision" />
                                                <StatusMessage for="paisSubdivision" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="localidad">{t("REG100_frm1_lbl_PaisSubdivisionLocalidad")}</label>
                                                <FieldSelectAsync name="localidad" />
                                                <StatusMessage for="localidad" />
                                            </div>
                                        </Col>

                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="codigoPostal">{t("REG100_frm1_lbl_codigoPostal")}</label>
                                                <Field name="codigoPostal" maxLength="15" />
                                                <StatusMessage for="codigoPostal" />
                                            </div>
                                        </Col>
                                    </Row>
                                </Tab>
                                <Tab eventKey="anexos" title={t("REG100_frm1_tab_anexos")}>
                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="anexo1">{t("REG100_frm1_lbl_anexo1")}</label>
                                                <FieldTextArea name="anexo1" maxLength="200" />
                                                <StatusMessage for="anexo1" />
                                            </div>
                                            <div className="form-group" >
                                                <label htmlFor="anexo3">{t("REG100_frm1_lbl_anexo3")}</label>
                                                <FieldTextArea name="anexo3" maxLength="200" />
                                                <StatusMessage for="anexo3" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="anexo2">{t("REG100_frm1_lbl_anexo2")}</label>
                                                <FieldTextArea name="anexo2" maxLength="200" />
                                                <StatusMessage for="anexo2" />
                                            </div>
                                            <div className="form-group" >
                                                <label htmlFor="anexo4">{t("REG100_frm1_lbl_anexo4")}</label>
                                                <FieldTextArea name="anexo4" maxLength="200" />
                                                <StatusMessage for="anexo4" />
                                            </div>
                                        </Col>

                                    </Row>
                                </Tab>
                                <Tab eventKey="notifications" title={t("REG100_frm1_tab_notifications")}>
                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="tipoNotificacion">{t("REG100_frm1_lbl_tipoNotificacion")}</label>
                                                <FieldSelect name="tipoNotificacion" />
                                                <StatusMessage for="tipoNotificacion" />
                                            </div>
                                        </Col>
                                    </Row>
                                    <Row style={{ display: mostrarCamposWebhook ? 'block' : 'none' }}>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="payloadUrl">{t("REG100_frm1_lbl_payloadUrl")}</label>
                                                <Field name="payloadUrl" />
                                                <StatusMessage for="payloadUrl" />
                                            </div>
                                            <div className="form-group" >
                                                <label htmlFor="secret">{t("REG100_frm1_lbl_secret")}</label>
                                                <Field type="password" name="secret" />
                                                <StatusMessage for="secret" />
                                            </div>
                                            <div className="form-group" >
                                                <label htmlFor="resecret">{t("REG100_frm1_lbl_resecret")}</label>
                                                <Field type="password" name="resecret" />
                                                <StatusMessage for="resecret" />
                                            </div>
                                            <div className="form-group">
                                                <Button variant="btn btn-outline-secondary" onClick={testWebhook}> {t("REG100_frm1_btn_WebhookTest")} </Button>
                                                <label className={"webhook-response " + errorClassName}></label>
                                            </div>
                                        </Col>
                                    </Row>
                                </Tab>
                            </Tabs>
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("REG100_frm1_btn_CANCELAR")} </Button>
                    <SubmitButton id="btnSubmitCreateEmpresa" variant="primary" label="REG100_frm1_btn_CREAR" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}

export const REG100CreateEmpresaModal = withPageContext(InternalREG100CreateEmpresaModal);