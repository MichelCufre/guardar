import React from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import * as Yup from 'yup';

function InternalSEG020CreateModal(props) {

    const { t } = useTranslation();

    const validationSchema = {

        descripcion: Yup.string().required()
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            if (query.buttonId == "btnSubmitConfirmarAsignar") {
                props.onHide(query.parameters, null, props.nexus);

            } else {
                props.onHide(null, null, props.nexus);
            }
        }
    }

    const handleFormBeforeInitialize = (context, form, query, nexus) => {

        if (props.usuario) {

            let parameters =
                [
                    { id: "idPerfil", value: props.usuario.find(x => x.id === "idPerfil").value }

                ];
            query.parameters = parameters;
        }
    }

    const handleClose = () => {
        props.onHide(null, null, props.nexus);
    };

    return (

        <Form
            application="SEG020"
            id="SEG020_form_1"
            validationSchema={validationSchema}
            onAfterSubmit={handleFormAfterSubmit}
            onBeforeInitialize={handleFormBeforeInitialize}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("SEG020_Sec0_mdlCreate_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <div className="form-group" >
                        <label htmlFor="idPerfil">{t("SEG020_frm1_lbl_idPerfil")}</label>
                        <Field name="idPerfil" readOnly />
                        <StatusMessage for="idPerfil" />
                    </div>
                    <div className="form-group" >
                        <label htmlFor="descripcion">{t("SEG020_frm1_lbl_descripcion")}</label>
                        <Field name="descripcion" />
                        <StatusMessage for="descripcion" />
                    </div>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("SEG020_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmar" variant="primary" label="SEG020_frm1_btn_confirmar" />
                <SubmitButton id="btnSubmitConfirmarAsignar" variant="primary" label="SEG020_frm1_btn_confirmarAsignar" />
            </Modal.Footer>
        </Form>
    );
}

export const SEG020CreateModal = withPageContext(InternalSEG020CreateModal);