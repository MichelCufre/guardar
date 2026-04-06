import React, { useState } from 'react';
import { Button, Col, Container, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldCheckbox, FieldDate, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { FieldTime } from '../../components/FormComponents/FormFieldTime';
import { withPageContext } from '../../components/WithPageContext';

function InternalFAC001Modal(props) {

    const { t } = useTranslation();

    const [showHoraHasta, setShowHoraHasta] = useState(false);
    const [showFechaDesde, setShowFechaDesde] = useState(false);

    const [editMode, setEditMode] = useState(false);

    const validationSchema = {
        nombre: Yup.string().required(),
        ingresarFechaDesde: Yup.string(),
        fechaDesde: Yup.string(),
        fechaHasta: Yup.string().required(),
        fechaProgramacion: Yup.string(),
        parcial: Yup.string(),
        horaDesde: Yup.string(),
        horaHasta: Yup.string(),
        horaFechaProgramacion: Yup.string(),
    };

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {

        if (props.ejecucion) {

            let parameters =
                [
                    { id: "nuEjecucion", value: props.ejecucion.find(x => x.id === "nuEjecucion").value },
                ];

            query.parameters = parameters;
            setEditMode(true);
        } else {
            setEditMode(false);
        }
    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        if (props.ejecucion) {

            let parameters =
                [
                    { id: "nuEjecucion", value: props.ejecucion.find(x => x.id === "nuEjecucion").value },
                ];

            query.parameters = parameters;
        }
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            nexus.getForm("FAC001_form_1").reset();
            props.onHide(props.nexus);
        }
    }

    const onAfterValidateField = (context, form, query, nexus) => {

        if (query.fieldId === "parcial") {
            setShowHoraHasta(!showHoraHasta);
        }

        if (query.fieldId === "ingresarFechaDesde") {
            setShowFechaDesde(!showFechaDesde);
        }
    };

    const onAfterInitialize = (context, form, query, nexus) => {
        const showParcial = query.parameters.find(p => p.id === "showParcial");
        const showFechaDesde = query.parameters.find(p => p.id === "showFechaDesde");

        if (showParcial && showParcial.value === "true")
            setShowHoraHasta(true);

        if (showFechaDesde && showFechaDesde.value === "true")
            setShowFechaDesde(true);
    };

    return (

        <Form
            application="FAC001"
            id="FAC001_form_1"
            application="FAC001"
            validationSchema={validationSchema}
            onBeforeInitialize={handleFormBeforeInitialize}
            onBeforeSubmit={onBeforeSubmit}
            onAfterInitialize={onAfterInitialize}
            onAfterSubmit={handleFormAfterSubmit}
            onAfterValidateField={onAfterValidateField}
        >
            <Modal.Header closeButton>
                <Modal.Title>{editMode ? t("FAC001_Sec0_mdlEdit_Titulo") : t("FAC001_Sec0_mdlCreate_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="nombre">{t("FAC001_frm1_lbl_nombre")}</label>
                                <Field name="nombre" />
                                <StatusMessage for="nombre" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="ingresarFechaDesde" />
                                <FieldCheckbox name="ingresarFechaDesde" label={t("FAC001_frm1_lbl_ingresarFechaDesde")} />
                                <StatusMessage for="ingresarFechaDesde" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="parcial" />
                                <FieldCheckbox name="parcial" label={t("FAC001_frm1_lbl_parcial")} />
                                <StatusMessage for="parcial" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" style={{ display: showFechaDesde ? 'block' : 'none' }}>
                                <label htmlFor="fechaDesde">{t("FAC001_frm1_lbl_fechaDesde")}</label>
                                <FieldDate name="fechaDesde" />
                                <StatusMessage for="fechaDesde" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" style={{ display: showFechaDesde && showHoraHasta ? 'block' : 'none' }}>
                                <label htmlFor="horaDesde">{t("FAC001_frm1_lbl_horaDesde")}</label>
                                <FieldTime name="horaDesde" />
                                <StatusMessage for="horaDesde" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="fechaHasta">{t("FAC001_frm1_lbl_fechaHasta")}</label>
                                <FieldDate name="fechaHasta" />
                                <StatusMessage for="fechaHasta" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" style={{ display: showHoraHasta ? 'block' : 'none' }}>
                                <label htmlFor="horaHasta">{t("FAC001_frm1_lbl_horaHasta")}</label>
                                <FieldTime name="horaHasta" />
                                <StatusMessage for="horaHasta" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="fechaProgramacion">{t("FAC001_frm1_lbl_fechaProgramacion")}</label>
                                <FieldDate name="fechaProgramacion" />
                                <StatusMessage for="fechaProgramacion" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" style={{ display: showHoraHasta ? 'block' : 'none' }}>
                                <label htmlFor="horaFechaProgramacion">{t("FAC001_frm1_lbl_horaFechaPrgrm")}</label>
                                <FieldTime name="horaFechaProgramacion" />
                                <StatusMessage for="horaFechaProgramacion" />
                            </div>
                        </Col>
                    </Row>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("FAC001_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmar" variant="primary" label="FAC001_frm1_btn_confirmar" />
            </Modal.Footer>
        </Form>
    );
}

export const FAC001Modal = withPageContext(InternalFAC001Modal);