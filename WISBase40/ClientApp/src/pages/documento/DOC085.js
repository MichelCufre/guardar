import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldDate, SubmitButton, FormButton as Button, StatusMessage } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import InputMask from 'react-input-mask';

export default function DOC085(props) {
    const { t } = useTranslation();
    const [isFormEnabled, setFormEnabled] = useState(true);
    const [isShowForm, setShowForm] = useState(false);
    const [isEditing, setEditing] = useState(false);
    const [isAutoGenerado, setAutoGenerado] = useState(true);
    const [rows, setRows] = useState({});

    const secondarySubmitStyle = { width: "300px !important" };
    const fieldSetStyle = { border: "1px solid #ddd", margin: "10px", width: "100%" };

    const initialValues = {
        nroDoc: "",
        nroDocNoGenerado: "",
        tpIngreso: "",
        cdEmpresa: "",
        descEmpresa: "",
        cdCliente: "",
        descProveedor: "",
        fechAlta: "",
        tp_dua: "",
        nroDua: "",
        fechDua: "",
        fechProgramado: "",
        nroExport: "",
        nroImport: "",
        fechMod: "",
        cdDespachante: "",
        descDespachante: "",
        fechEnviado: "",
        nroFactura: "",
        nroConocimiento: "",
        qtVolumen: "",
        qtBultos: "",
        qtPeso: "",
        cdUnidadMedida: "",
        descUniMedida: "",
        vlArbitraje: "",
        qtContenedor20: "",
        qtContenedor40: "",
        tpAlmacSeguro: "",
        descAlmacSeguro: "",
        cdVia: "",
        descVia: "",
        cdTransportadora: "",
        descTransportadora: "",
        descDocumento: "",
        cdMoneda: "",
        descMoneda: "",
        nuDocTransporte: "",
        descAnexo1: "",
        vlSeguro: "",
        nuAgenda: "",
        descAnexo2: "",
        descAnexo3: "",
        descAnexo4: "",
        descAnexo5: "",
        vlFlete: "",
        cdFuncionario: "",
        predio: "",
        vlOtrosGastos: "",
        totalFob: "",
        totalCif: "",
        totalCifLineas: "",
        icms: "",
        ii: "",
        ipi: "",
        iisuspenso: "",
        ipisuspenso: "",
        pisconfins: "",
        cdRegimenAduana: ""
    };
    const validationSchema =
    {
        tpIngreso: Yup.string().required(),
        nroDocNoGenerado: Yup.string().nullable(),
        cdCliente: Yup.string().required(),
        nroExport: Yup.string().nullable(),
        nroImport: Yup.string().nullable(),
        cdDespachante: Yup.string().nullable(),
        nroFactura: Yup.string().nullable(),
        nroConocimiento: Yup.string().nullable(),
        cdUnidadMedida: Yup.string().nullable(),
        cdVia: Yup.string().required(),
        fechProgramado: Yup.string().required(),
        qtVolumen: Yup.string().nullable(),
        vlArbitraje: Yup.string().nullable(),
        descAnexo1: Yup.string().nullable(),
        descAnexo2: Yup.string().nullable(),
        descAnexo3: Yup.string().nullable(),
        descAnexo4: Yup.string().nullable(),
        predio: Yup.string().required(),
        cdEmpresa: Yup.string().required(),
        qtBultos: Yup.string().nullable(),
        qtPeso: Yup.string().nullable(),
        qtContenedor20: Yup.string().nullable(),
        qtContenedor40: Yup.string().nullable(),
        tpAlmacSeguro: Yup.string().nullable(),
        cdTransportadora: Yup.string().nullable(),
        descDocumento: Yup.string().nullable(),
        cdMoneda: Yup.string().nullable(),
        vlSeguro: Yup.string().nullable(),
        vlFlete: Yup.string().nullable(),
        vlOtrosGastos: Yup.string().nullable(),
        icms: Yup.string().nullable(),
        ii: Yup.string().nullable(),
        ipi: Yup.string().nullable(),
        iisuspenso: Yup.string().nullable(),
        ipisuspenso: Yup.string().nullable(),
        pisconfins: Yup.string().nullable(),
        cdRegimenAduana: Yup.string().nullable()
    };

    const onAfterInitialize = (context, form, query, nexus) => {
        setFormEnabled(false);

        const _formatoNumDoc = query.parameters.find(p => p.id === "formatoNumDoc");

        if (_formatoNumDoc && _formatoNumDoc.value) {
            setFormatoNumDoc(_formatoNumDoc.value);
        }

        const _editar = query.parameters.find(p => p.id === "editar");

        if (_editar && _editar.value) {
            setShowForm(_editar.value);
        }
    };
    const onAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            setShowForm(false);
            nexus.getForm("DOC085_form_1").reset();
            nexus.getGrid("DOC085_grid_1").refresh();
        }
    };
    const onBeforeButtonAction = (context, form, query, nexus) => {

        if (query.buttonId === "showFormButton") {
            nexus.getForm("DOC085_form_1").reset();
            setShowForm(true);
            setEditing(true);
        }
        else if (query.buttonId === "hideFormButton") {
            nexus.getForm("DOC085_form_1").reset();
            setEditing(false);
            setShowForm(false);
        }
    };
    const onAfterButtonAction = (data, nexus) => {

        if (data.parameters !== undefined && data.parameters.find(f => f.id === "editar") !== undefined && data.parameters.find(f => f.id === "editar").value === "true" && data.buttonId === "btnEditar") {
            setShowForm(true);
            nexus.getForm("DOC085_form_1").reset();
            setEditing(true);
        }
        else if (data.buttonId === "BtnConfirmarContinuar") {
            setShowForm(false);
            nexus.getForm("DOC085_form_1").reset();
            nexus.getGrid("DOC085_grid_1").refresh();
        }
    };
    const onAfterValidateField = (context, form, query, nexus) => {

        if (query.fieldId === "tpIngreso") {
            const isAutoGeneradoParam = query.parameters.find(p => p.id === "isAutoGenerado");

            if (isAutoGeneradoParam && isAutoGeneradoParam.value === "true") {
                setAutoGenerado(true);
            }
            else if (isAutoGeneradoParam && isAutoGeneradoParam.value === "false") {
                setAutoGenerado(false);
            }
            
            const _formatoNumDoc = query.parameters.find(p => p.id === "formatoNumDoc");

            if (_formatoNumDoc && _formatoNumDoc.value) {
                setFormatoNumDoc(_formatoNumDoc.value);
            }
        }

        query.fieldId == ""


    };

    const formShowButtonClassName = isShowForm ? "hidden" : "";
    const formClassName = isShowForm ? "row mb-5" : "row mb-5 hidden";
    const confirmButtonClassName = isEditing ? "btn btn-warning" : "btn btn-warning hidden";

    const [formatoNumDoc, setFormatoNumDoc] = useState("");
    const [state, setState] = useState('');
    const onChange = (event) => {
        setState(event.target.value);
    }
    const onBeforeSubmit = (context, form, query, nexus) => {
        if (formatoNumDoc) {
            var param = {
                id: "nroDocNoGenerado",
                value: state
            }
            query.parameters.push(param);
        }
    }

    return (
        <Page icon="fas fa-file" title={t("DOC085_Sec0_pageTitle_Titulo")} {...props}>
            <Form id="DOC085_form_1" initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={onAfterSubmit}
                onBeforeButtonAction={onBeforeButtonAction}
                onAfterValidateField={onAfterValidateField}
                onAfterInitialize={onAfterInitialize}
            >
                <div className={formShowButtonClassName} style={{ textAlign: "center" }}>
                    <Button id="showFormButton" value={t("DOC085_Sec0_btn_AgregarNuevoDoc")} className="btn btn-success" style={{ margin: "15px" }} isLoading={isFormEnabled} />
                </div>
                <div className={formClassName}>
                    <div className="col">
                        <div className="row mb-4">
                            <div className="col-lg-8">
                                <h4 className="form-title">{t("DOC085_frm1_lbl_Legend1")}</h4>
                                <div className="row">
                                    <div className="col-lg-6">
                                        <div className="form-group">
                                            <label htmlFor="tpIngreso">{t("DOC085_frm1_lbl_TP_INGRESO")}</label><label style={{ color: "red" }}> *</label>
                                            <FieldSelect name="tpIngreso" />
                                            <StatusMessage for="tpIngreso" />
                                        </div>
                                        <div className="form-group" style={{ display: isAutoGenerado ? 'block' : 'none' }}>
                                            <label htmlFor="nroDoc">{t("DOC085_frm1_lbl_NU_DOCUMENTO")}</label>
                                            <Field name="nroDoc" readOnly />           
                                            <StatusMessage for="nroDoc" />
                                        </div>

                                        <div style={{ display: isAutoGenerado ? 'none' : 'block' }}>
                                            
                                        { formatoNumDoc ? (
                                            <div className="form-group">
                                                <label htmlFor="descDocumento">{t("DOC085_frm1_lbl_NU_DOCUMENTO")}</label>
                                                    <InputMask readonly className="undefined form-control" mask={formatoNumDoc} maskChar={null} value={state} onChange={onChange} />
                                                <Field hidden className="hidden" name="nroDocNoGenerado"/>
                                                <StatusMessage for="nroDocNoGenerado" />
                                            </div>
                                            )
                                                : (
                                            <div className="form-group">
                                                <label htmlFor="descDocumento">{t("DOC085_frm1_lbl_NU_DOCUMENTO")}</label>
                                                <Field name="nroDocNoGenerado" />
                                                <StatusMessage for="nroDocNoGenerado" />
                                            </div>
                                            )
                                        }
                                               
                                        </div>


                                        <div className="form-group">
                                            <label htmlFor="descDocumento">{t("DOC085_frm1_lbl_DS_DOCUMENTO")}</label>
                                            <Field name="descDocumento" />
                                            <StatusMessage for="descDocumento" />
                                        </div>
                                        <div className="form-group">
                                            <label htmlFor="cdEmpresa">{t("DOC085_frm1_lbl_CD_EMPRESA")}</label><label style={{ color: "red" }}> *</label>
                                            <FieldSelectAsync name="cdEmpresa" />
                                            <StatusMessage for="cdEmpresa" />
                                        </div>
                                        <div className="form-group hidden">
                                            <label htmlFor="descEmpresa">{t("DOC085_frm1_lbl_DS_EMPRESA")}</label>
                                            <Field name="descEmpresa" type="input" readOnly />
                                            <StatusMessage for="descEmpresa" />
                                        </div>
                                        <div className="form-group">
                                            <label htmlFor="cdCliente">{t("DOC085_frm1_lbl_CD_CLIENTE")}</label><label style={{ color: "red" }}> *</label>
                                            <FieldSelectAsync name="cdCliente" />
                                            <StatusMessage for="cdCliente" />
                                        </div>
                                        <div className="form-group hidden">
                                            <label htmlFor="descProveedor">{t("DOC085_frm1_lbl_DS_PROVEEDOR")}</label>
                                            <Field name="descProveedor" type="input" readOnly />
                                            <StatusMessage for="descProveedor" />
                                        </div>
                                        <div className="row">
                                            <div className="col-lg-4">
                                                <div className="form-group">
                                                    <label htmlFor="tp_dua">{t("DOC085_frm1_lbl_TP_DUA")}</label>
                                                    <Field name="tp_dua" readOnly />
                                                    <StatusMessage for="tp_dua" />
                                                </div>
                                            </div>
                                            <div className="col-lg-4">
                                                <div className="form-group">
                                                    <label htmlFor="nroDua">{t("DOC085_frm1_lbl_NRO_DUA")}</label>
                                                    <Field name="nroDua" type="input" readOnly />
                                                    <StatusMessage for="nroDua" />
                                                </div>
                                            </div>
                                            <div className="col-lg-4">
                                                <div className="form-group">
                                                    <label htmlFor="fechDua">{t("DOC085_frm1_lbl_DT_DUA")}</label>
                                                    <Field name="fechDua" type="input" readOnly />
                                                    <StatusMessage for="fechDua" />
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                    <div className="col-lg-6">
                                        <div className="form-group">
                                            <label htmlFor="cdDespachante">{t("DOC085_frm1_lbl_CD_DESPACHANTE")}</label>
                                            <FieldSelectAsync name="cdDespachante" />
                                            <StatusMessage for="cdDespachante" />
                                        </div>
                                        <div className="form-group hidden">
                                            <label htmlFor="descDespachante">{t("DOC085_frm1_lbl_DS_DESPACHANTE")}</label>
                                            <Field name="descDespachante" type="input" readOnly />
                                            <StatusMessage for="descDespachante" />
                                        </div>
                                        <div className="row">
                                            <div className="col-lg-6">
                                                <div className="form-group">
                                                    <label htmlFor="cdMoneda">{t("DOC085_frm1_lbl_CD_MONEDA")}</label>
                                                    <FieldSelect name="cdMoneda" />
                                                    <StatusMessage for="cdMoneda" />
                                                </div>
                                            </div>
                                            <div className="col-lg-6">
                                                <div className="form-group">
                                                    <label htmlFor="vlArbitraje">{t("DOC085_frm1_lbl_VL_ARBITRAJE")}</label>
                                                    <Field name="vlArbitraje" type="input" />
                                                    <StatusMessage for="vlArbitraje" />
                                                </div>
                                            </div>
                                        </div>
                                        <div className="form-group hidden">
                                            <label htmlFor="descMoneda">{t("DOC085_frm1_lbl_DS_MONEDA")}</label>
                                            <Field name="descMoneda" type="input" readOnly />
                                            <StatusMessage for="descMoneda" />
                                        </div>
                                        <div className="form-group">
                                            <label htmlFor="nroFactura">{t("DOC085_frm1_lbl_NU_FACTURA")}</label>
                                            <Field name="nroFactura" />
                                            <StatusMessage for="nroFactura" />
                                        </div>
                                        <div className="form-group">
                                            <label htmlFor="fechProgramado">{t("DOC085_frm1_lbl_DT_PROGRAMADO")}</label><label style={{ color: "red" }}> *</label>
                                            <FieldDate name="fechProgramado" />
                                            <StatusMessage for="fechProgramado" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div className="col-lg-4">
                                <div className="form-group">
                                    <label htmlFor="cdUnidadMedida">{t("DOC085_frm1_lbl_CD_UNIDAD_MEDIDA")}</label>
                                    <FieldSelectAsync name="cdUnidadMedida" />
                                    <StatusMessage for="cdUnidadMedida" />
                                </div>
                                <div className="form-group hidden">
                                    <label htmlFor="descUniMedida">{t("DOC085_frm1_lbl_DS_UNIDAD_MEDIDA")}</label>
                                    <Field name="descUniMedida" type="input" readOnly />
                                    <StatusMessage for="descUniMedida" />
                                </div>
                                <div className="row">
                                    <div className="col-md-4">
                                        <div className="form-group">
                                            <label htmlFor="qtBultos">{t("DOC085_frm1_lbl_QT_BULTOS")}</label>
                                            <Field name="qtBultos" />
                                            <StatusMessage for="qtBultos" />
                                        </div>
                                    </div>
                                    <div className="col-md-4">
                                        <div className="form-group">
                                            <label htmlFor="qtVolumen">{t("DOC085_frm1_lbl_QT_VOLUMEN")}</label>
                                            <Field name="qtVolumen" type="input" />
                                            <StatusMessage for="qtVolumen" />
                                        </div>
                                    </div>
                                    <div className="col-md-4">
                                        <div className="form-group">
                                            <label htmlFor="qtPeso">{t("DOC085_frm1_lbl_QT_PESO")}</label>
                                            <Field name="qtPeso" />
                                            <StatusMessage for="qtPeso" />
                                        </div>
                                    </div>
                                </div>


                                <div className="form-group">
                                    <label htmlFor="descAnexo3">{t("DOC085_frm1_lbl_ANEXO_3")}</label>
                                    <Field name="descAnexo3" />
                                    <StatusMessage for="descAnexo3" />
                                </div>

                                <div className="form-group">
                                    <label htmlFor="descAnexo4">{t("DOC085_frm1_lbl_ANEXO_4")}</label>
                                    <Field name="descAnexo4" />
                                    <StatusMessage for="descAnexo4" />
                                </div>
                                <div className="form-group">
                                    <label htmlFor="descAnexo5">{t("DOC085_frm1_lbl_ANEXO_5")}</label>
                                    <Field name="descAnexo5" />
                                    <StatusMessage for="descAnexo5" />
                                </div>
                                <div className="form-group hidden">
                                    <label htmlFor="fechEnviado">{t("DOC085_frm1_lbl_DT_ENVIADO")}</label>
                                    <FieldDate name="fechEnviado" readOnly />
                                    <StatusMessage for="fechEnviado" />
                                </div>
                                <div className="form-group hidden">
                                    <label htmlFor="fechAlta">{t("DOC085_frm1_lbl_DT_ADDROW")}</label>
                                    <FieldDate name="fechAlta" readOnly />
                                    <StatusMessage for="fechAlta" />
                                </div>
                                <div className="form-group hidden">
                                    <label htmlFor="fechMod">{t("DOC085_frm1_lbl_DT_UPDROW")}</label>
                                    <Field name="fechMod" type="input" readOnly />
                                    <StatusMessage for="fechMod" />
                                </div>
                            </div>
                        </div>

                        <div className="row mb-4">
                            <div className="col-lg-6">
                                <h4 className="form-title">{t("DOC085_frm1_lbl_Legend3")}</h4>
                                <div className="form-group">
                                    <label htmlFor="cdVia">Via</label><label style={{ color: "red" }}> *</label>
                                    <FieldSelect name="cdVia" />
                                    <StatusMessage for="cdVia" />
                                </div>
                                <div className="form-group hidden">
                                    <label htmlFor="descVia">{t("DOC085_frm1_lbl_DS_VIA")}</label>
                                    <Field name="descVia" type="input" readOnly />
                                    <StatusMessage for="descVia" />
                                </div>
                                <div className="row">
                                    <div className="col-lg-6">
                                        <div className="form-group">
                                            <label htmlFor="cdTransportadora">{t("DOC085_frm1_lbl_CD_TRANSPORTADORA")}</label>
                                            <FieldSelect name="cdTransportadora" />
                                            <StatusMessage for="cdTransportadora" />
                                        </div>
                                    </div>
                                    <div className="col-lg-6">
                                        <div className="form-group">
                                            <label htmlFor="nuDocTransporte">{t("DOC085_frm1_lbl_NU_DOC_TRANSPORTE")}</label>
                                            <Field name="nuDocTransporte" type="input" readOnly />
                                            <StatusMessage for="nuDocTransporte" />
                                        </div>
                                    </div>
                                </div>
                                <div className="form-group hidden">
                                    <label htmlFor="descTransportadora">{t("DOC085_frm1_lbl_DS_TRANSPORTADORA")}</label>
                                    <Field name="descTransportadora" type="input" readOnly />
                                    <StatusMessage for="descTransportadora" />
                                </div>
                                <div className="form-group">
                                    <label htmlFor="vlFlete">{t("DOC085_frm1_lbl_VL_FLETE")}</label>
                                    <Field name="vlFlete" type="input" />
                                    <StatusMessage for="vlFlete" />
                                </div>
                                <div className="form-group">
                                    <label htmlFor="vlSeguro">{t("DOC085_frm1_lbl_VL_SEGURO")}</label>
                                    <Field name="vlSeguro" type="input" />
                                    <StatusMessage for="vlSeguro" />
                                </div>
                                <div className="form-group">
                                    <label htmlFor="vlOtrosGastos">{t("DOC085_frm1_lbl_VL_OTROS_GASTOS")}</label>
                                    <Field name="vlOtrosGastos" type="input" />
                                    <StatusMessage for="vlOtrosGastos" />
                                </div>
                            </div>

                            <div className="col-lg-6">
                                <h4 className="form-title">{t("DOC085_frm1_lbl_Legend4")}</h4>   
                                <div className="form-group">
                                    <label htmlFor="predio">{t("DOC085_frm1_lbl_NU_PREDIO")}</label>
                                    <FieldSelect name="predio" />
                                    <StatusMessage for="predio" />
                                </div>
                                <div className="form-group">
                                    <label htmlFor="tpAlmacSeguro">{t("DOC085_frm1_lbl_TP_ALMACENAJE_SEGURO")}</label>
                                    <FieldSelect name="tpAlmacSeguro" />
                                    <StatusMessage for="tpAlmacSeguro" />
                                </div>
                                <div className="form-group hidden">
                                    <label htmlFor="descAlmacSeguro">{t("DOC085_frm1_lbl_DS_ALMACENAJE_SEGURO")}</label>
                                    <Field name="descAlmacSeguro" type="input" readOnly />
                                    <StatusMessage for="descAlmacSeguro" />
                                </div>
                                <div className="form-group">
                                    <label htmlFor="descAnexo1">{t("DOC085_frm1_lbl_ANEXO_1")}</label>
                                    <Field name="descAnexo1" />
                                    <StatusMessage for="descAnexo1" />
                                </div>
                                <div className="form-group">
                                    <label htmlFor="descAnexo2">{t("DOC085_frm1_lbl_ANEXO_2")}</label>
                                    <Field name="descAnexo2" />
                                    <StatusMessage for="descAnexo2" />
                                </div>


                                <div className="form-group hidden">
                                    <label htmlFor="nuAgenda">{t("DOC085_frm1_lbl_NU_AGENDA")}</label>
                                    <Field name="nuAgenda" type="input" />
                                    <StatusMessage for="nuAgenda" />
                                </div>
                                <div className="form-group hidden">
                                    <label htmlFor="cdFuncionario">{t("DOC085_frm1_lbl_CD_FUNCIONARIO")}</label>
                                    <Field name="cdFuncionario" type="input" />
                                    <StatusMessage for="cdFuncionario" />
                                </div>
                            </div>
                        </div>

                        <div className="row mb-4">
                            <div className="col">
                                <h4 className="form-title">{t("DOC085_frm1_lbl_Legend5")}</h4>
                                <div className="row">
                                    <div className="col-4">
                                        <div className="form-group">
                                            <label htmlFor="totalFob">{t("DOC085_frm1_lbl_TOTAL_FOB")}</label>
                                            <Field name="totalFob" readOnly />
                                            <StatusMessage for="totalFob" />
                                        </div>
                                    </div>
                                    <div className="col-4">
                                        <div className="form-group">
                                            <label htmlFor="totalCif">{t("DOC085_frm1_lbl_TOTAL_CIF")}</label>
                                            <Field name="totalCif" type="input" readOnly />
                                            <StatusMessage for="totalCif" />
                                        </div>
                                    </div>
                                    <div className="col-4">
                                        <div className="form-group">
                                            <label htmlFor="totalCifLineas">{t("DOC085_frm1_lbl_TOTAL_CIF_LINEAS")}</label>
                                            <Field name="totalCifLineas" type="input" readOnly />
                                            <StatusMessage for="totalCifLineas" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div className="row">
                            <div className="col">
                                <SubmitButton id="btnSubmit" value={t("DOC085_Sec0_btn_CONFIRMAR")} />
                                &nbsp;
                                <Button id="BtnConfirmarContinuar" value={t("DOC085_Sec0_btn_CONFIRMAR_CONTINUAR")} className={confirmButtonClassName} style={secondarySubmitStyle} />
                                &nbsp;
                                <Button id="hideFormButton" value={t("DOC085_Sec0_btn_CANCELAR")} className="btn btn-danger" />
                            </div>
                        </div>
                    </div>
                </div>
                <hr />
            </Form>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="DOC085_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onAfterButtonAction={onAfterButtonAction}
                        enableExcelExport
                    />
                </div>
            </div>
        </Page>
    );
}