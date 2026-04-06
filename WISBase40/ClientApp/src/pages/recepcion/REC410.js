import React, { useState, useEffect } from 'react';
import { Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldDate, FieldSelectAsync, FieldTextArea, FieldToggle, Form, FormButton, StatusMessage, FieldSelect } from '../../components/FormComponents/Form';
import { useForm } from '../../components/FormComponents/FormHook';
import { Page } from '../../components/Page';
import { REC410DetalleProductoSinClasificar } from './REC410DetalleProductoSinClasificar';
import { REC410SelecPos } from './REC410SeleccionarPosicion';
import { set } from 'date-fns/esm';
import $ from 'jquery';
export default function REC410(props) {

    const fieldsOrder = ['estacion', 'etiqueta', 'codigoProducto', 'lote', 'vencimiento', 'cantidad'];
    const { focus, siguiente, anterior, reset, clearErrors, resetFocus, showLoadingOverlay, hideLoadingOverlay, isLoading } = useForm(fieldsOrder);
    const resetFields = ["etiqueta", "tipoEtiqueta", "codigoProducto", "descripcionProducto", "lote", "grupo", "vencimiento", "cantidad"]
    const resetFieldsCloseClasificacion = ["codigoProducto", "descripcionProducto", "lote", "grupo", "vencimiento", "cantidad"]

    const { t } = useTranslation();

    const [showModalSeleccionPosicion, setShowModalSeleccionPosicion] = useState(false);
    const [showModalDetalleProductoSinClasificar, setShowModalDetalleProductoSinClasificar] = useState(false);
    const [showBtnFinalizar, setShowBtnFinalizar] = useState(false);
    const [reubicarEtiqueta, setReubicarEtiqueta] = useState(false);
    const [clasificarProductoNoEsperado, setClasificarProductoNoEsperado] = useState(false);
    const [clasificarLoteNoEsperado, setClasificarLoteNoEsperado] = useState(false);
    const [clasificarDeMas, setClasificarDeMas] = useState(false);
    const [productoAnterior, setProductoAnterior] = useState("");
    const [loteAnterior, setLoteAnterior] = useState("");
    const [barraEtiqueta, setBarraEtiqueta] = useState("");

    const [nexus, setNexus] = useState(null);
    const [submitFocus, setSubmitFocus] = useState(false);

    const [clasificacion, setClasificacion] = useState({});
    const [tipoEtiqueta, setTipoEtiqueta] = useState("");
    const [etiquetaExterna, setEtiquetaExterna] = useState("");
    const [estacion, setEstacion] = useState("");
    const [etiquetaLote, setEtiquetaLote] = useState("");
    const [cdEmpresa, setCdEmpresa] = useState("");
    const [faixa, setFaixa] = useState("");


    const validationSchema = {
        estacion: Yup.number().transform(value => (isNaN(value) ? undefined : value)),
        reabastecer: Yup.boolean(),
        ignorarStock: Yup.boolean(),
        etiqueta: Yup.string(),
        tipoEtiqueta: Yup.string(),
        codigoProducto: Yup.string(),
        descripcionProducto: Yup.string(),
        lote: Yup.string(),
        grupo: Yup.string(),
        vencimiento: Yup.string(),
        cantidad: Yup.string(),
    };

    const initialValues = {
        etiqueta: "",
        tipoEtiqueta: "",
        codigoProducto: "",
        descripcionProducto: "",
        lote: "",
        grupo: "",
        vencimiento: "",
    };

    useEffect(() => {
        if (submitFocus) {
            nexus.getForm("REC410_form").submit(focus);
            setSubmitFocus(false);
        }
    }, [focus, submitFocus]);

    const onAfterInitialize = (context, form, data, nexus) => {
        setNexus(nexus);
        form.fields.find(f => f.id === focus).readOnly = false;
        $("#ignorarStock").prop('disabled', true);
    };

    const onBeforeSubmit = (context, form, data, nexus) => {
        clearErrors(form);
        if (isLoading())
            context.abortServerCall = true;
        else {
            showLoadingOverlay();

            data.parameters = [
                { id: "Focus", value: focus }
            ];

            if (reubicarEtiqueta) {
                data.parameters.push({ id: "reubicarEtiqueta", value: "true" });
            }

            if (clasificarProductoNoEsperado) {
                data.parameters.push({ id: "productoAnterior", value: productoAnterior });
                data.parameters.push({ id: "clasificarProductoNoEsperado", value: "true" });
            }

            if (clasificarLoteNoEsperado) {
                data.parameters.push({ id: "loteAnterior", value: loteAnterior });
                data.parameters.push({ id: "clasificarLoteNoEsperado", value: "true" });
            }

            if (clasificarDeMas) {
                data.parameters.push({ id: "clasificarDeMas", value: "true" });
            }
            data.parameters.push({ id: "etiqueta", value: etiquetaExterna });
            data.parameters.push({ id: "tipoEtiqueta", value: tipoEtiqueta });
            data.parameters.push({ id: "cdEmpresa", value: cdEmpresa });
            setReubicarEtiqueta(false);

            if (data.buttonId !== "BorrarCampo") {
                setClasificarProductoNoEsperado(false);
                setClasificarLoteNoEsperado(false);
            }
            setClasificarDeMas(false);
        }
    };

    const onBeforeValidateField = (context, form, query, nexus) => {

        clearErrors(form);

        query.parameters = [
            {
                id: "etiqueta",
                value: etiquetaExterna,
            },
            {
                id: "tipoEtiqueta",
                value: tipoEtiqueta,
            },
            { id: "Focus", value: focus }
        ];
    }

    const onAfterSubmit = (context, form, data, nexus) => {

        if (context.responseStatus === "ERROR" || data.notifications.find(x => x.type == 1)) {
            hideLoadingOverlay();
        } else {
            let focusField = "";

            if (data.parameters.some(p => p.id === "etiquetaLote")) {
                let etiqueta = data.parameters.find(p => p.id === "etiqueta").value;
                let tipoEtiqueta = data.parameters.find(p => p.id === "tipoEtiqueta").value;

                setEtiquetaExterna(etiqueta);
                setTipoEtiqueta(tipoEtiqueta);

                form.fields.find(f => f.id === "etiqueta").value = etiqueta;
                document.getElementsByName("etiqueta")[0].value = etiqueta;
                form.fields.find(f => f.id === "tipoEtiqueta").value = tipoEtiqueta;
                document.getElementsByName("tipoEtiqueta")[0].value = tipoEtiqueta;

                setBarraEtiqueta(data.parameters.find(p => p.id === "barraEtiqueta").value)
                setEtiquetaLote(data.parameters.find(p => p.id === "etiquetaLote").value);
                setCdEmpresa(data.parameters.find(p => p.id === "cdEmpresa").value);
            }

            if (data.parameters.some(p => p.id === "faixa")) {
                setFaixa(data.parameters.find(p => p.id === "faixa").value);
            }

            if (data.parameters.some(p => p.id === "focusField")) {
                focusField = data.parameters.find(p => p.id === "focusField").value;
            }

            if (data.parameters.some(p => p.id === "Operacion")) {
                if (data.parameters.find(p => p.id === "Operacion").value == "SIGUIENTE") {

                    if (data.parameters.some(p => p.id === "showBtnFinalizar")) {
                        setShowBtnFinalizar(true);
                    }
                    siguiente(form, focusField, 1);
                } else if (data.parameters.find(p => p.id === "Operacion").value == "ANTERIOR") {
                    if (data.parameters.some(p => p.id === "showBtnFinalizar")) {
                        setShowBtnFinalizar(false);
                    }
                    anterior(form, focusField, 1);
                } else if (data.parameters.find(p => p.id === "Operacion").value == "ConfirmarReubicacionEtiqueta") {
                    nexus.showConfirmation({
                        message: "REC410_frm1_msg_reubicarEtiqueta",
                        acceptLabel: "General_Sec0_btn_CONFIRM_BOX_YES",
                        cancelLabel: "General_Sec0_btn_CONFIRM_BOX_NO",
                        onAccept: () => {
                            setReubicarEtiqueta(true);
                            setTimeout(() => {
                                nexus.getForm("REC410_form").submit("etiqueta");
                            }, 100);
                        },
                        onCancel: () => {
                            setReubicarEtiqueta(false);
                        }
                    });
                } else if (data.parameters.find(p => p.id === "Operacion").value == "ConfirmarProductoNoEsperado") {
                    nexus.showConfirmation({
                        message: "REC410_frm1_msg_productoNoEsperado",
                        acceptLabel: "General_Sec0_btn_CONFIRM_BOX_YES",
                        cancelLabel: "General_Sec0_btn_CONFIRM_BOX_NO",
                        onAccept: () => {
                            setClasificarProductoNoEsperado(true);
                            setTimeout(() => {
                                setProductoAnterior(form.fields.find(f => f.id === 'codigoProducto').value);
                                nexus.getForm("REC410_form").submit("BorrarCampo");
                            }, 100);
                        },
                        onCancel: () => {
                            nexus.getForm("REC410_form").submit("BorrarCampo");
                            setClasificarProductoNoEsperado(false);
                        }
                    });
                } else if (data.parameters.find(p => p.id === "Operacion").value == "ConfirmarLoteNoEsperado") {
                    nexus.showConfirmation({
                        message: "REC410_frm1_msg_loteNoEsperado",
                        acceptLabel: "General_Sec0_btn_CONFIRM_BOX_YES",
                        cancelLabel: "General_Sec0_btn_CONFIRM_BOX_NO",
                        onAccept: () => {
                            setClasificarLoteNoEsperado(true);
                            setTimeout(() => {
                                setLoteAnterior(form.fields.find(f => f.id === 'lote').value);
                                nexus.getForm("REC410_form").submit("BorrarCampo");
                            }, 100);
                        },
                        onCancel: () => {
                            nexus.getForm("REC410_form").submit("BorrarCampo");
                            setClasificarLoteNoEsperado(false);
                        }
                    });
                } else if (data.parameters.find(p => p.id === "Operacion").value == "ConfirmarCantidadDeMas") {
                    nexus.showConfirmation({
                        message: "REC410_frm1_msg_cantidadDeMas",
                        acceptLabel: "General_Sec0_btn_CONFIRM_BOX_YES",
                        cancelLabel: "General_Sec0_btn_CONFIRM_BOX_NO",
                        onAccept: () => {
                            setClasificarDeMas(true);
                            setTimeout(() => {
                                nexus.getForm("REC410_form").submit("cantidad");
                            }, 100);
                        },
                        onCancel: () => {
                            setClasificarDeMas(false);
                        }
                    });
                } else {
                    var sugerencia = "";

                    if (data.parameters.some(p => p.id === "sugerencia")) {
                        sugerencia = data.parameters.find(p => p.id === "sugerencia").value;
                    }

                    if (data.parameters.some(p => p.id === "focusField")) {
                        focusField = data.parameters.find(p => p.id === "focusField").value;
                    }

                    if (focusField && focus !== focusField) {
                        siguiente(form, focusField, 1);
                    }

                    setClasificacion({
                        estacion: form.fields.find(f => f.id === "estacion").value,
                        codigoProducto: form.fields.find(f => f.id === "codigoProducto").value,
                        descripcionProducto: form.fields.find(f => f.id === "descripcionProducto").value,
                        lote: form.fields.find(f => f.id === "lote").value,
                        cantidad: form.fields.find(f => f.id === "cantidad").value,
                        reabastecer: form.fields.find(f => f.id === "reabastecer").value,
                        ignorarStock: form.fields.find(f => f.id === "ignorarStock").value,
                        vencimiento: form.fields.find(f => f.id === "vencimiento").value,
                        etiquetaLote: data.parameters.find(p => p.id === "etiquetaLote").value,
                        cdEmpresa: data.parameters.find(p => p.id === "cdEmpresa").value,
                        faixa: data.parameters.find(p => p.id === "faixa").value,
                        sugerencia: sugerencia,
                        manejaSerie: data.parameters.find(p => p.id === "manejaSerie").value,
                    });
                    openFormDialogSeleccionPosicion();
                }

                if (data.parameters.find(p => p.id === "Operacion").value != "AbrirPopup")
                    hideLoadingOverlay();

            }
            else
                hideLoadingOverlay();
        }
    };

    const openFormDialogSeleccionPosicion = () => {
        setShowModalSeleccionPosicion(true);
    }

    const closeFormDialogSeleccionPosicion = (cantidadPendiente, numeroEtiqueta, manejaSerie) => {
        setShowModalSeleccionPosicion(false);

        if (cantidadPendiente > 0 && manejaSerie != "S") {

            nexus.getForm("REC410_form").readOnly = false;

            nexus.getForm("REC410_form").setFieldValue("cantidad", cantidadPendiente);
            resetFocus("cantidad");
        } else {
            nexus.getForm("REC410_form").readOnly = true;
            setShowBtnFinalizar(false);
            nexus.getForm("REC410_form").setFieldValue("etiqueta", numeroEtiqueta);
            reset(nexus.getForm("REC410_form"), "etiqueta", resetFieldsCloseClasificacion);
            setSubmitFocus(true);
        }
    }

    const closeFormDialogDetalleProductoSinClasificar = () => {
        setShowModalDetalleProductoSinClasificar(false);
        setShowBtnFinalizar(false);
        reset(nexus.getForm("REC410_form"), "estacion", resetFields);
        setSubmitFocus(true);
    }
    const onBeforeButtonAction = (context, form, query, nexus) => {
        query.parameters = [
            {
                id: "etiqueta",
                value: etiquetaExterna,
            },
            {
                id: "tipoEtiqueta",
                value: tipoEtiqueta,
            },
            { id: "Focus", value: focus }
        ];
    }
    const onAfterButtonAction = (context, form, query, nexus) => {

        if (query.parameters.find(d => d.id === "ProductoSinClasificar").value == "true") {
            setEstacion(form.fields.find(f => f.id === "estacion").value);
            setShowModalDetalleProductoSinClasificar(true);
        } else {
            nexus.showConfirmation({
                message: "REC410_frm1_msg_btnFinalizar",
                acceptLabel: "General_Sec0_btn_CONFIRM_BOX_YES",
                cancelLabel: "General_Sec0_btn_CONFIRM_BOX_NO",
                onAccept: () => { nexus.getForm("REC410_form").submit("btnFinalizar"); },
                onCancel: () => { }
            });
        }
    }
    const onAfterValidateField = (context, form, query, nexus) => {
        if (query.fieldId === "vencimiento" && query.parameters.find(d => d.id === "Error").value == "false") {
            nexus.getForm("REC410_form").submit("vencimiento");
        } else if (query.fieldId === "vencimiento" && query.parameters.find(d => d.id === "Error").value == "true") {
            hideLoadingOverlay();
        } else if (query.fieldId === "estacion" && query.parameters.find(d => d.id === "Error").value == "false") {
            nexus.getForm("REC410_form").submit("estacion");
        }
    }

    $('#reabastecer').change(function (event) {
        $("#ignorarStock").prop('disabled', !event.currentTarget.checked);
    })
    return (
        <Page
            title={t("REC410_Sec0_pageTitle_Titulo")}
            application="REC410"
            {...props}
        >
            <Form
                id="REC410_form"
                initialValues={initialValues}
                validationSchema={validationSchema}
                application="REC410"
                onAfterInitialize={onAfterInitialize}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={onAfterSubmit}
                onBeforeButtonAction={onBeforeButtonAction}
                onAfterButtonAction={onAfterButtonAction}
                onAfterValidateField={onAfterValidateField}
                onBeforeValidateField={onBeforeValidateField}
            >
                <Row>
                    <Col md={6}>
                        <div className="form-group">

                            <label htmlFor="estacion">{t("REC410_frm1_lbl_estacion")}</label>
                            <FieldSelect autoFocus={true} name="estacion" />
                            <StatusMessage for="estacion" />
                        </div>
                    </Col>
                    <Col md={3}>
                        <div className="form-group" style={{ marginTop: "40px" }}>
                            <FieldToggle name="reabastecer" label={t("REC410_frm1_lbl_reabastecer")} />
                        </div>
                    </Col>
                    <Col md={3}>
                        <div className="form-group" style={{ marginTop: "40px" }}>
                            <FieldToggle name="ignorarStock" label={t("REC410_frm1_lbl_ignorarStock")} />
                        </div>
                    </Col>
                </Row>
                <Row>
                    <Col md={6}>
                        <div className="form-group">
                            <label htmlFor="etiqueta">{t("REC410_frm1_lbl_etiqueta")}</label>
                            <Field name="etiqueta" onKeyPress={(event) => {
                                if (event.key === "Enter") {
                                    nexus.getForm("REC410_form").submit("etiqueta");
                                }
                            }} />
                            <StatusMessage for="etiqueta" />
                        </div>
                    </Col>
                    <Col md={6}>
                        <div className="form-group">
                            <label htmlFor="tipoEtiqueta">{t("REC410_frm1_lbl_tipoEtiqueta")}</label>
                            <Field name="tipoEtiqueta" readOnly />
                        </div>
                    </Col>
                </Row>
                <Row>
                    <Col md={6}>
                        <div className="form-group">
                            <label htmlFor="codigoProducto">{t("REC410_frm1_lbl_codigoProducto")}</label>
                            <Field name="codigoProducto" onKeyPress={(event) => {
                                if (event.key === "Enter") {
                                    nexus.getForm("REC410_form").submit("codigoProducto");
                                }
                            }} />
                            <StatusMessage for="codigoProducto" />
                        </div>
                    </Col>
                    <Col md={6}>
                        <div className="form-group">
                            <label htmlFor="grupo">{t("REC410_frm1_lbl_grupo")}</label>
                            <Field name="grupo" readOnly />
                        </div>
                    </Col>
                </Row>
                <Row>
                    <Col md={12}>
                        <div className="form-group">
                            <label htmlFor="descripcionProducto">{t("REC410_frm1_lbl_descripcionProducto")}</label>
                            <FieldTextArea name="descripcionProducto" readOnly />
                        </div>
                    </Col>
                </Row>
                <Row>
                    <Col md={6}>
                        <div className="form-group">
                            <label htmlFor="lote">{t("REC410_frm1_lbl_lote")}</label>
                            <Field name="lote" onKeyPress={(event) => {
                                if (event.key === "Enter") {
                                    nexus.getForm("REC410_form").submit("lote");
                                }
                            }} />
                            <StatusMessage for="lote" />
                        </div>
                    </Col>
                    <Col md={6}>
                        <div className="form-group">
                            <label htmlFor="vencimiento">{t("REC410_frm1_lbl_vencimiento")}</label>
                            <FieldDate name="vencimiento" onKeyDown={(event, fieldProps) => {
                                if (event.key === "Enter" && (!event.target.value || event.target.value === event.target.defaultValue)) {
                                    event.target.value = "";
                                    fieldProps.disabled = true;
                                    nexus.getForm("REC410_form").submit("vencimiento");
                                }
                            }} />
                            <StatusMessage for="vencimiento" />
                        </div>
                    </Col>
                </Row>
                <Row>
                    <Col md={6}>
                        <div className="form-group">
                            <label htmlFor="cantidad">{t("REC410_frm1_lbl_cantidad")}</label>
                            <Field name="cantidad" onKeyPress={(event) => {
                                if (event.key === "Enter") {
                                    nexus.getForm("REC410_form").submit("cantidad");
                                }
                            }} />
                            <StatusMessage for="cantidad" />
                        </div>
                    </Col>
                </Row>
                <Row>
                    <Col md={12}>
                        <div className="form-group">
                        </div>
                    </Col>
                </Row>
                <Row style={{ display: showBtnFinalizar ? "" : "none" }}>
                    <Col md={12}>
                        <div className="text-center">
                            <FormButton id="btnFinalizar" className="btn btn-primary" label="REC410_frm1_lbl_btnFinalizar" />
                        </div>
                    </Col>
                </Row>
            </Form>

            <Modal id="modal" show={showModalSeleccionPosicion} dialogClassName={"modal-50w"} backdrop="static" >
                <REC410SelecPos
                    show={showModalSeleccionPosicion}
                    onHide={closeFormDialogSeleccionPosicion}
                    estacion={clasificacion.estacion}
                    etiqueta={etiquetaExterna}
                    tipoEtiqueta={tipoEtiqueta}
                    codigoProducto={clasificacion.codigoProducto}
                    descripcionProducto={clasificacion.descripcionProducto}
                    lote={clasificacion.lote}
                    cantidad={clasificacion.cantidad}
                    reabastecer={clasificacion.reabastecer}
                    ignorarStock={clasificacion.ignorarStock}
                    vencimiento={clasificacion.vencimiento}
                    etiquetaLote={clasificacion.etiquetaLote}
                    cdEmpresa={cdEmpresa}
                    faixa={faixa}
                    sugerencia={clasificacion.sugerencia}
                    barraEtiqueta={barraEtiqueta}
                    hideLoadingOverlay={hideLoadingOverlay}
                    manejaSerie={clasificacion.manejaSerie}
                />
            </Modal>
            <Modal id="modal" show={showModalDetalleProductoSinClasificar} dialogClassName={"modal-50w"} backdrop="static" >
                <REC410DetalleProductoSinClasificar
                    show={showModalDetalleProductoSinClasificar}
                    onHide={closeFormDialogDetalleProductoSinClasificar}
                    etiqueta={etiquetaExterna}
                    tipoEtiqueta={tipoEtiqueta}
                    estacion={estacion}
                />
            </Modal>

        </Page>
    );
}