import React from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldDate, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';

export default function DOC340(props) {
    const { t } = useTranslation();
    const initialValues = {
        NU_DOCUMENTO: "",
        DS_DOCUMENTO: "",
        ESTADO: "",
        EMPRESA: ""
    };

    return (

        <Page
            icon="fas fa-edit"
            title={t("DOC340_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col">

                    <Form
                        id="DOC340_form_1"
                        initialValues={initialValues}
                    >
                        <div className="row">
                            <div className="col">
                                <div className="form-group">
                                    <label htmlFor="NU_DOCUMENTO">{t("DOC340_frm1_lbl_NU_DOCUMENTO")}</label>
                                    <Field name="NU_DOCUMENTO" readOnly />
                                    <StatusMessage for="NU_DOCUMENTO" />
                                </div>
                            </div>
                            <div className="col">
                                <div className="form-group">
                                    <label htmlFor="DS_DOCUMENTO">{t("DOC340_frm1_lbl_DS_DOCUMENTO")}</label>
                                    <Field name="DS_DOCUMENTO" readOnly />
                                    <StatusMessage for="DS_DOCUMENTO" />
                                </div>
                            </div>
                            <div className="col">
                                <div className="form-group">
                                    <label htmlFor="ESTADO">{t("DOC340_frm1_lbl_ESTADO")}</label>
                                    <Field name="ESTADO" readOnly />
                                    <StatusMessage for="ESTADO" />
                                </div>
                            </div>
                            <div className="col">
                                <div className="form-group">
                                    <label htmlFor="EMPRESA">{t("DOC340_frm1_lbl_EMPRESA")}</label>
                                    <Field name="EMPRESA" readOnly />
                                    <StatusMessage for="EMPRESA" />
                                </div>
                            </div>
                        </div>

                    </Form>
                </div>
            </div>
            <hr />
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="DOC340_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport enableExcelImport={false}/>
                </div>
            </div>
        </Page>
    );
}