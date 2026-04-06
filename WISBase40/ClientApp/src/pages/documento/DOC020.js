import React from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';

export default function DOC020(props) {
    const { t } = useTranslation();

    const fieldSetStyle = { border: "1px solid #ddd", margin: "10px", width: "100%" };

    const initialValues = {
        QT_DISPONIBLE: "",
        QT_RESERVADA: "",
        QT_MERCADERIA: "",
    };

    const validationSchema = {
    };

    const onAfterApplyFilter = (context, form, query, nexus) => {
        console.log("----- onAfterApplyFilter ")
        nexus.getForm("DOC020_form_1").reset();
    };
    const onAfterInitialize = (context, form, query, nexus) => {
        console.log("----- onAfterApplyFilter ")
        nexus.getForm("DOC020_form_1").reset();
    };

    const onAfterMenuItemAction = (context, data, nexus) => {
        if (data.redirect) {
            window.location = data.redirect;
        }
    }

    return (

        <Page
            icon="fas fa-file"
            title={t("DOC020_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                id="DOC020_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}>

                <div className="row col-12">
                    <fieldset className="row" style={fieldSetStyle}>
                        <legend>{t("DOC020_frm1_lbl_Totales")}</legend>
                        <div className="row col-12">
                            <div className="col-4">
                                <div className="form-group">
                                    <label htmlFor="QT_DISPONIBLE">{t("DOC020_frm1_lbl_QT_DISPONIBLE")}</label>
                                    <Field name="QT_DISPONIBLE" readOnly />
                                    <StatusMessage for="QT_DISPONIBLE" />
                                </div>
                            </div>
                            <div className="col-4">
                                <div className="form-group">
                                    <label htmlFor="QT_RESERVADA">{t("DOC020_frm1_lbl_QT_RESERVADA")}</label>
                                    <Field name="QT_RESERVADA" readOnly />
                                    <StatusMessage for="QT_RESERVADA" />
                                </div>
                            </div>
                            <div className="col-4">
                                <div className="form-group">
                                    <label htmlFor="QT_MERCADERIA">{t("DOC020_frm1_lbl_QT_MERCADERIA")}</label>
                                    <Field name="QT_MERCADERIA" readOnly />
                                    <StatusMessage for="QT_MERCADERIA" />
                                </div>
                            </div>
                        </div>
                        <div className="row col-12">
                            <div className="col-4">
                                <div className="form-group">
                                    <label htmlFor="QT_INGRESADA">{t("DOC020_frm1_lbl_QT_INGRESADA")}</label>
                                    <Field name="QT_INGRESADA" readOnly />
                                    <StatusMessage for="QT_INGRESADA" />
                                </div>
                            </div>
                            <div className="col-4">
                                <div className="form-group">
                                    <label htmlFor="QT_DESAFECTADA">{t("DOC020_frm1_lbl_QT_DESAFECTADA")}</label>
                                    <Field name="QT_DESAFECTADA" readOnly />
                                    <StatusMessage for="QT_DESAFECTADA" />
                                </div>
                            </div>
                            <div className="col-4">
                                <div className="form-group">
                                    <label htmlFor="QT_EXISTENCIA">{t("DOC020_frm1_lbl_QT_EXISTENCIA")}</label>
                                    <Field name="QT_EXISTENCIA" readOnly />
                                    <StatusMessage for="QT_EXISTENCIA" />
                                </div>
                            </div>
                        </div>
                    </fieldset>
                </div>
            </Form>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="DOC020_grid_1" rowsToFetch={30} rowsToDisplay={15}
                        onAfterApplyFilter={onAfterApplyFilter}
                        onAfterInitialize={onAfterInitialize}
                        enableExcelExport
                        onAfterMenuItemAction={onAfterMenuItemAction}
                    />
                </div>
            </div>
        </Page>
    );
}