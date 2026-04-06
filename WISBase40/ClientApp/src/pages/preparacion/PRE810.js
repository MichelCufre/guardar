import React, { useState } from 'react';
import { Col, FormGroup, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { FieldCheckbox, FieldRange, FieldSelect, FieldToggle, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import './PRE810.css';
import { PRE810AgregarColaTrabajo } from './PRE810AgregarColaTrabajo';
import { PRE810EditarPonderadorClientes } from './PRE810EditarPonderadorClientes';
import { PRE810EditarPonderadorCondLiberacion } from './PRE810EditarPonderadorCondLiberacion';
import { PRE810EditarPonderadorEmpresas } from './PRE810EditarPonderadorEmpresas';
import { PRE810EditarPonderadorFechaEmitido } from './PRE810EditarPonderadorFechaEmitido';
import { PRE810EditarPonderadorFechaEntrega } from './PRE810EditarPonderadorFechaEntrega';
import { PRE810EditarPonderadorFechaLiberado } from './PRE810EditarPonderadorFechaLiberado';
import { PRE810EditarPonderadorPonderacionGenerica } from './PRE810EditarPonderadorPonderacionGenerica';
import { PRE810EditarPonderadorRutas } from './PRE810EditarPonderadorRutas';
import { PRE810EditarPonderadorTipoExpedicion } from './PRE810EditarPonderadorTipoExpedicion';
import { PRE810EditarPonderadorTipoPedidos } from './PRE810EditarPonderadorTipoPedidos';
import { PRE810EditarPonderadorZonas } from './PRE810EditarPonderadorZonas';


export default function PRE810(props) {
    const { t } = useTranslation();

    const initialValues = {
        colaDeTrabajo: "",
        flagEmpresa: "",
        incrementoEmpresa: "",
        flagCliente: "",
        incrementoCliente: "",
        flagRuta: "",
        incrementoRuta: "",
        flagZona: "",
        incrementoZona: "",
        flagTipoPedido: "",
        incrementoTipoPedido: "",
        flagTipoExpedicion: "",
        incrementoTipoExpedicion: "",
        flagConficionLiberacion: "",
        incrementoCondicionLiberacion: "",
        flagFechaEntrega: "",
        incrementoFechaEntrega: "",
        flagFechaEmitido: "",
        incrementoFechaEmitido: "",
        flagFechaLiberado: "",
        incrementoFechaLiberado: "",
        flagPonderacionGenerica: "",
        incrementoPonderacionGenerica: "",
        flOrdenCalendario: ""
    };

    let validationSchema = {
        colaDeTrabajo: Yup.string().nullable(),
        flagEmpresa: Yup.boolean(),
        incrementoEmpresa: Yup.string(),
        flagCliente: Yup.boolean(),
        incrementoCliente: Yup.string(),
        flagRuta: Yup.boolean(),
        incrementoRuta: Yup.string(),
        flagZona: Yup.boolean(),
        incrementoZona: Yup.string(),
        flagTipoPedido: Yup.boolean(),
        incrementoTipoPedido: Yup.string(),
        flagTipoExpedicion: Yup.boolean(),
        incrementoTipoExpedicion: Yup.string(),
        flagConficionLiberacion: Yup.boolean(),
        incrementoCondicionLiberacion: Yup.string(),
        flagFechaEntrega: Yup.boolean(),
        incrementoFechaEntrega: Yup.string(),
        flagFechaEmitido: Yup.boolean(),
        incrementoFechaEmitido: Yup.string(),
        flagFechaLiberado: Yup.boolean(),
        incrementoFechaLiberado: Yup.string(),
        flagPonderacionGenerica: Yup.boolean(),
        incrementoPonderacionGenerica: Yup.string(),
        flOrdenCalendario: Yup.string(),
    };

    const marks = [
        {
            value: -100,
            label: '-100',
        },
        {
            value: -50,
            label: '-50',
        },
        {
            value: 0,
            label: '0',
        },
        {
            value: 50,
            label: '50',
        },
        {
            value: 100,
            label: '100',
        },
    ];

    const [showPopupAddColaTrab, setShowPopupAddColaTrab] = useState();
    const [nuColaDeTrabajo, setNuColaDeTrabajo] = useState()
    const [cdPonderador, setCdPonderador] = useState();
    const [flReload, setflReload] = useState(true);
    const [showPopupPonderadorEmpresas, setShowPopupPonderadorEmpresas] = useState(false);
    const [showPopupPonderadorClientes, setShowPopupPonderadorClientes] = useState(false);
    const [showPopupPonderadorRutas, setShowPopupPonderadorRutas] = useState(false);
    const [showPopupPonderadorZonas, setShowPopupPonderadorZonas] = useState(false);
    const [showPopupPonderadorTpPedidos, setShowPopupPonderadorTpPedidos] = useState(false);
    const [showPopupPonderadorTpExpedicion, setShowPopupPonderadorTpExpedicion] = useState(false);
    const [showPopupPonderadorCondLiberacion, setShowPopupPonderadorCondLiberacion] = useState(false);
    const [showPopupPonderadorFechaEntrega, setShowPopupPonderadorFechaEntrega] = useState(false);
    const [showPopupPonderadorFechaEmitido, setShowPopupPonderadorFechaEmitido] = useState(false);
    const [showPopupPonderadorFechaLiberado, setShowPopupPonderadorFechaLiberado] = useState(false);
    const [showPopupPonderadorPonderacionGenerica, setShowPopupPonderadorPonderacionGenerica] = useState(false);


    const [showForm, setShowForm] = useState(true);


    const openAddColaDeTrabDialog = () => {
        setShowPopupAddColaTrab(true);
    };
    const closeAddColaDeTrabDialog = () => {
        setShowPopupAddColaTrab(false);
    };

    const openEditarEmpresaDialog = () => {
        setShowPopupPonderadorEmpresas(true);
    }
    const closePonderadorEmpresasDialog = () => {
        setShowPopupPonderadorEmpresas(false);
    }


    const openEditarClienteDialog = () => {
        setShowPopupPonderadorClientes(true);
    }
    const closePonderadorClientesDialog = () => {
        setShowPopupPonderadorClientes(false);
    }


    const openEditarRutaDialog = () => {
        setShowPopupPonderadorRutas(true);
    }
    const closePonderadorRutasDialog = () => {
        setShowPopupPonderadorRutas(false);
    }


    const openEditarZonaDialog = () => {
        setShowPopupPonderadorZonas(true);
    }
    const closePonderadorZonasDialog = () => {
        setShowPopupPonderadorZonas(false);
    }


    const openEditarTpPedidosDialog = () => {
        setShowPopupPonderadorTpPedidos(true);
    }
    const closePonderadorTpPedidosDialog = () => {
        setShowPopupPonderadorTpPedidos(false);
    }
    const openEditarTpExpedicionDialog = () => {
        setShowPopupPonderadorTpExpedicion(true);
    }
    const closePonderadorTpExpedicionDialog = () => {
        setShowPopupPonderadorTpExpedicion(false);
    }


    const openEditarCondLiberacionDialog = () => {
        setShowPopupPonderadorCondLiberacion(true);
    }
    const closePonderadorCondLiberacionDialog = () => {
        setShowPopupPonderadorCondLiberacion(false);
    }

    const openEditarFechaEntregaDialog = () => {
        setShowPopupPonderadorFechaEntrega(true);
    }
    const closePonderadorFechaEntregaDialog = () => {
        setShowPopupPonderadorFechaEntrega(false);
    }

    const openEditarFechaEmitidoDialog = () => {
        setShowPopupPonderadorFechaEmitido(true);
    }
    const closePonderadorFechaEmitidoDialog = () => {
        setShowPopupPonderadorFechaEmitido(false);
    }

    const openEditarFechaLiberadoDialog = () => {
        setShowPopupPonderadorFechaLiberado(true);
    }
    const closePonderadorFechaLiberadoDialog = () => {
        setShowPopupPonderadorFechaLiberado(false);
    }

    const openEditarPonderacionGenericaDialog = () => {
        setShowPopupPonderadorPonderacionGenerica(true);
    }
    const closePonderadorPonderacionGenericaDialog = () => {
        setShowPopupPonderadorPonderacionGenerica(false);
    }

    const onBeforeSubmit = (context, form, query, nexus) => {
        if (query.buttonId !== "btnGuardar")
            context.abortServerCall = true;
        else {
            if (query.fieldId == "colaDeTrabajo") {
                var newColaDeTrabajo = nexus.getForm("PRE810_form_1").getFieldValue("colaDeTrabajo")

                //context.abortServerCall = true;
                if (newColaDeTrabajo != null && nuColaDeTrabajo != newColaDeTrabajo) {

                    setNuColaDeTrabajo(newColaDeTrabajo);

                    setShowForm(false);

                    setTimeout(() => {

                        setShowForm(true);

                    }, 1);

                }
            }
        }
    }

    const onBeforeValidateField = (context, form, query, nexus) => {

        var field = form.fields.find(w => w.id === "colaDeTrabajo");
        if (field.value == null || field.value == "") {
            field.value = nuColaDeTrabajo;
        }

        if (query.fieldId == "colaDeTrabajo") {

            var newColaDeTrabajo = nexus.getForm("PRE810_form_1").getFieldValue("colaDeTrabajo")
            //context.abortServerCall = true;
            if (newColaDeTrabajo != null && nuColaDeTrabajo != newColaDeTrabajo) {

                setNuColaDeTrabajo(newColaDeTrabajo);

                setShowForm(false);

                setTimeout(() => {

                    setShowForm(true);

                }, 1);
            }
        }
        else if (query.fieldId == "incrementoPonderacionGenerica") {

            var newColaDeTrabajo = nexus.getForm("PRE810_form_1").getFieldValue("colaDeTrabajo")

            if (newColaDeTrabajo != null && newColaDeTrabajo != "" && flReload) {

                setNuColaDeTrabajo(newColaDeTrabajo);

                setShowForm(false);

                setTimeout(() => {

                    setShowForm(true);

                }, 1);

                setflReload(false)
            }
        }

    };

    const onBeforeButtonAction = (context, form, query, nexus) => {

        if (query.buttonId != "colaDeTrabajo") {

            query.parameters = [
                {
                    id: "COLADETRABAJO",
                    value: nuColaDeTrabajo
                },
            ];

        }
        else {

            context.abortServerCall = true;
        }

    }


    const onAfterInitialize = (context, form, query, nexus) => {
        // var f = nexus.getForm("PRE810_form_1");
        //  var newColaDeTrabajo = nexus.getForm("PRE810_form_1").getFieldValue("incrementoEmpresa")
        // var sq = query.parameters.find(d => d.id === "flagEmpresa");
        // f.setFieldValue("flagEmpresa", query.parameters.find(d => d.id === "flagEmpresa").value);
        //setIncrementoEmpresa(query.parameters.find(d => d.id === "flagEmpresa").value);
        //f.setFieldValue("incrementoEmpresa", query.parameters.find(d => d.id === "incrementoEmpresa").value);
        // f.setFieldValue("flagCliente", query.parameters.find(d => d.id === "flagCliente").value);
        // f.setFieldValue("incrementoCliente", query.parameters.find(d => d.id === "incrementoCliente").value);
    };
    const formOnBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            {
                id: "COLADETRABAJO",
                value: nuColaDeTrabajo
            },
        ];
    };


    return (
        <Page
            title={t("PRE810_Sec0_pageTitle_Titulo")}
            {...props}
        >

            {(showForm ? (<Form
                id="PRE810_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeValidateField={onBeforeValidateField}
                onBeforeInitialize={formOnBeforeInitialize}
                onAfterInitialize={onAfterInitialize}
                onBeforeButtonAction={onBeforeButtonAction}
                onBeforeSubmit={onBeforeSubmit}
            >
                <Row>
                    <Col lg={10}>
                        <FormGroup>
                            <label htmlFor="colaDeTrabajo">{t("PRE810_frm1_lbl_CD_COLADETRABAJO")}</label>
                            <FieldSelect id="colaDeTrabajo" name="colaDeTrabajo" />
                            <StatusMessage for="colaDeTrabajo" />
                        </FormGroup>
                    </Col>
                    <Col lg={2}>
                        <FormGroup>
                            <button className="btn btn-primary" onClick={openAddColaDeTrabDialog} style={{ position: "absolute", bottom: "0em", marginBottom: "16px" }} hidden>{t("PRE810_Sec0_btn_Agregar")}</button>
                        </FormGroup>
                        <FormGroup>
                            <label htmlFor="flOrdenCalendario">{t("PRE810_frm1_lbl_flOrdenCalendario")}</label>
                            <FieldToggle name="flOrdenCalendario" />
                        </FormGroup>
                    </Col>
                </Row>
                <br/>
                <div style={{ margin: "0px 20px 20px 20px" }}>
                    <Row>
                        <Col>
                            <FormGroup>
                                <label>{t("PRE810_frm1_lbl_CD_PONDERADOR")}</label>
                            </FormGroup>
                        </Col>
                        <Col>
                            <FormGroup>
                                <label>{t("PRE810_frm1_lbl_FL_HABILITADO")}</label>
                            </FormGroup>
                        </Col>
                        <Col>
                            <FormGroup>
                                <label>{t("PRE810_frm1_lbl_BT_EDIT")}</label>
                            </FormGroup>
                        </Col>
                        <Col lg={8}>
                            <FormGroup>
                                <label>{t("PRE810_frm1_lbl_VL_INCREMENTO")}</label>
                            </FormGroup>
                        </Col>
                    </Row>
                    <hr style={{ marginTop: "0em" }} />
                    <Row>
                        <Col>
                            <FormGroup>
                                <label>{t("PRE810_frm1_lbl_CD_EMPRESA")}</label>
                            </FormGroup>
                        </Col>
                        <Col>
                            <FormGroup>
                                <FieldCheckbox
                                    name="flagEmpresa"
                                    className="mb-2"
                                />
                                <StatusMessage for="flagEmpresa" />
                            </FormGroup>
                        </Col>
                        <Col >
                            <button className="btn btn-link" onClick={openEditarEmpresaDialog}><i className="fas fa-edit" /></button>
                        </Col>
                        <Col lg={8}>
                            <FormGroup>
                                <FieldRange name="incrementoEmpresa" marks={marks} min={-100} max={100} showInput />
                                <StatusMessage for="incrementoEmpresa" />
                            </FormGroup>
                        </Col>
                    </Row>
                    <hr style={{ marginTop: "0em" }} />
                    <Row>
                        <Col>
                            <FormGroup>
                                <label>{t("PRE810_frm1_lbl_CD_CLIENTE")}</label>
                            </FormGroup>
                        </Col>
                        <Col>
                            <FormGroup>
                                <FieldCheckbox
                                    name="flagCliente"
                                    className="mb-2"
                                />
                                <StatusMessage for="flagCliente" />
                            </FormGroup>
                        </Col>

                        <Col >
                            <button className="btn btn-link" onClick={openEditarClienteDialog}><i className="fas fa-edit" /></button>
                        </Col>
                        <Col lg={8}>
                            <FormGroup>
                                <FieldRange name="incrementoCliente" marks={marks} min={-100} max={100} showInput />
                                <StatusMessage for="incrementoCliente" />
                            </FormGroup>
                        </Col>
                    </Row>
                    <hr style={{ marginTop: "0em" }} />
                    <Row>
                        <Col>
                            <FormGroup>
                                <label>{t("PRE810_frm1_lbl_CD_ROTA")}</label>
                            </FormGroup>
                        </Col>
                        <Col>
                            <FormGroup>
                                <FieldCheckbox
                                    name="flagRuta"
                                    className="mb-2"
                                />
                                <StatusMessage for="flagRuta" />
                            </FormGroup>
                        </Col>
                        <Col >
                            <button className="btn btn-link" onClick={openEditarRutaDialog}><i className="fas fa-edit" /></button>
                        </Col>
                        <Col lg={8}>
                            <FormGroup>
                                <FieldRange name="incrementoRuta" marks={marks} min={-100} max={100} showInput />
                                <StatusMessage for="incrementoRuta" />
                            </FormGroup>
                        </Col>
                    </Row>
                    <hr style={{ marginTop: "0em" }} />
                    <Row>
                        <Col>
                            <FormGroup>
                                <label>{t("PRE810_frm1_lbl_CD_ZONA")}</label>
                            </FormGroup>
                        </Col>
                        <Col>
                            <FormGroup>
                                <FieldCheckbox
                                    name="flagZona"
                                    className="mb-2"
                                />
                                <StatusMessage for="flagZona" />
                            </FormGroup>
                        </Col>
                        <Col >
                            <button className="btn btn-link" onClick={openEditarZonaDialog}><i className="fas fa-edit" /></button>
                        </Col>
                        <Col lg={8}>
                            <FormGroup>
                                <FieldRange name="incrementoZona" marks={marks} min={-100} max={100} showInput />
                                <StatusMessage for="incrementoZona" />
                            </FormGroup>
                        </Col>
                    </Row>
                    <hr style={{ marginTop: "0em" }} />
                    <Row>
                        <Col>
                            <FormGroup>
                                <label>{t("PRE810_frm1_lbl_TP_PEDIDO")}</label>
                            </FormGroup>
                        </Col>
                        <Col>
                            <FormGroup>
                                <FieldCheckbox
                                    name="flagTipoPedido"
                                    className="mb-2"
                                />
                                <StatusMessage for="flagTipoPedido" />
                            </FormGroup>
                        </Col>
                        <Col >
                            <button className="btn btn-link" onClick={openEditarTpPedidosDialog}><i className="fas fa-edit" /></button>
                        </Col>
                        <Col lg={8}>
                            <FormGroup>
                                <FieldRange name="incrementoTipoPedido" marks={marks} min={-100} max={100} showInput />
                                <StatusMessage for="incrementoTipoPedido" />
                            </FormGroup>
                        </Col>
                    </Row>
                    <hr style={{ marginTop: "0em" }} />
                    <Row>
                        <Col>
                            <FormGroup>
                                <label>{t("PRE810_frm1_lbl_TP_EXPEDICION")}</label>
                            </FormGroup>
                        </Col>
                        <Col>
                            <FormGroup>
                                <FieldCheckbox
                                    name="flagTipoExpedicion"
                                    className="mb-2"
                                />
                                <StatusMessage for="flagTipoExpedicion" />
                            </FormGroup>
                        </Col>
                        <Col >
                            <button className="btn btn-link" onClick={openEditarTpExpedicionDialog}><i className="fas fa-edit" /></button>
                        </Col>
                        <Col lg={8}>
                            <FormGroup>
                                <FieldRange name="incrementoTipoExpedicion" marks={marks} min={-100} max={100} showInput />
                                <StatusMessage for="incrementoTipoExpedicion" />
                            </FormGroup>
                        </Col>
                    </Row>
                    <hr style={{ marginTop: "0em" }} />
                    <Row>
                        <Col>
                            <FormGroup>
                                <label>{t("PRE810_frm1_lbl_CON_LIB")}</label>
                            </FormGroup>
                        </Col>
                        <Col>
                            <FormGroup>
                                <FieldCheckbox
                                    name="flagConficionLiberacion"
                                    className="mb-2"
                                />
                                <StatusMessage for="flagConficionLiberacion" />
                            </FormGroup>
                        </Col>
                        <Col >
                            <button className="btn btn-link" onClick={openEditarCondLiberacionDialog}><i className="fas fa-edit" /></button>
                        </Col>
                        <Col lg={8}>
                            <FormGroup>
                                <FieldRange name="incrementoCondicionLiberacion" marks={marks} min={-100} max={100} showInput />
                                <StatusMessage for="incrementoCondicionLiberacion" />
                            </FormGroup>
                        </Col>
                    </Row>
                    <hr style={{ marginTop: "0em" }} />
                    <Row>
                        <Col>
                            <FormGroup>
                                <label>{t("PRE810_frm1_lbl_DT_ENTREGA")}</label>
                            </FormGroup>
                        </Col>
                        <Col>
                            <FormGroup>
                                <FieldCheckbox
                                    name="flagFechaEntrega"
                                    className="mb-2"
                                />
                                <StatusMessage for="flagFechaEntrega" />
                            </FormGroup>
                        </Col>
                        <Col >
                            <button className="btn btn-link" onClick={openEditarFechaEntregaDialog}><i className="fas fa-edit" /></button>
                        </Col>
                        <Col lg={8}>
                            <FormGroup>
                                <FieldRange name="incrementoFechaEntrega" marks={marks} min={-100} max={100} showInput />
                                <StatusMessage for="incrementoFechaEntrega" />
                            </FormGroup>
                        </Col>
                    </Row>
                    <hr style={{ marginTop: "0em" }} />
                    <Row>
                        <Col>
                            <FormGroup>
                                <label>{t("PRE810_frm1_lbl_DT_EMITIDO")}</label>
                            </FormGroup>
                        </Col>
                        <Col>
                            <FormGroup>
                                <FieldCheckbox
                                    name="flagFechaEmitido"
                                    className="mb-2"
                                />
                                <StatusMessage for="flagFechaEmitido" />
                            </FormGroup>
                        </Col>
                        <Col >
                            <button className="btn btn-link" onClick={openEditarFechaEmitidoDialog}><i className="fas fa-edit" /></button>
                        </Col>
                        <Col lg={8}>
                            <FormGroup>
                                <FieldRange name="incrementoFechaEmitido" marks={marks} min={-100} max={100} showInput />
                                <StatusMessage for="incrementoFechaEmitido" />
                            </FormGroup>
                        </Col>
                    </Row>
                    <hr style={{ marginTop: "0em" }} />
                    <Row>
                        <Col>
                            <FormGroup>
                                <label>{t("PRE810_frm1_lbl_DT_LIBERADO")}</label>
                            </FormGroup>
                        </Col>
                        <Col>
                            <FormGroup>
                                <FieldCheckbox
                                    name="flagFechaLiberado"
                                    className="mb-2"
                                />
                                <StatusMessage for="flagFechaLiberado" />
                            </FormGroup>
                        </Col>
                        <Col >
                            <button className="btn btn-link" onClick={openEditarFechaLiberadoDialog}><i className="fas fa-edit" /></button>
                        </Col>
                        <Col lg={8}>
                            <FormGroup>
                                <FieldRange name="incrementoFechaLiberado" marks={marks} min={-100} max={100} showInput />
                                <StatusMessage for="incrementoFechaLiberado" />
                            </FormGroup>
                        </Col>
                    </Row>
                    <hr style={{ marginTop: "0em" }} />
                    <Row>
                        <Col>
                            <FormGroup>
                                <label>{t("PRE810_frm1_lbl_VL_PONDERACION_GENERICA")}</label>
                            </FormGroup>
                        </Col>
                        <Col>
                            <FormGroup>
                                <FieldCheckbox
                                    name="flagPonderacionGenerica"
                                    className="mb-2"
                                />
                                <StatusMessage for="flagPonderacionGenerica" />
                            </FormGroup>
                        </Col>
                        <Col >
                            <button className="btn btn-link" onClick={openEditarPonderacionGenericaDialog}><i className="fas fa-edit" /></button>
                        </Col>
                        <Col lg={8}>
                            <FormGroup>
                                <FieldRange name="incrementoPonderacionGenerica" marks={marks} min={-100} max={100} showInput />
                                <StatusMessage for="incrementoPonderacionGenerica" />
                            </FormGroup>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <FormGroup style={{ textAlign: "center" }}>
                                <SubmitButton className="btn btn-primary" id="btnGuardar" variant="primary" label="PRE810_frm1_btn_Guardar" />
                            </FormGroup>
                        </Col>

                    </Row>
                </div>
            </Form>) : null)
            }



            <PRE810AgregarColaTrabajo show={showPopupAddColaTrab} onHide={closeAddColaDeTrabDialog} />
            <PRE810EditarPonderadorEmpresas show={showPopupPonderadorEmpresas} onHide={closePonderadorEmpresasDialog} nuColaDeTrabajo={nuColaDeTrabajo} />
            <PRE810EditarPonderadorClientes show={showPopupPonderadorClientes} onHide={closePonderadorClientesDialog} nuColaDeTrabajo={nuColaDeTrabajo} />
            <PRE810EditarPonderadorRutas show={showPopupPonderadorRutas} onHide={closePonderadorRutasDialog} nuColaDeTrabajo={nuColaDeTrabajo} />
            <PRE810EditarPonderadorZonas show={showPopupPonderadorZonas} onHide={closePonderadorZonasDialog} nuColaDeTrabajo={nuColaDeTrabajo} />
            <PRE810EditarPonderadorTipoPedidos show={showPopupPonderadorTpPedidos} onHide={closePonderadorTpPedidosDialog} nuColaDeTrabajo={nuColaDeTrabajo} />
            <PRE810EditarPonderadorTipoExpedicion show={showPopupPonderadorTpExpedicion} onHide={closePonderadorTpExpedicionDialog} nuColaDeTrabajo={nuColaDeTrabajo} />
            <PRE810EditarPonderadorCondLiberacion show={showPopupPonderadorCondLiberacion} onHide={closePonderadorCondLiberacionDialog} nuColaDeTrabajo={nuColaDeTrabajo} />
            <PRE810EditarPonderadorFechaEntrega show={showPopupPonderadorFechaEntrega} onHide={closePonderadorFechaEntregaDialog} nuColaDeTrabajo={nuColaDeTrabajo} />
            <PRE810EditarPonderadorFechaEmitido show={showPopupPonderadorFechaEmitido} onHide={closePonderadorFechaEmitidoDialog} nuColaDeTrabajo={nuColaDeTrabajo} />
            <PRE810EditarPonderadorFechaLiberado show={showPopupPonderadorFechaLiberado} onHide={closePonderadorFechaLiberadoDialog} nuColaDeTrabajo={nuColaDeTrabajo} />
            <PRE810EditarPonderadorPonderacionGenerica show={showPopupPonderadorPonderacionGenerica} onHide={closePonderadorPonderacionGenericaDialog} nuColaDeTrabajo={nuColaDeTrabajo} />

        </Page>
    );
}