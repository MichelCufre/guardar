import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { FormButton } from '../../components/FormComponents/Form';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import { Page } from '../../components/Page';

export function PRE052AsociarPedidosModal(props) {
    const { t } = useTranslation();

    const handleAfterMenuItemAction = (context, data, nexus) => {
        data.parameters = [
            { id: "preparacion", value: props.preparacion },
            { id: "empresa", value: props.empresa }
        ];
        nexus.getGrid("AgregarPedido_grid_1").refresh();
        nexus.getGrid("QuitarPedido_grid_2").refresh();
    };

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("AgregarPedido_grid_1").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("QuitarPedido_grid_2").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    };

    const handleClose = () => {
        props.onHide();
    };

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "preparacion", value: props.preparacion },
            { id: "empresa", value: props.empresa }
        ];
    };
    const handleBeforeLoad = (data) => {
        data.parameters = [...data.parameters, { id: "preparacion", value: props.preparacion }, { id: "empresa", value: props.empresa }];
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRE052_Sec0_mdl_AsociarPedido_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    application="PRE052AsociarPedidos"
                    onBeforeLoad={handleBeforeLoad}
                    {...props}
                >
                    <Row>
                        <Col>
                            {<h4>{t("PRE052_grid1_colname_NU_PREPARACION")}: {props.preparacion}</h4>}
                        </Col>
                    </Row>
                    <hr />
                    <AddRemovePanel
                        onAdd={handleAdd}
                        onRemove={handleRemove}
                        from={(
                            <Grid
                                application="PRE052AsociarPedidos"
                                id="AgregarPedido_grid_1"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                onBeforeInitialize={applyParameters}
                                onBeforeFetch={applyParameters}
                                onBeforeFetchStats={applyParameters}
                                onBeforeExportExcel={applyParameters}
                                onBeforeMenuItemAction={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
                                onAfterMenuItemAction={handleAfterMenuItemAction}
                                enableExcelExport
                                enableSelection
                            />
                        )}
                        to={(
                            <Grid
                                application="PRE052AsociarPedidos"
                                id="QuitarPedido_grid_2"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                onBeforeInitialize={applyParameters}
                                onBeforeFetch={applyParameters}
                                onBeforeMenuItemAction={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeApplySort={applyParameters}
                                onAfterMenuItemAction={handleAfterMenuItemAction}
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
