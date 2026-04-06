import React, { useState, useRef } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useCustomTranslation } from '../../components/TranslationHook';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldCheckbox } from '../../components/FormComponents/Form';
import { notificationType } from '../../components/Enums';
import * as Yup from 'yup';
import { Page } from '../../components/Page';
import { withPageContext } from '../../components/WithPageContext';

function InternalREG040AtributosMasivosModal(props) {

    const { t } = useCustomTranslation();

    const initialValues = {
        tpUbicacion: "",
        rotatividad: "",
        zona: "",
        controlAcceso: ""
    };

    const validationSchema = {

        tpUbicacion: Yup.string(),
        rotatividad: Yup.string(),
        zona: Yup.string(),
        controlAcceso: Yup.string().nullable(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const onBeforeMenuItemAction = (context, data, nexus) => {
        data.parameters = [
            { id: "tpUbicacion", value: nexus.getForm("REG040AtributosMasivos_form_1").getFieldValue("tpUbicacion") },
            { id: "rotatividad", value: nexus.getForm("REG040AtributosMasivos_form_1").getFieldValue("rotatividad") },
            { id: "zona", value: nexus.getForm("REG040AtributosMasivos_form_1").getFieldValue("zona") },
            { id: "controlAcceso", value: nexus.getForm("REG040AtributosMasivos_form_1").getFieldValue("controlAcceso") }
        ];
    };


    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("REG040AtributosMasivos_grid_1").refresh();
        nexus.getForm("REG040AtributosMasivos_form_1").reset();
    };

    return (
        <Modal dialogClassName="modal-50w" show={props.show} onHide={props.onHide} >
            <Modal.Header closeButton>
                <Modal.Title>{t("REG040_Sec0_lbl_AtributosMasivos_Title")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    load
                    {...props}
                >
                    <Row>
                        <Col>
                            <Form
                                application="REG040AtributosMasivos"
                                id="REG040AtributosMasivos_form_1"
                                initialValues={initialValues}
                                validationSchema={validationSchema}
                            >
                                <Row>
                                    <Col lg="3">
                                        <div className="form-group">
                                            <label style={{ fontWeight: "bold" }} htmlFor="tpUbicacion">{t("REG040AtributosMasivos_frm1_lbl_tpUbicacion")}</label>
                                            <FieldSelect name="tpUbicacion" />
                                            <StatusMessage for="tpUbicacion" />
                                        </div>
                                    </Col>
                                    <Col lg="3">
                                        <div className="form-group">
                                            <label style={{ fontWeight: "bold" }} htmlFor="rotatividad">{t("REG040AtributosMasivos_frm1_lbl_rotatividad")}</label>
                                            <FieldSelect name="rotatividad" />
                                            <StatusMessage for="rotatividad" />
                                        </div>
                                    </Col>
                                    <Col lg="3">
                                        <div className="form-group">
                                            <label style={{ fontWeight: "bold" }} htmlFor="zona">{t("REG040AtributosMasivos_frm1_lbl_zona")}</label>
                                            <FieldSelect name="zona" />
                                            <StatusMessage for="zona" />
                                        </div>
                                    </Col>
                                    <Col lg="3">
                                        <div className="form-group">
                                            <label style={{ fontWeight: "bold" }} htmlFor="controlAcceso">{t("REG040_frm1_lbl_controlAcceso")}</label>
                                            <FieldSelect name="controlAcceso" />
                                            <StatusMessage for="controlAcceso" />
                                        </div>
                                    </Col>
                                </Row>
                            </Form>
                        </Col>
                    </Row>

                    <br />
                    <Grid
                        application="REG040AtributosMasivos"
                        id="REG040AtributosMasivos_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableSelection
                        onBeforeMenuItemAction={onBeforeMenuItemAction}
                        onAfterMenuItemAction={onAfterMenuItemAction}
                    />
                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="outline-secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_Cerrar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const REG040AtributosMasivosModal = withPageContext(InternalREG040AtributosMasivosModal);