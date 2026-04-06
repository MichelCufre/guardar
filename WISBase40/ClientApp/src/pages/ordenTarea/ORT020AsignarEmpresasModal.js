import React, { useState } from 'react';
import { Button, Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';

export function ORT020AsignarEmpresasModal(props) {
    const { t } = useTranslation();

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("ORT020AsignarEmpresas_grid_1").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("ORT020AsignarEmpresas_grid_2").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    };

    const handleClose = () => {
        props.onHide(null, null, props.nexus);
    };

    const addParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "cdInsumoManipuleo", value: props.codigo }
        ];
    }

    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("ORT020AsignarEmpresas_grid_1").refresh();
        nexus.getGrid("ORT020AsignarEmpresas_grid_2").refresh();

    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("ORT020_Sec0_mdlTitle_AsigEmpresasTitulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    {...props}
                    application="ORT020AsignarEmpresas"
                >
                    <Row>
                        <Col>
                            <h4>{t("ORT020_frm1_lbl_insumoManipuleo")}: {`${props.codigo}`} - {`${props.descripcion}`}</h4>
                        </Col>
                    </Row>

                    <hr />

                    <Row className='mt-3'>
                        <div className="col-12">
                            <AddRemovePanel
                                onAdd={handleAdd}
                                onRemove={handleRemove}
                                from={(
                                    <Grid
                                        application="ORT020AsignarEmpresas"
                                        id="ORT020AsignarEmpresas_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                        enableSelection
                                        onAfterMenuItemAction={onAfterMenuItemAction}
                                        onBeforeInitialize={addParameters}
                                        onBeforeFetch={addParameters}
                                        onBeforeApplyFilter={addParameters}
                                        onBeforeApplySort={addParameters}
                                        onBeforeExportExcel={addParameters}
                                        onBeforeMenuItemAction={addParameters}
                                        onBeforeFetchStats={addParameters}
                                    />
                                )}
                                to={(
                                    <Grid
                                        application="ORT020AsignarEmpresas"
                                        id="ORT020AsignarEmpresas_grid_2" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                        enableSelection
                                        onAfterMenuItemAction={onAfterMenuItemAction}
                                        onBeforeInitialize={addParameters}
                                        onBeforeFetch={addParameters}
                                        onBeforeApplyFilter={addParameters}
                                        onBeforeApplySort={addParameters}
                                        onBeforeExportExcel={addParameters}
                                        onBeforeMenuItemAction={addParameters}
                                        onBeforeFetchStats={addParameters}
                                    />
                                )}
                            />
                        </div>
                    </Row>

                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("ORT020_frm1_btn_cerrar")} </Button>
            </Modal.Footer>
        </Modal>
    );
}
