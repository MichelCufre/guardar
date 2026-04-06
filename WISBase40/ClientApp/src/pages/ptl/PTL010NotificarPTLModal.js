import React, { useState, useLayoutEffect, useEffect, useRef } from 'react';
import { Modal, Button, Row, Col } from 'react-bootstrap';
import { Form, Field, FieldSelect, FieldCheckbox, FieldDateTime, FieldSelectAsync, SubmitButton, StatusMessage, FieldDate } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { isUndefined, isNullOrUndefined } from 'util';

export function PTL010NotificarPTLModal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });
    const [nexus, setNexus] = useState(null);
    const [keyRowSelectedVlComparteContPicking, setKeyRowSelectedVlComparteContPicking] = useState(null);
    const [keyRowSelectedVlComparteContPickingOnlyOneRow, setKeyRowSelectedVlComparteContPickingOnlyOneRow] = useState(null);
    const [keyRowSelectedSubClase, setKeyRowSelectedSubClase] = useState(null);
    const [keyRowSelectedAgrupacionContenedorLeido, setKeyRowSelectedAgrupacionContenedorLeido] = useState(null);
    const [keyColorPtl, setKeyColorPtl] = useState(null);
    const [keyColorCodePtl, setKeyColorCodePtl] = useState(null);
    const [userAutenticado, setUserAutenticado] = useState(null);


    const initialValues = {
        contenedor: ""
    };

    const validationSchema = {
        contenedor: Yup.string().required()
    };

    const handleClose = () => {
        LimpiarConstantesSession();
        props.onHide();
    };

    //*********************************************************************************************
    //                                         BEFORE EVENTS
    //*********************************************************************************************
    const onBeforeInitializeGrid = (context, data, nexus) => {
        addParameters(context, data, nexus);
    };
    const onBeforeInitializeForm = (context, form, data, nexus) => {

        LimpiarConstantesSession();
        addParameters(context, data, nexus);
    }
    const onBeforeValidateFieldForm = (context, form, data, nexus) => {

        addParameters(context, data, nexus);
    }
    const onBeforeSubmit = (context, form, data, nexus) => {
        addParameters(context, data, nexus);
    }
    const onBeforeButtonActionForm = (context, form, query, nexus) => {
        addParameters(context, query, nexus);
    }
    const onBeforeFetchGrid = (context, data, nexus) => {
        addParameters(context, data, nexus);
    }
    const onBeforeApplySortGrid = (context, data, nexus) => {
        addParameters(context, data, nexus);
    }
    const onBeforeFetchStatsGrid = (context, data, nexus) => {
        addParameters(context, data, nexus);
    }
    const onBeforeApplyFilterGrid = (context, data, nexus) => {
        addParameters(context, data, nexus);
    }
    const onBeforeButtonActionGrid = (context, data, nexus) => {
        addParameters(context, data, nexus);
    }
    const onBeforeExportExcelGrid = (context, data, nexus) => {
        addParameters(context, data, nexus);
    }

    //*********************************************************************************************


    //*********************************************************************************************
    //                                         AFTER EVENTS
    //*********************************************************************************************
    const onAfterInitializeForm = (context, form, data, nexus) => {
        setNexus(nexus);
        setFocusField("contenedor");
    }
    const onAfterSubmitForm = (context, form, data, nexus) => {

        if (context.responseStatus !== "OK")
            return;

        if (data.buttonId === "SubmitContenedor") {
            if (data.parameters.some(x => x.id == "SOLICITAR_CREDENCIALES_USUARIO") &&
                data.parameters.find(x => x.id == "SOLICITAR_CREDENCIALES_USUARIO").value === "S") {
                nexus.showConfirmation({
                    message: "AUTORIZATE",
                    onAccept: (userId) => onAcceptConfirmation(nexus, userId)
                });
            }

            document.getElementById("btnSubmitIniciar").focus();
            return;
        }

        if (data.buttonId === "btnSubmitIniciar") {
            props.onHide();
        }

    };
    const onAfterButtonActionForm = (context, form, data, nexus) => {
        //Pone el color obtenido del PTL
        if (data.parameters.some(x => x.id == "PTL010_USER_COLOR")) {

            var colour = data.parameters.find(x => x.id == "PTL010_USER_COLOR").value;
            var userColourField = document.getElementById("userColour");

            userColourField.innerHTML = "<span style='color: " + colour + "; filter: invert(100%)'>" + t("PTL010NotificarPTL_frm1_lbl_ColorPTL") + "</span>";
            userColourField.style.backgroundColor = colour;
            userColourField.style.width = "100%";
            userColourField.style.textAlign = "center";

            var colorCode = data.parameters.find(x => x.id == "PTL010_USER_COLOR_CODE").value;

            setKeyColorPtl(colour);
            setKeyColorCodePtl(colorCode);
            setUserAutenticado(data.parameters.find(x => x.id == "userId").value);

            document.getElementById("btnSubmitIniciar").focus();
            return;
        }
    };
    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.some(x => x.id == "GRID_VL_COMP_CONT_PICK_PROD_ROW_ONLY_ONE_ROW")) {

            var value = parameters.find(x => x.id == "GRID_VL_COMP_CONT_PICK_PROD_ROW_ONLY_ONE_ROW").value;

            if (value === "S") {

                if (parameters.some(x => x.id == "GRID_VL_COMP_CONT_PICK_PROD_ROW_SELECTED")) {
                    setKeyRowSelectedVlComparteContPicking(parameters.find(x => x.id == "GRID_VL_COMP_CONT_PICK_PROD_ROW_SELECTED").value);
                    setKeyRowSelectedVlComparteContPickingOnlyOneRow(parameters.find(x => x.id == "GRID_VL_COMP_CONT_PICK_PROD_ROW_ONLY_ONE_ROW").value);
                    nexus.getGrid("PTL010NotPTL_grid_SubClase").refresh();
                }
            }
        }
    };
    const onAfterButtonAction = (data, nexus) => {

        debugger;
        if (data.parameters.some(x => x.id == "GRID_VL_COMP_CONT_PICK_PROD_ROW_SELECTED")) {

            if (data.parameters.some(x => x.id == "GRID_VL_COMP_CONT_PICK_PROD_ROW_ONLY_ONE_ROW")) {
                if (data.parameters.find(x => x.id == "GRID_VL_COMP_CONT_PICK_PROD_ROW_ONLY_ONE_ROW").value === "S") {
                    setKeyRowSelectedVlComparteContPicking(data.parameters.find(x => x.id == "GRID_VL_COMP_CONT_PICK_PROD_ROW_SELECTED").value);
                }
                else {
                    setKeyRowSelectedVlComparteContPicking(data.parameters.find(x => x.id == "GRID_VL_COMP_CONT_PICK_PROD_ROW_SELECTED").value);
                    nexus.getGrid("PTL010NotPTL_grid_VlComparte").refresh();
                    debugger;
                    setTimeout(() => {
                        nexus.getGrid("PTL010NotPTL_grid_SubClase").reset();

                    }, 1000);
                }
            }
            else {
                setKeyRowSelectedVlComparteContPicking(data.parameters.find(x => x.id == "GRID_VL_COMP_CONT_PICK_PROD_ROW_SELECTED").value);
                nexus.getGrid("PTL010NotPTL_grid_SubClase").refresh();
            }
        }

        if (data.parameters.some(x => x.id == "GRID_SUBCLASE_PROD_ROW_SELECTED")) {
            setKeyRowSelectedSubClase(data.parameters.find(x => x.id == "GRID_SUBCLASE_PROD_ROW_SELECTED").value);
            nexus.getGrid("PTL010NotPTL_grid_SubClase").refresh();
        }

    };
    const onAfterValidateField = (context, form, query, nexus) => {

        if (query.parameters.some(x => x.id == "AGRUPACION_CONTENEDOR_LEIDO")) {
            var json = query.parameters.find(x => x.id == "AGRUPACION_CONTENEDOR_LEIDO").value;
            setKeyRowSelectedAgrupacionContenedorLeido(json);
            nexus.getGrid("PTL010NotPTL_grid_VlComparte").refresh();
            nexus.getGrid("PTL010NotPTL_grid_SubClase").refresh();
        }
    }
    const onAfterFetchGrid = (context, newRows, parameters, nexus) => {


        if (!isNullOrUndefined(parameters.find(x => x.id == "GRID_VL_COMP_CONT_PICK_PROD_ROW_ONLY_ONE_ROW").value)) {
            setKeyRowSelectedVlComparteContPickingOnlyOneRow(parameters.find(x => x.id == "GRID_VL_COMP_CONT_PICK_PROD_ROW_ONLY_ONE_ROW").value);
        }

        if (!isNullOrUndefined(parameters.find(x => x.id == "GRID_VL_COMP_CONT_PICK_PROD_ROW_SELECTED").value)) {
            setKeyRowSelectedVlComparteContPicking(parameters.find(x => x.id == "GRID_VL_COMP_CONT_PICK_PROD_ROW_SELECTED").value);
        }

        if (!isNullOrUndefined(parameters.find(x => x.id == "GRID_SUBCLASE_PROD_ROW_SELECTED").value)) {
            setKeyRowSelectedSubClase(parameters.find(x => x.id == "GRID_SUBCLASE_PROD_ROW_SELECTED").value);
        }

    }

    //*********************************************************************************************

    //*********************************************************************************************
    //                                      AUXILIARIES FUNCTIONS
    //*********************************************************************************************
    const setFocusField = (fieldName) => {

        document.getElementsByName(fieldName)[0].focus();
        document.getElementsByName(fieldName)[0].readOnly = false;

        var inputs = document.getElementsByClassName("pepe");
        for (var i = 0; i < inputs.length; i++) {

            if (inputs.item(i).name != fieldName) {
                inputs.item(i).readOnly = true;
            }
        }


    }
    const addParameters = (context, data, nexus) => {

        data.parameters = data.parameters.concat([
            { id: "preparacion", value: props.preparacion },
            { id: "empresa", value: props.empresa },
            { id: "cliente", value: props.cliente },
            { id: "numeroAutomatismo", value: props.numeroAutomatismo },
            { id: "GRID_VL_COMP_CONT_PICK_PROD_ROW_SELECTED", value: keyRowSelectedVlComparteContPicking },
            { id: "GRID_SUBCLASE_PROD_ROW_SELECTED", value: keyRowSelectedSubClase },
            { id: "GRID_VL_COMP_CONT_PICK_PROD_ROW_ONLY_ONE_ROW", value: keyRowSelectedVlComparteContPickingOnlyOneRow },
            { id: "AGRUPACION_CONTENEDOR_LEIDO", value: keyRowSelectedAgrupacionContenedorLeido },
            { id: "PTL010_USER_COLOR", value: keyColorPtl },
            { id: "PTL010_USER_COLOR_CODE", value: keyColorCodePtl }
        ]);

        if (!data.parameters.some(x => x.id == "userId")) {
            data.parameters = data.parameters.concat([{ id: "userId", value: userAutenticado }]);
        }

    };
    const onAcceptConfirmation = (nexus, userId) => {
        nexus.getForm("PTL010NotificarPTL_form_1").clickButton("UserConfirmation", null, [
            { id: "userId", value: userId }
        ]);
    }

    const LimpiarConstantesSession = () => {
        setKeyRowSelectedVlComparteContPicking(null);
        setKeyRowSelectedVlComparteContPickingOnlyOneRow(null);
        setKeyRowSelectedSubClase(null);
        setKeyRowSelectedAgrupacionContenedorLeido(null);
        setKeyColorPtl(null);
        setUserAutenticado(null);
    }

    //*********************************************************************************************


    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Form
                id="PTL010NotificarPTL_form_1"
                application="PTL010NotificarPTL"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={onBeforeInitializeForm}
                onAfterInitialize={onAfterInitializeForm}
                onAfterSubmit={onAfterSubmitForm}
                onBeforeValidateField={onBeforeValidateFieldForm}
                onAfterValidateField={onAfterValidateField}
                onBeforeSubmit={onBeforeSubmit}
                onBeforeButtonAction={onBeforeButtonActionForm}
                onAfterButtonAction={onAfterButtonActionForm}

            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("PTL010NotificarPTL_Sec0_modalTitle_NotificarPTL")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col className="d-flex justify-content-center">
                            <div id="userColour" class="btn-sm"></div>
                        </Col>
                    </Row>
                    <hr />
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="contenedor">{t("PTL010NotificarPTL_frm1_lbl_CONTENEDOR")}</label>
                                <Field className="pepe" name="contenedor" onKeyPress={(event) => {
                                    if (event.key === "Enter") {
                                        nexus.getForm("PTL010NotificarPTL_form_1").submit("SubmitContenedor");
                                    }
                                }} />
                                <StatusMessage for="contenedor" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col lg="6">
                            <fieldset className="form-group border p-2 grid" >
                                <legend align="center" className="w-auto">{t("PTL010NotificarPTL_grid1_title_AgrupacionVlComparteContenedor")}</legend>
                                <Row>
                                    <Col>
                                        <Grid id="PTL010NotPTL_grid_VlComparte"
                                            rowsToFetch={10}
                                            rowsToDisplay={5}
                                            enableExcelExport
                                            application="PTL010NotiPTLVlComparte"
                                            onBeforeFetch={onBeforeFetchGrid}
                                            onBeforeInitialize={onBeforeInitializeGrid}
                                            onBeforeApplySort={onBeforeApplySortGrid}
                                            onBeforeApplyFilter={onBeforeApplyFilterGrid}
                                            onBeforeFetchStats={onBeforeFetchStatsGrid}
                                            onBeforeExportExcel={onBeforeExportExcelGrid}
                                            onBeforeButtonAction={onBeforeButtonActionGrid}
                                            onAfterInitialize={onAfterInitialize}
                                            onAfterFetch={onAfterFetchGrid}
                                            onAfterButtonAction={onAfterButtonAction}
                                        />
                                    </Col>
                                </Row>
                            </fieldset>

                        </Col>
                        <Col lg="6">
                            <fieldset className="form-group border p-2 grid" >
                                <legend align="center" className="w-auto">{t("PTL010NotificarPTL_grid1_title_SubClase")}</legend>

                                <Row>
                                    <Col>
                                        <Grid id="PTL010NotPTL_grid_SubClase"
                                            rowsToFetch={10}
                                            rowsToDisplay={5}
                                            enableExcelExport
                                            application="PTL010NotiPTLSubClasse"
                                            onBeforeInitialize={onBeforeInitializeGrid}
                                            onBeforeFetch={onBeforeFetchGrid}
                                            onBeforeFetchStats={onBeforeFetchStatsGrid}
                                            onBeforeApplySort={onBeforeApplySortGrid}
                                            onBeforeApplyFilter={onBeforeApplyFilterGrid}
                                            onBeforeExportExcel={onBeforeExportExcelGrid}
                                            onBeforeButtonAction={onBeforeButtonActionGrid}
                                            onAfterFetch={onAfterFetchGrid}
                                            onAfterButtonAction={onAfterButtonAction}
                                        />
                                    </Col>
                                </Row>
                            </fieldset>

                        </Col>

                    </Row>

                </Modal.Body>
                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleClose}>
                        {t("PTL010NotificarPTL_frm1_btn_cancelar")}
                    </Button>
                    <SubmitButton id="btnSubmitIniciar" variant="primary" label={t("PTL010NotificarPTL_frm1_btn_Iniciar")} />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}