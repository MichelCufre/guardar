import React, { useState } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { notificationType } from '../../components/Enums';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';

function InternalPRD190CreateLineaModal(props) {
    const { t } = useTranslation();

    const initialValues = {
        descripcion: "",
        tipoLinea: "",
        predio: ""
    };

    const validationSchema = {
        descripcion: Yup.string(),
        tipoLinea: Yup.string(),
        predio: Yup.string()
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleSubmit = () => {
        props.nexus.getForm("PRD190_form_1").submit();
    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        console.log("entra submit");
    }

    const handleGridBeforeSelectSearch = (context, grid, query, nexus) => {
    }

    const handleGridBeforeValidate = (context, data, nexus) => {
    }
    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("PRD190_grid_1").refresh();
            props.onHide();
        }
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRD190_Sec0_modalTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Row>
                    <Col>
                        <Form
                            id="PRD190_form_1"
                            initialValues={initialValues}
                            validationSchema={validationSchema}
                            onBeforeSubmit={handleFormBeforeSubmit}
                            onAfterSubmit={handleFormAfterSubmit}
                        >
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="descripcion">{t("PRD190_frm1_lbl_DS_PRDC_LINEA")}</label>
                                        <FieldTextArea name="descripcion" />
                                        <StatusMessage for="descripcion" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="tipoLinea">{t("PRD190_frm1_lbl_CD_TIPO_LINEA")}</label>
                                        <FieldSelect name="tipoLinea" />
                                        <StatusMessage for="tipoLinea" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="predio">{t("PRD190_frm1_lbl_NU_PREDIO")}</label>
                                        <FieldSelect name="predio" />
                                        <StatusMessage for="predio" />
                                    </div>
                                </Col>
                            </Row>
                        </Form>
                    </Col>
                </Row>


            </Modal.Body>
            <Modal.Footer>
                <Button variant="danger" onClick={handleClose}>
                    {t("PRD190_frm1_btn_CANCELAR")}
                </Button>
                <Button variant="primary" onClick={handleSubmit}>
                    {t("PRD190_frm1_btn_CREAR")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const PRD190CreateLineaModal = withPageContext(InternalPRD190CreateLineaModal);