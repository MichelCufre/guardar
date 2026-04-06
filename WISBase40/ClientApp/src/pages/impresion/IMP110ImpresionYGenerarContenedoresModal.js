import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime, FieldCheckbox } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import * as Yup from 'yup';

function InternalIMP110ImpresionYGenerarContenedoresModal(props) {

    const { t } = useTranslation();

    const [checked, setChecked] = useState(null);
    const [mostrarGenerar, setMostrarGenerar] = useState(null);
    const [mostrarReimprimir, setMostrarReimprimir] = useState(null);

    const validationSchema = {

        predio: Yup.string().required(),
        impresora: Yup.string().required(),
        estilo: Yup.string().required(),
        lenguaje: Yup.string(),
        descripcionLenguaje: Yup.string(),
        tipoContenedor: Yup.string().required(),
        contenedor: Yup.string(),
        numCopias: Yup.string(),
        cantGenerar: Yup.string(),
    };

    const onBeforeValidateField = (context, form, query, nexus) => {

        updateParameter(query.parameters, "isReimprimir", mostrarReimprimir);

    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {

            props.onHide(props.nexus);
        }
    }

    const onBeforeSubmit = (context, form, query, nexus) => {
        if (props.contenedor) {
            query.parameters = props.contenedor;
        }
        updateParameter(query.parameters, "isReimprimir", mostrarReimprimir);
        query.parameters.push({ id: "isSubmit", value: true });
    }

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = props.contenedor;

    }

    const onAfterInitialize = (context, form, query, nexus) => {
        setChecked(true);
        setMostrarGenerar(true);
        setMostrarReimprimir(false);
    }

    const handleChange = () => {
        setChecked(!checked);
        setMostrarGenerar(!mostrarGenerar);
        setMostrarReimprimir(!mostrarReimprimir);
    }

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    const updateParameter = (parameters, id, value) => {
        const index = parameters.findIndex(param => param.id === id);

        if (index !== -1) {
            parameters[index].value = value;
        }
        else {
            parameters.push({ id, value });
        }
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                application="IMP110GralContenedores"
                id="IMP110GralContenedores_form_1"
                validationSchema={validationSchema}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
                onAfterInitialize={onAfterInitialize}
                onBeforeInitialize={handleFormBeforeInitialize}
                onBeforeValidateField={onBeforeValidateField}
            >

                <Modal.Header closeButton>
                    <Modal.Title>{t("IMP110_Sec0_mdl_ReimpresionesContenedores_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Container fluid>
                        <Row>
                            <Col>
                                <Col>
                                    <div className="form-group" >
                                        <input className="form-check-input" type="radio" checked={checked} onChange={handleChange} />
                                        <label>
                                            {t("IMP110_frm1_lbl_checkGenerar")}
                                        </label>
                                    </div>
                                </Col>
                                <Col>
                                    <input className="form-check-input" type="radio" checked={!checked} onChange={handleChange} />
                                    <label>
                                        {t("IMP110_frm1_lbl_checkReimprimir")}
                                    </label>
                                </Col>
                            </Col>
                        </Row>

                        <hr></hr>
                        { /* GENERAR */}
                        <Row style={{ display: mostrarGenerar ? 'block' : 'none' }}>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="cantGenerar">{t("IMP110_frm1_lbl_cantGenerar")}</label>
                                    <Field name="cantGenerar" />
                                    <StatusMessage for="cantGenerar" />
                                </div>
                            </Col>
                        </Row>
                        { /* REIMPRIMIR */}
                        <Row style={{ display: mostrarReimprimir ? 'block' : 'none' }}>
                            <Col>
                                <Row>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="contenedor">{t("IMP110_frm1_lbl_contenedor")}</label>
                                            <FieldSelectAsync name="contenedor" />
                                            <StatusMessage for="contenedor" />
                                        </div>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="numCopias">{t("IMP110_frm1_lbl_numCopias")}</label>
                                            <Field name="numCopias" />
                                            <StatusMessage for="numCopias" />
                                        </div>
                                    </Col>
                                </Row>
                            </Col>
                        </Row>
                        <hr></hr>
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="predio">{t("IMP110_frm1_lbl_predio")}</label>
                                    <FieldSelect name="predio" />
                                    <StatusMessage for="predio" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="impresora">{t("IMP110_frm1_lbl_impresora")}</label>
                                    <FieldSelect name="impresora" />
                                    <StatusMessage for="impresora" />
                                </div>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="tipoContenedor">{t("IMP110_frm1_lbl_tipoContenedor")}</label>
                                    <FieldSelect name="tipoContenedor" />
                                    <StatusMessage for="tipoContenedor" />
                                </div>
                            </Col>
                            <Col>
                                <Row>
                                    <Col sm={4}>
                                        <div className="form-group" >
                                            <label htmlFor="lenguaje">{t("IMP110_frm1_lbl_lenguaje")}</label>
                                            <Field name="lenguaje" />
                                            <StatusMessage for="lenguaje" />
                                        </div>
                                    </Col>
                                    <Col sm={8}>
                                        <div className="form-group" >
                                            <label htmlFor="descripcionLenguaje">{t("IMP110_frm1_lbl_descripcionLenguaje")}</label>
                                            <Field name="descripcionLenguaje" />
                                            <StatusMessage for="descripcionLenguaje" />
                                        </div>
                                    </Col>
                                </Row>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="estilo">{t("IMP110_frm1_lbl_estilo")}</label>
                                    <FieldSelect name="estilo" />
                                    <StatusMessage for="estilo" />
                                </div>
                            </Col>
                        </Row>


                    </Container>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("IMP050_frm1_btn_cerrar")} </Button>
                    <SubmitButton id="btnSubmitConfirmar" variant="primary" label="IMP050_frm1_btn_confirmar" />
                </Modal.Footer>
            </Form>
        </Modal >
    );
}

export const IMP110ImpresionYGenerarContenedoresModal = withPageContext(InternalIMP110ImpresionYGenerarContenedoresModal);
