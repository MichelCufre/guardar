import React from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import * as Yup from 'yup';
import { Field, FieldSelect, FieldSelectAsync, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { useCustomTranslation } from '../../components/TranslationHook';
import { withPageContext } from '../../components/WithPageContext';

function InternalREG221UpdateModal(props) {
    const { t } = useCustomTranslation();

    const initialValues = {
        cantidadDiasValidacion: "",
    };

    const validationSchema = {
        cantidadDiasValidacion: Yup.string(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {

        query.parameters = [
            { id: "cliente", value: props.agente.find(a => a.id === "cliente").value },
            { id: "empresa", value: props.agente.find(a => a.id === "empresa").value },
            { id: "ventanaLiberacion", value: props.agente.find(a => a.id === "ventanaLiberacion").value }
        ];

    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        if (context.responseStatus === "OK") {
            nexus.getGrid("REG221_grid_1").refresh();
            props.onHide();
        }
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Form
                id="REG221Update_form_1"
                application="REG221"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={handleFormBeforeInitialize}
                onAfterSubmit={handleFormAfterSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("REG221_Sec0_mdlUpdate_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="empresa">{t("REG221_frm1_lbl_empresa")}</label>
                                <FieldSelectAsync name="empresa" />
                                <StatusMessage for="empresa" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="cliente">{t("REG221_frm1_lbl_cliente")}</label>
                                <FieldSelectAsync name="cliente" />
                                <StatusMessage for="cliente" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="ventanaLiberacion">{t("REG221_frm1_lbl_ventanaLiberacion")}</label>
                                <FieldSelect name="ventanaLiberacion" />
                                <StatusMessage for="ventanaLiberacion" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group">
                                <label htmlFor="cantidadDiasValidacion">{t("REG221_frm1_lbl_cantidadDiasValidacion")}</label>
                                <Field name="cantidadDiasValidacion" />
                                <StatusMessage for="cantidadDiasValidacion" />
                            </div>
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                        {t("REG221_frm1_btn_CANCELAR")}
                    </Button>
                    <SubmitButton id="btnSubmitCreateFamilia" variant="primary" label="REG221_frm1_btn_MODIFICAR" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}

export const REG221UpdateModal = withPageContext(InternalREG221UpdateModal);