import React from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldSelect, FieldSelectAsync, FieldToggle, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';

export function PRE052CreatePrepLibreModal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const initialValues = {
        descripcion: "",
        predio: "",
        empresa: "",
        cliente: "",
        tipoPreparacion: "",
        tipoExpedicion: "",
        tipoPedido: "",
        idReserva: "",
        permitePickearVencido: "",
        permitePickearAveriado: "",
        validarProductoProveedor: "",
    };

    const validationSchema = {
        descripcion: Yup.string(),
        predio: Yup.string().required(),
        empresa: Yup.string().required(),
        cliente: Yup.string().required(),
        tipoPreparacion: Yup.string(),
        tipoExpedicion: Yup.string().required(),
        tipoPedido: Yup.string().required(),
        idReserva: Yup.string(),
        permitePickearVencido: Yup.boolean(),
        permitePickearAveriado: Yup.boolean(),
        validarProductoProveedor: Yup.boolean()
    };

    const handleClose = () => {
        props.onHide(null, null, props.nexus);

    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("PRE052_grid_1").refresh();
            props.onHide();
        }
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                id="PRE052CreatePrepLibre_Form_1"
                application="PRE052CreatePrepLibre"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onAfterSubmit={handleFormAfterSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("PRE052_Sec0_mdl_PrepLibre_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="tipoPreparacion">{t("PRE052_frm1_lbl_Libre_tpPreparacion")}</label>
                                <FieldSelect name="tipoPreparacion" isClearable={true} />
                                <StatusMessage for="tipoPreparacion" />
                            </div>

                            <div className="form-group">
                                <label htmlFor="descripcion">{t("PRE052_frm1_lbl_Libre_Descripcion")}</label>
                                <Field name="descripcion" />
                                <StatusMessage for="descripcion" />
                            </div>
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="empresa">{t("PRE052_frm1_lbl_Libre_Empresa")}</label>
                                        <FieldSelectAsync name="empresa" isClearable={true} />
                                        <StatusMessage for="empresa" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="predio">{t("PRE052_frm1_lbl_Libre_Predio")}</label>
                                        <FieldSelect name="predio" />
                                        <StatusMessage for="predio" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="cliente">{t("PRE052_frm1_lbl_Libre_Cliente")}</label>
                                        <FieldSelectAsync name="cliente" isClearable={true} />
                                        <StatusMessage for="cliente" />
                                    </div>
                                </Col>
                            </Row>

                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="tipoExpedicion">{t("PRE052_frm1_lbl_Libre_tpExpedicion")}</label>
                                        <FieldSelect name="tipoExpedicion" isClearable={true} />
                                        <StatusMessage for="tipoExpedicion" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="tipoPedido">{t("PRE052_frm1_lbl_Libre_tpPedido")}</label>
                                        <FieldSelect name="tipoPedido" isClearable={true} />
                                        <StatusMessage for="tipoPedido" />
                                    </div>
                                </Col>
                            </Row>

                            <Row>
                                <div className="form-group">
                                    <label htmlFor="idReserva">{t("PRE052_frm1_lbl_Libre_IdReserva")}</label>
                                    <Field name="idReserva" />
                                    <StatusMessage for="idReserva" />
                                </div>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <FieldToggle name="permitePickearVencido" label={t("PRE052_frm1_lbl_PermitePickearVencido")} />
                                        <StatusMessage for="permitePickearVencido" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <FieldToggle name="permitePickearAveriado" label={t("PRE052_frm1_lbl_PermitePickearAveriado")} />
                                        <StatusMessage for="permitePickearAveriado" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <FieldToggle name="validarProductoProveedor" label={t("PRE052_frm1_lbl_ValidarProductoProveedor")} />
                                        <StatusMessage for="validarProductoProveedor" />
                                    </div>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleClose}>
                        {t("General_Sec0_btn_Cerrar")}
                    </Button>
                    <SubmitButton id="btnSubmitCreatePrepLibre" variant="primary" label="General_Sec0_btn_Confirmar" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}

