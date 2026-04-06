import React from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldSelectAsync, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';

export function STO810UpdateMapeoModal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const initialValues = {
        cdEmpresaOrigen: "",
        cdProdutoOrigen: "",
        cantidadOrigen: "",
        cdEmpresaDestino: "",
        cdProdutoDestino: "",
        cantidadDestino: "",
    };

    const validationSchema = {
        cdEmpresaOrigen: Yup.string().required(),
        cdProdutoOrigen: Yup.string().required(),
        cantidadOrigen: Yup.string().required(),
        cdEmpresaDestino: Yup.string().required(),
        cdProdutoDestino: Yup.string().required(),
        cantidadDestino: Yup.string().required(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            { id: "cdEmpresaOrigen", value: props.mapeo.cdEmpresaOrigen },
            { id: "cdEmpresaDestino", value: props.mapeo.cdEmpresaDestino },
            { id: "cdProdutoOrigen", value: props.mapeo.cdProdutoOrigen }
        ];
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("STO810_grid_1").refresh();
            props.onHide();
        }
    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "cdEmpresaOrigen", value: props.mapeo.cdEmpresaOrigen },
            { id: "cdEmpresaDestino", value: props.mapeo.cdEmpresaDestino },
            { id: "cdProdutoOrigen", value: props.mapeo.cdProdutoOrigen },
            { id: "isSubmit", value: true }
        ];
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                id="STO810_form_UpdateMapeo"
                application="STO810ModificarMapeo"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={handleFormBeforeInitialize}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("STO810_Sec0_modalTitle_Update")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col md={6}>
                            <div className="form-group">
                                <label htmlFor="cdEmpresaOrigen">{t("STO810_frm1_lbl_cdEmpresaOrigen")}</label>
                                <FieldSelectAsync name="cdEmpresaOrigen" />
                                <StatusMessage for="cdEmpresaOrigen" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="cdProdutoOrigen">{t("STO810_frm1_lbl_cdProdutoOrigen")}</label>
                                <FieldSelectAsync name="cdProdutoOrigen" />
                                <StatusMessage for="cdProdutoOrigen" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="cantidadOrigen">{t("STO810_frm1_lbl_cantidadOrigen")}</label>
                                <Field name="cantidadOrigen" />
                                <StatusMessage for="cantidadOrigen" />
                            </div>
                        </Col>
                        <Col md={6}>
                            <div className="form-group">
                                <label htmlFor="cdEmpresaDestino">{t("STO810_frm1_lbl_cdEmpresaDestino")}</label>
                                <FieldSelectAsync name="cdEmpresaDestino" />
                                <StatusMessage for="cdEmpresaDestino" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="cdProdutoDestino">{t("STO810_frm1_lbl_cdProdutoDestino")}</label>
                                <FieldSelectAsync name="cdProdutoDestino" />
                                <StatusMessage for="cdProdutoDestino" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="cantidadDestino">{t("STO810_frm1_lbl_cantidadDestino")}</label>
                                <Field name="cantidadDestino" />
                                <StatusMessage for="cantidadDestino" />
                            </div>
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleClose}>
                        {t("STO810_frm1_btn_Cancelar")}
                    </Button>
                    <SubmitButton id="btnSubmitUpdateMapeo" variant="primary" label="STO810_frm1_btn_Modificar" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}