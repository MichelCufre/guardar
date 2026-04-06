import React from 'react';
import { Button, Col, Container, Modal, Row } from 'react-bootstrap';
import * as Yup from 'yup';
import { Field, FieldSelect, FieldToggle, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { useCustomTranslation } from '../../components/TranslationHook';
import { withPageContext } from '../../components/WithPageContext';

function InternalREG700CreateRecorridoModal(props) {

    const { t } = useCustomTranslation();

    const handleClose = () => {
        props.onHide();
    };

    const initialValues = {
        nombre: "",
        predio: "",
        descripcion: "",
        isEnabled: true,
    };

    const validationSchema = {

        nombre: Yup.string().required().max(50, t("General_Sec0_Error_LargoMaxExcedidoConEsperado", [50])),
        predio: Yup.string().required(),
        descripcion: Yup.string().required().max(200, t("General_Sec0_Error_LargoMaxExcedidoConEsperado", [200])),
        isEnabled: Yup.boolean(),
    };


    const handleFormAfterSubmit = (context, form, query, nexus) => {

        if (query.buttonId == "btnSubmitConfirmarIrDetalle")
            props.onHide(query.parameters.find(d => d.id === "REG700_DETALLES_NU_RECORRIDO").value, query.parameters.find(d => d.id === "REG700_DETALLE_IMPORT_HABILITADO").value);
        else
            props.onHide();
    }

    return (

        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">

            <Form
                application="REG700Create"
                id="REG700Create_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onAfterSubmit={handleFormAfterSubmit}
            >

                <Modal.Header closeButton>
                    <Modal.Title>{t("REG700_Sec0_mdlCreate_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>

                    <Container fluid>
                        <Row>
                            <Col>
                                <Row>

                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="nombre">{t("REG700_frm1_lbl_nombre")}</label>
                                            <Field name="nombre" maxLength="50" />
                                            <StatusMessage for="nombre" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="predio">{t("REG700_frm1_lbl_predio")}</label>
                                            <FieldSelect name="predio" />
                                            <StatusMessage for="predio" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group">
                                            <FieldToggle name="isEnabled" label={t("REG700_frm1_lbl_isHabilitado")} />
                                            <StatusMessage for="isEnabled" />
                                        </div>
                                    </Col>
                                </Row>

                                <Row>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="descripcion">{t("REG700_frm1_lbl_descripcion")}</label>
                                            <Field name="descripcion" maxLength="200" />
                                            <StatusMessage for="descripcion" />
                                        </div>
                                    </Col>
                                </Row>

                            </Col>
                        </Row>
                    </Container>

                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}>{t("REG700_frm1_btn_Cerrar")}</Button>
                    <SubmitButton id="btnSubmit" id="btnSubmitConfirmar" variant="primary" label="REG700_frm1_btn_Confirmar" />
                    <SubmitButton id="btnSubmit" id="btnSubmitConfirmarIrDetalle" variant="primary" label="REG700_frm1_btn_CrearIrADetalles" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}

export const REG700CreateRecorridoModal = withPageContext(InternalREG700CreateRecorridoModal);