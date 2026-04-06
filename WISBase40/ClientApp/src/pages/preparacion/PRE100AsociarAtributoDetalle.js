import React, { useState, useEffect } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Modal, Button, Row, Col, FormGroup, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Form, Field, StatusMessage, SubmitButton, FieldSelectAsync } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import * as Yup from 'yup';
import PRE100DefinicionAtributo from './PRE100DefinicionAtributo';

export default function PRE100AsociarAtributoDetalle(props) {
    const { t } = useTranslation();

    const [update, setUpdate] = useState("N");
    const [nexus, setNexus] = useState("");
    const [listAtributos, setListAtributos] = useState("");
    const [showAtributoDefinicion, setShowAtributoDefinicion] = useState(false);
    const [readOnly, setReadOnly] = useState(false);
    const [datos, setDatos] = useState("");
    const [producto, setProducto] = useState("");
    const [identificador, setIdentificador] = useState("");
    const [cantidad, setCantidad] = useState("");

    const [coutRowGrid, setCoutRowGrid] = useState(0);
    const [coutRowGridDetalle, setCoutRowGridDetalle] = useState(0);

    const [empresaNombre, setEmpresaNombre] = useState("");
    const [agenteDescripcion, setAgenteDescripcion] = useState("");
    const [agenteCodigo, setAgenteCodigo] = useState("");
    const [agenteTipo, setAgenteTipo] = useState("");
    const [pedido, setPedido] = useState("");
    const [empresa, setEmpresa] = useState("");
    
    const initialValues = {
        cantidad: "",
        producto: "",
        identificador: ""
    };

    const validationSchema = {
        cantidad: Yup.string(),
        producto: Yup.string(),
        identificador: Yup.string()
    };

    //#region Page

    useEffect(() => {
        if (nexus && nexus.getForm("PRE100AsociarAtributoDetalle_form_1")) {
            if (coutRowGrid > 0 || coutRowGridDetalle > 0) {
                nexus.getForm("PRE100AsociarAtributoDetalle_form_1").submit("btnDeshabilitarCampos");

            } else {
                nexus.getForm("PRE100AsociarAtributoDetalle_form_1").submit("btnHabilitarCampos");
            }
        }
    }, [coutRowGrid, coutRowGridDetalle]);

    useEffect(() => {
        setShowAtributoDefinicion(false);

        if (listAtributos) {
            setShowAtributoDefinicion(true);
        }
    }, [listAtributos]);

    const onBeforePageLoad = (data) => {
        setProducto("");
        setIdentificador("");
        setCantidad("");

        var upd = (props.datos.find(d => d.id === "update")?.value ?? "N")
        var pedido = props.datos.find(d => d.id === "pedido").value;
        var empresa = props.datos.find(d => d.id === "empresa").value;

        var datos = [
            { id: "pedido", value: pedido },
            { id: "cliente", value: props.datos.find(d => d.id === "cliente").value },
            { id: "empresa", value: empresa },
            { id: "update", value: upd }

        ];

        if (upd === "S") {
            datos.push(
                { id: "producto", value: props.datos.find(d => d.id === "producto").value },
                { id: "faixa", value: props.datos.find(d => d.id === "faixa").value },
                { id: "identificador", value: props.datos.find(d => d.id === "identificador").value },
                { id: "idEspecificaIdentificador", value: props.datos.find(d => d.id === "idEspecificaIdentificador").value },
                { id: "idConfiguracion", value: props.datos.find(d => d.id === "idConfiguracion").value },
                { id: "cantidad", value: props.datos.find(d => d.id === "cantidad").value });
        }

        setUpdate(upd)
        setNexus(nexus);
        setDatos(datos);
        setPedido(pedido)
        setEmpresa(empresa)
        data.parameters = datos;
    }

    const onAfterPageLoad = (data) => {

        if (data.parameters.length > 0) {
            setReadOnly(data.parameters.find(x => x.id === "readOnly").value == "S")

            if (data.parameters.find(s => s.id === "empresaNombre")) {
                setEmpresaNombre(data.parameters.find(d => d.id === "empresaNombre").value);
                setAgenteDescripcion(data.parameters.find(d => d.id === "agenteDescripcion").value);
                setAgenteCodigo(data.parameters.find(d => d.id === "agenteCodigo").value);
                setAgenteTipo(data.parameters.find(d => d.id === "agenteTipo").value);
            }
        }
    }

    const handleClose = () => {
        props.onHide();
    };
    //#endregion

    //#region Grid

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "pedido", value: props.datos.find(d => d.id === "pedido").value },
            { id: "cliente", value: props.datos.find(d => d.id === "cliente").value },
            { id: "empresa", value: props.datos.find(d => d.id === "empresa").value },
            { id: "update", value: update },
            { id: "readOnly", value: readOnly },

        ];

        if (update === "S") {
            data.parameters.push(
                { id: "producto", value: props.datos.find(d => d.id === "producto").value },
                { id: "faixa", value: props.datos.find(d => d.id === "faixa").value },
                { id: "identificador", value: props.datos.find(d => d.id === "identificador").value },
                { id: "idEspecificaIdentificador", value: props.datos.find(d => d.id === "idEspecificaIdentificador").value },
                { id: "idConfiguracion", value: props.datos.find(d => d.id === "idConfiguracion").value },
                { id: "cantidad", value: props.datos.find(d => d.id === "cantidad").value });
        }
        else {
            data.parameters.push(
                { id: "producto", value: producto },
                { id: "identificador", value: identificador },
                { id: "cantidad", value: cantidad });
        }

        setNexus(nexus);
    };

    const onAfterFetch = (context, newRows, parameters, nexus) => {

        if (parameters.find(d => d.id === "coutRowGrid")) {
            setCoutRowGrid(parseInt(parameters.find(d => d.id === "coutRowGrid").value))
        }

        if (parameters.find(d => d.id === "coutRowGridDetalle")) {
            setCoutRowGridDetalle(parseInt(parameters.find(d => d.id === "coutRowGridDetalle").value))
        }
    }


    const handleAdd = (evt, nexus) => {
        nexus.getGrid("PRE100AsociarAtributoDetalle_grid_1").triggerMenuAction("btnAgregarAtributos", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("PRE100AsociarAtributoDetalle_grid_2").triggerMenuAction("btnQuitarAtributos", false, evt.ctrlKey);
    };

    const handleDetalleAdd = (evt, nexus) => {
        nexus.getGrid("PRE100AsociarAtributoDetalle_gridDetalle_1").triggerMenuAction("btnAgregarAtributosDetalle", false, evt.ctrlKey);
    };

    const handleDetalleRemove = (evt, nexus) => {
        nexus.getGrid("PRE100AsociarAtributoDetalle_gridDetalle_2").triggerMenuAction("btnQuitarAtributosDetalle", false, evt.ctrlKey);
    };

    const handleAfterMenuItemAction = (context, data, nexus) => {

        setUpdate(data.parameters.find(d => d.id === "update")?.value ?? "N")
        var cerrarModal = (data.parameters.find(d => d.id === "terminarOperacion")?.value ?? "N") == "S"

        if (cerrarModal) {
            handleClose()
        }
        else {

            if (data.gridId == "PRE100AsociarAtributoDetalle_grid_1") {

                if (data.parameters.find(d => d.id === "listAtributos").value === "") {
                    nexus.getGrid("PRE100AsociarAtributoDetalle_grid_1").refresh();
                    nexus.getGrid("PRE100AsociarAtributoDetalle_grid_2").refresh();
                }
                else {
                    setListAtributos(data.parameters.find(d => d.id === "listAtributos").value);
                    setDatosDefAtributo();
                }
            }
            else if (data.gridId == "PRE100AsociarAtributoDetalle_gridDetalle_1") {

                if (data.parameters.find(d => d.id === "listAtributos").value === "") {
                    nexus.getGrid("PRE100AsociarAtributoDetalle_gridDetalle_1").refresh();
                    nexus.getGrid("PRE100AsociarAtributoDetalle_gridDetalle_2").refresh();
                }
                else {
                    setListAtributos(data.parameters.find(d => d.id === "listAtributos").value);
                    setDatosDefAtributo();
                }
            }
            else {

                nexus.getGrid("PRE100AsociarAtributoDetalle_grid_1").refresh();
                nexus.getGrid("PRE100AsociarAtributoDetalle_grid_2").refresh();

                nexus.getGrid("PRE100AsociarAtributoDetalle_gridDetalle_1").refresh();
                nexus.getGrid("PRE100AsociarAtributoDetalle_gridDetalle_2").refresh();
            }
        }
    }

    const setDatosDefAtributo = (data) => {

        var datos = [
            { id: "pedido", value: props.datos.find(d => d.id === "pedido").value },
            { id: "cliente", value: props.datos.find(d => d.id === "cliente").value },
            { id: "empresa", value: props.datos.find(d => d.id === "empresa").value },
            { id: "update", value: update }

        ];

        if (update === "S") {
            datos.push(
                { id: "producto", value: props.datos.find(d => d.id === "producto").value },
                { id: "faixa", value: props.datos.find(d => d.id === "faixa").value },
                { id: "identificador", value: props.datos.find(d => d.id === "identificador").value },
                { id: "idEspecificaIdentificador", value: props.datos.find(d => d.id === "idEspecificaIdentificador").value },
                { id: "idConfiguracion", value: props.datos.find(d => d.id === "idConfiguracion").value },
                { id: "cantidad", value: props.datos.find(d => d.id === "cantidad").value });
        }
        else {
            datos.push(
                { id: "producto", value: producto },
                { id: "identificador", value: identificador },
                { id: "cantidad", value: cantidad });
        }

        setDatos(datos);
    }


    //#endregion

    //#region Form

    const applyParametersFormulario = (context, form, data, nexus) => {
        data.parameters = [
            { id: "pedido", value: props.datos.find(d => d.id === "pedido").value },
            { id: "cliente", value: props.datos.find(d => d.id === "cliente").value },
            { id: "empresa", value: props.datos.find(d => d.id === "empresa").value },
            { id: "update", value: update },
            { id: "readOnly", value: readOnly },
        ];

        if (update === "S") {
            data.parameters.push(
                { id: "producto", value: props.datos.find(d => d.id === "producto").value },
                { id: "faixa", value: props.datos.find(d => d.id === "faixa").value },
                { id: "identificador", value: props.datos.find(d => d.id === "identificador").value },
                { id: "idEspecificaIdentificador", value: props.datos.find(d => d.id === "idEspecificaIdentificador").value },
                { id: "idConfiguracion", value: props.datos.find(d => d.id === "idConfiguracion").value },
                { id: "cantidad", value: props.datos.find(d => d.id === "cantidad").value });
        }

        setNexus(nexus);
    };


    const onBeforeSelectSearch = (context, form, data, nexus) => {

        data.parameters = [
            { id: "pedido", value: props.datos.find(d => d.id === "pedido").value },
            { id: "cliente", value: props.datos.find(d => d.id === "cliente").value },
            { id: "empresa", value: props.datos.find(d => d.id === "empresa").value },
            { id: "update", value: update },
            { id: "readOnly", value: readOnly },
        ];

        if (update === "S") {
            data.parameters.push(
                { id: "producto", value: props.datos.find(d => d.id === "producto").value },
                { id: "faixa", value: props.datos.find(d => d.id === "faixa").value },
                { id: "identificador", value: props.datos.find(d => d.id === "identificador").value },
                { id: "idEspecificaIdentificador", value: props.datos.find(d => d.id === "idEspecificaIdentificador").value },
                { id: "idConfiguracion", value: props.datos.find(d => d.id === "idConfiguracion").value },
                { id: "cantidad", value: props.datos.find(d => d.id === "cantidad").value });
        }

        setNexus(nexus);
    };



    const onAfterValidateField = (context, form, data, nexus) => {

        setProducto(form.fields.find(f => f.id === "producto").value)
        setIdentificador(form.fields.find(f => f.id === "identificador").value)
        setCantidad(form.fields.find(f => f.id === "cantidad").value)

        setNexus(nexus);
    };

    const onBeforeSubmit = (context, form, data, nexus) => {

        if (data.buttonId != "btnSubmitAsociarAtributos" && data.buttonId != "btnDeshabilitarCampos" && data.buttonId != "btnHabilitarCampos") {
            context.abortServerCall = true;
        }
        else {

            data.parameters = [
                { id: "pedido", value: props.datos.find(d => d.id === "pedido").value },
                { id: "cliente", value: props.datos.find(d => d.id === "cliente").value },
                { id: "empresa", value: props.datos.find(d => d.id === "empresa").value },
                { id: "update", value: update },
                { id: "readOnly", value: readOnly },
            ];

            if (update === "S") {
                data.parameters.push(
                    { id: "producto", value: props.datos.find(d => d.id === "producto").value },
                    { id: "faixa", value: props.datos.find(d => d.id === "faixa").value },
                    { id: "identificador", value: props.datos.find(d => d.id === "identificador").value },
                    { id: "idEspecificaIdentificador", value: props.datos.find(d => d.id === "idEspecificaIdentificador").value },
                    { id: "idConfiguracion", value: props.datos.find(d => d.id === "idConfiguracion")?.value },
                    { id: "cantidad", value: props.datos.find(d => d.id === "cantidad").value });
            }

            setNexus(nexus);
        }
    }

    const onAfterSubmit = (context, form, query, nexus) => {
        setNexus(nexus);
        context.showErrorMessage = true;

        if (context.responseStatus === "OK" && query.buttonId === "btnSubmitAsociarAtributos") {
            handleClose()
        }
    }


    const closeAtributosFormDialog = (listAtributos) => {
        setListAtributos(listAtributos);

        nexus.getGrid("PRE100AsociarAtributoDetalle_grid_1").refresh();
        nexus.getGrid("PRE100AsociarAtributoDetalle_grid_2").refresh();
        nexus.getGrid("PRE100AsociarAtributoDetalle_gridDetalle_1").refresh();
        nexus.getGrid("PRE100AsociarAtributoDetalle_gridDetalle_2").refresh();
    };

    //#endregion

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-70w" backdrop="static">

            <Page
                {...props}
                application="PRE100AsociarAtributoDetalle"
                onBeforeLoad={onBeforePageLoad}
                onAfterLoad={onAfterPageLoad}
            >
                <Form
                    application="PRE100AsociarAtributoDetalle"
                    id="PRE100AsociarAtributoDetalle_form_1"
                    initialValues={initialValues}
                    validationSchema={validationSchema}
                    onBeforeInitialize={applyParametersFormulario}
                    onBeforeSelectSearch={onBeforeSelectSearch}
                    onBeforeValidateField={applyParametersFormulario}
                    onAfterValidateField={onAfterValidateField}
                    onBeforeSubmit={onBeforeSubmit}
                    onAfterSubmit={onAfterSubmit}
                >
                    <Modal.Header closeButton>
                        <Modal.Title>{t("PRE100AsociarAtributoDetalle_Sec0_modalTitle_Titulo")}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Row>
                            <Col lg={4}>
                                <Row >
                                    <Col lg={6}>
                                        <span style={{ fontWeight: "bold" }}> {t("PRE100_frm1_lbl_pedido")}: </span>
                                    </Col>
                                    <Col lg={6}>
                                        <span >{`${pedido}`}</span>
                                    </Col>
                                </Row>
                            </Col>
                            <Col lg={4}>
                                <Row>
                                    <Col lg={6}>
                                        <span style={{ fontWeight: "bold" }}>{t("PRE100_frm1_lbl_empresa")}: </span>
                                    </Col>
                                    <Col lg={6}>
                                        <span> {`${empresa}`} - {`${empresaNombre}`}</span>
                                    </Col>
                                </Row>
                            </Col>
                            <Col lg={4}>
                                <Row>
                                    <Col lg={6}>
                                        <span style={{ fontWeight: "bold" }}>{t("PRE100_frm1_lbl_cliente")}:</span>
                                    </Col>
                                    <Col lg={6}>
                                        <span> {`${agenteTipo}`}-{`${agenteCodigo}`}-{`${agenteDescripcion}`}  </span>
                                    </Col>
                                </Row>
                            </Col>
                        </Row>

                        <hr />

                        <Row>

                            <Col md={4}>
                                <div className="form-group">
                                    <label htmlFor="producto">{t("PRE100AsociarAtributoDetalle_frm1_lbl_Producto")}</label>
                                    <FieldSelectAsync id="producto" name="producto" />
                                    <StatusMessage for="producto" />
                                </div>
                            </Col>
                            <Col md={4}>
                                <div className="form-group" >
                                    <label htmlFor="identificador">{t("PRE100AsociarAtributoDetalle_frm1_lbl_Identificador")}</label>
                                    <Field id="identificador" name="identificador" />
                                    <StatusMessage for="identificador" />
                                </div>
                            </Col>
                            <Col md={4}>
                                <div className="form-group">
                                    <label htmlFor="cantidad">{t("PRE100AsociarAtributoDetalle_frm1_lbl_Cantidad")}</label>
                                    <Field name="cantidad" readOnly={readOnly} />
                                    <StatusMessage for="cantidad" />
                                </div>
                            </Col>
                        </Row>
                        <hr />

                        <AddRemovePanel
                            onAdd={handleAdd}
                            onRemove={handleRemove}
                            BtnDisabled={readOnly}
                            from={(
                                <div>
                                    <h5 className='form-title'>{t("PRE100AsociarAtributoDetalle_frm1_lbl_AtributosSinDefinir")}</h5>
                                    <Grid
                                        application="PRE100AsociarAtributoDetalle"
                                        id="PRE100AsociarAtributoDetalle_grid_1"
                                        rowsToFetch={30}
                                        rowsToDisplay={7}
                                        onBeforeInitialize={applyParameters}
                                        onBeforeFetch={applyParameters}
                                        onBeforeFetchStats={applyParameters}
                                        onBeforeMenuItemAction={applyParameters}
                                        onBeforeApplyFilter={applyParameters}
                                        onBeforeApplySort={applyParameters}
                                        onAfterMenuItemAction={handleAfterMenuItemAction}
                                        onBeforeExportExcel={applyParameters}
                                        enableExcelExport
                                        enableSelection
                                    />
                                </div>
                            )}
                            to={(<div>
                                <h5 className='form-title'>{t("PRE100AsociarAtributoDetalle_frm1_lbl_AtributosAsociados")}</h5>
                                <Grid
                                    application="PRE100AsociarAtributoDetalle"
                                    id="PRE100AsociarAtributoDetalle_grid_2"
                                    rowsToFetch={30}
                                    rowsToDisplay={7}
                                    onBeforeInitialize={applyParameters}
                                    onBeforeFetch={applyParameters}
                                    onBeforeFetchStats={applyParameters}
                                    onBeforeMenuItemAction={applyParameters}
                                    onBeforeApplyFilter={applyParameters}
                                    onBeforeApplySort={applyParameters}
                                    onAfterMenuItemAction={handleAfterMenuItemAction}
                                    onBeforeExportExcel={applyParameters}
                                    enableExcelExport
                                    enableSelection
                                    onAfterFetch={onAfterFetch}
                                />
                            </div>
                            )}
                        />

                        <AddRemovePanel
                            onAdd={handleDetalleAdd}
                            onRemove={handleDetalleRemove}
                            BtnDisabled={readOnly}
                            from={(
                                <div>
                                    <h5 className='form-title'>{t("PRE100AsociarAtributoDetalle_frm1_lbl_AtributosDetalleSinDefinir")}</h5>
                                    <Grid
                                        application="PRE100AsociarAtributoDetalle"
                                        id="PRE100AsociarAtributoDetalle_gridDetalle_1"
                                        rowsToFetch={30}
                                        rowsToDisplay={7}
                                        onBeforeInitialize={applyParameters}
                                        onBeforeFetch={applyParameters}
                                        onBeforeFetchStats={applyParameters}
                                        onBeforeMenuItemAction={applyParameters}
                                        onBeforeApplyFilter={applyParameters}
                                        onBeforeApplySort={applyParameters}
                                        onAfterMenuItemAction={handleAfterMenuItemAction}
                                        onBeforeExportExcel={applyParameters}
                                        enableExcelExport
                                        enableSelection
                                    />
                                </div>
                            )}
                            to={(<div>
                                <h5 className='form-title'>{t("PRE100AsociarAtributoDetalle_frm1_lbl_AtributosDetalleAsociados")}</h5>
                                <Grid
                                    application="PRE100AsociarAtributoDetalle"
                                    id="PRE100AsociarAtributoDetalle_gridDetalle_2"
                                    rowsToFetch={30}
                                    rowsToDisplay={7}
                                    onBeforeInitialize={applyParameters}
                                    onBeforeFetch={applyParameters}
                                    onBeforeFetchStats={applyParameters}
                                    onBeforeMenuItemAction={applyParameters}
                                    onBeforeApplyFilter={applyParameters}
                                    onBeforeApplySort={applyParameters}
                                    onAfterMenuItemAction={handleAfterMenuItemAction}
                                    onBeforeExportExcel={applyParameters}
                                    enableExcelExport
                                    enableSelection
                                    onAfterFetch={onAfterFetch}
                                />
                            </div>
                            )}
                        />


                    </Modal.Body>
                    <Modal.Footer>
                        <Button id="btnCerrar" variant="outline-secondary" onClick={handleClose}> {t("General_Sec0_btn_Cerrar")}</Button>
                        <SubmitButton id="btnSubmitAsociarAtributos" variant="primary" label="General_Sec0_btn_Confirmar" />
                    </Modal.Footer>

                    <PRE100DefinicionAtributo show={showAtributoDefinicion} onHide={closeAtributosFormDialog} datos={datos} listAtributos={listAtributos} />
                </Form>
            </Page>
        </Modal>
    );
}