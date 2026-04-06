import React, { useState, useRef, useEffect } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useCustomTranslation } from '../../components/TranslationHook';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { notificationType } from '../../components/Enums';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';
import { FormTabTitle } from '../../components/FormComponents/FormTabTitle';

function InternalREG220CreateAgenteModal(props) {
    const { t } = useCustomTranslation();

    const warningOrdenDeCarga = useRef({});
    const [isFormDisplayed, setFormDisplayed] = useState(false);

    const [hasChangesDetalle, setChangesDetalle] = useState(false);
    const [hasChangesAnexos, setChangesAnexos] = useState(false);

    const initialValues = {
        empresa: "",
        tipoAgente: "",
        codigo: "",
        descripcion: "",
        tipoFiscal: "",
        numeroFiscal: "",
        otroDatoFiscal: "",
        locacionGlobal: "",
        ruta: "",
        ordenCarga: "",
        pais: "",
        paisSubdivision: "",
        localidad: "",
        direccion: "",
        barrio: "",
        codigoPostal: "",
        telefonoPrincipal: "",
        telefonoSecundario: "",
        anexo1: "",
        anexo3: "",
        anexo2: "",
        anexo4: "",
        email: "",
    };

    const validationSchema = {

        empresa: Yup.string().required(),
        tipoAgente: Yup.string().required(),
        codigo: Yup.string().required(),
        descripcion: Yup.string().required(),
        tipoFiscal: Yup.string(),
        numeroFiscal: Yup.string(),
        otroDatoFiscal: Yup.string(),
        locacionGlobal: Yup.string(),
        ruta: Yup.string().required(),
        ordenCarga: Yup.string(),
        pais: Yup.string(),
        paisSubdivision: Yup.string(),
        localidad: Yup.string(),
        direccion: Yup.string(),
        barrio: Yup.string(),
        codigoPostal: Yup.string(),
        telefonoPrincipal: Yup.string(),
        telefonoSecundario: Yup.string(),
        valorManejaVidaUtil: Yup.string(),
        anexo1: Yup.string(),
        anexo3: Yup.string(),
        anexo2: Yup.string(),
        anexo4: Yup.string(),
        email: Yup.string(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormBeforeSubmit = (context, form, query, nexus) => {

        query.parameters = [
            { id: "isSubmit", value: true }
        ];

    }
    const handleFormBeforeValidateField = (context, form, query, nexus) => {

    }

    const handleFormAfterValidateField = (context, form, query, nexus) => {

        if (query.fieldId == "ordenCarga") {

            const warning = query.parameters.find(p => p.id === "warning");
            const tipoAgente = query.parameters.find(p => p.id === "tipoAgente");
            const codigoAgente = query.parameters.find(p => p.id === "codigoAgente");
            const descripcionAgente = query.parameters.find(p => p.id === "descripcionAgente");
            const empresaAgente = query.parameters.find(p => p.id === "empresaAgente");

            if (warning) {

                warningOrdenDeCarga.current = t(warning.value, [tipoAgente.value,
                codigoAgente.value,
                descripcionAgente.value,
                empresaAgente.value]
                );

                setFormDisplayed(true);
            } else {
                setFormDisplayed(false);
            }
        }

        const formComponent = nexus.getForm("REG220_form_1");

        if (formComponent) {
            setChangesDetalle(formComponent.hasChanges(["tipoFiscal", "numeroFiscal", "otroDatoFiscal", "locacionGlobal", "pais", "paisSubdivision", "localidad",
                "codigoPostal", "direccion", "barrio", "email", "telefonoPrincipal", "telefonoSecundario"]));
            setChangesAnexos(formComponent.hasChanges(["anexo1", "anexo3", "anexo2", "anexo4"]));
        }

    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("REG220_grid_1").refresh();
            props.onHide();
        }
    }

    const titleDetalles = (
        <FormTabTitle value="Detalles" hasChanges={hasChangesDetalle} />
    );
    const titleAnexos = (
        <FormTabTitle value="Anexos" hasChanges={hasChangesAnexos} />
    );

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Form
                id="REG220_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeSubmit={handleFormBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeValidateField={handleFormBeforeValidateField}
                onAfterValidateField={handleFormAfterValidateField}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("REG220_Sec0_mdlCreate_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col>
                            <Row>
                                <Col>
                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="empresa">{t("REG220_frm1_lbl_empresa")}</label>
                                                <FieldSelectAsync name="empresa" />
                                                <StatusMessage for="empresa" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="tipoAgente">{t("REG220_frm1_lbl_tipoAgente")}</label>
                                                <FieldSelect name="tipoAgente" />
                                                <StatusMessage for="tipoAgente" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="codigo">{t("REG220_frm1_lbl_codigo")}</label>
                                                <Field name="codigo" />
                                                <StatusMessage for="codigo" />
                                            </div>
                                        </Col>
                                    </Row>
                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="ruta">{t("REG220_frm1_lbl_ruta")}</label>
                                                <FieldSelect name="ruta" />
                                                <StatusMessage for="ruta" />
                                            </div>
                                        </Col>
                                    </Row>
                                </Col>
                                <Col>
                                    <Row>
                                        <Col>
                                            <div className="form-group">
                                                <label htmlFor="descripcion">{t("REG220_frm1_lbl_descripcion")}</label>
                                                <Field name="descripcion" />
                                                <StatusMessage for="descripcion" />
                                            </div>
                                        </Col>
                                        
                                    </Row>
                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="ordenCarga">{t("REG220_frm1_lbl_ordenCarga")}</label>
                                                <Field name="ordenCarga" />
                                                <StatusMessage for="ordenCarga" />
                                                <div style={{ display: isFormDisplayed ? 'block' : 'none' }}>
                                                    <span className="text-warning">
                                                        {`${warningOrdenDeCarga.current}`}
                                                    </span>
                                                </div>
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="valorManejaVidaUtil">{t("REG220_frm1_lbl_VL_ManejaVidaUtil")}</label>
                                                <Field name="valorManejaVidaUtil" />
                                                <StatusMessage for="valorManejaVidaUtil" />
                                            </div>
                                        </Col>
                                    </Row>
                                </Col>
                            </Row>
                            <Tabs defaultActiveKey="detail" transition={false} id="noanim-tab-example">
                                <Tab eventKey="detail" title={titleDetalles}>
                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="tipoFiscal">{t("REG220_frm1_lbl_tipoFiscal")}</label>
                                                <FieldSelect name="tipoFiscal" />
                                                <StatusMessage for="tipoFiscal" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="numeroFiscal">{t("REG220_frm1_lbl_numeroFiscal")}</label>
                                                <Field name="numeroFiscal" />
                                                <StatusMessage for="numeroFiscal" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="otroDatoFiscal">{t("REG220_frm1_lbl_otroDatoFiscal")}</label>
                                                <Field name="otroDatoFiscal" />
                                                <StatusMessage for="otroDatoFiscal" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="locacionGlobal">{t("REG220_frm1_lbl_locacionGlobal")}</label>
                                                <Field name="locacionGlobal" />
                                                <StatusMessage for="locacionGlobal" />
                                            </div>
                                        </Col>
                                    </Row>

                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="pais">{t("REG220_frm1_lbl_Pais")}</label>
                                                <FieldSelectAsync name="pais" />
                                                <StatusMessage for="pais" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="paisSubdivision">{t("REG220_frm1_lbl_PaisSubdivision")}</label>
                                                <FieldSelectAsync name="paisSubdivision" />
                                                <StatusMessage for="paisSubdivision" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="localidad">{t("REG220_frm1_lbl_PaisSubdivisionLocalidad")}</label>
                                                <FieldSelectAsync name="localidad" />
                                                <StatusMessage for="localidad" />
                                            </div>
                                        </Col>

                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="codigoPostal">{t("REG220_frm1_lbl_codigoPostal")}</label>
                                                <Field name="codigoPostal" />
                                                <StatusMessage for="codigoPostal" />
                                            </div>
                                        </Col>
                                    </Row>
                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="direccion">{t("REG220_frm1_lbl_direccion")}</label>
                                                <Field name="direccion" />
                                                <StatusMessage for="direccion" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="barrio">{t("REG220_frm1_lbl_barrio")}</label>
                                                <Field name="barrio" />
                                                <StatusMessage for="barrio" />
                                            </div>
                                        </Col>

                                    </Row>
                                    <Row xs={2} md={4} lg={12}>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="telefonoPrincipal">{t("REG220_frm1_lbl_telefonoPrincipal")}</label>
                                                <Field name="telefonoPrincipal" />
                                                <StatusMessage for="telefonoPrincipal" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="telefonoSecundario">{t("REG220_frm1_lbl_telefonoSecundario")}</label>
                                                <Field name="telefonoSecundario" />
                                                <StatusMessage for="telefonoSecundario" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="email">{t("REG220_frm1_lbl_email")}</label>
                                                <Field name="email" />
                                                <StatusMessage for="email" />
                                            </div>
                                        </Col>
                                    </Row>
                                </Tab>
                                <Tab eventKey="anexos" title={titleAnexos}>
                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="anexo1">{t("REG220_frm1_lbl_anexo1")}</label>
                                                <FieldTextArea name="anexo1" />
                                                <StatusMessage for="anexo1" />
                                            </div>
                                            <div className="form-group" >
                                                <label htmlFor="anexo3">{t("REG220_frm1_lbl_anexo3")}</label>
                                                <FieldTextArea name="anexo3" />
                                                <StatusMessage for="anexo3" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="anexo2">{t("REG220_frm1_lbl_anexo2")}</label>
                                                <FieldTextArea name="anexo2" />
                                                <StatusMessage for="anexo2" />
                                            </div>
                                            <div className="form-group" >
                                                <label htmlFor="anexo4">{t("REG220_frm1_lbl_anexo4")}</label>
                                                <FieldTextArea name="anexo4" />
                                                <StatusMessage for="anexo4" />
                                            </div>
                                        </Col>

                                    </Row>
                                </Tab>
                            </Tabs>
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                        {t("REG220_frm1_btn_CANCELAR")}
                    </Button>
                    <SubmitButton id="btnSubmitCreateFamilia" variant="primary" label="REG220_frm1_btn_CREAR" />
                </Modal.Footer>
            </Form>
        </Modal >
    );
}

export const REG220CreateAgenteModal = withPageContext(InternalREG220CreateAgenteModal);