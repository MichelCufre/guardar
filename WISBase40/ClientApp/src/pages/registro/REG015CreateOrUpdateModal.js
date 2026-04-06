import React, { useState } from 'react';
import { Modal, Col, Row, Button } from 'react-bootstrap';
import { StatusMessage, Field, FieldSelectAsync, SubmitButton, Form } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';

export function REG015CreateOrUpdate(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const initialValues = {
        empresa: "",
        cliente: "",
        producto: "",
        codigoExterno: "",
    };

    const validationSchema = {
        empresa: Yup.string().required(),
        cliente: Yup.string().required(),
        producto: Yup.string().required(),
        codigoExterno: Yup.string().required(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const onBeforeInitialize = (context, form, data, nexus) => {
        if (props.isUpdate) {
            data.parameters.push({ id: "isUpdate", value: "S" });
            data.parameters.push({ id: "empresa", value: props.empresa });
            data.parameters.push({ id: "cliente", value: props.cliente });
            data.parameters.push({ id: "producto", value: props.producto });
        }
    }

    const onBeforeSubmit = (context, form, data, nexus) => {
        if (props.isUpdate) {
            data.parameters.push({ id: "isUpdate", value: "S" });
            data.parameters.push({ id: "empresa", value: props.empresa });
            data.parameters.push({ id: "cliente", value: props.cliente });
            data.parameters.push({ id: "producto", value: props.producto });
        }
    }

    const onBeforeValidateField = (context, form, query, nexus) => {
        if (props.isUpdate) {
            query.parameters.push({ id: "isUpdate", value: "S" });
            query.parameters.push({ id: "empresa", value: props.empresa });
            query.parameters.push({ id: "cliente", value: props.cliente });
            query.parameters.push({ id: "producto", value: props.producto });
        }
    };

    const onAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "ERROR")
            return;

        nexus.getGrid("REG015_grid_1").refresh();

        props.onHide();
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                id="REG015_form_1"
                application="REG015CreateOrUpdate"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={onBeforeInitialize}
                onBeforeSubmit={onBeforeSubmit}
                onBeforeValidateField={onBeforeValidateField}
                onAfterSubmit={onAfterSubmit}

            >
                <Modal.Header closeButton>
                    <Modal.Title>{props.isUpdate ? t("REG015_Sec0_Title_Update") : t("REG015_Sec0_Title_Create")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>

                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="empresa">{t("PRE100_frm1_lbl_empresa")}</label>
                                <FieldSelectAsync name="empresa" isClearable={true} />
                                <StatusMessage for="empresa" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="cliente">{t("REG015_frm1_lbl_cliente")}</label>
                                <FieldSelectAsync name="cliente" isClearable={true} />
                                <StatusMessage for="cliente" />
                            </div>
                        </Col>
                    </Row>

                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="producto">{t("REG015_frm1_lbl_producto")}</label>
                                <FieldSelectAsync name="producto" isClearable={true} />
                                <StatusMessage for="producto" />
                            </div>
                        </Col>
                    </Row>

                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="codigoExterno">{t("REG015_frm1_lbl_CodigoExterno")}</label>
                                <Field name="codigoExterno" />
                                <StatusMessage for="codigoExterno" />
                            </div>
                        </Col>
                    </Row>

                </Modal.Body>
                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleClose}>
                        {t("General_Sec0_btn_Cerrar")}
                    </Button>
                    <SubmitButton id="btnSubmit" variant="primary" label="General_Sec0_btn_Confirmar" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}