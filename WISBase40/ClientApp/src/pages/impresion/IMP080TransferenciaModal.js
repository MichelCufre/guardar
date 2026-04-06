import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import * as Yup from 'yup';
import { set } from 'date-fns';

function InternalImpresionTransferenciaModal(props) {

    const { t } = useTranslation();

    const [isReimprimir, setIsReimprimir] = useState(false);

    const validationSchema = {

        predio: Yup.string().required(),
        impresora: Yup.string().required(),
        estilo: Yup.string().required(),
        lenguaje: Yup.string().required(),
        descripcionLenguaje: Yup.string(),
        numCopias: Yup.string(),
        cantGenerar: Yup.string(),
    };

    const onAfterInitialize = (context, form, query, nexus) => {
        if (props.tipoImpresion && props.tipoImpresion == "Reimprimir") {
            setIsReimprimir(true);
        } else {
            setIsReimprimir(false);
        }
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {

            props.onHide(props.nexus);
        }
    }

    const onBeforeSubmit = (context, form, query, nexus) => {

        if (props.rowSeleccionadas) {
            query.parameters = [{ id: "ListaFilasSeleccionadas", value: props.rowSeleccionadas }];
        };

        if (props.tipoImpresion && props.tipoImpresion == "Reimprimir") {
            query.parameters.push({
                id: "reimprimirEtiquetas", value: "true"
            });
        }else if(props.tipoImpresion && props.tipoImpresion == "Generar"){
            query.parameters.push({
                id: "generarEtiquetas", value: "true"
            });
        }

        query.parameters.push({ id: "isSubmit", value: true });
    }

    const handleBeforeValidate = (context, form, query, nexus) => {

        if (props.tipoImpresion && props.tipoImpresion == "Reimprimir") {
            query.parameters.push({
                id: "reimprimirEtiquetas", value: "true"
            });
        }else if(props.tipoImpresion && props.tipoImpresion == "Generar"){
            query.parameters.push({
                id: "generarEtiquetas", value: "true"
            });
        }
    }

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                application="IMP080Transferencia"
                id="IMP080Transferencia_form_1"
                validationSchema={validationSchema}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
                onAfterInitialize={onAfterInitialize}
                onBeforeValidateField={handleBeforeValidate}
            >

                <Modal.Header closeButton>
                    <Modal.Title>{isReimprimir ? t("IMP080_Sec0_mdl_Transferencia_reImprimirTitulo") : t("IMP080_Sec0_mdl_Transferencia_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Container fluid>
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="predio">{t("IMP080_frm1_lbl_predio")}</label>
                                    <FieldSelect name="predio" />
                                    <StatusMessage for="predio" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="impresora">{t("IMP080_frm1_lbl_impresora")}</label>
                                    <FieldSelect name="impresora" />
                                    <StatusMessage for="impresora" />
                                </div>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="estilo">{t("IMP080_frm1_lbl_estilo")}</label>
                                    <FieldSelect name="estilo" />
                                    <StatusMessage for="estilo" />
                                </div>
                            </Col>
                            <Col>
                                <Row>
                                    <Col sm={4}>
                                        <div className="form-group" >
                                            <label htmlFor="lenguaje">{t("IMP080_frm1_lbl_lenguaje")}</label>
                                            <Field name="lenguaje" />
                                            <StatusMessage for="lenguaje" />
                                        </div>
                                    </Col>
                                    <Col sm={8}>
                                        <div className="form-group" >
                                            <label htmlFor="descripcionLenguaje">{t("IMP080_frm1_lbl_descripcionLenguaje")}</label>
                                            <Field name="descripcionLenguaje" />
                                            <StatusMessage for="descripcionLenguaje" />
                                        </div>
                                    </Col>
                                </Row>
                            </Col>
                        </Row>
                        <Row style={{ display: isReimprimir ? 'block' : 'none' }}>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="numCopias">{t("IMP080_frm1_lbl_numCopias")}</label>
                                    <Field name="numCopias" />
                                    <StatusMessage for="numCopias" />
                                </div>
                            </Col>
                        </Row>
                        <Row style={{ display: !isReimprimir ? 'block' : 'none' }}>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="cantGenerar">{t("IMP080_frm1_lbl_cantGenerar")}</label>
                                    <Field name="cantGenerar" />
                                    <StatusMessage for="cantGenerar" />
                                </div>
                            </Col>
                        </Row>
                    </Container>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("IMP080_frm1_btn_cerrar")} </Button>
                    <SubmitButton id="btnSubmitConfirmar" variant="primary" label="IMP080_frm1_btn_confirmar" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}

export const IMP080TransferenciaModal = withPageContext(InternalImpresionTransferenciaModal);
