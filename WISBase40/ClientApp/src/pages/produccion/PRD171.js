import React, { useState, useRef, useEffect } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldNumber, StatusMessage, SubmitButton, FormButton } from '../../components/FormComponents/Form';
import { Row, Col, FormGroup, Card, ProgressBar, Modal, Button } from 'react-bootstrap';
import { FormTab, FormTabStep } from '../../components/FormComponents/FormTab';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';

export default function PRD171(props) {
    const { t } = useTranslation();

    const [ingreso, setIngreso] = useState({});
    const [linea, setLinea] = useState({});
    const [formula, setFormula] = useState({});
    const [pasada, setPasada] = useState({});
    const [modalDisplay, setModalDisplay] = useState(false);

    const initialValues = {
        valor: "",
        cantidadPasadas: ""
    };

    const validationSchema = {
        //cantidadPasadas: Yup.number().nullable(true).positive().typeError("Debe ingresar un numero entero positivo")
    };

    const handleBeforePageLoad = (data) => {
        data.parameters = [{ id: "nroIngreso", value: localStorage.getItem("PRD171_nroIngreso") }];
    }
    const handleAfterPageLoad = (data) => {
        const paramIngreso = data.parameters.find(d => d.id === "ingreso");
        const paramFormula = data.parameters.find(d => d.id === "formula");
        const paramLinea = data.parameters.find(d => d.id === "linea");

        if (paramIngreso && paramIngreso.value)
            setIngreso(JSON.parse(paramIngreso.value));

        if (paramFormula && paramFormula.value)
            setFormula(JSON.parse(paramFormula.value));

        if (paramLinea && paramLinea.value)
            setLinea(JSON.parse(paramLinea.value));
    }

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [{ id: "nroIngreso", value: localStorage.getItem("PRD171_nroIngreso") }];
    }
    const handleFormAfterInitialize = (context, form, query, nexus) => {
        const paramFinalizado = query.parameters.find(d => d.id === "finalizado");
        const paramPasada = query.parameters.find(d => d.id === "pasada");

        if (paramPasada && paramPasada.value) {
            const pasadaActual = JSON.parse(paramPasada.value);

            if (paramFinalizado) {
                const pasadaFinalizada = { ...pasadaActual };

                pasadaFinalizada.NumeroFormula = ingreso.CantidadIteracionesFormula;
                pasadaFinalizada.NumeroPasada = formula.CantidadPasadasPorFormula;
                pasadaFinalizada.Orden = pasadaActual.MaxOrden;
                pasadaFinalizada.Descripcion = "";

                setPasada(pasadaFinalizada);
            }
            else {            
                const pasadaActualizar = { ...pasadaActual };

                pasadaActualizar.NumeroFormula = pasadaActualizar.NumeroFormula - 1;
                pasadaActualizar.NumeroPasada = pasadaActualizar.NumeroPasada - 1;
                pasadaActualizar.Orden = pasadaActualizar.Orden - 1;

                setPasada(pasadaActualizar);
            }
        }
    }    
    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [{ id: "nroIngreso", value: localStorage.getItem("PRD171_nroIngreso") }];
    }
    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK")
            setModalDisplay(false);
    }
    const handleFormBeforeButtonAction = (context, form, query, nexus) => {
        context.abortServerCall = true;

        setModalDisplay(true);
    }

    const handleClose = () => {
        setModalDisplay(false);
    }

    const formulaProgressValue = pasada && formula ? Math.floor((pasada.NumeroFormula * 100) / (ingreso.CantidadIteracionesFormula || 1)) : 0;
    const pasadaProgressValue = pasada && formula ? Math.floor((pasada.NumeroPasada * 100) / (formula.CantidadPasadasPorFormula || 1)) : 0;
    const accionProgressValue = pasada ? Math.floor((pasada.Orden * 100) / (pasada.MaxOrden || 1)) : 0;

    const actionStyle = {
        visibility: pasada.MaxOrden && pasada.MaxOrden > 0 ? "visible" : "hidden"
    };

    return (
        <Page
            title={t("PRD171_Sec0_pageTitle_Titulo")}
            onBeforeLoad={handleBeforePageLoad}
            onAfterLoad={handleAfterPageLoad}
            load
            {...props}
        >
            <Row>
                <Col>
                    <Card bg="light">                        
                        <Form
                            id="PRD171_form_1"
                            initialValues={initialValues}
                            validationSchema={validationSchema}
                            onBeforeInitialize={handleFormBeforeInitialize}
                            onAfterInitialize={handleFormAfterInitialize}
                            onBeforeSubmit={handleFormBeforeSubmit}
                            onAfterSubmit={handleFormAfterSubmit}
                            onBeforeButtonAction={handleFormBeforeButtonAction}
                        >
                            <Card.Header>Progreso</Card.Header>
                            <Card.Body>
                                <div>{t("PRD171_form1_label_Formula")} {pasada.NumeroFormula}/{ingreso.CantidadIteracionesFormula}</div>
                                <ProgressBar now={formulaProgressValue} className="mb-3" />
                                <div>{t("PRD171_form1_label_Ciclo")} {pasada.NumeroPasada}/{formula.CantidadPasadasPorFormula}</div>
                                <ProgressBar now={pasadaProgressValue} className="mb-3" />
                                <div style={actionStyle}>
                                    <div>{t("PRD171_form1_label_Accion")} {pasada.Orden}/{pasada.MaxOrden}</div>
                                    <ProgressBar now={accionProgressValue} className="mb-3" />
                                </div>
                            </Card.Body>
                            <Card.Body className="border-top">
                                <Row>
                                    <Col>
                                        <Card.Title>{t(pasada.Descripcion)}</Card.Title>
                                        <div className="form-group">
                                            <label htmlFor="valor">{t("PRD171_form1_label_Valor")}</label>
                                            <Field name="valor" />
                                            <StatusMessage for="valor" />
                                        </div>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col>
                                        <SubmitButton id="btnAvanzarPasada" className="btn btn-primary mr-3" label={t("PRD171_form1_btn_AvanzarPasada")}></SubmitButton>
                                        <FormButton id="btnAbrirModal" variant="primary" label={t("PRD171_form1_btn_AvanzarMultiplesPasadas")}></FormButton>
                                    </Col>
                                </Row>
                            </Card.Body>
                            <Modal show={modalDisplay} onHide={handleClose}>
                                <Modal.Header closeButton>
                                    <Modal.Title>{t("PRD171_form1_title_CantidadCiclos")}</Modal.Title>
                                </Modal.Header>
                                <Modal.Body>
                                    <div className="form-group">
                                        <label htmlFor="cantidadPasadas">{t("PRD171_form1_label_CantidadCiclosValor")}</label>
                                        <FieldNumber name="cantidadPasadas" />
                                        <StatusMessage for="cantidadPasadas" />
                                    </div>
                                </Modal.Body>
                                <Modal.Footer>
                                    <Button variant="secondary" onClick={handleClose}>
                                        {t("General_Sec0_btn_Cancelar")}
                                    </Button>
                                    <SubmitButton id="btnAvanzarMultiplesPasadas" className="btn btn-primary" label={t("General_Sec0_btn_Confirmar")}></SubmitButton>
                                </Modal.Footer>
                            </Modal>
                        </Form>                        
                    </Card>
                </Col>
                <Col>
                    <Row>
                        <Col>
                            <h1 className="mb-3">{ingreso.NU_PRDC_INGRESO}</h1>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <h4 className="form-title">{t("PRD175_frm1_lbl_legend1")}</h4>
                            <Row>
                                <Col>
                                    <Row className="mb-2">
                                        <Col>
                                            <span className="text-muted">{t("PRD175_frm1_lbl_CD_LINEA")} </span>
                                            <span>{linea.Id}</span>
                                        </Col>
                                    </Row>
                                    <Row className="mb-2">
                                        <Col>
                                            <span className="text-muted">{t("PRD175_frm1_lbl_CD_ENDERECO_ENTRADA")} </span>
                                            <span>{linea.UbicacionEntrada}</span>
                                        </Col>
                                    </Row>
                                    <Row className="mb-2">
                                        <Col>
                                            <span className="text-muted">{t("PRD175_frm1_lbl_TP_LINEA")} </span>
                                            <span>{linea.TipoLinea}</span>
                                        </Col>
                                    </Row>
                                </Col>
                                <Col>
                                    <Row className="mb-2">
                                        <Col>
                                            <span className="text-muted">{t("PRD175_frm1_lbl_DS_LINEA")} </span>
                                            <span>{linea.Descripcion}</span>
                                        </Col>
                                    </Row>
                                    <Row className="mb-2">
                                        <Col>
                                            <span className="text-muted">{t("PRD175_frm1_lbl_CD_ENDERECO_SALIDA")} </span>
                                            <span>{linea.UbicacionSalida}</span>
                                        </Col>
                                    </Row>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <h4 className="form-title">{t("PRD175_frm1_lbl_legend2")}</h4>
                            <Row>
                                <Col>
                                    <Row className="mb-2">
                                        <Col>
                                            <span className="text-muted">{t("PRD175_frm1_lbl_CD_SITUACION")} </span>
                                            <span>{ingreso.Situacion}</span>
                                        </Col>
                                    </Row>
                                    <Row className="mb-2">
                                        <Col>
                                            <span className="text-muted">{t("PRD175_frm1_lbl_NU_PRDC_DEFNICION")} </span>
                                            <span>{formula.Id}</span>
                                        </Col>
                                    </Row>
                                    <Row className="mb-2">
                                        <Col>
                                            <span className="text-muted">{t("PRD175_frm1_lbl_NU_PREDIO")} </span>
                                            <span>{ingreso.Predio}</span>
                                        </Col>
                                    </Row>
                                    <Row className="mb-2">
                                        <Col>
                                            <span className="text-muted">{t("PRD175_frm1_lbl_NU_DOCUMENTO_INGRESO")} </span>
                                            <span>{ingreso.DocumentoIngreso}</span>
                                        </Col>
                                    </Row>
                                </Col>
                                <Col>
                                    <Row className="mb-2">
                                        <Col>
                                            <span className="text-muted">{t("PRD175_frm1_lbl_DT_ADDROW")} </span>
                                            <span>{ingreso.FechaAlta}</span>
                                        </Col>
                                    </Row>
                                    <Row className="mb-2">
                                        <Col>
                                            <span className="text-muted">{t("PRD175_frm1_lbl_QT_FORMULA")} </span>
                                            <span>{ingreso.CantidadIteracionesFormula}</span>
                                        </Col>
                                    </Row>
                                    <Row className="mb-2">
                                        <Col>
                                            <span className="text-muted">{t("PRD175_frm1_lbl_CD_FUNCIONARIO")} </span>
                                            <span>{ingreso.FuncionarioNombre}</span>
                                        </Col>
                                    </Row>
                                    <Row className="mb-2">
                                        <Col>
                                            <span className="text-muted">{t("PRD175_frm1_lbl_NU_DOCUMENTO_EGRESO")} </span>
                                            <span>{ingreso.DocumentoEgreso}</span>
                                        </Col>
                                    </Row>
                                </Col>
                            </Row>
                        </Col>
                    </Row>                    
                </Col>                
            </Row>            
        </Page>
    );
}