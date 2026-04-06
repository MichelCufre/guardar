import React, { useState, useRef, useEffect } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useCustomTranslation } from '../../components/TranslationHook';
import { withPageContext } from '../../components/WithPageContext';
import { Page } from '../../components/Page';
import { AddRemovePanel } from '../../components/AddRemovePanel';


function InternalPRE811TpPedidoModal(props) {

    const { t } = useCustomTranslation();

    const handleBeforeLoad = (data) => {
        data.parameters = [...data.parameters, { id: "keyPreferencia", value: props.preferencia }];
    }

    const applyParameters = (context, data, nexus) => {
        data.parameters = [{ id: "keyPreferencia", value: props.preferencia }];
    };

    const handleAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("AgregarTpPedido_grid_1").refresh();
        nexus.getGrid("QuitarTpPedido_grid_2").refresh();
    }

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("AgregarTpPedido_grid_1").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    }

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("QuitarTpPedido_grid_2").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    }

    const handleClose = () => {
        props.onHide();
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRE811TpPe_Sec0_modal_title")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    {...props}
                    onBeforeLoad={handleBeforeLoad}
                    application="PRE811PreferenciaTpPedido"
                >
                    <Row>
                        <Col>
                            <h4>{t("PRE811_frm1_lbl_Preferencia")}: {props.preferencia}</h4>
                        </Col>
                    </Row>
                    <hr />
                    <AddRemovePanel
                        onAdd={handleAdd}
                        onRemove={handleRemove}
                        from={(
                            <Grid
                                application="PRE811PreferenciaTpPedido"
                                id="AgregarTpPedido_grid_1"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                onBeforeInitialize={applyParameters}
                                onBeforeFetch={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
                                onBeforeMenuItemAction={applyParameters}
                                onAfterMenuItemAction={handleAfterMenuItemAction}
                                onBeforeExportExcel={applyParameters}
                                onBeforeFetchStats={applyParameters}
                                enableExcelExport
                                enableSelection
                            />
                        )}
                        to={(
                            <Grid
                                application="PRE811PreferenciaTpPedido"
                                id="QuitarTpPedido_grid_2"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                onBeforeInitialize={applyParameters}
                                onBeforeFetch={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
                                onBeforeMenuItemAction={applyParameters}
                                onAfterMenuItemAction={handleAfterMenuItemAction}
                                onBeforeExportExcel={applyParameters}
                                onBeforeFetchStats={applyParameters}
                                enableExcelExport
                                enableSelection
                            />
                        )}
                    />
                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                    {t("PRE811_frm1_btn_cerrar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const PRE811TpPedidoModal = withPageContext(InternalPRE811TpPedidoModal);