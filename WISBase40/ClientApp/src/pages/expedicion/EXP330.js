import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Field, Form, FormButton, StatusMessage } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';
import { LOVSelector } from '../../components/LOVSelector';
import { Page } from '../../components/Page';

export default function EXP330(props) {
    const { t } = useTranslation();
    const fieldSetStyle = { border: "1px solid #ddd" };

    const [NuPedido, setNuPedido] = useState(null);
    const [CdCliente, setCdCliente] = useState(null);
    const [CdEmpresa, setCdEmpresa] = useState(null);
    const [DsCliente, setDsCliente] = useState(null);
    const [NmEmpresa, setNmEmpresa] = useState(null);
    const [isShowGrid3, setShowGrid3] = useState(false);
    const [noPuedeFacturar, setNoPuedeFacturar] = useState(false);
    const [showLOV, setShowLOV] = useState(false);
    const [empresas, setEmpresas] = useState([]);
    const [NuContenedor, setNuContenedor] = useState(null);
    const [nexus, setNexus] = useState(null);
    const [empresaSubmit, setEmpresaSubmit] = useState(null);

    useEffect(() => {
        if (empresaSubmit && nexus) {
            nexus.getForm("EXP330_form_1").submit();
        }
    }, [empresaSubmit]);

    const Grid3ShowClassName = isShowGrid3 ? "row mb-4" : "hidden";

    const initialValues = {
        NuContenedor: "",
        NuPedido: "",
        CdCliente: "",
        DsCliente: "",
        CdEmpresa: "",
        NmEmpresa: "",
    };

    const onBeforeFetch = (context, data, nexus) => {
        data.parameters = [
            { id: "NuPedido", value: NuPedido },
            { id: "CdCliente", value: CdCliente },
            { id: "CdEmpresa", value: CdEmpresa },
            { id: "DsCliente", value: DsCliente },
            { id: "NmEmpresa", value: NmEmpresa },
        ];
    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "empresaSubmit", value: empresaSubmit },
        ];            
    }

    const onAfterSubmit = (context, form, query, nexus) => {

        setNoPuedeFacturar(false);
        setEmpresaSubmit(null);

        if (query.parameters.length > 0) {

            if (query.parameters.some(d => d.id === "empresas")) {
                setNexus(nexus);
                setEmpresas(JSON.parse(query.parameters.find(d => d.id === "empresas").value));
                setShowLOV(true);
                setNuContenedor(query.parameters.find(d => d.id === "NuContenedor").value);
            }
            else if (context.responseStatus === "ERROR") return;
            else {
                setEmpresas([]);
                setNuContenedor(null);

                nexus.getForm("EXP330_form_1").reset(query.parameters);

                const _NuPedido = query.parameters.find(d => d.id === "NuPedido");
                const _CdCliente = query.parameters.find(d => d.id === "CdCliente");
                const _CdEmpresa = query.parameters.find(d => d.id === "CdEmpresa");
                const _DsCliente = query.parameters.find(d => d.id === "DsCliente");
                const _NmEmpresa = query.parameters.find(d => d.id === "NmEmpresa");
                const _NuContenedor = query.parameters.find(d => d.id === "NuContenedor");
                const _NuPreparacion = query.parameters.find(d => d.id === "NuPreparacion");

                setNuPedido(_NuPedido ? _NuPedido.value : null);
                setCdCliente(_CdCliente ? _CdCliente.value : null);
                setCdEmpresa(_CdEmpresa ? _CdEmpresa.value : null);
                setDsCliente(_DsCliente ? _DsCliente.value : null);
                setNmEmpresa(_NmEmpresa ? _NmEmpresa.value : null);

                window.localStorage.setItem('NuPedido', _NuPedido ? _NuPedido.value : null);
                window.localStorage.setItem('CdCliente', _CdCliente ? _CdCliente.value : null);
                window.localStorage.setItem('CdEmpresa', _CdEmpresa ? _CdEmpresa.value : null);
                window.localStorage.setItem('DsCliente', _DsCliente ? _DsCliente.value : null);
                window.localStorage.setItem('NmEmpresa', _NmEmpresa ? _NmEmpresa.value : null);
                window.localStorage.setItem('NuContenedor', _NuContenedor ? _NuContenedor.value : null);
                window.localStorage.setItem('NuPreparacion', _NuPreparacion ? _NuPreparacion.value : null);

                nexus.getGrid("EXP330_grid_1").refresh();
                nexus.getGrid("EXP330_grid_2").refresh();
                nexus.getGrid("EXP330_grid_3").refresh();
            }
        }
    };

    const validationSchema = {
    };

    const onAfterButtonAction = (context, form, query, nexus) => {

        setNoPuedeFacturar(false);

        nexus.getForm("EXP330_form_1").reset(query.parameters);

        const _NuPedido = query.parameters.find(d => d.id === "NuPedido");
        const _CdCliente = query.parameters.find(d => d.id === "CdCliente");
        const _CdEmpresa = query.parameters.find(d => d.id === "CdEmpresa");
        const _DsCliente = query.parameters.find(d => d.id === "DsCliente");
        const _NmEmpresa = query.parameters.find(d => d.id === "NmEmpresa");

        setNuPedido(_NuPedido ? _NuPedido.value : null);
        setCdCliente(_CdCliente ? _CdCliente.value : null);
        setCdEmpresa(_CdEmpresa ? _CdEmpresa.value : null);
        setDsCliente(_DsCliente ? _DsCliente.value : null);
        setNmEmpresa(_NmEmpresa ? _NmEmpresa.value : null);

        nexus.getGrid("EXP330_grid_1").refresh();
        nexus.getGrid("EXP330_grid_2").refresh();
        nexus.getGrid("EXP330_grid_3").refresh();
    };

    const onBeforeButtonAction = (context, form, query, nexus) => {

        if (query.buttonId == "BtnFacturar" && noPuedeFacturar) {
            context.abortServerCall = true;
        }
        else {
            setNoPuedeFacturar(true);
        }

        query.parameters = [
            { id: "NuPedido", value: window.localStorage.getItem('NuPedido') },
            { id: "CdCliente", value: window.localStorage.getItem('CdCliente') },
            { id: "CdEmpresa", value: window.localStorage.getItem('CdEmpresa') },
            { id: "NmEmpresa", value: window.localStorage.getItem('NmEmpresa') },
            { id: "DsCliente", value: window.localStorage.getItem('DsCliente') },
            { id: "NuContenedor", value: window.localStorage.getItem('NuContenedor') },
            { id: "NuPreparacion", value: window.localStorage.getItem('NuPreparacion') }
        ];
    };

    const onBeforeValidateField = (context, form, query, nexus) => {
        context.abortServerCall = true;
    }

    const Form2OnBeforeButtonAction = (context, form, query, nexus) => {
        context.abortServerCall = true;
        setShowGrid3(!isShowGrid3);
    };

    const FormOnBeforeInitialize = (context, form, query, nexus) => {
        context.abortServerCall = true;
    };

    const handleOnHideLOV = () => {
        setCdEmpresa(null);
        setEmpresaSubmit(null);
        setNuContenedor(null);
        setEmpresas([]);
        setShowLOV(false);
    };

    const handleOnSelectLOV = (value) => {
        if (value) {
            setCdEmpresa(value);
            setEmpresaSubmit(value);
            setShowLOV(false);
        }
    };

    return (

        <Page
            title={t("EXP330_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <div className="modal-body row">
                <div className="col-8">
                    <Form
                        id="EXP330_form_1"
                        validationSchema={validationSchema}
                        initialValues={initialValues}
                        onBeforeSubmit={onBeforeSubmit}
                        onAfterSubmit={onAfterSubmit}
                        onAfterButtonAction={onAfterButtonAction}
                        onBeforeButtonAction={onBeforeButtonAction}
                        onBeforeValidateField={onBeforeValidateField}
                    >
                        <div className="row p-5"  >
                            <div className="col-10">

                                <fieldset className="row col-12" style={fieldSetStyle}>
                                    <div className="col-12">
                                        <div className="row col-12">
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label htmlFor="NuContenedor">{t("EXP330_frm1_lbl_NU_CONTENEDOR")}</label>
                                                    <Field name="NuContenedor" value={NuContenedor} />
                                                    <StatusMessage for="NuContenedor" />
                                                </div>
                                            </div>
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label htmlFor="NuPedido">{t("EXP330_frm1_lbl_NU_PEDIDO")}</label>
                                                    <Field name="NuPedido" value={NuPedido} readOnly />
                                                    <StatusMessage for="NuPedido" />
                                                </div>
                                            </div>

                                        </div>

                                        <div className="row col-12">
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label htmlFor="CdCliente">{t("EXP330_frm1_lbl_CD_CLIENTE")}</label>
                                                    <Field name="CdCliente" value={CdCliente} readOnly />
                                                    <StatusMessage for="CdCliente" />
                                                </div>
                                            </div>
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label htmlFor="DsCliente">{t("EXP330_frm1_lbl_DS_CLIENTE")}</label>
                                                    <Field name="DsCliente" value={DsCliente} readOnly />
                                                    <StatusMessage for="DsCliente" />
                                                </div>
                                            </div>
                                        </div>

                                        <div className="row col-12">
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label htmlFor="CdEmpresa">{t("EXP330_frm1_lbl_CD_EMPRESA")}</label>
                                                    <Field name="CdEmpresa" value={CdEmpresa} readOnly />
                                                    <StatusMessage for="CdEmpresa" />
                                                </div>
                                            </div>
                                            <div className="col-6">
                                                <div className="form-group">
                                                    <label htmlFor="NmEmpresa">{t("EXP330_frm1_lbl_DS_EMPRESA")}</label>
                                                    <Field name="NmEmpresa" value={NmEmpresa}  readOnly />
                                                    <StatusMessage for="NmEmpresa" />
                                                </div>
                                            </div>
                                        </div>
                                        <button type="submit" hidden></button>
                                    </div>
                                </fieldset>
                            </div>

                            <div className="col-2 btn-group-vertical">
                                <div className="btn-group">
                                    <FormButton id="BtnDescartar" label="EXP330_frm1_btn_Descartar" />&nbsp;
                                </div>&nbsp;
                                <div className="btn-group">
                                    <FormButton id="BtnExpedir" label="EXP330_frm1_btn_Expedir" />&nbsp;
                                </div>&nbsp;
                                <div className="btn-group">
                                    <FormButton id="BtnFacturar" label="EXP330_frm1_btn_Facturar" />&nbsp;
                                </div>

                            </div>
                        </div>


                    </Form>

                </div>
                <div className="col-4 mt-3">
                    <Grid id="EXP330_grid_2" rowsToFetch={10} rowsToDisplay={10} onBeforeFetch={onBeforeFetch} />
                </div>
            </div>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="EXP330_grid_1" rowsToFetch={30} rowsToDisplay={15} onBeforeFetch={onBeforeFetch} />
                </div>
            </div>

            <Form onBeforeButtonAction={Form2OnBeforeButtonAction} onBeforeInitialize={FormOnBeforeInitialize}>
                <div style={{ textAlign: "center" }}>
                    <FormButton id="btnShowGrid3" label="EXP330_frm2_lbl_BtnShowGrid3" variant="success" className="mb-4" />
                </div>
            </Form>

            <div className={Grid3ShowClassName}>
                <div className="col-12">
                    <Grid id="EXP330_grid_3" rowsToFetch={30} rowsToDisplay={15} onBeforeFetch={onBeforeFetch} />
                </div>
            </div>

            <LOVSelector
                show={showLOV}
                items={empresas}
                onHide={handleOnHideLOV}
                onSelect={handleOnSelectLOV}
                title={t("General_Sec0_modalTitle_EmpresaSelector")}
            ></LOVSelector>
        </Page>
    );

}