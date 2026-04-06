import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Page } from '../../components/Page';
import { Form, Field, FieldDate, FieldDateTime, FieldCheckbox, FieldSelect, FieldSelectAsync, FormButton, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import { Row, Col, FormGroup, Button } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { FieldFile } from '../../components/FormComponents/FormFieldFile';

export default function EVT000(props) {
    const { t } = useTranslation();

    const [ipCompartida, setIpCompartida] = useState(null);

    const [COL1, setCol1] = useState(null);
    const [COL2, setCol2] = useState(null);
    const [COL3, setCol3] = useState(null);
    const [COL4, setCol4] = useState(null);

    const [COL5, setCol5] = useState(null);
    const [COL6, setCol6] = useState(null);
    const [COL7, setCol7] = useState(null);
    const [COL8, setCol8] = useState(null);

    const [file, setFile] = useState("{}");

    const [flAux, setFlAux] = useState("F");

    const PageOnAfterLoad = (data) => {
        setIpCompartida(data.parameters.find(d => d.id === "IP_COMPARTIDA").value);

        if (data.parameters.some(d => d.id === "FL_AUX" && d.value !== "F")) {
            setFlAux("I");

            setCol1(data.parameters.some(d => d.id === "COL1") ? data.parameters.find(d => d.id === "COL1").value : null);
            setCol2(data.parameters.some(d => d.id === "COL2") ? data.parameters.find(d => d.id === "COL2").value : null);
            setCol3(data.parameters.some(d => d.id === "COL3") ? data.parameters.find(d => d.id === "COL3").value : null);
            setCol4(data.parameters.some(d => d.id === "COL4") ? data.parameters.find(d => d.id === "COL4").value : null);
            setCol5(data.parameters.some(d => d.id === "COL5") ? data.parameters.find(d => d.id === "COL5").value : null);
            setCol6(data.parameters.some(d => d.id === "COL6") ? data.parameters.find(d => d.id === "COL6").value : null);
            setCol7(data.parameters.some(d => d.id === "COL7") ? data.parameters.find(d => d.id === "COL7").value : null);
            setCol8(data.parameters.some(d => d.id === "COL8") ? data.parameters.find(d => d.id === "COL8").value : null);
        }
        else {
            setFlAux("F");
        }

    };

    const validationSchema = {

        TP_DOCUMENTO: Yup.string().required().max(50),
        DS_OBSERVACION: Yup.string().nullable().max(200),
        DS_REFERENCIA2: Yup.string().nullable().max(300),
        DS_ANEXO: Yup.string().nullable().max(200),

    };

    const onAfterSubmit = (context, form, query, nexus) => {

        if (context.status === "ERROR") return;

        if (query.resetForm) {
            //setFlAux("F");
        }

        nexus.getGrid("EVT000_grid_1").refresh();
    };
    const onAfterButtonAction = (data, nexus) => {

        if (data.buttonId === "btnBorrar" || data.buttonId === "btnInactivar") {
            nexus.getGrid("EVT000_grid_1").refresh();
        }
        else if (data.buttonId === "btnVideo") {

            try {
            const el = document.createElement('textarea');
                el.value = data.parameters.find(w => w.id === "LK_RUTA").value;
            document.body.appendChild(el);
            el.select();
            document.execCommand('copy');
            document.body.removeChild(el);
            } catch (err) {
            }
        }
    };
    const GridOnBeforeButtonAction = (context, data, nexus) => {

        context.abortServerCall = true;

        if (data.buttonId === "btnEditar") {
            nexus.getForm("EVT000_form_1").reset([
                { id: "FORM_UPDATE", value: "S" },
                { id: "NU_ARCHIVO_ADJUNTO", value: data.row.cells.find(w => w.column == "NU_ARCHIVO_ADJUNTO").value },
                { id: "CD_EMPRESA", value: data.row.cells.find(w => w.column == "CD_EMPRESA").value },
                { id: "CD_MANEJO", value: data.row.cells.find(w => w.column == "CD_MANEJO").value },
                { id: "DS_REFERENCIA", value: data.row.cells.find(w => w.column == "DS_REFERENCIA").value },

            ]);
        }
        else if (data.buttonId === "btnVer") {
            var link = data.row.cells.find(w => w.column == "LK_RUTA").value;
            window.open(ipCompartida + link.replace(/#/gi, '%23'), "_newtab");
        }
        else if (data.buttonId === "btnDetalles") {
            context.abortServerCall = false;


        }
        else if (data.buttonId === "btnBorrar") {
            context.abortServerCall = false;


        }
        else if (data.buttonId === "btnInactivar") {
            context.abortServerCall = false;

        }
        else if (data.buttonId === "btnVideo") {
            context.abortServerCall = false;
        }

    };

    const FormOnBeforeButtonAction = (context, form, query, nexus) => {

        context.abortServerCall = true;

        if (query.buttonId === "btnFiltrar") {
            nexus.getGrid("EVT000_grid_1").refresh();
        }
        else if (query.buttonId === "btnCancelar") {
            nexus.getForm("EVT000_form_1").reset();
            setFlAux("F");
        }

    };

    const addParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "FL_AUX", flAux },
            { id: "CD_EMPRESA", value: nexus.getForm("EVT000_form_1").getFieldValue("CD_EMPRESA") },
            { id: "DS_REFERENCIA1", value: nexus.getForm("EVT000_form_1").getFieldValue("DS_REFERENCIA1") },
            { id: "TP_DOCUMENTO", value: nexus.getForm("EVT000_form_1").getFieldValue("TP_DOCUMENTO") },
            { id: "VL_FILTER", value: nexus.getForm("EVT000_form_1").getFieldValue("VL_FILTER") },
        ];
    };

    const FormOnAfterInitialize = (context, form, query, nexus) => {

        if (query.parameters.some(d => d.id === "FL_AUX")) {
            setFlAux("U");
        }

    };

    const onBeforeSubmit = (context, form, query, nexus) => {

        query.parameters = [
            { id: "FL_AUX", value: flAux },
            { id: "FILE", value: file },
        ];

    };

    return (

        <Page
            title={t("EVT000_Sec0_pageTitle_Titulo")}
            {...props}
            onAfterLoad={PageOnAfterLoad}
            load
        >

            <Form
                id="EVT000_form_1"
                validationSchema={validationSchema}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={onAfterSubmit}
                onBeforeButtonAction={FormOnBeforeButtonAction}
                onAfterInitialize={FormOnAfterInitialize}
            >

                <div className={(flAux == "I" || flAux == "U") ? "" : "hidden"}>

                    <Row>
                        {(COL1) &&
                            <Col>
                                <p>{`${COL1}`}</p>
                            </Col>
                        }
                        {(COL2) &&
                            <Col>
                                <p>{`${COL2}`}</p>
                            </Col>
                        }
                        {(COL3) &&
                            <Col>
                                <p>{`${COL3}`}</p>
                            </Col>
                        }
                        {(COL4) &&
                            <Col>
                                <p>{`${COL4}`}</p>
                            </Col>
                        }
                    </Row>

                    <Row>

                        {(COL5) &&
                            <Col>
                                <p>{`${COL5}`}</p>
                            </Col>
                        }

                        {(COL6) &&
                            <Col>
                                <p>{`${COL6}`}</p>
                            </Col>
                        }
                        {(COL7) &&
                            <Col>
                                <p>{`${COL7}`}</p>
                            </Col>
                        }
                        {(COL8) &&
                            <Col>
                                <p>{`${COL8}`}</p>
                            </Col>
                        }

                    </Row >

                    <hr />

                    <Row>
                        <Col lg="4">
                            <FormGroup>
                                <label htmlFor="ARCHIVO">{t("EVT000_frm1_lbl_ARCHIVO")}</label>
                                <FieldFile name="ARCHIVO" />
                                <StatusMessage for="ARCHIVO" />
                            </FormGroup>
                        </Col>
                        <Col lg="8">
                            <FormGroup>
                                <label htmlFor="DS_OBSERVACION">{t("EVT000_frm1_lbl_DS_OBSERVACION")}</label>
                                <Field name="DS_OBSERVACION" />
                                <StatusMessage for="DS_OBSERVACION" />
                            </FormGroup>
                        </Col>
                    </Row>
                </div >

                <Row>

                    <Col>
                        <FormGroup>
                            <label htmlFor="TP_DOCUMENTO">{t("EVT000_frm1_lbl_TP_DOCUMENTO")}</label>
                            <FieldSelect name="TP_DOCUMENTO" isClearable />
                            <StatusMessage for="TP_DOCUMENTO" />
                        </FormGroup>
                    </Col>

                    <div className={(flAux != "F") ? "col" : "hidden"}>
                        <FormGroup>
                            <label htmlFor="DS_REFERENCIA2">{t("EVT000_frm1_lbl_VL_FILTER")}</label>
                            <Field name="DS_REFERENCIA2" />
                            <StatusMessage for="DS_REFERENCIA2" />
                        </FormGroup>
                    </div>

                    <div className={(flAux == "F") ? "col" : "hidden"}>
                        <FormGroup>
                            <label htmlFor="VL_FILTER">{t("EVT000_frm1_lbl_VL_FILTER")}</label>
                            <Field name="VL_FILTER" />
                            <StatusMessage for="VL_FILTER" />
                        </FormGroup>
                    </div>

                    <div className={(flAux == "F") ? "col" : "hidden"}>
                        <label>{'   '}</label>
                        <FormGroup>
                            <FormButton id="btnFiltrar" variant="primary" label="General_Sec0_btn_Filtrar" />
                        </FormGroup>
                    </div>

                </Row>

                <div className={(flAux == "I" || flAux == "U") ? "" : "hidden"}>

                    <Row>
                        <Col>
                            <SubmitButton id="btnConfirmar" variant="primary" label="General_Sec0_btn_Confirmar" />
                            {'   '}
                            <FormButton id="btnCancelar" variant="outline-primary" label="General_Sec0_btn_Cancelar" />
                        </Col>
                    </Row>

                </div>

            </Form >

            <hr />

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="EVT000_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onBeforeButtonAction={GridOnBeforeButtonAction}
                        onAfterButtonAction={onAfterButtonAction}
                        onBeforeInitialize={addParameters}
                        onBeforeFetch={addParameters}
                        onBeforeFetchStats={addParameters}
                        onBeforeMenuItemAction={addParameters}
                        enableSelection
                    />
                </div>
            </div>

        </Page >
    );
}
