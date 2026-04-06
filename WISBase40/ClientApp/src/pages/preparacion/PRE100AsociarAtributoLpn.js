import React, { useState, useEffect } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Form, Field, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import * as Yup from 'yup';
import PRE100DefinicionAtributoLpn from './PRE100DefinicionAtributoLpn';

export default function PRE100AsociarAtributoLpn(props) {
    const { t } = useTranslation();

    const [update, setUpdate] = useState("N");
    const [nexus, setNexus] = useState("");
    const [listAtributos, setListAtributos] = useState("");
    const [showAtributoDefinicion, setShowAtributoDefinicion] = useState(false);
    const [readOnly, setReadOnly] = useState(false);
    const [datos, setDatos] = useState("");

    const [empresaNombre, setEmpresaNombre] = useState("");
    const [agenteDescripcion, setAgenteDescripcion] = useState("");
    const [agenteCodigo, setAgenteCodigo] = useState("");
    const [agenteTipo, setAgenteTipo] = useState("");
    const [pedido, setPedido] = useState("");
    const [empresa, setEmpresa] = useState("");

    const initialValues = { cantidad: "" };
    const validationSchema = { cantidad: Yup.string() };

    useEffect(() => {
        setShowAtributoDefinicion(false);

        if (listAtributos) {
            setShowAtributoDefinicion(true);
        }
    }, [listAtributos]);

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "pedido", value: props.datos.find(d => d.id === "pedido").value },
            { id: "cliente", value: props.datos.find(d => d.id === "cliente").value },
            { id: "empresa", value: props.datos.find(d => d.id === "empresa").value },
            { id: "producto", value: props.datos.find(d => d.id === "producto").value },
            { id: "faixa", value: props.datos.find(d => d.id === "faixa").value },
            { id: "identificador", value: props.datos.find(d => d.id === "identificador").value },
            { id: "idEspecificaIdentificador", value: props.datos.find(d => d.id === "idEspecificaIdentificador").value },
            { id: "tipoLpn", value: props.datos.find(d => d.id === "tipoLpn").value },
            { id: "idExternoLpn", value: props.datos.find(d => d.id === "idExternoLpn").value },
            { id: "idConfiguracion", value: props.datos.find(d => d.id === "idConfiguracion")?.value },
            { id: "cantidad", value: props.datos.find(d => d.id === "cantidad")?.value },
            { id: "update", value: update },
            { id: "readOnly", value: readOnly },

        ];

        setNexus(nexus);
    };

    const applyParametersFormulario = (context, form, data, nexus) => {
        data.parameters = [
            { id: "pedido", value: props.datos.find(d => d.id === "pedido").value },
            { id: "cliente", value: props.datos.find(d => d.id === "cliente").value },
            { id: "empresa", value: props.datos.find(d => d.id === "empresa").value },
            { id: "producto", value: props.datos.find(d => d.id === "producto").value },
            { id: "faixa", value: props.datos.find(d => d.id === "faixa").value },
            { id: "identificador", value: props.datos.find(d => d.id === "identificador").value },
            { id: "idEspecificaIdentificador", value: props.datos.find(d => d.id === "idEspecificaIdentificador").value },
            { id: "tipoLpn", value: props.datos.find(d => d.id === "tipoLpn").value },
            { id: "idExternoLpn", value: props.datos.find(d => d.id === "idExternoLpn").value },
            { id: "idConfiguracion", value: props.datos.find(d => d.id === "idConfiguracion")?.value },
            { id: "cantidad", value: props.datos.find(d => d.id === "cantidad")?.value },
            { id: "update", value: update },
            { id: "readOnly", value: readOnly },
        ];

        setNexus(nexus);
    };

    const handleAfterMenuItemAction = (context, data, nexus) => {

        setUpdate(data.parameters.find(d => d.id === "update")?.value ?? "N")
        var cerrarModal = (data.parameters.find(d => d.id === "terminarOperacion")?.value ?? "N") == "S"

        if (cerrarModal) {
            handleClose()
        }
        else {
            if (data.gridId == "PRE100AsociarAtributoLpn_grid_1") {

                if (data.parameters.find(d => d.id === "listAtributos").value === "") {
                    nexus.getGrid("PRE100AsociarAtributoLpn_grid_1").refresh();
                    nexus.getGrid("PRE100AsociarAtributoLpn_grid_2").refresh();
                }
                else {
                    setListAtributos(data.parameters.find(d => d.id === "listAtributos").value);
                    applyParameters(context, data, nexus);
                }
            }
            else if (data.gridId == "PRE100AsociarAtributoLpn_gridDetalle_1") {

                if (data.parameters.find(d => d.id === "listAtributos").value === "") {
                    nexus.getGrid("PRE100AsociarAtributoLpn_gridDetalle_1").refresh();
                    nexus.getGrid("PRE100AsociarAtributoLpn_gridDetalle_2").refresh();
                }
                else {
                    setListAtributos(data.parameters.find(d => d.id === "listAtributos").value);
                    applyParameters(context, data, nexus);
                }
            }
            else {

                nexus.getGrid("PRE100AsociarAtributoLpn_grid_1").refresh();
                nexus.getGrid("PRE100AsociarAtributoLpn_grid_2").refresh();

                nexus.getGrid("PRE100AsociarAtributoLpn_gridDetalle_1").refresh();
                nexus.getGrid("PRE100AsociarAtributoLpn_gridDetalle_2").refresh();
            }
        }
    }

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("PRE100AsociarAtributoLpn_grid_1").triggerMenuAction("btnAgregarAtributos", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("PRE100AsociarAtributoLpn_grid_2").triggerMenuAction("btnQuitarAtributos", false, evt.ctrlKey);
    };

    const handleDetalleAdd = (evt, nexus) => {
        nexus.getGrid("PRE100AsociarAtributoLpn_gridDetalle_1").triggerMenuAction("btnAgregarAtributosDetalle", false, evt.ctrlKey);
    };

    const handleDetalleRemove = (evt, nexus) => {
        nexus.getGrid("PRE100AsociarAtributoLpn_gridDetalle_2").triggerMenuAction("btnQuitarAtributosDetalle", false, evt.ctrlKey);
    };

    const closeAtributosLpnFormDialog = (listAtributos) => {
        setListAtributos(listAtributos);

        nexus.getGrid("PRE100AsociarAtributoLpn_grid_1").refresh();
        nexus.getGrid("PRE100AsociarAtributoLpn_grid_2").refresh();
        nexus.getGrid("PRE100AsociarAtributoLpn_gridDetalle_1").refresh();
        nexus.getGrid("PRE100AsociarAtributoLpn_gridDetalle_2").refresh();
    };

    const onBeforePageLoad = (data) => {

        var upd = (props.datos.find(d => d.id === "update")?.value ?? "N")
        var pedido = props.datos.find(d => d.id === "pedido").value;
        var empresa = props.datos.find(d => d.id === "empresa").value;

        var datos = [
            { id: "pedido", value: pedido },
            { id: "cliente", value: props.datos.find(d => d.id === "cliente").value },
            { id: "empresa", value: empresa },
            { id: "producto", value: props.datos.find(d => d.id === "producto").value },
            { id: "faixa", value: props.datos.find(d => d.id === "faixa").value },
            { id: "identificador", value: props.datos.find(d => d.id === "identificador").value },
            { id: "idEspecificaIdentificador", value: props.datos.find(d => d.id === "idEspecificaIdentificador").value },
            { id: "tipoLpn", value: props.datos.find(d => d.id === "tipoLpn").value },
            { id: "idExternoLpn", value: props.datos.find(d => d.id === "idExternoLpn").value },
            { id: "idConfiguracion", value: props.datos.find(d => d.id === "idConfiguracion")?.value },
            { id: "cantidad", value: props.datos.find(d => d.id === "cantidad")?.value },
            { id: "update", value: upd }

        ];

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

    const onBeforeSubmit = (context, form, data, nexus) => {

        if (data.buttonId != "btnSubmitAsociarAtributos") {
            context.abortServerCall = true;
        }
        else {
            data.parameters = [
                { id: "pedido", value: props.datos.find(d => d.id === "pedido").value },
                { id: "cliente", value: props.datos.find(d => d.id === "cliente").value },
                { id: "empresa", value: props.datos.find(d => d.id === "empresa").value },
                { id: "producto", value: props.datos.find(d => d.id === "producto").value },
                { id: "faixa", value: props.datos.find(d => d.id === "faixa").value },
                { id: "identificador", value: props.datos.find(d => d.id === "identificador").value },
                { id: "idEspecificaIdentificador", value: props.datos.find(d => d.id === "idEspecificaIdentificador").value },
                { id: "tipoLpn", value: props.datos.find(d => d.id === "tipoLpn").value },
                { id: "idExternoLpn", value: props.datos.find(d => d.id === "idExternoLpn").value },
                { id: "idConfiguracion", value: props.datos.find(d => d.id === "idConfiguracion")?.value },
                { id: "cantidad", value: props.datos.find(d => d.id === "cantidad")?.value },
                { id: "update", value: update },
                { id: "readOnly", value: readOnly },
            ];

            setNexus(nexus);
        }
    }

    const handleClose = () => {
        props.onHide();
    };

    const onAfterSubmit = (context, form, query, nexus) => {
        setNexus(nexus);
        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            handleClose()
        }
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-70w" backdrop="static">

            <Page
                {...props}
                application="PRE100AsociarAtributoLpn"
                onBeforeLoad={onBeforePageLoad}
                onAfterLoad={onAfterPageLoad}
            >
                <Form
                    application="PRE100AsociarAtributoLpn"
                    id="PRE100AsociarAtributoLpn_form_1"
                    initialValues={initialValues}
                    validationSchema={validationSchema}
                    onBeforeInitialize={applyParametersFormulario}
                    onBeforeValidateField={applyParametersFormulario}
                    onBeforeSubmit={onBeforeSubmit}
                    onAfterSubmit={onAfterSubmit}
                >
                    <Modal.Header closeButton>
                        <Modal.Title>{t("PRE100AsociarAtributoLpn_Sec0_modalTitle_Titulo")}</Modal.Title>
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
                            <Col md={2}>
                                <div className="form-group">
                                    <label htmlFor="tipoLpn">{t("PRE100AsociarAtributoLpn_frm1_lbl_TipoLpn")}</label>
                                    <Field name="tipoLpn" readOnly />
                                    <StatusMessage for="tipoLpn" />
                                </div>
                            </Col>
                            <Col md={3}>
                                <div className="form-group">
                                    <label htmlFor="idExternoLpn">{t("PRE100AsociarAtributoLpn_frm1_lbl_IdExternoLpn")}</label>
                                    <Field name="idExternoLpn" readOnly />
                                    <StatusMessage for="idExternoLpn" />
                                </div>
                            </Col>
                            <Col md={3}>
                                <div className="form-group">
                                    <label htmlFor="producto">{t("PRE100AsociarAtributoLpn_frm1_lbl_Producto")}</label>
                                    <Field name="producto" readOnly />
                                    <StatusMessage for="producto" />
                                </div>
                            </Col>
                            <Col md={2}>
                                <div className="form-group">
                                    <label htmlFor="identificador">{t("PRE100AsociarAtributoLpn_frm1_lbl_Identificador")}</label>
                                    <Field name="identificador" readOnly />
                                    <StatusMessage for="identificador" />
                                </div>
                            </Col>
                            <Col md={2}>
                                <div className="form-group">
                                    <label htmlFor="cantidad">{t("PRE100AsociarAtributoLpn_frm1_lbl_Cantidad")}</label>
                                    <Field name="cantidad" />
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
                                    <h5 className='form-title'>{t("PRE100AsociarAtributoLpn_frm1_lbl_AtributosSinDefinir")}</h5>
                                    <Grid
                                        application="PRE100AsociarAtributoLpn"
                                        id="PRE100AsociarAtributoLpn_grid_1"
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
                                <h5 className='form-title'>{t("PRE100AsociarAtributoLpn_frm1_lbl_AtributosAsociados")}</h5>
                                <Grid
                                    application="PRE100AsociarAtributoLpn"
                                    id="PRE100AsociarAtributoLpn_grid_2"
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
                        />

                        <AddRemovePanel
                            onAdd={handleDetalleAdd}
                            onRemove={handleDetalleRemove}
                            BtnDisabled={readOnly}
                            from={(
                                <div>
                                    <h5 className='form-title'>{t("PRE100AsociarAtributoLpn_frm1_lbl_AtributosDetalleSinDefinir")}</h5>
                                    <Grid
                                        application="PRE100AsociarAtributoLpn"
                                        id="PRE100AsociarAtributoLpn_gridDetalle_1"
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
                                <h5 className='form-title'>{t("PRE100AsociarAtributoLpn_frm1_lbl_AtributosDetalleAsociados")}</h5>
                                <Grid
                                    application="PRE100AsociarAtributoLpn"
                                    id="PRE100AsociarAtributoLpn_gridDetalle_2"
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
                        />


                    </Modal.Body>
                    <Modal.Footer>
                        <Button id="btnCerrar" variant="outline-secondary" onClick={handleClose}> {t("General_Sec0_btn_Cerrar")}</Button>
                        <SubmitButton id="btnSubmitAsociarAtributos" variant="primary" label="General_Sec0_btn_Confirmar" />
                    </Modal.Footer>

                    <PRE100DefinicionAtributoLpn show={showAtributoDefinicion} onHide={closeAtributosLpnFormDialog} datos={datos} listAtributos={listAtributos} />
                </Form>
            </Page>
        </Modal>
    );
}