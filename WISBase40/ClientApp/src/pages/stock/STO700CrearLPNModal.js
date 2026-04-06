import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldToggle, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Container, ToggleButton } from 'react-bootstrap';
import * as Yup from 'yup';


function InternalSTO700CrearLPNModal(props) {

    const { t, i18n } = useTranslation();

    const [isTipoSoloGenerar, setIsTipoSoloGenerar] = useState("hidden");
    const [isTipoGenerarImprimir, setIsTipoGenerarImprimir] = useState("hidden");


    const initialValues = {
        tipo: "",
        cantidadLPN: "",
        empresa: "",
        generarImprimir: "",
        predio: "",
        impresora: "",
        estilo: "",
        lenguaje: "",
        descripcion: "",
        cantidadCopias: "",
        packingList: "",
    };

    const validationSchema = {
        tipo: Yup.string().required(),
        cantidadLPN: Yup.string().required(),
        empresa: Yup.string().required(),
        generarImprimir: Yup.string().required(),
        predio: Yup.string(),
        impresora: Yup.string(),
        estilo: Yup.string(),
        lenguaje: Yup.string(),
        descripcion: Yup.string(),
        cantidadCopias: Yup.string(),
        packingList: Yup.string(),
    };

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    const onAfterValidateField = (context, form, query, nexus) => {

        const generarImprimir = query.parameters.find(p => p.id === "generarImprimir");

        if (generarImprimir) {
            setIsTipoSoloGenerar("hidden");
            setIsTipoGenerarImprimir("hidden");
        }

        if (generarImprimir) {
            switch (generarImprimir.value) {
                case 'false':
                    setIsTipoSoloGenerar("");
                    setIsTipoGenerarImprimir("hidden");
                    break;
                case 'true':
                    setIsTipoSoloGenerar("hidden");
                    setIsTipoGenerarImprimir("");
                    break;
            }
        }

    };

    const onAfterSubmit = (context, form, query, nexus) => {
        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            props.onHide(props.nexus);
        }
    }

    const onBeforeSubmit = (context, form, query, nexus) => {
        if (props.rowSeleccionadas) {
            query.parameters = [{ id: "ListaFilasSeleccionadas", value: props.rowSeleccionadas }];
        }
        query.parameters.push({ id: "isSubmit", value: true });
    }

    // allow keys to be phrases having `:`
    i18n.options.nsSeparator = '::';

    return (

        <Form
            application="STO700CrearLPN"
            id="STO700CrearLPN_form_1"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onAfterValidateField={onAfterValidateField}
            onAfterSubmit={onAfterSubmit}
            onBeforeSubmit={onBeforeSubmit}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("STO700CrearLPN_Sec0_mdlEdit_Crear")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="empresa">{t("STO700_Sec0_Info_mdl_Empresa")}</label>
                                <FieldSelectAsync name="empresa" />
                                <StatusMessage for="empresa" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="tipo">{t("STO700_Sec0_Info_mdl_tipo")}</label>
                                <FieldSelectAsync name="tipo" />
                                <StatusMessage for="tipo" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="packingList">{t("STO700_Sec0_Info_mdl_PackingList")}</label>
                                <Field name="packingList" />
                                <StatusMessage for="packingList" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="cantidadLPN">{t("STO700_Sec0_Info_mdl_cantidadLPN")}</label>
                                <Field name="cantidadLPN" />
                                <StatusMessage for="cantidadLPN" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col sm={1}>
                            <div style={{ textAlign: "right", fontSize: "16px" }}>
                                <label htmlFor="generarImprimir">{t("STO700_Sec0_Info_mdl_Generar")}</label>
                            </div>
                        </Col>
                        <Col sm={3}>
                            <div style={{ textAlign: "left", float: "left", fontSize: "16px"}}>
                                <FieldToggle name="generarImprimir" label={t("STO700_Sec0_Info_mdl_GenerarImprimir")} />
                                <StatusMessage for="generarImprimir" />
                            </div>
                        </Col>
                    </Row>
                    <div className={isTipoGenerarImprimir}>
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="predio">{t("STO700_Sec0_Info_mdl_predio")}</label>
                                    <FieldSelect name="predio" />
                                    <StatusMessage for="predio" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="impresora">{t("STO700_Sec0_Info_mdl_impresora")}</label>
                                    <FieldSelect name="impresora" />
                                    <StatusMessage for="impresora" />
                                </div>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="lenguaje">{t("STO700_Sec0_Info_mdl_lenguaje")}</label>
                                    <Field name="lenguaje" />
                                    <StatusMessage for="lenguaje" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="descripcion">{t("STO700_Sec0_Info_mdl_descripcion")}</label>
                                    <Field name="descripcion" />
                                    <StatusMessage for="descripcion" />
                                </div>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="estilo">{t("STO700_Sec0_Info_mdl_estilo")}</label>
                                    <Field name="estilo" />
                                    <StatusMessage for="estilo" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="cantidadCopias">{t("STO700_Sec0_Info_mdl_cantidadCopias")}</label>
                                    <Field name="cantidadCopias" />
                                    <StatusMessage for="cantidadCopias" />
                                </div>
                            </Col>
                        </Row>
                </div>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("STO700_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmar" variant="primary" label="STO700_frm1_btn_confirmar" />
            </Modal.Footer>
        </Form>
    );
}

export const STO700CrearLPNModal = withPageContext(InternalSTO700CrearLPNModal);