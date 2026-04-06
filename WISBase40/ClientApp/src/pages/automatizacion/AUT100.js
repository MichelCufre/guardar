import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, FieldSelect } from '../../components/FormComponents/Form';
import { Row, Col, FormGroup, Tab, Tabs, Button } from 'react-bootstrap';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import { AUT100Modal } from './AUT100Modal';
import { EjecucionesAutomatismoGrid } from './EjecucionesAutomatismoGrid';


export default function AUT100(props) {

    const { t } = useTranslation();

    const [keyTab, setKeyTab] = useState(null);

    const [automatismoObj, setAutomatismo] = useState(null);

    const [showPopup, setShowPopup] = useState(false);

    const [allowUpdate, setAllowUpdate] = useState(() => automatismoObj != null);

    const [mostrarViews, setMostrarViews] = useState(false);

    const [isUpdate, setIsUpdate] = useState(false);

    const validationSchema = {
        automatismo: Yup.string(),
        tipoUbicacion: Yup.string()
    };

    const onBeforeFetch = (context, data, nexus) => {
        data.filters.push(
            { ColumnId: "CD_ZONA_UBICACION", value: nexus.getForm("AUT100_form_1").getFieldValue("automatismo") }
        );

        if (data.gridId == "AUT100_grid_1") {
            data.parameters.push({ id: "tipoUbicacion", value: nexus.getForm("AUT100_form_2").getFieldValue("tipoUbicacion") });
        }
    }

    const onBeforeSelectSearch = (context, grid, query, nexus) => {
        query.parameters.push({ id: "automatismo", value: automatismoObj });
    }

    const onBeforeValidateRow = (context, data, nexus) => {
        data.parameters.push({ id: "automatismo", value: automatismoObj });
    }

    const handleFormAfterValidateField = (context, form, query, nexus) => {

        if (query.fieldId == "automatismo") {
            setMostrarViews(true);

            nexus.getGrid("AUT100_grid_1").refresh();
            nexus.getGrid("PRE130_grid_1").refresh();
            nexus.getGrid("PRE350_grid_1").refresh();
            nexus.getGrid("REG050_grid_1").refresh();
           
            var automatismo = query.parameters.find(w => w.id === "AUT100_NU_AUTOMATISMO").value;

            setAutomatismo(automatismo);
            setKeyTab("ejecuciones");

            nexus.getGrid("AUT100Ejecuciones_grid_1").reset();

            setAllowUpdate(true);
        }

        if (query.fieldId == "tipoUbicacion") {

            query.parameters = [
                { id: "tipoUbicacion", value: nexus.getForm("AUT100_form_2").getFieldValue("tipoUbicacion") },
            ];

            nexus.getGrid("AUT100_grid_1").refresh();
        }
    }

    const handleGridEjecucionesBeforeInitialize = (context, data, nexus) => {
        if (!mostrarViews && automatismoObj == null && keyTab != "ejecuciones")
            context.abortServerCall = true;
        else
            data.parameters = [{ id: "AUT100_NU_AUTOMATISMO", value: automatismoObj }];

    };

    const addParameters = (context, data, nexus) => {
        data.parameters = [{ id: "AUT100_NU_AUTOMATISMO", value: automatismoObj }];
    };

    //******** Modal Functions *************
    const openInsertPopup = () => {
        setIsUpdate(false);
        setShowPopup(true);
    }

    const openUpdatePopup = () => {
        setIsUpdate(true);
        setShowPopup(true);
    }

    const closeFormDialog = () => {
        setShowPopup(false);
        setIsUpdate(false);
    }

    //*********************

    return (

        <Page
            title={t("AUT100_Sec0_pageTitle_Titulo")}
            application="AUT100"
            {...props}
        >


            <Form
                id="AUT100_form_1"
                application="AUT100"
                validationSchema={validationSchema}
                onAfterValidateField={handleFormAfterValidateField}
            >

                <Row>
                    <Col lg="4">
                        <FormGroup>
                            <div style={{ textAlign: "center", marginBottom: '6vh' }}>
                                <label htmlFor="automatismo">{t("AUT100_frm1_lbl_automatismo")}</label>
                                <FieldSelect name="automatismo" />
                            </div>
                        </FormGroup>
                    </Col>
                    <Col lg="4" style={{
                        marginBottom: '6vh' 
                    }}>
                        <Button title={t("AUT100_frm1_lbl_InsertAutomatismo")} id="BtnInsert" className={"mt-4 ml-2 mr-2 btn btn-outline-primary form-control-sm"} variant="outline-primary" onClick={() => {
                            openInsertPopup();
                        }}>
                            <i className="fa fa-plus" />
                        </Button>
                        
                        <Button title={t("AUT100_frm1_lbl_EditAutomatismo")} id="BtnConfiguration" className={"mt-4 ml-2 mr-2 btn btn-outline-primary form-control-sm"} variant="outline-primary" onClick={() => {
                            openUpdatePopup();
                        }} disabled={!allowUpdate}>
                            <i className="fa fa-edit" />
                        </Button>
                    </Col>
                </Row>
            </Form>

            <div style={{ display: mostrarViews ? 'block' : 'none' }}>
                <Tabs transition={false} id="noanim-tab-example" style={{ marginBottom: '1vh' }}
                    onSelect={(k) => setKeyTab(k)}
                >
                    <Tab eventKey="ejecuciones" title={t("AUT100_frm1_tab_ejecuciones")}>
                        <EjecucionesAutomatismoGrid
                            id="AUT100Ejecuciones_grid_1"
                            onBeforeInitialize={handleGridEjecucionesBeforeInitialize}
                            onBeforeFetch={addParameters}
                            onBeforeFetchStats={addParameters}
                            onBeforeExportExcel={addParameters}
                            onBeforeApplyFilter={addParameters}
                            onBeforeApplySort={addParameters}
                        />
                    </Tab>
                    <Tab eventKey="stock" title={t("AUT100_frm1_tab_stock")}>
                        <Form
                            id="AUT100_form_2"
                            application="AUT100"
                            validationSchema={validationSchema}
                            onAfterValidateField={handleFormAfterValidateField}
                        >
                            <div style={{ marginBottom: '6vh' }}>
                                <label htmlFor="tipoUbicacion">{t("AUT100_frm1_lbl_tipoUbicacion")}</label>
                                <FieldSelect name="tipoUbicacion" />
                            </div>

                        </Form>
                        <Grid id="AUT100_grid_1" application="AUT100" rowsToFetch={30} rowsToDisplay={15}

                            enableExcelExport
                            onBeforeFetch={onBeforeFetch}
                            onBeforeFetchStats={onBeforeFetch}
                            onBeforeExportExcel={onBeforeFetch}
                            onBeforeApplyFilter={onBeforeFetch}
                            onBeforeApplySort={onBeforeFetch}
                        />
                    </Tab>

                    <Tab eventKey="consutlaPreparaciones" title={t("AUT100_frm1_tab_consutlaPreparaciones")}>
                        <Grid
                            application="PRE130"
                            id="PRE130_grid_1" rowsToFetch={30} rowsToDisplay={15}
                            enableExcelExport
                            onBeforeFetch={onBeforeFetch}
                            onBeforeFetchStats={onBeforeFetch}
                            onBeforeExportExcel={onBeforeFetch}                            
                            onBeforeApplyFilter={onBeforeFetch}
                            onBeforeApplySort={onBeforeFetch}
                        />
                    </Tab>

                    <Tab eventKey="rbstecmntsPendientes" title={t("AUT100_frm1_tab_rbstecmntsPendientes")}>
                        <Grid
                            application="PRE350"
                            id="PRE350_grid_1" rowsToFetch={30} rowsToDisplay={15}
                            enableExcelExport
                            onBeforeFetch={onBeforeFetch}
                            onBeforeFetchStats={onBeforeFetch}
                            onBeforeExportExcel={onBeforeFetch}
                            onBeforeApplyFilter={onBeforeFetch}
                            onBeforeApplySort={onBeforeFetch}
                        />
                    </Tab>

                    <Tab eventKey="asignacionProductos" title={t("AUT100_frm1_tab_asignacionProductos")}>
                        <Grid
                            application="REG050"
                            id="REG050_grid_1" rowsToFetch={30} rowsToDisplay={15}
                            enableExcelExport
                            onBeforeFetch={onBeforeFetch}
                            onBeforeFetchStats={onBeforeFetch}
                            onBeforeExportExcel={onBeforeFetch}
                            onBeforeApplyFilter={onBeforeFetch}
                            onBeforeApplySort={onBeforeFetch}
                            onBeforeSelectSearch={onBeforeSelectSearch}
                            onBeforeCommit={onBeforeFetch}
                            onBeforeValidateRow={onBeforeValidateRow}
                        />
                    </Tab>
                </Tabs>
            </div>

            <AUT100Modal show={showPopup} onHide={closeFormDialog} automatismo={automatismoObj} isUpdate={isUpdate} />


        </Page>
    );
}