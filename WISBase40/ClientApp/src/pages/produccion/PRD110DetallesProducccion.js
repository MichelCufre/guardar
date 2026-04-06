import React, { useState } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldSelect, FieldSelectAsync, StatusMessage } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { AddRemovePanel } from '../../components/AddRemovePanel';

function InternalPRD110DetallesProducccion(props) {
    const { t } = useTranslation();

    const handleClose = () => {
        props.onHide();
    };

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "idIngreso", value: props.ingresoProduccion },
        ];
    }

    const onBeforeInitialize =  (context, data, nexus) => {
        nexus.parameters = [{ id: "idIngreso", value: props.ingresoProduccion }];
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRD110_Sec0_modalTitle_TituloDetallesProduccion")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form
                    id="PRD110DetallesProducccion_form"
                    application="PRD110DetallesProducccion"
                    onBeforeInitialize={onBeforeInitialize}
                >
                    <Row>
                    <Col md={3}>
                        <div className="form-group" >
                            <label htmlFor="idInternoProduccion">{t("PRD113_frm1_lbl_produccion")}</label>
                            <Field name="idInternoProduccion" readOnly />
                            <StatusMessage for="idInternoProduccion" />
                        </div>
                    </Col>
                    <Col md={3}>
                        <div className="form-group" >
                            <label htmlFor="idExternoProduccion">{t("PRD113_frm1_lbl_idExternoProduccion")}</label>
                            <Field name="idExternoProduccion" readOnly />
                            <StatusMessage for="idExternoProduccion" />
                        </div>
                    </Col>
                    <Col md={3}>
                        <div className="form-group" >
                            <label htmlFor="descripcionProduccion">{t("PRD113_frm1_lbl_descripcionProduccion")}</label>
                            <Field name="descripcionProduccion" readOnly />
                            <StatusMessage for="descripcionProduccion" />
                        </div>
                        </Col>
                    </Row>
                </Form>
                <Row>
                    <Col>
                        <Row>
                            <Col span={6} style={{ maxWidth: "50%" }}>
                                <h2>{t("PRD110_form1_title_Insumos")}</h2>
                                <Grid
                                    application="PRD110DetallesProducccion"
                                    id="PRD110DetallesProducccion_grid_1"
                                    rowsToFetch={16}
                                    rowsToDisplay={8}
                                    enableExcelExport
                                    onBeforeInitialize={applyParameters}
                                    onBeforeFetch={applyParameters}
                                    onBeforeFetchStats={applyParameters}
                                    onBeforeApplyFilter={applyParameters}
                                    onBeforeApplySort={applyParameters}
                                    onBeforeExportExcel={applyParameters}
                                />
                            </Col>
                            <Col span={6} style={{ maxWidth: "50%" }}>
                                <h2>{t("PRD110_form1_title_Salidas")}</h2>
                                <Grid
                                    application="PRD110DetallesProducccion"
                                    id="PRD110DetallesProducccion_grid_2"
                                    rowsToFetch={16}
                                    rowsToDisplay={8}
                                    enableExcelExport
                                    onBeforeInitialize={applyParameters}
                                    onBeforeFetch={applyParameters}
                                    onBeforeFetchStats={applyParameters}
                                    onBeforeApplyFilter={applyParameters}
                                    onBeforeApplySort={applyParameters}
                                    onBeforeExportExcel={applyParameters}
                                />
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Modal.Body>
        </Modal>
    );
}

export const PRD110DetallesProducccion = withPageContext(InternalPRD110DetallesProducccion);