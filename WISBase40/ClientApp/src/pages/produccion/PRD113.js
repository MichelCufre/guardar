import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { Form, Field, FieldToggle, FieldSelect, StatusMessage, FieldDate } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { useToaster } from '../../components/ToasterHook';
import { notificationType } from '../../components/Enums';
import { PRD113ConsumoParcialModal } from './PRD113ConsumoParcialModal';
import { PRD113ProductoNoEsperadoModal } from './PRD113ProductoNoEsperadoModal';
import { PRD113ProductosExpulsable } from './PRD113ProductosExpulsable';
import { PRD113ConfirmarFinalizarProduccionModal } from './PRD113ConfirmarFinalizarProduccionModal';

export default function PRD113(props) {
    const { t } = useTranslation();
    const toaster = useToaster();


    const initialValues = {
        idInternoProduccion: "",
        idExternoProduccion: "",
        descripcionProduccion: "",
        tipoEstacion: "",
        idEspacioProduccion: "",
        descripcionEspacioProduccion: "",
        idModalidadLoteProduccion: "",
        loteUtilizar: "",
        verLineasSinSaldo: "",
        fechaVencimiento: ""
    };
    const initialValuesConsumos = {
        verLineasSinSaldo: ""
    };

    const [_nexus, setNexus] = useState(null);
    const [_form, setform] = useState(null);
    const [isProductoNoEsperadoProduccionModalOpen, setIsProductoNoEsperadoProduccionModalOpen] = useState(false);
    const [isParcialProduccionModalOpen, setIsParcialProduccionModalOpen] = useState(false);

    const [isProduccionHabilitada, setIsProduccionHabilitada] = useState(true);

    const [keyTab, setKeyTab] = useState(null);
    const [keyTabConsumo, setKeyTabConsumo] = useState(null);

    const [idInsumoProduccion, setIdInsumoProduccion] = useState();
    const [idIngresoProduccion, setIdIngresoProduccion] = useState();
    const [ubicacionProduccion, setUbicacionProduccion] = useState();
    const [esConsumible, setEsConsumible] = useState();
    const [cantidadReal, setCantidadReal] = useState();
    const [isRequiredModalidadLote, setIsRequiredModalidadLote] = useState(true);
    const [isRequiredVencimiento, setIsRequiredVencimiento] = useState(true);
    const [modalidadLote, setModalidadLote] = useState();
    const [lote, setLote] = useState();
    const [vencimiento, setVencimiento] = useState(null);
    const [codigoProducto, setCodigoProducto] = useState();
    const [numeroIdentificador, setNumeroIdentificador] = useState();

    const [showConfirmacionPopup, setShowConfirmacionPopup] = useState(false);

    const [diferenciaProducido, setDiferenciaProducido] = useState(false);
    const [diferenciaConsumo, setDiferenciaConsumo] = useState(false);
    const [remanenteProduccion, setRemanenteProduccion] = useState(false);
    const [remanenteInsumos, setRemanenteInsumos] = useState(false);

    const validationSchema = {
        idInternoProduccion: Yup.string(),
        idExternoProduccion: Yup.string(),
        descripcionProduccion: Yup.string(),
        tipoEstacion: Yup.string(),
        idEspacioProduccion: Yup.string(),
        descripcionEspacioProduccion: Yup.string(),
        idModalidadLoteProduccion: Yup.string(),
        loteUtilizar: Yup.string(),
        verLineasSinSaldo: Yup.boolean(),
        fechaVencimiento: Yup.string(),
    };
    const validationSchemaConsumos = {
        verLineasSinSaldo: Yup.boolean()
    }

    const handleBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnConsumir") {
            data.parameters.push({ id: "ubicacionProduccion", value: ubicacionProduccion });
        }
        else if (data.buttonId === "btnConsumirConfirmacion") {
            data.parameters.push({ id: "ubicacionProduccion", value: ubicacionProduccion });
        }
        else if (data.buttonId === "btnParcial") {
            context.abortServerCall = true;
            setIdInsumoProduccion(data.row.cells.find(d => d.column === "NU_PRDC_INGRESO_REAL").value);
            setIdIngresoProduccion(data.row.cells.find(d => d.column === "NU_PRDC_INGRESO").value);
            setCantidadReal(data.row.cells.find(d => d.column === "QT_REAL").value);
            setEsConsumible(data.row.cells.find(d => d.column === "FL_CONSUMIBLE").value);
            setCodigoProducto(data.row.cells.find(d => d.column === "CD_PRODUTO").value);
            setNumeroIdentificador(data.row.cells.find(d => d.column === "NU_IDENTIFICADOR").value);
            setIsParcialProduccionModalOpen(true);

        }
    }

    const onBeforeMenuItemAction = (context, data, nexus) => {
        const modifiedRows = _nexus.getGrid("PRD113_grid_1").getModifiedRows();

        data.parameters.push(
            { id: "ubicacionProduccion", value: ubicacionProduccion },
            { id: "modifiedRows", value: JSON.stringify(modifiedRows) });
    }

    const handleAfterButtonAction = (data, nexus) => {

        if (data.buttonId === "btnConsumir" || data.buttonId === "btnConsumirConfirmacion") {
            nexus.getGrid("PRD113_grid_1").refresh();
        }
        else if (data.buttonId === "btnParcial") {
            nexus.getGrid("PRD113_grid_1").refresh();
        }
        else if (data.buttonId === "btnDesafectar") {
            nexus.getGrid("PRD113_grid_1").refresh();
        }
    }

    const addFormsParams = (context, data, nexus) => {
        var verLineasConSaldo = nexus.getForm("PRD113_form_insumos").getFieldValue("verLineasSinSaldo");

        data.parameters.push({ id: "modalidadLote", value: modalidadLote });
        data.parameters.push({ id: "loteUtilizar", value: lote });
        if (verLineasConSaldo == true)
            data.parameters.push({ id: "insumosConsumidos", value: "S" });
        else
            data.parameters.push({ id: "insumosConsumidos", value: "N" });
    }

    const actualizarGrilla = () => {
        _nexus.getGrid("PRD113_grid_1").refresh();
    }

    const actualizarGrillaProducir = () => {
        _nexus.getGrid("PRD113Producir_grid_1").refresh();
    }


    const onAfterInitializeForm = (context, form, query, nexus) => {
        setKeyTab("stock");
        setKeyTabConsumo("consumo");
        setIsProduccionHabilitada(false);
        setUbicacionProduccion(form.fields.find(x => x.id == "ubicacionProduccion").value);
        setIdIngresoProduccion(form.fields.find(x => x.id == "idInternoProduccion").value);

        setIsRequiredModalidadLote(query.parameters.find(x => x.id == "PRD113_REQUIRED_MODALIDAD").value);
        setIsRequiredVencimiento(query.parameters.find(x => x.id == "PRD113_REQUIRED_VENCIMIENTO").value);

        if (query.parameters.some(x => x.id == "PRD113_HABILITADO_PRODUCCION")) {

            var habilitadoProduccion = query.parameters.find(x => x.id == "PRD113_HABILITADO_PRODUCCION").value;
            if (habilitadoProduccion == "N") {
                setIsProduccionHabilitada(true);
            }
        }

        if (query.parameters.some(x => x.id == "idModalidadLoteProduccion")) {
            let lote = query.parameters.find(x => x.id == "loteUtilizar").value
            let idModalidadLoteProduccion = query.parameters.find(x => x.id == "idModalidadLoteProduccion").value;
            setModalidadLote(idModalidadLoteProduccion);
            setLote(lote);
            nexus.getGrid("PRD113Producir_grid_1").refresh();
        }
        setform(form)
        setNexus(nexus);
    };

    const onBeforeValidateField = (context, form, query, nexus) => {

        if (query.fieldId == "verLineasSinSaldo") {
            context.abortServerCall = true;
            actualizarGrilla();
        } else {
            query.parameters = [
                { id: "nuIngresoProduccion", value: idIngresoProduccion },
                { id: "isRequiredModalidadLote", value: isRequiredModalidadLote },
                { id: "isRequiredVencimiento", value: isRequiredVencimiento }
            ];
        }
    };

    const onAfterSubmitForm = (context, form, query, nexus) => {

        nexus.getGrid("PRD113_grid_1").refresh();

        if (query.buttonId == "FinalizarProduccion") {

            if (query.parameters.some(x => x.id == "CONFIRMAR_FINALIZAR_PRODUCCION")) {
                query.resetForm = false;

                if (query.parameters.find(x => x.id == "CONFIRMAR_FINALIZAR_PRODUCCION").value == "S") {

                    setDiferenciaProducido(query.parameters.find(d => d.id === "DIFERENCIA_PRODUCIDO").value === 'True');
                    setDiferenciaConsumo(query.parameters.find(d => d.id === "DIFERENCIA_CONSUMO").value === 'True');
                    setRemanenteProduccion(query.parameters.find(d => d.id === "REEMANENTE_PRODUCCCION").value === 'True');
                    setRemanenteInsumos(query.parameters.find(d => d.id === "REMANENTE_INSUMOS").value === 'True');

                    setShowConfirmacionPopup(true);
                }
            }
            else {
                if (context.responseStatus === "OK") {
                    nexus.getGrid("PRD113_grid_1").refresh();

                    if (query.parameters.some(x => x.id == "PRD113_HABILITADO_PRODUCCION")) {

                        var habilitadoProduccion = query.parameters.find(x => x.id == "PRD113_HABILITADO_PRODUCCION").value;
                        if (habilitadoProduccion == "N") {
                            setIsProduccionHabilitada(true);
                        }
                    }
                }
            }
        }

        if (query.buttonId == "NotificarProduccion") {
            if (context.responseStatus === "OK") {
                nexus.getGrid("PRD113_grid_1").refresh();
                nexus.getGrid("PRD113Producir_grid_1").refresh();
            }
        }
        else if (query.buttonId == "ConfirmarFinalizarProduccion") {
            if (context.responseStatus === "OK") {
                nexus.getGrid("PRD113_grid_1").refresh();
                setShowConfirmacionPopup(false);
                if (query.parameters.some(x => x.id == "PRD113_HABILITADO_PRODUCCION")) {

                    var habilitadoProduccion = query.parameters.find(x => x.id == "PRD113_HABILITADO_PRODUCCION").value;
                    if (habilitadoProduccion == "N") {
                        setIsProduccionHabilitada(true);
                    }
                }
            }
        }
        else {
            if (context.responseStatus === "OK") {
                nexus.getGrid("PRD113_grid_1").refresh();
                nexus.getGrid("PRD113Producir_grid_1").refresh();
                nexus.getGrid("PRD113Stock_grid_1").refresh();
                nexus.getGrid("PRD113_grid_2").refresh();
                nexus.getGrid("PRD113_grid_3").refresh();
                if (query.buttonId == "btnProducir") {
                    _nexus.getForm("PRD113_form").submit("disableModalidadLoteProduccion");
                }
            }
        }

    };

    const openFormProductosNoEsperados = () => {
        var modalidadLote = _nexus.getForm("PRD113_form").getFieldValue("idModalidadLoteProduccion");
        let loteUtilizar = _nexus.getForm("PRD113_form").getFieldValue("loteUtilizar")
        if (modalidadLote != "") {
            if (loteUtilizar === "") {
                toaster.toastError("PRD113_grid1_Error_LoteRequerido");
            } else {
                setLote(loteUtilizar);
                setModalidadLote(modalidadLote);
                setIsProductoNoEsperadoProduccionModalOpen(true);
            }
        } else {
            toaster.toastError("PRD113_Grid_Error_ModalidadLoteRequerida");
        }
    }

    const confirmarFinalizarProduccion = () => {
        _nexus.getForm("PRD113_form").submit("ConfirmarFinalizarProduccion");
    }

    const finalizarProduccion = () => {
        _nexus.getForm("PRD113_form").submit("FinalizarProduccion");
    }

    const notificarProduccion = () => {
        _nexus.getForm("PRD113_form").submit("NotificarProduccion");
    }

    const onHideParcial = () => {
        setIsParcialProduccionModalOpen(false);
        _nexus.getGrid("PRD113_grid_1").refresh();
    }

    const onHideNoEsperado = (confirmoProduccion) => {
        setIsProductoNoEsperadoProduccionModalOpen(false)
        _nexus.getGrid("PRD113_grid_3").refresh();
        if (confirmoProduccion === "S") {
            _nexus.getForm("PRD113_form").submit("disableModalidadLoteProduccion");
        }
    }

    const onAfterValidateField = (context, form, query, nexus) => {
        setLote(nexus.getForm("PRD113_form").getFieldValue("loteUtilizar"))
        let idModalidadLoteProduccion = _nexus.getForm("PRD113_form").getFieldValue("idModalidadLoteProduccion");
        setModalidadLote(idModalidadLoteProduccion);
        if (query.fieldId == "idModalidadLoteProduccion" || query.fieldId == "loteUtilizar") {
            actualizarGrillaProducir();
        } else {
            setVencimiento(nexus.getForm("PRD113_form").getFieldValue("fechaVencimiento"))
        }
    }

    const consumirMasivo = (evt) => {
        _nexus.getGrid("PRD113_grid_1").triggerMenuAction("btnConsumir", false, evt.ctrlKey);
    }

    const desafectarLinea = (evt) => {
        _nexus.getGrid("PRD113_grid_1").triggerMenuAction("btnDesafectar", false, evt.ctrlKey);
    }

    const producir = (evt) => {
        _nexus.getForm("PRD113_form").submit("btnProducir");
    }

    const onBeforeSubmit = (context, form, data, nexus) => {

        const rowsProducir = _nexus.getGrid("PRD113Producir_grid_1").getModifiedRows();
        var grid = nexus.getGrid("PRD113Producir_grid_1");

        if (data.buttonId === "btnProducir") {
            if (grid.hasError()) {
                context.abortServerCall = true;

                nexus.toast(notificationType.error, "General_Sec0_Error_Error07");

                return false;
            } else {

                data.parameters = [
                    { id: "rowsProducir", value: JSON.stringify(rowsProducir) },
                    { id: "nuIngresoProduccion", value: idIngresoProduccion },
                    { id: "validarModalidad", value: "S" },
                    { id: "isRequiredModalidadLote", value: isRequiredModalidadLote },
                    { id: "isRequiredVencimiento", value: isRequiredVencimiento },
                    { id: "fechaVencimiento", value: vencimiento },
                    { id: "isSubmit", value: "S" }
                ];
            }
        }
    }


    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("PRD113_grid_1").refresh();
    }

    const closeConfirmacionPopup = () => {
        setShowConfirmacionPopup(false);
    }

    return (
        <Page
            title={t("PRD113_Sec0_pageTitle_Titulo")}
            application="PRD113"
            {...props}
        >
            <Form
                id="PRD113_form"
                initialValues={initialValues}
                validationSchema={validationSchema}
                application="PRD113"
                onAfterInitialize={onAfterInitializeForm}
                onAfterSubmit={onAfterSubmitForm}
                onBeforeValidateField={onBeforeValidateField}
                onAfterValidateField={onAfterValidateField}
                onBeforeSubmit={onBeforeSubmit}
            >
                <Row>
                    <Col md={3}>
                        <div className="form-group" >
                            <label htmlFor="idInternoProduccion">{t("PRD113_frm1_lbl_produccion")}</label>
                            <Field name="idInternoProduccion" readOnly />
                            <StatusMessage for="idInternoProduccion" />
                        </div>
                    </Col>
                    <Col md={3}>
                        <div className="form-group" >
                            <label htmlFor="idExternoProduccion">{t("PRD113_frm1_lbl_idExternoProduccion")}</label>
                            <Field name="idExternoProduccion" readOnly />
                            <StatusMessage for="idExternoProduccion" />
                        </div>
                    </Col>
                    <Col md={3}>
                        <div className="form-group" >
                            <label htmlFor="descripcionProduccion">{t("PRD113_frm1_lbl_descripcionProduccion")}</label>
                            <Field name="descripcionProduccion" readOnly />
                            <StatusMessage for="descripcionProduccion" />
                        </div>
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <div className="form-group" >
                            <label htmlFor="tipoEstacion">{t("PRD113_frm1_lbl_tipoEstacion")}</label>
                            <Field name="tipoEstacion" readOnly />
                            <StatusMessage for="tipoEstacion" />
                        </div>
                    </Col>
                    <Col md={3}>
                        <div className="form-group" >
                            <label htmlFor="idEspacioProduccion">{t("PRD113_frm1_lbl_idEspacioProduccion")}</label>
                            <Field name="idEspacioProduccion" readOnly />
                            <StatusMessage for="idEspacioProduccion" />
                        </div>
                    </Col>
                    <Col md={3}>
                        <div className="form-group" >
                            <label htmlFor="descripcionEspacioProduccion">{t("PRD113_frm1_lbl_descripcionEspacioProduccion")}</label>
                            <Field name="descripcionEspacioProduccion" readOnly />
                            <StatusMessage for="descripcionEspacioProduccion" />
                        </div>
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <div className="form-group" >
                            <label htmlFor="empresa">{t("PRD113_form1_label_Empresa")}</label>
                            <Field name="empresa" readOnly />
                            <StatusMessage for="empresa" />
                        </div>
                    </Col>
                    <Col md={3}>
                        <div className="form-group" >
                            <label htmlFor="nombreEmpresa">{t("PRD113_form1_label_NmEmpresa")}</label>
                            <Field name="nombreEmpresa" readOnly />
                            <StatusMessage for="nombreEmpresa" />
                        </div>
                    </Col>
                    <Col md={3}>
                        <div className="form-group" >
                            <label htmlFor="ubicacionEntrada">{t("PRD113_frm1_lbl_ubicacionEntrada")}</label>
                            <Field name="ubicacionEntrada" readOnly />
                            <StatusMessage for="ubicacionEntrada" />
                        </div>
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <div className="form-group" >
                            <label htmlFor="ubicacionProduccion">{t("PRD113_frm1_lbl_ubicacionProduccion")}</label>
                            <Field name="ubicacionProduccion" readOnly />
                            <StatusMessage for="ubicacionProduccion" />
                        </div>
                    </Col>
                    <Col md={3}>
                        <div className="form-group" >
                            <label htmlFor="ubicacionSalida">{t("PRD113_frm1_lbl_ubicacionSalida")}</label>
                            <Field name="ubicacionSalida" readOnly />
                            <StatusMessage for="ubicacionSalida" />
                        </div>
                    </Col>
                    <Col md={3}>
                        <div className="form-group" >
                            <label htmlFor="ubicacionSalidaTran">{t("PRD113_frm1_lbl_ubicacionSalidaTran")}</label>
                            <Field name="ubicacionSalidaTran" readOnly />
                            <StatusMessage for="ubicacionSalidaTran" />
                        </div>
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <div className="form-group" >
                            <label htmlFor="idModalidadLoteProduccion">{t("PRD113_frm1_lbl_modalidadLote")}</label>
                            <FieldSelect name="idModalidadLoteProduccion" />
                            <StatusMessage for="idModalidadLoteProduccion" />
                        </div>
                    </Col>
                    <Col md={3}>
                        <div className="form-group" >
                            <label htmlFor="loteUtilizar">{t("PRD113_frm1_lbl_loteUtilizar")}</label>
                            <Field name="loteUtilizar" />
                            <StatusMessage for="loteUtilizar" />
                        </div>
                    </Col>
                    <Col md={3}>

                        <div className="form-group">
                            <label htmlFor="fechaVencimiento">{t("PRE100_frm1_lbl_fechaVencimiento")}</label>
                            <FieldDate name="fechaVencimiento" />
                            <StatusMessage for="fechaVencimiento" />
                        </div>
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <div className="text-center form-group">
                            <Button id="btnNotificar" variant="primary" onClick={notificarProduccion} disabled={isProduccionHabilitada}>
                                {t("PRD113_frm1_lbl_btnNotificar")}
                            </Button>
                        </div>
                    </Col>
                    <Col md={3}>
                        <div className="text-center form-group">
                            <Button id="btnFinalizar" variant="primary" onClick={finalizarProduccion} disabled={isProduccionHabilitada}>
                                {t("PRD113_frm1_lbl_btnFinalizar")}
                            </Button>
                        </div>
                    </Col>
                </Row>
            </Form>
            <Tabs defaultActiveKey="consumo" transition={false} id="noanim-tab-example"
                activeKey={keyTabConsumo}
                onSelect={(k) => setKeyTabConsumo(k)}
            >
                <Tab eventKey="consumo" title={t("PRD113_tab_title_Consumir")}>
                    <br />
                    <Row>
                        <Form
                            id="PRD113_form_insumos"
                            initialValues={initialValuesConsumos}
                            validationSchema={validationSchemaConsumos}
                            application="PRD113Insumos"
                            onBeforeValidateField={onBeforeValidateField}
                            onAfterValidateField={onAfterValidateField}
                        >
                            <Col md={3}>
                                <div className="text-center form-group" style={{ display: 'flex', height: '100%' }}>
                                    <FieldToggle name="verLineasSinSaldo" onChange={actualizarGrilla} />
                                    <label style={{ marginLeft: '8px' }}>{t("PRD113_frm1_lbl_VerLineasSinSaldo")}</label>
                                </div>
                            </Col>
                        </Form>
                    </Row>
                    <br />
                    <Row>
                        <Col span={6} style={{ maxWidth: "85%" }}>
                            <Grid
                                id="PRD113_grid_1"
                                application="PRD113Insumos"
                                rowsToFetch={30}
                                rowsToDisplay={10}
                                enableExcelExport
                                enableExcelImport={false}
                                enableSelection
                                onBeforeButtonAction={handleBeforeButtonAction}
                                onAfterButtonAction={handleAfterButtonAction}
                                onBeforeMenuItemAction={onBeforeMenuItemAction}
                                onAfterMenuItemAction={onAfterMenuItemAction}
                                onBeforeFetch={addFormsParams}
                                onBeforeApplyFilter={addFormsParams}
                                onBeforeApplySort={addFormsParams}
                                onBeforeExportExcel={addFormsParams}
                            />
                        </Col>
                        <Col span={6} style={{ display: 'flex', flexDirection: 'column', justifyContent: 'center', alignItems: 'center', height: '200px', maxWidth: '15%' }}>
                            <Row style={{ marginBottom: '10px', width: '100%', justifyContent: 'center' }}>
                                <button id="btnConsumirMasivo" className="btn btn-primary w-100" onClick={consumirMasivo} disabled={isProduccionHabilitada}>{t("PRD113_Sec0_btn_Consumir")}</button>
                            </Row>
                            <Row style={{ marginBottom: '10px', width: '100%', justifyContent: 'center' }}>
                                <button id="btnDesafectar" className="btn btn-primary w-100" onClick={desafectarLinea} disabled={isProduccionHabilitada}>{t("PRD113_Sec0_btn_Desafectar")}</button>
                            </Row>
                        </Col>
                    </Row>
                </Tab>
                <Tab eventKey="producir" title={t("PRD113_tab_title_Producir")}>
                    <br></br>
                    <Row>
                        <Col span={6} style={{ maxWidth: "85%" }}>
                            <Grid
                                id="PRD113Producir_grid_1"
                                application="PRD113Producir"
                                rowsToFetch={30}
                                rowsToDisplay={10}
                                enableExcelExport
                                enableExcelImport={false}
                                onBeforeInitialize={addFormsParams}
                                onBeforeFetch={addFormsParams}
                                onBeforeFetchStats={addFormsParams}
                                onBeforeExportExcel={addFormsParams}

                            />
                        </Col>
                        <Col span={6} style={{ display: 'flex', flexDirection: 'column', justifyContent: 'center', alignItems: 'center', height: '200px', maxWidth: '15%', marginTop: '90px' }}>
                            <Row style={{ marginBottom: '10px', width: '100%', justifyContent: 'center' }}>
                                <button id="btnProducir" className="btn btn-primary w-100" onClick={producir} disabled={isProduccionHabilitada}>{t("PRD112_Sec0_btn_Producir")}</button>
                            </Row>
                            <Row style={{ marginBottom: '10px', width: '100%', justifyContent: 'center' }}>
                                <button id="btnAgregarProductoNoEsperado" className="btn btn-primary w-100" onClick={openFormProductosNoEsperados} disabled={isProduccionHabilitada}>{t("PRD112_Sec0_btn_ProducirNoEsperado")}</button>
                            </Row>
                        </Col>
                    </Row>
                </Tab>
                <Tab eventKey="expulsar" title={t("PRD113_tab_title_Expulsar")}>
                    <PRD113ProductosExpulsable isProduccionHabilitada={isProduccionHabilitada} />
                </Tab>
            </Tabs>

            <PRD113ConsumoParcialModal show={isParcialProduccionModalOpen} onHide={onHideParcial} idInsumoProduccion={idInsumoProduccion} idIngresoProduccion={idIngresoProduccion} cantidadReal={cantidadReal} esConsumible={esConsumible} codigoProducto={codigoProducto} numeroIdentificador={numeroIdentificador} />
            <PRD113ProductoNoEsperadoModal show={isProductoNoEsperadoProduccionModalOpen} onHide={onHideNoEsperado} idIngresoProduccion={idIngresoProduccion} ubicacionProduccion={ubicacionProduccion} lote={lote} modalidadLote={modalidadLote} />
            <br />
            <Tabs defaultActiveKey="esperados" transition={false} id="noanim-tab-example"
                activeKey={keyTab}
                onSelect={(k) => setKeyTab(k)}
            >
                <Tab eventKey="stock" title={t("PRD113_Nav1_lbl_tab_3")}>

                    <Row className='mt-3'>
                        <Col>
                            <Grid
                                id="PRD113Stock_grid_1"
                                application="PRD113StockProduccion"
                                rowsToFetch={30}
                                rowsToDisplay={10}
                                enableExcelExport
                            />
                        </Col>
                    </Row>
                </Tab>
                <Tab eventKey="esperados" title={t("PRD113_Nav1_lbl_tab_1")}>

                    <Row className='mt-3'>
                        <Col>
                            <Grid
                                id="PRD113_grid_2"
                                application="PRD113ProdEsp"
                                rowsToFetch={30}
                                rowsToDisplay={10}
                                enableExcelExport
                            />
                        </Col>
                    </Row>
                </Tab>
                <Tab eventKey="noEsperados" title={t("PRD113_Nav1_lbl_tab_2")}>

                    <Row className='mt-3'>
                        <Col>
                            <Grid
                                id="PRD113_grid_3"
                                application="PRD113ProdNoEsp"
                                rowsToFetch={30}
                                rowsToDisplay={10}
                                enableExcelExport
                            />
                        </Col>
                    </Row>
                </Tab>
            </Tabs>

            <PRD113ConfirmarFinalizarProduccionModal
                show={showConfirmacionPopup}
                onHide={closeConfirmacionPopup}
                confirmarFinalizar={confirmarFinalizarProduccion}
                diferenciaProducido={diferenciaProducido}
                diferenciaConsumo={diferenciaConsumo}
                remanenteProduccion={remanenteProduccion}
                remanenteInsumos={remanenteInsumos}
                idIngresoProduccion={idIngresoProduccion}
                ubicacionProduccion={ubicacionProduccion}
            />

        </Page >
    );
}
