import React, { useState } from 'react';
import { Modal, Button, Row, Col } from 'react-bootstrap';
import { Form, Field, FieldSelect, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';


export function REG010CreateEquipoModal(props) {
    const { t } = useTranslation();

    const initialValues = {
        descripcion: "",
        tipo: "",
        predio: ""
    };

    const validationSchema = {
        descripcion: Yup.string().required(),
        tipo: Yup.string().required(),
        predio: Yup.string().required()
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {

    }

    const handleFormAfterValidateField = (context, form, query, nexus) => {
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("REG010_grid_1").refresh();
            props.onHide();
        }
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                application="REG010CreateEquipo"
                id="REG010_form_CreateEquipo"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={handleFormBeforeInitialize}
                onAfterSubmit={handleFormAfterSubmit}
                onAfterValidateField={handleFormAfterValidateField}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("REG010_Sec0_modalTitle_TituloCrear")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="descripcion">{t("REG010_frm_lbl_Descripcion")}</label>
                                <Field name="descripcion" />
                                <StatusMessage for="descripcion" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="tipo">{t("REG010_frm_lbl_Tipo")}</label>
                                <FieldSelect name="tipo" />
                                <StatusMessage for="tipo" />
                            </div>
                            <div className="form-group" >
                                <label htmlFor="predio">{t("REG010_frm_lbl_Predio")}</label>
                                <FieldSelect name="predio" />
                                <StatusMessage for="predio" />
                            </div>                           
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                        {t("REG010_frm_btn_Cancelar")}
                    </Button>
                    <SubmitButton id="btnSubmitCreateEquipo" variant="primary" label="REG010_frm_btn_Crear" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}
