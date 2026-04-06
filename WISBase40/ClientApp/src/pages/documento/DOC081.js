import React from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldDate, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';

export default function DOC081(props) {
    const { t } = useTranslation();

    const initialValues = {
        NU_DOCUMENTO: "",
        TP_DOCUMENTO: "",
        AUXDS_DOCUMENTO: "",
        CD_EMPRESA: "",
        NM_EMPRESA: "",
        TP_DUA: "",
        NU_DUA: "",
        NU_EXPORT: "",
        NU_IMPORT: "",
        DS_DOCUMENTO: "",
        NU_FACTURA: "",
        NU_CONOCIMIENTO: "",
        ID_ESTADO: "",
        DT_ADDROW: "",
        DT_UPDROW: "",
        DT_PROGRAMADO: "",
        DT_ENVIADO: "",        
        QT_VALIDAR_LINEAS: "",
        QT_VALIDAR_VALOR: ""

    };

    const validationSchema = {
        QT_VALIDAR_LINEAS: Yup.string().required(),
        QT_VALIDAR_VALOR: Yup.string().required()
    };
    const onAfterSubmit = (context, form, query, nexus) => {
        //Refresh grilla
        nexus.getGrid("DOC081_grid_1").refresh();
    };

    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("DOC081_grid_1").refresh();
    }
    return (
        <Page
            icon="fas fa-file"
            title={t("DOC081_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col">
                    <Form
                        id="DOC081_form_1"
                        initialValues={initialValues}
                        validationSchema={validationSchema}
                        onAfterSubmit={onAfterSubmit}
                    >
                        <div className="row">
                            <div className="col">
                                <div className="row">
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="NU_DOCUMENTO">{t("DOC081_frm1_lbl_NU_DOCUMENTO")}</label>
                                            <Field name="NU_DOCUMENTO" readOnly />
                                            <StatusMessage for="NU_DOCUMENTO" />
                                        </div>
                                    </div>
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="TP_DOCUMENTO">{t("DOC081_frm1_lbl_TP_DOCUMENTO")}</label>
                                            <Field name="TP_DOCUMENTO" readOnly />
                                            <StatusMessage for="TP_DOCUMENTO" />
                                        </div>
                                    </div>
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="AUXDS_DOCUMENTO">{t("DOC081_frm1_lbl_AUXDS_DOCUMENTO")}</label>
                                            <Field name="AUXDS_DOCUMENTO" readOnly />
                                            <StatusMessage for="AUXDS_DOCUMENTO" />
                                        </div>
                                    </div>
                                </div>
                                <div className="row">
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="CD_EMPRESA">{t("DOC081_frm1_lbl_CD_EMPRESA")}</label>
                                            <Field name="CD_EMPRESA" readOnly />
                                            <StatusMessage for="CD_EMPRESA" />
                                        </div>
                                    </div>
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="NM_EMPRESA">{t("DOC081_frm1_lbl_NM_EMPRESA")}</label>
                                            <Field name="NM_EMPRESA" readOnly />
                                            <StatusMessage for="NM_EMPRESA" />
                                        </div>
                                    </div>
                                </div>
                                <div className="row">
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="TP_DUA">{t("DOC081_frm1_lbl_TP_DUA")}</label>
                                            <Field name="TP_DUA" readOnly />
                                            <StatusMessage for="TP_DUA" />
                                        </div>
                                    </div>
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="NU_DUA">{t("DOC081_frm1_lbl_NU_DUA")}</label>
                                            <Field name="NU_DUA" readOnly />
                                            <StatusMessage for="NU_DUA" />
                                        </div>
                                    </div>
                                </div>
                                <div className="row">
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="NU_EXPORT">{t("DOC081_frm1_lbl_NU_EXPORT")}</label>
                                            <Field name="NU_EXPORT" readOnly />
                                            <StatusMessage for="NU_EXPORT" />
                                        </div>
                                    </div>
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="NU_IMPORT">{t("DOC081_frm1_lbl_NU_IMPORT")}</label>
                                            <Field name="NU_IMPORT" readOnly />
                                            <StatusMessage for="NU_IMPORT" />
                                        </div>
                                    </div>
                                </div>
                                <div className="row">
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="DS_DOCUMENTO">{t("DOC081_frm1_lbl_DS_DOCUMENTO")}</label>
                                            <Field name="DS_DOCUMENTO" readOnly />
                                            <StatusMessage for="DS_DOCUMENTO" />
                                        </div>
                                    </div>
                                </div>
                                <div className="row">
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="NU_FACTURA">{t("DOC081_frm1_lbl_NU_FACTURA")}</label>
                                            <Field name="NU_FACTURA" readOnly />
                                            <StatusMessage for="NU_FACTURA" />
                                        </div>
                                    </div>
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="NU_CONOCIMIENTO">{t("DOC081_frm1_lbl_NU_CONOCIMIENTO")}</label>
                                            <Field name="NU_CONOCIMIENTO" readOnly />
                                            <StatusMessage for="NU_CONOCIMIENTO" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div className="col">
                                <div className="row">
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="ID_ESTADO">{t("DOC081_frm1_lbl_ID_ESTADO")}</label>
                                            <Field name="ID_ESTADO" readOnly />
                                            <StatusMessage for="ID_ESTADO" />
                                        </div>
                                    </div>
                                </div>
                                <div className="row">
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="DT_ADDROW">{t("DOC081_frm1_lbl_DT_ADDROW")}</label>
                                            <FieldDate name="DT_ADDROW" readOnly />
                                            <StatusMessage for="DT_ADDROW" />
                                        </div>
                                    </div>
                                </div>
                                <div className="row">
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="DT_UPDROW">{t("DOC081_frm1_lbl_DT_UPDROW")}</label>
                                            <FieldDate name="DT_UPDROW" readOnly />
                                            <StatusMessage for="DT_UPDROW" />
                                        </div>
                                    </div>
                                </div>
                                <div className="row">
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="DT_PROGRAMADO">{t("DOC081_frm1_lbl_DT_PROGRAMADO")}</label>
                                            <FieldDate name="DT_PROGRAMADO" readOnly />
                                            <StatusMessage for="DT_PROGRAMADO" />
                                        </div>
                                    </div>
                                </div>
                                <div className="row">
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="DT_ENVIADO">{t("DOC081_frm1_lbl_DT_ENVIADO")}</label>
                                            <FieldDate name="DT_ENVIADO" readOnly />
                                            <StatusMessage for="DT_ENVIADO" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div className="row">
                            <div className="col-5">
                                <div className="form-group">
                                    <label htmlFor="QT_VALIDAR_LINEAS">{t("DOC081_frm1_lbl_QT_VALIDAR_LINEAS")}</label>
                                    <Field name="QT_VALIDAR_LINEAS"  />
                                    <StatusMessage for="QT_VALIDAR_LINEAS" />
                                </div>
                            </div>
                            <div className="col-5">
                                <div className="form-group">
                                    <label htmlFor="QT_VALIDAR_VALOR">{t("DOC081_frm1_lbl_QT_VALIDAR_VALOR")}</label>
                                    <Field name="QT_VALIDAR_VALOR"  />
                                    <StatusMessage for="QT_VALIDAR_VALOR" />
                                </div>
                            </div>
                            <div className="col-2">
                                <div style={{ marginTop: "17%" }}>
                                    <SubmitButton id="btnSubmit" value={t("DOC081_Sec0_btn_Validar")}/>
                                </div>
                            </div>
                        </div>

                    </Form>
                </div>
            </div>
            <hr />
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="DOC081_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onAfterCommit={onAfterCommit}
                        enableExcelExport
                        enableExcelImport
                    />
                </div>
            </div>
        </Page>
    );
}