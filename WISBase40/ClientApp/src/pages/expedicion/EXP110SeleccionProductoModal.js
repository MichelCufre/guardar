import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldDate, SubmitButton, FormButton, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import { Modal, Button, Row, Col, FormGroup, Tab, Tabs, fieldset, Container } from 'react-bootstrap';
import * as Yup from 'yup';
import { isNullOrUndefined } from 'util';


export const EXP110SeleccionProductoModal = (props) => {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [contenedorDestino, setContenedorDestino] = useState(null);
    const [pedidoProductoContRowSelected, setPedidoProductoContRowSelected] = useState(null);    
    const [pedidoProductoLoteRowSelected, setPedidoProductoLoteRowSelected] = useState(null);
    const [pedidoProductoLoteOnlyOneRow, setPedidoProductoLoteOnlyOneRow] = useState(null);
    const [isSummiting, setIsSummiting] = useState(false);


    //******** Events Functions *************
    const validationSchema = {
        codigoProducto: Yup.string(),
        descripcionProducto: Yup.string(),
        cantidadProducto: Yup.string().required()
        //cantidadProducto: Yup.number().transform(value => (isNaN(value) ? undefined : value)).required()
    };
    const setFocusField = (fieldName) => {
        document.getElementsByName(fieldName)[0].focus();
    }
    const handleClose = (auxContenedorImpresion, data) => {
        var imprimirResumenPicking = "";
        var pedidoCompleto = "";
        var terminePedidoContOrigen = "";
        var ContOrigenVacio = "";
        var facturoContenedorEmpaque = "";

        if (!isNullOrUndefined(data)) {
            if (data.parameters.some(x => x.id == "AUX_PED_COMPLETO_IMP_RESUMEN")) {
                imprimirResumenPicking = data.parameters.find(x => x.id == "AUX_PED_COMPLETO_IMP_RESUMEN").value;
            }
            if (data.parameters.some(x => x.id == "AUX_PEDIDO_COMPLETO")) {
                pedidoCompleto = data.parameters.find(x => x.id == "AUX_PEDIDO_COMPLETO").value;
            }
            if (data.parameters.some(x => x.id == "AUX_TERMINE_PEDIDO_CONT_ORIG")) {
                terminePedidoContOrigen = data.parameters.find(x => x.id == "AUX_TERMINE_PEDIDO_CONT_ORIG").value;
            }
            if (data.parameters.some(x => x.id == "AUX_CONT_ORIG_VACIO")) {
                ContOrigenVacio = data.parameters.find(x => x.id == "AUX_CONT_ORIG_VACIO").value;
            }
            if (data.parameters.some(x => x.id == "AUX_FACTURO_CONTENEDOR_EMPAQUE")) {
                facturoContenedorEmpaque = data.parameters.find(x => x.id == "AUX_FACTURO_CONTENEDOR_EMPAQUE").value;
            }

            props.onHide(props.nexus, auxContenedorImpresion, imprimirResumenPicking, pedidoCompleto, terminePedidoContOrigen, ContOrigenVacio, facturoContenedorEmpaque, data.parameters);
        }
        else {
            props.onHide(props.nexus, auxContenedorImpresion, imprimirResumenPicking, pedidoCompleto);
        }
    };

    //******** Before functions *************
    const onBeforeValidateField = (context, form, query, nexus) => {
        props.getParametersFormArray().map(param => {
            query.parameters.push({ id: param.id, value: param.value })
        });

        query.parameters.push({ id: "AUX_ROW_SELECTED_PEDPRODCONT", value: pedidoProductoContRowSelected });
        query.parameters.push({ id: "AUX_ROW_SELECTED_PEDPRODLOTE", value: pedidoProductoLoteRowSelected });
        query.parameters.push({ id: "AUX_TIENE_UNA_ROW", value: pedidoProductoLoteOnlyOneRow });
    }
    const onBeforeInitialize = (context, form, data, nexus) => {
        props.getParametersFormArray().map(param => {
            data.parameters.push({ id: param.id, value: param.value })
        });

        data.parameters.push({ id: "AUX_ROW_SELECTED_PEDPRODCONT", value: pedidoProductoContRowSelected });
        data.parameters.push({ id: "AUX_ROW_SELECTED_PEDPRODLOTE", value: pedidoProductoLoteRowSelected });
        data.parameters.push({ id: "AUX_TIENE_UNA_ROW", value: pedidoProductoLoteOnlyOneRow });
    }
    const onBeforeSubmit = (context, form, data, nexus) => {

        if (data.buttonId == "btnSubmitConfirmar" && isSummiting) {
            context.abortServerCall = true;
        }
        else {
            setIsSummiting(true);
        }

        props.getParametersFormArray().map(param => {
            data.parameters.push({ id: param.id, value: param.value })
        })

        if (contenedorDestino !== null) {
            data.parameters.push({ id: "AUX_CONTENEDOR_DESTINO", value: contenedorDestino })
        }

        data.parameters.push({ id: "AUX_ROW_SELECTED_PEDPRODCONT", value: pedidoProductoContRowSelected });
        data.parameters.push({ id: "AUX_ROW_SELECTED_PEDPRODLOTE", value: pedidoProductoLoteRowSelected });
        data.parameters.push({ id: "AUX_CONT_IMPRESION", value: props.auxContenedorImpresion });
        data.parameters.push({ id: "AUX_TIENE_UNA_ROW", value: pedidoProductoLoteOnlyOneRow });

        if (!data.parameters.some(x => x.id == "SUMMIT_EMPAQUETAR_PROD")) {
            data.parameters.push({ id: "SUMMIT_EMPAQUETAR_PROD", value: "N" });
        }

        if (!data.parameters.some(x => x.id == "SUMMIT_EMPAQUETAR_TODO")) {
            data.parameters.push({ id: "SUMMIT_EMPAQUETAR_TODO", value: "N" });
        }

        if (!data.parameters.some(x => x.id == "SUCCESS_SUMMIT")) {
            data.parameters.push({ id: "SUCCESS_SUMMIT", value: "N" });
        }
    }
    const onBeforeInitializeGrid = (context, data, nexus) => {
        props.getParametersFormArray().map(param => {
            data.parameters.push({ id: param.id, value: param.value })
        });

        if (pedidoProductoContRowSelected !== null) {
            data.parameters.push({ id: "AUX_ROW_SELECTED_PEDPRODCONT", value: pedidoProductoContRowSelected });
        }

        if (pedidoProductoLoteRowSelected !== null) {
            data.parameters.push({ id: "AUX_ROW_SELECTED_PEDPRODLOTE", value: pedidoProductoLoteRowSelected });
        }
    }
    //**************************************

    //******** After functions *************
    const onAfterInitialize = (context, form, data, nexus) => {
        if (data.parameters.some(x => x.id == "AUX_TIENE_SOLO_UN_REGISTRO")) {
            var value = data.parameters.find(x => x.id == "AUX_TIENE_SOLO_UN_REGISTRO").value;
            if (value === "S") {
                setFocusField("cantidadProducto");
            }
        }

    }
    const onAfterButtonAction = (data, nexus) => {

        if (data.parameters.some(x => x.id == "AUX_ROW_SELECTED_PEDPRODCONT")) {
            var valuePedProdCont = data.parameters.find(x => x.id == "AUX_ROW_SELECTED_PEDPRODCONT").value;
            setPedidoProductoContRowSelected(valuePedProdCont);
            nexus.getGrid("EXP110_grid_PedProdCont").reset();
            nexus.getGrid("EXP110_grid_PedProdLote").reset();
            nexus.getForm("EXP110SelecProducto_form_1").reset();
        }

        if (data.parameters.some(x => x.id == "AUX_ROW_SELECTED_PEDPRODLOTE")) {
            var valuePedProdLote = data.parameters.find(x => x.id == "AUX_ROW_SELECTED_PEDPRODLOTE").value;
            setPedidoProductoLoteRowSelected(valuePedProdLote);
            nexus.getGrid("EXP110_grid_PedProdLote").reset();
            nexus.getForm("EXP110SelecProducto_form_1").reset();
        }
    }
    const onAfterSubmit = (context, form, data, nexus) => {

        setIsSummiting(false);

        if (data.parameters.some(x => x.id == "AUX_CONTENEDOR_DESTINO")) {
            var value = data.parameters.find(x => x.id == "AUX_CONTENEDOR_DESTINO").value;
            setContenedorDestino(value);
        }

        if (data.parameters.some(x => x.id == "CONFIRMATION_MSG_EMPAQUETAR_TODO")) {
            var value = data.parameters.find(x => x.id == "CONFIRMATION_MSG_EMPAQUETAR_TODO").value;

            if (value === "S") {
                nexus.showConfirmation({
                    message: "EXP110_form1_Msg_EmpaquetarTodo",
                    acceptLabel: "General_Sec0_btn_CONFIRM_BOX_YES",
                    cancelLabel: "General_Sec0_btn_CONFIRM_BOX_NO",
                    onAccept: () => onAcceptConfirmation(nexus),
                    onCancel: () => onCancelConfirmation(nexus)
                });
            }
        }
        else {

            if (data.parameters.find(x => x.id == "SUMMIT_EMPAQUETAR_PROD").value === "S" &&
                data.parameters.find(x => x.id == "SUCCESS_SUMMIT").value === "N" &&
                data.parameters.find(x => x.id == "SUMMIT_EMPAQUETAR_TODO").value === "N") {
                nexus.getForm("EXP110SelecProducto_form_1").submit("BtnEmpaquetarProducto");
                context.abortHideLoading = true;
            }
        }

        if (data.buttonId == "btnSubmitConfirmar" && data.parameters.some(x => x.id == "AUX_CONTENEDOR_NUEVO")) {
            var form = nexus.getForm("EXP110_form");
            var nuContenedor = data.parameters.find(x => x.id == "AUX_CONTENEDOR_NUEVO").value;
            var nuPreparacion = data.parameters.find(x => x.id == "AUX_PREPARACION_NUEVO").value;
            var idExternoDestino = data.parameters.find(x => x.id == "AUX_ID_EXTERNO_CONTENEDOR_NUEVO").value;

            form.setFieldValue("contenedorDestino", nuContenedor);
            form.setFieldValue("preparacionDestino", nuPreparacion);
            form.setFieldValue("idExternoContenedorDestino", idExternoDestino);
        }

        if ((data.buttonId == "BtnEmpaquetarProducto" || data.buttonId == "BtnEmpaquetarTodo") &&
            data.parameters.some(x => x.id == "AUX_SET_FIELD_CD_BARRAS_PROD")) {

            if (data.parameters.some(x => x.id == "AUX_CONT_IMPRESION")) {
                var value = data.parameters.find(x => x.id == "AUX_CONT_IMPRESION").value;
                handleClose(value, data);
            }

            var form = nexus.getForm("EXP110_form");
            form.setFieldValue("codigoBarraProducto", "");
            form.setFieldValue("descripcionProducto", "");

            nexus.getGrid("EXP110_grid_EmpaquetaPicking").reset();
            nexus.getGrid("EXP110_grid_ContPendientes").reset();
            nexus.getGrid("EXP110_grid_PickPend").reset();

            setFocusField("codigoBarraProducto");
        }
        else if ((data.buttonId == "BtnEmpaquetarProducto" || data.buttonId == "BtnEmpaquetarTodo") &&
            data.parameters.some(x => x.id == "SUCCESS_SUMMIT")) {

            if (data.parameters.some(x => x.id == "AUX_CONT_IMPRESION")) {
                var value = data.parameters.find(x => x.id == "AUX_CONT_IMPRESION").value;
                handleClose(value, data);
            }
            else {
                handleClose(null, data);
            }
        }

    }
    const onAfterFetch = (context, newRows, parameters, nexus) => {

        if ((!parameters.some(x => x.id == "AUX_ROW_SELECTED_PEDPRODCONT") ||
            parameters.find(x => x.id == "AUX_ROW_SELECTED_PEDPRODCONT").value === null) &&
            (!parameters.some(x => x.id == "AUX_ROW_SELECTED_PEDPRODCONT") ||
                parameters.find(x => x.id == "AUX_ROW_SELECTED_PEDPRODLOTE").value === null)) {
            var form = nexus.getForm("EXP110SelecProducto_form_1");
            form.setFieldValue("anexo4", "");
            form.setFieldValue("cantidadProducto", "");
        }

        if ((!parameters.some(x => x.id == "AUX_ROW_SELECTED_PEDPRODCONT") ||
            parameters.find(x => x.id == "AUX_ROW_SELECTED_PEDPRODCONT").value === null) &&
            (!parameters.some(x => x.id == "AUX_ROW_SELECTED_PEDPRODCONT") ||
                parameters.find(x => x.id == "AUX_ROW_SELECTED_PEDPRODLOTE").value === null)) {
            var form = nexus.getForm("EXP110SelecProducto_form_1");
            form.setFieldValue("anexo4", "");
            form.setFieldValue("cantidadProducto", "");
        }

        if (parameters.some(x => x.id == "AUX_ROW_SELECTED_PEDPRODCONT")) {
            var value = parameters.find(x => x.id == "AUX_ROW_SELECTED_PEDPRODCONT").value;
            setPedidoProductoContRowSelected(value);
        }

    }
    const onAfterApplyFilter = (context, form, data, nexus) => {

        if (!data.parameters.some(x => x.id == "AUX_ROW_SELECTED_PEDPRODCONT") && data.parameters.find(x => x.id == "AUX_ROW_SELECTED_PEDPRODCONT").value === null) {
            var form = nexus.getForm("EXP110SelecProducto_form_1");
            form.setFieldValue("anexo4", "");
            form.setFieldValue("cantidadProducto", "");
        }

        if (!data.parameters.some(x => x.id == "AUX_ROW_SELECTED_PEDPRODLOTE") && data.parameters.find(x => x.id == "AUX_ROW_SELECTED_PEDPRODLOTE").value === null) {
            var form = nexus.getForm("EXP110SelecProducto_form_1");
            form.setFieldValue("anexo4", "");
            form.setFieldValue("cantidadProducto", "");
        }
    }
    const onAfterInitializeGrid = (context, grid, parameters, nexus) => {

        if (parameters.some(x => x.id == "AUX_TIENE_UNA_ROW")) {
            var value = parameters.find(x => x.id === "AUX_TIENE_UNA_ROW").value;
            setPedidoProductoLoteOnlyOneRow(value);
        }
        if (parameters.some(x => x.id == "AUX_ROW_SELECTED_PEDPRODCONT")) {

            var value = parameters.find(x => x.id == "AUX_ROW_SELECTED_PEDPRODCONT").value;
            setPedidoProductoContRowSelected(value);
        }
        nexus.getForm("EXP110SelecProducto_form_1").reset();
    }


    //**************************************


    const addParameters = (context, data, nexus) => {
        props.getParametersFormArray().map(param => {
            data.parameters.push({ id: param.id, value: param.value })
        });

        data.parameters.push({ id: "AUX_ROW_SELECTED_PEDPRODCONT", value: pedidoProductoContRowSelected });
        data.parameters.push({ id: "AUX_ROW_SELECTED_PEDPRODLOTE", value: pedidoProductoLoteRowSelected });
    }
    const onAcceptConfirmation = (nexus) => {
        nexus.getForm("EXP110SelecProducto_form_1").submit("BtnEmpaquetarTodo");
    }
    const onCancelConfirmation = (nexus) => {
        nexus.getForm("EXP110SelecProducto_form_1").submit("BtnEmpaquetarProducto");
    }

    //**************************************

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Form
                application="EXP110SelecProd"
                id="EXP110SelecProducto_form_1"
                validationSchema={validationSchema}
                onBeforeInitialize={onBeforeInitialize}
                onAfterInitialize={onAfterInitialize}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={onAfterSubmit}
                onBeforeValidateField={onBeforeValidateField}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("EXP110SelecProducto_Sec0_mdl_SelecProducto_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col lg="4">
                            <div className="form-group" >
                                <label htmlFor="codigoProducto">{t("EXP110SelecProducto_frm1_lbl_codigoProducto")}</label>
                                <Field name="codigoProducto" readOnly />
                                <StatusMessage for="codigoProducto" />
                            </div>
                        </Col>
                        <Col lg="8">
                            <div className="form-group" >
                                <label htmlFor="descripcionProducto">{t("EXP110SelecProducto_frm1_lbl_descripcionProducto")}</label>
                                <Field name="descripcionProducto" readOnly />
                                <StatusMessage for="descripcionProducto" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col lg="12">
                            <Grid
                                id="EXP110_grid_PickingProducto"
                                rowsToFetch={10}
                                rowsToDisplay={5}
                                enableExcelExport
                                application="EXP110SelecProd"
                                onBeforeInitialize={onBeforeInitializeGrid}
                                onBeforeFetch={addParameters}
                                onBeforeApplySort={addParameters}
                                onBeforeExportExcel={addParameters}
                            />
                        </Col>
                    </Row>
                    <Row>
                        <Col lg="6">
                            <fieldset className="form-group border p-2 grid" >
                                <legend align="center" className="w-auto">{t("EXP110Seleccion_grid1_title_PedidoProducto")}</legend>
                                <Row>
                                    <Col>
                                        <Grid id="EXP110_grid_PedProdCont"
                                            rowsToFetch={10}
                                            rowsToDisplay={5}
                                            enableExcelExport
                                            application="EXP110ProdCont"
                                            onBeforeInitialize={onBeforeInitializeGrid}
                                            onBeforeFetch={addParameters}
                                            onBeforeApplySort={addParameters}
                                            onBeforeApplyFilter={addParameters}
                                            onBeforeExportExcel={addParameters}
                                            onAfterButtonAction={onAfterButtonAction}
                                            onAfterFetch={onAfterFetch}
                                            onAfterApplyFilter={onAfterApplyFilter}
                                            onAfterInitialize={onAfterInitializeGrid}
                                        />
                                    </Col>
                                </Row>
                            </fieldset>

                        </Col>
                        <Col lg="6">
                            <fieldset className="form-group border p-2 grid" >
                                <legend align="center" className="w-auto">{t("EXP110Seleccion_grid1_title_PedidoProductoLote")}</legend>

                                <Row>
                                    <Col>
                                        <Grid id="EXP110_grid_PedProdLote"
                                            rowsToFetch={10}
                                            rowsToDisplay={5}
                                            enableExcelExport
                                            application="EXP110ProdLote"
                                            onBeforeInitialize={onBeforeInitializeGrid}
                                            onBeforeFetch={addParameters}
                                            onBeforeExportExcel={addParameters}
                                            onBeforeApplyFilter={addParameters}
                                            onBeforeApplySort={addParameters}
                                            onAfterButtonAction={onAfterButtonAction}
                                            onAfterFetch={onAfterFetch}
                                            onAfterApplyFilter={onAfterApplyFilter}
                                            onAfterInitialize={onAfterInitializeGrid}
                                        />
                                    </Col>
                                </Row>
                            </fieldset>
                        </Col>
                    </Row>
                    <Row>
                        <Col lg="8">
                            <div className="form-group" >
                                <label htmlFor="anexo4">{t("EXP110SelecProducto_frm1_lbl_anexo4")}</label>
                                <Field name="anexo4" readOnly />
                                <StatusMessage for="anexo4" />
                            </div>
                        </Col>
                        <Col lg="4">
                            <div className="form-group" >
                                <label htmlFor="cantidadProducto">{t("EXP110SelecProducto_frm1_lbl_cantidadProducto")}</label>
                                <Field name="cantidadProducto" onKeyPress={(event) => {

                                    if (event.key === "Enter") {
                                        props.nexus.getForm("EXP110SelecProducto_form_1").submit("btnSubmitConfirmar");
                                    }
                                }} />
                                <StatusMessage for="cantidadProducto" />
                            </div>
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("EXP110SelecProducto_frm1_btn_cerrar")} </Button>
                    <SubmitButton id="btnSubmitConfirmar" variant="primary" label="EXP110SelecProducto_frm1_btn_confirmar" />
                </Modal.Footer>
            </Form>

        </Modal>
    );
};