import React, { useState, useEffect } from 'react';
import { Col, Modal, Row, Button } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldTextArea, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useForm } from '../../components/FormComponents/FormHook'
export const REC410SelecPos = (props) => {

    const fieldsOrder = ['cantidadSeparar', 'posicionEquipo'];
    const { focus, siguiente, anterior, reset, showLoadingOverlay, hideLoadingOverlay, isLoading, clearErrors } = useForm(fieldsOrder);

    const { t } = useTranslation();

    const [nexus, setNexus] = useState(null);
    const [stateForm, setStateForm] = useState(null);
    const [sugerencia, setSugerencia] = useState(props.sugerencia);
    const [initialize, setInitialize] = useState(false);
    const [showGrid, setShowGrid] = useState(false);
    const [isReabastecer, setIsReabastecer] = useState(false);
    const [_cantidadClasificada, setCantidadClasificada] = useState(0);
    const formClassName = (focus == "posicionEquipo") ? "" : "hidden";
    const validationSchema = {
        estacion: Yup.number().transform(value => (isNaN(value) ? undefined : value)),
        codigoProducto: Yup.string(),
        descripcionProducto: Yup.string(),
        cantidadOriginal: Yup.number().transform(value => (isNaN(value) ? undefined : value)),
        cantidadClasificada: Yup.number().transform(value => (isNaN(value) ? undefined : value)),
        cantidadSeparar: Yup.number().transform(value => (isNaN(value) ? undefined : value)),
        lote: Yup.string(),
        destino: Yup.string(),
        descripcionZona: Yup.string(),
        codigoZona: Yup.string(),
        posicionEquipo: Yup.string(),
        vencimiento: Yup.string(),
    };

    const initialValues = {
        estacion: props.estacion,
        codigoProducto: props.codigoProducto,
        descripcionProducto: props.descripcionProducto,
        cantidadOriginal: props.cantidad,
        cantidadSeparar: props.cantidad,
        cantidadClasificada: 0,
        lote: props.lote,
        vencimiento: props.vencimiento,
        posicionEquipo: "",
    };


    const onBeforeValidateField = (context, form, query, nexus) => {

        clearErrors(form);

        query.parameters = [
            { id: "etiqueta", value: props.etiqueta },
            { id: "tipoEtiqueta", value: props.tipoEtiqueta },
            { id: "Focus", value: focus },
            { id: "cdEmpresa", value: props.cdEmpresa },
            { id: "faixa", value: props.faixa },
            { id: "isReabastecer", value: isReabastecer },
            { id: "manejaSerie", value: props.manejaSerie },
        ];
    }

    const onBeforeInitialize = (context, form, data, nexus) => {
        clearErrors(form);
        props.hideLoadingOverlay();
        showLoadingOverlay("modal-body");
        document.getElementsByClassName("btn-close")[0].style.display = 'none';

        if (!data.parameters.length) {
            data.parameters = [
                { id: "estacion", value: props.estacion },
                { id: "etiqueta", value: props.etiqueta },
                { id: "tipoEtiqueta", value: props.tipoEtiqueta },
                { id: "codigoProducto", value: props.codigoProducto },
                { id: "descripcionProducto", value: props.descripcionProducto },
                { id: "lote", value: props.lote },
                { id: "cantidadOriginal", value: props.cantidad },
                { id: "cantidadSeparar", value: props.cantidad },
                { id: "cantidadClasificada", value: 0 },
                { id: "reabastecer", value: props.reabastecer },
                { id: "ignorarStock", value: props.ignorarStock },
                { id: "vencimiento", value: props.vencimiento },
                { id: "etiquetaLote", value: props.etiquetaLote },
                { id: "cdEmpresa", value: props.cdEmpresa },
                { id: "faixa", value: props.faixa },
                { id: "sugerencia", value: sugerencia },
                { id: "manejaSerie", value: props.manejaSerie },
            ];
        }
    }

    const onBeforeSubmit = (context, form, data, nexus) => {
        clearErrors(form);

        if (isLoading())
            context.abortServerCall = true;
        else {
            showLoadingOverlay("modal-body");
            data.parameters = [
                { id: "Focus", value: focus },
                { id: "etiqueta", value: props.etiqueta },
                { id: "tipoEtiqueta", value: props.tipoEtiqueta },
                { id: "sugerencia", value: sugerencia },
                { id: "codigoProducto", value: props.codigoProducto },
                { id: "lote", value: props.lote },
                { id: "cantidad", value: props.cantidad },
                { id: "reabastecer", value: props.reabastecer },
                { id: "ignorarStock", value: props.ignorarStock },
                { id: "vencimiento", value: props.vencimiento },
                { id: "etiquetaLote", value: props.etiquetaLote },
                { id: "cdEmpresa", value: props.cdEmpresa },
                { id: "faixa", value: props.faixa },
                { id: "isReabastecer", value: isReabastecer },
                { id: "manejaSerie", value: props.manejaSerie },
            ];
        }
    }

    const onAfterInitialize = (context, form, data, nexus) => {
        setNexus(nexus);
        setStateForm(form);

        let focusField = ""

        if (data.parameters.some(p => p.id === "focusField")) {
            focusField = data.parameters.find(p => p.id === "focusField").value;
        }

        setIsReabastecer(data.parameters.find(p => p.id === "isReabastecer").value);

        var sugerencia = data.parameters.find(p => p.id === "sugerencia");

        if (sugerencia && sugerencia.value) {
            setSugerencia(sugerencia.value);
        } else {
            setSugerencia("");
        }

        if (data.parameters.some(p => p.id === "Operacion") && data.parameters.find(p => p.id === "Operacion").value == "SIGUIENTE") {
            siguiente(form, focusField, 1);
        }

        nexus.getGrid("REC410SelecPos_grid_1").refresh();

        hideLoadingOverlay("modal-body");

        setInitialize(true);
    }

    const onAfterSubmit = (context, form, data, nexus) => {

        var cantidadOriginal = parseFloat(form.fields.find(f => f.id === "cantidadOriginal").value);
        var cantidadClasificada = parseFloat(form.fields.find(f => f.id === "cantidadClasificada").value);
        var cantidadPendiente = cantidadOriginal - cantidadClasificada;
        var sugerencia = data.parameters.find(p => p.id === "sugerencia");

        if (sugerencia && sugerencia.value) {
            setSugerencia(sugerencia.value);
        } else {
            setSugerencia("");
        }

        if (context.responseStatus === "ERROR" || data.notifications.find(x => x.type == 1)) {
            hideLoadingOverlay("modal-body");
        } else if (data.buttonId === "btnCerrar" || cantidadPendiente === 0) {
            props.onHide(cantidadPendiente, props.barraEtiqueta, props.manejaSerie);
            hideLoadingOverlay("modal-body");
        } else {
            let focusField = ""

            if (data.parameters.some(p => p.id === "focusField")) {
                focusField = data.parameters.find(p => p.id === "focusField").value;
            }
            if (data.parameters.some(p => p.id === "isReabastecer")) {
                setIsReabastecer(data.parameters.find(p => p.id === "isReabastecer").value);
            }
            if (data.parameters.some(p => p.id === "Operacion")) {
                if (data.parameters.find(p => p.id === "Operacion").value == "SIGUIENTE") {
                    siguiente(form, focusField, 1);
                } else if (data.parameters.find(p => p.id === "Operacion").value == "ANTERIOR") {
                    anterior(form, focusField, 1, false);
                } else {
                    var cantidadSeparar = props.cantidad;
                    var cantidadClasificada = 0;

                    if (data.parameters.some(p => p.id === "cantidadSeparar")) {
                        cantidadSeparar = data.parameters.find(p => p.id === "cantidadSeparar").value;
                        cantidadClasificada = data.parameters.find(p => p.id === "cantidadClasificada").value;

                    }
                    setCantidadClasificada(cantidadClasificada);

                    setInitialize(false);

                    if (data.parameters.some(p => p.id === "focusField")) {
                        focusField = data.parameters.find(p => p.id === "focusField").value;
                    }

                    setIsReabastecer(data.parameters.find(p => p.id === "isReabastecer").value);

                    var sugerencia = data.parameters.find(p => p.id === "sugerencia");

                    if (sugerencia && sugerencia.value) {
                        setSugerencia(sugerencia.value);
                    } else {
                        setSugerencia("");
                    }
                    anterior(form, focusField, 1, true);
                    nexus.getGrid("REC410SelecPos_grid_1").refresh();
                }
            }

            setStateForm(form);
            nexus.getGrid("REC410SelecPos_grid_1").refresh();
            hideLoadingOverlay("modal-body");
        }
    }

    const addParameters = (context, data, nexus) => {
        data.parameters = [
            {
                id: "estacion",
                value: props.estacion,
            },
            {
                id: "sugerencia",
                value: sugerencia,
            },
        ];

        if (stateForm) {
            data.parameters.push({
                id: "destino",
                value: stateForm.fields.find(f => f.id === "destino").value,
            });
            data.parameters.push({
                id: "zona",
                value: stateForm.fields.find(f => f.id === "codigoZona").value,
            });
        }
    }

    const onAfterFetch = (context, newRows, parameters, nexus) => {
        var equipoNuevo = parameters.find(p => p.id == "equipoNuevo").value;
        setShowGrid(equipoNuevo == "false");
    }



    useEffect(() => {
        if (initialize) {
            stateForm.fields.find(f => f.id === focus).readOnly = false;
            document.getElementsByName(focus)[0].focus();
        }

    }, [initialize]);


    const handleClick = () => {

        if (focus == "cantidadSeparar") {
            var cantidadOriginal = parseFloat(props.cantidad);
            var cantidadClasificada = parseFloat(_cantidadClasificada)
            var cantidadPendiente = cantidadOriginal - cantidadClasificada;
            props.onHide(cantidadPendiente, props.barraEtiqueta, props.manejaSerie);
        } else {
            nexus.getForm("REC410SelecPos_form_1").submit("btnCerrar");
        }
    }

    return (
        <Page
            application="REC410SelecPos"
            {...props}
        >
            <Form
                application="REC410SelecPos"
                id="REC410SelecPos_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={onBeforeInitialize}
                onAfterInitialize={onAfterInitialize}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={onAfterSubmit}
                onBeforeValidateField={onBeforeValidateField}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("REC410SelecPos_Sec0_mdl_SelecPosicion_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col md={6}>
                            <div className="form-group" >
                                <label htmlFor="codigoProducto">{t("REC410SelecPos_frm1_lbl_codigoProducto")}</label>
                                <Field name="codigoProducto" className="form-control-sm wis-field" readOnly />
                                <Field name="estacion" hidden className="form-control-sm wis-field" readOnly />
                                <Field name="vencimiento" hidden className="form-control-sm wis-field" readOnly />
                            </div>
                        </Col>
                        <Col md={6}>
                            <div className="form-group" >
                                <label htmlFor="lote">{t("REC410SelecPos_frm1_lbl_lote")}</label>
                                <Field name="lote" className="form-control-sm wis-field" readOnly />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col md={12}>
                            <div className="form-group" >
                                <label htmlFor="descripcionProducto">{t("REC410SelecPos_frm1_lbl_descripcionProducto")}</label>
                                <FieldTextArea name="descripcionProducto" className="form-control-sm wis-field" readOnly />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col md={4}>
                            <div className="form-group" >
                                <label htmlFor="cantidadSeparar">{t("REC410SelecPos_frm1_lbl_cantidadSeparar")}</label>
                                <Field name="cantidadSeparar" className="form-control-sm wis-field" onKeyDown={(event) => {
                                    if (event.key === "Enter") {
                                        nexus.getForm("REC410SelecPos_form_1").submit("cantidadSeparar");
                                    }
                                }} />
                                <StatusMessage for="cantidadSeparar" />
                            </div>
                        </Col>
                        <Col md={4}>
                            <div className="form-group" >
                                <label htmlFor="cantidadClasificada">{t("REC410SelecPos_frm1_lbl_cantidadClasificada")}</label>
                                <Field name="cantidadClasificada" className="form-control-sm wis-field" readOnly />
                            </div>
                        </Col>
                        <Col md={4}>
                            <div className="form-group" >
                                <label htmlFor="cantidadOriginal">{t("REC410SelecPos_frm1_lbl_cantidadOriginal")}</label>
                                <Field name="cantidadOriginal" className="form-control-sm wis-field" readOnly />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col md={6}>
                            <div className="form-group" >
                                <label htmlFor="destino">{t("REC410SelecPos_frm1_lbl_destino")}</label>
                                <Field name="destino" className="form-control-sm wis-field" readOnly />
                            </div>
                        </Col>
                        <Col md={6}>
                            <div className="form-group" >
                                <label htmlFor="descripcionZona">{t("REC410SelecPos_frm1_lbl_zona")}</label>
                                <Field name="descripcionZona" className="form-control-sm wis-field" readOnly />
                                <Field name="codigoZona" hidden className="form-control-sm wis-field" readOnly />
                            </div>
                        </Col>
                    </Row>
                    <Row style={{ display: (focus === "posicionEquipo" ? "" : "none") }}>
                        <Col md={12}>
                            <fieldset className="form-group border p-2 grid" style={{ display: (showGrid ? "" : "none") }}>
                                <legend align="center" className="w-auto">{t("REC410_grid_title_SugerenciaEquipos")}</legend>
                                <Grid
                                    id="REC410SelecPos_grid_1"
                                    rowsToFetch={10}
                                    rowsToDisplay={5}
                                    enableExcelExport
                                    application="REC410SelecPos"
                                    onBeforeInitialize={addParameters}
                                    onBeforeFetch={addParameters}
                                    onBeforeFetchStats={addParameters}
                                    onBeforeApplySort={addParameters}
                                    onBeforeExportExcel={addParameters}
                                    onAfterFetch={onAfterFetch}
                                />
                            </fieldset>
                            <p class="form-control-lg form-control text-center" style={{ display: (showGrid ? "none" : "") }}>
                                {t("REC410SelecPos_frm1_lbl_equipoNuevo")}
                            </p>
                        </Col>
                    </Row>
                    <Row>
                        <Col md={6}>
                            <div className={formClassName}>
                          
                            <div className="form-group" >
                                <label htmlFor="posicionEquipo">{t("REC410SelecPos_frm1_lbl_posicionEquipo")}</label>
                                <Field name="posicionEquipo" className="form-control-sm wis-field" onKeyDown={(event) => {
                                    if (event.key === "Enter") {
                                        nexus.getForm("REC410SelecPos_form_1").submit("posicionEquipo");
                                    }
                                }} />
                                <StatusMessage for="posicionEquipo" />
                                </div>
                            </div>
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button id="btnCerrar" variant="outline-secondary" onClick={handleClick}>   {t("General_Sec0_btn_Cerrar")}</Button>
                    <SubmitButton id="btnConfirmar" variant="primary" label="REC410SelecPos_frm1_btn_confirmar" />
                </Modal.Footer>
            </Form>
        </Page>
    );
};