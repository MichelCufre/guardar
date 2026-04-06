import React, { useState } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs, FormGroup } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldCheckbox, FieldSelect, FieldSelectAsync, FieldDate, SubmitButton, FormButton, StatusMessage, FieldDateTime } from '../../components/FormComponents/Form';
import { notificationType } from '../../components/Enums';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';

function InternalREG080UpdatePuertaModal(props) {
    const { t } = useTranslation();

    const [editMode, setModeEdit] = useState(false);

    const initialValues = {
        CD_PORTA: "",
        DS_PORTA: "",
        CD_SITUACAO: "",
        ID_BLOQUE: "",
        NU_PREDIO: "",
        TP_PUERTA: "",
    };

    const validationSchema = {
        CD_PORTA: Yup.string().required(),
        DS_PORTA: Yup.string().nullable().max(30),
        CD_SITUACAO: Yup.string().required(),
        NU_PREDIO: Yup.string().nullable().max(15).required(),
        CD_EMPRESA: Yup.string().nullable().required(),
        ID_BLOQUE: Yup.string().nullable().max(1).required(),
        TP_PUERTA: Yup.string().nullable().max(10).required()
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [{ id: "idPuertaEmbarque", value: props.puerta }];
        if (props.puerta)
            setModeEdit(true);
    };

    const handleAfterInitialize = (context, form, query, nexus) => {

        if (props.puerta)
            setModeEdit(true);
    };

    const handleBeforeSubmit = (context, form, query, nexus) => {
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("REG080_grid_1").refresh();
            props.onHide();
        }
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Form
                id="REG080Update_form_1"
                application="REG080Mantenimiento"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeInitialize={handleFormBeforeInitialize}
                onBeforeSubmit={handleBeforeSubmit}
                onAfterSubmit={handleFormAfterSubmit}
                onAfterInitialize={handleAfterInitialize}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("REG080_Sec0_modalTitle_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div>
                        <Row>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="CD_PORTA">{t("REG080_frm1_lbl_CD_PORTA")}</label>
                                    <Field name="CD_PORTA" />
                                    <StatusMessage for="CD_PORTA" />
                                </FormGroup>
                            </Col>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="DS_PORTA">{t("REG080_frm1_lbl_DS_PORTA")}</label>
                                    <Field name="DS_PORTA" />
                                    <StatusMessage for="DS_PORTA" />
                                </FormGroup>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="CD_SITUACAO">{t("REG080_frm1_lbl_CD_SITUACAO")}</label>
                                    <FieldSelect name="CD_SITUACAO" />
                                    <StatusMessage for="CD_SITUACAO" />
                                </FormGroup>
                            </Col>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="ID_BLOQUE">{t("REG080_frm1_lbl_ID_BLOQUE")}</label>
                                    <Field name="ID_BLOQUE" />
                                    <StatusMessage for="ID_BLOQUE" />
                                </FormGroup>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="TP_PUERTA">{t("REG080_frm1_lbl_TP_PUERTA")}</label>
                                    <FieldSelect name="TP_PUERTA" />
                                    <StatusMessage for="TP_PUERTA" />
                                </FormGroup>
                            </Col>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="NU_PREDIO">{t("REG080_frm1_lbl_NU_PREDIO")}</label>
                                    <FieldSelect name="NU_PREDIO" />
                                    <StatusMessage for="NU_PREDIO" />
                                </FormGroup>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="CD_EMPRESA">{t("REG080_frm1_lbl_CD_EMPRESA")}</label>
                                    <FieldSelectAsync name="CD_EMPRESA" />
                                    <StatusMessage for="CD_EMPRESA" />
                                </FormGroup>
                            </Col>
                            <Col style={{ display: editMode ? 'block' : 'none' }}>
                                <FormGroup>
                                    <label htmlFor="CD_ENDERECO">{t("REG080_frm1_lbl_CD_ENDERECO")}</label>
                                    <Field name="CD_ENDERECO" />
                                    <StatusMessage for="CD_ENDERECO" />
                                </FormGroup>
                            </Col>
                        </Row>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <SubmitButton id="btnConfirmar" label="General_Sec0_btn_Confirmar" />
                    &nbsp;
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("General_Sec0_btn_Cancelar")} </Button>
                </Modal.Footer>
            </Form>
        </Modal>
    )
}

export const REG080UpdatePuerta = withPageContext(InternalREG080UpdatePuertaModal);