import React, { useState } from 'react';
import { Modal, Button, Row, Col } from 'react-bootstrap';
import { Form, Field, SubmitButton, FieldSelect, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';

export function REG300CreateGrupoModal(props) {
    const { t } = useTranslation();

    const initialValues = {
        codigoGrupo: "",
        descripcion: "",
        clase: "",
    };

    const validationSchema = {
        codigoGrupo: Yup.string().required(),
        descripcion: Yup.string().required(),
        clase: Yup.string().required(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("REG300_grid_1").refresh();
            props.onHide();
        }
    };

    return (
        <Form
            application="REG300CreateGrupo"
            id="REG300_form_CreateGrupo"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onAfterSubmit={handleFormAfterSubmit}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("REG300_Sec0_Title_CrearGrupo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Row>
                    <Col>
                        <div className="form-group">
                            <label htmlFor="codigoGrupo">{t("REG300_frm_lbl_CodigoGrupo")}</label>
                            <Field name="codigoGrupo" />
                            <StatusMessage for="codigoGrupo" />
                        </div>
                        <div className="form-group">
                            <label htmlFor="descripcion">{t("REG300_frm_lbl_Descripcion")}</label>
                            <Field name="descripcion" />
                            <StatusMessage for="descripcion" />
                        </div>
                        <div className="form-group">
                            <label htmlFor="clase">{t("REG300_frm_lbl_Clase")}</label>
                            <FieldSelect name="clase" />
                            <StatusMessage for="clase" />
                        </div>
                    </Col>
                </Row>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                    {t("REG300_frm_btn_Cancelar")}
                </Button>
                <SubmitButton id="btnSubmitCreateGrupo" variant="primary" label="REG300_frm_btn_Crear" />
            </Modal.Footer>
        </Form>

    );
}