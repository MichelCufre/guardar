import React, { useState, useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldCheckbox, FieldSelect, FieldSelectAsync, FieldDate, SubmitButton, FormButton, StatusMessage, FieldDateTime } from '../../components/FormComponents/Form';
import { Row, Col, FormGroup } from 'react-bootstrap';
import { FormTab, FormTabStep } from '../../components/FormComponents/FormTab';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';

export default function PRE450(props) {
    const { t } = useTranslation();
    const validationSchema = {
        dtFin: Yup.string().nullable()
    }
    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("PRE450_grid_1").refresh();
        nexus.getGrid("PRE450_grid_2").refresh();
    }
    const onBeforeMenuItemAction = (context, data, nexus) => {
        data.parameters = [
            { id: "dtFin", value: nexus.getForm("AUT010_form_1").getFieldValue("dtFin") }
        ];
    }

    const onBeforeValidateField = (context, form, query, nexus) => {
        context.abortServerCall = true;
    }
    const onBeforeSubmit = (context, form, query, nexus) => {
        context.abortServerCall = true;
    }
    return (

        <Page
            title={t("PRE450_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                id="AUT010_form_1"
                validationSchema={validationSchema}
                onBeforeValidateField={onBeforeValidateField}
                onBeforeSubmit={onBeforeSubmit}
            >
                <Row>
                    <FormGroup style={{ marginLeft: 20 }}>
                        <label htmlFor="dtFin">{t("PRE450_frm1_lbl_DT_FIN")}</label>
                        <FieldDate name="dtFin" />
                        <StatusMessage for="dtFin" />
                    </FormGroup>
                </Row>
                <Row>
                    <Col lg={6}>

                        <Grid id="PRE450_grid_1" rowsToFetch={30} rowsToDisplay={15} enableSelection
                            onAfterMenuItemAction={onAfterMenuItemAction}
                            onBeforeMenuItemAction={onBeforeMenuItemAction} />
                    </Col>
                    <Col lg={6}>
                        <Grid id="PRE450_grid_2" rowsToFetch={30} rowsToDisplay={15} enableSelection
                            onAfterMenuItemAction={onAfterMenuItemAction} />
                    </Col>
                </Row>
            </Form>










        </Page>
    );
}