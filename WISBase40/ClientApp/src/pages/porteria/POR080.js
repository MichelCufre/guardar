import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Page } from '../../components/Page';
import { Form, Field, FieldDate, FieldDateTime, FieldCheckbox, FieldSelect, FieldSelectAsync, FormButton, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import { Row, Col, FormGroup, Button } from 'react-bootstrap';
import { FormTab, FormTabStep } from '../../components/FormComponents/FormTab';
import { Grid } from '../../components/GridComponents/Grid';

export default function POR080(props) {
    const { t } = useTranslation();

    const [isShowForm, setIsShowForm] = useState(false);
    const [encontroPreRegistro, setEncontroPreRegistro] = useState(false);

    const rt = {
        unknown: "",
        proverdor: "PRO",
        cliente: "CLI",
        empresa: "EMP",
        asociado: "ACL",
        particular: "POF",
    };

    const [isShowGrid, setShowGrid] = useState(false);
    const GridShowClassName = isShowGrid ? "row mb-4" : "row mb-4";

    const [isHiddenContainer, setIsHiddenContainer] = useState(false);
    const HiddenContainerClassName = isHiddenContainer ? "hidden" : "";

    const [currentTab, setCurrentTab] = useState("tabTipoRegistro");
    const [tr, setTipoRegistro] = useState(rt.unknown);
    const [tipoMotivo, setTipoMotivo] = useState(null);

    //const [tiposPorteriaHabilitados, setTiposPorteriaHabilitados] = useState([]);
    const [motivosSegunTipoPorteria, setMotivosSegunTipoPorteria] = useState([]);

    const [SelectionGridPersonas, setSelectionGridPersonas] = useState(null);
    const [SelectionGridContainers, setSelectionGridContainers] = useState(null);

    const [isInsertContacto, setIsInsertContacto] = useState(false);

    const HideForm = (evt) => {
        setIsShowForm(false);
        evt.preventDefault();
    }

    const AlterHideGrid = (evt) => {
        setShowGrid(!isShowGrid);
        evt.preventDefault();
    }

    const onGoNextTab = (nexus) => {

        if (currentTab === "tabTipoRegistro") {
            setCurrentTab("tabDatosVehiculo");
            nexus.getForm("POR080_form_1").setFieldValue("CURRENT", "tabDatosVehiculo");
        }
        else if (currentTab === "tabDatosVehiculo") {
            setCurrentTab("tabDatosGenerales")
            nexus.getForm("POR080_form_1").setFieldValue("CURRENT", "tabDatosGenerales");
        }
        else if (currentTab === "tabDatosGenerales") {
            setCurrentTab("tabAsociarContactos")
            nexus.getForm("POR080_form_1").setFieldValue("CURRENT", "tabAsociarContactos");
        }

    };

    const onGoTabDatosVehiculo = (nexus, type) => {

        setCurrentTab("tabDatosVehiculo");
        setTipoRegistro(type);

        let form = nexus.getForm("POR080_form_1");

        form.setFieldValue("CD_TP_POTERIA_REGISTRO", type);
        form.setFieldValue("CURRENT", "tabDatosVehiculo");

        form.setFieldValue("CD_POTERIA_MOTIVO", motivosSegunTipoPorteria[type]);

        setTipoMotivo(motivosSegunTipoPorteria[type]);

    };

    const onGoPreviusTab = (nexus) => {

        if (currentTab === "tabDatosVehiculo") {
            nexus.getForm("POR080_form_1").reset();
        }
        else if (currentTab === "tabDatosGenerales") {
            setCurrentTab("tabDatosVehiculo");
            nexus.getForm("POR080_form_1").setFieldValue("CURRENT", "tabDatosVehiculo");

        }
        else if (currentTab === "tabAsociarContactos") {
            setCurrentTab("tabDatosGenerales");
        }

    }


    const initialValues = {
        CD_TRANSPORTE_V1: "PORREGSITAUT",
    };

    const validationSchema = {


        //Vehiculo
        CD_TRANSPORTE_V1: Yup.string().when('CURRENT', {
            is: 'tabDatosVehiculo',
            then: Yup.string().required().max(20),
            otherwise: Yup.string().nullable(),
        }),
        VL_MATRICULA_1_ENTRADA: Yup.string().when('CURRENT', {
            is: 'tabDatosVehiculo',
            then: Yup.string().nullable().max(30),
            otherwise: Yup.string().nullable(),
        }),
        VL_MATRICULA_2_ENTRADA: Yup.string().when('CURRENT', {
            is: 'tabDatosVehiculo',
            then: Yup.string().nullable().max(30),
            otherwise: Yup.string().nullable(),
        }),
        CD_SECTOR: Yup.string().when('CURRENT', {
            is: 'tabDatosVehiculo',
            then: Yup.string().required().max(20),
            otherwise: Yup.string().nullable(),
        }),

        NU_PREDIO: Yup.string().when('CURRENT', {
            is: 'tabDatosVehiculo',
            then: Yup.string().nullable().max(10),
            otherwise: Yup.string().nullable(),
        }),

        //Datos Generales

        CD_POTERIA_MOTIVO: Yup.string().when('CURRENT', {
            is: 'tabDatosGenerales',
            then: Yup.string().nullable().max(20),
            otherwise: Yup.string().nullable(),
        }),

        CD_EMPRESA: Yup.string().when('CURRENT', {
            is: 'tabDatosGenerales',
            then: Yup.string().nullable().max(20),
            otherwise: Yup.string().nullable(),
        }),

        CD_CLIENTE: Yup.string().when('CURRENT', {
            is: 'tabDatosGenerales',
            then: Yup.string().nullable().max(20),
            otherwise: Yup.string().nullable(),
        }),

        NU_CONTAINER: Yup.string().when('CURRENT', {
            is: 'tabDatosGenerales',
            then: Yup.string().nullable().max(20),
            otherwise: Yup.string().nullable(),
        }),


        // Asociar contactos

        NU_DOCUMENTO_SEARCH: Yup.string().when('CURRENT', {
            is: 'tabAsociarContactos',
            then: Yup.string().nullable().max(20),
            otherwise: Yup.string().nullable(),
        }),


        NU_DOCUMENTO: Yup.string().when('CURRENT', {
            is: 'tabAsociarContactos',
            then: Yup.string().nullable().max(20),
            otherwise: Yup.string().nullable(),
        }),

        NM_PERSONA: Yup.string().when('CURRENT', {
            is: 'tabAsociarContactos',
            then: Yup.string().nullable().max(20),
            otherwise: Yup.string().nullable(),
        }),

        AP_PERSONA: Yup.string().when('CURRENT', {
            is: 'tabAsociarContactos',
            then: Yup.string().nullable().max(20),
            otherwise: Yup.string().nullable(),
        }),

        NU_CELULAR: Yup.string().when('CURRENT', {
            is: 'tabAsociarContactos',
            then: Yup.string().nullable().max(20),
            otherwise: Yup.string().nullable(),
        }),


    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        context.abortServerCall = true;

        if (query.buttonId === "btnConfirmar") {

            context.abortServerCall = false;

            query.parameters = [
                { id: "SelectionGridPersonas", value: SelectionGridPersonas },
                { id: "SelectionGridContainers", value: SelectionGridContainers },
                { id: "FL_PRE_REGISTRO", value: "S" },
            ];

        }
        else if (query.buttonId === "btnConfirmarVehiculo") {
            context.abortServerCall = false;
        }
        else if (query.buttonId === "btnConfirmarGeneral") {
            context.abortServerCall = false;
        }
        else if (query.buttonId === "btnConfirmarContacto") {
            context.abortServerCall = false;

            query.parameters = [
                { id: "SelectionGridPersonas", value: SelectionGridPersonas },
            ];
        }
        else if (query.buttonId === "btnConfirmarContainer") {
            context.abortServerCall = false;

            query.parameters = [
                { id: "SelectionGridContainers", value: SelectionGridContainers },
            ];
        }

    };

    const onAfterSubmit = (context, form, query, nexus) => {

        if (context.responseStatus === "ERROR") return;

        if (query.buttonId === "btnConfirmar") {

            setIsShowForm(false);
            nexus.getForm("POR080_form_1").reset();
            nexus.getGrid("POR080_grid_1").refresh();
            nexus.getGrid("POR080_grid_Personas").refresh();
        }
        else if (query.buttonId === "btnConfirmarVehiculo") {

            if (form.fields.find(w => w.id == "CD_TRANSPORTE_V1").value == "PORREGSITAPI") {
                setIsHiddenContainer(true);
            }
            else {
                setIsHiddenContainer(false);
            }

            onGoNextTab(nexus);
        }
        else if (query.buttonId === "btnConfirmarGeneral") {
            onGoNextTab(nexus);
        }
        else if (query.buttonId === "btnConfirmarContacto") {

            setSelectionGridPersonas(query.parameters.find(w => w.id === "SelectionGridPersonas").value);

            nexus.getGrid("POR080_grid_Personas").refresh();

        }
        else if (query.buttonId === "btnConfirmarContainer") {

            setSelectionGridContainers(query.parameters.find(w => w.id === "SelectionGridContainers").value);

            nexus.getGrid("POR080_grid_Containers").refresh();

        }


    };

    const formOnBeforeButtonAction = (context, form, query, nexus) => {

        context.abortServerCall = true;

        if (query.buttonId === "btnOpenForm") {
            setCurrentTab("tabTipoRegistro");
            nexus.getForm("POR080_form_1").reset();
            setSelectionGridPersonas(null);
            nexus.getGrid("POR080_grid_Personas").reset();
            setIsShowForm(true);
        }
        else if (query.buttonId === "btnCliente") {
            onGoTabDatosVehiculo(nexus, rt.cliente);
        }
        else if (query.buttonId === "btnEmpresa") {
            onGoTabDatosVehiculo(nexus, rt.empresa);
        }
        else if (query.buttonId === "btnProvedor") {
            onGoTabDatosVehiculo(nexus, rt.proverdor);
        }
        else if (query.buttonId === "btnParticular") {
            onGoTabDatosVehiculo(nexus, rt.particular);
        }
        else if (query.buttonId === "btnVolver") {
            onGoPreviusTab(nexus);
        }
    };

    const formOnAfterInitialize = (context, form, query, nexus) => {
        //setTiposPorteriaHabilitados(JSON.parse(query.parameters.find(w => w.id == "tiposPorteriaHabilitados").value));
        setSelectionGridPersonas(JSON.stringify([]));
        setMotivosSegunTipoPorteria(JSON.parse(query.parameters.find(w => w.id == "motivosSegunTipoPorteria").value));
        setCurrentTab("tabTipoRegistro");
    };

    const onBeforeFetch = (context, data, nexus) => {

        if (data.gridId == "POR080_grid_Containers") {
            data.parameters = [
                {
                    id: "SelectionGridContainers",
                    value: SelectionGridContainers
                }
            ];
        }
        else if (data.gridId == "POR080_grid_Personas") {
            data.parameters = [
                {
                    id: "SelectionGridPersonas",
                    value: SelectionGridPersonas
                }
            ];
        }

    };

    const onBeforeValidateField = (context, form, query, nexus) => {

        if (query.fieldId === "CD_POTERIA_MOTIVO") {
            setTipoMotivo(form.fields.find(w => w.id === "CD_POTERIA_MOTIVO").value)
        }
        else if (query.fieldId === "VL_MATRICULA_1_ENTRADA") {
            query.parameters = [
                {
                    id: "SelectionGridContainers",
                    value: JSON.stringify([])
                }
            ];

        }

    }

    const onAfterValidateField = (context, form, query, nexus) => {

        if (query.fieldId === "VL_MATRICULA_1_ENTRADA") {
            let containers = query.parameters.find(w => w.id === "SelectionGridContainers").value;

            if (containers != "") {
                setSelectionGridContainers(containers);
                nexus.getGrid("POR080_grid_Containers").refresh();
            }

        }
        else if (query.fieldId === "CD_TRANSPORTE_V1") {

            let fieldVL_MATRICULA_1_ENTRADA = form.fields.find(w => w.id == "VL_MATRICULA_1_ENTRADA");

            fieldVL_MATRICULA_1_ENTRADA.value = "";
            //fieldVL_MATRICULA_1_ENTRADA.readOnly = true;

        }

    }

    const onKeyPressMatricula = (e) => {

        var key = e.keyCode || e.which;
        if (key == 13) {
            setFocusField("VL_MATRICULA_2_ENTRADA");
        }

    }

    const setFocusField = (fieldName) => {
        document.getElementsByName(fieldName)[0].focus();

    }

    const gridOnBeforeButtonAction = (context, data, nexus) => {

        if (data.gridId == "POR080_grid_Personas") {
            data.parameters = [
                { id: "SelectionGridPersonas", value: SelectionGridPersonas },
            ];
        }
        else if (data.gridId == "POR080_grid_Containers") {

            data.parameters = [
                { id: "SelectionGridContainers", value: SelectionGridContainers },
            ];
        }

    }

    const onAfterButtonAction = (data, nexus) => {

        if (data.gridId == "POR080_grid_Personas") {
            setSelectionGridPersonas(data.parameters.find(w => w.id === "SelectionGridPersonas").value);

            nexus.getGrid("POR080_grid_Personas").refresh();
        }
        else if (data.gridId == "POR080_grid_Containers") {

            setSelectionGridContainers(data.parameters.find(w => w.id === "SelectionGridContainers").value);

            nexus.getGrid("POR080_grid_Containers").refresh();
        }

    }


    return (

        <Page
            title={t("POR080_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <Form
                id="POR080_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={onAfterSubmit}
                onBeforeButtonAction={formOnBeforeButtonAction}
                onAfterInitialize={formOnAfterInitialize}
                onBeforeValidateField={onBeforeValidateField}
                onAfterValidateField={onAfterValidateField}
            >
                <Field name="CD_TP_POTERIA_REGISTRO" hidden />
                <Field name="CURRENT" hidden />



                <FormTab
                    current={currentTab}
                >

                    <FormTabStep id="tabTipoRegistro" label="POR010_frm1_lbl_tabTipoRegistro">

                        <div style={{ marginLeft: "20%", minHeight: "20vw", marginTop: "10vw" }} className="">
                            <FormButton id="btnCliente" className="mr-5 buttonBox" label="POR010_frm1_btn_Cliente" variant="outline-primary" />
                            <FormButton id="btnEmpresa" className="mr-5 buttonBox" label="POR010_frm1_btn_Empresa" variant="outline-primary" />
                            <FormButton id="btnParticular" className="mr-5 buttonBox" label="POR010_frm1_btn_Particular" variant="outline-primary" />
                        </div>

                    </FormTabStep>

                    <FormTabStep id="tabDatosVehiculo" label="POR010_frm1_lbl_tabDatosVehiculo">

                        <Row>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="CD_TRANSPORTE_V1">{t("POR010_frm1_lbl_CD_TRANSPORTE_V1")}</label>
                                    <FieldSelect name="CD_TRANSPORTE_V1" />
                                    <StatusMessage for="CD_TRANSPORTE_V1" />
                                </FormGroup>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="VL_MATRICULA_1_ENTRADA">{t("POR010_frm1_lbl_VL_MATRICULA_1_ENTRADA")}</label>
                                    <Field name="VL_MATRICULA_1_ENTRADA" onKeyPress={onKeyPressMatricula} />
                                    <StatusMessage for="VL_MATRICULA_1_ENTRADA" />
                                </FormGroup>


                            </Col>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="VL_MATRICULA_2_ENTRADA">{t("POR010_frm1_lbl_VL_MATRICULA_2_ENTRADA")}</label>
                                    <Field name="VL_MATRICULA_2_ENTRADA" />
                                    <StatusMessage for="VL_MATRICULA_2_ENTRADA" />
                                </FormGroup>
                            </Col>
                        </Row>

                        <Row>
                            <Col>
                                <FormGroup >
                                    <label htmlFor="NU_PREDIO">{t("POR010_frm1_lbl_NU_PREDIO")}</label>
                                    <FieldSelect name="NU_PREDIO" className={(tipoMotivo == "PORREGMOTBAL") ? "readOnly" : ""} />
                                    <StatusMessage for="NU_PREDIO" />
                                </FormGroup>
                            </Col>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="CD_SECTOR">{t("POR010_frm1_lbl_CD_SECTOR")}</label>
                                    <FieldSelect name="CD_SECTOR" />
                                    <StatusMessage for="CD_SECTOR" />
                                </FormGroup>
                            </Col>
                        </Row>

                        <Row hidden>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="VL_PESO_ENTRADA_VE">{t("POR010_frm1_lbl_VL_PESO_ENTRADA_VE")}</label>
                                    <Field name="VL_PESO_ENTRADA_VE" />
                                    <StatusMessage for="VL_PESO_ENTRADA_VE" />
                                </FormGroup>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <FormButton id="btnVolver" label="General_Sec0_btn_Volver" variant="link" />
                                &nbsp;
                                    <SubmitButton id="btnConfirmarVehiculo" variant="primary" label="General_Sec0_btn_Continuar" />
                            </Col>
                        </Row>


                    </FormTabStep>

                    <FormTabStep id="tabDatosGenerales" label="POR010_frm1_lbl_tabDatosGenerales">

                        <Row>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="CD_POTERIA_MOTIVO">{t("POR010_frm1_lbl_CD_POTERIA_MOTIVO")}</label>
                                    <FieldSelect name="CD_POTERIA_MOTIVO" className={(tr == rt.proverdor || tr == rt.asociado || tr == rt.particular) ? "readOnly" : ""} />
                                    <StatusMessage for="CD_POTERIA_MOTIVO" />
                                </FormGroup>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="CD_EMPRESA">{t("POR010_frm1_lbl_CD_EMPRESA")}</label>
                                    <FieldSelectAsync name="CD_EMPRESA" className={(tr == rt.particular || tipoMotivo == "PORREGMOTBAL") ? "readOnly" : ""} isClearable/>
                                    <StatusMessage for="CD_EMPRESA" />
                                </FormGroup>
                            </Col>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="CD_CLIENTE">{t("POR010_frm1_lbl_CD_CLIENTE")}</label>
                                    <FieldSelectAsync name="CD_CLIENTE" className={(tr == rt.particular || tipoMotivo == "PORREGMOTBAL") ? "readOnly" : ""} isClearable/>
                                    <StatusMessage for="CD_CLIENTE" />
                                </FormGroup>
                            </Col>
                        </Row>
                        <Row className={HiddenContainerClassName}>
                            <Col lg="6">
                                <Row>
                                    <Col lg="8">
                                        <FormGroup>
                                            <label htmlFor="NU_CONTAINER">{t("POR080_frm1_lbl_NU_CONTAINER")}</label>
                                            <FieldSelectAsync name="NU_CONTAINER" isClearable />
                                            <StatusMessage for="NU_CONTAINER" />
                                        </FormGroup>
                                    </Col>
                                    <Col lg="4">
                                        <FormGroup>
                                            <SubmitButton id="btnConfirmarContainer" className="mt-4 ml-2 mr-5" variant="primary" label="General_Sec0_btn_Agregar" />
                                        </FormGroup>
                                    </Col>
                                </Row>

                            </Col>
                            <Col lg="6">
                                <Grid id="POR080_grid_Containers" rowsToFetch={10} rowsToDisplay={10}
                                    onBeforeFetch={onBeforeFetch}
                                    onBeforeButtonAction={gridOnBeforeButtonAction}
                                    onAfterButtonAction={onAfterButtonAction}
                                />
                            </Col>
                        </Row>

                        <Row>
                            <Col>
                                <FormButton id="btnVolver" label="General_Sec0_btn_Volver" variant="link" />
                                &nbsp;
                                    <SubmitButton id="btnConfirmarGeneral" variant="primary" label="General_Sec0_btn_Continuar" />
                            </Col>
                        </Row>

                    </FormTabStep>

                    <FormTabStep id="tabAsociarContactos" label="POR010_frm1_lbl_tabAsociarContactos">
                        <Row>

                            <Col className={!isInsertContacto ? '' : 'hidden'}>
                                <FormGroup>
                                    <label htmlFor="NU_DOCUMENTO_SEARCH">{t("POR010_frm1_lbl_NU_DOCUMENTO_SEARCH")}</label>
                                    <FieldSelectAsync name="NU_DOCUMENTO_SEARCH" />
                                    <StatusMessage for="NU_DOCUMENTO_SEARCH" />
                                </FormGroup>
                            </Col>

                            <Col className={isInsertContacto ? '' : 'hidden'}>
                                <FormGroup>
                                    <label htmlFor="NU_DOCUMENTO">{t("POR010_frm1_lbl_NU_DOCUMENTO")}</label>
                                    <Field name="NU_DOCUMENTO" />
                                    <StatusMessage for="NU_DOCUMENTO" />
                                </FormGroup>
                            </Col>

                            <Col>
                                <FormGroup>
                                    <br />
                                    <Button id="btnModoBuscar" className={(isInsertContacto) ? "readOnly mt-2 ml-2 mr-5" : "mt-2 ml-2 mr-5"} variant="outline-primary" onClick={() => {
                                        setIsInsertContacto(!isInsertContacto);

                                    }} ><i className="fas fa-plus" /></Button>
                                    <Button id="btnModoAgregar" className={(!isInsertContacto) ? "readOnly mt-2 mr-5" : "mt-2 mr-5"} variant="outline-success" onClick={() => {
                                        setIsInsertContacto(!isInsertContacto);
                                    }} ><i className="fas fa-search" /></Button>
                                </FormGroup>
                            </Col>

                        </Row>
                        <Row className={isInsertContacto ? '' : 'hidden'}>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="NM_PERSONA">{t("POR010_frm1_lbl_NM_PERSONA")}</label>
                                    <Field name="NM_PERSONA" className={(isInsertContacto) ? "" : ""} />
                                    <StatusMessage for="NM_PERSONA" />
                                </FormGroup>
                            </Col>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="AP_PERSONA">{t("POR010_frm1_lbl_AP_PERSONA")}</label>
                                    <Field name="AP_PERSONA" className={(isInsertContacto) ? "" : ""} />
                                    <StatusMessage for="AP_PERSONA" />
                                </FormGroup>
                            </Col>
                            <Col hidden>
                                <FormGroup>
                                    <label htmlFor="NU_CELULAR">{t("POR010_frm1_lbl_NU_CELULAR")}</label>
                                    <Field name="NU_CELULAR" className={(isInsertContacto) ? "" : ""} />
                                    <StatusMessage for="NU_CELULAR" />
                                </FormGroup>
                            </Col>
                        </Row>

                        <Row>

                            <Col>
                                <SubmitButton id="btnConfirmarContacto" variant="primary" label="General_Sec0_btn_Agregar" />
                            </Col>
                        </Row>
                        <br />
                        <br />
                        <br />
                        <Row>
                            <Col>
                                <Grid id="POR080_grid_Personas" rowsToFetch={10} rowsToDisplay={10}
                                    onBeforeFetch={onBeforeFetch}
                                    onBeforeFetchStats={onBeforeFetch}
                                    onBeforeButtonAction={gridOnBeforeButtonAction}
                                    onAfterButtonAction={onAfterButtonAction}
                                />
                            </Col>
                        </Row>

                        <Row>
                            <Col>
                                <FormButton id="btnVolver" label="General_Sec0_btn_Volver" variant="link" />
                                &nbsp;
                                    <SubmitButton id="btnConfirmar" variant="primary" label="General_Sec0_btn_Confirmar" />
                            </Col>
                        </Row>
                    </FormTabStep>

                </FormTab>

            </Form>

            <br />
            <div className={GridShowClassName}>
                <div className="col-12">

                    <Grid id="POR080_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport

                    />
                </div>
            </div>

        </Page >
    );
}

