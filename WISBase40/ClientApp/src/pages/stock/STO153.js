import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Row, Col, FormGroup, Container } from 'react-bootstrap';
import { Form, Field, FieldSelect, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import * as Yup from 'yup';

export default function STO153(props) {

    const { t } = useTranslation();

    const validationSchema = {
        CD_CONTROL: Yup.number().transform(value => (isNaN(value) ? undefined : value)).required()
    };

    const initialValues = {
        CD_CONTROL: ""
    };

    const addParameters = (context, data, nexus) => {
        data.parameters = [
            {
                id: "CD_CONTROL",
                value: nexus.getForm("STO153_form_1").getFieldValue("CD_CONTROL") || ""
            }
        ];
    };

    const handleGridAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("STO153_grid_1").refresh();
    }

    const onAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "ERROR") return;

        query.parameters = [
            {
                id: "CD_CONTROL",
                value: nexus.getForm("STO153_form_1").getFieldValue("CD_CONTROL") || ""
            }
        ];

        nexus.getGrid("STO153_grid_1").clickMenuItem("BtnConfirmar");

        context.abortServerCall = true;
    };

    return (
        <Page
            icon="fas fa-cubes"
            title={t("STO153_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <Form
                id="STO153_form_1"
                application="STO153"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onAfterSubmit={onAfterSubmit}
            >
                <Container fluid>
                    <Row>
                        <Col>
                            <Row>
                                <Col>
                                    <FormGroup>
                                        <label htmlFor="CD_CONTROL">{t("STO153_frm1_lbl_CD_CONTROL")}</label>
                                        <FieldSelect name="CD_CONTROL" />
                                        <StatusMessage for="CD_CONTROL" />
                                    </FormGroup>
                                </Col>
                                <Col>
                                    <Col>
                                        <FormGroup>
                                            <br />
                                            <SubmitButton id="BtnConfirmar" variant="primary" className="ml-2 mr-5" label="STO153_Sec0_btn_ConfirmarControl" />
                                        </FormGroup>
                                    </Col>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Container>
            </Form>

            <br />

            <div className="row mb-4">
                <div className="col">
                    <Grid
                        application="STO153"
                        id="STO153_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableSelection
                        enableExcelExport
                        onBeforeFetch={addParameters}
                        onBeforeExportExcel={addParameters}
                        onBeforeFetchStats={addParameters}
                        onBeforeMenuItemAction={addParameters}
                        onAfterMenuItemAction={handleGridAfterMenuItemAction}
                    />
                </div>
            </div>
        </Page>
    );
}