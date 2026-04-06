import $ from 'jquery';
import React from 'react';
import { Button, Col, FormGroup, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldNumber, FieldSelect, Form, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { withPageContext } from '../../components/WithPageContext';

export function InternalPRE810EditarPonderadorPonderacionGenerica(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const initialValues = {
        valor: "",
        valorHasta: " ",
        operador: "",
        ponderacion: "",
    };

    const validationSchema = {
        valor: Yup.string(),
        operador: Yup.string(),
        ponderacion: Yup.string(),
        valorHasta: Yup.string(),
    };

    const handleClose = () => {
        props.onHide();
    };

    const GridOnAfterButtonAction = (data, nexus) => {
        nexus.getForm("PRE810PonderacionGenerica").reset(data.parameters);
    };

    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("PRE810PonderadorGenerica_grid_1").refresh();
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
        nexus.getGrid("PRE810PonderadorGenerica_grid_1").reset();
        nexus.getForm("PRE810PonderadorGenerica").reset();
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
                $("#labelValorDesde").hide();
                $("#labelValorHasta").hide();
                $("#valorHasta").hide();
                $("#btnCancelar").hide();
                $("#labelValor").show();
                $("#valor").show();
                nexus.getForm("PRE810PonderacionGenerica").setFieldValue("valorHasta", "");
            }
            else {
                $("#labelValor").hide();
                $("#labelValorDesde").show();
                $("#labelValorHasta").show();
                $("#valorHasta").show();
                $("#btnCancelar").hide();
            }
        }
    }

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        var operador = parameters.parameters.find(s => s.id == "operadorEntrega");
        if (operador) {
            if (operador.value != "entre") {
                $("#labelValorDesde").hide();
                $("#labelValorHasta").hide();
                $("#valorHasta").hide();
                $("#btnCancelar").show();
                $("#labelValor").show();
                $("#valor").show();
            }
            else {
                $("#labelValor").hide();
                $("#labelValorDesde").show();
                $("#labelValorHasta").show();
                $("#valorHasta").show();
                $("#btnCancelar").show();
            }
        }
        else {
            $("#labelValorDesde").hide();
            $("#labelValorHasta").hide();
            $("#valorHasta").hide();
            $("#btnCancelar").hide();
            $("#labelValor").show();
            $("#valor").show();
        }
    };

    const handleCancel = () => {
        $('#PRE810PonderacionGenerica').trigger("reset");
    };

    return (
        <Modal dialogClassName="modal-50w" show={props.show} onHide={props.onHide} >
            <Modal.Header closeButton>
                <Modal.Title>{t("PRE810_Sec0_lbl_Title_VL_PONDERACION_GENERICA")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    load
                    {...props}
                >
                    <Row>
                        <Col>
                            <Form
                                id="PRE810PonderadorGenerica"
                                application="PRE810PonderadorGenerica"
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
                                            <label id="labelValorDesde" style={{ fontWeight: "bold" }} htmlFor="valor">{t("PRE810_frm1_lbl_valorDesde")}</label>
                                            <label id="labelValor" style={{ fontWeight: "bold" }} htmlFor="valor">{t("PRE810_frm1_lbl_valor")}</label>
                                            <Field id="valor" name="valor" />
                                            <StatusMessage for="valor" />
                                        </FormGroup>
                                    </Col>

                                    <Col lg="2">
                                        <FormGroup>
                                            <label id="labelValorHasta" style={{ fontWeight: "bold" }} htmlFor="valor">{t("PRE810_frm1_lbl_valorHasta")}</label>
                                            <Field id="valorHasta" name="valorHasta" />
                                            <StatusMessage for="valorHasta" />
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
                        id="PRE810PonderadorGenerica_grid_1"
                        application="PRE810PonderadorGenerica"
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

export const PRE810EditarPonderadorPonderacionGenerica = withPageContext(InternalPRE810EditarPonderadorPonderacionGenerica);