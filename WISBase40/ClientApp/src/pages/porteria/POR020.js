import React, { useState, useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldCheckbox, FieldSelect, FieldSelectAsync, FieldDate, SubmitButton, FormButton, StatusMessage, FieldDateTime } from '../../components/FormComponents/Form';
import { Row, Col, FormGroup, Button } from 'react-bootstrap';
import { FormTab, FormTabStep } from '../../components/FormComponents/FormTab';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import { notificationType } from '../../components/Enums';

export default function POR020(props) {
    const { t } = useTranslation();


    const [currentTab, setCurrentTab] = useState("tabTipoSalida");

    const [isShowForm, setIsShowForm] = useState(false);
    const FormShowClassName = "";

    const [isConfirm, setIsConfirm] = useState(true);
    const [isVehiculo, setIsVehiculo] = useState(false);
    const [isHiddenContainers, setIsHiddenContainers] = useState(true);
    const HiddenContainersClassName = isHiddenContainers ? "hidden" : "";

    const [SelectionGridPersonas, setSelectionGridPersonas] = useState("[]");
    const [notSelectionGridPersonas, setNotSelectionGridPersonas] = useState("[]"); //???

    const [SelectionGridContainers, setSelectionGridContainers] = useState(null);

    const HideForm = (evt) => {
        setIsShowForm(false);
        evt.preventDefault();
    }

    const formClassName = "";




    const goToFin = () => {
        setCurrentTab("tabFin");
    };

    const goToVehiculo = (evt) => {

        setCurrentTab("tabVehiculo");
        evt.preventDefault();
    };


    const onTabClose = (evt) => {
        HideForm(evt);
    };

    const addParameters = (context, data, nexus) => {

        let nform = nexus.getForm("POR020_form_1");

        data.parameters = [
            {
                id: "NU_PORTERIA_VEHICULO",
                value: nform.getFieldValue("NU_PORTERIA_VEHICULO") || "-1"
            },
            {
                id: "FL_CON_VEHICULO",
                value: isVehiculo ? "S" : "M"
            },
            {
                id: "FL_CON_CONTENEDORES",
                value: nform.getFieldValue("FL_CON_CONTENEDORES") || "-1"
            },
            { id: "SelectionGridPersonas", value: SelectionGridPersonas },
            { id: "notSelectionGridPersonas", value: notSelectionGridPersonas },
            { id: "SelectionGridContainers", value: SelectionGridContainers },
        ];
    };

    const onBeforeButtonAction = (context, form, query, nexus) => {

        context.abortServerCall = true;

        if (query.buttonId === "btnGoVehiculo") {

            if (form.fields.find(w => w.id == "FL_CON_CONTENEDORES").value == "S") {
                setIsHiddenContainers(false);
            }
            else {
                setIsHiddenContainers(true);
            }

            nexus.getGrid("POR020_grid_Personas").reset();
            nexus.getGrid("POR020_grid_Containers").refresh();

            goToFin();

        }
        else if (query.buttonId === "btnFinalizar") {
            nexus.getGrid("POR020_grid_Personas").triggerMenuAction("btnConfirmarMenuItem");
        }
        else if (query.buttonId === "btnConfirmarContacto") {

            context.abortServerCall = false;

            query.parameters = [
                { id: "SelectionGridPersonas", value: SelectionGridPersonas },
                { id: "notSelectionGridPersonas", value: notSelectionGridPersonas },
            ];

        }
        else if (query.buttonId === "btnVehiculo") {
            setIsVehiculo(true);
            setCurrentTab("tabVehiculo");
            context.forceUpdateFields = true;

            formOnClearFields(form);

        }
        else if (query.buttonId === "btnPersona") {
            setIsVehiculo(false);
            setCurrentTab("tabFin");
            setSelectionGridPersonas("[]");
            setNotSelectionGridPersonas("[]");
            setSelectionGridContainers("[]");
            setIsHiddenContainers(true);

            formOnClearFields(form);

            nexus.getGrid("POR020_grid_Personas").refresh();

            context.forceUpdateFields = true;

        }
        else if (query.buttonId === "btnVolver") {

            if (currentTab == "tabFin") {

                if (isVehiculo) {
                    setCurrentTab("tabVehiculo");
                }
                else {
                    setCurrentTab("tabTipoSalida");
                }

            }
            else if (currentTab == "tabVehiculo") {
                context.forceUpdateFields = true;

                setSelectionGridPersonas("[]");
                setNotSelectionGridPersonas("[]");
                formOnClearFields(form);
                setCurrentTab("tabTipoSalida");
            }

        }

    };

    const onAfterButtonAction = (context, form, query, nexus) => {

        if (query.buttonId === "btnConfirmarContacto") {

            setSelectionGridPersonas(query.parameters.find(w => w.id === "SelectionGridPersonas").value);
            setNotSelectionGridPersonas(query.parameters.find(w => w.id === "notSelectionGridPersonas").value);

            nexus.getGrid("POR020_grid_Personas").refresh();
        }

    };

    const onAfterMenuItemAction = (context, data, nexus) => {
        context.abortUpdate = true;

        setCurrentTab("tabTipoSalida");
        setSelectionGridPersonas("[]");
        setNotSelectionGridPersonas("[]");
        setSelectionGridContainers("[]");

    };

    const onBeforeValidateField = (context, form, query, nexus) => {

        if (query.fieldId == "FL_CON_CONTENEDORES") {

            context.abortServerCall = true;

        }
    };

    const onAfterValidateField = (context, form, query, nexus) => {

        if (query.fieldId == "VL_MATRICULA_1" || query.fieldId == "VL_DOCUMENTO") {

            let tempNuPorteriaVehiculo = form.fields.find(w => w.id == "NU_PORTERIA_VEHICULO").value;

            if (tempNuPorteriaVehiculo == "") {
                setSelectionGridPersonas("[]");
                setNotSelectionGridPersonas("[]");

                let field = form.fields.find(w => w.id == query.fieldId);

                if (field.value != "") {

                    field.error = {
                        message: "POR020_frm1_Error_Error1",
                    };
                    field.status = 2;
                }

            }

            setSelectionGridContainers(query.parameters.find(w => w.id == "SelectionGridContainers").value)
            nexus.getGrid("POR020_grid_Containers").refresh();

            setIsConfirm(tempNuPorteriaVehiculo == "");
        }
    }

    const onBeforeSubmit = (context, form, query, nexus) => {

        context.abortServerCall = true;

     };

    const onKeyPressMatriculaOrDocumento = (e) => {

        var key = e.keyCode || e.which;
        if (key == 13) {
            document.getElementsByName("VL_MATRICULA_2")[0].focus();
        }

    }

    const formOnClearFields = (form) => {

        form.fields.find(w => w.id == "NU_PORTERIA_VEHICULO").value = "";
        form.fields.find(w => w.id == "VL_MATRICULA_1").value = "";
        form.fields.find(w => w.id == "VL_MATRICULA_2").value = "";
        //form.fields.find(w => w.id == "VL_DOCUMENTO").value = "";

        form.fields.find(w => w.id == "VL_PESO_ENTRADA").value = "";

        form.fields.find(w => w.id == "DS_SECTOR").value = "";
        form.fields.find(w => w.id == "DS_POTERIA_MOTIVO").value = "";
        form.fields.find(w => w.id == "NM_AGENTE").value = "";
        form.fields.find(w => w.id == "NM_EMPRESA").value = "";
        form.fields.find(w => w.id == "DT_PORTERIA_ENTRADA").value = "";
    }

    const validationSchema = {

        VL_MATRICULA_1: Yup.string().nullable().max(30),
        FL_CON_CONTENEDORES: Yup.string(),
        VL_DOCUMENTO: Yup.string().nullable().max(15),
        NU_DOCUMENTO_SEARCH: Yup.string(),
    };

    const gridOnBeforeButtonAction = (context, data, nexus) => {

        if (data.gridId == "POR020_grid_Personas") {
            data.parameters = [
                { id: "SelectionGridPersonas", value: SelectionGridPersonas },
                { id: "notSelectionGridPersonas", value: notSelectionGridPersonas },
            ];
        }
        else if (data.gridId == "POR020_grid_Containers") {
            data.parameters = [
                { id: "SelectionGridContainers", value: SelectionGridContainers },
            ];
        }

    }

    const gridOnAfterButtonAction = (data, nexus) => {


        if (data.gridId == "POR020_grid_Personas") {
            setSelectionGridPersonas(data.parameters.find(w => w.id === "SelectionGridPersonas").value);
            setNotSelectionGridPersonas(data.parameters.find(w => w.id === "notSelectionGridPersonas").value);

            nexus.getGrid("POR020_grid_Personas").refresh();
        }
        else if (data.gridId == "POR020_grid_Containers") {

            setSelectionGridContainers(data.parameters.find(w => w.id === "SelectionGridContainers").value);

            nexus.getGrid("POR020_grid_Containers").refresh();
        }


    }

    const onBeforeFetch = (context, data, nexus) => {

        if (data.gridId == "POR020_grid_Containers") {
            data.parameters = [
                {
                    id: "SelectionGridContainers",
                    value: SelectionGridContainers
                }
            ];
        }
    };

    return (

        <Page
            title={t("POR020_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <Form
                id="POR020_form_1"
                validationSchema={validationSchema}
                onBeforeButtonAction={onBeforeButtonAction}
                onAfterButtonAction={onAfterButtonAction}
                onBeforeSubmit={onBeforeSubmit}
                onBeforeValidateField={onBeforeValidateField}
                onAfterValidateField={onAfterValidateField}
            >

                <Field name="VL_PESO_ENTRADA" hidden />

                <div className={formClassName}>
                    <Row>
                        <Col>
                            <FormTab
                                current={currentTab}
                            >

                                <FormTabStep id="tabTipoSalida" label="POR020_frm1_lbl_tabTipoSalida">

                                    <div style={{ marginLeft: "30%", minHeight: "20vw", marginTop: "10vw" }} className="">
                                        <FormButton id="btnVehiculo" className="mr-5 buttonBox" label="POR020_frm1_btn_Vehiculo" variant="outline-primary" />
                                        <FormButton id="btnPersona" className="mr-5 buttonBox" label="POR020_frm1_btn_Persona" variant="outline-primary" />
                                    </div>

                                </FormTabStep>

                                <FormTabStep id="tabVehiculo" label="General_Sec_0lbl_tabDatos">
                                    <div className={FormShowClassName}>
                                        <Row>
                                            <Col>
                                                <FormGroup>
                                                    <label htmlFor="VL_MATRICULA_1">{t("POR020_frm1_lbl_VL_MATRICULA_1")}</label>
                                                    <Field name="VL_MATRICULA_1" id="VL_MATRICULA_1" onKeyPress={onKeyPressMatriculaOrDocumento} />
                                                    <StatusMessage for="VL_MATRICULA_1" />
                                                </FormGroup>
                                            </Col>
                                            <Col>
                                                <FormGroup>
                                                    <label htmlFor="FL_CON_CONTENEDORES">{t("POR020_frm1_lbl_FL_CON_CONTENEDORES")}</label>
                                                    <FieldSelect name="FL_CON_CONTENEDORES" />
                                                    <StatusMessage for="FL_CON_CONTENEDORES" />
                                                </FormGroup>
                                            </Col>
                                            <Col hidden >
                                                <FormGroup>
                                                    <label htmlFor="NU_PORTERIA_VEHICULO">{t("POR020_frm1_lbl_NU_PORTERIA_VEHICULO")}</label>
                                                    <Field name="NU_PORTERIA_VEHICULO" readOnly />
                                                    <StatusMessage for="NU_PORTERIA_VEHICULO" />
                                                </FormGroup>
                                            </Col>
                                        </Row>
                                        <hr />
                                        <Row>
                                            <Col>
                                                <FormGroup>
                                                    <label htmlFor="VL_MATRICULA_2">{t("POR020_frm1_lbl_VL_MATRICULA_2")}</label>
                                                    <Field name="VL_MATRICULA_2" readOnly />
                                                    <StatusMessage for="VL_MATRICULA_2" />
                                                </FormGroup>
                                            </Col>
                                            <Col>
                                                <FormGroup>
                                                    <label htmlFor="DS_SECTOR">{t("POR020_frm1_lbl_DS_SECTOR")}</label>
                                                    <Field name="DS_SECTOR" readOnly />
                                                    <StatusMessage for="DS_SECTOR" />
                                                </FormGroup>
                                            </Col>
                                            <Col>
                                                <FormGroup>
                                                    <label htmlFor="DS_POTERIA_MOTIVO">{t("POR020_frm1_lbl_DS_POTERIA_MOTIVO")}</label>
                                                    <Field name="DS_POTERIA_MOTIVO" readOnly />
                                                    <StatusMessage for="DS_POTERIA_MOTIVO" />
                                                </FormGroup>
                                            </Col>

                                        </Row>
                                        <Row>
                                            <Col>
                                                <FormGroup>
                                                    <label htmlFor="NM_AGENTE">{t("POR020_frm1_lbl_NM_AGENTE")}</label>
                                                    <Field name="NM_AGENTE" readOnly />
                                                    <StatusMessage for="NM_AGENTE" />
                                                </FormGroup>
                                            </Col>
                                            <Col>
                                                <FormGroup>
                                                    <label htmlFor="NM_EMPRESA">{t("POR020_frm1_lbl_NM_EMPRESA")}</label>
                                                    <Field name="NM_EMPRESA" readOnly />
                                                    <StatusMessage for="NM_EMPRESA" />
                                                </FormGroup>
                                            </Col>
                                            <Col>
                                                <FormGroup>
                                                    <label htmlFor="DT_PORTERIA_ENTRADA">{t("POR020_frm1_lbl_DT_PORTERIA_ENTRADA")}</label>
                                                    <FieldDateTime name="DT_PORTERIA_ENTRADA" readOnly />
                                                    <StatusMessage for="DT_PORTERIA_ENTRADA" />
                                                </FormGroup>
                                            </Col>
                                        </Row>

                                        <Row>
                                            <Col>
                                                <FormButton id="btnVolver" variant="link" label="General_Sec0_btn_Volver" />
                                                 &nbsp;
                                                <FormButton id="btnGoVehiculo" label="General_Sec0_btn_Siguiente" className={(isConfirm) ? "readOnly" : ""} />
                                            </Col>
                                        </Row>

                                    </div>

                                </FormTabStep>
                                <FormTabStep id="tabFin" label="POR020_Sec0_lbl_Personas">

                                    <Row>

                                        <Col>
                                            <FormGroup>
                                                <label htmlFor="NU_DOCUMENTO_SEARCH">{t("POR010_frm1_lbl_NU_DOCUMENTO_SEARCH")}</label>
                                                <FieldSelectAsync name="NU_DOCUMENTO_SEARCH" />
                                                <StatusMessage for="NU_DOCUMENTO_SEARCH" />
                                            </FormGroup>
                                        </Col>
                                        <Col>
                                            <FormGroup>
                                                <br />
                                                <FormButton id="btnConfirmarContacto" variant="primary" label="General_Sec0_btn_Agregar" />
                                            </FormGroup>
                                        </Col>
                                        <Col className={HiddenContainersClassName}>
                                            <Grid id="POR020_grid_Containers" rowsToFetch={10} rowsToDisplay={10}
                                                onBeforeFetch={onBeforeFetch}
                                                onBeforeFetchStats={onBeforeFetch}
                                                onBeforeButtonAction={gridOnBeforeButtonAction}
                                                onAfterButtonAction={gridOnAfterButtonAction}
                                            />
                                        </Col>
                                    </Row>
                                    <hr />
                                    <br />
                                    <br />
                                    <Row>
                                        <Col>
                                            <div className="row mb-4">
                                                <div className="col-12">
                                                    <Grid id="POR020_grid_Personas" rowsToFetch={5} rowsToDisplay={15}
                                                        onBeforeInitialize={addParameters}
                                                        onBeforeFetch={addParameters}
                                                        onBeforeFetchStats={addParameters}
                                                        onBeforeApplyFilter={addParameters}
                                                        onBeforeApplySort={addParameters}
                                                        onBeforeMenuItemAction={addParameters}
                                                        onAfterMenuItemAction={onAfterMenuItemAction}
                                                        onBeforeButtonAction={gridOnBeforeButtonAction}
                                                        onAfterButtonAction={gridOnAfterButtonAction}
                                                    />
                                                </div>
                                            </div>

                                        </Col>
                                    </Row>

                                    <Row>
                                        <Col>
                                            <FormButton id="btnVolver" variant="link" label="General_Sec0_btn_Volver" />
                                            &nbsp;
                                            <FormButton id="btnFinalizar" label="General_Sec0_btn_Confirmar" />
                                        </Col>
                                    </Row>
                                </FormTabStep>
                            </FormTab>


                        </Col>
                    </Row>
                </div>
            </Form>
        </Page >
    );
}
