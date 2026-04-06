import React from 'react';
import { Button, Col, Container, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { FieldSelect, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { withPageContext } from '../../components/WithPageContext';

function InternalPRE812AsignarFuncCola(props) {

    const { t } = useTranslation();

    const validationSchema = {

        usuario: Yup.string().required(),
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            props.onHide(props.nexus);
        }
    }

    const onBeforeSubmit = (context, form, query, nexus) => {
        if (props.rowSeleccionadas) {
            query.parameters = [{ id: "ListaFilasSeleccionadas", value: props.rowSeleccionadas }];
        }
        query.parameters.push({ id: "isSubmit", value: true });
    }
    const onBeforeValidateField = (context, form, query, nexus) => {
        if (props.rowSeleccionadas) {
            query.parameters = [{ id: "ListaFilasSeleccionadas", value: props.rowSeleccionadas }];
        }
    }

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                application="PRE812AsignarFuncCola"
                id="PRE812AF_form_1"
                validationSchema={validationSchema}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
                onBeforeValidateField={onBeforeValidateField}
            >

                <Modal.Header closeButton>
                    <Modal.Title>{t("PRE812AF_Sec0_mdl_asignar_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Container fluid>
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="usuario">{t("PRE812AF_frm1_lbl_usuario")}</label>
                                    <FieldSelect name="usuario" />
                                    <StatusMessage for="usuario" />
                                </div>
                            </Col>
                        </Row>
                    </Container>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("PRE812AF_frm1_btn_cerrar")} </Button>
                    <SubmitButton id="btnSubmitConfirmar" variant="primary" label="PRE812AF_frm1_btn_confirmar" />
                </Modal.Footer>
            </Form>
        </Modal >
    );
}

export const PRE812AsignarFuncCola = withPageContext(InternalPRE812AsignarFuncCola);