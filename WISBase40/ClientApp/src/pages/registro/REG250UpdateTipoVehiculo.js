import React, { useState } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs, Card } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldDate, FieldSelect, FieldSelectAsync, FieldTextArea, FormButton, SubmitButton, StatusMessage, FieldRangePercentage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { FieldCheckbox } from '../../components/FormComponents/Form';
import { FormWarningMessage } from '../../components/FormComponents/FormWarningMessage';


export function REG250UpdateTipoVehiculoModal(props) {
    const { t } = useTranslation();

    const initialValues = {
        descripcion: "",
        volumen: "",
        peso: "",
        pallets: "",
        porcentajeOcupacionVolumen: "",
        porcentajeOcupacionPeso: "",
        porcentajeOcupacionPallet: "",
        refrigerado: "",
        cargaLateral: "",
        admiteZorra: "",
        soloCabina: ""
    };

    const validationSchema = {
        descripcion: Yup.string().required().max(100),
        volumen: Yup.string(),
        peso: Yup.string(),
        pallets: Yup.string(),
        porcentajeOcupacionVolumen: Yup.string(),
        porcentajeOcupacionPeso: Yup.string(),
        porcentajeOcupacionPallet: Yup.string(),
        refrigerado: Yup.string(),
        cargaLateral: Yup.string(),
        admiteZorra: Yup.string(),
        soloCabina: Yup.string()
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [{ id: "idTipoVehiculo", value: props.tipoVehiculo }];
    }

    const handleFormBeforeValidateField = (context, form, query, nexus) => {
        query.parameters = [{ id: "idTipoVehiculo", value: props.tipoVehiculo }];
    };

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [{ id: "idTipoVehiculo", value: props.tipoVehiculo }];
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("REG250_grid_1").refresh();
            props.onHide();
        }
    };

    const marks = [
        {
            value: 0,
            label: '0%',
        },
        {
            value: 25,
            label: '25%',
        },
        {
            value: 50,
            label: '50%',
        },
        {
            value: 75,
            label: '75%',
        },
        {
            value: 100,
            label: '100%',
        },
    ];

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                application="REG250UpdateTipoVehiculo"
                id="REG250_form_UpdateTipoVehiculo"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={handleFormBeforeInitialize}
                onBeforeSubmit={handleFormBeforeSubmit}
                onBeforeValidateField={handleFormBeforeValidateField}
                onAfterSubmit={handleFormAfterSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("REG250_Sec0_modalTitle_TituloModificar")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="descripcion">{t("REG250_frm_lbl_Descripcion")}</label>
                                <Field name="descripcion" />
                                <StatusMessage for="descripcion" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <Card bg="light" className="mb-3">
                                <Card.Body>
                                    <div className="form-group">
                                        <label htmlFor="volumen">{t("REG250_frm_lbl_Volumen")}</label>
                                        <Field name="volumen" />
                                        <StatusMessage for="volumen" />
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="porcentajeOcupacionVolumen">{t("REG250_frm_lbl_PorcentajeOcupacionVolumen")}</label>
                                        <FieldRangePercentage name="porcentajeOcupacionVolumen" marks={marks} showInput />
                                        <StatusMessage for="porcentajeOcupacionVolumen" />
                                    </div>
                                </Card.Body>
                            </Card>
                            <Card bg="light" className="mb-3">
                                <Card.Body>
                                    <div className="form-group">
                                        <label htmlFor="peso">{t("REG250_frm_lbl_Peso")}</label>
                                        <Field name="peso" />
                                        <StatusMessage for="peso" />
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="porcentajeOcupacionPeso">{t("REG250_frm_lbl_PorcentajeOcupacionPeso")}</label>
                                        <FieldRangePercentage name="porcentajeOcupacionPeso" marks={marks} showInput />
                                        <StatusMessage for="porcentajeOcupacionPeso" />
                                    </div>
                                </Card.Body>
                            </Card>
                            <Card bg="light" className="mb-3">
                                <Card.Body>
                                    <div className="form-group">
                                        <label htmlFor="pallets">{t("REG250_frm_lbl_Pallets")}</label>
                                        <Field name="pallets" />
                                        <StatusMessage for="pallets" />
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="porcentajeOcupacionPallet">{t("REG250_frm_lbl_PorcentajeOcupacionPallet")}</label>
                                        <FieldRangePercentage name="porcentajeOcupacionPallet" marks={marks} showInput />
                                        <StatusMessage for="porcentajeOcupacionPallet" />
                                    </div>
                                </Card.Body>
                            </Card>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <FieldCheckbox label={t("REG250_frm_lbl_Refrigerado")} name="refrigerado" />
                                <StatusMessage for="refrigerado" />
                            </div>
                            <div className="form-group">
                                <FieldCheckbox label={t("REG250_frm_lbl_CargaLateral")} name="cargaLateral" />
                                <StatusMessage for="cargaLateral" />
                            </div>
                            <div className="form-group">
                                <FieldCheckbox label={t("REG250_frm_lbl_AdmiteZorra")} name="admiteZorra" />
                                <StatusMessage for="admiteZorra" />
                            </div>
                            <div className="form-group">
                                <FieldCheckbox label={t("REG250_frm_lbl_SoloCabina")} name="soloCabina" />
                                <StatusMessage for="soloCabina" />
                            </div>
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                        {t("REG250_frm_btn_Cancelar")}
                    </Button>
                    <SubmitButton id="btnSubmitCreateTipoVehiculo" variant="primary" label="REG250_frm_btn_Actualizar" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}