import React, { useState } from 'react';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldDate, FieldCheckbox, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container, Div } from 'react-bootstrap';
import * as Yup from 'yup';

export default function COF100(props) {

    const { t } = useTranslation();

    const validationSchema = {

        estado: Yup.string().required(),
        idTenant: Yup.string().required(),
        apiTracking: Yup.string().required(),
        apiWMS: Yup.string().required(),
        apiUsers: Yup.string().required(),
        accessTokenURL: Yup.string().required(),
        grantType: Yup.string().required(),
        clientId: Yup.string().required(),
        clientSecret: Yup.string().required(),
        scope: Yup.string().required(),
        tpCont: Yup.string().required(),
        tpContFicticio: Yup.string().required(),
        cantDias: Yup.string().required(),
        agrupacionCD: Yup.string().required(),
        fechaInicial: Yup.string().required(),
    };

    const onAfterSubmit = (context, form, query, nexus) => {

        if (query.buttonId !== "btnGuardarParametros") {
            window.location.reload();
        }
    };

    return (

        <Page
            application="COF100"
            title={t("COF100_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                application="COF100"
                id="COF100_form_1"
                validationSchema={validationSchema}
                onAfterSubmit={onAfterSubmit}
            >
                <Container fluid className="d-flex justify-content-center" style={{ width: "50%" }}>
                    <Row>
                        <Col>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="estado">{t("COF100_frm1_lbl_estado")}</label>
                                        <Field name="estado" />
                                        <StatusMessage for="estado" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="scope">{t("COF100_frm1_lbl_scope")}</label>
                                        <Field name="scope" />
                                        <StatusMessage for="scope" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="idTenant">{t("COF100_frm1_lbl_idTenant")}</label>
                                        <Field name="idTenant" />
                                        <StatusMessage for="idTenant" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="grantType">{t("COF100_frm1_lbl_grantType")}</label>
                                        <Field name="grantType" />
                                        <StatusMessage for="grantType" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="clientId">{t("COF100_frm1_lbl_clientId")}</label>
                                        <Field name="clientId" />
                                        <StatusMessage for="clientId" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="clientSecret">{t("COF100_frm1_lbl_clientSecret")}</label>
                                        <Field name="clientSecret" />
                                        <StatusMessage for="clientSecret" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="apiTracking">{t("COF100_frm1_lbl_apiTracking")}</label>
                                        <Field name="apiTracking" />
                                        <StatusMessage for="apiTracking" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="apiWMS">{t("COF100_frm1_lbl_apiWMS")}</label>
                                        <Field name="apiWMS" />
                                        <StatusMessage for="apiWMS" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="accessTokenURL">{t("COF100_frm1_lbl_accessTokenURL")}</label>
                                        <Field name="accessTokenURL" />
                                        <StatusMessage for="accessTokenURL" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="apiUsers">{t("COF100_frm1_lbl_apiUsers")}</label>
                                        <Field name="apiUsers" />
                                        <StatusMessage for="apiUsers" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="tpCont">{t("COF100_frm1_lbl_tpCont")}</label>
                                        <Field name="tpCont" />
                                        <StatusMessage for="tpCont" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="tpContFicticio">{t("COF100_frm1_lbl_tpConFicticio")}</label>
                                        <Field name="tpContFicticio" />
                                        <StatusMessage for="tpContFicticio" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="agrupacionCD">{t("COF100_frm1_lbl_agrupacionCD")}</label>
                                        <Field name="agrupacionCD" />
                                        <StatusMessage for="agrupacionCD" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="cantDias">{t("COF100_frm1_lbl_cantDias")}</label>
                                        <Field name="cantDias" />
                                        <StatusMessage for="cantDias" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="fechaInicial">{t("COF100_frm1_lbl_fechaInicial")}</label>
                                        <FieldDate name="fechaInicial" />
                                        <StatusMessage for="fechaInicial" />
                                    </div>
                                </Col>
                                <Col>
                                </Col>
                            </Row>

                            <Row>
                                <Col>
                                    <SubmitButton id="btnDesactivarTracking" variant="primary" label="COF100_frm1_btn_DesactivarTracking" />
                                </Col>
                                <Col className="text-center">
                                    <SubmitButton id="btnGuardarParametros" variant="primary" label="COF100_frm1_btn_GuardarParametros" />
                                </Col>
                                <Col>
                                    <SubmitButton className="float-end" id="btnSincronizarDatos" variant="primary" label="COF100_frm1_btn_SincronizarDatos" />
                                </Col>
                            </Row>
                            <br />
                            <br />
                            <h6 className="form-title">{t("COF100_frm1_lbl_qtPedidos")}</h6>
                            <Row>
                                <Col className="col-6">
                                    <div className="form-group">
                                        <Field name="qtPedidos" />
                                        <StatusMessage for="qtPedidos" />
                                    </div>
                                </Col>
                                <Col className="col-6">
                                    <SubmitButton id="btnSincronizarPedidos" variant="primary" label="COF100_frm1_btn_SincronizarPedidos" />
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Container>
            </Form>
        </Page>
    );
}