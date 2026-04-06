import React, { useState, useRef, useEffect } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldCheckbox, FieldSelect, FieldToggle, FieldSelectAsync, SubmitButton, FormButton, StatusMessage } from '../../components/FormComponents/Form';
import { Row, Col, FormGroup } from 'react-bootstrap';
import { FormTab, FormTabStep } from '../../components/FormComponents/FormTab';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import Accordion from '@mui/material/Accordion';
import AccordionSummary from '@mui/material/AccordionSummary';
import AccordionDetails from '@mui/material/AccordionDetails';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { forEach } from 'lodash';
import { FormWarningMessage } from '../../components/FormComponents/FormWarningMessage';

export default function INV410(props) {
    const { t } = useTranslation();
    const [isShowForm, setShowForm] = useState(false);
    const [currentTab, setCurrentTab] = useState("tabTipoMovimiento");
    const [inventarioType, setInventarioType] = useState("");
    const [summary, setSummary] = useState({});
    const [descripcionTipoInventario, setDescripcionTipoInventario] = useState("");

    const refInventario = useRef(null);
    const [nexus, setNexus] = useState(null);
    const [filtro, setFiltro] = useState({ AtributosCabezal: [], AtributosDetalle: [] });
    const [showWarningUbicOtrosInv, setShowWarningUbicOtrosInv] = useState(false);

    useEffect(() => {
        if (nexus) {

            if (nexus.getGrid("INV410_grid_Lpn"))
                nexus.getGrid('INV410_grid_Lpn').refresh();

            if (nexus.getGrid("INV410_grid_LpnQuitar"))
                nexus.getGrid('INV410_grid_LpnQuitar').refresh();

            if (nexus.getGrid("INV410_grid_DetalleLpn"))
                nexus.getGrid('INV410_grid_DetalleLpn').refresh();

            if (nexus.getGrid("INV410_grid_DetalleLpnQuitar"))
                nexus.getGrid('INV410_grid_DetalleLpnQuitar').refresh();
        }

    }, [filtro]);


    const initialValues = {
        nuInventario: "",
        descInventario: "",
        empresa: "",
        tipoCierreConteo: "",
        predio: "",
        modificarStockEnDif: false,
        bloquearUsrConteoSucesivo: false,
        controlarVencimiento: false,
        permiteIngresoMotivo: false,
        actualizarConteoFin: false,
        excluirSueltos: false,
        excluirLpns: false,
        restablecerLpnFinalizado: false,
        generarPrimerConteo: false,
        permiteAsociarUbicOtrosInv: false,
    };

    const validationSchema = {
        descInventario: Yup.string().nullable(),
        empresa: Yup.string().nullable(),
        tipoCierreConteo: Yup.string().required(),
        predio: Yup.string().required(),
        modificarStockEnDif: Yup.boolean(),
        bloquearUsrConteoSucesivo: Yup.boolean(),
        controlarVencimiento: Yup.boolean(),
        permiteIngresoMotivo: Yup.boolean(),
        actualizarConteoFin: Yup.boolean(),
        excluirSueltos: Yup.boolean(),
        excluirLpns: Yup.boolean(),
        restablecerLpnFinalizado: Yup.boolean(),
        generarPrimerConteo: Yup.boolean(),
        permiteAsociarUbicOtrosInv: Yup.boolean(),
    };

    const formShowButtonClassName = isShowForm ? "hidden" : "";
    const formClassName = isShowForm ? "" : "hidden";

    useEffect(() => {
        if (!nexus) return;

        if (inventarioType !== "SUBI") {
            const form = nexus.getForm("INV410_form_1");
            if (!form) return;

            form.setFieldValue("permiteAsociarUbicOtrosInv", false);
        }
    }, [inventarioType, nexus]);


    const hideForm = () => {
        setShowForm(false);
    };

    const resetFormData = () => {
        setInventarioType("");
        setSummary({});
        refInventario.current = null;
    };

    const onBeforeButtonAction = (context, form, query, nexus) => {
        if (query.buttonId === "showFormButton") {
            goToMovimiento(nexus);
        } else if (query.buttonId === "hideFormButton") {
            hideForm();
            resetFormData();
            nexus.getForm("INV410_form_1").reset();
        }
        else if (query.buttonId === "btnLoadSummary") {
            query.parameters = [
                { id: "nuInventario", value: refInventario.current.nuInventario }
            ];
        }
        else if (query.buttonId === "btnFinish" || query.buttonId === "btnVolverInicio") {
            context.abortServerCall = true;
            nexus.getGrid("INV410_grid_1").refresh();
            goToMovimiento(nexus);
        }
        else if (query.buttonId === "btnVolverInicio") {
            context.abortServerCall = true;
            setDescripcionTipoInventario("");

            setCurrentTab("tabTipoMovimiento");
        }
        else if (query.buttonId === "btnHabilitar") {
            query.parameters = [
                { id: "nuInventario", value: refInventario.current.nuInventario }
            ];
        }
        else if (query.buttonId === "btnAplicarFiltro") {
            var filtro = { AtributosCabezal: [], AtributosDetalle: [] };

            forEach(nexus.getGrid('INV410_grid_LpnAtrCab').getModifiedRows(), row => {
                var atributo = row.cells.find(c => c.column == 'VL_ATRIBUTO' && c.value != '');

                if (atributo) {
                    filtro.AtributosCabezal.push({ Id: row.id, Value: atributo.value });
                }
            });

            forEach(nexus.getGrid('INV410_grid_DetalleLpnAtrCab').getModifiedRows(), row => {
                var atributo = row.cells.find(c => c.column == 'VL_ATRIBUTO' && c.value != '');

                if (atributo) {
                    filtro.AtributosCabezal.push({ Id: row.id, Value: atributo.value });
                }
            });

            forEach(nexus.getGrid('INV410_grid_DetalleLpnAtrDet').getModifiedRows(), row => {
                var atributo = row.cells.find(c => c.column == 'VL_ATRIBUTO' && c.value != '');

                if (atributo) {
                    filtro.AtributosDetalle.push({ Id: row.id, Value: atributo.value });
                }
            });

            setNexus(nexus);
            setFiltro(filtro);
        }
    };

    const onAfterButtonAction = (context, form, query, nexus) => {
        if (query.buttonId === "showFormButton") {
            setShowForm(true);
        } else if (query.buttonId === "btnLoadSummary") {
            const summary = query.parameters.reduce((result, parameter) => {
                result[parameter.id] = parameter.value;

                return result;
            }, {});

            setSummary(summary);

            setCurrentTab("tabFin");
        }
        else if (query.buttonId === "btnHabilitar") {
            nexus.getGrid("INV410_grid_1").refresh();
            goToMovimiento(nexus);
        }
    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        if (currentTab === "tabRegistros") {
            context.abortServerCall = true;

            return false;
        }

        query.Parameters = [{
            id: "tipoInventario",
            value: inventarioType
        }];
    };
    const onAfterSubmit = (context, form, query, nexus) => {
        setShowWarningUbicOtrosInv(false);

        if (context.status === "ERROR")
            return false;

        refInventario.current = {
            nuInventario: form.fields.find(d => d.id === "nuInventario").value
        };

        if (inventarioType === "SREG") {
            nexus.getGrid("INV410_grid_Registros").refresh();
            nexus.getGrid("INV410_grid_RegistrosQuitar").refresh();
        }
        else if (inventarioType === "SUBI") {
            nexus.getGrid("INV410_grid_Ubicacion").refresh();
            nexus.getGrid("INV410_grid_UbicacionQuitar").refresh();
        }
        else if (inventarioType === "SLPN") {
            nexus.getGrid("INV410_grid_Lpn").refresh();
            nexus.getGrid("INV410_grid_LpnQuitar").refresh();
        }
        else if (inventarioType === "SLPNDET") {
            nexus.getGrid("INV410_grid_DetalleLpn").refresh();
            nexus.getGrid("INV410_grid_DetalleLpnQuitar").refresh();
        }

        nexus.getGrid("INV410_grid_1").refresh();

        setCurrentTab("tabRegistros");
    };

    const appendParameters = (context, data, nexus) => {
        if (refInventario.current) {
            data.parameters = [
                { id: "filtro", value: JSON.stringify(filtro) },
                { id: "nuInventario", value: refInventario.current.nuInventario }
            ];
        }
    };

    const onAfterMenuItemActionRegistros = (context, data, nexus) => {
        if (inventarioType === "SREG") {
            nexus.getGrid("INV410_grid_Registros").refresh();
            nexus.getGrid("INV410_grid_RegistrosQuitar").refresh();
        }
        else if (inventarioType === "SUBI") {
            nexus.getGrid("INV410_grid_Ubicacion").refresh();
            nexus.getGrid("INV410_grid_UbicacionQuitar").refresh();
        }
        else if (inventarioType === "SLPN") {
            nexus.getGrid("INV410_grid_Lpn").refresh();
            nexus.getGrid("INV410_grid_LpnQuitar").refresh();
        }
        else if (inventarioType === "SLPNDET") {
            nexus.getGrid("INV410_grid_DetalleLpn").refresh();
            nexus.getGrid("INV410_grid_DetalleLpnQuitar").refresh();
        }
    };

    const onAfterButtonActionInventario = (data, nexus) => {

        if (data.buttonId === "btnHabilitarInventario"
            || data.buttonId === "btnCancelarInventario"
            || data.buttonId === "btnCierreParcial"
            || data.buttonId === "btnCerrarInventario"
            || data.buttonId === "btnRegenerarInventario"
        ) {
            nexus.getGrid("INV410_grid_1").refresh();
        }

    };
    const createInventario = (evt, tpInventario, descripcion) => {
        if (currentTab === "tabRegistros")
            return false;

        evt.preventDefault();

        setInventarioType(tpInventario);
        setDescripcionTipoInventario(" " + t(descripcion));
        setCurrentTab("tabConfiguracion");
    };

    const goToConfiguracion = (evt) => {
        setCurrentTab("tabConfiguracion");
        evt.preventDefault();
    };
    const goToMovimiento = (nexus) => {
        setShowForm(false);
        setCurrentTab("tabTipoMovimiento");
        resetFormData();
    };
    const goToRegistros = (evt) => {
        setCurrentTab("tabRegistros");
        evt.preventDefault();
    };
    const onTabClose = (evt) => {
        hideForm();
    };
    const onBeforeValidateField = (context, form, query, nexus) => {
        if (currentTab === "tabRegistros") {
            context.abortServerCall = true;
        }

        if (query.fieldId === "permiteAsociarUbicOtrosInv") {
            const value = nexus
                .getForm("INV410_form_1")
                .getFieldValue("permiteAsociarUbicOtrosInv");

            setShowWarningUbicOtrosInv(!!value);
        }
    };

    const onBeforeFetch = (context, data, nexus) => {
        if (data.rowsToSkip == 0) {
            if (data.gridId == "INV410_grid_LpnAtrCab") {
                setFiltro({ AtributosCabezal: [], AtributosDetalle: [] });
            }
            else if (data.gridId == "INV410_grid_DetalleLpnAtrCab") {
                setFiltro({ AtributosCabezal: [], AtributosDetalle: filtro.AtributosDetalle });
            }
            else if (data.gridId == "INV410_grid_DetalleLpnAtrDet") {
                setFiltro({ AtributosCabezal: filtro.AtributosCabezal, AtributosDetalle: [] });
            }
        }
    }

    const onAfterApplySort = (context, rows, parameters) => {
        if (data.gridId == "INV410_grid_LpnAtrCab") {
            setFiltro({ AtributosCabezal: [], AtributosDetalle: [] });
        }
        else if (data.gridId == "INV410_grid_DetalleLpnAtrCab") {
            setFiltro({ AtributosCabezal: [], AtributosDetalle: filtro.AtributosDetalle });
        }
        else if (data.gridId == "INV410_grid_DetalleLpnAtrDet") {
            setFiltro({ AtributosCabezal: filtro.AtributosCabezal, AtributosDetalle: [] });
        }
    }

    const registrosClassName = inventarioType === "SREG" ? "" : "hidden";
    const ubicacionesClassName = inventarioType === "SUBI" ? "" : "hidden";
    const lpnClassName = inventarioType === "SLPN" ? "" : "hidden";
    const lpnDetalleClassName = inventarioType === "SLPNDET" ? "" : "hidden";

    const summaryEmpresa = summary.empresa ? <li>{`${summary.nombreEmpresa} (${summary.empresa})`}</li> : null;

    const summaryContent = (
        <React.Fragment>
            <h4>{t("INV410_Sec0_lbl_InventarioX")} {summary.inventario}</h4>
            <ul className="list-unstyled">
                {summaryEmpresa}
                <li>{summary.descripcion}</li>
                <li>{summary.cierreConteoDescripcion}</li>
            </ul>
            <h5>{t("INV410_Sec0_lbl_Características")}</h5>
            <ul className="list-unstyled">
                <li>{summary.actualizarConteoFin === "S" ? t("INV410_Sec0_lbl_ActualizarConteoFin") : ""}</li>
                <li>{summary.bloquearConteosSucesivos === "S" ? t("INV410_Sec0_lbl_BloquearConteosSucesivos") : ""}</li>
                <li>{summary.controlarVencimiento === "S" ? t("INV410_Sec0_lbl_ControlarVencimiento") : ""}</li>
                <li>{summary.modificarStockEnDiferencia === "S" ? t("INV410_Sec0_lbl_ModificarStockEnDiferencia") : ""}</li>
                <li>{summary.permiteIngresarMotivo === "S" ? t("INV410_Sec0_lbl_PermiteIngresarMotivo") : ""}</li>
                <li>{summary.excluirSueltos === "S" ? t("INV410_Sec0_lbl_ExcluiraStockSuelto") : ""}</li>
                <li>{summary.excluirLpns === "S" ? t("INV410_Sec0_lbl_ExcluiraStockLpn") : ""}</li>
                <li>{summary.restablecerLpnFinalizado === "S" ? t("INV410_Sec0_lbl_RestableceraLpnFinalizado") : ""}</li>
                <li>{summary.generarPrimerConteo === "S" ? t("INV410_Sec0_lbl_GenerarPrimerConteo") : ""}</li>
                <li>{summary.permiteAsociarUbicOtrosInv === "S" ? t("INV410_Sec0_lbl_PermiteAsociarUbicOtrosInv") : ""}</li>
            </ul>
        </React.Fragment>
    );

    return (
        <Page
            icon="fas fa-copy"
            application="INV410"
            title={t("INV410_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                id="INV410_form_1"
                application="INV410"
                validationSchema={validationSchema}
                initialValues={initialValues}
                onBeforeValidateField={onBeforeValidateField}
                onBeforeButtonAction={onBeforeButtonAction}
                onAfterButtonAction={onAfterButtonAction}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={onAfterSubmit}
            >
                <Field type="hidden" name="nuInventario" />
                <div className={formShowButtonClassName} style={{ textAlign: "center" }}>
                    <FormButton id="showFormButton" label="INV410_frm1_btn_MostrarForm" variant="success" className="mb-4" />
                </div>
                <div className={formClassName}>
                    <Row>
                        <Col>
                            <FormTab
                                current={currentTab}
                                title={t("INV410_frm1_lbl_Title") + descripcionTipoInventario}
                                onClose={onTabClose}
                            >
                                <FormTabStep id="tabTipoMovimiento" label="INV410_frm1_lbl_TP_INVENTARIO">
                                    <div style={{ minHeight: 400 }}>
                                        <h3 className="mt-2">{t("INV410_Sec0_lbl_SelectTPInventario")}</h3>
                                        <Row>
                                            <Col>
                                                <button className="btn btn-lg btn-light mr-5" onClick={(evt) => { createInventario(evt, "SUBI", evt.target.textContent) }}>{t("INV410_Sec0_lbl_PorUbicacion")}</button>
                                                <button className="btn btn-lg btn-light mr-5" onClick={(evt) => { createInventario(evt, "SREG", evt.target.textContent) }}>{t("INV410_Sec0_lbl_PorRegistro")}</button>
                                                <button className="btn btn-lg btn-light mr-5" onClick={(evt) => { createInventario(evt, "SLPN", evt.target.textContent) }}>{t("INV410_Sec0_lbl_PorLpn")}</button>
                                                <button className="btn btn-lg btn-light mr-5" onClick={(evt) => { createInventario(evt, "SLPNDET", evt.target.textContent) }}>{t("INV410_Sec0_lbl_PorLpnDetalle")}</button>
                                            </Col>
                                        </Row>
                                    </div>
                                </FormTabStep>
                                <FormTabStep id="tabConfiguracion" label="INV410_Sec0_lbl_Configuracion">
                                    <h3>{t("INV410_Sec0_lbl_DatosInventario")}</h3>
                                    <Row>
                                        <Col>
                                            <FormGroup>
                                                <label htmlFor="descInventario">{t("INV_frm1_lbl_DS_INVENTARIO")}</label>
                                                <Field name="descInventario" />
                                                <StatusMessage for="descInventario" />
                                            </FormGroup>
                                            <Row>
                                                <Col lg={6}>
                                                    <FormGroup>
                                                        <label htmlFor="empresa">{t("General_frm1_lbl_NM_EMPRESA")}</label>
                                                        <FieldSelectAsync name="empresa" isClearable={true} />
                                                        <StatusMessage for="empresa" />
                                                    </FormGroup>
                                                </Col>
                                                <Col lg={6}>
                                                    <FormGroup>
                                                        <label htmlFor="tipoCierreConteo">{t("INV410_frm1_lbl_ND_CIERRE_CONTEO")}</label><label style={{ color: "red" }}> *</label>
                                                        <FieldSelect name="tipoCierreConteo" />
                                                        <StatusMessage for="tipoCierreConteo" />
                                                    </FormGroup>
                                                </Col>
                                            </Row>
                                            <FormGroup>
                                                <label htmlFor="predio">{t("INV410_frm1_lbl_NU_PREDIO")}</label><label style={{ color: "red" }}> *</label>
                                                <FieldSelect name="predio" />
                                                <StatusMessage for="predio" />
                                            </FormGroup>
                                        </Col>
                                        <Col>
                                            <FormGroup>
                                                <FieldCheckbox
                                                    name="actualizarConteoFin"
                                                    label={t("INV410_frm1_lbl_FL_ACTUALIZAR_CONTEO_FIN")}
                                                    className="mb-2"
                                                />
                                                <StatusMessage for="actualizarConteoFin" />
                                            </FormGroup>
                                            <FormGroup>
                                                <FieldCheckbox
                                                    name="bloquearUsrConteoSucesivo"
                                                    label={t("INV410_frm1_lbl_FL_BLOQ_USR_CONTEO_SUCESIVO")}
                                                    className="mb-2"
                                                />
                                                <StatusMessage for="bloquearUsrConteoSucesivo" />
                                            </FormGroup>
                                            <FormGroup>
                                                <FieldCheckbox
                                                    name="controlarVencimiento"
                                                    label={t("INV410_frm1_lbl_FL_CONTROLAR_VENCIMIENTO")}
                                                    className="mb-2"
                                                />
                                                <StatusMessage for="controlarVencimiento" />
                                            </FormGroup>
                                            <FormGroup>
                                                <FieldCheckbox
                                                    name="modificarStockEnDif"
                                                    label={t("INV410_frm1_lbl_FL_MODIFICAR_STOCK_EN_DIF")}
                                                    className="mb-2"
                                                />
                                                <StatusMessage for="modificarStockEnDif" />
                                            </FormGroup>
                                            <FormGroup>
                                                <FieldCheckbox
                                                    name="permiteIngresoMotivo"
                                                    label={t("INV410_frm1_lbl_FL_PERMITE_INGRESO_MOTIVO")}
                                                    className="mb-2"
                                                />
                                                <StatusMessage for="permiteIngresoMotivo" />
                                            </FormGroup>
                                            <FormGroup>
                                                <FieldCheckbox
                                                    name="excluirSueltos"
                                                    label={t("INV410_frm1_lbl_ExcluirSueltos")}
                                                    className="mb-2"
                                                />
                                                <StatusMessage for="excluirSueltos" />
                                            </FormGroup>
                                            <FormGroup>
                                                <FieldCheckbox
                                                    name="excluirLpns"
                                                    label={t("INV410_frm1_lbl_ExcluirLpns")}
                                                    className="mb-2"
                                                />
                                                <StatusMessage for="excluirLpns" />
                                            </FormGroup>
                                            <FormGroup>
                                                <FieldCheckbox
                                                    name="restablecerLpnFinalizado"
                                                    label={t("INV410_frm1_lbl_RestablecerLpnFinalizado")}
                                                    className="mb-2"
                                                />
                                                <StatusMessage for="restablecerLpnFinalizado" />
                                            </FormGroup>
                                            <FormGroup>
                                                <FieldCheckbox
                                                    name="generarPrimerConteo"
                                                    label={t("INV410_frm1_lbl_GenerarPrimerConteo")}
                                                    className="mb-2"
                                                />
                                                <StatusMessage for="generarPrimerConteo" />
                                            </FormGroup>
                                            <div className={inventarioType === "SUBI" ? "" : "hidden"}>
                                                <FormGroup>
                                                    <FieldCheckbox
                                                        name="permiteAsociarUbicOtrosInv"
                                                        label={t("INV410_frm1_lbl_PermiteAsociarUbicOtrosInv")}
                                                        className="mb-2"
                                                    />
                                                    <StatusMessage for="permiteAsociarUbicOtrosInv" />
                                                </FormGroup>
                                            </div>
                                        </Col>
                                    </Row>
                                    <div className={inventarioType === "SUBI" ? "" : "hidden"}>
                                        <Row>
                                            <FormWarningMessage message={t("INV410_Sec0_msg_WarningUbicOtroInv")} show={showWarningUbicOtrosInv} />
                                        </Row>
                                    </div>
                                    <hr />

                                    <Row>
                                        <Col>
                                            <FormButton id="btnVolverInicio" variant="link" className="mr-2" label="INV410_Sec0_btn_Volver" />
                                            &nbsp;
                                            <SubmitButton id="btnContinuarInventario" className="mr-2" variant="primary" label="INV410_Sec0_btn_Siguiente" />
                                        </Col>
                                    </Row>

                                </FormTabStep>
                                <FormTabStep id="tabRegistros" label="INV410_Sec0_lbl_Registros">

                                    <div className={registrosClassName}>
                                        <Row>
                                            <Col lg={6}>
                                                <h3>{t("INV410_Sec0_lbl_RegistrosDisponibles")}</h3>
                                                <Grid
                                                    id="INV410_grid_Registros"
                                                    application="INV410"
                                                    rowsToFetch={30}
                                                    rowsToDisplay={15}
                                                    enableSelection
                                                    enableExcelExport
                                                    onBeforeFetch={appendParameters}
                                                    onBeforeApplyFilter={appendParameters}
                                                    onBeforeApplySort={appendParameters}
                                                    onBeforeMenuItemAction={appendParameters}
                                                    onBeforeFetchStats={appendParameters}
                                                    onBeforeInitialize={appendParameters}
                                                    onBeforeExportExcel={appendParameters}
                                                    onAfterMenuItemAction={onAfterMenuItemActionRegistros}
                                                />
                                            </Col>
                                            <Col lg={6}>
                                                <h3>{t("INV410_Sec0_lbl_RegistrosSeleccionados")}</h3>
                                                <Grid
                                                    id="INV410_grid_RegistrosQuitar"
                                                    application="INV410"
                                                    rowsToFetch={30}
                                                    rowsToDisplay={15}
                                                    enableSelection
                                                    enableExcelExport
                                                    onBeforeFetch={appendParameters}
                                                    onBeforeApplyFilter={appendParameters}
                                                    onBeforeApplySort={appendParameters}
                                                    onBeforeMenuItemAction={appendParameters}
                                                    onBeforeFetchStats={appendParameters}
                                                    onBeforeInitialize={appendParameters}
                                                    onBeforeExportExcel={appendParameters}
                                                    onAfterMenuItemAction={onAfterMenuItemActionRegistros}
                                                />
                                            </Col>
                                        </Row>
                                    </div>

                                    <div className={ubicacionesClassName}>
                                        <Row>
                                            <Col lg={6}>
                                                <h3>{t("INV410_Sec0_lbl_UbicacionesDisponibles")}</h3>
                                                <Grid
                                                    id="INV410_grid_Ubicacion"
                                                    application="INV410"
                                                    rowsToFetch={30}
                                                    rowsToDisplay={15}
                                                    enableSelection
                                                    enableExcelExport
                                                    onBeforeFetch={appendParameters}
                                                    onBeforeApplyFilter={appendParameters}
                                                    onBeforeApplySort={appendParameters}
                                                    onBeforeMenuItemAction={appendParameters}
                                                    onBeforeFetchStats={appendParameters}
                                                    onBeforeInitialize={appendParameters}
                                                    onBeforeExportExcel={appendParameters}
                                                    onAfterMenuItemAction={onAfterMenuItemActionRegistros}
                                                />
                                            </Col>
                                            <Col lg={6}>
                                                <h3>{t("INV410_Sec0_lbl_UbicacionesSeleccionadas")}</h3>
                                                <Grid
                                                    id="INV410_grid_UbicacionQuitar"
                                                    application="INV410"
                                                    rowsToFetch={30}
                                                    rowsToDisplay={15}
                                                    enableSelection
                                                    enableExcelExport
                                                    onBeforeFetch={appendParameters}
                                                    onBeforeApplyFilter={appendParameters}
                                                    onBeforeApplySort={appendParameters}
                                                    onBeforeMenuItemAction={appendParameters}
                                                    onBeforeFetchStats={appendParameters}
                                                    onBeforeInitialize={appendParameters}
                                                    onBeforeExportExcel={appendParameters}
                                                    onAfterMenuItemAction={onAfterMenuItemActionRegistros}
                                                />
                                            </Col>
                                        </Row>
                                    </div>

                                    <div className={lpnClassName}>
                                        <Accordion>
                                            <AccordionSummary
                                                expandIcon={<ExpandMoreIcon />}
                                                aria-controls="panel1-content"
                                                id="panel1-header"
                                            >
                                                {t("INV410_Sec0_lbl_LpnFiltros")}
                                            </AccordionSummary>
                                            <AccordionDetails>
                                                <Row>
                                                    <Col>
                                                        <h4 className='form-title'>{t("INV410_Sec0_title_LpnAtrCab")}</h4>
                                                    </Col>
                                                </Row>
                                                <Row>
                                                    <div className="row mb-4 text-center">
                                                        <div className="col-12">
                                                            <Grid
                                                                id="INV410_grid_LpnAtrCab"
                                                                application="INV410"
                                                                rowsToFetch={30}
                                                                rowsToDisplay={5}
                                                                enableExcelExport
                                                                editable={true}
                                                                onBeforeFetch={onBeforeFetch}
                                                                onAfterApplySort={onAfterApplySort}
                                                            />
                                                        </div>
                                                    </div>
                                                </Row>
                                                <Row>
                                                    <div className="row mb-4 text-center">
                                                        <div className="col-12">
                                                            <FormButton id="btnAplicarFiltro" variant="primary" value={t("General_Sec0_btn_AplicarFiltro")} />
                                                        </div>
                                                    </div>
                                                </Row>
                                            </AccordionDetails>
                                        </Accordion>

                                        <br />

                                        <Row>
                                            <Col lg={6}>
                                                <h3>{t("INV410_Sec0_lbl_RegistrosDisponibles")}</h3>
                                                <Grid
                                                    id="INV410_grid_Lpn"
                                                    application="INV410"
                                                    rowsToFetch={30}
                                                    rowsToDisplay={15}
                                                    enableSelection
                                                    enableExcelExport
                                                    onBeforeFetch={appendParameters}
                                                    onBeforeApplyFilter={appendParameters}
                                                    onBeforeApplySort={appendParameters}
                                                    onBeforeMenuItemAction={appendParameters}
                                                    onBeforeFetchStats={appendParameters}
                                                    onBeforeInitialize={appendParameters}
                                                    onBeforeExportExcel={appendParameters}
                                                    onAfterMenuItemAction={onAfterMenuItemActionRegistros}
                                                />
                                            </Col>

                                            <Col lg={6}>
                                                <h3>{t("INV410_Sec0_lbl_RegistrosSeleccionados")}</h3>
                                                <Grid
                                                    id="INV410_grid_LpnQuitar"
                                                    application="INV410"
                                                    rowsToFetch={30}
                                                    rowsToDisplay={15}
                                                    enableSelection
                                                    enableExcelExport
                                                    onBeforeFetch={appendParameters}
                                                    onBeforeApplyFilter={appendParameters}
                                                    onBeforeApplySort={appendParameters}
                                                    onBeforeMenuItemAction={appendParameters}
                                                    onBeforeFetchStats={appendParameters}
                                                    onBeforeInitialize={appendParameters}
                                                    onBeforeExportExcel={appendParameters}
                                                    onAfterMenuItemAction={onAfterMenuItemActionRegistros}
                                                />
                                            </Col>
                                        </Row>
                                    </div>

                                    <div className={lpnDetalleClassName}>
                                        <Accordion>
                                            <AccordionSummary
                                                expandIcon={<ExpandMoreIcon />}
                                                aria-controls="panel1-content"
                                                id="panel1-header"
                                            >
                                                {t("INV410_Sec0_lbl_LpnFiltros")}
                                            </AccordionSummary>
                                            <AccordionDetails>
                                                <Row>
                                                    <Col lg={6}>
                                                        <h4 className='form-title'>{t("INV410_Sec0_title_LpnDetAtrCab")}</h4>
                                                    </Col>
                                                    <Col lg={6}>
                                                        <h4 className='form-title'>{t("INV410_Sec0_title_LpnDetAtrDet")}</h4>
                                                    </Col>
                                                </Row>
                                                <Row>
                                                    <Col lg={6}>
                                                        <Grid
                                                            id="INV410_grid_DetalleLpnAtrCab"
                                                            application="INV410"
                                                            rowsToFetch={30}
                                                            rowsToDisplay={5}
                                                            enableExcelExport
                                                            editable={true}
                                                            onBeforeFetch={onBeforeFetch}
                                                            onAfterApplySort={onAfterApplySort}
                                                        />
                                                    </Col>
                                                    <Col lg={6}>
                                                        <Grid
                                                            id="INV410_grid_DetalleLpnAtrDet"
                                                            application="INV410"
                                                            rowsToFetch={30}
                                                            rowsToDisplay={5}
                                                            enableExcelExport
                                                            editable={true}
                                                            onBeforeFetch={onBeforeFetch}
                                                            onAfterApplySort={onAfterApplySort}
                                                        />
                                                    </Col>
                                                </Row>
                                                <Row>
                                                    <div className="row mb-4 text-center">
                                                        <div className="col-12">
                                                            <FormButton id="btnAplicarFiltro" variant="primary" value={t("General_Sec0_btn_AplicarFiltro")} />
                                                        </div>
                                                    </div>
                                                </Row>
                                            </AccordionDetails>
                                        </Accordion>

                                        <br />

                                        <Row >
                                            <Col lg={6}>
                                                <h3>{t("INV410_Sec0_lbl_RegistrosDisponibles")}</h3>
                                                <Grid
                                                    id="INV410_grid_DetalleLpn"
                                                    application="INV410"
                                                    rowsToFetch={30}
                                                    rowsToDisplay={15}
                                                    enableSelection
                                                    enableExcelExport
                                                    onBeforeFetch={appendParameters}
                                                    onBeforeApplyFilter={appendParameters}
                                                    onBeforeApplySort={appendParameters}
                                                    onBeforeMenuItemAction={appendParameters}
                                                    onBeforeFetchStats={appendParameters}
                                                    onBeforeInitialize={appendParameters}
                                                    onBeforeExportExcel={appendParameters}
                                                    onAfterMenuItemAction={onAfterMenuItemActionRegistros}
                                                />
                                            </Col>
                                            <Col lg={6}>
                                                <h3>{t("INV410_Sec0_lbl_RegistrosSeleccionados")}</h3>
                                                <Grid
                                                    id="INV410_grid_DetalleLpnQuitar"
                                                    application="INV410"
                                                    rowsToFetch={30}
                                                    rowsToDisplay={15}
                                                    enableSelection
                                                    enableExcelExport
                                                    onBeforeFetch={appendParameters}
                                                    onBeforeApplyFilter={appendParameters}
                                                    onBeforeApplySort={appendParameters}
                                                    onBeforeMenuItemAction={appendParameters}
                                                    onBeforeFetchStats={appendParameters}
                                                    onBeforeInitialize={appendParameters}
                                                    onBeforeExportExcel={appendParameters}
                                                    onAfterMenuItemAction={onAfterMenuItemActionRegistros}
                                                />
                                            </Col>
                                        </Row>
                                    </div>

                                    <hr />

                                    <Row>
                                        <Col>
                                            <button id="btnVolverConfiguracion" className="btn btn-link mr-2" onClick={goToConfiguracion}>{t("INV410_Sec0_btn_Volver")}</button>
                                            &nbsp;
                                            <FormButton id="btnLoadSummary" className="btn btn-primary" label="INV410_Sec0_btn_Siguiente" />
                                        </Col>
                                    </Row>
                                </FormTabStep>
                                <FormTabStep id="tabFin" label="INV410_Sec0_lbl_FinInventario">
                                    <Row>
                                        <Col>
                                            <h3 className="mb-4">{t("INV410_Sec0_lbl_Resumen")}</h3>
                                            <Row>
                                                <Col>
                                                    {summaryContent}
                                                </Col>
                                            </Row>
                                        </Col>
                                    </Row>
                                    <hr />
                                    <Row>
                                        <Col>
                                            <button className="btn btn-link mr-2" onClick={goToRegistros}>{t("INV410_Sec0_btn_Volver")}</button>
                                            &nbsp;
                                            <FormButton id="btnHabilitar" variant="warning" label="INV410_Sec0_btn_HabilitarYFinalizar" />
                                            &nbsp;
                                            <FormButton id="btnFinish" variant="primary" label="INV410_Sec0_btn_FinalizarInv" />
                                        </Col>
                                    </Row>
                                </FormTabStep>
                            </FormTab>

                            <div className="row col-12 hidden">
                                <div className="row">
                                    <div className="col">
                                        <SubmitButton id="BtnSubmit" label="General_Sec0_btn_Confirmar" variant="primary" />&nbsp;
                                        <FormButton id="BtnConfirmarContinuar" label="General_Sec0_btn_ConfirmarRedirect" variant="warning" />&nbsp;
                                        <FormButton id="BtnActualizarParametro" label="General_Sec0_btn_ActualizarParametro" variant="success" />&nbsp;
                                        <FormButton id="hideFormButton" label="General_Sec0_btn_Cancelar" variant="danger" />&nbsp;
                                    </div>
                                </div>
                            </div>
                        </Col>
                    </Row>
                </div>
            </Form>
            <hr />
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="INV410_grid_1"
                        application="INV410"
                        rowsToFetch={30} rowsToDisplay={15}
                        enableExcelExport
                        onAfterButtonAction={onAfterButtonActionInventario}
                    />
                </div>
            </div>
        </Page >
    );


}
