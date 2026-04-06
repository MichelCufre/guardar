import React, { useRef } from 'react';
import { Col, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldSelect, FieldSelectAsync, Form, FormButton, StatusMessage } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';

export default function PRE640(props) {
    const { t } = useTranslation();


    const refFiltro = useRef(null);
    refFiltro.current = {
        USERID: "", CD_EMPRESA: "", NU_PREDIO: "", FL_FINALIZADOS: ""
    };

    const initialValues = {
    };

    const validationSchema = {
        USERID: Yup.string(),
        CD_EMPRESA: Yup.string(),
        NU_PREDIO: Yup.string(),
        FL_FINALIZADOS: Yup.string(),
    };

    const addParameters = (context, data, nexus) => {

        if (data.parameters.length == 0) {

            data.parameters = [
                {
                    id: "FILTROS",
                    value: JSON.stringify(refFiltro.current)
                }
            ];
        }

    };

    const formOnBeforeInitialize = (context, form, query, nexus) => {

        query.parameters = [
            {
                id: "FILTROS",
                value: JSON.stringify(refFiltro.current)
            }
        ];
    }

    const formOnAfterInitialize = (context, form, data, nexus) => {
        const param = data.parameters.find(d => d.id === "FILTROS");
        const filtros = JSON.parse(param.value);

        refFiltro.current.USERID = filtros.USERID;
        refFiltro.current.CD_EMPRESA = filtros.CD_EMPRESA;
        refFiltro.current.NU_PREDIO = filtros.NU_PREDIO;
        refFiltro.current.FL_FINALIZADOS = filtros.FL_FINALIZADOS;
    }

    const onBeforeValidateField = (context, form, query, nexus) => {
        context.abortServerCall = true;
        context.forceUpdateFields = true;

        if (query.fieldId === "USERID") {
            refFiltro.current.USERID = form.fields.find(w => w.id === "USERID").value;
        } else if (query.fieldId === "CD_EMPRESA") {
            refFiltro.current.CD_EMPRESA = form.fields.find(w => w.id === "CD_EMPRESA").value;
        } else if (query.fieldId === "NU_PREDIO") {
            refFiltro.current.NU_PREDIO = form.fields.find(w => w.id === "NU_PREDIO").value;
        }
        else if (query.fieldId === "FL_FINALIZADOS") {
            refFiltro.current.FL_FINALIZADOS = form.fields.find(w => w.id === "FL_FINALIZADOS").value;
        }

        setTimeout(() => {
            nexus.getGrid("PRE640_grid_1").reset([{ id: "FILTROS", value: JSON.stringify(refFiltro.current) }]);
            nexus.getForm("PRE640_form_1").clickButton("btnFiltrar");
        }, 100);

    };

    const onBeforeButtonAction = (context, form, query, nexus) => {

        query.parameters = [
            {
                id: "FILTROS",
                value: JSON.stringify(refFiltro.current)
            }
        ];
    }

    const onAfterButtonAction = (context, form, query, nexus) => {

    }

    //const onAfterInitialize = (context, grid, parameters, nexus) => {
    //    const param = parameters.find(d => d.id === "preSeleccion");
    //    const selection = JSON.parse(param.value);
    //    nexus.getGrid("PRE640_grid_1").setSelection(false, selection);
    //};

    //const onAfterMenuItemAction = (context, data, nexus) => {
    //    nexus.getForm("PRE640_form_1").clickButton("btnFiltrar");
    //    nexus.getGrid("PRE640_grid_1").refresh();
    //};

    return (
        <Page
            title={t("PRE640_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <Row>
                <Col>
                    <Form
                        id="PRE640_form_1"
                        initialValues={initialValues}
                        validationSchema={validationSchema}
                        onBeforeValidateField={onBeforeValidateField}
                        onBeforeButtonAction={onBeforeButtonAction}
                        onAfterButtonAction={onAfterButtonAction}
                        onBeforeInitialize={formOnBeforeInitialize}
                        onAfterInitialize={formOnAfterInitialize}
                        onBeforeFetchStats={addParameters}
                    >
                        <Row>
                            {/*<Col lg="3" >*/}
                            {/*    <div className="form-group" >*/}
                            {/*        <label htmlFor="USERID">{t("PRE640_frm1_lbl_USERID")}</label>*/}
                            {/*        <FieldSelectAsync name="USERID" isClearable />*/}
                            {/*        <StatusMessage for="USERID" />*/}
                            {/*    </div>*/}
                            {/*</Col>*/}
                            <Col lg="3">
                                <div className="form-group">
                                    <label htmlFor="CD_EMPRESA">{t("PRE640_frm1_lbl_CD_EMPRESA")}</label>
                                    <FieldSelectAsync name="CD_EMPRESA" isClearable />
                                    <StatusMessage for="CD_EMPRESA" />
                                </div>
                            </Col>
                            <Col lg="3">
                                <div className="form-group">
                                    <label htmlFor="NU_PREDIO">{t("PRE640_frm1_lbl_NU_PREDIO")}</label>
                                    <FieldSelect name="NU_PREDIO" isClearable />
                                    <StatusMessage for="NU_PREDIO" />
                                </div>
                            </Col>
                            <Col lg="3">
                                <div className="form-group">
                                    <label htmlFor="FL_FINALIZADOS">{t("PRE640_frm1_lbl_FL_FINALIZADOS")}</label>
                                    <FieldSelect name="FL_FINALIZADOS" />
                                    <StatusMessage for="FL_FINALIZADOS" />
                                </div>
                            </Col>
                        </Row>
                        <hr />
                        <Row style={{ justifyContent: 'center' }}>
                            <Col lg="2" className="mx-2" style={{ backgroundColor: "#EC7063" }}>
                                <div className="form-group" >
                                    <label htmlFor="QT_NUEVOS">{t("PRE640_frm1_lbl_QT_NUEVOS")}</label>
                                    <Field name="QT_NUEVOS" readOnly />

                                </div>
                            </Col>
                            {/*<Col lg="2" className="mx-2" style={{ backgroundColor: "#F78181" }}>*/}
                            {/*    <div className="form-group" >*/}
                            {/*        <label htmlFor="QT_CON_RESPON">{t("PRE640_frm1_lbl_QT_CON_RESPON")}</label>*/}
                            {/*        <Field name="QT_CON_RESPON" readOnly />*/}

                            {/*    </div>*/}
                            {/*</Col>*/}
                            <Col lg="2" className="mx-2" style={{ backgroundColor: "#fcb362" }}>
                                <div className="form-group" >
                                    <label htmlFor="QT_LIBERADOS">{t("PRE640_frm1_lbl_QT_LIBERADOS")}</label>
                                    <Field name="QT_LIBERADOS" readOnly />

                                </div>
                            </Col>
                            <Col lg="2" className="mx-2" style={{ backgroundColor: "#fbff90" }}>
                                <div className="form-group" >
                                    <label htmlFor="QT_PREPARACION">{t("PRE640_frm1_lbl_QT_PREPARACION")}</label>
                                    <Field name="QT_PREPARACION" readOnly />

                                </div>
                            </Col>
                            <Col lg="2" className="mx-2" style={{ backgroundColor: "#9FF781" }}>
                                <div className="form-group" >
                                    <label htmlFor="QT_PEDIDOS_CARGANDO">{t("PRE640_frm1_lbl_QT_PEDIDOS_CARGANDO")}</label>
                                    <Field name="QT_PEDIDOS_CARGANDO" readOnly />
                                </div>
                            </Col>

                            <FormButton id="btnFiltrar" label="Filtrar" className="hidden" />

                        </Row>
                    </Form>
                </Col>
            </Row>
            <hr />

            <Row className="mb-4">
                <Col>
                    <Grid
                        id="PRE640_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onBeforeExportExcel={addParameters}
                        enableExcelExport
                        onBeforeInitialize={addParameters}
                        //enableSelection
                        //onAfterInitialize={onAfterInitialize}
                        onBeforeFetch={addParameters}
                        onBeforeApplyFilter={addParameters}
                        onBeforeApplySort={addParameters}
                    //onBeforeMenuItemAction={addParameters}
                    //onAfterMenuItemAction={onAfterMenuItemAction}
                    />
                </Col>
            </Row>


        </Page>
    );
}