import React, { useState } from 'react';
import { Modal, Col, Row, Button } from 'react-bootstrap';
import { StatusMessage, FieldSelectAsync, FieldToggle, SubmitButton, Form, FieldSelect } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';

export function DOC100Update(props) {
    const { t } = useTranslation("translation", { useSuspense: false });
    const [showEmpresaIngreso, setShowEmpresaIngreso] = useState(false);

    const initialValues = {
        empresaEgreso: "",
        empresaIngreso: "",
        preparacion: "",
        tpOperativa: "",
        autoDocIngreso: "",
        docIngreso: "",
        autoDocEgreso: "",
        docEgreso: "",
    };

    const validationSchema = {
        empresaEgreso: Yup.string().required(),
        empresaIngreso: Yup.string(),
        preparacion: Yup.string().required(),
        tpOperativa: Yup.string().required(),
        autoDocIngreso: Yup.boolean(),
        docIngreso: Yup.string(),
        autoDocEgreso: Yup.boolean(),
        docEgreso: Yup.string(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [{ id: "nroDocPrep", value: props.nroDocPrep }];

    }

    const onAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("DOC100_grid_1").refresh();
            props.onHide();
        }
    };

    const onBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [{ id: "nroDocPrep", value: props.nroDocPrep }];
    }

    const onAfterValidateField = (context, form, query, nexus) => {
        if (query.fieldId === "tpOperativa") {
            const reqEmpresaIngreso = query.parameters.find(p => p.id === "reqEmpresaIngreso");
            setShowEmpresaIngreso(reqEmpresaIngreso && reqEmpresaIngreso.value === "true");
        }
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                id="DOC100_form_Update"
                application="DOC100Update"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onAfterSubmit={onAfterSubmit}
                onBeforeSubmit={onBeforeSubmit}
                onBeforeInitialize={onBeforeInitialize}
                onAfterValidateField={onAfterValidateField}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("DOC100_Sec0_modalTitle_Update")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col>
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="tpOperativa">{t("DOC100_frm1_lbl_tpOperativa")}</label>
                                        <FieldSelect name="tpOperativa" />
                                        <StatusMessage for="tpOperativa" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="empresaEgreso">{t("DOC100_frm1_lbl_empresaEgreso")}</label>
                                        <FieldSelectAsync name="empresaEgreso" isClearable={true} />
                                        <StatusMessage for="empresaEgreso" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="preparacion">{t("DOC100_frm1_lbl_preparacion")}</label>
                                        <FieldSelectAsync name="preparacion" isClearable={true} />
                                        <StatusMessage for="preparacion" />
                                    </div>
                                </Col>
                            </Row>
                            <Row style={{ display: showEmpresaIngreso? 'block': 'none' }}>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="empresaIngreso">{t("DOC100_frm1_lbl_empresaIngreso")}</label>
                                        <FieldSelectAsync name="empresaIngreso" isClearable={true} />
                                        <StatusMessage for="empresaIngreso" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" style={{ marginTop: "40px" }}>
                                        <FieldToggle name="autoDocIngreso" label={t("DOC100_frm1_lbl_autoDocIngreso")} />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" style={{ marginTop: "40px" }}>
                                        <FieldToggle name="autoDocEgreso" label={t("DOC100_frm1_lbl_autoDocEgreso")} />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="docIngreso">{t("DOC100_frm1_lbl_docIngreso")}</label>
                                        <FieldSelectAsync name="docIngreso" isClearable={true} />
                                        <StatusMessage for="docIngreso" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group">
                                        <label htmlFor="docEgreso">{t("DOC100_frm1_lbl_docEgreso")}</label>
                                        <FieldSelectAsync name="docEgreso" isClearable={true} />
                                        <StatusMessage for="docEgreso" />
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
                    <SubmitButton id="btnSubmitUpdate" variant="primary" label="General_Sec0_btn_Confirmar" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}

