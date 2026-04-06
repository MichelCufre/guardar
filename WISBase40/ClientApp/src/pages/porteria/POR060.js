import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, FieldSelect, FormButton, SubmitButton, StatusMessage, FieldSelectLegacy } from '../../components/FormComponents/Form';
import { Row, Col, FormGroup, Button } from 'react-bootstrap';
import { useToaster } from '../../components/ToasterHook';

export default function POR060(props) {
    const { t } = useTranslation();
    const toaster = useToaster();

    const [isHiddenGrid, setIsHiddenGrid] = useState(true);

    const [nuPorteriaVehiculo, setNuPorteriaVehiculo] = useState(null);
    const [filtros, setFiltros] = useState(null);

    const formOnBeforeButtonAction = (context, form, query, nexus) => {

        context.abortServerCall = true;

        if (query.buttonId === "btnConfirmar") {
            nexus.getGrid("POR060_grid_Agregar").triggerMenuAction("btnAgregar");
        }
        else if (query.buttonId === "btnVolver") {
            nexus.getGrid("POR060_grid_Agregar").clearRows();
            nexus.getGrid("POR060_grid_Quitar").clearRows();
            setIsHiddenGrid(true);
        }

    }

    const formOnBeforeSubmit = (context, form, query, nexus) => {

        context.abortServerCall = true;

    }

    const onBeforeValidateField = (context, form, query, nexus) => {
        context.abortServerCall = true;

        if (query.fieldId === "FL_FILTER") {

            let parameters = [
                {
                    id: "FILTROS",
                    value: filtros || ""
                },
                {
                    id: "FL_FILTER",
                    value: form.fields.find(w => w.id === "FL_FILTER").value || ""
                },
            ];

            nexus.getGrid("POR060_grid_Agregar").refresh(parameters);

        }

    }


    const addParameters = (context, data, nexus) => {

        if (data.parameters.length === 0) {

            data.parameters = [
                {
                    id: "FILTROS",
                    value: filtros || ""
                },
                {
                    id: "FL_FILTER",
                    value: nexus.getForm("POR060_form_1").getFieldValue("FL_FILTER") || ""
                },
            ];
        }

    };

    const onBeforeMenuItemAction = (context, data, nexus) => {

        if (!data.selection.allSelected && data.selection.keys.length === 0) {
            context.abortServerCall = true;
            if (data.gridId === "POR060_grid_Agregar") {
                nexus.getGrid("POR060_grid_Quitar").triggerMenuAction("btnQuitar");
            }
            else if (data.gridId === "POR060_grid_Quitar") {
                nexus.getGrid("POR060_grid_Agregar").clearRows();
                nexus.getGrid("POR060_grid_Quitar").clearRows();
                toaster.toastSuccess("General_Sec0_msg_CambiosGuardados");
                setIsHiddenGrid(true);

            }
        }
        else {
            addParameters(context, data, nexus);
        }

    };

    const onAfterMenuItemAction = (context, data, nexus) => {

        if (data.gridId === "POR060_grid_Agregar") {
            nexus.getGrid("POR060_grid_Quitar").triggerMenuAction("btnQuitar");
        }
        else if (data.gridId === "POR060_grid_Quitar") {
            nexus.getGrid("POR060_grid_Agregar").clearRows();
            nexus.getGrid("POR060_grid_Quitar").clearRows();

            toaster.toastSuccess("General_Sec0_msg_CambiosGuardados");

            setIsHiddenGrid(true);
        }

    };

    const gridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {

            const _NU_PORTERIA_VEHICULO = data.row.cells.find(w => w.column === "NU_PORTERIA_VEHICULO").value;
            const _CD_EMPRESA = data.row.cells.find(w => w.column === "CD_EMPRESA").value;
            const _CD_CLIENTE = data.row.cells.find(w => w.column === "CD_CLIENTE").value;

            const _filtros = JSON.stringify({
                NU_PORTERIA_VEHICULO: _NU_PORTERIA_VEHICULO,
                CD_EMPRESA: _CD_EMPRESA,
                CD_CLIENTE: _CD_CLIENTE,
            });

            const parameters = [
                {
                    id: "FILTROS",
                    value: _filtros || ""
                },
                {
                    id: "FL_FILTER",
                    value: nexus.getForm("POR060_form_1").getFieldValue("FL_FILTER") || ""
                },
            ];


            nexus.getGrid("POR060_grid_Agregar").refresh(parameters);

            nexus.getGrid("POR060_grid_Quitar").refresh(parameters);

            setNuPorteriaVehiculo(_NU_PORTERIA_VEHICULO);
            setFiltros(_filtros);

            context.abortServerCall = true;

            setIsHiddenGrid(false);
        }
    };


    const validationSchema = {
        FL_FILTER: Yup.string(),
    };

    return (

        <Page
            title={t("POR060_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <Form
                id="POR060_form_1"
                initialValues={{ FL_FILTER: "S" }}
                validationSchema={validationSchema}
                onBeforeValidateField={onBeforeValidateField}
                onBeforeButtonAction={formOnBeforeButtonAction}
                onBeforeSubmit={formOnBeforeSubmit}
            >
                <div className={isHiddenGrid ? 'hidden' : ''}>

                    <Row className="mb-4">
                        <Col lg="2">
                            <FormGroup>
                                <h3>{`${t("POR060_frm1_lbl_Vehiculo")} ${nuPorteriaVehiculo}`}</h3>
                            </FormGroup>
                        </Col>
                        <Col lg="4">
                            <FormGroup>
                                <FieldSelect name="FL_FILTER" />
                            </FormGroup>
                        </Col>
                    </Row>
                    <hr></hr>
                    <Row>
                        <Col>
                            <h3>{t("POR060_frm1_lbl_SubTituloAgregar")}</h3>
                            <Grid id="POR060_grid_Agregar" rowsToFetch={30} rowsToDisplay={10} enableSelection
                                onBeforeInitialize={addParameters}
                                onBeforeFetch={addParameters}
                                onBeforeFetchStats={addParameters}
                                onBeforeApplyFilter={addParameters}
                                onBeforeApplySort={addParameters}
                                onBeforeMenuItemAction={onBeforeMenuItemAction}
                                onAfterMenuItemAction={onAfterMenuItemAction}
                            />
                        </Col>
                        <Col>
                            <h3>{t("POR060_frm1_lbl_SubTituloQuitar")}</h3>
                            <Grid id="POR060_grid_Quitar" rowsToFetch={30} rowsToDisplay={10} enableSelection
                                onBeforeInitialize={addParameters}
                                onBeforeFetch={addParameters}
                                onBeforeFetchStats={addParameters}
                                onBeforeApplyFilter={addParameters}
                                onBeforeApplySort={addParameters}
                                onBeforeMenuItemAction={onBeforeMenuItemAction}
                                onAfterMenuItemAction={onAfterMenuItemAction}

                            />
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <FormButton id="btnVolver" variant="link" label="General_Sec0_btn_Volver" />
                            <FormButton id="btnConfirmar" variant="primary" label="General_Sec0_btn_Confirmar" />
                        </Col>
                    </Row>
                </div>

            </Form>

            <div className={!isHiddenGrid ? 'hidden' : 'row mb-4'}>
                <div className="col-12">
                    <Grid id="POR060_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onBeforeButtonAction={gridOnBeforeButtonAction}
                    />
                </div>
            </div>

        </Page>
    );
}
