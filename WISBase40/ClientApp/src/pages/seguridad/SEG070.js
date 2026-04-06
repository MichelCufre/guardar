import React, { useState } from 'react';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Form, Field, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container, Div } from 'react-bootstrap';
import * as Yup from 'yup';

export default function SEG070(props) {

    const { t } = useTranslation();

    const validationSchema = {

        passwordOld: Yup.string().required(),
        passwordNueva: Yup.string().required(),
        rePasswordNueva: Yup.string().required(),

    };
    const onAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            nexus.getForm("SEG070_form_1").reset();
        }
    }
    return (

        <Page
            title={t("SEG070_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                application="SEG070"
                id="SEG070_form_1"
                loadingOverlay={false}
                validationSchema={validationSchema}
                onAfterSubmit={onAfterSubmit}
            >
                <Container fluid class="d-flex flex-column" style={{ width: "30%" }}>
                    <div className="form-group" >
                        <label htmlFor="passwordOld">{t("SEG070_frm1_lbl_passwordOld")}</label>
                        <Field type="password" name="passwordOld" />
                        <StatusMessage for="passwordOld" />
                    </div>
                    <div className="form-group" >
                        <label htmlFor="passwordNueva">{t("SEG070_frm1_lbl_passwordNueva")}</label>
                        <Field type="password" name="passwordNueva" />
                        <StatusMessage for="passwordNueva" />
                    </div>
                    <div className="form-group" >
                        <label htmlFor="rePasswordNueva">{t("SEG070_frm1_lbl_rePasswordNueva")}</label>
                        <Field type="password" name="rePasswordNueva" />
                        <StatusMessage for="rePasswordNueva" />
                    </div>
                    <br></br>
                    <SubmitButton id="btnSubmitConfirmar" variant="primary" label="SEG070_frm1_btn_confirmar" />
                </Container>
            </Form>
        </Page>
    );
}