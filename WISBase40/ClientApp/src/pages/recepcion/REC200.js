import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, FormButton, FieldSelect, StatusMessage, FieldToggle } from '../../components/FormComponents/Form';
import { Row, Col } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Container } from 'react-bootstrap';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import { right } from '@popperjs/core';

export default function REC200(props) {
    const { t } = useTranslation();
    const [agenda, setAgenda] = useState(null);
    const [seleccionTipo, setSeleccionTipo] = useState(null);
    const [isInfoDisplayed, setIsInfoDisplayed] = useState(false);
    const [noPuedeSubmit, setNoPuedeSubmit] = useState(false);
    const [crossDockingType, setCrossDockingType] = useState("");
    const [consumirOtrosDocs, setConsumirOtrosDocs] = useState("");

    const initialValues = {
        tpSeleccion: "",
        agenda: "",
        consumirOtrosDocs: "",

    };
    const [infoDetalle, setInfoDetalle] = useState({
        NU_AGENDA: "", CD_EMPRESA: "", NM_EMPRESA: "", NU_DOCUMENTO: ""
    });

    const validationSchema = {
        tpSeleccion: Yup.string(),
        consumirOtrosDocs: Yup.boolean(),
    };

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        setAgenda(parameters.find(d => d.id === "NU_AGENDA").value);

        if (parameters.find(d => d.id === "REC200_NU_AGENDA") != null) {

            setInfoDetalle({
                NU_AGENDA: parameters.find(d => d.id === "REC200_NU_AGENDA").value,
                CD_EMPRESA: parameters.find(d => d.id === "REC200_CD_EMPRESA").value,
                NM_EMPRESA: parameters.find(d => d.id === "REC200_NM_EMPRESA").value,
                NU_DOCUMENTO: parameters.find(d => d.id === "REC200_NU_DOCUMENTO").value,
            });

            setIsInfoDisplayed(true);
        } else {

            setIsInfoDisplayed(false);
        }
    };

    const onAfterPageLoad = (data) => {
        setAgenda(data.parameters.find(d => d.id === "agenda").value);
    };

    const addParameters = (context, data, nexus) => {
        data.parameters = [
            {
                id: "tpSeleccion",
                value: seleccionTipo //|| form.fields.find(d => d.id === "tpSeleccion").value
            },
            {
                id: "NU_AGENDA",
                value: agenda || "-1"
            },
            {
                id: "tpCrossDocking",
                value: crossDockingType
            },
            {
                id: "consumirOtrosDocs",
                value: consumirOtrosDocs
            }
        ];
    };

    const onBeforeFormButtonAction = (context, form, query, nexus) => {

        if (query.buttonId == "btnIniciarCrossDocking" && noPuedeSubmit) {
            context.abortServerCall = true;
        }
        else {
            setNoPuedeSubmit(true);

            query.parameters = [
                {
                    id: "NU_AGENDA",
                    value: agenda || "-1"
                },
                {
                    id: "tpCrossDocking",
                    value: crossDockingType
                },
                {
                    id: "consumirOtrosDocs",
                    value: consumirOtrosDocs
                }
            ];
        }
    };

    const onAfterFormButtonAction = (context, form, query, nexus) => {

        if (query.buttonId == "btnIniciarCrossDocking") {
            setNoPuedeSubmit(false);
            nexus.getGrid("REC200_grid_1").refresh();
            nexus.getGrid("REC200_grid_2").refresh();
        }

    };

    const onAfterMenuItemAction = (context, data, nexus) => {

        nexus.getGrid("REC200_grid_1").refresh();
        nexus.getGrid("REC200_grid_2").refresh();
    };

    const onBeforeValidateField = (context, form, query, nexus) => {
        context.abortServerCall = true;

        setSeleccionTipo(form.fields.find(d => d.id === "tpSeleccion").value);
        setConsumirOtrosDocs(form.fields.find(d => d.id === "consumirOtrosDocs").value);

        if (query.fieldId != "consumirOtrosDocs") {
            nexus.getGrid("REC200_grid_1").refresh();
            nexus.getGrid("REC200_grid_2").refresh();
        }
    };

    const onAfterInitializeForm = (context, form, query, nexus) => {
        if (query.parameters.some(d => d.id === "tpCrossDockinDefault")) {
            setCrossDockingType(query.parameters.find(d => d.id === "tpCrossDockinDefault").value);
        }
        if (query.parameters.some(d => d.id === "tpCrossDockinDefault")) {
            setCrossDockingType(query.parameters.find(d => d.id === "tpCrossDockinDefault").value);
        }
    };

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("REC200_grid_1").triggerMenuAction("btnAgregarPedido", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("REC200_grid_2").triggerMenuAction("btnQuitarPedido", false, evt.ctrlKey);
    };
    return (
        <Page
            load
            title={t("REC200_Sec0_pageTitle_Titulo")}
            {...props}
            onAfterLoad={onAfterPageLoad}
        >
            <Container fluid style={{ display: isInfoDisplayed ? 'block' : 'none' }} >
                <Row>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC210_Sec0_Info_Cabezal_Agenda")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalle.NU_AGENDA}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC210_Sec0_Info_Cabezal_Empresa")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalle.CD_EMPRESA}`} - {`${infoDetalle.NM_EMPRESA}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC200_Sec0_Info_Cabezal_Documento")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalle.NU_DOCUMENTO}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
            <hr style={{ display: isInfoDisplayed ? 'block' : 'none' }}></hr>

            <Row>
                <Col>
                    <Form
                        id="REC200_form_1"
                        initialValues={initialValues}
                        validationSchema={validationSchema}
                        onBeforeButtonAction={onBeforeFormButtonAction}
                        onAfterButtonAction={onAfterFormButtonAction}
                        onBeforeValidateField={onBeforeValidateField}
                        onAfterInitialize={onAfterInitializeForm}
                    >
                        <Row>
                            <Col lg="3">
                                <div className="form-group">
                                    <label style={{ fontWeight: "bold" }} htmlFor="tpSeleccion">{t("REC200_frm1_lbl_TipoSeleccion")}</label>
                                    <FieldSelect name="tpSeleccion" />
                                    <StatusMessage for="tpSeleccion" />
                                </div>
                            </Col>

                            <Col lg="3">
                                <div className="form-group">
                                    <label htmlFor="tpCrossDocking">{t("REC200_frm1_lbl_TipoCrossDocking")}</label>
                                    <FieldSelect name="tpCrossDocking" />
                                    <StatusMessage for="tpCrossDocking" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="consumirOtrosDocs">{t("REC200_frm1_lbl_consumirOtrosDocs")}</label>
                                    <FieldToggle name="consumirOtrosDocs" />
                                </div>
                            </Col>
                        </Row>
                    </Form>
                </Col>
            </Row>

            <br />
            <AddRemovePanel
                onAdd={handleAdd}
                onRemove={handleRemove}
                from={(
                    <Grid
                        application="REC200"
                        id="REC200_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onBeforeInitialize={addParameters}
                        onBeforeFetch={addParameters}
                        onBeforeExportExcel={addParameters}
                        onBeforeMenuItemAction={addParameters}
                        onBeforeApplyFilter={addParameters}
                        onBeforeApplySort={addParameters}
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        onAfterInitialize={onAfterInitialize}
                        enableExcelExport
                        enableSelection
                    />
                )}
                to={(
                    <Grid
                        application="REC200"
                        id="REC200_grid_2"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onBeforeInitialize={addParameters}
                        onBeforeFetch={addParameters}
                        onBeforeExportExcel={addParameters}
                        onBeforeMenuItemAction={addParameters}
                        onBeforeApplyFilter={addParameters}
                        onBeforeApplySort={addParameters}
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        enableExcelExport
                        enableSelection
                    />
                )}
            />

            <Row>
                <Col>
                    <Form
                        id="REC200_form_2"
                        initialValues={initialValues}
                        onBeforeButtonAction={onBeforeFormButtonAction}
                        onAfterButtonAction={onAfterFormButtonAction}
                        onBeforeValidateField={onBeforeValidateField}
                    >
                        <div style={{ float: "right" }} className="form-group">
                            <FormButton id="btnIniciarCrossDocking" label="REC200_frm1_btn_IniciarCrossDocking" />
                        </div>
                    </Form>
                </Col>
            </Row>
        </Page>
    );
}