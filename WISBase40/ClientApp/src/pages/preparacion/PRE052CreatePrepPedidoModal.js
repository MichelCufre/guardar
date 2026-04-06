import React, { useState, useRef } from 'react';
import { Modal, Button, Row, Col } from 'react-bootstrap';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldToggle, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';

export function PRE052CreatePrepPedidoModal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const initialValues = {
        descripcion: "",
        predio: "",
        empresa: "",
        tipoPreparacion: "",
        permitePickearVencido: "",
        permitePickearAveriado: ""
    };

    const validationSchema = {
        descripcion: Yup.string(),
        predio: Yup.string().required(),
        empresa: Yup.string().required(),
        tipoPreparacion: Yup.string(),
        permitePickearVencido: Yup.boolean(),
        permitePickearAveriado: Yup.boolean()
    };

    const handleClose = () => {
        props.onHide();

    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("PRE052_grid_1").refresh();

            if (query.buttonId === "btnSubmitConfirmarAsociarPed") {
                props.onHide({
                    preparacion: query.parameters.find(a => a.id === "idPreparacion").value,
                    empresa: query.parameters.find(a => a.id === "empresa").value
                });
                return;
            }

            props.onHide();
        }
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                id="PRE052CreatePrepPedido_Form_1"
                application="PRE052CreatePrepPedido"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onAfterSubmit={handleFormAfterSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("PRE052_Sec0_mdl_PrepPedidos_Titulo")}</Modal.Title>
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
                            </Row>
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleClose}>{t("General_Sec0_btn_Cerrar")}</Button>
                    <SubmitButton id="btnSubmitCreatePrepPedido" variant="primary" label="General_Sec0_btn_Confirmar" />
                    <SubmitButton id="btnSubmitConfirmarAsociarPed" variant="primary" className="btn btn-info" label="PRE052_Sec0_btn_AsociarPedidos" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}

