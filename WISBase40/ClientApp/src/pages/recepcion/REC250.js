import React, { useRef } from 'react';
import { Col, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Field, FieldSelect, FieldSelectAsync, Form, FormButton, StatusMessage } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';

export default function REC250(props) {
    const { t } = useTranslation();


    const refFiltro = useRef(null);
    refFiltro.current = {
        USERID: "", CD_EMPRESA: "", NU_PREDIO: "", FL_CERRADO: ""
    };

    const initialValues = {
    };

    const validationSchema = {
        USERID: Yup.string(),
        CD_EMPRESA: Yup.string(),
        NU_PREDIO: Yup.string(),
        FL_CERRADO: Yup.string(),
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
    const onAfterButtonAction = (context, form, query, nexus) => {

        context.forceUpdateFields = true;

    }

    const onBeforeValidateField = (context, form, query, nexus) => {
        context.abortServerCall = true;

        if (query.fieldId === "USERID") {
            refFiltro.current.USERID = form.fields.find(w => w.id === "USERID").value;
        }
        else if (query.fieldId === "CD_EMPRESA") {
            refFiltro.current.CD_EMPRESA = form.fields.find(w => w.id === "CD_EMPRESA").value;
        } else if (query.fieldId === "NU_PREDIO") {
            refFiltro.current.NU_PREDIO = form.fields.find(w => w.id === "NU_PREDIO").value;
        }
        else if (query.fieldId === "FL_CERRADO") {
            refFiltro.current.FL_CERRADO = form.fields.find(w => w.id === "FL_CERRADO").value;
        }

        setTimeout(() => {

            nexus.getGrid("REC250_grid_1").reset([{ id: "FILTROS", value: JSON.stringify(refFiltro.current) }]);
            nexus.getForm("REC250_form_1").clickButton("btnFiltrar");

        }, 100);

    };

    const onAfterMenuItemAction = (context, data, nexus) => {

        nexus.getForm("REC250_form_1").clickButton("btnFiltrar");
        nexus.getGrid("REC250_grid_1").refresh();

    };
    const onBeforeButtonAction = (context, form, query, nexus) => {

        query.parameters = [
            {
                id: "FILTROS",
                value: JSON.stringify(refFiltro.current)
            }
        ];
    }
    const onAfterInitialize = (context, grid, parameters, nexus) => {

        setTimeout(() => {
            let grid = nexus.getGrid("REC250_grid_1");

            if (grid) {

                grid.reset([{ id: "FILTROS", value: JSON.stringify(refFiltro.current) }]);

                const selection = JSON.parse(parameters.find(d => d.id === "preSeleccion").value);

                grid.setSelection(false, selection);
            }

        }, 60000);
    };

    const onBeforeFetchStats = (context, parameters, nexus) => {

    }

    return (
        <Page
            title={t("REC250_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <Row>
                <Col>
                    <Form
                        id="REC250_form_1"
                        initialValues={initialValues}
                        validationSchema={validationSchema}
                        onBeforeValidateField={onBeforeValidateField}
                        onBeforeButtonAction={onBeforeButtonAction}
                        onAfterButtonAction={onAfterButtonAction}
                    >
                        <Row>
                            <Col lg="3" >
                                <div className="form-group" >
                                    <label htmlFor="USERID">{t("REC250_frm1_lbl_USERID")}</label>
                                    <FieldSelectAsync name="USERID" isClearable />
                                    <StatusMessage for="USERID" />
                                </div>
                            </Col>
                            <Col lg="3">
                                <div className="form-group">
                                    <label htmlFor="CD_EMPRESA">{t("REC250_frm1_lbl_CD_EMPRESA")}</label>
                                    <FieldSelectAsync name="CD_EMPRESA" isClearable />
                                    <StatusMessage for="CD_EMPRESA" />
                                </div>
                            </Col>
                            <Col lg="3">
                                <div className="form-group">
                                    <label htmlFor="NU_PREDIO">{t("REC250_frm1_lbl_NU_PREDIO")}</label>
                                    <FieldSelect name="NU_PREDIO" isClearable />
                                    <StatusMessage for="NU_PREDIO" />
                                </div>
                            </Col>
                            <Col lg="3">
                                <div className="form-group">
                                    <label htmlFor="FL_CERRADO">{t("REC250_frm1_lbl_FL_CERRADO")}</label>
                                    <FieldSelect name="FL_CERRADO" isClearable />
                                    <StatusMessage for="FL_CERRADO" />
                                </div>
                            </Col>
                        </Row>
                        <hr />
                        <Row style={{ justifyContent: 'center' }}>
                            <Col lg="2" className="mx-2" style={{ backgroundColor: "#EC7063" }}>
                                <div className="form-group" >
                                    <label htmlFor="QT_ABIERTAS">{t("REC250_frm1_lbl_QT_ABIERTAS")}</label>
                                    <Field name="QT_ABIERTAS" readOnly />

                                </div>
                            </Col>
                            <Col lg="2" className="mx-2" style={{ backgroundColor: "#F78181" }}>
                                <div className="form-group" >
                                    <label htmlFor="QT_CON_RESPON">{t("REC250_frm1_lbl_QT_CON_RESPON")}</label>
                                    <Field name="QT_CON_RESPON" readOnly />

                                </div>
                            </Col>
                            <Col lg="2" className="mx-2" style={{ backgroundColor: "#fcb362" }}>
                                <div className="form-group" >
                                    <label htmlFor="QT_LIBERADAS">{t("REC250_frm1_lbl_QT_LIBERADAS")}</label>
                                    <Field name="QT_LIBERADAS" readOnly />

                                </div>
                            </Col>
                            <Col lg="2" className="mx-2" style={{ backgroundColor: "#fbff90" }}>
                                <div className="form-group" >
                                    <label htmlFor="QT_RECEPCION">{t("REC250_frm1_lbl_QT_RECEPCION")}</label>
                                    <Field name="QT_RECEPCION" readOnly />

                                </div>
                            </Col>
                            <Col lg="2" className="mx-2" style={{ backgroundColor: "#9FF781" }}>
                                <div className="form-group" >
                                    <label htmlFor="QT_RECIBIDAS">{t("REC250_frm1_lbl_QT_RECIBIDAS")}</label>
                                    <Field name="QT_RECIBIDAS" readOnly />
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
                        id="REC250_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeInitialize={addParameters}

                        onAfterInitialize={onAfterInitialize}
                        onBeforeFetch={addParameters}
                        onBeforeApplyFilter={addParameters}
                        onBeforeApplySort={addParameters}
                        onBeforeMenuItemAction={addParameters}
                        onBeforeExportExcel={addParameters}
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        onBeforeFetchStats={addParameters}

                        enableSelection
                    />
                </Col>
            </Row>


        </Page>
    );
}