import React, { useState } from 'react';
import { Modal, Button, Row, Col } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldSelect, FieldDate, FieldSelectAsync, StatusMessage, FieldTextArea } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';

function InternalPRD113ProductoNoEsperadoModal(props) {
    const { t } = useTranslation();

    const [hasPermission, setHasPermission] = useState(true);
    const validationSchema = {
        empresa: Yup.string(),
        producto: Yup.string(),
        cantidad: Yup.string(),
        motivo: Yup.string(),
        dsMotivo: Yup.string(),
        fechaVencimiento: Yup.string()
    };

    const handleClose = () => {
        props.onHide("N");
    };

    const handleSubmit = () => {
        props.nexus.getForm("PRD113_form_ProductoNoEsperado").submit();
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            props.onHide("S");
            nexus.getGrid("PRD113_grid_3").refresh();
        }
    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "lote", value: props.lote },
            { id: "idIngresoProduccion", value: props.idIngresoProduccion },
            { id: "ubicacionProduccion", value: props.ubicacionProduccion },
            { id: "modalidadLote", value: props.modalidadLote },
        ];
    }

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            { id: "idIngresoProduccion", value: props.idIngresoProduccion },
            { id: "ubicacionProduccion", value: props.ubicacionProduccion },
            { id: "modalidadLote", value: props.modalidadLote },
        ];
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRD113_Sec0_modalTitle_TituloProductoNoEsperadoProduccion")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Row>
                    <Col>
                        <Form
                            id="PRD113_form_ProductoNoEsperado"
                            application="PRD113ProductoNoEsperado"
                            validationSchema={validationSchema}
                            onBeforeInitialize={handleFormBeforeInitialize}
                            onAfterSubmit={handleFormAfterSubmit}
                            onBeforeSubmit={handleFormBeforeSubmit}
                        >
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="empresa">{t("PRD113_form1_label_Empresa")}</label>
                                        <Field id="empresa" name="empresa"/>
                                        <StatusMessage for="empresa" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="producto">{t("PRD113_form1_label_Producto")}</label>
                                        <FieldSelectAsync id="producto" name="producto" />
                                        <StatusMessage for="producto" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col sm={6}>
                                    <div className="form-group" >
                                        <label htmlFor="fechaVencimiento">{t("PRD113_frm1_lbl_fechaVencimiento")}</label>
                                        <FieldDate name="fechaVencimiento" />
                                        <StatusMessage for="fechaVencimiento" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="cantidad">{t("PRD113_form1_label_Cantidad")}</label>
                                        <Field id="cantidad" name="cantidad" />
                                        <StatusMessage for="cantidad" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="motivo">{t("PRD113_form1_label_Motivo")}</label>
                                        <FieldSelect id="motivo" name="motivo" />
                                        <StatusMessage for="motivo" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="dsMotivo">{t("PRD113_form1_label_DsMotivo")}</label>
                                        <Field id="dsMotivo" name="dsMotivo" />
                                        <StatusMessage for="dsMotivo" />
                                    </div>
                                </Col>
                            </Row>
                        </Form>
                    </Col>
                </Row>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="danger" onClick={handleClose}>
                    {t("PRD113_frm1_btn_Cancelar")}
                </Button>
                <Button variant="primary" onClick={handleSubmit} disabled={!hasPermission}>
                    {t("PRD113_frm1_btn_Confirmar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const PRD113ProductoNoEsperadoModal = withPageContext(InternalPRD113ProductoNoEsperadoModal);