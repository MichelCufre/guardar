import React, { useState } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { notificationType } from '../../components/Enums';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';

function InternalREG220RutasAgenteModal(props) {

    const { t } = useTranslation();

    const handleClose = () => {
        props.onHide();
    };

    const handleGridBeforeFetch = (context, data, nexus) => {

        data.parameters = [
            { id: "keyCodigo", value: props.agente.find(a => a.id === "CodigoInterno").value },
            { id: "keyEmpresa", value: props.agente.find(a => a.id === "IdEmpresa").value }
        ];

    }

    const handleGridBeforeValidate = (context, data, nexus) => {

        data.parameters = [
            { id: "keyCodigo", value: props.agente.find(a => a.id === "CodigoInterno").value },
            { id: "keyEmpresa", value: props.agente.find(a => a.id === "IdEmpresa").value }
        ];
    }

    const handleGridBeforeCommit = (context, data, nexus) => {

        data.parameters = [
            { id: "keyCodigo", value: props.agente.find(a => a.id === "CodigoInterno").value },
            { id: "keyEmpresa", value: props.agente.find(a => a.id === "IdEmpresa").value }
        ];

    }

    const handleGridAfterCommit = (context, rows, parameters, nexus) => {

        //if (context.status === "OK") {

        //    nexus.getGrid("REG220Rutas_grid_1").refresh();
        //}

    };
    const handleBeforeFetch = (context, data, nexus) => {

        data.parameters = [
            { id: "keyCodigo", value: props.agente.find(a => a.id === "CodigoInterno").value },
            { id: "keyEmpresa", value: props.agente.find(a => a.id === "IdEmpresa").value }
        ];

    };

    const handleBeforeApplyFilter = (context, data, nexus) => {

        data.parameters = [
            { id: "keyCodigo", value: props.agente.find(a => a.id === "CodigoInterno").value },
            { id: "keyEmpresa", value: props.agente.find(a => a.id === "IdEmpresa").value }
        ];

    };

    const handleBeforeExportExcel = (context, data, nexus) => {

        data.parameters = [
            { id: "keyCodigo", value: props.agente.find(a => a.id === "CodigoInterno").value },
            { id: "keyEmpresa", value: props.agente.find(a => a.id === "IdEmpresa").value }
        ];

    };

    const handleBeforeFetchStats = (context, data, nexus) => {

        data.parameters = [
            { id: "keyCodigo", value: props.agente.find(a => a.id === "CodigoInterno").value },
            { id: "keyEmpresa", value: props.agente.find(a => a.id === "IdEmpresa").value }
        ];

    };


    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">

            <Modal.Header closeButton>
                <Modal.Title>{t("REG220_SecRutas_pageTitle_Titulo")}</Modal.Title>
            </Modal.Header>

            <Modal.Body>
                <Container fluid>

                    <Row>
                        <Col>
                            <br />
                            <Grid
                                application="REG220Rutas"
                                id="REG220Rutas_grid_1"
                                rowsToFetch={15}
                                rowsToDisplay={8}
                                enableExcelExport={true}
                                enableExcelImport={false}
                                onBeforeInitialize={handleGridBeforeFetch}
                                onBeforeValidateRow={handleGridBeforeValidate}
                                onBeforeCommit={handleGridBeforeCommit}
                                onBeforeFetch={handleBeforeFetch}
                                onBeforeApplyFilter={handleBeforeApplyFilter}
                                onBeforeExportExcel={handleBeforeExportExcel}
                                onBeforeFetchStats={handleBeforeFetchStats}
                                onAfterCommit={handleGridAfterCommit}


                            />
                        </Col>
                    </Row>

                </Container>
            </Modal.Body>

            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                    {t("REG220_mdlRutas_btn_CERRAR")}
                </Button>
            </Modal.Footer>

        </Modal>
    );
}

export const REG220RutasAgenteModal = withPageContext(InternalREG220RutasAgenteModal);