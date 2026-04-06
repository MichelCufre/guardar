import React, { useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldCheckbox, FieldSelect, FieldSelectAsync, FieldDate, SubmitButton, FormButton, StatusMessage } from '../../components/FormComponents/Form';
import { Row, Col, Button } from 'react-bootstrap';
import { FormTab, FormTabStep } from '../../components/FormComponents/FormTab';
import * as Yup from 'yup';



export default function PRE670(props) {
    const { t } = useTranslation();

    const refFiltro = useRef(null);
    refFiltro.current = {
        FL_AJUSTAR_DETALLE: "",
    };
    const validationSchema = {
        FL_AJUSTAR_DETALLE: Yup.string(),
    };
    const onBeforeCommit = (context, data, nexus) => {
        data.parameters = [
            { id: "FL_AJUSTAR_DETALLE", value: nexus.getForm("PRE670_form_1").getFieldValue("FL_AJUSTAR_DETALLE") }
        ];
    }

    const onAfterMenuItemAction = (context, data, nexus) => {
        context.forceRefresh = true;
    }

    const onBeforeValidateField = (context, form, query, nexus) => {
        context.abortServerCall = true;
        context.forceUpdateFields = true;
        if (query.fieldId === "FL_AJUSTAR_DETALLE") {
            refFiltro.current.FL_AJUSTAR_DETALLE = form.fields.find(w => w.id === "FL_AJUSTAR_DETALLE").value;
        }
    };

    const RedirectLiberacionOnda = () => {
        window.location = "/preparacion/PRE052";
    };

    const formOnBeforeInitialize = (context, form, query, nexus) => {

        query.parameters = [
            {
                id: "FILTROS",
                value: JSON.stringify(refFiltro.current)
            }
        ];
    }

    const initialValues = {
    };

    return (

        <Page
            title={t("Master_Menu_lbl_WPREPARACION_WPRE670")}
            {...props}
        >
            <Form
                id="PRE670_form_1"
                initialValues={initialValues}
                onBeforeInitialize={formOnBeforeInitialize}
                onBeforeValidateField={onBeforeValidateField}
                validationSchema={validationSchema}

            >
                <Row>
                    <Col lg="4">
                        <div className="form-group">
                            <label htmlFor="FL_AJUSTAR_DETALLE">{t("PRE670_frm1_lbl_FL_AJUSTAR_DETALLE")}</label>
                            <FieldSelect name="FL_AJUSTAR_DETALLE" />
                            <StatusMessage for="FL_AJUSTAR_DETALLE" />
                        </div>
                    </Col>
                    <Col lg="4">
                        <label>{' '}</label>
                        <div className="form-group">
                            <Button variant="outline-primary" className="mr-3 mt-2" onClick={RedirectLiberacionOnda}>{t("Master_Menu_lbl_WPREPARACION_WPRE050")}</Button>
                        </div>
                    </Col>
                </Row>
            </Form>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="PRE670_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        onBeforeCommit={onBeforeCommit}
                        enableSelection />
                </div>
            </div>            
        </Page>
    );
}