import React, { useState, useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, FormButton, FieldCheckbox, FieldSelect, Field, FieldSelectAsync, StatusMessage, FieldDateTime, SubmitButton } from '../../components/FormComponents/Form';
import { Row, Col, FormGroup } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';

export default function PRE680(props) {
    const { t } = useTranslation();


    const refFiltro = useRef(null);
    refFiltro.current = {
        DT_DESDE: "", CD_EMPRESA: "", NU_PREDIO: "", DT_HASTA: "", NU_PREDIO_STOCK_DEFAULT: "",
    };

    const initialValues = {
    };

    const validationSchema = {
        NU_PREDIO: Yup.string().required(),
        CD_EMPRESA: Yup.string().required(),
        DT_HASTA: Yup.string().required(),
        DT_DESDE: Yup.string().required(),
        NU_PREDIO_STOCK_DEFAULT: Yup.string(),
        cmbo_Redondiar: Yup.string(),
    };

    const addParameters = (context, data, nexus) => {

        if (data.parameters.length == 0) {

            data.parameters = [
                {
                    id: "FILTROS",
                    value: JSON.stringify(refFiltro.current)
                }
            ];
        }

    };

    const onAfterCommit = (context, rows, parameters, nexus) => {


        nexus.getForm("PRE680_form_1").reset();

        refFiltro.current = {
            DT_DESDE: "", CD_EMPRESA: "", NU_PREDIO: "", DT_HASTA: "", NU_PREDIO_STOCK_DEFAULT: "", cmbo_Redondiar :""
        };

    };


    const formOnBeforeInitialize = (context, form, query, nexus) => {

        query.parameters = [
            {
                id: "FILTROS",
                value: JSON.stringify(refFiltro.current)
            }
        ];
    }

    const onBeforeValidateField = (context, form, query, nexus) => {
        context.abortServerCall = true;
        context.forceUpdateFields = true;

        if (query.fieldId === "DT_DESDE") {
            refFiltro.current.DT_DESDE = form.fields.find(w => w.id === "DT_DESDE").value;
        }
        else if (query.fieldId === "CD_EMPRESA") {
            refFiltro.current.CD_EMPRESA = form.fields.find(w => w.id === "CD_EMPRESA").value;
        } else if (query.fieldId === "NU_PREDIO") {
            refFiltro.current.NU_PREDIO = form.fields.find(w => w.id === "NU_PREDIO").value;
        }
        else if (query.fieldId === "DT_HASTA") {
            refFiltro.current.DT_HASTA = form.fields.find(w => w.id === "DT_HASTA").value;
        }
        else if (query.fieldId === "NU_PREDIO_STOCK_DEFAULT") {
            refFiltro.current.NU_PREDIO_STOCK_DEFAULT = form.fields.find(w => w.id === "NU_PREDIO_STOCK_DEFAULT").value;
            var ctb = form.fields.find(w => w.id === "cmbo_Redondiar");
            if (refFiltro.current.NU_PREDIO_STOCK_DEFAULT === "") {
                ctb.disabled = true;

            }
            else {

                ctb.disabled = false;
            }
        }
        else if (query.fieldId === "cmbo_Redondiar") {
            refFiltro.current.cmbo_Redondiar = form.fields.find(w => w.id === "cmbo_Redondiar").value;
        }
    };

    const onBeforeSubmit = (context, form, query, nexus) => {

        query.parameters = [
            {
                id: "FILTROS",
                value: JSON.stringify(refFiltro.current)
            }
        ];
    }

    const onAfterSubmit = (context, form, query, nexus) => {
        nexus.getGrid("PRE680_grid_1").refresh();

    }

    const onAfterInitialize = (context, grid, parameters, nexus) => {


    };

    return (
        <Page
            title={t("PRE680_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <Row>
                <Col>
                    <Form
                        id="PRE680_form_1"
                        initialValues={initialValues}
                        validationSchema={validationSchema}
                        onBeforeValidateField={onBeforeValidateField}
                        onBeforeSubmit={onBeforeSubmit}
                        onAfterSubmit={onAfterSubmit}
                        onBeforeInitialize={formOnBeforeInitialize}
                    >
                        <Row>
                            <Col lg="3" >
                                <FormGroup>
                                    <label htmlFor="DT_DESDE">{t("PRE680_frm1_lbl_DT_DESDE")}</label>
                                    <FieldDateTime name="DT_DESDE" />
                                </FormGroup>
                            </Col>
                            <Col lg="3">
                                <FormGroup>
                                    <label htmlFor="DT_HASTA">{t("PRE680_frm1_lbl_DT_HASTA")}</label>
                                    <FieldDateTime name="DT_HASTA" />
                                </FormGroup>
                            </Col>
                            <Col lg="3">
                                <div className="form-group">
                                    <label htmlFor="CD_EMPRESA">{t("PRE680_frm1_lbl_CD_EMPRESA")}</label>
                                    <FieldSelectAsync name="CD_EMPRESA" isClearable />
                                    <StatusMessage for="CD_EMPRESA" />
                                </div>
                            </Col>
                            <Col lg="3">
                                <div className="form-group">
                                    <label htmlFor="NU_PREDIO">{t("PRE680_frm1_lbl_NU_PREDIO")}</label>
                                    <FieldSelect name="NU_PREDIO" isClearable />
                                    <StatusMessage for="NU_PREDIO" />
                                </div>
                            </Col>
                            <Col lg="3">
                                <div className="form-group">
                                    <label htmlFor="NU_PREDIO_STOCK_DEFAULT">{t("PRE680_frm1_lbl_NU_PREDIO_STOCK_DEFAULT")}</label>
                                    <FieldSelect name="NU_PREDIO_STOCK_DEFAULT" isClearable />
                                    <StatusMessage for="NU_PREDIO_STOCK_DEFAULT" />
                                </div>
                            </Col>
                            <Col lg="3">
                                <FormGroup>
                                    <label htmlFor="cmbo_Redondiar">{t("PRE680_frm1_lbl_Redondiar")}</label>
                                    <FieldSelect name="cmbo_Redondiar" />
                                    <StatusMessage for="cmbo_Redondiar"  />
                                </FormGroup>
                            </Col>
                            <Col lg="3">
                                <label>{'   '}</label>
                                <FormGroup>
                                    <SubmitButton id="btnFiltrar" label="PRE680_frm1_btn_Consultar" variant="primary" className="mb-4" />
                                </FormGroup>
                            </Col>

                        </Row>
                        <hr />
                    </Form>
                </Col>
            </Row>
            <hr />

            <Row className="mb-4">
                <Col>
                    <Grid
                        id="PRE680_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onAfterCommit={onAfterCommit}
                        onBeforeExportExcel={addParameters}
                        enableExcelExport
                        onBeforeInitialize={addParameters}
                        //   enableSelection
                        onAfterInitialize={onAfterInitialize}
                        onBeforeFetch={addParameters}
                        onBeforeFetchStats={addParameters}
                        onBeforeApplyFilter={addParameters}
                        onBeforeApplySort={addParameters}
                        onBeforeMenuItemAction={addParameters}
                    />
                </Col>
            </Row>


        </Page>
    );
}