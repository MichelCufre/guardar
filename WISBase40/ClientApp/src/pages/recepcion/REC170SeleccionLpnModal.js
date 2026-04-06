import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Modal, Button, Row, Col } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import { withPageContext } from '../../components/WithPageContext';

function InternalREC170SeleccionLpnModal(props) {

    const { t } = useTranslation();

    const applyParameters = (context, data, nexus) => {

        data.parameters = [
            {
                id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value
            }
        ];
    };

    const handleBeforeLoad = (data) => {
        data.parameters = [...data.parameters, { id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value }];
    }

    const handleAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("REC170SeleccionLpn_grid_1").refresh();
        nexus.getGrid("REC170SeleccionLpn_grid_2").refresh();
    }

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("REC170SeleccionLpn_grid_1").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("REC170SeleccionLpn_grid_2").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    };

    const handleClose = () => {
        props.onHide(null, null);
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("REC170SeleccionLpn_Sec0_lbl_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    {...props}
                    application="REC170SeleccionLpn"
                    onBeforeLoad={handleBeforeLoad}
                >
                    <Row>
                        <Col>
                        </Col>
                    </Row>
                    <AddRemovePanel
                        onAdd={handleAdd}
                        onRemove={handleRemove}
                        from={(
                            <Grid
                                application="REC170SeleccionLpn"
                                id="REC170SeleccionLpn_grid_1"
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
                                application="REC170SeleccionLpn"
                                id="REC170SeleccionLpn_grid_2"
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
                <Button onClick={handleClose} id="btnCerrar" variant="outline-secondary">{t("General_Sec0_btn_Cerrar")}</Button>
            </Modal.Footer>
        </Modal>
    );
}
export const REC170SeleccionLpnModal = withPageContext(InternalREC170SeleccionLpnModal);
