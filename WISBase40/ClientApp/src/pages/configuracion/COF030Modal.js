import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import * as Yup from 'yup';

function InternalCOF030Modal(props) {

    const { t } = useTranslation();

    const [editMode, setEditMode] = useState(false);

    const validationSchema = {

        estilo: Yup.string().required(),
        lenguaje: Yup.string().required(),
        cuerpo: Yup.string().nullable(),
        altura: Yup.string().nullable(),
        largura: Yup.string().nullable()
    };

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        if (props.template) {

            let parameters =
                [
                    { id: "idTemplate", value: props.template.find(x => x.id === "idTemplate").value },
                    { id: "lenguaje", value: props.template.find(x => x.id === "lenguaje").value },
                ];

            query.parameters = parameters;
            setEditMode(true);
        } else {
            setEditMode(false);
        }
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            nexus.getForm("COF030_form_1").reset();
            props.onHide(props.nexus);
        }
    }

    return (

        <Form
            application="COF030"
            id="COF030_form_1"
            application="COF030"
            validationSchema={validationSchema}
            onAfterSubmit={handleFormAfterSubmit}
            onBeforeInitialize={handleFormBeforeInitialize}
        >
            <Modal.Header closeButton>
                <Modal.Title>{editMode ? t("COF030_Sec0_mdlEdit_Titulo") : t("COF030_Sec0_mdlCreate_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="estilo">{t("COF030_frm1_lbl_estilo")}</label>
                                <FieldSelect name="estilo" />
                                <StatusMessage for="estilo" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="lenguaje">{t("COF030_frm1_lbl_lenguaje")}</label>
                                <FieldSelect name="lenguaje" />
                                <StatusMessage for="lenguaje" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="cuerpo">{t("COF030_frm1_lbl_cuerpo")}</label>
                                <FieldTextArea name="cuerpo" />
                                <StatusMessage for="cuerpo" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="altura">{t("COF030_frm1_lbl_altura")}</label>
                                <Field name="altura" />
                                <StatusMessage for="altura" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="largura">{t("COF030_frm1_lbl_largura")}</label>
                                <Field name="largura" />
                                <StatusMessage for="largura" />
                            </div>
                        </Col>
                    </Row>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("COF030_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmar" variant="primary" label="COF030_frm1_btn_confirmar" />
            </Modal.Footer>
        </Form>
    );
}

export const COF030Modal = withPageContext(InternalCOF030Modal);