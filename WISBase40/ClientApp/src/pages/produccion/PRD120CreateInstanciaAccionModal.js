import React, { useState } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldSelect, FieldSelectAsync, StatusMessage, FieldCheckbox, SubmitButton } from '../../components/FormComponents/Form';
import { notificationType } from '../../components/Enums';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';

function InternalPRD120CreateInstanciaAccionModal(props) {
    const { t } = useTranslation();

    const initialValues = {
        cdAccion: "",
        dsAccion: "",
        dsInstancia: "",
        vlParametro1: "",
        vlParametro2: "",
        vlParametro3: "",
    };

    const validationSchema = {
        cdAccion: Yup.string(),
        dsInstancia: Yup.string(),
        vlParametro1: Yup.string(),
        vlParametro2: Yup.string(),
        vlParametro3: Yup.string(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleSubmit = () => {
        props.nexus.getForm("PRD120_form_1").submit();
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("PRD120_grid_1").refresh();
            props.onHide();
        }
    }


    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRD120_Sec0_modalTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Row>
                    <Col>
                        <Form
                            id="PRD120_form_1"
                            initialValues={initialValues}
                            validationSchema={validationSchema}
                            onAfterSubmit={handleFormAfterSubmit}
                        >
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="cdAccion">{t("PRD120_frm1_lbl_CD_PRDC_ACCION")}</label>
                                        <FieldSelectAsync name="cdAccion" />
                                        <StatusMessage for="cdAccion" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="dsAccion">{t("PRD120_frm1_lbl_DS_ACCION")}</label>
                                        <Field name="dsAccion" readOnly />
                                        <StatusMessage for="dsAccion" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="dsInstancia">{t("PRD120_frm1_lbl_DS_INSTANCIA_ACCION")}</label>
                                        <Field name="dsInstancia" />
                                        <StatusMessage for="dsInstancia" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="vlParametro1">{t("PRD120_frm1_lbl_PARAMETRO_1")}</label>
                                        <Field name="vlParametro1" />
                                        <StatusMessage for="vlParametro1" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="vlParametro2">{t("PRD120_frm1_lbl_PARAMETRO_2")}</label>
                                        <Field name="vlParametro2" />
                                        <StatusMessage for="vlParametro2" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="vlParametro3">{t("PRD120_frm1_lbl_PARAMETRO_3")}</label>
                                        <Field name="vlParametro3" />
                                        <StatusMessage for="vlParametro3" />
                                    </div>
                                </Col>
                            </Row>
                        </Form>
                    </Col>
                </Row>

            </Modal.Body>
            <Modal.Footer>
                <Button variant="danger" onClick={handleClose}>
                    {t("PRD120_frm1_btn_Cancelar")}
                </Button>
                <Button variant="primary" onClick={handleSubmit}>
                    {t("PRD120_frm1_btn_Confirmar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const PRD120CreateInstanciaAccionModal = withPageContext(InternalPRD120CreateInstanciaAccionModal);