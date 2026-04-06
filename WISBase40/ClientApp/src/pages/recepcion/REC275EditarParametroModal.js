import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldToggle, FieldSelectAsync, StatusMessage, SubmitButton, FieldSelect } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import * as Yup from 'yup';

function InternalREC275EditarParametroModal(props) {

    const { t } = useTranslation();

    const initialValues = {
        logica: "",
        codigoInstancia: "",
        descripcionProceso: "",
        codigoEstrategia: "",
        codigoParametro: "", 
        descripcionParametro: "",
        valorParametro: "", 
    };

    const validationSchema = {
        logica: Yup.string(),
        codigoInstancia: Yup.string(),
        descripcionProceso: Yup.string(),
        codigoEstrategia: Yup.string(),
        codigoParametro: Yup.string(),
        descripcionParametro: Yup.string(),
        valorParametro: Yup.string(),
    };

    const handleClose = () => {
        props.onHide(null, props.nexus);
    };

    const onAfterInitialize = (context, form, query, nexus) => {
        form.fields.find(f => f.id == "logica").value = props.logica;
        form.fields.find(f => f.id == "codigoInstancia").value = props.codigoInstancia;
        form.fields.find(f => f.id == "descripcionProceso").value = props.descripcionProceso;
        form.fields.find(f => f.id == "codigoEstrategia").value = props.codigoEstrategia;
    };

    const addParameters = (context, data, nexus) => {
        let parameters =
            [
                { id: "logica", value: props.logica },
                { id: "codigoParametro", value: props.codigoParametro },
                { id: "descripcionParametro", value: props.descripcionParametro },
                { id: "valorParametro", value: props.valorParametro },
                
            ];
        nexus.parameters = parameters;
    }

    const onBeforeSubmit = (context, form, query, nexus) => {
        context.abortServerCall = true;

        var valorParametro = form.fields.find(f => f.id == "valorParametro").value;
        var tipoParametro = getTipoParametro();

        props.onHide(valorParametro, nexus);
    }

    const getTipoParametro = () => {
        switch (props.codigoParametro) {
            case "GENERAR_PARCIAL":
            case "UBIC_BAJAS_ALTAS":
            case "UBIC_MULTIPRODUCTO":
            case "UBIC_MULTILOTE":
            case "RESPETA_FIFO":
            case "PRODUCTOS_COINCIDENTES":
            case "LOTES_COINCIDENTES":
            case "AREA":
            case "RESPETA_LOTE":
            case "RESPETA_VENCIMIENTO":
            case "IGNORAR_VENCIMIENTO_STOCK":
            case "MODALIDAD_REABASTECIMIENTO":
            case "TIPO_PICKING":
                return "BOOL";
            case "CONTROL_ACCESO":
            case "EMPRESA":
            case "FAMILIA":
            case "ROTATIVIDAD":
            case "TIPO_UBICACION":
            case "UBIC_INICIO":
            case "ZONA_UBICACION":
            case "PALLET":
                return "SELECT";
            default:
                return "TEXT";
        }
    }

    const renderFieldValorCampo = () => {
        switch (getTipoParametro()) {
            case "BOOL":
                return (<FieldSelect name="valorParametro" isClearable={true} />);
            case "SELECT":
                return (<FieldSelectAsync name="valorParametro" isClearable={true} />)
            default:
                return (<Field name="valorParametro" />);
        }
    }

    return (

        <Form
            application={props.application}
            id="REC275EditarParametro_form_1"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onAfterInitialize={onAfterInitialize}
            onBeforeInitialize={addParameters}
            onBeforeSubmit={onBeforeSubmit}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("REC275_Sec0_editParam_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="codigoEstrategia">{t("REC275_grid1_colname_CodigoEstrategia")}</label>
                                <Field name="codigoEstrategia" readOnly />
                                <StatusMessage for="codigoEstrategia" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="logica">{t("REC275_select_colname_Logica")}</label>
                                <Field name="logica" readOnly />
                                <StatusMessage for="logica" />
                            </div>
                        </Col>
                    </Row>

                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="codigoInstancia">{t("REC275_grid4_colname_CodigoInstancia")}</label>
                                <Field name="codigoInstancia" readOnly />
                                <StatusMessage for="codigoInstancia" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="descripcionProceso">{t("REC275_Modal_Campo_NombreLogica")}</label>
                                <Field name="descripcionProceso" readOnly />
                                <StatusMessage for="descripcionProceso" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="codigoParametro">{t("REC275_Modal_Campo_CodigoParametro")}</label>
                                <Field name="codigoParametro" readOnly />
                                <StatusMessage for="codigoParametro" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="descripcionParametro">{t("REC275_Modal_Campo_DescripcionParametro")}</label>
                                <Field name="descripcionParametro" readOnly />
                                <StatusMessage for="descripcionParametro" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="valorParametro">{t("REC275_Modal_Campo_ValorParametro")}</label>
                                {renderFieldValorCampo()}
                                <StatusMessage for="valorParametro" />
                            </div>
                        </Col>
                    </Row>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("REC275_frm1_btn_Cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmarParametro" variant="primary" label="REC275_frm1_btn_Confirmar" />
            </Modal.Footer>
        </Form>
    );
}

export const REC275EditarParametroModal = withPageContext(InternalREC275EditarParametroModal);