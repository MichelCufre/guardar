import React, { useState } from 'react';
import { Form, Field, FieldSelect, SubmitButton, FieldNumber, StatusMessage, FormButton, FieldToggle } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import * as Yup from 'yup';

export function AUT100Modal(props) {

    const { t } = useTranslation();

    const [keyTab, setKeyTab] = useState(null);

    const [automatismoObj, setAutomatismo] = useState(null);

    const [mostrarUbicacionesFieldset, setMostrarUbicacionesFieldset] = useState(true);

    const [isUpdate, setIsUpdate] = useState(false);

    const [isTabPosicionesDisabled, setIsTabPosicionesDisabled] = useState(true);

    const [isTabInterfacesDisabled, setIsTabInterfacesDisabled] = useState(true);

    const [isTabPuestosDisabled, setIsTabPuestosDisabled] = useState(true);

    const [isTabCaracteristicasDisabled, setIsTabCaracteristicasDisabled] = useState(true);

    const [showRestablecerBtn, setShowRestablecerBtn] = useState(false);

    //---------------TAB GENERAL---------------

    const validationSchemaForm1 = {
        numeroAutomatismo: Yup.string(),
        codigoExterno: Yup.string(),
        tipo: Yup.string().required(),
        predio: Yup.string().required(),
        descripcion: Yup.string().required(),
        zonaUbicacion: Yup.string(),
        qtPicking: Yup.string(),
        qtEntrada: Yup.string(),
        qtAjuste: Yup.string(),
        qtRechazo: Yup.string(),
        qtTransito: Yup.string(),
        qtSalida: Yup.string(),
        isEnabled: Yup.boolean(),
    };

    const initialValues = {
        numeroAutomatismo: "",
        codigoExterno: "",
        tipo: "",
        predio: "",
        descripcion: "",
        zonaUbicacion: "",
        qtPicking: "",
        qtEntrada: "",
        qtAjuste: "",
        qtRechazo: "",
        qtTransito: "",
        qtSalida: "",
        isEnabled: true,
    };

    const handleClose = () => {
        setIsUpdate(false);
        setAutomatismo(null);
        setKeyTab(null);
        props.onHide();
    };

    const onBeforeInitialize = (context, form, data, nexus) => {
        setKeyTab("general");
        setIsTabPosicionesDisabled(true);
        setIsTabInterfacesDisabled(true);
        setIsTabPuestosDisabled(true);
        setIsTabCaracteristicasDisabled(true);
        setMostrarUbicacionesFieldset(true);
        setShowRestablecerBtn(false);

        if (props.isUpdate) {
            data.parameters.push({ id: "IS_UPDATE", value: "S" });
            
            setAutomatismo(props.automatismo);
            form.fields.find(w => w.id == "numeroAutomatismo").value = props.automatismo;
        }
        else if (isUpdate) {
            data.parameters.push({ id: "IS_UPDATE", value: "S" });
            form.fields.find(w => w.id == "numeroAutomatismo").value = automatismoObj;
        }
    }

    const onAfterInitialize = (context, form, query, nexus) => {

        if (query.parameters.some(x => x.id == "AUT100_POSICIONES_CREADAS")) {
            var value = query.parameters.find(x => x.id == "AUT100_POSICIONES_CREADAS").value;

            if (value === "S") {
                setMostrarUbicacionesFieldset(false);

                setIsTabPosicionesDisabled(false);
                nexus.getGrid("AUT100Modal_grid_1").reset();
            }
        }

        if (query.parameters.some(x => x.id == "AUT100_INTERFACES_CREADAS")) {
            var value = query.parameters.find(x => x.id == "AUT100_INTERFACES_CREADAS").value;

            if (value === "S") {
                setIsTabInterfacesDisabled(false);
                nexus.getGrid("AUT100Modal_grid_2").reset();
            }
        }

        if (query.parameters.some(x => x.id == "AUT100_CARACTERISTICAS_CREADAS")) {
            var value = query.parameters.find(x => x.id == "AUT100_CARACTERISTICAS_CREADAS").value;

            if (value === "S") {
                setIsTabCaracteristicasDisabled(false);
                nexus.getGrid("AUT100Modal_grid_3").reset();
                nexus.getGrid("AUT100Modal_grid_4").reset();
            }
        }

        if (query.parameters.some(x => x.id == "SHOW_GRID_PUESTOS")) {
            var value = query.parameters.find(x => x.id == "SHOW_GRID_PUESTOS").value;

            if (value == "S") {
                setIsTabPuestosDisabled(false);

                var params = query.parameters;

                nexus.getGrid("AUT100Modal_grid_5").reset(params);
            }
        }
    };

    const onBeforeButtonAction = (context, form, query, nexus) => {
        debugger;
        if (query.buttonId === "restablecerValoresBtn") {
            context.abortServerCall = true;
            nexus.showConfirmation({
                message: "General_Sec0_Info_DeseaGuardarCambios",
                onAccept: () => nexus.getForm("AUT100Modal_form_1").clickButton("reestablecerConfirmado"),
                onCancel: () => context.abortServerCall = true,
            });

        } else if (query.buttonId === "reestablecerConfirmado") {
            
            query.buttonId = "restablecerValoresBtn";
            query.parameters.push({ id: "AUT100_NU_AUTOMATISMO", value: automatismoObj });
        }
    };

    const onAfterButtonAction = (context, form, query, nexus) => {

        if (query.buttonId === "restablecerValoresBtn") {
            nexus.getGrid("AUT100Modal_grid_3").refresh();
            nexus.getGrid("AUT100Modal_grid_4").refresh();
        }
    };

    const onBeforeSubmit = (context, form, data, nexus) => {
        data.parameters.push({ id: "isSubmit", value: true });
        switch (keyTab) {
            case "general":
                return onBeforeSubmitGeneralTab(data, nexus);

            case "posiciones":
                return onBeforeSubmitPosicionesTab(context, nexus);

            case "interfaces":
                return onBeforeSubmitInterfaces(context, nexus);

            case "puestos":
                return onBeforeSubmitPuestos(context, nexus);

            case "caracteristicas":
                return onBeforeSubmitCaracteristicasTab(context, nexus);
        }
    }

    function onBeforeSubmitGeneralTab(data, nexus) {
        if (props.isUpdate || isUpdate)
            data.parameters.push({ id: "IS_UPDATE", value: "S" });

        data.parameters.push({ id: "UPDATE_UBICACIONES", value: mostrarUbicacionesFieldset });
        
        setAutomatismo(nexus.getForm("AUT100Modal_form_1").getFieldValue("numeroAutomatismo"));
    }

    function onBeforeSubmitCaracteristicasTab(context, nexus) {
        context.abortServerCall = true;
        nexus.showConfirmation({
            message: "General_Sec0_Info_DeseaGuardarCambios",
            onAccept: () => commitGridsCaracteristicas(nexus)
        });
    }

    function onBeforeSubmitPosicionesTab(context, nexus) {
        context.abortServerCall = true;
        nexus.showConfirmation({
            message: "General_Sec0_Info_DeseaGuardarCambios",
            onAccept: () => commitGridPosiciones(nexus)
        });
    }

    function onBeforeSubmitInterfaces(context, nexus) {
        context.abortServerCall = true;
        nexus.showConfirmation({
            message: "General_Sec0_Info_DeseaGuardarCambios",
            onAccept: () => commitGridInterfaces(nexus)
        });
    }

    function onBeforeSubmitPuestos(context, nexus) {
        context.abortServerCall = true;
        nexus.showConfirmation({
            message: "General_Sec0_Info_DeseaGuardarCambios",
            onAccept: () => commitGridPuestos(nexus)
        });
    }

    const onAfterFormSubmit = (context, form, data, nexus) => {
        if (context.responseStatus === "ERROR") return;

        setIsUpdate(true);

        if (data.parameters.some(x => x.id == "AUT100_NU_AUTOMATISMO")) {
            var numeroAutomatismo = data.parameters.find(x => x.id == "AUT100_NU_AUTOMATISMO").value;
            
            setAutomatismo(numeroAutomatismo);
        }

        if (data.parameters.some(x => x.id == "AUT100_POSICIONES_CREADAS")) {
            var value = data.parameters.find(x => x.id == "AUT100_POSICIONES_CREADAS").value;

            if (value === "S") {
                cleanUbicacionesFieldset(nexus);
                setIsTabPosicionesDisabled(false);

                setMostrarUbicacionesFieldset(false);

                nexus.getGrid("AUT100Modal_grid_1").reset();
            }
            else {
                nexus.getForm("AUT100Modal_form_1").reset();
            }
        }

        if (data.parameters.some(x => x.id == "AUT100_INTERFACES_CREADAS")) {
            var value = data.parameters.find(x => x.id == "AUT100_INTERFACES_CREADAS").value;

            if (value === "S") {
                setIsTabInterfacesDisabled(false);
                nexus.getGrid("AUT100Modal_grid_2").reset();
            }
        }

        if (data.parameters.some(x => x.id == "AUT100_CARACTERISTICAS_CREADAS")) {
            var value = data.parameters.find(x => x.id == "AUT100_CARACTERISTICAS_CREADAS").value;

            if (value === "S") {
                setIsTabCaracteristicasDisabled(false);
                nexus.getGrid("AUT100Modal_grid_3").reset();
                nexus.getGrid("AUT100Modal_grid_4").reset();
            }
        }

        setIsTabPuestosDisabled(false);
        nexus.getGrid("AUT100Modal_grid_5").reset();

        nexus.getForm("AUT100_form_1").reset();
    }

    const cleanUbicacionesFieldset = (nexus) => {
        var form = nexus.getForm("AUT100Modal_form_1");

        form.setFieldValue("qtPicking", "");
        form.setFieldValue("qtEntrada", "");
        form.setFieldValue("qtAjuste", "");
        form.setFieldValue("qtRechazo", "");
        form.setFieldValue("qtTransito", "");
        form.setFieldValue("qtSalida", "");
    }

    //------------------------------


    //---------------GRID POSICIONES---------------

    const handleGridBeforeInitialize = (context, data, nexus) => {

        if (keyTab != "posiciones" && automatismoObj == null)
            context.abortServerCall = true;
        else
            data.parameters = [{ id: "AUT100_NU_AUTOMATISMO", value: automatismoObj }];
    };

    const onAfterCommit = (context, rows, data, nexus) => {

        if (context.status === "ERROR") return;

        nexus.getGrid("AUT100Modal_grid_1").refresh();
    };

    const addParameters = (context, data, nexus) => {
        var nuAutomatismo = nexus.getForm("AUT100Modal_form_1").getFieldValue("numeroAutomatismo");

        if (nuAutomatismo == null) nuAutomatismo = automatismoObj;

        data.parameters = [{ id: "AUT100_NU_AUTOMATISMO", value: nuAutomatismo }];
    };

    function commitGridPosiciones(nexus) {
        nexus.getGrid("AUT100Modal_grid_1").commit(true, true);
    }

    //------------------------------


    //---------------GRID INTERFACES---------------

    const handleGridInterfacesBeforeInitialize = (context, data, nexus) => {

        if (keyTab != "interfaces" && automatismoObj == null)
            context.abortServerCall = true;

        else
            data.parameters = [{ id: "AUT100_NU_AUTOMATISMO", value: automatismoObj }];
    };

    const onAfterInterfacesGridCommit = (context, rows, data, nexus) => {

        if (context.status === "ERROR") return;

        nexus.getGrid("AUT100Modal_grid_2").refresh();
    };

    function commitGridInterfaces(nexus) {
        nexus.getGrid("AUT100Modal_grid_2").commit(true, true);
    }

    //------------------------------


    //---------------GRID PUESTOS---------------

    const handleGridPuestosBeforeInitialize = (context, data, nexus) => {

        var nuAutomatismoParam = data.parameters.find(x => x.id == "NU_AUTOMATISMO");
        var nuAutomatismo = "";

        if (nuAutomatismoParam == null || nuAutomatismoParam.value == "") nuAutomatismo = automatismoObj;

        else nuAutomatismo = nuAutomatismoParam.value;

        if (keyTab != "puestos" && automatismoObj == null)
            context.abortServerCall = true;

        else
            data.parameters = [{ id: "AUT100_NU_AUTOMATISMO", value: nuAutomatismo }];
    };

    const onAfterPuestosGridCommit = (context, rows, data, nexus) => {

        if (context.status === "ERROR") return;

        nexus.getGrid("AUT100Modal_grid_5").refresh();
    };

    function commitGridPuestos(nexus) {
        nexus.getGrid("AUT100Modal_grid_5").commit(true, true);
    }

    //------------------------------

    //---------------GRID CARACTERÍSTICAS---------------

    const handleGridCaracteristicasBeforeInitialize = (context, data, nexus) => {

        if (keyTab != "caracteristicas" && automatismoObj == null)
            context.abortServerCall = true;

        else
            data.parameters = [{ id: "AUT100_NU_AUTOMATISMO", value: automatismoObj }];
    };

    const onAfterCaracteristicasGridCommit = (context, rows, parameters, nexus) => {

        if (context.status === "ERROR") return;

        nexus.getGrid("AUT100Modal_grid_3").refresh();
        nexus.getGrid("AUT100Modal_grid_4").refresh();

        if (parameters.find(x => x.id == "UPDATE_GRID_POSICIONES")) {
            nexus.getGrid("AUT100Modal_grid_1").refresh();
        }
    };

    const onBeforeSelectSearch = (context, grid, query, nexus) => {
        query.parameters = [
            { id: "AUT100_NU_AUTOMATISMO", value: automatismoObj },
        ];
    }

    function commitGridsCaracteristicas(nexus) {
        nexus.getGrid("AUT100Modal_grid_3").commit(true, true);
        nexus.getGrid("AUT100Modal_grid_4").commit(true, true);
    }

    //------------------------------

    //---------------TABS---------------

    function changeTab(k) {
        setKeyTab(k);

        if (k === "caracteristicas")
            setShowRestablecerBtn(true);
        else
            setShowRestablecerBtn(false);
    }

    //------------------------------

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-70w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{props.isUpdate ? t("AUT100ConfigurarAutomatismo_Sec0_modalTitle_Titulo") : t("AUT100AgregarAutomatismo_Sec0_modalTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Form
                id="AUT100Modal_form_1"
                application="AUT100Modal"
                validationSchema={validationSchemaForm1}
                initialValues={initialValues}
                onBeforeInitialize={onBeforeInitialize}
                onAfterInitialize={onAfterInitialize}
                onBeforeButtonAction={onBeforeButtonAction}
                onAfterButtonAction={onAfterButtonAction}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={onAfterFormSubmit}
            >
                <Modal.Body>

                    <Tabs transition={false} id="noanim-tab-example2"
                        onSelect={(k) => changeTab(k)}
                    >

                        <Tab eventKey="general" title={t("AUT100Modal_frm1_tab_general")}>

                            <Row>
                                <br />
                                <Col md={8}>
                                    <Row>
                                        <Col md={4}>
                                            <div className="form-group">
                                                <label htmlFor="numeroAutomatismo">{t("AUT100_frm1_lbl_numeroAutomatismo")}</label>
                                                <Field name="numeroAutomatismo" />
                                                <StatusMessage for="numeroAutomatismo" />
                                            </div>
                                        </Col>
                                    </Row>

                                    <Row>
                                        <Col md={4}>
                                            <div className="form-group">
                                                <label htmlFor="codigoExterno">{t("AUT100_frm1_lbl_codigoExterno")}</label>
                                                <Field name="codigoExterno" />
                                                <StatusMessage for="codigoExterno" />
                                            </div>
                                        </Col>
                                        <Col md={4}>
                                            <div className="form-group">
                                                <label htmlFor="tipo">{t("AUT100_frm1_lbl_Tipo")}</label>
                                                <FieldSelect name="tipo" />
                                                <StatusMessage for="tipo" />
                                            </div>
                                        </Col>
                                        <Col md={4}>
                                            <div className="form-group">
                                                <label htmlFor="predio">{t("AUT100_frm1_lbl_Predio")}</label>
                                                <FieldSelect name="predio" />
                                                <StatusMessage for="predio" />
                                            </div>
                                        </Col>
                                    </Row>

                                    <Row>
                                        <Col>
                                            <div className="form-group">
                                                <label htmlFor="descripcion">{t("AUT100_frm1_lbl_Descripcion")}</label>
                                                <Field name="descripcion" />
                                                <StatusMessage for="descripcion" />
                                            </div>
                                        </Col>
                                    </Row>

                                    <Row>
                                        <Col md={4}>
                                            <div className="form-group">
                                                <label htmlFor="zonaUbicacion">{t("AUT100_frm1_lbl_zonaUbicacion")}</label>
                                                <Field name="zonaUbicacion" />
                                                <StatusMessage for="zonaUbicacion" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group">
                                                <FieldToggle name="isEnabled" label={t("AUT100_frm1_lbl_isHabilitado")} />
                                                <StatusMessage for="isEnabled" />
                                            </div>
                                        </Col>

                                    </Row>
                                </Col>

                                <Col md={4}>
                                    <div style={{ display: mostrarUbicacionesFieldset ? 'block' : 'none' }}>
                                        <fieldset className="form-group border p-2">
                                            <legend> {t("AUT100_frm1_lbl_generarUbicacionesLegend")}</legend>

                                            <Row>
                                                <Col md={6}>
                                                    <div className="form-group">
                                                        <label htmlFor="qtPicking">{t("AUT100_frm1_lbl_qtPicking")}</label>
                                                        <FieldNumber name="qtPicking" />
                                                        <StatusMessage for="qtPicking" />
                                                    </div>
                                                </Col>
                                                <Col md={6}>
                                                    <div className="form-group">
                                                        <label htmlFor="qtEntrada">{t("AUT100_frm1_lbl_qtEntrada")}</label>
                                                        <FieldNumber name="qtEntrada" />
                                                        <StatusMessage for="qtEntrada" />
                                                    </div>
                                                </Col>
                                            </Row>

                                            <Row>
                                                <Col md={6}>
                                                    <div className="form-group">
                                                        <label htmlFor="qtAjuste">{t("AUT100_frm1_lbl_qtAjuste")}</label>
                                                        <FieldNumber name="qtAjuste" />
                                                        <StatusMessage for="qtAjuste" />
                                                    </div>
                                                </Col>
                                                <Col md={6}>
                                                    <div className="form-group">
                                                        <label htmlFor="qtRechazo">{t("AUT100_frm1_lbl_qtRechazo")}</label>
                                                        <FieldNumber name="qtRechazo" />
                                                        <StatusMessage for="qtRechazo" />
                                                    </div>
                                                </Col>
                                            </Row>

                                            <Row>
                                                <Col md={6}>
                                                    <div className="form-group">
                                                        <label htmlFor="qtTransito">{t("AUT100_frm1_lbl_qtTransito")}</label>
                                                        <FieldNumber name="qtTransito" />
                                                        <StatusMessage for="qtTransito" />
                                                    </div>
                                                </Col>
                                                <Col md={6}>
                                                    <div className="form-group">
                                                        <label htmlFor="qtSalida">{t("AUT100_frm1_lbl_qtSalida")}</label>
                                                        <FieldNumber name="qtSalida" />
                                                        <StatusMessage for="qtSalida" />
                                                    </div>
                                                </Col>
                                            </Row>
                                        </fieldset>
                                    </div>
                                </Col>
                            </Row>
                        </Tab>

                        <Tab eventKey="posiciones" title={t("AUT100Modal_frm1_tab_posiciones")} disabled={isTabPosicionesDisabled}>
                            <Grid
                                id="AUT100Modal_grid_1"
                                application="AUT100Posiciones"
                                enableExcelExport
                                onBeforeInitialize={handleGridBeforeInitialize}
                                onBeforeFetch={addParameters}
                                onBeforeCommit={addParameters}
                                onAfterCommit={onAfterCommit}
                                onBeforeFetchStats={addParameters}
                                onBeforeExportExcel={addParameters}
                                onBeforeApplyFilter={addParameters}
                                onBeforeApplySort={addParameters}
                                rowsToFetch={30}
                                rowsToDisplay={15}
                            />
                        </Tab>

                        <Tab eventKey="interfaces" title={t("AUT100Modal_frm1_tab_interfaces")} disabled={isTabInterfacesDisabled}>
                            <Grid
                                id="AUT100Modal_grid_2"
                                application="AUT100Interfaces"
                                enableExcelExport
                                onBeforeInitialize={handleGridInterfacesBeforeInitialize}
                                onBeforeFetch={addParameters}
                                onBeforeCommit={addParameters}
                                onAfterCommit={onAfterInterfacesGridCommit}
                                onBeforeFetchStats={addParameters}
                                onBeforeExportExcel={addParameters}
                                onBeforeApplyFilter={addParameters}
                                onBeforeApplySort={addParameters}
                                rowsToFetch={30}
                                rowsToDisplay={15}
                            />
                        </Tab>

                        <Tab eventKey="puestos" title={t("AUT100Modal_frm1_tab_puestos")} disabled={isTabPuestosDisabled}>
                            <Grid
                                id="AUT100Modal_grid_5"
                                application="AUT100Puestos"
                                enableExcelExport
                                onBeforeInitialize={handleGridPuestosBeforeInitialize}
                                onBeforeFetch={addParameters}
                                onBeforeCommit={addParameters}
                                onAfterCommit={onAfterPuestosGridCommit}
                                onBeforeFetchStats={addParameters}
                                onBeforeExportExcel={addParameters}
                                onBeforeApplyFilter={addParameters}
                                onBeforeApplySort={addParameters}
                                rowsToFetch={30}
                                rowsToDisplay={15}
                            />
                        </Tab>

                        <Tab eventKey="caracteristicas" title={t("AUT100Modal_frm1_tab_caracteristicas")} disabled={isTabCaracteristicasDisabled}>
                            <h5> {t("AUT100Modal_form1_title_MultiValueCaracteristicas")} </h5>
                            <Grid
                                id="AUT100Modal_grid_3"
                                application="AUT100Caracteristicas"
                                enableExcelExport
                                onBeforeInitialize={handleGridCaracteristicasBeforeInitialize}
                                onBeforeFetch={addParameters}
                                onBeforeCommit={addParameters}
                                onAfterCommit={onAfterCaracteristicasGridCommit}
                                onBeforeFetchStats={addParameters}
                                onBeforeExportExcel={addParameters}
                                onBeforeApplyFilter={addParameters}
                                onBeforeApplySort={addParameters}
                                rowsToFetch={16}
                                rowsToDisplay={8}
                            />

                            <h5> {t("AUT100Modal_form1_title_UniqueValueCaracteristicas")} </h5>
                            <Grid
                                id="AUT100Modal_grid_4"
                                application="AUT100Caracteristicas"
                                enableExcelExport
                                onBeforeInitialize={handleGridCaracteristicasBeforeInitialize}
                                onBeforeFetch={addParameters}
                                onBeforeCommit={addParameters}
                                onAfterCommit={onAfterCaracteristicasGridCommit}
                                onBeforeFetchStats={addParameters}
                                onBeforeExportExcel={addParameters}
                                onBeforeApplyFilter={addParameters}
                                onBeforeSelectSearch={onBeforeSelectSearch}
                                onBeforeApplySort={addParameters}
                                rowsToFetch={10}
                                rowsToDisplay={5}
                            />
                        </Tab>
                    </Tabs>
                </Modal.Body>
                <Modal.Footer>

                    <div style={{ marginRight: "auto", display: showRestablecerBtn ? "block" : "none" }} className="mb-2">
                        <FormButton id="restablecerValoresBtn" label="AUT100Modal_frm1_btn_ReestablecerCaracteristicas" variant="outline-secondary" />
                    </div>

                    <Button variant="outline-secondary" onClick={handleClose}>
                        {t("General_Sec0_btn_Cerrar")}
                    </Button>

                    <SubmitButton id="btnConfirmarDiferencia" variant="primary" label="AUT100AgregarModal_Sec0_btn_Confirmar" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}