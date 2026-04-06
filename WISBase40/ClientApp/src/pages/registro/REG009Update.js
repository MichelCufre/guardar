import React, { useRef, useState } from 'react';
import { Button, Col, Container, Modal, Row, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldSelect, FieldSelectAsync, FieldTextArea, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';

export default function REG009Update(props) {

    const { t } = useTranslation();

    const [keyTab, setKeyTab] = useState("infoPrincipal");

    const [modeEdit, setModeEdit] = useState(false);

    const [infoProducto, setInfoProducto] = useState("");

    const confirmClaseRef = useRef(false);
    const confirmManejoIdRef = useRef(false);

    const validationSchema = {
        empresa: Yup.string().required(),
        codigo: Yup.string().required(),
        descripcion: Yup.string().required(),
        clase: Yup.string().required(),
        familia: Yup.string().required(),
        manejoIdentificador: Yup.string().required(),
        manejoFecha: Yup.string().required(),
        modalidadIngresoLote: Yup.string().required(),
        unidadBultos: Yup.string().required(),
        unidadDistribucion: Yup.string().required(),
        aceptaDecimales: Yup.string(),
        unidadMedida: Yup.string(),
        rotatividad: Yup.string(),
        situacion: Yup.string(),
        manejoDiasLiberacion: Yup.string(),
        diasLiberacion: Yup.string(),
        diasDuracion: Yup.string(),
        diasValidez: Yup.string(),
        stockMinimo: Yup.string(),
        stockMaximo: Yup.string(),
        pesoBruto: Yup.string(),
        pesoNeto: Yup.string(),
        altura: Yup.string(),
        ancho: Yup.string(),
        profundidad: Yup.string(),
        volumenCC: Yup.string(),
        mercadologico: Yup.string(),
        reducida: Yup.string(),
        productoEmpresaRef: Yup.string(),
        NCM: Yup.string(),
        ajusteInventario: Yup.string(),
        display: Yup.string(),
        subBulto: Yup.string(),
        ultimoCosto: Yup.string(),
        precioVenta: Yup.string(),
        exclusivo: Yup.string(),
        ayudaColector: Yup.string(),
        componente1: Yup.string(),
        componente2: Yup.string(),
        anexo1: Yup.string(),
        anexo2: Yup.string(),
        anexo3: Yup.string(),
        anexo4: Yup.string(),
        anexo5: Yup.string(),
        grupoConsulta: Yup.string(),
        ramo: Yup.string(),
        codigoBase: Yup.string(),
        talle: Yup.string(),
        color: Yup.string(),
        temporada: Yup.string(),
        categoria1: Yup.string(),
        categoria2: Yup.string(),
        ventanaLiberacion: Yup.string(),
    };

    const handleClose = () => {
        props.onHide(props.nexxus);

        confirmManejoIdRef.current = false;
        confirmClaseRef.current = false;
    };

    const handleOnAfterInitialize = (context, form, query, nexus) => {
        if (query.parameters.find(x => x.id === "REG009_PRODUCTO_EDIT")) {

            setInfoProducto(query.parameters.find(x => x.id === "REG009_PRODUCTO_EDIT").value);
        }

        confirmManejoIdRef.current = false;
        confirmClaseRef.current = false;
    };

    const handleFormAfterValidateField = (context, form, query) => {
        if (query.fieldId === "clase") {
            confirmClaseRef.current = !!query.parameters.find(
                p => p.id === "SHOW_CONFIRMACION_CAMBIO_CLASE"
            );
        }

        if (query.fieldId === "manejoIdentificador") {
            confirmManejoIdRef.current = !!query.parameters.find(
                p => p.id === "SHOW_CONFIRMACION_CAMBIO_MANEJO_ID"
            );
        }
    };

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        const confirmClase = confirmClaseRef.current;
        const confirmManejoId = confirmManejoIdRef.current;

        if (!confirmClase && !confirmManejoId)
            return;

        context.abortServerCall = true;

        if (confirmClase) {
            showConfirmacionClase(
                context,
                nexus,
                confirmManejoId
            );
        } else {
            showConfirmacionManejoId(context, nexus);
        }
    }

    const showConfirmacionClase = (context, nexus, confirmManejoId) => {
        nexus.showConfirmation({
            message: "REG009_Sec0_Warn_StockEnUbicaciones",
            onAccept: () => {
                confirmClaseRef.current = false;

                if (confirmManejoId) {
                    setTimeout(() => {
                        showConfirmacionManejoId(context, nexus);
                    }, 0);
                } else {
                    confirmManejoIdRef.current = false;

                    setTimeout(() => {
                        nexus.getForm("REG009_form_Update").submit();
                    }, 0);
                }
            },
            onCancel: () => {
                context.abortServerCall = true;
            }
        });
    };

    const showConfirmacionManejoId = (context, nexus) => {
        nexus.showConfirmation({
            message: "REG009_Sec0_Warn_ManejoIdentificadorProducto",
            onAccept: () => {
                confirmManejoIdRef.current = false;
                confirmClaseRef.current = false;

                setTimeout(() => {
                    nexus.getForm("REG009_form_Update").submit();
                }, 0);
            },
            onCancel: () => {
                context.abortServerCall = true;
            }
        });
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            props.onHide(props.nexxus);
        }

        confirmManejoIdRef.current = false;
        confirmClaseRef.current = false;
    };

    const addParameters = (context, data, nexus) => {

        if (props.producto && props.producto.Empresa != null) {

            data.parameters = [
                { id: "idProducto", value: props.producto.Producto },
                { id: "idEmpresa", value: props.producto.Empresa }
            ];
        }
    }

    const handleFormBeforeInitialize = (context, form, query, nexus) => {

        if (props.producto && props.producto.Empresa != null) {

            query.parameters = [
                { id: "idProducto", value: props.producto.Producto },
                { id: "idEmpresa", value: props.producto.Empresa }
            ];
        }
    }


    return (
        <Page
            {...props}
            application="REG009Update"
        >
            <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
                <Form
                    application="REG009Update"
                    id="REG009_form_Update"
                    validationSchema={validationSchema}
                    onAfterValidateField={handleFormAfterValidateField}
                    onBeforeSubmit={handleFormBeforeSubmit}
                    onAfterSubmit={handleFormAfterSubmit}
                    onBeforeInitialize={handleFormBeforeInitialize}
                    onAfterInitialize={handleOnAfterInitialize}
                >
                    <Modal.Header closeButton>
                        <Modal.Title>{t("REG009_Sec0_modalTitle_EditarProducto")}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Container fluid>
                            <Tabs defaultActiveKey="infoPrincipal" transition={false} id="noanim-tab-example"
                                activeKey={keyTab}
                                onSelect={(k) => setKeyTab(k)}
                            >

                                <Tab eventKey="infoPrincipal" title={t("REG009_frm1_tab_infoPrin")}>
                                    <br></br>
                                    <Row>
                                        {/*Columna 1*/}
                                        <Col>
                                            <h5 className='form-title'>{t("REG009_frm_subT_General")}</h5>
                                            <Row>
                                                <Col>
                                                    <div className="form-group">
                                                        <label htmlFor="empresa">{t("REG009_frm_lbl_empresa")}</label>
                                                        <FieldSelectAsync name="empresa" />
                                                        <StatusMessage for="empresa" />

                                                    </div>
                                                </Col>
                                                <Col>
                                                    <div className="form-group">
                                                        <label htmlFor="codigo">{t("REG009_frm_lbl_codigo")}</label>
                                                        <Field name="codigo" />
                                                        <StatusMessage for="codigo" />
                                                    </div>
                                                </Col>
                                            </Row>
                                            <Row>
                                                <Col>
                                                    <div className="form-group">
                                                        <label htmlFor="descripcion">{t("REG009_frm_lbl_descripcion")}</label>
                                                        <Field name="descripcion" />
                                                        <StatusMessage for="descripcion" />
                                                    </div>
                                                </Col>
                                            </Row>
                                            <Row>
                                                <Col>
                                                    <Row>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="unidadMedida">{t("REG009_frm_lbl_unidadMedida")}</label>
                                                                <FieldSelectAsync name="unidadMedida" />
                                                                <StatusMessage for="unidadMedida" />
                                                            </div>
                                                        </Col>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="clase">{t("REG009_frm_lbl_clase")}</label>
                                                                <FieldSelect name="clase" />
                                                                <StatusMessage for="clase" />
                                                            </div>
                                                        </Col>
                                                    </Row>
                                                    <Row>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="familia">{t("REG009_frm_lbl_familia")}</label>
                                                                <FieldSelectAsync name="familia" />
                                                                <StatusMessage for="familia" />
                                                            </div>
                                                        </Col>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="ramo">{t("REG009_frm_lbl_ramo")}</label>
                                                                <FieldSelect name="ramo" />
                                                                <StatusMessage for="ramo" />
                                                            </div>
                                                        </Col>
                                                    </Row>
                                                    <Row>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="rotatividad">{t("REG009_frm_lbl_rotatividad")}</label>
                                                                <FieldSelect name="rotatividad" />
                                                                <StatusMessage for="rotatividad" />
                                                            </div>
                                                        </Col>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="situacion">{t("REG009_frm_lbl_situacion")}</label>
                                                                <FieldSelect name="situacion" />
                                                                <StatusMessage for="situacion" />
                                                            </div>
                                                        </Col>
                                                    </Row>
                                                    <Row>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="manejoIdentificador">{t("REG009_frm_lbl_manejoIdentificador")}</label>
                                                                <FieldSelect name="manejoIdentificador" />
                                                                <StatusMessage for="manejoIdentificador" />
                                                            </div>
                                                        </Col>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="modalidadIngresoLote">{t("REG009_frm_lbl_modalidadIngresoLote")}</label>
                                                                <FieldSelect name="modalidadIngresoLote" />
                                                                <StatusMessage for="modalidadIngresoLote" />
                                                            </div>
                                                        </Col>
                                                    </Row>
                                                </Col>
                                            </Row>
                                        </Col>

                                        {/*Columna 2*/}
                                        <Col>
                                            <h5 className='form-title'>{t("REG009_frm_subT_ManejoFecha")}</h5>
                                            <Row>
                                                <Col>
                                                    <div className="form-group">
                                                        <label htmlFor="manejoFecha">{t("REG009_frm_lbl_manejoFecha")}</label>
                                                        <FieldSelect name="manejoFecha" />
                                                        <StatusMessage for="manejoFecha" />
                                                    </div>
                                                    <div className="form-group">
                                                        <label htmlFor="manejoDiasLiberacion">{t("REG009_frm_lbl_manejoDiasLiberacion")}</label>
                                                        <FieldSelect name="manejoDiasLiberacion" />
                                                        <StatusMessage for="manejoDiasLiberacion" />
                                                    </div>
                                                    <Row>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="diasLiberacion">{t("REG009_frm_lbl_diasLiberacion")}</label>
                                                                <Field name="diasLiberacion" />
                                                                <StatusMessage for="diasLiberacion" />
                                                            </div>
                                                        </Col>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="diasDuracion">{t("REG009_frm_lbl_diasDuracion")}</label>
                                                                <Field name="diasDuracion" />
                                                                <StatusMessage for="diasDuracion" />
                                                            </div>
                                                        </Col>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="diasValidez">{t("REG009_frm_lbl_diasValidez")}</label>
                                                                <Field name="diasValidez" />
                                                                <StatusMessage for="diasValidez" />
                                                            </div>
                                                        </Col>
                                                    </Row>
                                                    <Row>
                                                        <div className="form-group">
                                                            <label htmlFor="ventanaLiberacion">{t("REG009_frm_lbl_ventanaLiberacion")}</label>
                                                            <FieldSelect name="ventanaLiberacion" />
                                                            <StatusMessage for="ventanaLiberacion" />
                                                        </div>
                                                    </Row>
                                                    <h5 className='form-title'>{t("REG009_frm_subT_Stock")}</h5>
                                                    <div className="form-group">
                                                        <label htmlFor="aceptaDecimales">{t("REG009_frm_lbl_aceptaDecimales")}</label>
                                                        <FieldSelect name="aceptaDecimales" />
                                                        <StatusMessage for="aceptaDecimales" />
                                                    </div>
                                                    <Row>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="stockMinimo">{t("REG009_frm_lbl_stockMinimo")}</label>
                                                                <Field name="stockMinimo" />
                                                                <StatusMessage for="stockMinimo" />
                                                            </div>
                                                        </Col>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="stockMaximo">{t("REG009_frm_lbl_stockMaximo")}</label>
                                                                <Field name="stockMaximo" />
                                                                <StatusMessage for="stockMaximo" />
                                                            </div>
                                                        </Col>
                                                    </Row>
                                                </Col>
                                            </Row>
                                        </Col>
                                        {/*Columna 3*/}
                                        <Col>
                                            <h5 className='form-title'>{t("REG009_frm_subT_Dimensiones")}</h5>
                                            <Row>
                                                <Col>
                                                    <Row>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="pesoNeto">{t("REG009_frm_lbl_pesoNeto")}</label>
                                                                <Field name="pesoNeto" />
                                                                <StatusMessage for="pesoNeto" />
                                                            </div>
                                                        </Col>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="pesoBruto">{t("REG009_frm_lbl_pesoBruto")}</label>
                                                                <Field name="pesoBruto" />
                                                                <StatusMessage for="pesoBruto" />
                                                            </div>
                                                        </Col>
                                                    </Row>
                                                    <Row>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="altura">{t("REG009_frm_lbl_altura")}</label>
                                                                <Field name="altura" />
                                                                <StatusMessage for="altura" />
                                                            </div>
                                                        </Col>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="ancho">{t("REG009_frm_lbl_ancho")}</label>
                                                                <Field name="ancho" />
                                                                <StatusMessage for="ancho" />
                                                            </div>
                                                        </Col>
                                                    </Row>
                                                    <Row>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="profundidad">{t("REG009_frm_lbl_profundidad")}</label>
                                                                <Field name="profundidad" />
                                                                <StatusMessage for="profundidad" />
                                                            </div>
                                                        </Col>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="volumenCC">{t("REG009_frm_lbl_volumenCC")}</label>
                                                                <Field name="volumenCC" />
                                                                <StatusMessage for="volumenCC" />
                                                            </div>
                                                        </Col>
                                                    </Row>
                                                    <Row>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="unidadBultos">{t("REG009_frm_lbl_unidadBultos")}</label>
                                                                <Field name="unidadBultos" />
                                                                <StatusMessage for="unidadBultos" />
                                                            </div>
                                                        </Col>
                                                        <Col>
                                                            <div className="form-group">
                                                                <label htmlFor="unidadDistribucion">{t("REG009_frm_lbl_unidadDistribucion")}</label>
                                                                <Field name="unidadDistribucion" />
                                                                <StatusMessage for="unidadDistribucion" />
                                                            </div>
                                                        </Col>
                                                    </Row>
                                                </Col>
                                            </Row>
                                            <h5 className='form-title'>{t("REG009_frm_subT_GrupoConsulta")}</h5>
                                            <div className="form-group">
                                                <label htmlFor="grupoConsulta">{t("REG009_frm_lbl_grupoConsulta")}</label>
                                                <FieldSelect name="grupoConsulta" />
                                                <StatusMessage for="grupoConsulta" />
                                            </div>
                                        </Col>
                                    </Row>
                                </Tab>
                                <Tab eventKey="infoSecundaria" title={t("REG009_frm1_tab_infoSecu")}>
                                    <br></br>
                                    <Row>
                                        <Col>
                                            <div className="form-group">
                                                <label htmlFor="mercadologico">{t("REG009_frm_lbl_mercadologico")}</label>
                                                <Field name="mercadologico" />
                                                <StatusMessage for="mercadologico" />
                                            </div>
                                            <div className="form-group">
                                                <label htmlFor="reducida">{t("REG009_frm_lbl_reducida")}</label>
                                                <Field name="reducida" />
                                                <StatusMessage for="reducida" />
                                            </div>
                                            <div className="form-group">
                                                <label htmlFor="productoEmpresaRef">{t("REG009_frm_lbl_ReferenciaProdEmpresa")}</label>
                                                <Field name="productoEmpresaRef" />
                                                <StatusMessage for="productoEmpresaRef" />
                                            </div>

                                            <div className="form-group">
                                                <label htmlFor="NCM">{t("REG009_frm_lbl_NCM")}</label>
                                                <FieldSelectAsync name="NCM" />
                                                <StatusMessage for="NCM" />
                                            </div>
                                            <div className="form-group">
                                                <label htmlFor="ajusteInventario">{t("REG009_frm_lbl_ajusteInventario")}</label>
                                                <Field name="ajusteInventario" />
                                                <StatusMessage for="ajusteInventario" />
                                            </div>
                                            <Row>
                                                <Col>
                                                    <div className="form-group">
                                                        <label htmlFor="codigoBase">{t("REG009_frm_lbl_codigoBase")}</label>
                                                        <Field name="codigoBase" />
                                                        <StatusMessage for="codigoBase" />
                                                    </div>
                                                </Col>
                                                <Col>
                                                    <div className="form-group">
                                                        <label htmlFor="talle">{t("REG009_frm_lbl_talle")}</label>
                                                        <Field name="talle" />
                                                        <StatusMessage for="talle" />
                                                    </div>
                                                </Col>
                                            </Row>
                                        </Col>
                                        <Col>
                                            <Row>
                                                <Col>
                                                    <div className="form-group">
                                                        <label htmlFor="display">{t("REG009_frm_lbl_display")}</label>
                                                        <FieldSelect name="display" />
                                                        <StatusMessage for="display" />
                                                    </div>
                                                </Col>
                                                <Col>
                                                    <div className="form-group">
                                                        <label htmlFor="ayudaColector">{t("REG009_frm_lbl_ayudaColector")}</label>
                                                        <Field name="ayudaColector" />
                                                        <StatusMessage for="ayudaColector" />
                                                    </div>
                                                </Col>
                                            </Row>
                                            <div className="form-group">
                                                <label htmlFor="subBulto">{t("REG009_frm_lbl_subBulto")}</label>
                                                <Field name="subBulto" />
                                                <StatusMessage for="subBulto" />
                                            </div>
                                            <div className="form-group">
                                                <label htmlFor="ultimoCosto">{t("REG009_frm_lbl_ultimoCosto")}</label>
                                                <Field name="ultimoCosto" />
                                                <StatusMessage for="ultimoCosto" />
                                            </div>
                                            <div className="form-group">
                                                <label htmlFor="precioVenta">{t("REG009_frm_lbl_precioVenta")}</label>
                                                <Field name="precioVenta" />
                                                <StatusMessage for="precioVenta" />
                                            </div>
                                            <div className="form-group">
                                                <label htmlFor="exclusivo">{t("REG009_frm_lbl_exclusivo")}</label>
                                                <Field name="exclusivo" />
                                                <StatusMessage for="exclusivo" />
                                            </div>
                                            <Row>
                                                <Col>
                                                    <div className="form-group">
                                                        <label htmlFor="color">{t("REG009_frm_lbl_color")}</label>
                                                        <Field name="color" />
                                                        <StatusMessage for="color" />
                                                    </div>
                                                </Col>
                                                <Col>
                                                    <div className="form-group">
                                                        <label htmlFor="temporada">{t("REG009_frm_lbl_temporada")}</label>
                                                        <Field name="temporada" />
                                                        <StatusMessage for="temporada" />
                                                    </div>
                                                </Col>
                                            </Row>
                                        </Col>
                                    </Row>
                                    <Row>
                                        <Col>
                                            <div className="form-group">
                                                <label htmlFor="categoria1">{t("REG009_frm_lbl_categoria1")}</label>
                                                <Field name="categoria1" />
                                                <StatusMessage for="categoria1" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group">
                                                <label htmlFor="categoria2">{t("REG009_frm_lbl_categoria2")}</label>
                                                <Field name="categoria2" />
                                                <StatusMessage for="categoria2" />
                                            </div>
                                        </Col>
                                    </Row>
                                </Tab>
                                <Tab eventKey="anexos" title={t("REG009_frm1_tab_anexos")}>
                                    <br></br>
                                    <h5 className='form-title'>{t("REG009_frm_subT_Facturacion")}</h5>
                                    <Row>
                                        <Col>
                                            <div className="form-group">
                                                <label htmlFor="componente1">{t("REG009_frm_lbl_componente1")}</label>
                                                <FieldSelect name="componente1" />
                                                <StatusMessage for="componente1" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group">
                                                <label htmlFor="componente2">{t("REG009_frm_lbl_componente2")}</label>
                                                <FieldSelect name="componente2" />
                                                <StatusMessage for="componente2" />
                                            </div>
                                        </Col>
                                    </Row>
                                    <hr></hr>
                                    <Row>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="anexo1">{t("REG009_frm_lbl_anexo1")}</label>
                                                <FieldTextArea name="anexo1" maxLength="200" />
                                                <StatusMessage for="anexo1" />
                                            </div>
                                            <div className="form-group" >
                                                <label htmlFor="anexo2">{t("REG009_frm_lbl_anexo2")}</label>
                                                <FieldTextArea name="anexo2" maxLength="200" />
                                                <StatusMessage for="anexo2" />
                                            </div>
                                            <div className="form-group" >
                                                <label htmlFor="anexo5">{t("REG009_frm_lbl_anexo5")}</label>
                                                <FieldTextArea name="anexo5" maxLength="200" />
                                                <StatusMessage for="anexo5" />
                                            </div>
                                        </Col>
                                        <Col>
                                            <div className="form-group" >
                                                <label htmlFor="anexo3">{t("REG009_frm_lbl_anexo3")}</label>
                                                <FieldTextArea name="anexo3" maxLength="200" />
                                                <StatusMessage for="anexo3" />
                                            </div>
                                            <div className="form-group" >
                                                <label htmlFor="anexo4">{t("REG009_frm_lbl_anexo4")}</label>
                                                <FieldTextArea name="anexo4" maxLength="200" />
                                                <StatusMessage for="anexo4" />
                                            </div>
                                        </Col>
                                    </Row>
                                </Tab>

                            </Tabs>
                        </Container>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="btn btn-outline-secondary" onClick={handleClose}>{t("REG009_frm_btn_Cancelar")}</Button>
                        <SubmitButton id="btnSubmit" variant="primary" label="REG009_frm_btn_Crear" />
                    </Modal.Footer>

                </Form >

            </Modal >
        </Page>
    );
}