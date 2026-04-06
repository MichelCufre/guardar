import React, { useState, useRef, useEffect } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import { Page } from '../../components/Page';

export function REG700AsociarAplicacionModal(props) {

    const { t } = useTranslation("translation", { useSuspense: false });

    const handleBeforeLoad = (data) => {
        data.parameters = [...data.parameters, { id: "recorrido", value: props.numeroRecorrido }];
    }

    const applyParameters = (context, data, nexus) => {
        data.parameters = [{ id: "recorrido", value: props.numeroRecorrido },];
    };

    const handleAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("AgregarAplicacion_grid_1").refresh();
        nexus.getGrid("QuitarAplicacion_grid_2").refresh();
    }

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("AgregarAplicacion_grid_1").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("QuitarAplicacion_grid_2").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    };

    const handleClose = () => {
        props.onHide();
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("REG700AsociarAplicacion_Sec0_modalTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    {...props}
                    application="REG700AsociarAplicacion"
                    onBeforeLoad={handleBeforeLoad}
                >
                    <Row>
                        <Col>
                            <h4>{t("REG700_frm1_lbl_Recorrido")}: {props.numeroRecorrido}</h4>
                        </Col>
                        <Col>
                            <h4>{t("REG700_frm1_lbl_Nombre")}: {props.nombreRecorrido}</h4>
                        </Col>
                        <Col>
                            <h4>{t("REG700_frm1_lbl_Predio")}: {props.predio}</h4>
                        </Col>
                    </Row>
                    <hr />
                    <AddRemovePanel
                        onAdd={handleAdd}
                        onRemove={handleRemove}
                        BtnDisabled={(!(props.isRecorridoDefaul == "N"))}
                        from={(
                            <Grid
                                application="REG700AsociarAplicacion"
                                id="AgregarAplicacion_grid_1"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                onBeforeInitialize={applyParameters}
                                onBeforeFetch={applyParameters}
                                onBeforeFetchStats={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
                                onBeforeMenuItemAction={applyParameters}
                                onAfterMenuItemAction={handleAfterMenuItemAction}
                                onBeforeExportExcel={applyParameters}
                                enableExcelExport
                                enableSelection={(props.isRecorridoDefaul == "N")}
                            />
                        )}
                        to={(
                            <Grid
                                application="REG700AsociarAplicacion"
                                id="QuitarAplicacion_grid_2"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                onBeforeInitialize={applyParameters}
                                onBeforeFetch={applyParameters}
                                onBeforeFetchStats={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
                                onBeforeMenuItemAction={applyParameters}
                                onAfterMenuItemAction={handleAfterMenuItemAction}
                                onBeforeExportExcel={applyParameters}
                                enableExcelExport
                                enableSelection={(props.isRecorridoDefaul == "N")}
                            />
                        )}
                    />
                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button onClick={handleClose} id="btnCerrar" variant="outline-secondary">{t("REG700_frm1_btn_cerrar")}</Button>
            </Modal.Footer>
        </Modal>
    );
}