import React, { useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldDate, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import '../../css/DOC363.css';

export default function DOC363(props) {
    const filtroAjuste = useRef("");
    const filtroDocumento = useRef("");
    const filtroTipoDocumento = useRef("");
    const filtroActa = useRef("");

    const initialValues = {
        nroDoc: "",
        tpDocumento: "",
        estado: "",
        empresa: "",
        fechaCreacion: "",
        cantLineas: ""
    };

    const onAfterButtonActionGrid = (data, nexus) => {
        if (data.buttonId === "btnSeleccionar" && data.parameters && data.parameters.find(f => f.id === "FILTRO_AJUSTES") !== undefined && data.parameters.find(f => f.id === "FILTRO_DOCUMENTO") !== undefined && data.parameters.find(f => f.id === "FILTRO_TIPO_DOC") !== undefined) {

            filtroAjuste.current = data.parameters.find(f => f.id === "FILTRO_AJUSTES");
            filtroDocumento.current = data.parameters.find(f => f.id === "FILTRO_DOCUMENTO");
            filtroTipoDocumento.current = data.parameters.find(f => f.id === "FILTRO_TIPO_DOC");

            nexus.getGrid("DOC363_grid_ajustes").refresh();
            nexus.getForm("DOC363_form_1").reset();
        }
    };
    const onAfterFetch = (context, newRows, parameters, nexus) => {
        if (parameters && parameters.find(f => f.id === "AJUSTES_ACTA") !== undefined) {
            filtroActa.current = parameters.find(f => f.id === "AJUSTES_ACTA");
        }
    };

    const addParameters = (context, data, nexus) => {

        if (filtroAjuste.current && filtroDocumento.current && filtroTipoDocumento.current) {
            data.parameters = [
                {
                    id: "FILTRO_AJUSTES",
                    value: filtroAjuste.current.value
                },
                {
                    id: "FILTRO_DOCUMENTO",
                    value: filtroDocumento.current.value
                },
                {
                    id: "FILTRO_TIPO_DOC",
                    value: filtroTipoDocumento.current.value
                }
            ];
        }
    };
    const addParametersForm = (context, data, query, nexus) => {

        if (filtroDocumento.current && filtroTipoDocumento.current) {
            query.parameters = [
                {
                    id: "FILTRO_DOCUMENTO",
                    value: filtroDocumento.current.value
                },
                {
                    id: "FILTRO_TIPO_DOC",
                    value: filtroTipoDocumento.current.value
                }
            ];


            if (filtroActa.current) {
                query.parameters.push(
                    {
                        id: "AJUSTES_ACTA",
                        value: filtroActa.current.value
                    }
                );
            }
        }
    };

    const { t } = useTranslation();
    return (

        <Page
            icon="fas fa-file"
            title={t("DOC363_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <Form id="DOC363_form_1" initialValues={initialValues} onBeforeInitialize={addParametersForm} onBeforeSubmit={addParametersForm}>
                <div className="row">
                    <div className="col-4">
                        <div className="form-group">
                            <label htmlFor="nroDoc">{t("DOC363_frm1_lbl_NU_DOCUMENTO")}</label>
                            <Field name="nroDoc" readOnly />
                            <StatusMessage for="nroDoc" />
                        </div>
                    </div>
                    <div className="col-4">
                        <div className="form-group">
                            <label htmlFor="tpDocumento">{t("DOC363_frm1_lbl_TP_DOCUMENTO")}</label>
                            <Field name="tpDocumento" readOnly />
                            <StatusMessage for="tpDocumento" />
                        </div>
                    </div>
                    <div className="col-4">
                        <div className="form-group">
                            <label htmlFor="estado">{t("DOC363_frm1_lbl_ESTADO")}</label>
                            <Field name="estado" readOnly />
                            <StatusMessage for="estado" />
                        </div>
                    </div>
                </div>
                <div className="row">
                    <div className="col-4">
                        <div className="form-group">
                            <label htmlFor="empresa">{t("DOC363_frm1_lbl_EMPRESA")}</label>
                            <Field name="empresa" readOnly />
                            <StatusMessage for="empresa" />
                        </div>
                    </div>
                    <div className="col-4">
                        <div className="form-group">
                            <label htmlFor="fechaCreacion">{t("DOC363_frm1_lbl_DT_ADDROW")}</label>
                            <FieldDate name="fechaCreacion" readOnly />
                            <StatusMessage for="fechaCreacion" />
                        </div>
                    </div>
                    <div className="col-4">
                        <div className="form-group">
                            <label htmlFor="cantLineas">{t("DOC363_frm1_lbl_QT_LINEAS")}</label>
                            <Field name="cantLineas" readOnly />
                            <StatusMessage for="cantLineas" />
                        </div>
                    </div>
                </div>
                <div className="row" style={{ margin: "15px" }}>
                    <div className="col">
                        <SubmitButton id="btnSubmit" value={t("DOC363_Sec0_btn_Confirmar")} />
                    </div>
                </div>
            </Form>

            <div className="row">
                <div className="col-6">
                    <Grid id="DOC363_grid_ajustes"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeFetch={addParameters}
                        onAfterFetch={onAfterFetch}
                    />
                </div>

                <div className="col-6">
                    <Grid id="DOC363_grid_documentos"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onAfterButtonAction={onAfterButtonActionGrid}
                    />
                </div>
            </div>

        </Page>
    );
}