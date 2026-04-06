import React, { useState, useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import * as Yup from 'yup';

function InternalIMP110ImpresionGralContenedoresModal(props) {

    const { t } = useTranslation();

    const datosContenedor = useRef({});
    const [infoContenedor, setInfoContenedor] = useState({
        contenedor: "", preparacion: ""
    });

    const validationSchema = {

        predio: Yup.string().required(),
        impresora: Yup.string().required(),
        estilo: Yup.string().required(),
        lenguaje: Yup.string(),
        descripcionLenguaje: Yup.string(),
        tipoContenedor: Yup.string().required(),
        numCopias: Yup.string().required(),
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {

            props.onHide(props.nexus);
        }
    }

    const onBeforeSubmit = (context, form, query, nexus) => {
        if (props.contenedor) {
            query.parameters = props.contenedor;
            //query.parameters.push({ id: "isSubmit", value: true });

            [
                { id: "contenedor", value: props.contenedor.find(x => x.id === "contenedor").value },
                { id: "preparacion", value: props.contenedor.find(x => x.id === "preparacion").value },
            ];
        }
        query.parameters.push({ id: "isSubmit", value: true });
    }

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = props.contenedor;

        setInfoContenedor({
            contenedor: props.contenedor.find(d => d.id === "contenedor").value,
            preparacion: props.contenedor.find(d => d.id === "preparacion").value,
        });
    }


    const handleClose = () => {
        props.onHide(props.nexus);
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                application="IMP110GralReimpresionContenedores"
                id="IMP110GralReimpresionContenedores_form_1"
                validationSchema={validationSchema}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeInitialize={handleFormBeforeInitialize}
            >

                <Modal.Header closeButton>
                    <Modal.Title>{t("IMP110_Sec0_mdl_GralContenedores_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Container fluid>
                        <Row className="mb-2">
                            <Col>
                                <span className="text-muted">{t("IMP110_frm1_lbl_infoContenedor")} </span>
                                <span>{`${infoContenedor.contenedor}`}</span>
                            </Col>
                            <Col>
                                <span className="text-muted">{t("IMP110_frm1_lbl_infoPreparacion")} </span>
                                <span>{`${infoContenedor.preparacion}`}</span>
                            </Col>
                        </Row>

                        <hr /> 

                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="predio">{t("IMP050_frm1_lbl_predio")}</label>
                                    <FieldSelect name="predio" />
                                    <StatusMessage for="predio" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="impresora">{t("IMP050_frm1_lbl_impresora")}</label>
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
                                            <label htmlFor="lenguaje">{t("IMP050_frm1_lbl_lenguaje")}</label>
                                            <Field name="lenguaje" />
                                            <StatusMessage for="lenguaje" />
                                        </div>
                                    </Col>
                                    <Col sm={8}>
                                        <div className="form-group" >
                                            <label htmlFor="descripcionLenguaje">{t("IMP050_frm1_lbl_descripcionLenguaje")}</label>
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
                                    <label htmlFor="estilo">{t("IMP050_frm1_lbl_estilo")}</label>
                                    <FieldSelect name="estilo" />
                                    <StatusMessage for="estilo" />
                                </div>
                            </Col>

                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="numCopias">{t("IMP050_frm1_lbl_numCopias")}</label>
                                    <Field name="numCopias" />
                                    <StatusMessage for="numCopias" />
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
        </Modal>
    );
}

export const IMP110ImpresionGralContenedoresModal = withPageContext(InternalIMP110ImpresionGralContenedoresModal);
