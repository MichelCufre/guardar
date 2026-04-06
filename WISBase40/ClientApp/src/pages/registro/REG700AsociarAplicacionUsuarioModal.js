import React, { useState, useRef, useEffect } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import { Page } from '../../components/Page';

export function REG700AsociarAplicacionUsuarioModal(props) {

    const { t } = useTranslation("translation", { useSuspense: false });

    const handleBeforeLoad = (data) => {
        data.parameters = [...data.parameters, { id: "recorrido", value: props.numeroRecorrido }];
    }

    const applyParameters = (context, data, nexus) => {
        data.parameters = [{ id: "recorrido", value: props.numeroRecorrido },
        { id: "predio", value: props.predio }];
    };

    const handleAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("AgregarAplicacionUsuario_grid_1").refresh();
        nexus.getGrid("QuitarAplicacionUsuario_grid_2").refresh();
    }

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("AgregarAplicacionUsuario_grid_1").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("QuitarAplicacionUsuario_grid_2").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    };

    const handleClose = () => {
        props.onHide();
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("REG700AsociarAplicacionUsuario_Sec0_modalTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    {...props}
                    application="REG700AsociarAplicacionUsuario"
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
                        from={(
                            <Grid
                                application="REG700AsociarAplicacionUsuario"
                                id="AgregarAplicacionUsuario_grid_1"
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
                                application="REG700AsociarAplicacionUsuario"
                                id="QuitarAplicacionUsuario_grid_2"
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
                <Button onClick={handleClose} id="btnCerrar" variant="outline-secondary">{t("REG700_frm1_btn_cerrar")}</Button>
            </Modal.Footer>
        </Modal>
    );
}