import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldSelect, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import { Modal, Button, Row, Col } from 'react-bootstrap';
import { EXP110ConfiguracionInicialModal } from './EXP110ConfiguracionInicialModal';
import { EXP110ImpresionBultosModal } from './EXP110ImpresionBultosModal';
import { EXP110SeleccionProductoModal } from './EXP110SeleccionProductoModal';
import { EXP110InformacionPedidoMesaEmpaque } from './EXP110InformacionPedidoMesaEmpaque';
import { PRE340InfDetallePedido } from '../preparacion/PRE340InfDetallePedido';
import { PRE340InfDetallePedidoCamion } from '../preparacion/PRE340InfDetallePedidoCamion';
import CloseButton from 'react-bootstrap/CloseButton';

import * as Yup from 'yup';
import { isNullOrUndefined } from 'util';


export default function EXP110(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [showPopup, setShowPopup] = useState(false);
    const [showProductoPopup, setShowProductoPopup] = useState(false);
    const [showImpresionBultoPopup, setShowImpresionBultoPopup] = useState(false);
    const [conf_inicial, setconf_inicial] = useState(null);
    const [nexus, setNexus] = useState(null);
    const [paremtrosDetalle, setParemtrosDetalleparemtrosDetalle] = useState(null);
    const [contenedorDestinoData, setContenedorDestinoData] = useState(null);
    const [contenedorOrigenData, setContenedorOrigenData] = useState(null);
    const [nuContenedor, setNuContenedor] = useState(null);
    const [idExternoContenedorOrigen, setIdExternoContenedorOrigen] = useState(null);
    const [nuPreparacion, setNuPreparacion] = useState(null);
    const [productoLeido, setProductoLeido] = useState(null);
    const [modalSelectProductoParameters, setModalSelectProductoParameters] = useState(null);
    const [auxContenedorDestino, setAuxContenedorDestino] = useState(null);
    const [auxContenedorBultoData, setAuxContenedorBultoData] = useState(null);

    const [showPedidosMesaEmpaquePopup, setShowPopupPedidosMesaEmpaquePopup] = useState(false);
    const [showInfoPedidosPopup, setshowInfoPedidosPopup] = useState(false);

    const [showInfoCamionesPedidosPopup, setshowInfoCamionesPedidosPopup] = useState(false);

    const [isHiddenBtnPedidoMesa, setisHiddenBtnPedidoMesa] = useState(true);
    const classBtnPedidoMesa = isHiddenBtnPedidoMesa ? "hidden" : "";

    const [Pedido, setPedido] = useState("");
    const [Cliente, setCliente] = useState("");
    const [Empresa, setEmpresa] = useState("");
    const [auxContenedorImpresion, setAuxContenedorImpresion] = useState(null);

    const [showModal, setshowModal] = useState(false);
    const [permiteCerrarEtiqueta, setPermiteCerrarEtiqueta] = useState(false);

    const initialValues = {
        contenedorDestino: "",
        idExternoContenedorDestino: "",
        contenedorOrigen: "",
        idExternoContenedorOrigen: "",
        codigoBarraProducto: "",
        descripcionProducto: "",
        codigoCliente: "",
        numeroPedido: "",
        descripcionEntrega: "",
        tipoExpedicion: "",
        codigoRuta: "",
        fechaEntrega: "",
        descripcionAnexo4: ""
    };


    //******** Modal Functions *************
    const openFormDialogPedidoEstacion = () => {
        if (conf_inicial != null) {
            setShowPopupPedidosMesaEmpaquePopup(true);
            if (showModal == false) {
                setshowModal(true);

            }
        }
    }
    const closeXFormDialog = (context, data, nexus) => {
        setshowInfoPedidosPopup(false);
        setShowPopupPedidosMesaEmpaquePopup(false);
        setshowInfoCamionesPedidosPopup(false);
        setshowModal(false);
    }

    const closeVolverFormDialog = () => {

        if (showPedidosMesaEmpaquePopup) {
            setshowInfoPedidosPopup(false);
            setShowPopupPedidosMesaEmpaquePopup(false);
            setshowInfoCamionesPedidosPopup(false);
            setshowModal(false);
        } else if (showInfoPedidosPopup) {
            setShowPopupPedidosMesaEmpaquePopup(true);
            setshowInfoPedidosPopup(false);
            setshowInfoCamionesPedidosPopup(false);
            setshowModal(true);
        } else if (showInfoCamionesPedidosPopup) {
            setShowPopupPedidosMesaEmpaquePopup(false);
            setshowInfoPedidosPopup(true);
            setshowInfoCamionesPedidosPopup(false);
            setshowModal(true);
        }
    }
    const closeFormDialog = (context, data, nexus) => {
        if (context) {

            if (context.parameters.some(x => x.id == "BTNID")) {

                if (context.parameters.find(x => x.id == "BTNID").value == "btnInfoPedido") {
                    setShowPopupPedidosMesaEmpaquePopup(false);
                    setPedido(context.parameters.find(x => x.id == "NU_PEDIDO").value);
                    setCliente(context.parameters.find(x => x.id == "CD_CLIENTE").value);
                    setEmpresa(context.parameters.find(x => x.id == "CD_EMPRESA").value);
                    setshowInfoPedidosPopup(true);
                } if (context.parameters.find(x => x.id == "BTNID").value == "btnInfoCamionPedido") {
                    setshowInfoPedidosPopup(false);
                    setShowPopupPedidosMesaEmpaquePopup(false);
                    setshowInfoCamionesPedidosPopup(true);
                }
            }
        }
    }
    const closePopup = (nexus, query) => {

        if (query && query.parameters.some(x => x.id == "CONF_INICIAL")) {
            setconf_inicial(query.parameters.find(x => x.id == "CONF_INICIAL").value);
            nexus.getForm("EXP110_form").submit("BtnLimpiarTodoFormulario");
            setFocusField("contenedorDestino");
            setisHiddenBtnPedidoMesa(false);
        }

        setShowPopup(false);
    }

    const closeImpresionBultoPopup = (nexus, imprimirResumenPicking, datosContBulto, query) => {
        setShowImpresionBultoPopup(false);

        if (!isNullOrUndefined(datosContBulto)) {
            setAuxContenedorBultoData(datosContBulto);
        }
        if (imprimirResumenPicking === "S") {
            nexus.showConfirmation({
                message: "EXP110_form1_Msg_ImprimirResumenEmpaquetado",
                acceptLabel: "General_Sec0_btn_CONFIRM_BOX_YES",
                cancelLabel: "General_Sec0_btn_CONFIRM_BOX_NO",
                onAccept: () => onAcceptConfirmationImprimirResumenEmpaquetadoBulto(nexus, "N"),
                onCancel: () => onCancelConfirmationImprimirResumenEmpaquetadoBulto(nexus, "N")
            });
        }
    }

    const closeProductoPopup = (nexus, auxContenedorImpresion, imprimirResumenPicking, pedidoCompleto, terminePedidoContOrigen, contOrigenVacio, facturoContenedorEmpaque, parameters, pesoNuevo, productoAnterior) => {
        setShowProductoPopup(false);
        if (!isNullOrUndefined(parameters)) {
            setAuxContenedorImpresion(auxContenedorImpresion);
            setModalSelectProductoParameters(parameters);

            if (!isNullOrUndefined(productoAnterior)) {
                nexus.getForm("EXP110_form").setFieldValue("productoAnterior", productoAnterior);
            }

            if (!isNullOrUndefined(pesoNuevo)) {
                nexus.getForm("EXP110_form").setFieldValue("pesoEmpaque", pesoNuevo);
            }

            if (imprimirResumenPicking === "S") {
                nexus.showConfirmation({
                    message: "EXP110_form1_Msg_ImprimirResumenEmpaquetado",
                    acceptLabel: "General_Sec0_btn_CONFIRM_BOX_YES",
                    cancelLabel: "General_Sec0_btn_CONFIRM_BOX_NO",
                    onAccept: () => onAcceptConfirmationImprimirResumenEmpaquetado(nexus, pedidoCompleto),
                    onCancel: () => onCancelConfirmationImprimirResumenEmpaquetado(nexus, pedidoCompleto)
                });
            }
            else if (facturoContenedorEmpaque === "S") {
                nexus.getForm("EXP110_form").submit("BtnLimpiarTodoFormulario");
            }
            else if (pedidoCompleto === "S") {
                nexus.getForm("EXP110_form").submit("BtnLimpiarCamposFormulario");
            }
            else if (terminePedidoContOrigen == "S" && contOrigenVacio == "S") {
                nexus.getForm("EXP110_form").submit("BtnLimpiarCamposFormulario");
            }
            else if (terminePedidoContOrigen == "S" && contOrigenVacio == "N") {
                var form = nexus.getForm("EXP110_form");
                form.setFieldValue("codigoBarraProducto", "");
                form.setFieldValue("descripcionProducto", "");

                nexus.getGrid("EXP110_grid_EmpaquetaPicking").reset();
                nexus.getGrid("EXP110_grid_ContPendientes").reset();
                nexus.getGrid("EXP110_grid_PickPend").reset();

                setFocusField("codigoBarraProducto");
            }
        }

    }
    //**************************************

    //******** Events Functions *************

    const onBeforeValidateField = (context, form, query, nexus) => {
        context.abortServerCall = true;
    }

    const onBeforeSubmit = (context, form, data, nexus) => {

        data.parameters = [
            {
                id: "CONF_INICIAL",
                value: conf_inicial
            },
            {
                id: "CONT_DESTINO_DATA",
                value: contenedorDestinoData
            },
            {
                id: "CONT_ORIGEN_DATA",
                value: contenedorOrigenData
            },
            {
                id: "AUX_CONT_ORIGEN_NU_CONTENEDOR",
                value: nuContenedor
            },
            {
                id: "AUX_CONT_ORIGEN_NU_PREPARACION",
                value: nuPreparacion
            }, {
                id: "AUX_CONT_ORIGEN_ID_EXTERNO_CONTENEDOR",
                value: idExternoContenedorOrigen
            },
            {
                id: "NU_PEDIDO",
                value: Pedido
            },
            {
                id: "CD_CLIENTE",
                value: Cliente
            },
            {
                id: "CD_EMPRESA",
                value: Empresa
            },
            {
                id: "AUX_CONT_IMPRESION",
                value: auxContenedorImpresion
            }
        ];


        if (data.buttonId === "codigoBarraProducto") {
            data.parameters.push({
                id: "CONT_ORIGEN_SUMMIT",
                value: "S"
            });
        }

        if (data.buttonId === "BtnImprimirResumenEmpaquetado") {
            if (modalSelectProductoParameters !== null) {
                data.parameters = modalSelectProductoParameters;
            }
        }

        if (data.buttonId === "BtnImprimirResumenEmpaquetadoBulto") {
            data.parameters.push({
                id: "AUX_DATOS_CONT_BULTO",
                value: auxContenedorBultoData
            });
        }

    };

    const onAfterSubmit = (context, form, data, nexus) => {

        if (context.responseStatus === "ERROR") return;

        var formaux = nexus.getForm("EXP110_form");

        if (data.parameters.some(x => x.id == "AUX_PRODUCTO_ANTERIOR")) {
            formaux.setFieldValue("productoAnterior", data.parameters.find(x => x.id == "AUX_PRODUCTO_ANTERIOR").value);
        }

        if (data.parameters.some(x => x.id == "AUX_CONT_IMPRESION")) {
            setAuxContenedorImpresion(data.parameters.find(x => x.id == "AUX_CONT_IMPRESION").value);
        }

        if (data.parameters.some(x => x.id == "EMPAQUETARTODO_COMPLETADO")) {

            formaux.setFieldValue("codigoBarraProducto", "");
            formaux.setFieldValue("descripcionProducto", "");

            setContenedorOrigenData(null);
            setFocusField("contenedorOrigen");

            nexus.getGrid("EXP110_grid_EmpaquetaPicking").reset();
            nexus.getGrid("EXP110_grid_ContPendientes").reset();
            nexus.getGrid("EXP110_grid_PickPend").reset();
            nexus.getForm("EXP110_form").submit("BtnLimpiarTodoFormulario");

            return;
        }

        if (data.parameters.some(x => x.id == "CONFIRMATION_MSG_EMPAQUETARTODO")) {
            var value = data.parameters.find(x => x.id == "CONFIRMATION_MSG_EMPAQUETARTODO").value;

            if (value === "S") {
                nexus.showConfirmation({
                    message: "EXP110_form1_Msg_EmpaquetarTodoQuestion",
                    acceptLabel: "General_Sec0_btn_CONFIRM_BOX_YES",
                    cancelLabel: "General_Sec0_btn_CONFIRM_BOX_NO",
                    onAccept: () => onAcceptConfirmationEmpaquetarTodo(nexus),
                    onCancel: () => onCancelConfirmationEmpaquetarTodo(nexus)
                });
            }
            else {
                if ((isNullOrUndefined(value) || value === "N") && data.buttonId !== "BtnEmpaquetarTodo") {
                    onAcceptConfirmationEmpaquetarTodo(nexus);
                }
            }
        }

        if (data.parameters.some(x => x.id == "CIERRE_ETIQUETA_COMPLETADO")) {

            formaux.setFieldValue("codigoBarraProducto", "");
            formaux.setFieldValue("descripcionProducto", "");

            setContenedorOrigenData(null);
            setFocusField("contenedorDestino");

            nexus.getGrid("EXP110_grid_EmpaquetaPicking").reset();
            nexus.getGrid("EXP110_grid_ContPendientes").reset();
            nexus.getGrid("EXP110_grid_PickPend").reset();
            nexus.getForm("EXP110_form").submit("BtnLimpiarTodoFormulario");

            return;
        }

        if (data.parameters.some(x => x.id == "CONFIRMATION_MSG_CIERRE_ETIQUETA")) {
            var value = data.parameters.find(x => x.id == "CONFIRMATION_MSG_CIERRE_ETIQUETA").value;

            if (value === "S") {
                nexus.showConfirmation({
                    message: "EXP110_form1_Msg_CerrarEtiquetaEmpaque",
                    acceptLabel: "General_Sec0_btn_CONFIRM_BOX_YES",
                    cancelLabel: "General_Sec0_btn_CONFIRM_BOX_NO",
                    onAccept: () => onAcceptConfirmationCierreEtiqueta(nexus),
                    onCancel: () => onCancelConfirmationCierreEtiqueta(nexus)
                });
            }
            else {
                if ((isNullOrUndefined(value) || value === "N") && data.buttonId !== "BtnCerrarEtiquetaEmpaque") {
                    onAcceptConfirmationCierreEtiqueta(nexus);
                }
            }
        }

        if (data.parameters.some(x => x.id == "AUX_CONT_ORIG_VACIO")) {

            nexus.getGrid("EXP110_grid_EmpaquetaPicking").reset();
            nexus.getGrid("EXP110_grid_ContPendientes").reset();
            nexus.getGrid("EXP110_grid_PickPend").reset();

            formaux.setFieldValue("codigoBarraProducto", "");
            formaux.setFieldValue("descripcionProducto", "");

            if (data.parameters.find(x => x.id == "AUX_CONT_ORIG_VACIO").value == "N") {

                setFocusField("codigoBarraProducto");
            }
            else {
                setContenedorOrigenData(null);
                setFocusField("contenedorOrigen");

                nexus.getForm("EXP110_form").submit("BtnLimpiarCamposFormulario");
            }

            return;
        }

        if (data.parameters.some(x => x.id == "CONFIRMATION_MSG")) {
            var value = data.parameters.find(x => x.id == "CONFIRMATION_MSG").value;

            if (value === "S") {
                var argMsgValue = data.parameters.find(x => x.id == "CONFIRMATION_MSG_ARG").value;
                nexus.showConfirmation({
                    message: "EXP110_form1_Msg_PedidoYaIniciado",
                    acceptLabel: "General_Sec0_btn_CONFIRM_BOX_YES",
                    cancelLabel: "General_Sec0_btn_CONFIRM_BOX_NO",
                    argsMessage: [argMsgValue],
                    onAccept: () => onAcceptConfirmation(nexus),
                    onCancel: () => onCancelConfirmation(nexus)
                });
            }
            else {
                if ((isNullOrUndefined(value) || value === "N") && data.buttonId !== "contenedorOrigenConfirmado") {
                    onAcceptConfirmation(nexus);
                }
            }
        }

        if (data.parameters.some(x => x.id == "CONT_DESTINO_DATA")) {

            var contDestinoData = data.parameters.find(x => x.id == "CONT_DESTINO_DATA").value;
            setContenedorDestinoData(contDestinoData);
        }

        if (data.parameters.some(x => x.id == "AUX_CONT_ORIGEN_NU_CONTENEDOR") && data.parameters.some(x => x.id == "AUX_CONT_ORIGEN_NU_PREPARACION")) {
            var contenedor = data.parameters.find(x => x.id == "AUX_CONT_ORIGEN_NU_CONTENEDOR").value;
            var preparacion = data.parameters.find(x => x.id == "AUX_CONT_ORIGEN_NU_PREPARACION").value;

            setNuContenedor(contenedor);
            setNuPreparacion(preparacion);

            if (data.parameters.some(x => x.id == "AUX_CONT_ORIGEN_ID_EXTERNO_CONTENEDOR")) {

                var idExternoOrigen = data.parameters.find(x => x.id == "AUX_CONT_ORIGEN_ID_EXTERNO_CONTENEDOR").value;
                setIdExternoContenedorOrigen(idExternoOrigen);
            }
        }

        if (data.buttonId === "contenedorOrigen" && form.fields.find(x => x.id == "contenedorOrigen").value === "") {
            setAuxContenedorImpresion(null);
            setContenedorDestinoData(null);
            setContenedorOrigenData(null);
            setNuContenedor(null);
            setNuPreparacion(null);
            setProductoLeido(null);
            setModalSelectProductoParameters(null);
            setAuxContenedorDestino(null);
            setAuxContenedorBultoData(null);

            nexus.getForm("EXP110_form").reset();
            nexus.getGrid("EXP110_grid_ContPendientes").reset();
            nexus.getGrid("EXP110_grid_PickPend").reset();
        }
        else if (data.buttonId === "contenedorDestino" && data.parameters.some(x => x.id == "CONF_INICIAL") && !isNullOrUndefined(data.parameters.find(x => x.id == "CONF_INICIAL").value)) {

            setFocusField("contenedorOrigen");
            if (data.parameters.some(x => x.id == "CONTENEDOR_DESTINO_NUEVO") && data.parameters.find(x => x.id == "CONTENEDOR_DESTINO_NUEVO").value !== "S") {
                nexus.getGrid("EXP110_grid_ContPendientes").reset();
                nexus.getGrid("EXP110_grid_PickPend").reset();
            }

        }
        else if (data.buttonId === "contenedorOrigenConfirmado" && data.parameters.some(x => x.id == "GO_CODIGO_BARRA_PROD")) {
            var setFocusCodigoBarras = data.parameters.find(x => x.id == "GO_CODIGO_BARRA_PROD").value;
            if (setFocusCodigoBarras === "S") {

                if (data.parameters.some(x => x.id == "CONT_ORIGEN_DATA")) {
                    setContenedorOrigenData(data.parameters.find(x => x.id == "CONT_ORIGEN_DATA").value);
                }

                setTimeout(() => {
                    setFocusField("codigoBarraProducto");
                }, 100);

                context.abortServerCall = true;
                nexus.getGrid("EXP110_grid_EmpaquetaPicking").reset();
                nexus.getGrid("EXP110_grid_ContPendientes").reset();
                nexus.getGrid("EXP110_grid_PickPend").reset();
            }
        }
        else if (data.buttonId === "codigoBarraProducto" && form.fields.find(x => x.id == "codigoBarraProducto").value === "") {

            context.abortServerCall = true;
            setContenedorOrigenData(null);

            setFocusField("contenedorOrigen");

            setTimeout(() => {

            }, 100);


            nexus.getForm("EXP110_form").submit("BtnLimpiarCamposFormulario");
            nexus.getGrid("EXP110_grid_EmpaquetaPicking").reset();
            nexus.getGrid("EXP110_grid_ContPendientes").reset();
            nexus.getGrid("EXP110_grid_PickPend").reset();
        }
        else if (data.buttonId === "codigoBarraProducto" && form.fields.find(x => x.id == "codigoBarraProducto").value !== "") {

            if (data.parameters.some(x => x.id == "AUX_PROD_LEIDO")) {
                setProductoLeido(data.parameters.find(x => x.id == "AUX_PROD_LEIDO").value);
                if (data.parameters.some(x => x.id == "AUX_CONTENEDOR_DESTINO_JSON")) {
                    setAuxContenedorDestino(data.parameters.find(x => x.id == "AUX_CONTENEDOR_DESTINO_JSON").value);
                }
                setShowProductoPopup(true);
            }

        }
        else if (data.buttonId === "BtnImprimirResumenEmpaquetado") {

            setFocusField("contenedorOrigen");
            nexus.getGrid("EXP110_grid_EmpaquetaPicking").reset();
            nexus.getGrid("EXP110_grid_ContPendientes").reset();
            nexus.getGrid("EXP110_grid_PickPend").reset();
        }
        else if (data.buttonId === "BtnLimpiarCamposFormulario") {

            setFocusField("contenedorOrigen");
            nexus.getGrid("EXP110_grid_EmpaquetaPicking").reset();
            nexus.getGrid("EXP110_grid_ContPendientes").reset();
            nexus.getGrid("EXP110_grid_PickPend").reset();
        }
        else if (data.buttonId === "BtnLimpiarTodoFormulario") {

            setAuxContenedorImpresion(null);
            setContenedorDestinoData(null);
            setContenedorOrigenData(null);
            setNuContenedor(null);
            setNuPreparacion(null);
            setProductoLeido(null);
            setModalSelectProductoParameters(null);
            setAuxContenedorDestino(null);
            setAuxContenedorBultoData(null);

            setFocusField("contenedorDestino");
            nexus.getGrid("EXP110_grid_EmpaquetaPicking").reset();
            nexus.getGrid("EXP110_grid_ContPendientes").reset();
            nexus.getGrid("EXP110_grid_PickPend").reset();
        }

        data.parameters = [
            {
                id: "CONF_INICIAL",
                value: conf_inicial
            },
            {
                id: "CONT_DESTINO_DATA",
                value: contenedorDestinoData
            },
            {
                id: "CONT_ORIGEN_DATA",
                value: contenedorOrigenData
            },
            {
                id: "AUX_CONT_ORIGEN_NU_CONTENEDOR",
                value: nuContenedor
            },
            {
                id: "AUX_CONT_ORIGEN_NU_PREPARACION",
                value: nuPreparacion
            },
            {
                id: "AUX_CONT_ORIGEN_ID_EXTERNO_CONTENEDOR",
                value: idExternoContenedorOrigen
            },
            {
                id: "AUX_PROD_LEIDO",
                value: productoLeido
            },

            {
                id: "NU_PEDIDO",
                value: Pedido
            },
            {
                id: "CD_CLIENTE",
                value: Cliente
            },
            {
                id: "CD_EMPRESA",
                value: Empresa
            }
        ];
    };

    const validationSchema = {
        contenedorDestino: Yup.string(),
        contenedorOrigen: Yup.string(),
        codigoBarraProducto: Yup.string(),
        //modalidad: Yup.string()
    };

    const onAfterLoad = (data) => {

        setShowPopup(true);

        if (data && data.parameters) {
            setPermiteCerrarEtiqueta(data.parameters.find(d => d.id === "PermiteCerrarEtiqueta").value === "true");
        } else {
            setPermiteCerrarEtiqueta(false);
        }
    };

    const onAfterInitialize = (context, form, data, nexus) => {
        setNexus(nexus);
        setFocusField("contenedorDestino");
    };

    //**************************************

    //******** Helpers Functions *************  
    const addParametersForm = (context, form, data, nexus) => {
        data.parameters = [
            {
                id: "CONF_INICIAL",
                value: conf_inicial
            },
            {
                id: "CONT_DESTINO_DATA",
                value: contenedorDestinoData
            },
            {
                id: "CONT_ORIGEN_DATA",
                value: contenedorOrigenData
            },
            {
                id: "AUX_CONT_ORIGEN_NU_CONTENEDOR",
                value: nuContenedor
            },
            {
                id: "AUX_CONT_ORIGEN_NU_PREPARACION",
                value: nuPreparacion
            },
            {
                id: "AUX_CONT_ORIGEN_ID_EXTERNO_CONTENEDOR",
                value: idExternoContenedorOrigen
            },
            {
                id: "AUX_PROD_LEIDO",
                value: productoLeido
            },
            {
                id: "NU_PEDIDO",
                value: Pedido
            },
            {
                id: "CD_CLIENTE",
                value: Cliente
            },
            {
                id: "CD_EMPRESA",
                value: Empresa
            }
        ];
    };

    function GetParametersFormArray() {
        return ([
            {
                id: "CONF_INICIAL",
                value: conf_inicial
            },
            {
                id: "CONT_DESTINO_DATA",
                value: contenedorDestinoData
            },
            {
                id: "CONT_ORIGEN_DATA",
                value: contenedorOrigenData
            },
            {
                id: "AUX_CONT_ORIGEN_NU_CONTENEDOR",
                value: nuContenedor
            },
            {
                id: "AUX_CONT_ORIGEN_NU_PREPARACION",
                value: nuPreparacion
            },
            {
                id: "AUX_CONT_ORIGEN_ID_EXTERNO_CONTENEDOR",
                value: idExternoContenedorOrigen
            },
            {
                id: "AUX_PROD_LEIDO",
                value: productoLeido
            },
            {
                id: "AUX_CONTENEDOR_DESTINO_JSON",
                value: auxContenedorDestino
            }
        ]);
    };

    const addParameters = (context, data, nexus) => {
        data.parameters = [
            {
                id: "CONF_INICIAL",
                value: conf_inicial
            },
            {
                id: "CONT_DESTINO_DATA",
                value: contenedorDestinoData
            },
            {
                id: "CONT_ORIGEN_DATA",
                value: contenedorOrigenData
            },
            {
                id: "AUX_CONT_ORIGEN_NU_CONTENEDOR",
                value: nuContenedor
            },
            {
                id: "AUX_CONT_ORIGEN_NU_PREPARACION",
                value: nuPreparacion
            },
            {
                id: "AUX_CONT_ORIGEN_ID_EXTERNO_CONTENEDOR",
                value: idExternoContenedorOrigen
            },
            {
                id: "AUX_PROD_LEIDO",
                value: productoLeido
            },
            {
                id: "NU_PEDIDO",
                value: Pedido
            },
            {
                id: "CD_CLIENTE",
                value: Cliente
            },
            {
                id: "CD_EMPRESA",
                value: Empresa
            }
        ];


        if (data.parameters.some(x => x.id == "CONT_DESTINO_DATA") &&
            (data.parameters.find(x => x.id == "CONT_DESTINO_DATA").value === "" ||
                data.parameters.find(x => x.id == "CONT_DESTINO_DATA").value === null)) {
            var form = nexus.getForm("EXP110_form");

            var value = form.getFieldValue("contenedorDestino");
            if (value !== "(NUEVO)") {
                data.parameters.push({ id: "AUX_FIELD_CONT_DESTINO", value: value });
            }
        }

        setParemtrosDetalleparemtrosDetalle(data.parameters);
    };

    const setFocusField = (fieldName) => {

        document.getElementsByName(fieldName)[0].focus();
        document.getElementsByName(fieldName)[0].readOnly = false;

        var inputs = document.getElementsByClassName("wis-field");
        for (var i = 0; i < inputs.length; i++) {

            if (inputs.item(i).name != fieldName) {
                inputs.item(i).readOnly = true;
            }
        }
    }

    const onAcceptConfirmationCierreEtiqueta = (nexus) => {
        nexus.getForm("EXP110_form").submit("BtnConfirmarCerrarEtiquetaEmpaque");
    }
    const onCancelConfirmationCierreEtiqueta = (nexus) => {

    }

    const onAcceptConfirmationEmpaquetarTodo = (nexus) => {
        nexus.getForm("EXP110_form").submit("contenedorOrigenConfirmadoEmpaquetarTodo");
    }

    const onAcceptConfirmation = (nexus) => {
        nexus.getForm("EXP110_form").submit("contenedorOrigenConfirmado");
    }

    const onCancelConfirmationEmpaquetarTodo = (nexus) => {

    }

    const onCancelConfirmation = (nexus) => {
        nexus.getForm("EXP110_form").setFieldValue("contenedorOrigen", "");
    }

    const onAcceptConfirmationImprimirResumenEmpaquetado = (nexus, pedidoCompleto) => {
        nexus.getForm("EXP110_form").submit("BtnImprimirResumenEmpaquetado");
    }

    const onCancelConfirmationImprimirResumenEmpaquetado = (nexus, pedidoCompleto) => {

        if (pedidoCompleto === "S") {
            nexus.getForm("EXP110_form").submit("BtnLimpiarCamposFormulario");
        }
    }

    const onAcceptConfirmationImprimirResumenEmpaquetadoBulto = (nexus, pedidoCompleto) => {
        nexus.getForm("EXP110_form").submit("BtnImprimirResumenEmpaquetadoBulto");
    }

    const onCancelConfirmationImprimirResumenEmpaquetadoBulto = (nexus, pedidoCompleto) => {

        //if (pedidoCompleto === "S") {
        //    nexus.getForm("EXP110_form").submit("BtnLimpiarCamposFormulario");
        //}
    }
    //**************************************

    const showFormPedidosMesaEmpaquePopup = () => {

        return (<EXP110InformacionPedidoMesaEmpaque show={showPedidosMesaEmpaquePopup} onHide={closeFormDialog} addParameters={addParameters} addParametersForm={addParametersForm} />);
    }

    const showFormshowInfoPedidosPopup = () => {

        return (<PRE340InfDetallePedido show={showInfoPedidosPopup} onHide={closeFormDialog} addParameters={addParameters} addParametersForm={addParametersForm} />);
    }

    const showFormCamionesPedidosPopup = () => {

        return (<PRE340InfDetallePedidoCamion show={showFormCamionesPedidosPopup} onHide={closeFormDialog} addParameters={addParameters} addParametersForm={addParametersForm} />);
    }

    const handleFormAfterValidateField = (context, form, query, nexus) => {

    };

    const keyPress = (evt) => {
        if (evt.key === "F8") {
            nexus.getForm("EXP110_form").submit("BtnConfirmarEmpaquetarTodo");
        }
    }

    const showBtnCerrarEtiqueta = () => {
        return (
            <Button title={t("EXP110_frm1_btn_CerrarEtiqueta")} id="BtnCerrarEtiquetaEmpaque" className="ml-2 mr-2 btn btn-outline-primary form-control-sm" variant="outline-primary" onClick={() => {

                var elemento = document.getElementsByName("contenedorDestino")[0];
                var valorOrigen = elemento.value;

                if (!valorOrigen || valorOrigen == "(NUEVO)") {
                    nexus.toastException({ name: "", message: "EXP110_form1_Error_ContenedorDestinoNulo" });
                } else {
                    nexus.getForm("EXP110_form").submit("BtnPreConfirmarCerrarEtiqueta");
                }

            }} ><i className="fa fa-box" /></Button>
        );
    }

    return (
        <Page
            title={t("EXP110_Sec0_pageTitle_Titulo")}
            application="EXP110Form"
            onAfterLoad={onAfterLoad}
            {...props}
        >

            <Form
                id="EXP110_form"
                initialValues={initialValues}
                validationSchema={validationSchema}
                application="EXP110Form"
                onAfterInitialize={onAfterInitialize}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={onAfterSubmit}
                onBeforeValidateField={onBeforeValidateField}
                onAfterValidateField={handleFormAfterValidateField}
            >
                <Row>
                    <Col md={2}>
                        <div className="form-group">
                            <label className="form-control-sm" htmlFor="contenedorDestino">{t("EXP110_frm1_lbl_contenedorDestino")}</label>
                            <Field className="form-control-sm wis-field" name="contenedorDestino" onKeyPress={(event) => {
                                if (event.key === "Enter") {
                                    nexus.getForm("EXP110_form").submit("contenedorDestino");
                                }
                            }} />
                            <StatusMessage for="contenedorDestino" />
                        </div>
                        <div className="form-group" hidden>
                            <label className="form-control-sm" htmlFor="preparacionDestino">{t("EXP110_frm1_lbl_preparacionDestino")}</label>
                            <Field className="form-control-sm wis-field" name="preparacionDestino" />
                            <StatusMessage for="preparacionDestino" />
                        </div>
                    </Col>
                    <Col md={2}>
                        <div className="form-group">
                            <label className="form-control-sm" htmlFor="idExternoContenedorDestino">{t("EXP110_frm1_lbl_idExternoContenedorDestino")}</label>
                            <Field className="form-control-sm wis-field" name="idExternoContenedorDestino" />
                            <StatusMessage for="idExternoContenedorDestino" />
                        </div>
                    </Col>
                    <Col md={4}>
                        <div className="form-group">
                            <label className="form-control-sm" htmlFor="descripcionEntrega" >{t("EXP110_frm1_lbl_descripcionEntrega")}</label>
                            <Field className="form-control-sm wis-field" name="descripcionEntrega" readOnly />
                            <StatusMessage for="descripcionEntrega" />
                        </div>

                    </Col>
                    <Col md={4}>
                        <div className="form-group">
                            <label className="form-control-sm" htmlFor="descripcionAnexo4">{t("EXP110_frm1_lbl_descripcionAnexo4")}</label>
                            <Field className="form-control-sm wis-field" name="descripcionAnexo4" readOnly />
                            <StatusMessage for="descripcionAnexo4" />
                        </div>
                    </Col>
                </Row>
                <Row>
                    <Col md={2}>
                        <div className="form-group">
                            <label className="form-control-sm" htmlFor="fechaEntrega">{t("EXP110_frm1_lbl_fechaEntrega")}</label>
                            <Field className="form-control-sm wis-field" name="fechaEntrega" readOnly />
                            <StatusMessage for="fechaEntrega" />
                        </div>
                    </Col>
                    <Col md={2}>
                        <div className="form-group">
                            <label className="form-control-sm" htmlFor="pesoEmpaque">{t("EXP110_frm1_lbl_pesoEmpaque")}</label>
                            <Field className="form-control-sm wis-field" name="pesoEmpaque" readOnly />
                            <StatusMessage for="pesoEmpaque" />
                        </div>
                    </Col>
                    <Col md={4}>
                        <div className="form-group">
                            <label className="form-control-sm" htmlFor="numeroPedido">{t("EXP110_frm1_lbl_numeroPedido")}</label>
                            <Field className="form-control-sm wis-field" name="numeroPedido" readOnly />
                            <StatusMessage for="numeroPedido" />
                        </div>
                    </Col>
                    <Col md={4}>
                        <div className="form-group">
                            <label className="form-control-sm" htmlFor="codigoCliente">{t("EXP110_frm1_lbl_codigoCliente")}</label>
                            <Field className="form-control-sm wis-field" name="codigoCliente" readOnly />
                            <StatusMessage for="codigoCliente" />
                        </div>
                    </Col>

                </Row>
                <Row>
                    <Col md={2}>
                        <div className="form-group">
                            <label className="form-control-sm" htmlFor="contenedorOrigen">{t("EXP110_frm1_lbl_contenedorOrigen")}</label>
                            <Field className="form-control-sm wis-field" name="contenedorOrigen" readOnly onKeyDown={keyPress} onKeyPress={(event) => {
                                if (event.key === "Enter") {
                                    nexus.getForm("EXP110_form").submit("contenedorOrigen");
                                }
                            }} />
                            <StatusMessage for="contenedorOrigen" />
                        </div>
                    </Col>
                    <Col md={2}>
                        <div className="form-group">
                            <label className="form-control-sm" htmlFor="idExternoContenedorOrigen">{t("EXP110_frm1_lbl_idExternoContenedorOrigen")}</label>
                            <Field className="form-control-sm wis-field" name="idExternoContenedorOrigen" readOnly />
                            <StatusMessage for="idExternoContenedorOrigen" />
                        </div>
                    </Col>
                    <Col md={4}>
                        <div className="form-group">
                            <label className="form-control-sm" htmlFor="codigoRuta">{t("EXP110_frm1_lbl_codigoRuta")}</label>
                            <Field className="form-control-sm wis-field" name="codigoRuta" readOnly />
                            <StatusMessage for="codigoRuta" />
                        </div>
                    </Col>
                    <Col md={4} className="pl-0">
                        <div className="form-group">
                            <label className="form-control-sm" htmlFor="tipoPedido">{t("EXP110_frm1_lbl_tipoPedido")}</label>
                            <Field className="form-control-sm wis-field" name="tipoPedido" readOnly />
                            <StatusMessage for="tipoPedido" />
                        </div>
                    </Col>
                </Row>
                <Row>

                    <Col md={2}>
                        <div className="form-group" /> {/*visual*/}
                        <div className="form-group">
                            <label className="form-control-sm" htmlFor="codigoBarraProducto">{t("EXP110_frm1_lbl_codigoBarraProducto")}</label>
                            <Field className="form-control-sm wis-field" name="codigoBarraProducto" readOnly onKeyPress={(event) => {
                                if (event.key === "Enter") {
                                    nexus.getForm("EXP110_form").submit("codigoBarraProducto");
                                }
                            }} />
                            <StatusMessage for="codigoBarraProducto" />
                        </div>
                    </Col>
                    <Col md={2}>
                        <div className="form-group" /> {/*visual*/}
                        <div className="form-group">
                            <label className="form-control-sm" htmlFor="descripcionProducto">{t("EXP110_frm1_lbl_descripcionProducto")}</label>
                            <Field className="form-control-sm wis-field" name="descripcionProducto" readOnly />
                            <StatusMessage for="descripcionProducto" />
                        </div>
                    </Col>
                    <Col>
                        <div className="mb-2" style={{ textAlign: "center", width: "100%" }}>
                            <Button title={t("EXP110_frm1_lbl_BtnPedidosEstacion")} id="BtnPedidos" className={classBtnPedidoMesa + "ml-2 mr-2 btn btn-outline-primary form-control-sm"} variant="outline-primary" onClick={openFormDialogPedidoEstacion} ><i className="fa fa-clipboard" /></Button>
                            <Button title={t("EXP110_frm1_lbl_BtnEspecificacionBulto")} id="BtnBultos" className="ml-2 mr-2 btn btn-outline-primary form-control-sm" variant="outline-primary" onClick={() => {
                                setShowImpresionBultoPopup(true);
                            }} ><i className="fa fa-archive" /></Button>
                            <Button title={t("EXP110_frm1_lbl_BtnConfiguracion")} id="BtnConfiguracion" className="ml-2 mr-2 btn btn-outline-primary form-control-sm" variant="outline-primary" onClick={() => {
                                setShowPopup(true);

                            }} ><i className="fa fa-wrench" /></Button>
                            <Button title={t("EXP110_frm1_lbl_BtnEmpaquetarTodo")} id="BtnEmpaquetarTodo" className="ml-2 mr-2 btn btn-outline-primary form-control-sm" variant="outline-primary" onClick={() => {

                                var elemento = document.getElementsByName("contenedorOrigen")[0];

                                if (!elemento.readOnly) {
                                    nexus.toastException({ name: "", message: "EXP110_frm1_error_NuContenedorOrigenNull" });
                                }
                                else {

                                    var valorOrigen = elemento.value;

                                    if (!valorOrigen) {
                                        nexus.toastException({ name: "", message: "EXP110_frm1_error_NuContenedorOrigenNull" });
                                    } else {
                                        nexus.getForm("EXP110_form").submit("BtnConfirmarEmpaquetarTodo");
                                    }
                                }

                            }} ><i className="fa fa-reply-all" /></Button>

                            {permiteCerrarEtiqueta ? showBtnCerrarEtiqueta() : null}
                        </div>

                        <Field className="form-control-sm mb-3 wis-field" name="productoAnterior" readOnly />
                    </Col>

                    <Col md={4} className="hidden pl-0">
                        <div className="form-group" /> {/*visual*/}
                        <div className="form-group">
                            <label htmlFor="modalidad">{t("EXP110ConfInicial_frm1_lbl_modalidad")}</label>
                            <FieldSelect name="modalidad" />
                            <StatusMessage for="modalidad" />
                        </div>
                    </Col>
                    <Col md={4} className="pl-0">
                        <div className="form-group" /> {/*visual*/}
                        <div className="form-group">
                            <label className="form-control-sm" htmlFor="tipoExpedicion">{t("EXP110_frm1_lbl_tipoExpedicion")}</label>
                            <Field className="form-control-sm wis-field" name="tipoExpedicion" readOnly />
                            <StatusMessage for="tipoExpedicion" />
                        </div>
                    </Col>
                </Row>
            </Form >

            <Row>
                <Col>
                    <Row>
                        <Col>
                            <Grid
                                id="EXP110_grid_EmpaquetaPicking"
                                rowsToFetch={10}
                                rowsToDisplay={5}
                                enableExcelExport
                                application="EXP110EmpaquetadoPicking"
                                onBeforeInitialize={addParameters}
                                onBeforeFetch={addParameters}
                                onBeforeApplySort={addParameters}
                                onBeforeApplyFilter={addParameters}
                                onBeforeExportExcel={addParameters}
                                onBeforeFetchStats={addParameters}
                            />
                        </Col>
                    </Row>
                </Col>
            </Row>

            <Row>
                <Col lg="6">

                    <fieldset className="form-group border p-2 grid" >
                        <legend align="center" className="w-auto">{t("EXP110_grid1_title_ContenedoresPendientes")}</legend>
                        <Row>
                            <Col>
                                <Grid id="EXP110_grid_ContPendientes"
                                    rowsToFetch={10}
                                    rowsToDisplay={5}
                                    enableExcelExport
                                    application="EXP110ContenedoresPedido"
                                    onBeforeFetch={addParameters}
                                    onBeforeInitialize={addParameters}
                                    onBeforeApplySort={addParameters}
                                    onBeforeApplyFilter={addParameters}
                                    onBeforeExportExcel={addParameters}
                                    onBeforeFetchStats={addParameters}
                                />
                            </Col>
                        </Row>
                    </fieldset>

                </Col>
                <Col lg="6">
                    <fieldset className="form-group border p-2 grid" >
                        <legend align="center" className="w-auto">{t("EXP110_grid_title_PickPendientes")}</legend>

                        <Row>
                            <Col>
                                <Grid id="EXP110_grid_PickPend"
                                    rowsToFetch={10}
                                    rowsToDisplay={5}
                                    enableExcelExport
                                    application="EXP110PickeosPendientes"
                                    onBeforeInitialize={addParameters}
                                    onBeforeFetch={addParameters}
                                    onBeforeApplySort={addParameters}
                                    onBeforeApplyFilter={addParameters}
                                    onBeforeExportExcel={addParameters}
                                    onBeforeFetchStats={addParameters}
                                />
                            </Col>
                        </Row>
                    </fieldset>

                </Col>

            </Row>
            {
                showPopup &&

                <EXP110ConfiguracionInicialModal show={showPopup} onHide={closePopup} conf_inicial={conf_inicial} />
            }
            {
                showProductoPopup &&

                <EXP110SeleccionProductoModal show={showProductoPopup} onHide={closeProductoPopup} addParameters={addParameters} getParametersFormArray={GetParametersFormArray} nexus={nexus} auxContenedorImpresion={auxContenedorImpresion} />
            }
            {
                showImpresionBultoPopup &&

                <EXP110ImpresionBultosModal show={showImpresionBultoPopup} onHide={closeImpresionBultoPopup} conf_inicial={conf_inicial} nexus={nexus} />
            }

            <Modal show={showModal} onHide={closeXFormDialog} dialogClassName="modal-90w" backdrop="static" >
                <Modal.Header  >
                    <div className="left">  <i
                        onClick={closeVolverFormDialog} ><i className="fa fa-arrow-left" /></i></div>
                    <CloseButton onClick={closeXFormDialog} />
                </Modal.Header>

                {showPedidosMesaEmpaquePopup ? showFormPedidosMesaEmpaquePopup() : null}
                {showInfoPedidosPopup ? showFormshowInfoPedidosPopup() : null}
                {showInfoCamionesPedidosPopup ? showFormCamionesPedidosPopup() : null}
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={closeVolverFormDialog}> {t("EXP110ConfInicial_frm1_btn_volver")} </Button>
                </Modal.Footer>
            </Modal>
        </Page >
    );
}