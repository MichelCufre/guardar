import React from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldSelect, FieldSelectAsync, FieldToggle, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';

export function STO820CreateTraspasoModal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const initialValues = {
        idExterno: "",
        descripcion: "",
        cdEmpresaOrigen: "",
        cdEmpresaDestino: "",
        tpTraspaso: "",
        flFinalizarConPreparacion: "",
        flPropagarLPN: "",
        flReplicaProductos: "",
        flReplicaCB: "",
        flCtrlCaractIguales: "",
        flReplicaAgentes: "",
    };

    const validationSchema = {
        idExterno: Yup.string(),
        descripcion: Yup.string(),
        cdEmpresaOrigen: Yup.string().required(),
        cdEmpresaDestino: Yup.string().required(),
        tpTraspaso: Yup.string().required(),
        flFinalizarConPreparacion: Yup.boolean(),
        flPropagarLPN: Yup.boolean(),
        flReplicaProductos: Yup.boolean(),
        flReplicaCB: Yup.boolean(),
        flCtrlCaractIguales: Yup.boolean(),
        flReplicaAgentes: Yup.boolean(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("STO820_grid_1").refresh();
            props.onHide();
        }
    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        query.parameters.push({ id: "isSubmit", value: true });
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                id="STO820_form_CreateTraspaso"
                application="STO820CrearTraspaso"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("STO820_Sec0_modalTitle_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col md={6}>
                            <div className="form-group">
                                <label htmlFor="idExterno">{t("STO820_frm1_lbl_idExterno")}</label>
                                <Field name="idExterno" />
                                <StatusMessage for="idExterno" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="descripcion">{t("STO820_frm1_lbl_descripcion")}</label>
                                <Field name="descripcion" />
                                <StatusMessage for="descripcion" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="cdEmpresaOrigen">{t("STO820_frm1_lbl_cdEmpresaOrigen")}</label>
                                <FieldSelectAsync name="cdEmpresaOrigen" />
                                <StatusMessage for="cdEmpresaOrigen" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="cdEmpresaDestino">{t("STO820_frm1_lbl_cdEmpresaDestino")}</label>
                                <FieldSelectAsync name="cdEmpresaDestino" />
                                <StatusMessage for="cdEmpresaDestino" />
                            </div>
                        </Col>
                        <Col md={3}>
                            <div className="form-group">
                                <label htmlFor="nuDocumentoIngreso">{t("STO820_frm1_lbl_nuDocumentoIngreso")}</label>
                                <Field name="nuDocumentoIngreso" />
                                <StatusMessage for="nuDocumentoIngreso" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="nuDocumentoEgreso">{t("STO820_frm1_lbl_nuDocumentoEgreso")}</label>
                                <Field name="nuDocumentoEgreso" />
                                <StatusMessage for="nuDocumentoEgreso" />
                            </div>
                        </Col>
                        <Col md={3}>
                            <div className="form-group">
                                <label htmlFor="tpDocumentoIngreso">{t("STO820_frm1_lbl_tpDocumentoIngreso")}</label>
                                <Field name="tpDocumentoIngreso" />
                                <StatusMessage for="tpDocumentoIngreso" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="tpDocumentoEgreso">{t("STO820_frm1_lbl_tpDocumentoEgreso")} </label>
                                <Field name="tpDocumentoEgreso" />
                                <StatusMessage for="tpDocumentoEgreso" />
                            </div>
                        </Col>
                    </Row>
                    <hr />
                    <Row>
                        <Col md={12}>
                            <div className="form-group">
                                <label htmlFor="tpTraspaso">{t("STO820_frm1_lbl_tpTraspaso")}</label>
                                <FieldSelect name="tpTraspaso" />
                                <StatusMessage for="tpTraspaso" />
                            </div>
                        </Col>
                    </Row>
                    <hr />
                    <Row>
                        <Col md={6}>
                            <div className="form-group">
                                <FieldToggle name="flFinalizarConPreparacion" label={t("STO820_frm1_lbl_flFinalizarConPreparacion")} />
                                <StatusMessage for="flFinalizarConPreparacion" />
                            </div>
                        </Col>

                        <Col md={6}>
                            <div className="form-group">
                                <FieldToggle name="flPropagarLPN" label={t("STO820_frm1_lbl_flPropagarLPN")} />
                                <StatusMessage for="flPropagarLPN" />
                            </div>
                        </Col>
                    </Row>
                    <hr />
                    <Row>
                        <Col md={6}>
                            <div className="form-group">
                                <FieldToggle name="flReplicaProductos" label={t("STO820_frm1_lbl_flReplicaProductos")} />
                                <StatusMessage for="flReplicaProductos" />
                            </div>
                            <div className="form-group">
                                <FieldToggle name="flReplicaCB" label={t("STO820_frm1_lbl_flReplicaCB")} />
                                <StatusMessage for="flReplicaCB" />
                            </div>
                        </Col>
                        <Col md={6}>
                            <div className="form-group">
                                <FieldToggle name="flReplicaAgentes" label={t("STO820_frm1_lbl_flReplicaAgentes")} />
                                <StatusMessage for="flReplicaAgentes" />
                            </div>
                            <div className="form-group">
                                <FieldToggle name="flCtrlCaractIguales" label={t("STO820_frm1_lbl_flCtrlCaractIguales")} />
                                <StatusMessage for="flCtrlCaractIguales" />
                            </div>
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleClose}>
                        {t("STO820_frm1_btn_Cancelar")}
                    </Button>
                    <SubmitButton id="btnSubmitCreateTraspaso" variant="primary" label="STO820_frm1_btn_Crear" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}