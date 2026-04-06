import $ from 'jquery';
import React from 'react';
import { Button, Col, FormGroup, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { FieldNumber, FieldSelect, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { withPageContext } from '../../components/WithPageContext';

export function InternalPRE810EditarPonderadorFechaEntrega(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const initialValues = {
        dias: "",
        horas: "",
        operador: "",
        ponderacion: "",
        diasHasta: "0",
        horasHasta: "0",
    };

    const validationSchema = {
        dias: Yup.string(),
        horas: Yup.string(),
        operador: Yup.string(),
        ponderacion: Yup.string(),

        diasHasta: Yup.string(),
        horasHasta: Yup.string(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const GridOnAfterButtonAction = (data, nexus) => {
        nexus.getForm("PRE810FechaEntrega_form_1").reset(data.parameters);
    };

    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("PRE810PonderadorFechaEntrega_grid_1").refresh();
    }

    const addParameters = (context, data, nexus) => {
        data.parameters = [
            {
                id: "nuColaDeTrabajo",
                value: props.nuColaDeTrabajo
            },
        ];
    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            {
                id: "nuColaDeTrabajo",
                value: props.nuColaDeTrabajo
            },
        ];
    };

    const onAfterSubmit = (context, form, query, nexus) => {
        if (context.status === "ERROR")
            return false;
        nexus.getGrid("PRE810PonderadorFechaEntrega_grid_1").reset();
        nexus.getForm("PRE810FechaEntrega_form_1").reset();
    };

    const GridOnBeforeButtonAction = (context, data, nexus) => {

        if (data.buttonId === "btnEditar") {

            data.parameters = [
                {
                    id: "nuColaDeTrabajo",
                    value: props.nuColaDeTrabajo
                },
            ];
        }
    };

    const onAfterValidateField = (context, form, query, nexus) => {
        var operador = query.parameters.find(s => s.id == "operacionSelect")
        if (operador) {
            if (operador.value != "entre") {
                $("#labelDiasDesde").hide();
                $("#labelHorasDesde").hide();
                $("#labelDiasHasta").hide();
                $("#diasHasta").hide();
                $("#labelHorasHasta").hide();
                $("#horasHasta").hide();
                $("#btnCancelar").hide();
                $("#labelDias").show();
                $("#dias").show();
                $("#labelHoras").show();
                $("#horas").show();
                nexus.getForm("PRE810FechaEntrega_form_1").setFieldValue("diasHasta", "");
                nexus.getForm("PRE810FechaEntrega_form_1").setFieldValue("horasHasta", "");
            }
            else {
                $("#labelDias").hide();
                $("#labelHoras").hide();
                $("#labelDiasDesde").show();
                $("#labelHorasDesde").show();
                $("#labelDiasHasta").show();
                $("#diasHasta").show();
                $("#labelHorasHasta").show();
                $("#horasHasta").show();
                $("#btnCancelar").hide();
            }
        }
    }

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        var operador = parameters.parameters.find(s => s.id == "operadorEntrega");
        if (operador) {
            if (operador.value != "entre") {
                $("#labelDiasDesde").hide();
                $("#labelHorasDesde").hide();
                $("#labelDiasHasta").hide();
                $("#diasHasta").hide();
                $("#labelHorasHasta").hide();
                $("#horasHasta").hide();
                $("#btnCancelar").show();
                $("#labelDias").show();
                $("#dias").show();
                $("#labelHoras").show();
                $("#horas").show();
            }
            else {
                $("#labelDias").hide();
                $("#labelHoras").hide();
                $("#labelDiasDesde").show();
                $("#labelHorasDesde").show();
                $("#labelDiasHasta").show();
                $("#diasHasta").show();
                $("#labelHorasHasta").show();
                $("#horasHasta").show();
                $("#btnCancelar").show();
            }
        }
        else {
            $("#labelDiasDesde").hide();
            $("#labelHorasDesde").hide();
            $("#labelDiasHasta").hide();
            $("#diasHasta").hide();
            $("#labelHorasHasta").hide();
            $("#horasHasta").hide();
            $("#btnCancelar").hide();
            $("#labelDias").show();
            $("#dias").show();
            $("#labelHoras").show();
            $("#horas").show();
        }
    };

    const handleCancel = () => {
        $('#PRE810FechaEntrega_form_1').trigger("reset");
    };

    return (
        <Modal dialogClassName="modal-50w" show={props.show} onHide={props.onHide} >
            <Modal.Header closeButton>
                <Modal.Title>{t("PRE810_Sec0_lbl_Title_DT_ENTREGA")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    load
                    {...props}
                >
                    <Row>
                        <Col>
                            <Form
                                id="PRE810FechaEntrega_form_1"
                                application="PRE810PonderadorFechaEntrega"
                                initialValues={initialValues}
                                validationSchema={validationSchema}
                                onBeforeSubmit={onBeforeSubmit}
                                onAfterSubmit={onAfterSubmit}
                                onAfterValidateField={onAfterValidateField}
                                onAfterInitialize={onAfterInitialize}
                            >
                                <Row>
                                    <Col lg="2">
                                        <FormGroup>
                                            <label id="labelDiasDesde" style={{ fontWeight: "bold" }} htmlFor="dias" >{t("PRE810_frm1_lbl_diasDesde")}</label>
                                            <label id="labelDias" style={{ fontWeight: "bold" }} htmlFor="dias">{t("PRE810_frm1_lbl_dias")}</label>
                                            <FieldNumber id="dias" name="dias" />
                                            <StatusMessage for="dias" />
                                        </FormGroup>
                                    </Col>
                                    <Col lg="2">
                                        <FormGroup>
                                            <label id="labelHorasDesde" style={{ fontWeight: "bold" }} htmlFor="horas" >{t("PRE810_frm1_lbl_horasDesde")}</label>
                                            <label id="labelHoras" style={{ fontWeight: "bold" }} htmlFor="horas">{t("PRE810_frm1_lbl_horas")}</label>
                                            <FieldNumber id="horas" name="horas" />
                                            <StatusMessage for="horas" />
                                        </FormGroup>
                                    </Col>
                                    <Col lg="2">
                                        <FormGroup>
                                            <label id="labelDiasHasta" style={{ fontWeight: "bold" }} htmlFor="dias" >{t("PRE810_frm1_lbl_diasHasta")}</label>
                                            <FieldNumber id="diasHasta" name="diasHasta" />
                                            <StatusMessage for="diasHasta" />
                                        </FormGroup>
                                    </Col>
                                    <Col lg="2">
                                        <FormGroup>
                                            <label id="labelHorasHasta" style={{ fontWeight: "bold" }} htmlFor="horas" >{t("PRE810_frm1_lbl_horasHasta")}</label>
                                            <FieldNumber id="horasHasta" name="horasHasta" />
                                            <StatusMessage for="horasHasta" />
                                        </FormGroup>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col lg="3">
                                        <FormGroup>
                                            <label style={{ fontWeight: "bold" }} htmlFor="operador">{t("PRE810_frm1_lbl_operacion")}</label>
                                            <FieldSelect id="operador" name="operador" />
                                            <StatusMessage for="operador" />
                                        </FormGroup>
                                    </Col>
                                    <Col lg="3">
                                        <FormGroup>
                                            <label style={{ fontWeight: "bold" }} htmlFor="ponderacion">{t("PRE810_frm1_lbl_ponderacion")}</label>
                                            <FieldNumber name="ponderacion" />
                                            <StatusMessage for="ponderacion" />
                                        </FormGroup>
                                    </Col>
                                    <Col lg="2" className="mt-2">
                                        <FormGroup className="mt-4">
                                            <SubmitButton id="btnGuardar" variant="primary" label="PRE810_frm1_btn_DtEntregaGuardar" />
                                        </FormGroup>
                                    </Col>
                                    <Col lg="2" className="mt-2">
                                        <FormGroup className="mt-4">
                                            <SubmitButton id="btnCancelar" onClick={handleCancel} variant="secondary" label="PRE810_frm1_btn_DtEntregaCancelar" />
                                        </FormGroup>
                                    </Col>
                                </Row>
                            </Form>
                        </Col>
                    </Row>

                    <br />
                    <Grid
                        id="PRE810PonderadorFechaEntrega_grid_1"
                        application="PRE810PonderadorFechaEntrega"
                        rowsToFetch={30}
                        rowsToDisplay={10}
                        onBeforeInitialize={addParameters}
                        onAfterButtonAction={GridOnAfterButtonAction}
                        onBeforeFetch={addParameters}
                        onBeforeButtonAction={GridOnBeforeButtonAction}
                        onBeforeApplyFilter={addParameters}
                        onBeforeApplySort={addParameters}
                        onAfterCommit={onAfterCommit}
                        onBeforeCommit={addParameters}
                        onBeforeFetchStats={addParameters}
                    />
                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="outline-secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_Cerrar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const PRE810EditarPonderadorFechaEntrega = withPageContext(InternalPRE810EditarPonderadorFechaEntrega);