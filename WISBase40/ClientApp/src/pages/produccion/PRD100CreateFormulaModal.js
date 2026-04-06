import React from 'react';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldSelectAsync, FieldTextArea, StatusMessage } from '../../components/FormComponents/Form';
import { notificationType } from '../../components/Enums';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';


function InternalPRD100CreateFormulaModal(props) {
    const { t } = useTranslation();

    const initialValues = {
        codigo: "",
        nombre: "",
        descripcion: "",
        empresa: ""
    };

    const validationSchema = {
        codigo: Yup.string().required(),
        nombre: Yup.string().required(),
        descripcion: Yup.string().required(),
        empresa: Yup.string().required()
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleSubmit = () => {
        props.nexus.getForm("PRD100_form_1").submit();
    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        if (nexus.getGrid("PRD101_grid_1").hasError() || nexus.getGrid("PRD101_grid_2").hasError()) {
            context.abortServerCall = true;

            nexus.toast(notificationType.error, "PRD100_form1_error_ErroresEnLineasEntradaSalida");

            return false;
        }

        const rowsEntrada = nexus.getGrid("PRD101_grid_1").getModifiedRows();
        const rowsSalida = nexus.getGrid("PRD101_grid_2").getModifiedRows();

        query.parameters = [
            { id: "rowsEntrada", value: JSON.stringify(rowsEntrada) },
            { id: "rowsSalida", value: JSON.stringify(rowsSalida) }
        ];
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {

            nexus.getGrid("PRD100_grid_1").refresh();
            handleClose();
        }
    }

    const handleGridBeforeSelectSearch = (context, grid, query, nexus) => {
        query.parameters = [{ id: "empresa", value: nexus.getForm("PRD100_form_1").getFieldValue("empresa") }];
    }

    const handleGridBeforeValidate = (context, data, nexus) => {
        data.parameters = [
            { id: "empresa", value: nexus.getForm("PRD100_form_1").getFieldValue("empresa") }
        ];
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRD100_form1_title_CrearFormula")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Row>
                    <Col>
                        <Form
                            application="PRD100"
                            id="PRD100_form_1"
                            initialValues={initialValues}
                            validationSchema={validationSchema}
                            onBeforeSubmit={handleFormBeforeSubmit}
                            onAfterSubmit={handleFormAfterSubmit}
                        >
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="codigo">{t("PRD100_form1_label_Codigo")}</label>
                                        <Field name="codigo" />
                                        <StatusMessage for="codigo" />
                                    </div>
                                    <div className="form-group" >
                                        <label htmlFor="nombre">{t("PRD100_form1_label_Nombre")}</label>
                                        <Field name="nombre" />
                                        <StatusMessage for="nombre" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="empresa">{t("PRD100_form1_label_Empresa")}</label>
                                        <FieldSelectAsync name="empresa" />
                                        <StatusMessage for="empresa" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="descripcion">{t("PRD100_form1_label_Descripcion")}</label>
                                        <FieldTextArea name="descripcion" />
                                        <StatusMessage for="descripcion" />
                                    </div>
                                </Col>
                            </Row>
                        </Form>
                    </Col>
                </Row>
                <Row>
                    <Col>
                        <Tabs defaultActiveKey="detail" transition={false} id="noanim-tab-example">
                            <Tab eventKey="detail" title="Entrada/Salida">
                                <h2>{t("PRD100_form1_title_Insumo")}</h2>
                                <Grid
                                    application="PRD101"
                                    id="PRD101_grid_1"
                                    rowsToFetch={16}
                                    rowsToDisplay={8}
                                    onBeforeValidateRow={handleGridBeforeValidate}
                                    onBeforeSelectSearch={handleGridBeforeSelectSearch}
                                />
                                <h2>{t("PRD100_form1_title_Producido")}</h2>
                                <Grid
                                    application="PRD101"
                                    id="PRD101_grid_2"
                                    rowsToFetch={16}
                                    rowsToDisplay={8}
                                    onBeforeValidateRow={handleGridBeforeValidate}
                                    onBeforeSelectSearch={handleGridBeforeSelectSearch}
                                />
                            </Tab>
                        </Tabs>
                    </Col>
                </Row>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    {t("PRD100_form1_btn_Cancelar")}
                </Button>
                <Button variant="primary" onClick={handleSubmit}>
                    {t("PRD100_form1_btn_Crear")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const PRD100CreateFormulaModal = withPageContext(InternalPRD100CreateFormulaModal);