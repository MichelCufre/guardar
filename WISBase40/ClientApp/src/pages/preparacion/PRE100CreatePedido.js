import React, { useState } from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldDate, FieldSelect, FieldSelectAsync, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';

export function PRE100CreatePedido(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [showBtnDetalles, setShowBtnDetalles] = useState(false);
    const [showBtnDetallesLpn, setShowBtnDetallesLpn] = useState(false);

    const initialValues = {
        pedido: "",
        predio: "",
        empresa: "",
        cliente: "",
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
        pedido: Yup.string(),
        predio: Yup.string(),
        empresa: Yup.string(),
        cliente: Yup.string(),
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

    const onAfterInitialize = (context, form, query, nexus) => {
        const showBtnDetalles = query.parameters.find(p => p.id === "showBtnDetalles");
        const showBtnDetallesLpn = query.parameters.find(p => p.id === "showBtnDetallesLpn");

        if (showBtnDetalles?.value === "true")
            setShowBtnDetalles(true);

        if (showBtnDetallesLpn?.value === "true")
            setShowBtnDetallesLpn(true);
    };

    const handleClose = () => {
        props.onHide();
    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        query.parameters.push({ id: "isSubmit", value: true });
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("PRE100_grid_1").refresh();

            if (query.buttonId === "btnSubmitConfirmarIrDetalle") {
                props.onHide({ flujoLpn: false, pedido: query.parameters.find(a => a.id === "pedido").value, cliente: query.parameters.find(a => a.id === "cliente").value, empresa: query.parameters.find(a => a.id === "empresa").value });
                return;
            }
            else if (query.buttonId === "btnCrearDetalleLpn") {
                props.onHide({ flujoLpn: true, pedido: query.parameters.find(a => a.id === "pedido").value, cliente: query.parameters.find(a => a.id === "cliente").value, empresa: query.parameters.find(a => a.id === "empresa").value });
                return;
            }
            props.onHide();
        }
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Form
                id="PRE100_form_CreatePedido"
                application="PRE100CreatePedido"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onAfterInitialize={onAfterInitialize}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeSubmit={onBeforeSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("PRE100_Sec0_modalTitle_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="pedido">{t("PRE100_frm1_lbl_pedido")}</label>
                                <Field name="pedido" />
                                <StatusMessage for="pedido" />
                            </div>

                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="empresa">{t("PRE100_frm1_lbl_empresa")}</label>
                                        <FieldSelectAsync name="empresa" isClearable={true} />
                                        <StatusMessage for="empresa" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="predio">{t("PRE100_frm1_lbl_predio")}</label>
                                        <FieldSelect name="predio" />
                                        <StatusMessage for="predio" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="cliente">{t("PRE100_frm1_lbl_cliente")}</label>
                                        <FieldSelectAsync name="cliente" isClearable={true} />
                                        <StatusMessage for="cliente" />
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
                            <div className="form-group">
                                <label htmlFor="tipoExpedicion">{t("PRE100_frm1_lbl_tipoExpedicion")}</label>
                                <FieldSelect name="tipoExpedicion" isClearable={true} />
                                <StatusMessage for="tipoExpedicion" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="tipoPedido">{t("PRE100_frm1_lbl_tipoPedido")}</label>
                                <FieldSelect name="tipoPedido" isClearable={true} />
                                <StatusMessage for="tipoPedido" />
                            </div>
                        </Col>
                        <Col>

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
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="idReserva">{t("PRE100_frm1_lbl_idReserva")}</label>
                                        <Field name="idReserva" />
                                        <StatusMessage for="idReserva" />
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
                    <Row>
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
                    <SubmitButton id="btnSubmitCreatePedido" variant="primary" label="General_Sec0_btn_Confirmar" />
                    <div style={{ display: showBtnDetalles ? "block" : "none" }}>
                        <SubmitButton id="btnSubmitConfirmarIrDetalle" variant="primary" label="PRE100_frm1_btn_confirmarIrDetalle" />
                    </div>
                    <div style={{ display: showBtnDetallesLpn ? "block" : "none" }}>
                        <SubmitButton id="btnCrearDetalleLpn" variant="primary" label="PRE100_frm1_btn_CrearDetalleLpn" />
                    </div>
                </Modal.Footer>
            </Form>
        </Modal>
    );
}