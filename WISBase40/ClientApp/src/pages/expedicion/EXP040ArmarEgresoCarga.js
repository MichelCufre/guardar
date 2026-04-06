import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { FormButton } from '../../components/FormComponents/Form';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import { Page } from '../../components/Page';

export function EXP040ArmarEgresoCargaModal(props) {   
    const { t } = useTranslation("translation", { useSuspense: false });

    const applyParameters = (context, data, nexus) => {
        data.parameters = [ { id: "ruta", value: props.ruta },
                            { id: "camion", value: props.camion },                            
                            { id: "empresa", value: props.empresa }];
    };

    const handleBeforeLoad = (data) => {
        data.parameters = [...data.parameters, { id: "camion", value: props.camion }];
    }

    const handleAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("AgregarCarga_grid_1").refresh();
        nexus.getGrid("QuitarCarga_grid_2").refresh();
    }

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("AgregarCarga_grid_1").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("QuitarCarga_grid_2").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    };

    const handleClose = () => {
        props.onHide();
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("EXP040ArmarEgresoCarga_Sec0_modalTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    {...props}
                    application="EXP040ArmarEgresoCarga"
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
                                application="EXP040ArmarEgresoCarga"
                                id="AgregarCarga_grid_1"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                onBeforeInitialize={applyParameters}
                                onBeforeFetch={applyParameters}
                                onBeforeFetchStats={applyParameters}
                                onBeforeMenuItemAction={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
                                onAfterMenuItemAction={handleAfterMenuItemAction}
                                onBeforeExportExcel={applyParameters}
                                enableExcelExport
                                enableSelection
                            />
                        )}
                        to={(
                            <Grid
                                application="EXP040ArmarEgresoCarga"
                                id="QuitarCarga_grid_2"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                onBeforeInitialize={applyParameters}
                                onBeforeFetch={applyParameters}
                                onBeforeFetchStats={applyParameters}
                                onBeforeMenuItemAction={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
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