import React, { useState } from 'react';
import { Modal, Button, Row, Col} from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldSelect, FieldSelectAsync, StatusMessage, FieldToggle, SubmitButton } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { notificationType } from '../../components/Enums';

function InternalPRD110CreateIngresoModal(props) {
    const { t } = useTranslation();
    
    const [isModalConFormula, setIsModalConFormula] = useState("hidden");
    const [isConFormula, setIsConFormula] = useState("false");

    const initialValues = {
        cdFormula: "",
        dsFormula: "",
        tpIngreso: "",
        qtFormula: "",
        predio: "",
        ingresoConFormula: "",
        generarPedido: "",
        empresa: "",
        idExterno: "",
    };

    const validationSchema = {
        cdFormula: Yup.string(),
        dsFormula: Yup.string(),
        tpIngreso: Yup.string(),
        qtFormula: Yup.string(),
        predio: Yup.string(),
        ingresoConFormula: Yup.string(),
        generarPedido: Yup.string(),
        empresa: Yup.string(),
        idExterno: Yup.string(),
    };

    const handleClose = () => {
        props.onHide();
    };



    const onAfterValidateField = (context, form, query, nexus) => {

        const isConFormula = query.parameters.find(p => p.id === "ingresoConFormula");

        if (isConFormula != null) {
            if (isConFormula.value === "true") {
                form.fields.find(w => w.id == "ingresoConFormula").value = true;
                setIsModalConFormula("");
            }
            else {
                setIsModalConFormula("hidden");
            }

            setIsConFormula(isConFormula.value)
        }

    };

    const handleFormAfterSubmit = (context, form, query, nexus) => {

        if (context.responseStatus === "OK") {
            props.onHide();
        }
    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {

        if (isConFormula == "true") {
            query.parameters = [
                { id: "empresa", value: nexus.getForm("PRD110CrearIngreso").getFieldValue("empresa") },
                { id: "tipoIngreso", value: nexus.getForm("PRD110CrearIngreso").getFieldValue("tpIngreso") },
                { id: "codigoFormula", value: nexus.getForm("PRD110CrearIngreso").getFieldValue("cdFormula") },
                { id: "isConFormula", value: isConFormula },
                { id: "isSubmit", value: true },
            ];
        } else {
            if (nexus.getGrid("PRD110CrearIngreso_grid_1").hasError()) {
                context.abortServerCall = true;
                nexus.toast(notificationType.error, "PRD110_form1_error_LineasEntradaErrores");
                return false;
            }

            if (nexus.getGrid("PRD110CrearIngreso_grid_2").hasError()) {
                context.abortServerCall = true;
                nexus.toast(notificationType.error, "PRD110_form1_error_LineasSalidaErrores");
                return false;
            }

            if (nexus.getGrid("PRD110CrearIngreso_grid_1").getModifiedRows().length === 0) {
                context.abortServerCall = true;
                nexus.toast(notificationType.error, "PRD110_form1_error_LineasEntradaFaltantes");
                return false;
            }

            if (nexus.getGrid("PRD110CrearIngreso_grid_1").getModifiedRows().length === 0) {
                context.abortServerCall = true;
                nexus.toast(notificationType.error, "PRD110_form1_error_LineasSalidaFaltantes");
                return false;
            }

            const rowsEntrada = nexus.getGrid("PRD110CrearIngreso_grid_1").getModifiedRows();
            const rowsSalida = nexus.getGrid("PRD110CrearIngreso_grid_2").getModifiedRows();

            query.parameters = [
                { id: "rowsEntrada", value: JSON.stringify(rowsEntrada) },
                { id: "rowsSalida", value: JSON.stringify(rowsSalida) },
                { id: "empresa", value: nexus.getForm("PRD110CrearIngreso").getFieldValue("empresa") },
                { id: "tipoIngreso", value: nexus.getForm("PRD110CrearIngreso").getFieldValue("tpIngreso") },
                { id: "codigoFormula", value: nexus.getForm("PRD110CrearIngreso").getFieldValue("cdFormula") },
                { id: "isConFormula", value: isConFormula },
                { id: "isSubmit", value: true },
            ];
        }
    }

    const handleGridBeforeSelectSearch = (context, grid, query, nexus) => {
        query.parameters = [
            { id: "empresa", value: nexus.getForm("PRD110CrearIngreso").getFieldValue("empresa") },
        ];
    }

    const handleGridBeforeValidate = (context, data, nexus) => {

        const rowsEntrada = nexus.getGrid("PRD110CrearIngreso_grid_1").getModifiedRows();

        data.parameters = [
            { id: "rowsEntrada", value: JSON.stringify(rowsEntrada) },
            { id: "empresa", value: nexus.getForm("PRD110CrearIngreso").getFieldValue("empresa") }
        ];
    }


    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w">
            <Page
                {...props}
                application="PRD110CrearIngreso"
            >
                <Form

                    application="PRD110CrearIngreso"
                    id="PRD110CrearIngreso"
                    initialValues={initialValues}
                    validationSchema={validationSchema}
                    onAfterSubmit={handleFormAfterSubmit}
                    onAfterValidateField={onAfterValidateField}
                    onBeforeSubmit={handleFormBeforeSubmit}
                >
                    <Modal.Header closeButton>
                        <Modal.Title>{t("PRD110_Sec0_modalTitle_Titulo")}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Row>
                            <Col>

                                <Row>
                                    <Col md={3}>
                                        <div className="form-group" >
                                            <label htmlFor="empresa">{t("PRD110_form1_label_Empresa")}</label>
                                            <FieldSelectAsync name="empresa" />
                                            <StatusMessage for="empresa" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="predio">{t("PRD110_form1_label_Predio")}</label>
                                            <FieldSelect name="predio" />
                                            <StatusMessage for="predio" />
                                        </div>
                                    </Col>
                                    <Col>
                                    </Col>
                                    <Col>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col>
                                        <div className="form-group" >
                                            <label htmlFor="tpIngreso">{t("PRD110_frm1_lbl_ND_TIPO")}</label>
                                            <FieldSelect name="tpIngreso" />
                                            <StatusMessage for="tpIngreso" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" style={{ marginTop: "30px" }}>
                                            <FieldToggle name="ingresoConFormula" label={t("PRD110_frm1_lbl_ingresoConFormula")} />
                                            <StatusMessage for="ingresoConFormula" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" style={{ marginTop: "30px" }}>
                                            <FieldToggle name="generarPedido" label={t("PRD110_frm1_lbl_generarPedido")} />
                                            <StatusMessage for="generarPedido" />
                                        </div>
                                    </Col>
                                    <Col>
                                        <div className="form-group" style={{ marginTop: "30px" }}>
                                            <FieldToggle name="generaPreparacion" label={t("PRD112_form1_field_generaPreparacion")} />
                                            <StatusMessage for="generaPreparacion" />
                                        </div>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col className={isModalConFormula}>
                                        <div className="form-group" >
                                            <label htmlFor="cdFormula">{t("PRD110_frm1_lbl_CD_PRDC_DEFINICION")}</label>
                                            <FieldSelectAsync name="cdFormula" />
                                            <StatusMessage for="cdFormula" />
                                        </div>
                                    </Col>
                                    <Col className={isModalConFormula}>
                                        <div className="form-group" >
                                            <label htmlFor="dsFormula">{t("PRD110_frm1_lbl_DS_DEFINICION")}</label>
                                            <Field name="dsFormula" readOnly />
                                            <StatusMessage for="dsFormula" />
                                        </div>
                                    </Col>
                                    <Col className={isModalConFormula}>
                                        <div className="form-group" >
                                            <label htmlFor="qtFormula">{t("PRD110_frm1_lbl_QT_FORMULA")}</label>
                                            <Field name="qtFormula" />
                                            <StatusMessage for="qtFormula" />
                                        </div>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col md={4}>
                                        <div className="form-group" >
                                            <label htmlFor="idExterno">{t("PRD110_form1_label_IdExterno")}</label>
                                            <Field name="idExterno" />
                                            <StatusMessage for="idExterno" />
                                        </div>
                                    </Col>
                                </Row>
                                <div className={isModalConFormula ? "" : "hidden"}>
                                    <Row>
                                        <Col span={6} style={{ maxWidth: "50%" }}>
                                            <h2>{t("PRD110_form1_title_Entrada")}</h2>
                                            <Grid
                                                application="PRD110CrearIngreso"
                                                id="PRD110CrearIngreso_grid_1"
                                                rowsToFetch={16}
                                                rowsToDisplay={8}
                                                onBeforeSelectSearch={handleGridBeforeSelectSearch}
                                                onBeforeValidateRow={handleGridBeforeValidate}

                                            />
                                        </Col>
                                        <Col span={6} style={{ maxWidth: "50%" }}>
                                            <h2>{t("PRD110_form1_title_Salida")}</h2>
                                            <Grid
                                                application="PRD110CrearIngreso"
                                                id="PRD110CrearIngreso_grid_2"
                                                rowsToFetch={16}
                                                rowsToDisplay={8}
                                                onBeforeSelectSearch={handleGridBeforeSelectSearch}
                                                onBeforeValidateRow={handleGridBeforeValidate}
                                            />
                                        </Col>
                                    </Row>
                                </div >
                            </Col>
                        </Row>

                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="danger" onClick={handleClose}>
                            {t("PRD110_frm1_btn_Cancelar")}
                        </Button>
                        <SubmitButton id="btnSubmitConfirmar" variant="primary" label="PRD110_frm1_btn_Confirmar" />

                    </Modal.Footer>
                </Form>
            </Page >
        </Modal >
    );
}

export const PRD110CreateIngresoModal = withPageContext(InternalPRD110CreateIngresoModal);