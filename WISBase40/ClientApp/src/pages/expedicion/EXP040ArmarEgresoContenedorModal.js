import React, { useState, useRef, useEffect } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import { Page } from '../../components/Page';

export function EXP040ArmarEgresoContenedorModal(props) {
    console.log("ArmarEgresoContenedor");

    const { t } = useTranslation("translation", { useSuspense: false });

    const handleBeforeLoad = (data) => {
        data.parameters = [...data.parameters, { id: "camion", value: props.camion }];
    }

    const applyParameters = (context, data, nexus) => {
        data.parameters = [ { id: "ruta", value: props.ruta },
                            { id: "camion", value: props.camion },
                            { id: "empresa", value: props.empresa }];
    };

    const handleAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("AgregarContenedor_grid_1").refresh();
        nexus.getGrid("QuitarContenedor_grid_2").refresh();
    }

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("AgregarContenedor_grid_1").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("QuitarContenedor_grid_2").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    };

    const handleClose = () => {
        props.onHide();
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("EXP040ArmarEgresoContenedor_Sec0_modalTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    {...props}
                    application="EXP040ArmarEgresoContenedor"
                    onBeforeLoad={handleBeforeLoad}
                >
                    <Row>
                        <Col>
                            <h4>{t("EXP040_frm1_lbl_Camion")}: {props.camion}</h4>
                        </Col>
                    </Row>
                    <hr />
                    <AddRemovePanel
                        onAdd={handleAdd}
                        onRemove={handleRemove}
                        from={(
                            <Grid
                                application="EXP040ArmarEgresoContenedor"
                                id="AgregarContenedor_grid_1"
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
                                enableSelection
                            />
                        )}
                        to={(
                            <Grid
                                application="EXP040ArmarEgresoContenedor"
                                id="QuitarContenedor_grid_2"
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
                                enableSelection
                            />
                        )}
                    />
                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button onClick={handleClose} id="btnCerrar" variant="outline-secondary">{t("EXP040_frm1_btn_cerrar")}</Button>
            </Modal.Footer>
        </Modal>
    );
}