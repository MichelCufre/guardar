import React, { useState } from 'react';
import { Button, Col, Container, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { withPageContext } from '../../components/WithPageContext';

function InternalCOF060Modal(props) {

    const { t } = useTranslation();

    const [editMode, setEditMode] = useState(false);

    const validationSchema = {

        dsServidor: Yup.string().required(),
        dominioServidor: Yup.string(),
        clientId: Yup.string().required(),
    };

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        if (props.servidor) {

            let parameters =
                [
                    { id: "codigoServidor", value: props.servidor.find(x => x.id === "codigoServidor").value },
                ];

            query.parameters = parameters;
            setEditMode(true);
        } else {
            setEditMode(false);
        }
    };

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        if (props.servidor) {
            query.parameters = [{ id: "codigoServidor", value: props.servidor.find(x => x.id === "codigoServidor").value }];
        }
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            nexus.getForm("COF060_form_1").reset();
            props.onHide(props.nexus);
        }
    }

    return (

        <Form
            application="COF060"
            id="COF060_form_1"
            application="COF060"
            validationSchema={validationSchema}
            onBeforeSubmit={handleFormBeforeSubmit}
            onAfterSubmit={handleFormAfterSubmit}
            onBeforeInitialize={handleFormBeforeInitialize}
        >
            <Modal.Header closeButton>
                <Modal.Title>{editMode ? t("COF060_Sec0_mdlEdit_Titulo") : t("COF060_Sec0_mdlCreate_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="dsServidor">{t("COF060_frm1_lbl_dsServidor")}</label>
                                <Field name="dsServidor" />
                                <StatusMessage for="dsServidor" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="dominioServidor">{t("COF060_frm1_lbl_dominioServidor")}</label>
                                <Field name="dominioServidor" />
                                <StatusMessage for="dominioServidor" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="clientId">{t("COF060_frm1_lbl_clientId")}</label>
                                <Field name="clientId" />
                                <StatusMessage for="clientId" />
                            </div>
                        </Col>
                    </Row>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("COF060_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmar" variant="primary" label="COF060_frm1_btn_confirmar" />
            </Modal.Footer>
        </Form>
    );
}

export const COF060Modal = withPageContext(InternalCOF060Modal);