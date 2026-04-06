import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import * as Yup from 'yup';

function InternalCOF040Modal(props) {

    const { t } = useTranslation();

    const [editMode, setEditMode] = useState(false);

    const validationSchema = {

        codigo: Yup.string().required(),
        predio: Yup.string().required(),
        descripcion: Yup.string().required(),
        lenguaje: Yup.string().required(),
        servidor: Yup.string().required(),
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            nexus.getForm("COF040_form_1").reset();
            props.onHide(props.nexus);
        }
    }

    const handleFormBeforeInitialize = (context, form, query, nexus) => {

        if (props.impresora) {

            let parameters =
                [
                    { id: "idImpresora", value: props.impresora.find(x => x.id === "idImpresora").value },
                    { id: "predio", value: props.impresora.find(x => x.id === "predio").value }

                ];

            query.parameters = parameters;
            setEditMode(true);
        } else {
            setEditMode(false);
        }
    };

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    return (

        <Form
            application="COF040"
            id="COF040_form_1"
            application="COF040"
            validationSchema={validationSchema}
            onAfterSubmit={handleFormAfterSubmit}
            onBeforeInitialize={handleFormBeforeInitialize}
        >
            <Modal.Header closeButton>
                <Modal.Title>{editMode ? t("COF040_Sec0_mdlEdit_Titulo") : t("COF040_Sec0_mdlCreate_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="predio">{t("COF040_frm1_lbl_predio")}</label>
                                <FieldSelect name="predio" />
                                <StatusMessage for="predio" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="codigo">{t("COF040_frm1_lbl_codigo")}</label>
                                <Field name="codigo" />
                                <StatusMessage for="codigo" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="descripcion">{t("COF040_frm1_lbl_descripcion")}</label>
                                <Field name="descripcion" />
                                <StatusMessage for="descripcion" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="lenguaje">{t("COF040_frm1_lbl_lenguaje")}</label>
                                <FieldSelect name="lenguaje" />
                                <StatusMessage for="lenguaje" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="servidor">{t("COF040_frm1_lbl_servidor")}</label>
                                <FieldSelect name="servidor" />
                                <StatusMessage for="servidor" />
                            </div>
                        </Col>
                    </Row>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("COF040_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmar" variant="primary" label="COF040_frm1_btn_confirmar" />
            </Modal.Footer>
        </Form>
    );
}

export const COF040Modal = withPageContext(InternalCOF040Modal);