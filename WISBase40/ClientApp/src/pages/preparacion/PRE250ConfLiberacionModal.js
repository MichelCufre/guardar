import React, { useState, useRef } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldToggle, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';

import { CheckboxList } from '../../components/CheckboxList';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';

function InternalPRE250ConfLiberacionModal(props) {

    const { t } = useTranslation();
    const [opcionalHeight, setOpcionalHeight] = useState(20);

    const [isCreate, setIsCreate] = useState(false);

    const [isUpdate, setIsUpdate] = useState(false);

    const [infoRegla, setInfoRegla] = useState({
        NU_REGLA: ""
    });

    const [seleccionEmpresa, setSeleccionEmpresa] = useState(null);

    const [itemsListCondicion, setItemsListCondicion] = useState([]);
    const [allSelectedItemsListCondicion, setAllSelectedItemsListCondicion] = useState(false);

    const [itemsListTpPedido, setItemsListTpPedido] = useState([]);
    const [allSelectedItemsListTpPedido, setAllSelectedItemsListTpPedido] = useState(false);

    const [itemsListTpExpedicion, setItemsListTpExpedicion] = useState([]);
    const [allSelectedItemsListTpExpedicion, setAllSelectedItemsListTpExpedicion] = useState(false);

    const validationSchema = {
        ubicacionCompleta: Yup.string(),
        ubicacionIncompleta: Yup.string(),
        prepSoloCamion: Yup.string(),
        agrupPorCamion: Yup.string(),
        liberarPorUnidades: Yup.string(),
        liberarPorCurvas: Yup.string(),
        manejaVidaUtil: Yup.string(),
        agrupacion: Yup.string().required(),

        stock: Yup.string(),
        pedidos: Yup.string(),
        repartirEscasez: Yup.string(),
        respetaFifo: Yup.string(),
        priorizarDesborde: Yup.string(),
        usarSoloStkPicking: Yup.boolean(),
        excluirUbicPicking: Yup.boolean(),
        stockDtmi: Yup.string(),

        empresa: Yup.string().required(),
        condicionLiberacion: Yup.string(),
        tpPedido: Yup.string(),
        tpExpedicion: Yup.string(),

        predio: Yup.string(),
        pedidosNuevos: Yup.boolean(),
        onda: Yup.string().required(),
        tpAgente: Yup.string(),
        ordenPedidosAuto: Yup.string(),
        clientesPorPrep: Yup.string().required(),
        diasColaTrabajo: Yup.string().nullable(),
    };

    const initialValues = {

        ubicacionCompleta: "",
        ubicacionIncompleta: "",
        prepSoloCamion: "",
        agrupPorCamion: "",
        liberarPorUnidades: "",
        liberarPorCurvas: "",
        manejaVidaUtil: "",
        agrupacion: "",

        stock: "",
        pedidos: "",
        repartirEscasez: "",
        respetaFifo: "",
        priorizarDesborde: "",
        usarSoloStkPicking: "",
        excluirUbicPicking: "",
        stockDtmi: "",

        empresa: "",
        condicionLiberacion: "",
        tpPedido: "",
        tpExpedicion: "",

        predio: "",
        pedidosNuevos: "",
        onda: "",
        tpAgente: "",
        ordenPedidosAuto: "",
        clientesPorPrep: "",
        diasColaTrabajo: "",
    }

    const handleChangeCondicion = (event, id) => {

        //setItemsListModificar(itemsList);

        let itemsModificar = [...itemsListCondicion];

        let item = itemsModificar.find(d => d.id === id);

        item.selected = !item.selected;

        setItemsListCondicion(itemsModificar);
    };

    const handleChangeTpPedido = (event, id) => {

        //setItemsListModificar(itemsList);

        let itemsModificar = [...itemsListTpPedido];

        console.log(itemsModificar);

        let item = itemsModificar.find(d => d.id === id);

        item.selected = !item.selected;

        setItemsListTpPedido(itemsModificar);
    };

    const handleChangeTpExpedicion = (event, id) => {

        //setItemsListModificar(itemsList);

        let itemsModificar = [...itemsListTpExpedicion];

        console.log(itemsModificar);

        let item = itemsModificar.find(d => d.id === id);

        item.selected = !item.selected;

        setItemsListTpExpedicion(itemsModificar);
    };


    const handleAfterInitialize = (context, form, query, nexus) => {

        let jsonCondiciones = query.parameters.find(w => w.id === "ListItemsCondicion").value;
        var arrayCondiciones = JSON.parse(jsonCondiciones.toString());

        let jsonTpPedido = query.parameters.find(w => w.id === "ListItemsTpPedido").value;
        var arrayTpPedido = JSON.parse(jsonTpPedido.toString());

        let jsonTpExpedicion = query.parameters.find(w => w.id === "ListItemsTpExpedicion").value;
        var arrayTpExpedicion = JSON.parse(jsonTpExpedicion.toString());

        let empresa = query.parameters.find(w => w.id === "emp").value;

        setItemsListCondicion(arrayCondiciones);
        setItemsListTpPedido(arrayTpPedido);
        setItemsListTpExpedicion(arrayTpExpedicion);
        setSeleccionEmpresa(empresa);

        if (props.regla) {
            setIsCreate(false)
            setIsUpdate(true)
            setInfoRegla({ NU_REGLA: props.regla.find(a => a.id === "nuRegla").value });
        }
        else {
            setIsCreate(true)
            setIsUpdate(false)
        }
    };

    const handleBeforeInitialize = (context, form, query, nexus) => {

        let parameters = []

        if (props.regla)
            parameters = [{ id: "nuRegla", value: props.regla.find(a => a.id === "nuRegla").value }];

        if (props.formulario)
            props.formulario.forEach(element =>
                parameters.push(element)
            );

        query.parameters = parameters;
    }

    const onBeforeSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (query.buttonId == "btnSubmitConfirmarConf") {
            let parameters =
                [
                    { id: "ubicacionCompleta", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("ubicacionCompleta") },
                    { id: "ubicacionIncompleta", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("ubicacionIncompleta") },
                    { id: "prepSoloCamion", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("prepSoloCamion") },
                    { id: "agrupPorCamion", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("agrupPorCamion") },
                    { id: "liberarPorUnidades", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("liberarPorUnidades") },
                    { id: "liberarPorCurvas", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("liberarPorCurvas") },
                    { id: "manejaVidaUtil", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("manejaVidaUtil") },
                    { id: "agrupacion", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("agrupacion") },

                    { id: "stock", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("stock") },
                    { id: "pedidos", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("pedidos") },
                    { id: "repartirEscasez", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("repartirEscasez") },
                    { id: "respetaFifo", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("respetaFifo") },
                    { id: "priorizarDesborde", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("priorizarDesborde") },
                    { id: "usarSoloStkPicking", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("usarSoloStkPicking") },
                    { id: "excluirUbicPicking", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("excluirUbicPicking") },
                    { id: "stockDtmi", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("stockDtmi") },

                    { id: "empresa", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("empresa") },
                    { id: "condicionLiberacion", value: JSON.stringify(itemsListCondicion) },
                    { id: "tpPedido", value: JSON.stringify(itemsListTpPedido) },
                    { id: "tpExpedicion", value: JSON.stringify(itemsListTpExpedicion) },

                    { id: "predio", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("predio") },
                    { id: "pedidosNuevos", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("pedidosNuevos") },
                    { id: "onda", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("onda") },
                    { id: "tpAgente", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("tpAgente") },
                    { id: "ordenPedidosAuto", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("ordenPedidosAuto") },
                    { id: "clientesPorPrep", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("clientesPorPrep") },
                    { id: "diasColaTrabajo", value: nexus.getForm("PRE250ConfigLiberacion_form_1").getFieldValue("diasColaTrabajo") },
                    { id: "isSubmit", value: true }
                ];

            if (props.formulario)
                props.formulario.forEach(element =>
                    parameters.push(element)
                );

            if (props.regla)
                parameters.push({ id: "nuRegla", value: props.regla.find(a => a.id === "nuRegla").value });

            query.parameters = parameters;

        }
        //else {
        //    props.onHide(null);
        //}

    }

    const handleSelectAllItemsCondicion = (selected) => {
        setAllSelectedItemsListCondicion(selected);

        setItemsListCondicion(itemsListCondicion.map(d => ({ ...d, selected: selected })));
    }

    const handleSelectAllItemsTpPedido = (selected) => {
        setAllSelectedItemsListTpPedido(selected);

        setItemsListTpPedido(itemsListTpPedido.map(d => ({ ...d, selected: selected })));
    }

    const handleSelectAllItemsTpExpedicion = (selected) => {
        setAllSelectedItemsListTpExpedicion(selected);

        setItemsListTpExpedicion(itemsListTpExpedicion.map(d => ({ ...d, selected: selected })));
    }

    const handleClose = (id) => {
        let idTest = id.currentTarget.id;
        if (idTest == "btnVolverRegla") {
            props.onHide(props.formulario, null, idTest, props.nexus);
        } else {
            props.onHide(null, null, null, props.nexus);
        }
    };

    const onAfterSubmit = (context, data, nexus) => {
        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            if (nexus.buttonId == "btnSubmitConfirmarConf") {
                props.onHide(null, null, null, props.nexus);
            }
        }
    }

    const onBeforeValidateField = (context, form, query, nexus) => {
        if (props.regla)
            query.parameters = [{ id: "nuRegla", value: props.regla.find(a => a.id === "nuRegla").value }];
    }

    return (

        <Form
            application="PRE250ConfLiberacion"
            id="PRE250ConfigLiberacion_form_1"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onAfterInitialize={handleAfterInitialize}
            onBeforeInitialize={handleBeforeInitialize}
            onBeforeValidateField={onBeforeValidateField}
            onAfterSubmit={onAfterSubmit}
            onBeforeSubmit={onBeforeSubmit}
        >

            <Modal.Header closeButton>
                <Modal.Title>
                    <Container fluid style={{ display: isCreate ? 'block' : 'none' }} >
                        {t("PRE250_frm1_title_ConfigLiberacion")}
                    </Container>
                    <Container fluid style={{ display: isUpdate ? 'block' : 'none' }} >
                        {t("PRE250_frm1_title_ConfigLiberacionEdit")} {`${infoRegla.NU_REGLA}`}
                    </Container>
                </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="empresa">{t("PRE250_frm1_lbl_empresa")}</label>
                                <FieldSelectAsync name="empresa" />
                                <StatusMessage for="empresa" />
                            </div>

                            <div className="form-group" >
                                <label htmlFor="predio">{t("PRE250_frm1_lbl_predio")}</label>
                                <FieldSelect name="predio" />
                                <StatusMessage for="predio" />
                            </div>

                            <div className="form-group" >
                                <label htmlFor="onda">{t("PRE250_frm1_lbl_onda")}</label>
                                <FieldSelect name="onda" />
                                <StatusMessage for="onda" />
                            </div>

                            <div className="form-group" >
                                <label htmlFor="tpAgente">{t("PRE250_frm1_lbl_tpAgente")}</label>
                                <FieldSelect name="tpAgente" />
                                <StatusMessage for="tpAgente" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="condicionLiberacion">{t("PRE250_frm1_lbl_condicionLiberacion")}</label>
                                <CheckboxList
                                    items={itemsListCondicion}
                                    onChange={handleChangeCondicion}
                                    height={opcionalHeight}
                                    allSelected={allSelectedItemsListCondicion}
                                    onSelectAllChange={handleSelectAllItemsCondicion}
                                />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="tpExpedicion">{t("PRE250_frm1_lbl_tpExpedicion")}</label>
                                <CheckboxList
                                    items={itemsListTpExpedicion}
                                    onChange={handleChangeTpExpedicion}
                                    height={opcionalHeight}
                                    allSelected={allSelectedItemsListTpExpedicion}
                                    onSelectAllChange={handleSelectAllItemsTpExpedicion}
                                />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="tpPedido">{t("PRE250_frm1_lbl_tpPedido")}</label>
                                <CheckboxList
                                    items={itemsListTpPedido}
                                    onChange={handleChangeTpPedido}
                                    height={opcionalHeight}
                                    allSelected={allSelectedItemsListTpPedido}
                                    onSelectAllChange={handleSelectAllItemsTpPedido}
                                />
                            </div>
                        </Col>
                    </Row>
                    <hr></hr>
                    <Row>
                        {/* Columna 1*/}
                        <Col>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="ubicacionCompleta">{t("PRE250_frm1_lbl_ubicacionCompleta")}</label>
                                        <FieldSelect name="ubicacionCompleta" />
                                        <StatusMessage for="ubicacionCompleta" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="ubicacionIncompleta">{t("PRE250_frm1_lbl_ubicacionIncompleta")}</label>
                                        <FieldSelect name="ubicacionIncompleta" />
                                        <StatusMessage for="ubicacionIncompleta" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="manejaVidaUtil">{t("PRE250_frm1_lbl_manejaVidaUtil")}</label>
                                        <FieldSelect name="manejaVidaUtil" />
                                        <StatusMessage for="manejaVidaUtil" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="agrupacion">{t("PRE250_frm1_lbl_agrupacion")}</label>
                                        <FieldSelect name="agrupacion" />
                                        <StatusMessage for="agrupacion" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="prepSoloCamion">{t("PRE250_frm1_lbl_prepSoloCamion")}</label>
                                        <FieldSelect name="prepSoloCamion" />
                                        <StatusMessage for="prepSoloCamion" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="agrupPorCamion">{t("PRE250_frm1_lbl_agrupPorCamion")}</label>
                                        <FieldSelect name="agrupPorCamion" />
                                        <StatusMessage for="agrupPorCamion" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" style={{ marginTop: "40px" }}>
                                        <FieldToggle name="usarSoloStkPicking" label={t("PRE250_frm1_lbl_usarSoloStkPicking")} />
                                    </div>
                                </Col>
                            </Row>
                        </Col>
                        {/* Columna 2*/}
                        <Col>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="stock">{t("PRE250_frm1_lbl_stock")}</label>
                                        <FieldSelect name="stock" />
                                        <StatusMessage for="stock" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="pedidos">{t("PRE250_frm1_lbl_pedidos")}</label>
                                        <FieldSelect name="pedidos" />
                                        <StatusMessage for="pedidos" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="repartirEscasez">{t("PRE250_frm1_lbl_repartirEscasez")}</label>
                                        <FieldSelect name="repartirEscasez" />
                                        <StatusMessage for="repartirEscasez" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="respetaFifo">{t("PRE250_frm1_lbl_respetaFifo")}</label>
                                        <FieldSelect name="respetaFifo" />
                                        <StatusMessage for="respetaFifo" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="ordenPedidosAuto">{t("PRE250_frm1_lbl_ordenPedidos")}</label>
                                        <FieldSelect name="ordenPedidosAuto" />
                                        <StatusMessage for="ordenPedidosAuto" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="clientesPorPrep">{t("PRE250_frm1_lbl_clientesPorPrep")}</label>
                                        <Field name="clientesPorPrep" />
                                        <StatusMessage for="clientesPorPrep" />
                                    </div>
                                </Col>
                            </Row>
                        </Col>
                        {/* Columna 3*/}
                        <Col>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="liberarPorUnidades">{t("PRE250_frm1_lbl_liberarPorUnidades")}</label>
                                        <FieldSelect name="liberarPorUnidades" />
                                        <StatusMessage for="liberarPorUnidades" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="liberarPorCurvas">{t("PRE250_frm1_lbl_liberarPorCurvas")}</label>
                                        <FieldSelect name="liberarPorCurvas" disabled={true} />
                                        <StatusMessage for="liberarPorCurvas" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="priorizarDesborde">{t("PRE250_frm1_lbl_priorizarDesborde")}</label>
                                        <FieldSelect name="priorizarDesborde" />
                                        <StatusMessage for="priorizarDesborde" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="stockDtmi">{t("PRE250_frm1_lbl_stockDtmi")}</label>
                                        <FieldSelect name="stockDtmi" />
                                        <StatusMessage for="stockDtmi" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="diasColaTrabajo">{t("PRE250_frm1_lbl_diasColaTrabajo")}</label>
                                        <FieldSelect name="diasColaTrabajo" />
                                        <StatusMessage for="diasColaTrabajo" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" style={{ marginTop: "40px" }}>
                                        <FieldToggle name="pedidosNuevos" label={t("PRE250_frm1_lbl_pedidosNuevos")} />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" style={{ marginTop: "40px" }}>
                                        <FieldToggle name="excluirUbicPicking" label={t("PRE052_frm1_lbl_ExcluirUbicPicking")} />
                                    </div>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("PRE052_frm1_btn_cerrar")} </Button>
                <Button id="btnVolverRegla" variant="btn btn-outline-secondary" onClick={handleClose.bind(this)}> {t("PRE052_frm1_btn_atras")} </Button>
                <SubmitButton id="btnSubmitConfirmarConf" variant="primary" label="PRE250_frm1_btn_Confirmar" />
            </Modal.Footer>
        </Form >

    );
}

export const PRE250ConfLiberacionModal = withPageContext(InternalPRE250ConfLiberacionModal);