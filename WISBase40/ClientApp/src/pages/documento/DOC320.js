import React, { useState, useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldDate, SubmitButton, FormButton as Button, StatusMessage, FieldDateTime } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import Modal from 'react-bootstrap/Modal';


export default function DOC320(props) {
    const { t } = useTranslation();

    const [isFormEnabled, setFormEnabled] = useState(true);
    const [isFormDisplayed, setFormDisplayed] = useState(false);
    const [isPredioRequired, setPredioRequired] = useState(false);

    const [showModal, setShowModal] = useState(false);
    const handleCloseModal = () => setShowModal(false);
    const numeroAgrupadorRow = useRef({});
    const tipoAgrupadorRow = useRef({});

    const secondarySubmitStyle = { width: "300px !important" };
    const fieldSetStyle = { border: "1px solid #ddd", margin: "10px", width: "100%" };
    const confirmButtonClassName = "btn btn-warning";

    // Metodos Form 1
    const initialValues = {
        tpAgrupador: "",
        idEstado: "",
        fechaCreacion: "",
        nroAgrupador: "",
        nroLacre: "",
        fechaSalida: new Date().toUTCString(),
        qtVolumen: "",
        qtPeso: "",
        cdEmpresa: "",
        qtPesoLiquido: "",
        cdTransportadora: "",
        tipoVehiculo: "",
        motorista: "",
        placa: "",
        vlTotal: "",
        anexo1: "",
        anexo2: "",
        anexo3: "",
        anexo4: "",

    };
    const validationSchema =
    {
        tpAgrupador: Yup.string().required(),
        idEstado: Yup.string().nullable(),
        fechaCreacion: Yup.string().nullable(),
        nroAgrupador: Yup.string().nullable(),
        nroLacre: Yup.string().required(),
        fechaSalida: Yup.string().required(),
        qtVolumen: Yup.string().required(),
        qtPeso: Yup.string().required(),
        vlTotal: Yup.string().required(),
        cdEmpresa: Yup.string().nullable(),
        qtPesoLiquido: Yup.string().required(),
        cdTransportadora: Yup.string().required(),
        tipoVehiculo: Yup.string().required(),
        motorista: Yup.string().required(),
        placa: Yup.string().required(),
        anexo1: Yup.string().required(),
        anexo2: Yup.string().required(),
        anexo3: Yup.string().nullable(),
        anexo4: Yup.string().nullable(),
        nuPredio: Yup.string().nullable(),

    };


    const onAfterInitialize = (context, data, query, nexus) => {
        setFormEnabled(false);

        const requierePredio = query.parameters.find(p => p.id === "ManejaPredio");

        if (requierePredio && requierePredio.value == "true") {
            setPredioRequired(true);
        }
        else if (requierePredio && requierePredio.value == "false") {
            setPredioRequired(false);
        }
    };

    const onBeforeButtonAction = (context, form, query, nexus) => {
        if (query.buttonId === "showFormButton") {
            context.abortServerCall = true;
            nexus.getForm("DOC320_form_1").reset();
            setFormDisplayed(true);
        }
        else if (query.buttonId === "hideModalButton") {
            context.abortServerCall = true;
            setShowModal(false);
        }

    };
    const onAfterButtonActionForm = (context, form, query, nexus) => {
        if (query.buttonId === "hideFormButton") {
            nexus.getForm("DOC320_form_1").reset();
            setFormDisplayed(false);
        }
    };
    const onAfterSubmit = (context, form, data, nexus) => {
        if (data.parameters !== undefined && data.parameters.find(f => f.id === "success") !== undefined && data.parameters.find(f => f.id === "success").value === "true") {
            setFormDisplayed(false);
            nexus.getForm("DOC320_form_1").reset();
            nexus.getGrid("DOC320_grid_1").refresh();
        }
    };
    const onAfterValidateField = (context, form, query, nexus) => {
        const requierePredio = query.parameters.find(p => p.id === "ManejaPredio");

        if (requierePredio && requierePredio.value == "true") {
            setPredioRequired(true);
        }
        else if (requierePredio && requierePredio.value == "false") {
            setPredioRequired(false);
        }

    };

    // Metodos Form 2
    const initialValues_2 = {
        motivoCancelacion: ""
    };
    const validationSchema_2 =
    {
        motivoCancelacion: Yup.string().nullable()
    };
    const onBeforeSubmit_2 = (context, form, query, nexus) => {
        const parameters = [
            {
                id: "NU_AGRUPADOR",
                value: numeroAgrupadorRow.current
            },
            {
                id: "TP_AGRUPADOR",
                value: tipoAgrupadorRow.current
            }
        ];

        query.parameters = parameters;
    };
    const onAfterSubmit_2 = (context, form, data, nexus) => {

        if (context.responseStatus === "OK") {
            setShowModal(false);
            nexus.getGrid("DOC320_grid_1").refresh();
        }
    };

    // Metodos Grilla
    const onAfterButtonActionGrid = (data, nexus) => {
        if (data.parameters !== undefined && data.parameters.find(f => f.id === "editar") !== undefined && data.parameters.find(f => f.id === "editar").value === "true" && data.buttonId === "btnEditar") {
            nexus.getForm("DOC320_form_1").reset();
            setFormDisplayed(true);
        }
        if (data.buttonId === "btnEnvioMasivo")
            nexus.getGrid("DOC320_grid_1").refresh();
    };

    const onBeforeGridButtonAction = (context, data, nexus) => {

        console.log(data);

        //if (data.buttonId === "btnPDF") {
        //    context.abortServerCall = true;

        //    const parameters = [
        //        {
        //            id: "NU_AGRUPADOR",
        //            value: data.row.cells.find(c => c.column == "NU_AGRUPADOR").value
        //        },
        //        {
        //            id: "TP_AGRUPADOR",
        //            value: data.row.cells.find(c => c.column == "TP_AGRUPADOR").value
        //        }
        //    ];

        //    nexus.generateReport("DOC320_Report", parameters);
        //}

        if (data.buttonId === "btnCancelarEnvio") {

            numeroAgrupadorRow.current = data.row.cells.find(c => c.column == "NU_AGRUPADOR").value;
            tipoAgrupadorRow.current = data.row.cells.find(c => c.column == "TP_AGRUPADOR").value;

            nexus.getForm("DOC320_form_2").reset();
            context.abortServerCall = true;
            setShowModal(true);

        }
    }

    return (

        <Page
            icon="fas fa-list"
            title={t("DOC320_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form id="DOC320_form_1"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onAfterInitialize={onAfterInitialize}
                onBeforeButtonAction={onBeforeButtonAction}
                onAfterSubmit={onAfterSubmit}
                onAfterButtonAction={onAfterButtonActionForm}
                onAfterValidateField={onAfterValidateField}
            >
                <div className="row" style={{ textAlign: "center", display: isFormDisplayed ? 'none' : 'block' }}>
                    <Button id="showFormButton" value={t("DOC320_Sec0_btn_AgregarNuevoAgr")} className="btn btn-success" style={{ margin: "15px" }} isLoading={isFormEnabled} />
                </div>

                <div style={{ display: isFormDisplayed ? 'block' : 'none' }}>
                    <div className="col">

                        <div className="row mb-4">
                            <div className="col-lg-12">
                                <h4 className="form-title">{t("DOC320_frm1_lbl_Legend1")}</h4>
                                <div className="row">

                                    <div className="col-lg-3">
                                        <div className="form-group">
                                            <label htmlFor="tpAgrupador">{t("DOC320_frm1_lbl_TP_AGRUPADOR")}</label><label style={{ color: "red" }}> *</label>
                                            <FieldSelect name="tpAgrupador" />
                                            <StatusMessage for="tpAgrupador" />
                                        </div>
                                    </div>

                                    <div className="col-lg-3">
                                        <div className="form-group">
                                            <label htmlFor="nroAgrupador">{t("DOC320_frm1_lbl_NU_AGRUPADOR")}</label>
                                            <Field name="nroAgrupador" readOnly />
                                            <StatusMessage for="nroAgrupador" />
                                        </div>
                                    </div>

                                    <div className="col-lg-3">
                                        <div className="form-group">
                                            <label htmlFor="fechaSalida">{t("DOC320_frm1_lbl_DT_SAIDA")}</label><label style={{ color: "red" }}> *</label>
                                            <FieldDateTime name="fechaSalida" type="input" readOnly />
                                            <StatusMessage for="fechaSalida" />
                                        </div>
                                    </div>

                                    <div className="col-lg-3">
                                        <div className="form-group">
                                            <label htmlFor="cdEmpresa">{t("DOC320_frm1_lbl_CD_EMPRESA")}</label>
                                            <FieldSelectAsync name="cdEmpresa" />
                                            <StatusMessage for="cdEmpresa" />
                                        </div>
                                    </div>

                                    <div className="form-group hidden">
                                        <label htmlFor="idEstado">{t("DOC320_frm1_lbl_ID_ESTADO")}</label>
                                        <Field name="idEstado" type="input" readOnly />
                                        <StatusMessage for="idEstado" />
                                    </div>

                                </div>
                            </div>
                            <div className="col-lg-6">
                                <h4 className="form-title">{t("DOC320_frm1_lbl_Legend2")}</h4>
                                <div className="row">
                                    <div className="col-6">
                                        <div className="form-group">
                                            <label htmlFor="qtPesoLiquido">{t("DOC320_frm1_lbl_QT_PESO_LIQUIDO")}</label><label style={{ color: "red" }}> *</label>
                                            <Field name="qtPesoLiquido" type="input" />
                                            <StatusMessage for="qtPesoLiquido" />
                                        </div>
                                    </div>
                                    <div className="col-6">
                                        <div className="form-group">
                                            <label htmlFor="qtPeso">{t("DOC320_frm1_lbl_QT_PESO")}</label><label style={{ color: "red" }}> *</label>
                                            <Field name="qtPeso" type="input" />
                                            <StatusMessage for="qtPeso" />
                                        </div>
                                    </div>
                                    <div className="col-6">
                                        <div className="form-group">
                                            <label htmlFor="qtVolumen">{t("DOC320_frm1_lbl_QT_VOLUMEN")}</label><label style={{ color: "red" }}> *</label>
                                            <Field name="qtVolumen" type="input" />
                                            <StatusMessage for="qtVolumen" />
                                        </div>
                                    </div>
                                    <div className="col-6">
                                        <div className="form-group">
                                            <label htmlFor="vlTotal">{t("DOC320_frm1_lbl_VL_TOTAL")}</label><label style={{ color: "red" }}> *</label>
                                            <Field name="vlTotal" type="input" />
                                            <StatusMessage for="vlTotal" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div className="col-lg-6">
                                <h4 className="form-title">{t("DOC320_frm1_lbl_Legend3")}</h4>
                                <div className="row">
                                    <div className="col-6">
                                        <div className="form-group">
                                            <label htmlFor="cdTransportadora">{t("DOC320_frm1_lbl_CD_TRANSPORTADORA")}</label><label style={{ color: "red" }}> *</label>
                                            <FieldSelectAsync name="cdTransportadora" />
                                            <StatusMessage for="cdTransportadora" />
                                        </div>
                                    </div>
                                    <div className="col-6">
                                        <div className="form-group">
                                            <label htmlFor="tipoVehiculo">{t("DOC320_frm1_lbl_TP_VEICULO")}</label><label style={{ color: "red" }}> *</label>
                                            <FieldSelectAsync name="tipoVehiculo" />
                                            <StatusMessage for="tipoVehiculo" />
                                        </div>
                                    </div>
                                    <div className="col-12">
                                        <div className="form-group">
                                            <label htmlFor="motorista">{t("DOC320_frm1_lbl_DS_MOTORISTA")}</label><label style={{ color: "red" }}> *</label>
                                            <Field name="motorista" type="input" />
                                            <StatusMessage for="motorista" />
                                        </div>
                                    </div>
                                    <div className="col-6">
                                        <div className="form-group">
                                            <label htmlFor="placa">{t("DOC320_frm1_lbl_DS_PLACA")}</label><label style={{ color: "red" }}> *</label>
                                            <Field name="placa" type="input" />
                                            <StatusMessage for="placa" />
                                        </div>
                                    </div>
                                    <div className="col-6">
                                        <div className="form-group">
                                            <label htmlFor="nroLacre">{t("DOC320_frm1_lbl_NU_LACRE")}</label><label style={{ color: "red" }}> *</label>
                                            <Field name="nroLacre" type="input" />
                                            <StatusMessage for="nroLacre" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div className="col-lg-6">
                                <h4 className="form-title">{t("DOC320_frm1_lbl_Legend4")}</h4>
                                <div className="row">
                                    <div className="col-6">
                                        <div className="form-group">
                                            <label htmlFor="anexo1">{t("DOC320_frm1_lbl_ANEXO1")}</label><label style={{ color: "red" }}> *</label>
                                            <Field name="anexo1" type="input" />
                                            <StatusMessage for="anexo1" />
                                        </div>
                                    </div>
                                    <div className="col-6">
                                        <div className="form-group">
                                            <label htmlFor="anexo2">{t("DOC320_frm1_lbl_ANEXO2")}</label><label style={{ color: "red" }}> *</label>
                                            <Field name="anexo2" type="input" />
                                            <StatusMessage for="anexo2" />
                                        </div>
                                    </div>
                                    <div className="col-6">
                                        <div className="form-group">
                                            <label htmlFor="anexo3">{t("DOC320_frm1_lbl_ANEXO3")}</label>
                                            <Field name="anexo3" type="input" />
                                            <StatusMessage for="anexo3" />
                                        </div>
                                    </div>
                                    <div className="col-6">
                                        <div className="form-group">
                                            <label htmlFor="anexo4">{t("DOC320_frm1_lbl_ANEXO4")}</label>
                                            <Field name="anexo4" type="input" />
                                            <StatusMessage for="anexo4" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div className="col-lg-6" style={{ display: isPredioRequired ? 'block' : 'none' }}>
                                <h4 className="form-title">{t("DOC320_frm1_lbl_Legend5")}</h4>
                                <div className="row">
                                    <div className="col-6">
                                        <div className="form-group">
                                            <label htmlFor="nuPredio">{t("DOC320_frm1_lbl_NU_PREDIO")}</label><label style={{ color: "red" }}> *</label>
                                            <FieldSelect name="nuPredio" />
                                            <StatusMessage for="nuPredio" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>

                        <div className="row">
                            <div className="col">
                                <SubmitButton id="btnSubmit" value={t("DOC320_Sec0_btn_Confirmar")} />
                                &nbsp;
                                <SubmitButton id="BtnConfirmarContinuar" value={t("DOC320_Sec0_btn_ConfirmarContinuar")} className={confirmButtonClassName} style={secondarySubmitStyle} />
                                &nbsp;
                                <Button id="hideFormButton" value={t("DOC320_Sec0_btn_Cancelar")} className="btn btn-danger" />
                            </div>
                        </div>

                    </div>
                </div>

                <hr />




            </Form>


            <Form id="DOC320_form_2"
                initialValues={initialValues_2}
                validationSchema={validationSchema_2}
                onAfterSubmit={onAfterSubmit_2}
                onBeforeSubmit={onBeforeSubmit_2}
            >
                <Modal show={showModal} onHide={handleCloseModal} size={"lg"} aria-labelledby={"contained-modal-title-vcenter"} centered >
                    <Modal.Header closeButton>
                        <Modal.Title id="contained-modal-title-vcenter">
                            <span className="text-muted">{t("DOC320_Modal_lbl_title")} </span>
                        </Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <p>
                            <span className="text-muted">{t("DOC320_Modal_lbl_body")} </span>
                        </p>
                        <Field name="motivoCancelacion" type="textarea" />
                    </Modal.Body>
                    <Modal.Footer>
                        <SubmitButton id="btnSubmit" value={t("DOC320_Sec0_btn_Confirmar")} />
                        <Button id="hideModalButton" value={t("DOC320_Modal_btn_Cancelar")} className="btn btn-light" />
                    </Modal.Footer>
                </Modal>

            </Form>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="DOC320_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onAfterButtonAction={onAfterButtonActionGrid}
                        onBeforeButtonAction={onBeforeGridButtonAction} />
                </div>
            </div>

        </Page>

    );
}