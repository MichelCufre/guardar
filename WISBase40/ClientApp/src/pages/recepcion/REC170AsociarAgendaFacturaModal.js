import React, { useState } from 'react';
import { Button, Col, Container, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import { Form } from '../../components/FormComponents/Form';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { withPageContext } from '../../components/WithPageContext';

function InternalREC170AsociarAgendaFacturaModal(props) {
    console.log("AsociarAgendaFactura");

    const { t } = useTranslation("translation", { useSuspense: false });
    const [infoAgenda, setInfoAgenda] = useState(null);

    const applyParameters = (context, data, nexus) => {

        if (props.agenda) {

            data.parameters = [
                { id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value },

            ];
        }
    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {

        if (props.agenda) {
            if (props.agenda.find(a => a.id === "idAgenda").value != null) {
                setInfoAgenda(props.agenda.find(a => a.id === "idAgenda").value);
            }
        }
    };

    const handleAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("AgregarFactura_grid_1").refresh();
        nexus.getGrid("QuitarFactura_grid_2").refresh();
    };

    const handleAdd = (evt, nexus, data) => {
        nexus.getGrid("AgregarFactura_grid_1").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus, data) => {
        nexus.getGrid("QuitarFactura_grid_2").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    };

    const handleClose = () => {
        props.onHide();
    }

    return (
        <Page

            //title={t("REC170_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                application="REC170AsociarAgendaFactura"
                id="AgregarFactura_grid"
                onBeforeInitialize={applyParameters}
                onBeforeFetch={applyParameters}
                onBeforeFetchStats={applyParameters}
                onBeforeApplyFilter={applyParameters}
                onBeforeApplySort={applyParameters}
                onBeforeMenuItemAction={applyParameters}
                onAfterMenuItemAction={handleAfterMenuItemAction}
                onBeforeExportExcel={applyParameters}
                onBeforeInitialize={handleFormBeforeInitialize}
            >
                <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
                    <Modal.Header closeButton>
                        <Modal.Title>{t("REC170AsociarAgendaFactura_Sec0_modalTitle_Titulo")}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Container fluid>
                            <Row>
                                <Col>
                                    <h4>{t("REC170_frm1_lbl_CAgenda")}: {`${infoAgenda}`}</h4>
                                </Col>
                            </Row>
                            <hr />
                            <AddRemovePanel
                                onAdd={handleAdd}
                                onRemove={handleRemove}
                                from={(
                                    <Grid
                                        application="REC170AsociarAgendaFactura"
                                        id="AgregarFactura_grid_1"
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
                                        application="REC170AsociarAgendaFactura"
                                        id="QuitarFactura_grid_2"
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
                        </Container>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                            {t("REC400_frm1_btn_cerrar")}
                        </Button>
                    </Modal.Footer>
                </Modal>
            </Form>
        </Page>
    );
}

export const REC170AsociarAgendaFacturaModal = withPageContext(InternalREC170AsociarAgendaFacturaModal);
