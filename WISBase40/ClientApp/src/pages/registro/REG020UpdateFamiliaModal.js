import React, { useState } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldNumber } from '../../components/FormComponents/Form';
import { notificationType } from '../../components/Enums';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';

function InternalREG020UpdateFamiliaModal(props) {
    const { t } = useTranslation();

    const initialValues = {
        codigoFamilia: "",
        descripcionFamilia: ""
    };

    const validationSchema = {
        codigoFamilia: Yup.string().required().max(10),
        descripcionFamilia: Yup.string().required().max(100),
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [{ id: "keyCodigoFamilia", value: props.familia }];
    }


    const handleFormBeforeSubmit = (context, form, query, nexus) => {
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("REG020_grid_1").refresh();
            props.onHide();
        }
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                id="REG020Update_form_1"
                application="REG020"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={handleFormBeforeInitialize}
                onBeforeSubmit={handleFormBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("REG020_Sec0_modalTitle_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>                        
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="codigoFamilia">{t("REG020_frm1_lbl_codigoFamilia")}</label>
                                <Field name="codigoFamilia" />
                                <StatusMessage for="codigoFamilia" />
                            </div>
                            <div className="form-group">
                                <label htmlFor="descripcionFamilia">{t("REG020_frm1_lbl_descripcionFamilia")}</label>
                                <FieldTextArea name="descripcionFamilia" />
                                <StatusMessage for="descripcionFamilia" />
                            </div>
                        </Col>
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                        {t("REG020_frm1_btn_CANCELAR")}
                    </Button>
                    <SubmitButton id="btnSubmitUpdateFamilia" variant="primary" label="REG020_frm1_btn_EDITAR" />
                </Modal.Footer>
            </Form>
        </Modal>
    );
}

export const REG020UpdateFamiliaModal = withPageContext(InternalREG020UpdateFamiliaModal);