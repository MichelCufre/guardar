import React from 'react';
import { Col, Modal, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { withPageContext } from '../../components/WithPageContext';

function InternalPRD110DetallesTeoricos(props) {
    const { t } = useTranslation();

    const handleClose = () => {
        props.onHide();
    };

    const handleBeforeInitialize = (context, data, nexus) => {
        data.parameters = [
            { id: "idIngreso", value: props.ingresoEditar },
        ];
    }

    const handleBeforeCommit = (context, data, nexus) => {

        data.parameters = [
            { id: "idIngreso", value: props.ingresoEditar },
            { id: "empresa", value: props.empresa },
        ];
    };

    const applyParameters = (context, grid, query, nexus) => {
        query.parameters = [
            { id: "idIngreso", value: props.ingresoEditar },
            { id: "empresa", value: props.empresa },
        ];
    }

    const onBeforeValidateRow = (context, data, nexus) => {
        data.parameters.push({ id: "idIngreso", value: props.ingresoEditar });
        data.parameters.push({ id: "empresa", value: props.empresa });
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRD110_Sec0_modalTitle_TituloDetalles")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Row>
                    <Col>
                        <Row>
                            <Col span={6} style={{ maxWidth: "50%" }}>
                                <h2>{t("PRD110_form1_title_Entrada")}</h2>
                                <Grid
                                    application="PRD110DetallesTeoricos"
                                    id="PRD110Detalles_grid_1"
                                    rowsToFetch={16}
                                    rowsToDisplay={8}
                                    enableExcelExport
                                    onBeforeInitialize={handleBeforeInitialize}
                                    onBeforeCommit={handleBeforeCommit}
                                    onBeforeSelectSearch={applyParameters}
                                    onBeforeFetch={handleBeforeInitialize}
                                    onBeforeFetchStats={handleBeforeInitialize}
                                    onBeforeApplyFilter={handleBeforeInitialize}
                                    onBeforeApplySort={handleBeforeInitialize}
                                    onBeforeExportExcel={handleBeforeInitialize}
                                    onBeforeValidateRow={onBeforeValidateRow}
                                />
                            </Col>
                            <Col span={6} style={{ maxWidth: "50%" }}>
                                <h2>{t("PRD110_form1_title_Salida")}</h2>
                                <Grid
                                    application="PRD110DetallesTeoricos"
                                    id="PRD110Detalles_grid_2"
                                    rowsToFetch={16}
                                    rowsToDisplay={8}
                                    enableExcelExport
                                    onBeforeInitialize={handleBeforeInitialize}
                                    onBeforeCommit={handleBeforeCommit}
                                    onBeforeSelectSearch={applyParameters}
                                    onBeforeFetch={handleBeforeInitialize}
                                    onBeforeFetchStats={handleBeforeInitialize}
                                    onBeforeApplyFilter={handleBeforeInitialize}
                                    onBeforeApplySort={handleBeforeInitialize}
                                    onBeforeExportExcel={handleBeforeInitialize}
                                    onBeforeValidateRow={onBeforeValidateRow}
                                />
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Modal.Body>
        </Modal>
    );
}

export const PRD110DetallesTeoricos = withPageContext(InternalPRD110DetallesTeoricos);