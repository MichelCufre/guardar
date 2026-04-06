import React, { useState, useEffect } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldCheckbox, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container, Div } from 'react-bootstrap';
import * as Yup from 'yup';
import { add } from 'date-fns/fp';
import { AddRemovePanel } from '../../components/AddRemovePanel';

function InternalSEG030AsignarInfoModal(props) {
    const { t } = useTranslation();
    const [keyTab, setKeyTab] = useState("predios");
    const [infoUsuario, setInfoUsuario] = useState(null);

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        if (parameters.find(x => x.id === "SEG030_USUARIO") != null) {
            setInfoUsuario(parameters.find(x => x.id === "SEG030_USUARIO").value);
        }
    }

    const addParameters = (context, data, nexus) => {

        if (props.boton && props.boton.find(x => x.id === "btnConfiEmpre") && keyTab != "empresas") {
            setKeyTab("empresas")
        } else if (props.boton && props.boton.find(x => x.id === "btnConfiPredios") && keyTab != "predios") {
            setKeyTab("predios")
        }

        data.parameters = [
            { id: "idUsuario", value: props.usuario.find(x => x.id === "idUsuario").value }

        ];
    }

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("SEG030Asignar_grid_1").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    };
    const handleRemove = (evt, nexus) => {
        nexus.getGrid("SEG030Asignar_grid_2").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    };
    const handleAdd2 = (evt, nexus) => {
        nexus.getGrid("SEG030Asignar_grid_3").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    };
    const handleRemove2 = (evt, nexus) => {
        nexus.getGrid("SEG030Asignar_grid_4").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    };
    const handleAdd3 = (evt, nexus) => {
        nexus.getGrid("SEG030Asignar_grid_5").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    };
    const handleRemove3 = (evt, nexus) => {
        nexus.getGrid("SEG030Asignar_grid_6").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    };

    const handleClose = () => {
        props.onHide(null, null, props.nexus);
    };

    const onAfterMenuItemAction = (context, data, nexus) => {

        nexus.getGrid("SEG030Asignar_grid_1").refresh();
        nexus.getGrid("SEG030Asignar_grid_2").refresh();
        nexus.getGrid("SEG030Asignar_grid_3").refresh();
        nexus.getGrid("SEG030Asignar_grid_4").refresh();
        nexus.getGrid("SEG030Asignar_grid_5").refresh();
        nexus.getGrid("SEG030Asignar_grid_6").refresh();

        /*if (data.gridId == "SEG030Asignar_grid_1" || data.data.gridId == "SEG030Asignar_grid_2") {
            nexus.getGrid("SEG030Asignar_grid_1").refresh();
            nexus.getGrid("SEG030Asignar_grid_2").refresh();
        }
        else if (data.gridId == "SEG030Asignar_grid_3" || data.data.gridId == "SEG030Asignar_grid_4") {
            nexus.getGrid("SEG030Asignar_grid_3").refresh();
            nexus.getGrid("SEG030Asignar_grid_4").refresh();
        }
        else if (data.gridId == "SEG030Asignar_grid_5" || data.data.gridId == "SEG030Asignar_grid_6") {
            nexus.getGrid("SEG030Asignar_grid_5").refresh();
            nexus.getGrid("SEG030Asignar_grid_6").refresh();
        }*/
    }

    return (
        <Container fluid>
            <Modal.Header closeButton>
                <Modal.Title>{t("SEG030_Sec0_mdlAsigInf_Titulo")} - {`${infoUsuario}`}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    {...props}
                    application="SEG030Asignar"

                >
                    <Tabs transition={false} id="noanim-tab-example"
                        //activeKey={keyTab}
                        onSelect={(k) => setKeyTab(k)}
                    >
                        <Tab eventKey="predios" title={t("SEG030_frm1_tab_predios")}>
                            <Row className='mt-3'>
                                <div className="col-12">
                                    <h3>{t("SEG030_Sec0_mdlAsigna2_PREDIOS")}</h3>
                                    <AddRemovePanel
                                        onAdd={handleAdd}
                                        onRemove={handleRemove}
                                        from={(
                                            <Grid
                                                application="SEG030Asignar"
                                                id="SEG030Asignar_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                                enableSelection
                                                onAfterInitialize={onAfterInitialize}
                                                onBeforeInitialize={addParameters}
                                                onBeforeFetch={addParameters}
                                                onBeforeApplyFilter={addParameters}
                                                onBeforeApplySort={addParameters}
                                                onBeforeExportExcel={addParameters}
                                                onAfterMenuItemAction={onAfterMenuItemAction}
                                                onBeforeMenuItemAction={addParameters}
                                                onBeforeFetchStats={addParameters}
                                            />
                                        )}
                                        to={(
                                            <Grid
                                                application="SEG030Asignar"
                                                id="SEG030Asignar_grid_2" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                                enableSelection
                                                onAfterInitialize={onAfterInitialize}
                                                onBeforeInitialize={addParameters}
                                                onBeforeFetch={addParameters}
                                                onBeforeApplyFilter={addParameters}
                                                onBeforeApplySort={addParameters}
                                                onBeforeExportExcel={addParameters}
                                                onAfterMenuItemAction={onAfterMenuItemAction}
                                                onBeforeMenuItemAction={addParameters}
                                                onBeforeFetchStats={addParameters}
                                            />
                                        )}
                                    />
                                </div>
                            </Row>
                        </Tab>
                        <Tab eventKey="empresas" title={t("SEG030_frm1_tab_empresas")} >
                            <Row className='mt-3'>
                                <div className="col-12">
                                    <h3>{t("SEG030_Sec0_mdlAsigna2_empresas")}</h3>
                                    <AddRemovePanel
                                        onAdd={handleAdd2}
                                        onRemove={handleRemove2}
                                        from={(
                                            <Grid
                                                application="SEG030Asignar"
                                                id="SEG030Asignar_grid_3" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                                enableSelection
                                                onAfterInitialize={onAfterInitialize}
                                                onBeforeInitialize={addParameters}
                                                onBeforeFetch={addParameters}
                                                onBeforeApplyFilter={addParameters}
                                                onBeforeApplySort={addParameters}
                                                onBeforeExportExcel={addParameters}
                                                onAfterMenuItemAction={onAfterMenuItemAction}
                                                onBeforeMenuItemAction={addParameters}
                                                onBeforeFetchStats={addParameters}
                                            />
                                        )}
                                        to={(
                                            <Grid
                                                application="SEG030Asignar"
                                                id="SEG030Asignar_grid_4" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                                enableSelection
                                                onAfterInitialize={onAfterInitialize}
                                                onBeforeInitialize={addParameters}
                                                onBeforeFetch={addParameters}
                                                onBeforeApplyFilter={addParameters}
                                                onBeforeApplySort={addParameters}
                                                onBeforeExportExcel={addParameters}
                                                onAfterMenuItemAction={onAfterMenuItemAction}
                                                onBeforeMenuItemAction={addParameters}
                                                onBeforeFetchStats={addParameters}
                                            />
                                        )}
                                    />
                                </div>
                            </Row>
                        </Tab>
                        <Tab eventKey="grupoConsulta" title={t("SEG030_frm1_tab_gruposConsulta")} >
                            <Row className='mt-3'>
                                <div className="col-12">
                                    <h3>{t("SEG030_Sec0_mdlAsigna2_gruposConsulta")}</h3>
                                    <AddRemovePanel
                                        onAdd={handleAdd3}
                                        onRemove={handleRemove3}
                                        from={(
                                            <Grid
                                                application="SEG030Asignar"
                                                id="SEG030Asignar_grid_5" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                                enableSelection
                                                onAfterInitialize={onAfterInitialize}
                                                onBeforeInitialize={addParameters}
                                                onBeforeFetch={addParameters}
                                                onBeforeApplyFilter={addParameters}
                                                onBeforeApplySort={addParameters}
                                                onBeforeExportExcel={addParameters}
                                                onAfterMenuItemAction={onAfterMenuItemAction}
                                                onBeforeMenuItemAction={addParameters}
                                                onBeforeFetchStats={addParameters}
                                            />
                                        )}
                                        to={(
                                            <Grid
                                                application="SEG030Asignar"
                                                id="SEG030Asignar_grid_6" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                                enableSelection
                                                onAfterInitialize={onAfterInitialize}
                                                onBeforeInitialize={addParameters}
                                                onBeforeFetch={addParameters}
                                                onBeforeApplyFilter={addParameters}
                                                onBeforeApplySort={addParameters}
                                                onBeforeExportExcel={addParameters}
                                                onAfterMenuItemAction={onAfterMenuItemAction}
                                                onBeforeMenuItemAction={addParameters}
                                                onBeforeFetchStats={addParameters}
                                            />
                                        )}
                                    />
                                </div>
                            </Row>

                        </Tab>
                    </Tabs>
                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("SEG030_frm1_btn_cerrar")} </Button>
            </Modal.Footer>
        </Container>
    );
}

export const SEG030AsignarInfoModal = withPageContext(InternalSEG030AsignarInfoModal);