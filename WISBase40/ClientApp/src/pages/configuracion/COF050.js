import React, { useState, useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { Form, Field, FormButton, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';


export default function COF050(props) {

    const { t } = useTranslation();

    const lenguaje = useRef("");

    const validationSchema = {
        lenguajeModificarFiltro: new Yup.string().nullable(),
    };

    const handleFilters = (context, form, query, nexus) => {
        const modifiedRows = nexus.getGrid("COF050_grid_1").getModifiedRows();
        if (modifiedRows.length > 0) {
            context.abortServerCall = true;
            nexus.showConfirmation({
                message: "General_Sec0_Info_DeseaAnularCambios",
                acceptLabel: "General_Sec0_btn_DeshacerCambios",
                onAccept: () => handleFilters2(context, form, query, nexus)
            });
        }
        else
        {
            handleFilters2(context, form, query, nexus)
        }
    };

    const handleFilters2 = (context, form, query, nexus) => {
        context.abortServerCall = true;
        if (query.buttonId === "btnLeguajeFiltro") {
            lenguaje.current = form.fields.find(f => f.id === "lenguajeModificarFiltro").value;
        }

        nexus.getGrid("COF050_grid_1").refresh();
    };

    const CargarParametrosGrilla = (context, data, nexus) => {
        data.parameters = [
            { id: "lenguajeModificarFiltro", value: lenguaje.current }
        ];
    }

    const handleOnBeforeCommit = (context, data, nexus) => {
        data.parameters = [
            { id: "lenguajeModificarFiltro", value: lenguaje.current }
        ];
    }

    return (
        <Page
            title={t("COF050_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                application="COF050"
                id="COF050_form_1"
                application="COF050"
                validationSchema={validationSchema}
                onBeforeButtonAction={handleFilters}
            >
                <Row>
                    <Col>
                        <div className="form-group" >
                            <label htmlFor="lenguajeModificarFiltro">{t("COF050_frm1_lbl_lenguajeModificarFiltro")}</label>
                            <FieldSelect name="lenguajeModificarFiltro" />
                            <StatusMessage for="lenguajeModificarFiltro" />
                        </div>
                    </Col>
                    <Col>
                    </Col>
                </Row>
                <hr /> 
                <Row>
                    <Col>
                        <div className="form-group" >
                            <FormButton id="btnLeguajeFiltro" variant="primary" label="COF050_frm1_btn_leguajeFiltro" />
                        </div>
                    </Col>
                </Row>
            </Form>


                <div className="row mb-4">
                    <div className="col-12">
                    <Grid id="COF050_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport enableExcelImport={false}
                        onBeforeFetch={CargarParametrosGrilla}
                        onBeforeApplyFilter={CargarParametrosGrilla}
                        onBeforeApplySort={CargarParametrosGrilla}
                        onBeforeExportExcel={CargarParametrosGrilla}
                        onBeforeCommit={handleOnBeforeCommit}
                        onBeforeMenuItemAction={handleOnBeforeCommit}
                        />
                    </div>
                </div>

        </Page>
    );
}