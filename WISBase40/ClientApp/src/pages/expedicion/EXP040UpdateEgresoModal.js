import React, { useState } from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldSelect, FieldSelectAsync, FieldToggle, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';

export function EXP040UpdateEgresoModal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });
    const [warnRutaAsociada, setWarningRutaAsociada] = useState(false);

    const initialValues = {
        descripcion: "",
        matricula: "",
        predio: "",
        vehiculo: "",
        codigoRuta: "",
        codigoEmpresa: "",
        codigoTransportista: "",
        codigoPuerta: "",
        respetaOrdenCarga: "N",
        habilitarTracking: "N",
        habilitarRuteo: "N",
        controlContenedores: "N",

    };

    const validationSchema = {
        descripcion: Yup.string().required().max(50),
        matricula: Yup.string(),
        predio: Yup.string().required().max(15),
        vehiculo: Yup.string(),
        codigoRuta: Yup.string().max(3),
        codigoEmpresa: Yup.string().max(10),
        codigoTransportista: Yup.string().max(10),
        codigoPuerta: Yup.string().max(3),
        codigoVehiculo: Yup.string(),
        respetaOrdenCarga: Yup.boolean(),
        habilitarTracking: Yup.boolean(),
        habilitarRuteo: Yup.boolean(),
        controlContenedores: Yup.boolean(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        if (warnRutaAsociada)
            setWarningRutaAsociada(false);

        query.parameters = [{ id: "camion", value: props.camion }];
    }

    const handleFormAfterValidateField = (context, form, query, nexus) => {
        if (query.fieldId === "codigoRuta" || query.fieldId === "predio") {
            const parameter = query.parameters.find(d => d.id === "rutaYaAsociada" && d.value === "S");

            if (parameter) {
                setWarningRutaAsociada(true);
            } else {
                setWarningRutaAsociada(false);
            }
        }
    };

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "camion", value: props.camion },
            { id: "isSubmit", value: true }
        ];
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("EXP040_grid_1").refresh();
            props.onHide();
        }
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                id="EXP040_form_UpdateEgreso"
                application="EXP040UpdateEgreso"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={handleFormBeforeInitialize}
                onBeforeSubmit={handleFormBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
                onAfterValidateField={handleFormAfterValidateField}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("EXP040_Sec0_modalTitle_Update")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col>
                            <h3>Camion: {props.camion}</h3>
                            <h5>{props.descripcion}</h5>
                        </Col>
                    </Row>
                    <hr />
                    <Row>
                        <Col md={6}>
                            <div className="form-group">
                                <label htmlFor="descripcion">{t("EXP040_frm1_lbl_descripcion")}</label>
                                <Field name="descripcion" />
                                <StatusMessage for="descripcion" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="predio">{t("EXP040_frm1_lbl_predio")}</label>
                                <FieldSelect name="predio" />
                                <StatusMessage for="predio" />
                            </div>
                        </Col>
                        <Col md={6}>
                            <div className="form-group">
                                <label htmlFor="codigoEmpresa">{t("EXP040_frm1_lbl_empresa")}</label>
                                <FieldSelectAsync name="codigoEmpresa" isClearable={true} />
                                <StatusMessage for="codigoEmpresa" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="codigoPuerta">{t("EXP040_frm1_lbl_puerta")}</label>
                                <FieldSelectAsync name="codigoPuerta" isClearable={true} />
                                <StatusMessage for="codigoPuerta" />
                            </div>
                        </Col>
                    </Row>
                    <hr />
                    <Row>
                        <Col md={6}>
                            <div className="form-group">
                                <label htmlFor="codigoVehiculo">{t("EXP040_frm1_lbl_vehiculo")}</label>
                                <FieldSelectAsync name="codigoVehiculo" />
                                <StatusMessage for="codigoVehiculo" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="codigoTransportista">{t("EXP040_frm1_lbl_transportista")}</label>
                                <FieldSelectAsync name="codigoTransportista" isClearable={true} />
                                <StatusMessage for="codigoTransportista" />
                            </div>
                        </Col>
                        <Col md={6}>
                            <div className="form-group">
                                <label htmlFor="matricula">{t("EXP040_frm1_lbl_matricula")}</label>
                                <Field name="matricula" />
                                <StatusMessage for="matricula" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="codigoRuta">{t("EXP040_frm1_lbl_ruta")}</label>
                                <FieldSelectAsync name="codigoRuta" isClearable={true} />
                                <StatusMessage for="codigoRuta" />
                            </div>
                        </Col>
                    </Row>
                    <hr />
                    <Row>
                        <Col md={6}>
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <FieldToggle name="respetaOrdenCarga" label={t("EXP040_frm1_lbl_respetaOrdenCarga")} />
                                        <StatusMessage for="respetaOrdenCarga" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <FieldToggle name="habilitarTracking" label={t("EXP040_frm1_lbl_habilitarTracking")} />
                                        <StatusMessage for="habilitarTracking" />
                                    </div>
                                </Col>
                            </Row>
                        </Col>
                        <Col md={6}>
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <FieldToggle name="habilitarRuteo" label={t("EXP040_frm1_lbl_habilitarRuteo")} />
                                        <StatusMessage for="habilitarRuteo" />
                                    </div>
                                </Col>
                                <Col >
                                    <div className="form-group">
                                        <FieldToggle name="controlContenedores" label={t("EXP040_frm1_lbl_controlContenedores")} />
                                        <StatusMessage for="controlContenedores" />
                                    </div>
                                </Col>
                            </Row>
                        </Col>

                    </Row>

                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                        {t("EXP040_frm1_btn_cancelar")}
                    </Button>
                    <SubmitButton id="btnSubmitCreateEgreso" variant="primary" label="EXP040_frm1_btn_editar" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}