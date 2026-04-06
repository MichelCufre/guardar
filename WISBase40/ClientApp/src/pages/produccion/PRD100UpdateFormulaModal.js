import React from 'react';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldTextArea, StatusMessage } from '../../components/FormComponents/Form';
import { notificationType } from '../../components/Enums';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';

export function InternalPRD100UpdateFormulaModal(props) {
    const { t } = useTranslation();

    const initialValues = {
        codigo: "",
        nombre: "",
        descripcion: "",
        empresa: ""
    };

    const validationSchema = {
        codigo: Yup.string(),
        nombre: Yup.string(),
        descripcion: Yup.string(),
        empresa: Yup.number()
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [{ id: "formula", value: props.formula }];
    }

    const handleSubmit = () => {
        props.nexus.getForm("PRD100Update_form_1").submit();
    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        console.log("entra submit");

        if (nexus.getGrid("PRD101_grid_1").hasError() || nexus.getGrid("PRD101_grid_2").hasError()) {
            context.abortServerCall = true;

            nexus.toast(notificationType.error, "Hay errores en las lineas de entrada o salida, no se puede crear formula. Corrija los errores antes de continuar");

            return false;
        }

        const rowsEntrada = nexus.getGrid("PRD101_grid_1").getModifiedRows();
        const rowsSalida = nexus.getGrid("PRD101_grid_2").getModifiedRows();

        query.parameters = [
            { id: "formula", value: props.formula },
            { id: "rowsEntrada", value: JSON.stringify(rowsEntrada) },
            { id: "rowsSalida", value: JSON.stringify(rowsSalida) }
        ];
    }
    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("PRD101_grid_1").refresh();
            nexus.getGrid("PRD101_grid_2").refresh();

            handleClose();
        }
        else {
            const rowsEntrada = query.parameters.find(d => d.id === "rowsEntrada").value;
            const rowsSalida = query.parameters.find(d => d.id === "rowsSalida").value;

            if(rowsEntrada)
                nexus.getGrid("PRD101_grid_1").updateRows(JSON.parse(rowsEntrada));

            if(rowsSalida)
                nexus.getGrid("PRD101_grid_2").updateRows(JSON.parse(rowsSalida));

            context.showErrorMessage = true;
        }
    }

    const addParams = (context, data, nexus) => {
        data.parameters = [{ id: "formula", value: props.formula }];
    }

    const handleGridBeforeSelectSearch = (context, grid, query, nexus) => {
        query.parameters = [
            { id: "formula", value: props.formula },
            { id: "empresa", value: nexus.getForm("PRD100Update_form_1").getFieldValue("empresa") }
        ];
    }

    const onAfterCommit = (context, rows, data, nexus) => {
        nexus.getGrid("PRD101_grid_1").refresh();
        nexus.getGrid("PRD101_grid_2").refresh();

    };

    const handleGridBeforeValidate = (context, data, nexus) => {
        data.parameters = [
            { id: "formula", value: props.formula },
            { id: "empresa", value: nexus.getForm("PRD100Update_form_1").getFieldValue("empresa") }
        ];
    }

    const handleGridBeforeCommit = (context, data, nexus) => {
        data.parameters = [
            { id: "formula", value: props.formula },
            { id: "empresa", value: nexus.getForm("PRD100Update_form_1").getFieldValue("empresa") },
        ];
    }


    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRD100_form1_title_ModificarFormula")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Row>
                    <Col>
                        <Form
                            application="PRD100Update"
                            id="PRD100Update_form_1"
                            initialValues={initialValues}
                            validationSchema={validationSchema}
                            onBeforeInitialize={handleFormBeforeInitialize}
                            onBeforeSubmit={handleFormBeforeSubmit}
                            onAfterSubmit={handleFormAfterSubmit}
                        >
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="codigo">{t("PRD100_form1_label_Codigo")}</label>
                                        <Field name="codigo" readOnly />
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
                                        <Field name="empresa" readOnly />
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
                            <Tab eventKey="detail" title={t("PRD100_form1_tab_EntradaSalida")}>
                                <h2>{t("PRD100_form1_title_Insumo")}</h2>
                                <Grid
                                    application="PRD101"
                                    id="PRD101_grid_1"
                                    rowsToFetch={16}
                                    rowsToDisplay={8}
                                    enableExcelExport
                                    enableExcelImport={false}
                                    onBeforeExportExcel={addParams}
                                    onBeforeInitialize={addParams}
                                    onBeforeFetch={addParams}
                                    onBeforeApplyFilter={addParams}
                                    onBeforeApplySort={addParams}
                                    onBeforeValidateRow={handleGridBeforeValidate}
                                    onBeforeSelectSearch={handleGridBeforeSelectSearch}
                                    onBeforeCommit={handleGridBeforeCommit}
                                    onAfterCommit={onAfterCommit}
                                />
                                <h2>{t("PRD100_form1_title_Producido")}</h2>
                                <Grid
                                    application="PRD101"
                                    id="PRD101_grid_2"
                                    rowsToFetch={16}
                                    rowsToDisplay={8}
                                    enableExcelExport
                                    enableExcelImport={false}
                                    onBeforeExportExcel={addParams}
                                    onBeforeInitialize={addParams}
                                    onBeforeFetch={addParams}
                                    onBeforeApplyFilter={addParams}
                                    onBeforeApplySort={addParams}
                                    onBeforeValidateRow={handleGridBeforeValidate}
                                    onBeforeSelectSearch={handleGridBeforeSelectSearch}
                                    onBeforeCommit={handleGridBeforeCommit}
                                    onAfterCommit={onAfterCommit}
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
                    {t("PRD100_form1_btn_Guardar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const PRD100UpdateFormulaModal = withPageContext(InternalPRD100UpdateFormulaModal);
