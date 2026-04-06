import React, { useState } from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldDate, FieldSelect, FieldSelectAsync, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';

export function PRE100UpdatePedido(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [empresaNombre, setEmpresaNombre] = useState("");
    const [agenteDescripcion, setAgenteDescripcion] = useState("");
    const [agenteCodigo, setAgenteCodigo] = useState("");
    const [agenteTipo, setAgenteTipo] = useState("");

    const initialValues = {
        predio: "",
        ruta: "",
        tipoExpedicion: "",
        tipoPedido: "",
        liberarDesde: "",
        liberarHasta: "",
        fechaEmision: "",
        fechaEntrega: "",
        memo: "",
        direccionEntrega: "",
        anexo: "",
        idReserva: "",
        ComparteContenedorPicking: "",
        ComparteContenedorEntrega: "",
        telofonoPrincipal: "",
        telefonoSecundario: "",
        latitud: "",
        longitud: "",
        zona: "",
    };

    const validationSchema = {
        predio: Yup.string(),
        ruta: Yup.string(),
        tipoExpedicion: Yup.string(),
        tipoPedido: Yup.string(),
        liberarDesde: Yup.string(),
        liberarHasta: Yup.string(),
        fechaEmision: Yup.string(),
        fechaEntrega: Yup.string(),
        memo: Yup.string(),
        direccionEntrega: Yup.string(),
        anexo: Yup.string(),
        idReserva: Yup.string(),
        ComparteContenedorPicking: Yup.string(),
        ComparteContenedorEntrega: Yup.string(),
        telofonoPrincipal: Yup.string(),
        telefonoSecundario: Yup.string(),
        latitud: Yup.string(),
        longitud: Yup.string(),
        zona: Yup.string(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            { id: "pedido", value: props.pedido },
            { id: "cliente", value: props.cliente },
            { id: "empresa", value: props.empresa }
        ];
    }

    const handleFormAfterInitialize = (context, form, query, nexus) => {
        setEmpresaNombre(query.parameters.find(d => d.id === "empresaNombre").value);
        setAgenteDescripcion(query.parameters.find(d => d.id === "agenteDescripcion").value);
        setAgenteCodigo(query.parameters.find(d => d.id === "agenteCodigo").value);
        setAgenteTipo(query.parameters.find(d => d.id === "agenteTipo").value);
    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "pedido", value: props.pedido },
            { id: "cliente", value: props.cliente },
            { id: "empresa", value: props.empresa },
            { id: "isSubmit", value: true }
        ];
    }

    const handleFormBeforeValidateField = (context, form, query, nexus) => {
        query.parameters = [
            { id: "pedido", value: props.pedido },
            { id: "cliente", value: props.cliente },
            { id: "empresa", value: props.empresa }
        ];
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("PRE100_grid_1").refresh();
            props.onHide();
        }
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Form
                id="PRE100_form_UpdatePedido"
                application="PRE100UpdatePedido"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeValidateField={handleFormBeforeValidateField}
                onBeforeInitialize={handleFormBeforeInitialize}
                onAfterInitialize={handleFormAfterInitialize}
                onBeforeSubmit={handleFormBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("PRE100UpdatePedido_Sec0_modalTitle_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>

                    <Row>
                        <Col lg={4}>
                            <Row >
                                <Col lg={4}>
                                    <span style={{ fontWeight: "bold" }}> {t("PRE100_frm1_lbl_pedido")}: </span>
                                </Col>
                                <Col lg={8}>
                                    <span >{`${props.pedido}`}</span>
                                </Col>
                            </Row>
                        </Col>
                        <Col lg={4}>
                            <Row>
                                <Col lg={4}>
                                    <span style={{ fontWeight: "bold" }}>{t("PRE100_frm1_lbl_empresa")}: </span>
                                </Col>
                                <Col lg={8}>
                                    <span> {`${props.empresa}`} - {`${empresaNombre}`}</span>
                                </Col>
                            </Row>
                        </Col>
                        <Col lg={4}>
                            <Row>
                                <Col lg={4}>
                                    <span style={{ fontWeight: "bold" }}>{t("PRE100_frm1_lbl_cliente")}:</span>
                                </Col>
                                <Col lg={8}>
                                    <span> {`${agenteTipo}`}-{`${agenteCodigo}`}-{`${agenteDescripcion}`}  </span>
                                </Col>
                            </Row>
                        </Col>
                    </Row>

                    <hr />

                    <Row>
                        <Col>
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="predio">{t("PRE100_frm1_lbl_predio")}</label>
                                        <FieldSelect name="predio" />
                                        <StatusMessage for="predio" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="ruta">{t("PRE100_frm1_lbl_ruta")}</label>
                                        <FieldSelectAsync name="ruta" isClearable={true} />
                                        <StatusMessage for="ruta" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="tipoExpedicion">{t("PRE100_frm1_lbl_tipoExpedicion")}</label>
                                        <FieldSelect name="tipoExpedicion" isClearable={true} />
                                        <StatusMessage for="tipoExpedicion" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="tipoPedido">{t("PRE100_frm1_lbl_tipoPedido")}</label>
                                        <FieldSelect name="tipoPedido" isClearable={true} />
                                        <StatusMessage for="tipoPedido" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col md={6}>
                                    <div className="form-group">
                                        <label htmlFor="liberarDesde">{t("PRE100_frm1_lbl_liberarDesde")}</label>
                                        <FieldDate name="liberarDesde" />
                                        <StatusMessage for="liberarDesde" />
                                    </div>
                                </Col>
                                <Col md={6}>
                                    <div className="form-group">
                                        <label htmlFor="liberarHasta">{t("PRE100_frm1_lbl_liberarHasta")}</label>
                                        <FieldDate name="liberarHasta" />
                                        <StatusMessage for="liberarHasta" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col md={6}>
                                    <div className="form-group">
                                        <label htmlFor="fechaEmision">{t("PRE100_frm1_lbl_fechaEmision")}</label>
                                        <FieldDate name="fechaEmision" />
                                        <StatusMessage for="fechaEmision" />
                                    </div>
                                </Col>
                                <Col md={6}>
                                    <div className="form-group">
                                        <label htmlFor="fechaEntrega">{t("PRE100_frm1_lbl_fechaEntrega")}</label>
                                        <FieldDate name="fechaEntrega" />
                                        <StatusMessage for="fechaEntrega" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col md={6}>
                                    <div className="form-group">
                                        <label htmlFor="direccionEntrega">{t("PRE100_frm1_lbl_direccionEntrega")}</label>
                                        <Field name="direccionEntrega" />
                                        <StatusMessage for="direccionEntrega" />
                                    </div>
                                </Col>
                                <Col md={6}>
                                    <div className="form-group">
                                        <label htmlFor="zona">{t("PRE100_frm1_lbl_zona")}</label>
                                        <FieldSelect name="zona" isClearable={true} />
                                        <StatusMessage for="zona" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="memo">{t("PRE100_frm1_lbl_memo")}</label>
                                        <Field name="memo" />
                                        <StatusMessage for="memo" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="anexo">{t("PRE100_frm1_lbl_anexo")}</label>
                                        <Field name="anexo" />
                                        <StatusMessage for="anexo" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col md={6}>
                                    <div className="form-group">
                                        <label htmlFor="idReserva">{t("PRE100_frm1_lbl_idReserva")}</label>
                                        <Field name="idReserva" />
                                        <StatusMessage for="idReserva" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="ComparteContenedorPicking">{t("PRE100_frm1_lbl_ComparteContenedorPicking")}</label>
                                        <Field name="ComparteContenedorPicking" />
                                        <StatusMessage for="ComparteContenedorPicking" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="ComparteContenedorEntrega">{t("PRE100_frm1_lbl_ComparteContenedorEntrega")}</label>
                                        <Field name="ComparteContenedorEntrega" />
                                        <StatusMessage for="ComparteContenedorEntrega" />
                                    </div>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="telofonoPrincipal">{t("PRE100_frm1_lbl_telofonoPrincipal")}</label>
                                <Field name="telofonoPrincipal" />
                                <StatusMessage for="telofonoPrincipal" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="telefonoSecundario">{t("PRE100_frm1_lbl_telefonoSecundario")}</label>
                                <Field name="telefonoSecundario" />
                                <StatusMessage for="telefonoSecundario" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="latitud">{t("PRE100_frm1_lbl_latitud")}</label>
                                <Field name="latitud" />
                                <StatusMessage for="latitud" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="longitud">{t("PRE100_frm1_lbl_longitud")}</label>
                                <Field name="longitud" />
                                <StatusMessage for="longitud" />
                            </div>
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleClose}>
                        {t("General_Sec0_btn_Cerrar")}
                    </Button>
                    <SubmitButton id="btnSubmitUpdatePedido" variant="primary" label="General_Sec0_btn_Editar" />
                </Modal.Footer>
            </Form>
        </Modal >
    );
}