import React, { useState } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldDate, FieldSelect, FieldSelectAsync, FieldTextArea, FormButton, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { FieldCheckbox } from '../../components/FormComponents/Form';
import { FormWarningMessage } from '../../components/FormComponents/FormWarningMessage';
import { FieldTime } from '../../components/FormComponents/FormFieldTime';


export function REG240CreateVehiculoModal(props) {
    const { t } = useTranslation();

    const initialValues = {
        descripcion: "",
        transportista: "",
        placa: "",
        tipo: "",
        predio: "",
        disponibilidadDesde: "",
        disponibilidadHasta: ""
    };

    const validationSchema = {
        descripcion: Yup.string().required(),
        transportista: Yup.string(),
        placa: Yup.string(),
        tipo: Yup.string(),
        predio: Yup.string(),
        disponibilidadDesde: Yup.string(),
        disponibilidadHasta: Yup.string()
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
            nexus.getGrid("REG240_grid_1").refresh();
            props.onHide();
        }
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                application="REG240CreateVehiculo"
                id="REG240_form_CreateVehiculo"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={handleFormBeforeInitialize}
                onAfterSubmit={handleFormAfterSubmit}
                onAfterValidateField={handleFormAfterValidateField}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("REG240_Sec0_modalTitle_TituloCrear")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="tipo">{t("REG240_frm_lbl_Tipo")}</label>
                                <FieldSelectAsync name="tipo" />
                                <StatusMessage for="tipo" />
                            </div>
                            <div className="form-group" >
                                <label htmlFor="predio">{t("REG240_frm_lbl_Predio")}</label>
                                <FieldSelect name="predio" />
                                <StatusMessage for="predio" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="descripcion">{t("REG240_frm_lbl_Descripcion")}</label>
                                <Field name="descripcion" />
                                <StatusMessage for="descripcion" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="placa">{t("REG240_frm_lbl_Placa")}</label>
                                <Field name="placa" />
                                <StatusMessage for="placa" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="transportista">{t("REG240_frm_lbl_Transportista")}</label>
                                <FieldSelectAsync name="transportista" />
                                <StatusMessage for="transportista" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="marca">{t("REG240_frm_lbl_Marca")}</label>
                                <Field name="marca" />
                                <StatusMessage for="marca" />
                            </div>
                            <Row>
                                <Col md={6}>
                                    <div className="form-group">
                                        <label htmlFor="disponibilidadDesde">{t("REG240_frm_lbl_DispDesde")}</label>
                                        <FieldTime name="disponibilidadDesde" />
                                        <StatusMessage for="disponibilidadDesde" />
                                    </div>
                                </Col>
                                <Col md={6}>
                                    <div className="form-group">
                                        <label htmlFor="disponibilidadHasta">{t("REG240_frm_lbl_DispHasta")}</label>
                                        <FieldTime name="disponibilidadHasta" />
                                        <StatusMessage for="disponibilidadHasta" />
                                    </div>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                        {t("REG240_frm_btn_Cancelar")}
                    </Button>
                    <SubmitButton id="btnSubmitCreateVehiculo" variant="primary" label="REG240_frm_btn_Crear" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}