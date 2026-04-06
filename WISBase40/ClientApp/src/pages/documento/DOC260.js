import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, FieldSelect, Field, FieldDate, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import { Button } from 'react-bootstrap';


export default function DOC260(props) {
    const { t } = useTranslation();

    const [isDocumentSelected, setDocumentSelected] = useState("hidden");
    const [isDUARequired, setDUARequired] = useState("hidden");
    const [isDTIRequired, setDTIRequired] = useState("hidden");
    const [isRefExternaRequired, setRefExternaRequired] = useState("hidden");
    const [isFacturaRequired, setFacturaRequired] = useState("hidden");
    const [isAgendaRequired, setAgendaRequired] = useState("hidden");

    const initialValues = {
        nroDoc: "",
        tpDocumento: "",
        nuevoEstadoDocumento: "",
        tpDua: "",
        nroDua: "",
        fechVerificadoDua: "",
        nroDTI: "",
        fechDTI: "",
        tpRefExterna: "",
        nroRefExterna: "",
        fechRefExterna: "",
        nroFactura: "",
        nroAgenda: ""
    };

    const validationSchema =
    {
        nuevoEstadoDocumento: Yup.string(),
        tpDua: Yup.string().nullable(),
        nroDua: Yup.string().nullable(),
        fechVerificadoDua: Yup.string().nullable(),
        nroFactura: Yup.string().nullable(),
        nroAgenda: Yup.string().nullable(),
        nroDTI: Yup.string().nullable(),
        fechDTI: Yup.string().nullable(),
        tpRefExterna: Yup.string().nullable(),
        nroRefExterna: Yup.string().nullable(),
        fechRefExterna: Yup.string().nullable(),
    };

    const onBeforeButtonAction = (context, data, nexus) => {

        context.abortServerCall = true;

        console.log("inin");

        setDUARequired("hidden");
        setDTIRequired("hidden");
        setRefExternaRequired("hidden");
        setFacturaRequired("hidden");
        setAgendaRequired("hidden");

        let Documento = data.row.cells.find(d => d.column === "NU_DOCUMENTO").value;
        let TipoDocumento = data.row.cells.find(d => d.column === "TP_DOCUMENTO").value;

        window.localStorage.setItem('NU_DOCUMENTO', Documento);
        window.localStorage.setItem('TP_DOCUMENTO', TipoDocumento);

        const parameters = [
            {
                id: "NU_DOCUMENTO",
                value: Documento
            },
            {
                id: "TP_DOCUMENTO",
                value: TipoDocumento
            }
        ];

        nexus.getForm("DOC260_form_1").reset(parameters);
        setDocumentSelected("");

    };

    const onAfterInitialize = (context, form, query, nexus) => {
        const mostrar = query.parameters.find(p => p.id === "mostrarForm");

        console.log(query);

        if (mostrar && mostrar.value === "true")
            setDocumentSelected("");

    };

    const onBeforeValidateField = (context, form, query, nexus) => {
        const parameters = [
            {
                id: "NU_DOCUMENTO",
                value: window.localStorage.getItem('NU_DOCUMENTO')
            },
            {
                id: "TP_DOCUMENTO",
                value: window.localStorage.getItem('TP_DOCUMENTO')
            }
        ];

        query.parameters = parameters;
    };

    const onAfterValidateField = (context, form, query, nexus) => {
        const DUARequired = query.parameters.find(p => p.id === "DUARequired");
        const DTIRequired = query.parameters.find(p => p.id === "DTIRequired");
        const RefExternaRequired = query.parameters.find(p => p.id === "RefExternaRequired");
        const facturaRequiered = query.parameters.find(p => p.id === "facturaRequiered");
        const agendaRequiered = query.parameters.find(p => p.id === "agendaRequiered");

        if (DUARequired && DUARequired.value == "true") {
            setDUARequired("");
        } else {
            setDUARequired("hidden");
        }

        if (DTIRequired && DTIRequired.value == "true") {
            setDTIRequired("");
        } else {
            setDTIRequired("hidden");
        }

        if (RefExternaRequired && RefExternaRequired.value == "true") {
            setRefExternaRequired("");
        } else {
            setRefExternaRequired("hidden");
        }

        if (facturaRequiered && facturaRequiered.value == "true") {
            setFacturaRequired("");
        } else {
            setFacturaRequired("hidden");
        }

        if (agendaRequiered && agendaRequiered.value == "true") {
            setAgendaRequired("");
        } else {
            setAgendaRequired("hidden");
        }
    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        const parameters = [
            {
                id: "NU_DOCUMENTO",
                value: window.localStorage.getItem('NU_DOCUMENTO')
            },
            {
                id: "TP_DOCUMENTO",
                value: window.localStorage.getItem('TP_DOCUMENTO')
            }
        ];

        query.parameters = parameters;
    };

    const onAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            //Refresh grilla
            nexus.getGrid("DOC260_grid_1").refresh();

            setDUARequired("hidden");
            setDTIRequired("hidden");
            setRefExternaRequired("hidden");
            setFacturaRequired("hidden");
            setAgendaRequired("hidden");

            //Refresh form
            const parameters = [
                {
                    id: "NU_DOCUMENTO",
                    value: window.localStorage.getItem('NU_DOCUMENTO')
                },
                {
                    id: "TP_DOCUMENTO",
                    value: window.localStorage.getItem('TP_DOCUMENTO')
                }
            ];

            //nexus.getForm("DOC260_form_1").reset(parameters);
            setDocumentSelected("hidden");
        }
    };

    const handleClose = (evt) => {
        setDocumentSelected("hidden");
    }

    return (
        <Page icon="fas fa-edit" title={t("DOC260_Sec0_pageTitle_Titulo")} {...props}>

            <Form id="DOC260_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}
                
                onBeforeValidateField={onBeforeValidateField}
                onAfterValidateField={onAfterValidateField}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={onAfterSubmit}>

                <div name="mainContainer" className={isDocumentSelected}>
                    <div className="row">
                        <div className="col-4">
                            <div className="form-group">
                                <label htmlFor="nroDoc">{t("DOC260_frm1_lbl_NU_DOCUMENTO")}</label>
                                <Field name="nroDoc" readOnly />
                                <StatusMessage for="nroDoc" />
                            </div>
                        </div>
                        <div className="col-4">
                            <div className="form-group">
                                <label htmlFor="tpDocumento">{t("DOC260_frm1_lbl_TP_DOCUMENTO")}</label>
                                <Field name="tpDocumento" readOnly />
                                <StatusMessage for="tpDocumento" />
                            </div>
                        </div>
                        <div className="col-4">
                            <div className="form-group">
                                <label htmlFor="nuevoEstadoDocumento">{t("DOC260_frm1_lbl_NEW_ESTADO")}</label>
                                <FieldSelect name="nuevoEstadoDocumento" />
                                <StatusMessage for="nuevoEstadoDocumento" />
                            </div>
                        </div>
                    </div>

                    <div className={isRefExternaRequired}>
                        <div className="row">
                            <div className="col-4">
                                <div className="form-group">
                                    <label htmlFor="tpRefExterna">{t("DOC260_frm1_lbl_TP_REF_EXTERNA")}</label>
                                    <FieldSelect name="tpRefExterna" />
                                    <StatusMessage for="tpRefExterna" />
                                </div>
                            </div>
                            <div className="col-4">
                                <div className="form-group">
                                    <label htmlFor="nroRefExterna">{t("DOC260_frm1_lbl_NU_REF_EXTERNA")}</label>
                                    <Field name="nroRefExterna" />
                                    <StatusMessage for="nroRefExterna" />
                                </div>
                            </div>
                            <div className="col-4">
                                <div className="form-group">
                                    <label htmlFor="fechRefExterna">{t("DOC260_frm1_lbl_DT_REF_EXTERNA")}</label>
                                    <FieldDate name="fechRefExterna" />
                                    <StatusMessage for="fechRefExterna" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className={isDTIRequired}>
                        <div className="row">
                            <div className="col-4">
                                <div className="form-group">
                                    <label htmlFor="nroDTI">{t("DOC260_frm1_lbl_NU_DTI")}</label>
                                    <Field name="nroDTI" />
                                    <StatusMessage for="nroDTI" />
                                </div>
                            </div>
                            <div className="col-4">
                                <div className="form-group">
                                    <label htmlFor="fechDTI">{t("DOC260_frm1_lbl_DT_DTI")}</label>
                                    <FieldDate name="fechDTI" />
                                    <StatusMessage for="fechDTI" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className={isDUARequired}>
                        <div className="row">
                            <div className="col-4">
                                <div className="form-group">
                                    <label htmlFor="tpDua">{t("DOC260_frm1_lbl_TP_DUA")}</label>
                                    <FieldSelect name="tpDua" />
                                    <StatusMessage for="tpDua" />
                                </div>
                            </div>
                            <div className="col-4">
                                <div className="form-group">
                                    <label htmlFor="nroDua">{t("DOC260_frm1_lbl_NU_DUA")}</label>
                                    <Field name="nroDua" />
                                    <StatusMessage for="nroDua" />
                                </div>
                            </div>
                            <div className="col-4">
                                <div className="form-group">
                                    <label htmlFor="fechVerificadoDua">{t("DOC260_frm1_lbl_DT_VERIFICADO_DUA")}</label>
                                    <FieldDate name="fechVerificadoDua" />
                                    <StatusMessage for="fechVerificadoDua" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className={isFacturaRequired}>
                        <div className="row">
                            <div className="col-4">
                                <div className="form-group">
                                    <label htmlFor="nroFactura">{t("DOC260_frm1_lbl_NU_FACTURA")}</label>
                                    <Field name="nroFactura" />
                                    <StatusMessage for="nroFactura" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className={isAgendaRequired}>
                        <div className="row">
                            <div className="col-4">
                                <div className="form-group">
                                    <label htmlFor="nroAgenda">{t("DOC260_frm1_lbl_NU_AGENDA")}</label>
                                    <FieldSelect name="nroAgenda" />
                                    <StatusMessage for="nroAgenda" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className="row" style={{ marginBottom: "15px" }}>
                        <div className="col">
                            <SubmitButton id="btnSubmit" value={t("DOC260_Sec0_btn_Confirmar")} />
                            &nbsp;
                            <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                                {t("EXP040_frm1_btn_cerrar")}
                            </Button>
                        </div>
                    </div>
                </div>

            </Form>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="DOC260_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeButtonAction={onBeforeButtonAction}
                    />
                </div>
            </div>
        </Page>
    );
}