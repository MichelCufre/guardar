import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useCustomTranslation } from '../../components/TranslationHook';
import { Form, SubmitButton, FormButton } from '../../components/FormComponents/Form';
import { withPageContext } from '../../components/WithPageContext';


function InternalREC170ValidarFacturaModal(props) {

    const { t } = useCustomTranslation();

    const [infoAgenda, setInfoAgenda] = useState(null);

    const applyParameters = (context, data, nexus) => {

        data.parameters = [
            {
                id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value
            }
        ];
    };

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        query.parameters = [
            { id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value },

        ];
    };

    const handleOnBeforeLoad = (data) => {
        setInfoAgenda(props.agenda.find(a => a.id === "idAgenda").value);
        data.parameters = [
            { id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value },

        ];
    };

    const onBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [{ id: "keyAgenda", value: props.agenda.find(a => a.id === "idAgenda").value }];
    }

    const handleClose = () => {
        props.onHide();
    }

    if (!props.show) {
        return null;
    }

    return (
        <Page
            {...props}
            onBeforeLoad={handleOnBeforeLoad}
        >
            <Form
                application="REC170ValidarFactura"
                id="REC170ValidarFactura_form_1"
                onBeforeSubmit={handleFormBeforeSubmit}
                onBeforeInitialize={onBeforeInitialize}

            >
                <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
                    <Modal.Header closeButton>
                        <Modal.Title>{t("REC170_Sec0_mdlValidar_Titulo")}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>

                        <Container fluid>

                            <Grid
                                application="REC170ValidarFactura"
                                id="REC170ValidarFactura_grid_1"
                                rowsToFetch={30}
                                rowsToDisplay={10}
                                onBeforeInitialize={applyParameters}
                                onBeforeButtonAction={applyParameters}
                                onBeforeApplySort={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeFetch={applyParameters}
                                onBeforeMenuItemAction={applyParameters}
                                onBeforeExportExcel={applyParameters}
                                enableExcelExport
                                autofocus={true}
                            />

                        </Container>

                    </Modal.Body>
                    <Modal.Footer>
                        <Button onClick={handleClose} id="btnCerrar" variant="outline-secondary">{t("REC170_frm1_btn_cerrar")}</Button>
                        <SubmitButton id="btnValidar" variant="primary" label="REC170_frm1_btn_validar" />
                    </Modal.Footer>
                </Modal>
            </Form>
        </Page>

    );
}


export const REC170ValidarFacturaModal = withPageContext(InternalREC170ValidarFacturaModal);