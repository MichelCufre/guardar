import React from 'react';
import { Button, Col, Container, Modal, Row } from 'react-bootstrap';
import * as Yup from 'yup';
import { Field, FieldSelect, FieldToggle, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { useCustomTranslation } from '../../components/TranslationHook';
import { withPageContext } from '../../components/WithPageContext';

function InternalREG700UpdateRecorridoModal(props) {

    const { t } = useCustomTranslation();

    const handleClose = () => {
        props.onHide();
    };

    const initialValues = {
        nombre: "",
        descripcion: "",
    };

    const validationSchema = {

        nombre: Yup.string().required().max(50, t("General_Sec0_Error_LargoMaxExcedidoConEsperado", [50])),
        descripcion: Yup.string().required().max(200, t("General_Sec0_Error_LargoMaxExcedidoConEsperado", [200])),
        isEnabled: Yup.boolean(),
    };

    const onBeforeInitialize = (context, form, data, nexus) => {
        data.parameters = [{ id: "REG700_DETALLES_NU_RECORRIDO", value: props.numeroRecorrido }];
    }

    const onBeforeSubmit = (context, form, data, nexus) => {
        data.parameters = [{ id: "REG700_DETALLES_NU_RECORRIDO", value: props.numeroRecorrido }];
    }

    const onBeforeValidateField = (context, form, data, nexus) => {
        data.parameters = [{ id: "REG700_DETALLES_NU_RECORRIDO", value: props.numeroRecorrido }];
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        if (context.responseStatus === "OK") {

            handleClose();
        }
    }

    return (

        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">

            <Form
                application="REG700Update"
                id="REG700Update_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={onBeforeInitialize}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeValidateField={onBeforeValidateField}

            >

                <Modal.Header closeButton>
                    <Modal.Title>{t("REG700Update_Sec0_mdlCreate_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>

                    <Container fluid>
                        <Row>
                            <Col>
                                <Row>

                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="idRecorrido">{t("REG700_frm1_lbl_idRecorrido")}</label>
                                            <Field name="idRecorrido" />
                                            <StatusMessage for="idRecorrido" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="nombre">{t("REG700_frm1_lbl_nombre")}</label>
                                            <Field name="nombre" maxLength="50" />
                                            <StatusMessage for="nombre" />
                                        </div>
                                    </Col>
                                    
                                </Row>

                                <Row>

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
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}>{t("REG700_frm1_btn_CANCELAR")}</Button>
                    <SubmitButton id="btnSubmit" variant="primary" label="REG700Update_frm1_btn_GUARDAR" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}

export const REG700UpdateRecorridoModal = withPageContext(InternalREG700UpdateRecorridoModal);