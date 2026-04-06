import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Row, Col, FormGroup } from 'react-bootstrap';
import { Form, FieldToggle } from '../../components/FormComponents/Form';
import * as Yup from 'yup';

export default function PRE120(props) {
    const { t } = useTranslation();

    let validationSchema = {
        tipoAnulacion: Yup.boolean(),
    };

    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("PRE120_grid_1").refresh();
    }

    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("PRE120_grid_1").refresh();
    }

    const onBeforeMenuItemAction = (context, data, nexus) => {
        if (nexus.getForm("PRE120_form_1").getFieldValue("tipoAnulacion") != null) {

            var tipo

            if (nexus.getForm("PRE120_form_1").getFieldValue("tipoAnulacion"))
                tipo = 2; //PreparacionPedido
            else
                tipo = 1; //Preparacion

            data.parameters = [
                { id: "tipoAnulacion", value: tipo }
            ];
        }
    };
    const onBeforeCommit = (context, data, nexus) => {
        if (nexus.getForm("PRE120_form_1").getFieldValue("tipoAnulacion") != null) {

            var tipo

            if (nexus.getForm("PRE120_form_1").getFieldValue("tipoAnulacion"))
                tipo = 2;//PreparacionPedido
            else
                tipo = 1;//Preparacion

            data.parameters = [
                { id: "tipoAnulacion", value: tipo }
            ];
        }
    };

    const onBeforeValidateField = (context, form, query, nexus) => {
        context.abortServerCall = true;
    }



    return (

        <Page
            title={t("PRE120_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                id="PRE120_form_1"
                onBeforeValidateField={onBeforeValidateField}
                validationSchema={validationSchema}
            >

                <div className="form-check form-check-inline" style={{ marginRight: "-1%" }}>
                    <label class="form-check-inline" for="tipoAnulacion">{t("Preparación")}</label>
                </div>
                <div className="form-check form-check-inline" style={{ marginRight: "-1%" }}>
                    <FieldToggle className="form-check-inline" name="tipoAnulacion" />
                </div>
                <div className="form-check form-check-inline"   >
                    <label class="form-check-inline" for="tipoAnulacion">{t("Preparación y Pedido")}</label>
                </div>
            </Form>

            <hr />

            <Grid
                id="PRE120_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableSelection
                enableExcelExport
                onAfterCommit={onAfterCommit}
                onBeforeCommit={onBeforeCommit}
                onAfterMenuItemAction={onAfterMenuItemAction}
                onBeforeMenuItemAction={onBeforeMenuItemAction}
            />
        </Page>
    );
}