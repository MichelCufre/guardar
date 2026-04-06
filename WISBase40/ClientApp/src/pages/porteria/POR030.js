import React, { useState,useRef } from 'react';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Page } from '../../components/Page';
import { Form, Field, FieldDate, FieldDateTime, FieldCheckbox, FieldSelect, FieldSelectAsync, FormButton, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import { Row, Col, FormGroup, Button } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';

export default function POR030(props) {
    const { t } = useTranslation();

    const refFiltro = useRef(null);

    refFiltro.current = {
        TP_REGISTRO: "",
        NU_VEHICULO: "",
        CD_EMPRESA: "",
        CD_SECTOR: "",
    };

    const validationSchema = {
        TP_REGISTRO: Yup.string(),
        NU_VEHICULO: Yup.string(),
        CD_EMPRESA: Yup.string(),
        CD_SECTOR: Yup.string(),
    };

    const onBeforeValidateField = (context, form, query, nexus) => {
        context.abortServerCall = true;

        if (query.fieldId === "TP_REGISTRO") {
            refFiltro.current.TP_REGISTRO = form.fields.find(w => w.id === "TP_REGISTRO").value;
        }
        else if (query.fieldId === "NU_VEHICULO") {
            refFiltro.current.NU_VEHICULO = form.fields.find(w => w.id === "NU_VEHICULO").value;
        }
        else if (query.fieldId === "CD_EMPRESA") {
            refFiltro.current.CD_EMPRESA = form.fields.find(w => w.id === "CD_EMPRESA").value;
        }
        else if (query.fieldId === "CD_SECTOR") {
            refFiltro.current.CD_SECTOR = form.fields.find(w => w.id === "CD_SECTOR").value;
        }

        nexus.getGrid("POR030_grid_1").reset([{ id: "FILTROS", value: JSON.stringify(refFiltro.current) }]);

    }


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

    return (

        <Page
            title={t("POR030_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <Form
                id="POR030_form_1"
                validationSchema={validationSchema}
                onBeforeValidateField={onBeforeValidateField}
            >

                <Row>
                    <Col>
                        <FormGroup>
                            <label htmlFor="NU_VEHICULO">{t("POR030_frm1_lbl_NU_VEHICULO")}</label>
                            <FieldSelectAsync name="NU_VEHICULO" isClearable/>
                            <StatusMessage for="NU_VEHICULO" />
                        </FormGroup>
                    </Col>
                    <Col>
                        <FormGroup>
                            <label htmlFor="CD_EMPRESA">{t("POR010_frm1_lbl_CD_EMPRESA")}</label>
                            <FieldSelectAsync name="CD_EMPRESA" isClearable/>
                            <StatusMessage for="CD_EMPRESA" />
                        </FormGroup>
                    </Col>
                    <Col>
                        <FormGroup>
                            <label htmlFor="CD_SECTOR">{t("POR010_frm1_lbl_CD_SECTOR")}</label>
                            <FieldSelect name="CD_SECTOR" isClearable/>
                            <StatusMessage for="CD_SECTOR" />
                        </FormGroup>
                    </Col>
                </Row>


            </Form>

            <hr />

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="POR030_grid_1" rowsToFetch={30} rowsToDisplay={15} 
                        onBeforeInitialize={addParameters}
                        onBeforeFetch={addParameters}
                        onBeforeFetchStats={addParameters}
                        onBeforeApplyFilter={addParameters}
                        onBeforeApplySort={addParameters}
                    />
                </div>
            </div>

        </Page>
    );
}
