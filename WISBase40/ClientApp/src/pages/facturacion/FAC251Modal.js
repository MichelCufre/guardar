import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldCheckbox, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import * as Yup from 'yup';
import { FieldTime } from '../../components/FormComponents/FormFieldTime';


function InternalFAC251Modal(props) {

    const { t } = useTranslation();

    const [showHoraHasta, setShowHoraHasta] = useState(false);

    const [editMode, setEditMode] = useState(false);

    const initialValues = {

        codigoFactura: "",
        codigoProceso: "",
        descripcionProceso: "",
        componente: "",
        numeroCuentaContable: "",
    };

    const validationSchema = {
        codigoFactura: Yup.string().required(),
        codigoProceso: Yup.string().required(),
        descripcionProceso: Yup.string().required(),
        componente: Yup.string().required(),
        numeroCuentaContable: Yup.string().required(),
    };

    const handleClose = () => {
        props.onHide(props.nexus);
    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        context.showErrorMessage = true;

        if (context.responseStatus === "OK") {
            nexus.getForm("FAC251_form_1").reset();
            props.onHide(props.nexus);
        }
    }

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        if (props.template) {

            let parameters =
                [
                    { id: "codigoProceso", value: props.template.find(x => x.id === "codigoProceso").value },
                ];

            query.parameters = parameters;
            setEditMode(true);
        } else {
            setEditMode(false);
        }
    };

    const onAfterValidateField = (context, form, query, nexus) => {
        if (query.fieldId === "parcial") {
            setShowHoraHasta(!showHoraHasta);
        }
    };

    return (

        <Form
            application="FAC251"
            id="FAC251_form_1"
            initialValues={initialValues}
            validationSchema={validationSchema}
            onAfterSubmit={handleFormAfterSubmit}
            onAfterValidateField={onAfterValidateField}
            onBeforeInitialize={handleFormBeforeInitialize}
        >
            <Modal.Header closeButton>
               <Modal.Title>{editMode ? t("FAC251_Sec0_mdlEdit_Titulo") : t("FAC251_Sec0_mdlCreate_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="codigoProceso">{t("FAC251_grid1_colname_CD_PROCESO")}</label>
                                <Field name="codigoProceso" />
                                <StatusMessage for="codigoProceso" />
                            </div>
                        </Col>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="descripcionProceso">{t("FAC251_grid1_colname_DS_PROCESO")}</label>
                                <Field name="descripcionProceso" />
                                <StatusMessage for="descripcionProceso" />
                            </div>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="codigoFactura">{t("FAC251_grid1_colname_NU_CD_FACTURACION")}</label>
                                <FieldSelect name="codigoFactura" />
                                <StatusMessage for="codigoFactura" />
                            </div>
                        </Col>
                        <Col>
                            <Row>
                                <Col sm={6}>
                                    <div className="form-group" >
                                        <label htmlFor="componente">{t("FAC251_grid1_colname_NU_COMPONENTE")}</label>
                                        <FieldSelect name="componente" />
                                        <StatusMessage for="componente" />
                                    </div>
                                </Col>
                                <Col sm={6}>
                                    <div className="form-group" >
                                        <label htmlFor="numeroCuentaContable">{t("FAC251_grid1_colname_NU_CUENTA_CONTABLE")}</label>
                                        <FieldSelect name="numeroCuentaContable" />
                                        <StatusMessage for="numeroCuentaContable" />
                                    </div>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("FAC251_frm1_btn_cerrar")} </Button>
                <SubmitButton id="btnSubmitConfirmar" variant="primary" label="FAC251_frm1_btn_confirmar" />

            </Modal.Footer>
        </Form>
    );
}

export const FAC251Modal = withPageContext(InternalFAC251Modal);