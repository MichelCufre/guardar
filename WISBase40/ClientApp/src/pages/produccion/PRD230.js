import React, { useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldDate, SubmitButton, Button, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import '../../css/RowColors.css';

export default function PRD230(props) {
    const { t } = useTranslation();
    const ingreso = useRef("");
    const tipoStock = useRef("");

    const onAfterPageLoad = (data) => {
        let numeroIngreso = data.parameters.find(p => p.id === "Ingreso");

        if (numeroIngreso) {
            ingreso.current = numeroIngreso;
        }
    };
    const onBeforeInitialize = (context, data, nexus) => {
        if (tipoStock.current) {
            let parameters = [
                {
                    id: "TP_STOCK",
                    value: tipoStock.current
                }
            ];

            if (ingreso.current)
                parameters.push({ id: "PRD_INGRESO", value: ingreso.current.value });

            data.parameters = parameters;
        }
    }
    const onBeforeCommit = (context, data, nexus) => {
        if (tipoStock.current) {
            let parameters = [
                {
                    id: "TP_STOCK",
                    value: tipoStock.current
                }
            ];

            if (ingreso.current)
                parameters.push({ id: "PRD_INGRESO", value: ingreso.current.value });

            data.parameters = parameters;
        }
    }
    const onBeforeFetch = (context, data, nexus) => {
        if (tipoStock.current) {
            let parameters = [
                {
                    id: "TP_STOCK",
                    value: tipoStock.current
                }
            ];

            if (ingreso.current)
                parameters.push({ id: "PRD_INGRESO", value: ingreso.current.value });

            data.parameters = parameters;
        }
    }
    const onBeforeApplyFilter = (context, data, nexus) => {
        if (tipoStock.current) {
            let parameters = [
                {
                    id: "TP_STOCK",
                    value: tipoStock.current
                }
            ];

            if (ingreso.current)
                parameters.push({ id: "PRD_INGRESO", value: ingreso.current.value });

            data.parameters = parameters;
        }
    }
    const onBeforeExportExcel = (context, data, nexus) => {
        if (tipoStock.current) {
            let parameters = [
                {
                    id: "TP_STOCK",
                    value: tipoStock.current
                }
            ];

            if (ingreso.current)
                parameters.push({ id: "PRD_INGRESO", value: ingreso.current.value });

            data.parameters = parameters;
        }
    }
    const onBeforeApplySort = (context, data, nexus) => {
        if (tipoStock.current) {
            let parameters = [
                {
                    id: "TP_STOCK",
                    value: tipoStock.current
                }
            ];

            if (ingreso.current)
                parameters.push({ id: "PRD_INGRESO", value: ingreso.current.value });

            data.parameters = parameters;
        }
    };
    const onBeforeValidateRow = (context, data, nexus) => {
        if (tipoStock.current) {
            let parameters = [
                {
                    id: "TP_STOCK",
                    value: tipoStock.current
                }
            ];

            if (ingreso.current)
                parameters.push({ id: "PRD_INGRESO", value: ingreso.current.value });

            data.parameters = parameters;
        }
    }

    const validationSchema =
    {
        tpSalida: Yup.string().required()
    };
    const onAfterSubmit = (context, form, query, nexus) => {
        tipoStock.current = form.fields.find(f => f.id === "tpSalida").value;

        nexus.getGrid("PRD230_grid_1").refresh();
    };
    const onBeforeValidateField = (context, form, query, nexus) => {
        context.abortServerCall = true;
    }

    return (

        <Page
            load
            icon="fas fa-list"
            title={t("PRD230_Sec0_pageTitle_Titulo")}
            onAfterLoad={onAfterPageLoad}
            {...props}
        >
            <Form id="PRD230_form_1" validationSchema={validationSchema} onAfterSubmit={onAfterSubmit} onBeforeValidateField={onBeforeValidateField}>

                <div className="row">
                    <div className="col-lg-3">
                        <div className="form-group">
                            <label htmlFor="tpSalida">{t("PRD230_frm1_lbl_TP_SALIDA")}</label>
                            <FieldSelect name="tpSalida" />
                            <StatusMessage for="tpSalida" />
                        </div>
                    </div>
                    <div className="col-3" style={{ marginTop: "30px" }}>
                        <SubmitButton id="btnSubmit" value={t("PRD230_Sec0_btn_FILTRAR")} />
                    </div>
                </div>

            </Form>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="PRD230_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeInitialize={onBeforeInitialize}
                        onBeforeCommit={onBeforeCommit}
                        onBeforeFetch={onBeforeFetch}
                        onBeforeExportExcel={onBeforeExportExcel}
                        onBeforeApplyFilter={onBeforeApplyFilter}
                        onBeforeApplySort={onBeforeApplySort}
                        onBeforeValidateRow={onBeforeValidateRow}
                    />
                </div>
            </div>
        </Page>
    );
}