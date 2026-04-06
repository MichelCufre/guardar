import React, { useEffect, useLayoutEffect, useRef, useState } from 'react';
import { Button, Col, FormGroup, Modal, Row, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldDate, FieldSelect, FieldSelectAsync, FieldToggle, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { FormWarningMessage } from '../../components/FormComponents/FormWarningMessage';
import { Grid } from '../../components/GridComponents/Grid';
import { useLoading } from '../../components/LoadingOverlay';
export function PRE052CreatePrepAdministrativoModal(props) {
    const { hideLoadingOverlay, showLoadingOverlay } = useLoading()
    const { t } = useTranslation("translation", { useSuspense: false });
    const [keyTab, setKeyTab] = useState("stockSuelto");
    const [stockSelection, setStockSelection] = useState(null);
    const [lpnSelection, setLpnSelection] = useState(null);
    const [_nexus, setNexus] = useState(null);
    const [isHabilitada, setIsHabilitada] = useState(false);
    const [isHabilitadoTabSuelto, setIsHabilitadoTabSuelto] = useState(true);
    const [isHabilitadoTabLpn, setIsHabilitadoTabLpn] = useState(true);

    const [_empresa, setEmpresa] = useState("");
    const [_predio, setPredio] = useState("");
    const [_tpPedido, setTpPedido] = useState("");
    const [_tpExpedicion, setTpExpedicion] = useState("");
    const [_ruta, setRuta] = useState("");
    const [_pedido, setPedido] = useState("");
    const [_fechaEntrega, setFechaEntrega] = useState("");
    const [_descripcion, setDescripcion] = useState("");
    const [_cliente, setCliente] = useState("");
    const [_preparacion, setPreparacion] = useState(null);
    const [_permitePickearVencido, setPermitePickearVencido] = useState(null);
    const [_cantidadPrevVenc, setCantidadPrevVenc] = useState(null);
    const [_condicionStock, setCondicionStock] = useState(null);
    const [isDisabledEmpresaAndPredio, setIsDisabledEmpresaAndPredio] = useState(false);
    const [isEmpresaDocumental, setIsEmpresaDocumental] = useState(false);

    const [menuItemActionGrid1, setMenuItemActionGrid1] = useState(false);
    const [menuItemActionGrid2, setMenuItemActionGrid2] = useState(false);

    const preparacionRef = useRef(null);

    const initialValues = {
        descripcion: "",
        predio: "",
        empresa: "",
        cliente: "",
        tipoExpedicion: "",
        tipoPedido: "",
        permitePickearVencido: "",
        condicionStock: "",
        ruta: "",
        pedido: "",
        fechaEntrega: "",
        cantidadPrevVenc: "",
        traspasoParcial: ""
    };

    const validationSchema = {
        descripcion: Yup.string(),
        predio: Yup.string().required(),
        empresa: Yup.string().required(),
        cliente: Yup.string().required(),
        tipoExpedicion: Yup.string().required(),
        tipoPedido: Yup.string().required(),
        permitePickearVencido: Yup.boolean(),
        condicionStock: Yup.string(),
        ruta: Yup.string().required(),
        pedido: Yup.string(),
        fechaEntrega: Yup.string(),
        cantidadPrevVenc: Yup.string(),
        traspasoParcial: Yup.string(),
    };

    useEffect(() => {
        if (menuItemActionGrid1 && menuItemActionGrid2 && _nexus !== null) {

            _nexus.getGrid("PickingSuelto_grid_1").commit(true, true);
        }
    }, [menuItemActionGrid1, menuItemActionGrid2]);

    
    useEffect(() => {
        if (_empresa !== "" && _empresa !== null && _predio !== "" && _predio !== null) {
            showLoadingOverlay();
            setIsDisabledEmpresaAndPredio(true);
        }

        if (_nexus !== null) {
            _nexus.getGrid("PickingSuelto_grid_1").refresh();
            _nexus.getGrid("PickingLpn_grid_2").refresh();
        }

    }, [_empresa, _predio, _nexus]);

    useLayoutEffect(() => {
        if (props.show) {
            resetState();
        }
    }, [props.show]);

    const onBeforeInitializeForms = (context, form, query, nexus) => {

        if (props.empresa !== undefined) {
            query.parameters.push({ id: "empresa", value: props.empresa });
            query.parameters.push({ id: "predio", value: props.predio });
            query.parameters.push({ id: "tpPedido", value: props.tpPedido });
            query.parameters.push({ id: "tpExpedicion", value: props.tpExpedicion });
            query.parameters.push({ id: "ruta", value: props.ruta });
            query.parameters.push({ id: "fechaEntrega", value: props.fechaEntrega });
            setIsHabilitadoTabSuelto(props.isHabilitadoTabSuelto);
            setIsHabilitadoTabLpn(props.isHabilitadoTabLpn);
            setKeyTab(!props.isHabilitadoTabSuelto ? (!props.isHabilitadoTabLpn ? " " : "stockLPN") : "stockSuelto");
        }
    };

    const onAfterInitializeForms = (context, form, query, nexus) => {

        if (query.parameters.some(d => d.id === "empresa"))
            setEmpresa(query.parameters.find(d => d.id === "empresa").value);

        if (query.parameters.some(d => d.id === "predio"))
            setPredio(query.parameters.find(d => d.id === "predio").value);

        if (query.parameters.some(d => d.id === "tpPedido"))
            setTpPedido(query.parameters.find(d => d.id === "tpPedido").value);

        if (query.parameters.some(d => d.id === "tpExpedicion"))
            setTpExpedicion(query.parameters.find(d => d.id === "tpExpedicion").value);

        if (query.parameters.some(d => d.id === "ruta"))
            setRuta(query.parameters.find(d => d.id === "ruta").value);

        if (query.parameters.some(d => d.id === "pedido"))
            setPedido(query.parameters.find(d => d.id === "pedido").value);

        if (query.parameters.some(d => d.id === "fechaEntrega"))
            setFechaEntrega(query.parameters.find(d => d.id === "fechaEntrega").value);

        setPermitePickearVencido(query.parameters.find(d => d.id === "permitePickearVencido").value);
        setCantidadPrevVenc(query.parameters.find(d => d.id === "cantidadPrevVenc").value);
        setCondicionStock(query.parameters.find(d => d.id === "condicionStock").value);

        if (query.parameters.some(d => d.id === "isEmpresaDocumental"))
            setIsEmpresaDocumental(query.parameters.find(d => d.id === "isEmpresaDocumental").value == "S");
    };

    const onAfterValidateField = (context, form, query, nexus) => {

        if (query.parameters.some(d => d.id === "empresa")) {
            setIsEmpresaDocumental(query.parameters.find(d => d.id === "isEmpresaDocumental").value == "S");
            setEmpresa(query.parameters.find(d => d.id === "empresa").value);
        }

        if (query.parameters.some(d => d.id === "cliente"))
            setCliente(query.parameters.find(d => d.id === "cliente").value);

        if (query.parameters.some(d => d.id === "predio"))
            setPredio(query.parameters.find(d => d.id === "predio").value);

        if (query.parameters.some(d => d.id === "tpPedido"))
            setTpPedido(query.parameters.find(d => d.id === "tpPedido").value);

        if (query.parameters.some(d => d.id === "tpExpedicion"))
            setTpExpedicion(query.parameters.find(d => d.id === "tpExpedicion").value);

        if (query.parameters.some(d => d.id === "ruta"))
            setRuta(query.parameters.find(d => d.id === "ruta").value);

        if (query.parameters.some(d => d.id === "pedido"))
            setPedido(query.parameters.find(d => d.id === "pedido").value);

        if (query.parameters.some(d => d.id === "fechaEntrega"))
            setFechaEntrega(query.parameters.find(d => d.id === "fechaEntrega").value);

        if (query.parameters.some(d => d.id === "descripcion"))
            setDescripcion(query.parameters.find(d => d.id === "descripcion").value);

        if (query.parameters.some(d => d.id === "permitePickearVencido") ||
            query.parameters.some(d => d.id === "cantidadPrevVenc") ||
            query.parameters.some(d => d.id === "condicionStock") || query.fieldId === "traspasoParcial") {

            if (query.parameters.some(d => d.id === "permitePickearVencido"))
                setPermitePickearVencido(query.parameters.find(d => d.id === "permitePickearVencido").value);

            if (query.parameters.some(d => d.id === "cantidadPrevVenc"))
                setCantidadPrevVenc(query.parameters.find(d => d.id === "cantidadPrevVenc").value);

            if (query.parameters.some(d => d.id === "condicionStock"))
                setCondicionStock(query.parameters.find(d => d.id === "condicionStock").value);

            _nexus.getGrid("PickingSuelto_grid_1").reset();
            _nexus.getGrid("PickingLpn_grid_2").reset();
        }

    };

    const onBeforeButtonAction = (context, form, data, nexus) => {
        data.parameters.push({ id: "pedido", value: _pedido });
    }

    const handleFormbeforeSubmit = (context, form, query, nexus) => {
        query.parameters.push({ id: "isSubmit", value: true });
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        setIsHabilitada(true);
        context.abortHideLoading = true;
        PickingMasivo();

    }
    const PickingMasivo = () => {
        _nexus.getGrid("PickingSuelto_grid_1").triggerMenuAction("btnExpedir", false);
        _nexus.getGrid("PickingLpn_grid_2").triggerMenuAction("btnExpedir", false);
    }


    const onAfterInitialize = (context, grid, parameters, nexus) => {
        setNexus(nexus);

    };

    const addParameters = (context, data, nexus) => {
        let traspasoParcial = document.getElementById("traspasoParcial").checked;
        data.parameters.push({ id: "traspasoParcial", value: traspasoParcial });
        data.parameters.push({ id: "empresa", value: _empresa });
        data.parameters.push({ id: "predio", value: _predio });
        data.parameters.push({ id: "permitePickearVencido", value: _permitePickearVencido });
        data.parameters.push({ id: "cantidadPrevVenc", value: _cantidadPrevVenc });
        data.parameters.push({ id: "condicionStock", value: _condicionStock });
    };

    const onAfterMenuItemAction = (context, data, nexus) => {

        if (data.gridId == "PickingSuelto_grid_1") {
            if (data.parameters.some(w => w.id === "PRD052_StockPicking"))
                setStockSelection(data.parameters.find(w => w.id === "PRD052_StockPicking").value)
            setMenuItemActionGrid1(true);
        }
        if (data.gridId == "PickingLpn_grid_2") {
            if (data.parameters.some(w => w.id === "PRD052_LpnPicking"))
                setLpnSelection(data.parameters.find(w => w.id === "PRD052_LpnPicking").value)
            setMenuItemActionGrid2(true);
        }
        context.abortUpdate = true;
        context.abortHideLoading = true;

    }
    const onBeforeCommit = (context, data, nexus) => {
        data.parameters.push({ id: "descripcion", value: _descripcion });
        data.parameters.push({ id: "predio", value: _predio });
        data.parameters.push({ id: "empresa", value: _empresa });
        data.parameters.push({ id: "cliente", value: _cliente });
        data.parameters.push({ id: "tpPedido", value: _tpPedido });
        data.parameters.push({ id: "tpExpedicion", value: _tpExpedicion });
        data.parameters.push({ id: "ruta", value: _ruta });
        data.parameters.push({ id: "pedido", value: _pedido });
        data.parameters.push({ id: "preparacion", value: _preparacion })
        data.parameters.push({ id: "fechaEntrega", value: _fechaEntrega });
        data.parameters.push({ id: "PRD052_StockPicking", value: stockSelection });
        data.parameters.push({ id: "PRD052_LpnPicking", value: lpnSelection });
    }

    const onAfterCommit = (context, rows, parameters, nexus) => {
        setIsHabilitada(false);

        if (parameters.some(x => x.id == "pedido")) {
            let pedido = parameters.find(x => x.id == "pedido").value
            _nexus.getForm("PRE052CreatePrepAdm_Form_1").setFieldValue("pedido", pedido);
            setPedido(pedido);
        }

        if (parameters.some(d => d.id === "preparacion")) {
            let prepa = parameters.find(x => x.id == "preparacion").value;
            setPreparacion(prepa);
            preparacionRef.current = prepa;
        }

        _nexus.getForm("PRE052CreatePrepAdm_Form_1").clickButton("disabledAllField");
        hideLoadingOverlay();
        if (!parameters.some(w => w.id === "ERROR") && !(context.status === "ERROR")) {

            nexus.showConfirmation({
                message: "General_Sec0_Info_DeseaSegirSeleccionado",
                acceptLabel: "General_Sec0_btn_SI",
                cancelLabel: "General_Sec0_btn_NO",
                onAccept: () => handleSegirPreparando(),
                onCancel: () => handleNoSegirPreparando()
            });
        }
        else {
            _nexus.getGrid("PickingSuelto_grid_1").reset();
            _nexus.getGrid("PickingLpn_grid_2").reset();
            setStockSelection(null);
            setLpnSelection(null);
            setMenuItemActionGrid1(false);
            setMenuItemActionGrid2(false);
        }
    }
    const resetState = () => {
        setEmpresa("");
        setPredio("");
        setTpPedido("");
        setTpExpedicion("");
        setRuta("");
        setPedido("");
        setFechaEntrega("");
        setDescripcion("");
        setCliente("");
        setPreparacion(null);
        preparacionRef.current = null;
        setStockSelection(null);
        setLpnSelection(null);
        setMenuItemActionGrid1(false);
        setMenuItemActionGrid2(false);
        setIsDisabledEmpresaAndPredio(false);
        setPermitePickearVencido("N");
        setCantidadPrevVenc("0");
        setCondicionStock("T");
    };
    const handleNoSegirPreparando = () => {
        if (props.empresa !== undefined) {
            props.onHide(preparacionRef.current);
        } else {
            props.onHide(_nexus);
        }

    };
    const handleSegirPreparando = () => {
        setMenuItemActionGrid1(false);
        setMenuItemActionGrid2(false);
        setStockSelection(null);
        setLpnSelection(null);
        _nexus.getGrid("PickingSuelto_grid_1").reset();
        _nexus.getGrid("PickingLpn_grid_2").reset();
    };

    return (
        <Modal show={props.show} onHide={handleNoSegirPreparando} dialogClassName="modal-80w" backdrop="static">
            <Form
                id="PRE052CreatePrepAdm_Form_1"
                application="PRE052CreatePrepAdm"
                initialValues={initialValues}
                onBeforeInitialize={onBeforeInitializeForms}
                onAfterInitialize={onAfterInitializeForms}
                validationSchema={validationSchema}
                onAfterValidateField={onAfterValidateField}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeSubmit={handleFormbeforeSubmit}
                onBeforeButtonAction={onBeforeButtonAction}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("PRE052CreatePrepAdm_Sec0_mdl_PrepTitulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col>
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="pedido">{t("PRE052CreatePrepAdm_frm1_lbl_pedido")}</label>
                                        <Field name="pedido" />
                                        <StatusMessage for="pedido" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="predio">{t("PRE052CreatePrepAdm_frm1_lbl_Predio")}</label>
                                        <FieldSelect name="predio" isDisabled={isDisabledEmpresaAndPredio} />
                                        <StatusMessage for="predio" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="descripcion">{t("PRE052CreatePrepAdm_frm1_lbl_Descripcion")}</label>
                                        <Field name="descripcion" />
                                        <StatusMessage for="descripcion" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="empresa">{t("PRE052CreatePrepAdm_frm1_lbl_Empresa")}</label>
                                        <FieldSelectAsync name="empresa" isClearable={true} isDisabled={isDisabledEmpresaAndPredio} />
                                        <StatusMessage for="empresa" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="cliente">{t("PRE052CreatePrepAdm_frm1_lbl_Cliente")}</label>
                                        <FieldSelectAsync name="cliente" isClearable={true} />
                                        <StatusMessage for="cliente" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="ruta">{t("PRE052CreatePrepAdm_frm1_lbl_ruta")}</label>
                                        <FieldSelectAsync name="ruta" isClearable={true} />
                                        <StatusMessage for="ruta" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="tipoExpedicion">{t("PRE052CreatePrepAdm_frm1_lbl_tpExpedicion")}</label>
                                        <FieldSelect name="tipoExpedicion" isClearable={true} />
                                        <StatusMessage for="tipoExpedicion" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="tipoPedido">{t("PRE052CreatePrepAdm_frm1_lbl_tpPedido")}</label>
                                        <FieldSelect name="tipoPedido" isClearable={true} />
                                        <StatusMessage for="tipoPedido" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="fechaEntrega">{t("PRE052CreatePrepAdm_frm1_lbl_fechaEntrega")}</label>
                                        <FieldDate name="fechaEntrega" />
                                        <StatusMessage for="fechaEntrega" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="condicionStock">{t("PRE250_frm1_lbl_condicionStock")}</label>
                                        <FieldSelect name="condicionStock" />
                                        <StatusMessage for="condicionStock" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="cantidadPrevVenc">{t("PRE052CreatePrepAdm_frm1_lbl_cantidadPrevVenc")}</label>
                                        <Field name="cantidadPrevVenc" />
                                        <StatusMessage for="cantidadPrevVenc" />
                                    </div>
                                </Col>
                                <Col>
                                    <br />
                                    <div className="form-group">
                                        <FieldToggle name="permitePickearVencido" label={t("PRE052CreatePrepAdm_frm1_lbl_PickearVencido")} />
                                        <StatusMessage for="permitePickearVencido" />
                                    </div>
                                </Col>
                            </Row>
                        </Col>
                        <Row>
                            <FormWarningMessage message={t("PRE052_Sec0_btn_EmpresaDocumental")} show={isEmpresaDocumental} />
                        </Row>
                    </Row>

                    <Tabs defaultActiveKey="stockSuelto" transition={false} id="noanim-tab-example"
                        activeKey={keyTab}
                        onSelect={(k) => setKeyTab(k)}
                    >
                        <Tab eventKey="stockSuelto" disabled={!isHabilitadoTabSuelto} title={t("PRD052_frm1_tab_stockSuelto")}>
                            <br></br>
                            <div className="form-group">
                                <FieldToggle id="traspasoParcial" name="traspasoParcial" label={t("PRE052CreatePrepAdm_frm1_lbl_traspasoParcial")} />
                                <StatusMessage for="traspasoParcial" />
                            </div>

                            <div className="row mb-1">
                                <div className="col">
                                    <Grid
                                        id="PickingSuelto_grid_1"
                                        application="PRE052CreatePrepAdm"
                                        rowsToFetch={30}
                                        rowsToDisplay={10}
                                        enableExcelExport
                                        enableExcelImport={false}
                                        enableSelection
                                        onAfterInitialize={onAfterInitialize}
                                        onBeforeInitialize={addParameters}
                                        onBeforeFetch={addParameters}
                                        onBeforeFetchStats={addParameters}
                                        onBeforeExportExcel={addParameters}
                                        onBeforeApplyFilter={addParameters}
                                        onBeforeApplySort={addParameters}
                                        onBeforeMenuItemAction={addParameters}
                                        onAfterMenuItemAction={onAfterMenuItemAction}
                                        onBeforeCommit={onBeforeCommit}
                                        onAfterCommit={onAfterCommit}
                                    />
                                </div>
                            </div>
                        </Tab>
                        <Tab eventKey="stockLPN" disabled={!isHabilitadoTabLpn} title={t("PRD052_frm1_tab_stockLPN")}>
                            <br></br>
                            <div className="row mb-1">
                                <div className="col">
                                    <Grid
                                        id="PickingLpn_grid_2"
                                        application="PRE052CreatePrepAdm"
                                        rowsToFetch={30}
                                        rowsToDisplay={10}
                                        enableExcelExport
                                        enableExcelImport={false}
                                        enableSelection
                                        onBeforeInitialize={addParameters}
                                        onBeforeFetch={addParameters}
                                        onBeforeFetchStats={addParameters}
                                        onBeforeExportExcel={addParameters}
                                        onBeforeApplyFilter={addParameters}
                                        onBeforeApplySort={addParameters}
                                        onBeforeMenuItemAction={addParameters}
                                        onAfterMenuItemAction={onAfterMenuItemAction}
                                    />
                                </div>
                            </div>
                        </Tab>
                    </Tabs>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleNoSegirPreparando}>
                        {t("General_Sec0_btn_Cerrar")}
                    </Button>
                    <FormGroup>
                        <SubmitButton id="btnSubmitConfirmar" label="General_Sec0_btn_Confirmar" variant="primary" disabled={isHabilitada} />
                    </FormGroup>
                </Modal.Footer>
            </Form>
        </Modal >
    );
}

