import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, FieldSelectAsync, FieldDate, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Row, Col, FormGroup } from 'react-bootstrap';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';

export default function STO395(props) {
    const { t } = useTranslation();

    const [producto, setProducto] = useState(null);
    const [empresa, setEmpresa] = useState(null);
    const [fechaInicio, setFechaInicio] = useState(null);
    const [fechaFin, setFechaFin] = useState(null);

    const [qtStockGeneral, setQtStockGeneral] = useState("");
    const [qtStockInicioPeriodo, setQtStockInicioPeriodo] = useState("");
    const [qtStockFinPeriodo, setQtStockFinPeriodo] = useState("");

    const initialValues = {
        DT_INICIO: "",
        DT_FIN: "",
        CD_EMPRESA: "",
        NM_EMPRESA: "",
        CD_PRODUTO: "",
        DS_PRODUTO: "",
        QT_STOCK_INICIO_PERIODO: "",
        QT_STOCK_GENERAL: "",
        QT_STOCK_FINAL_PERIODO: ""
    };

    const validationSchema =
    {
        CD_EMPRESA: Yup.string().required(),
        CD_PRODUTO: Yup.string().required().max(40),
        DT_INICIO: Yup.string().nullable(),
        DT_FIN: Yup.string().nullable()
    };

    const formOnBeforeInitialize = (context, form, query, nexus) => {

        if (query.parameters && query.parameters.length == 0) {

            query.parameters = [
                {
                    id: "CD_PRODUTO",
                    value: producto || null
                },
                {
                    id: "CD_EMPRESA",
                    value: empresa || null
                }
            ];

        }

    };

    const onAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            const productoParam = query.parameters.find(d => d.id === "CD_PRODUTO");
            const empresaParam = query.parameters.find(d => d.id === "CD_EMPRESA");
            const inicioParam = query.parameters.find(d => d.id === "DT_INICIO");
            const finParam = query.parameters.find(d => d.id === "DT_FIN");

            setProducto(productoParam ? productoParam.value : null);
            setEmpresa(empresaParam ? empresaParam.value : null);
            setFechaInicio(inicioParam ? inicioParam.value : null);
            setFechaFin(finParam ? finParam.value : null);

            nexus.getGrid("STO395_grid_1").refresh();
        }
    };

    const onBeforeFetch = (context, data, nexus) => {
        data.parameters = [
            { id: "CD_PRODUTO", value: producto },
            { id: "CD_EMPRESA", value: empresa },
            { id: "DT_INICIO", value: fechaInicio },
            { id: "DT_FIN", value: fechaFin }
        ];
    };

    const onAfterFetch = (context, newRows, parameters, nexus) => {
        const stockGralParam = parameters.find(d => d.id === "QT_STOCK_GENERAL");
        const stockInicioParam = parameters.find(d => d.id === "QT_STOCK_INICIO_PERIODO");
        const stockFinalParam = parameters.find(d => d.id === "QT_STOCK_FIN_PERIODO");

        setQtStockGeneral(stockGralParam ? stockGralParam.value : "");
        setQtStockInicioPeriodo(stockInicioParam ? stockInicioParam.value : "");
        setQtStockFinPeriodo(stockFinalParam ? stockFinalParam.value : "");
    };

    const onAfterPageLoad = (data) => {
        let productoParam = data.parameters.find(p => p.id === "CSTO395_CD_PRODUTO");
        let empresaParam = data.parameters.find(p => p.id === "CSTO395_CD_EMPRESA");

        if (productoParam && empresaParam) {

            setProducto(productoParam.value);
            setEmpresa(empresaParam.value);

        }
    };

    const onBeforeInitialize = (context, data, nexus) => {

        if (producto && empresa) {

            data.parameters = [
                { id: "CD_PRODUTO", value: producto },
                { id: "CD_EMPRESA", value: empresa },
            ];

        }

    };

    return (
        <Page
            title={t("STO395_Sec0_pageTitle_Titulo")}
            {...props}
            load
            onAfterLoad={onAfterPageLoad}
        >
            <div className="row mb-4">
                <div className="col">
                    <Form id="STO395_form_1" initialValues={initialValues}
                        validationSchema={validationSchema}
                        onAfterSubmit={onAfterSubmit}
                        onBeforeInitialize={formOnBeforeInitialize}
                    >

                        <Row>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="CD_EMPRESA">{t("STO395_frm1_lbl_CD_EMPRESA")} <span className="required-badge">*</span></label>
                                    <FieldSelectAsync name="CD_EMPRESA" />
                                    <StatusMessage for="CD_EMPRESA" />
                                </FormGroup>
                            </Col>
                            <Col>
                                <FormGroup>
                                    <label htmlFor="CD_PRODUTO">{t("STO395_frm1_lbl_CD_PRODUTO")} <span className="required-badge">*</span></label>
                                    <FieldSelectAsync name="CD_PRODUTO" />
                                    <StatusMessage for="CD_PRODUTO" />
                                </FormGroup>
                            </Col>
                         

                        </Row>
                        <Row className="ml-12">
                            <Col lg="4" >
                                <FormGroup>
                                    <label htmlFor="DT_INICIO">{t("STO395_frm1_lbl_DT_INICIO")}</label>
                                    <FieldDate name="DT_INICIO" />
                                    <StatusMessage for="DT_INICIO" />
                                </FormGroup>
                            </Col>
                            <Col lg="4">
                                <FormGroup>
                                    <label htmlFor="DT_FIN">{t("STO395_frm1_lbl_DT_FIN")}</label>
                                    <FieldDate name="DT_FIN" />
                                    <StatusMessage for="DT_FIN" />
                                </FormGroup>
                            </Col>
                            <Col lg="2">
                                <label>{' '}</label>
                                <FormGroup>
                                    <SubmitButton id="btnConfirmar" value={t("STO395_frm1_btn_CONFIRMAR")} />
                                </FormGroup>
                            </Col>
                        </Row>

                        <hr />

                        <div className="row">
                            <div className="col">
                                <div className="form-group">
                                    <label htmlFor="QT_STOCK_GENERAL">{t("STO395_frm1_lbl_QT_STOCK_GENERAL")}</label>
                                    <input className="form-control" name="QT_STOCK_GENERAL" readOnly value={qtStockGeneral} />
                                </div>
                            </div>
                            <div className="col">
                                <div className="form-group">
                                    <label htmlFor="QT_STOCK_INICIO_PERIODO">{t("STO395_frm1_lbl_QT_STOCK_INICIO_PERIODO")}</label>
                                    <input className="form-control" name="QT_STOCK_INICIO_PERIODO" readOnly value={qtStockInicioPeriodo} />
                                </div>
                            </div>
                            <div className="col">
                                <div className="form-group">
                                    <label htmlFor="QT_STOCK_FIN_PERIODO">{t("STO395_frm1_lbl_QT_STOCK_FIN_PERIODO")}</label>
                                    <input className="form-control" name="QT_STOCK_FIN_PERIODO" readOnly value={qtStockFinPeriodo} />
                                </div>
                            </div>
                        </div>
                    </Form>
                </div>
            </div>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="STO395_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeFetch={onBeforeFetch}
                        onBeforeFetchStats={onBeforeFetch}
                        onAfterFetch={onAfterFetch}
                        onBeforeExportExcel={onBeforeFetch}
                        onBeforeInitialize={onBeforeInitialize}
                        onAfterInitialize={onAfterFetch}
                        onBeforeApplyFilter={onBeforeFetch}
                        onBeforeApplySort={onBeforeFetch}
                    />
                </div>
            </div>
        </Page>
    );
}