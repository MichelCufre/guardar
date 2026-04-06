import React from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldDate, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import '../../css/DOC330.css';

export default function DOC330(props) {
    const { t } = useTranslation();

    const initialValues = {
        NU_AGRUPADOR: "",
        TP_AGRUPADOR: "",
        DS_AGRUPADOR: "",
        QT_VOLUMEN: "",
        QT_PESO: "",
        NU_LACRE: "",
        VL_TOTAL: "",
        ID_ESTADO: "",
        DT_ALTA: "",
        DT_SALIDA: ""
    };

    const validationSchema = {
        NU_AGRUPADOR: Yup.string().nullable(),
        TP_AGRUPADOR: Yup.string().nullable(),
        DS_AGRUPADOR: Yup.string().nullable(),
        QT_VOLUMEN: Yup.string().nullable(),
        QT_PESO: Yup.string().nullable(),
        NU_LACRE: Yup.string().nullable(),
        VL_TOTAL: Yup.string().nullable(),
        ID_ESTADO: Yup.string().nullable(),
        DT_ALTA: Yup.string().nullable(),
        DT_SALIDA: Yup.string().nullable()
    };
    const onAfterSubmit = (context, form, query, nexus) => {
        //Refresh grilla
        nexus.getGrid("DOC330_grid_1").refresh();
    };
    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("DOC330_grid_1").refresh();
    }

    return (
        <Page
            load
            icon="fas fa-file"
            title={t("DOC330_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col">
                    <Form
                        id="DOC330_form_1"
                        initialValues={initialValues}
                        validationSchema={validationSchema}
                        onAfterSubmit={onAfterSubmit}
                    >
                        <div className="row">
                            <div className="col">
                                <div className="row">
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="NU_AGRUPADOR">{t("DOC330_frm1_lbl_NU_AGRUPADOR")}</label>
                                            <Field name="NU_AGRUPADOR" readOnly />
                                            <StatusMessage for="NU_AGRUPADOR" />
                                        </div>
                                    </div>
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="DS_AGRUPADOR">{t("DOC330_frm1_lbl_DS_AGRUPADOR")}</label>
                                            <Field name="DS_AGRUPADOR" readOnly />
                                            <StatusMessage for="DS_AGRUPADOR" />
                                        </div>
                                    </div>
                                </div>
                                <div className="row">
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="transportadora">{t("DOC330_frm1_lbl_TRANSPORTADORA")}</label>
                                            <Field name="transportadora" readOnly />
                                            <StatusMessage for="transportadora" />
                                        </div>
                                    </div>
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="placa">{t("DOC330_frm1_lbl_PLACA")}</label>
                                            <Field name="placa" readOnly />
                                            <StatusMessage for="placa" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div className="col">
                                <div className="row">
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="ID_ESTADO">{t("DOC330_frm1_lbl_ID_ESTADO")}</label>
                                            <Field name="ID_ESTADO" readOnly />
                                            <StatusMessage for="ID_ESTADO" />
                                        </div>
                                    </div>
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="EMPRESA">{t("DOC330_frm1_lbl_EMPRESA")}</label>
                                            <Field name="EMPRESA" readOnly />
                                            <StatusMessage for="EMPRESA" />
                                        </div>
                                    </div>
                                </div>
                                <div className="row">
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="NU_LACRE">{t("DOC330_frm1_lbl_NU_LACRE")}</label>
                                            <Field name="NU_LACRE" readOnly />
                                            <StatusMessage for="NU_LACRE" />
                                        </div>
                                    </div>
                                    <div className="col">
                                        <div className="form-group">
                                            <label htmlFor="DT_SALIDA">{t("DOC330_frm1_lbl_DT_SALIDA")}</label>
                                            <Field name="DT_SALIDA" readOnly />
                                            <StatusMessage for="DT_SALIDA" />
                                        </div>
                                    </div>                                    
                                </div>                               
                            </div>                           
                        </div>
                        <div className="row">
                            <div className="col">
                                <SubmitButton id="btnSubmit" value={t("DOC330_Sec0_btn_Confirmar")} />
                            </div>
                        </div>
                    </Form>
                </div>
            </div>
            <hr />
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="DOC330_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        enableSelection={true}
                        onAfterMenuItemAction={onAfterMenuItemAction}
                    />
                </div>
            </div>
        </Page>
    );
}