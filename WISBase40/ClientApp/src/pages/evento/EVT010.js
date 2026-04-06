import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldCheckbox, FieldSelect, FieldSelectAsync, FieldDate, SubmitButton, FormButton, StatusMessage } from '../../components/FormComponents/Form';
import { FormTab, FormTabStep } from '../../components/FormComponents/FormTab';
import { Row, Col, FormGroup } from 'react-bootstrap';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';

export default function EVT010(props) {
    const { t } = useTranslation();

    return (

        <Page
            title={t("EVT010_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="EVT010_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport />
                </div>
            </div>
        </Page>
    );
}


//<Form
//    id="EVT010_form_1"

//>
//    <div>
//        <div className="col">
//            <div className="row col-12">
//                <fieldset className="row ml-5">
//                    <Row>
//                        <Col>
//                            <label htmlFor="NU_EVENTO">{t("EVT010_frm1_lbl_NU_EVENTO")}</label>
//                            <Field name="NU_EVENTO" />
//                            <StatusMessage for="NU_EVENTO" readOnly />
//                        </Col>


//                        <Col>

//                            <label htmlFor="DS_EVENTO">{t("EVT010_frm1_lbl_DS_EVENTO")}</label>
//                            <Field name="DS_EVENTO" />
//                            <StatusMessage for="DS_EVENTO" />
//                        </Col>

//                        <Col className="mt-4">
//                            <FormGroup>
//                                <FieldCheckbox
//                                    name="FL_PROGRAMADO"
//                                    label={t("EVT010_frm1_lbl_FL_PROGRAMADO")}
//                                    className="mb-2"
//                                />
//                                <StatusMessage for="FL_PROGRAMADO" />
//                            </FormGroup>
//                        </Col>

//                        <Col className="mt-4">
//                            <SubmitButton id="BtnSubmit" label="General_Sec0_btn_Confirmar" variant="primary" />&nbsp;
//                                        </Col>

//                    </Row>
//                    <hr />

//                </fieldset>

//            </div>
//        </div>
//    </div>

//</Form>
