import React, { useState } from 'react';
import { Modal, Col, Row, Button, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, StatusMessage} from '../../components/FormComponents/Form';

import { useTranslation } from 'react-i18next';

export default function PRD111ConsumirStock(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const initialValues = {};
    const validationSchema = {};

    const onBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = [
            { id: "codigoEspacio", value: props.codigoEspacio}
        ];
    }
    
    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "codigoEspacio", value: props.codigoEspacio}
        ];
    };
    
    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("PRD111ConsumirStock_grid_1").refresh();
    }

    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("PRD111ConsumirStock_grid_1").refresh();
    }

    const handleClose = () => {
        props.onHide();
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-70w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRD111ConsumirStock_Sec0_Title_ConsumirStock")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    application="PRD111ConsumirStock"
                    {...props}
                >
                    <Form
                        application="PRD111ConsumirStock"
                        id="PRD111ConsumirStock_form_1"
                        initialValues={initialValues}
                        validationSchema={validationSchema}
                        onBeforeInitialize={onBeforeInitialize}
                    >
                        <Row>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="codigo">{t("PRD111ConsumirStock_form_lbl_codigo")}</label>
                                    <Field name="codigo" readOnly />
                                    <StatusMessage for="codigo" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="descripcion">{t("PRD111ConsumirStock_form_lbl_descripcion")}</label>
                                    <Field name="descripcion" readOnly />
                                    <StatusMessage for="descripcion" />
                                </div>
                            </Col>
                            <Col>
                                <div className="form-group" >
                                    <label htmlFor="tipo">{t("PRD111ConsumirStock_form_lbl_tipo")}</label>
                                    <Field name="tipo" readOnly />
                                    <StatusMessage for="tipo" />
                                </div>
                            </Col>
                        </Row>                      
                    </Form>
                    
                    <hr />

                    <Grid
                        id="PRD111ConsumirStock_grid_1"
                        application="PRD111ConsumirStock"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        enableSelection
                        onBeforeInitialize={applyParameters}
                        onBeforeSelectSearch={applyParameters}
                        onBeforeValidateRow={applyParameters}
                        onBeforeCommit={applyParameters}
                        onAfterCommit={onAfterCommit}
                        onBeforeExportExcel={applyParameters}
                        onBeforeFetch={applyParameters}
                        onBeforeFetchStats={applyParameters}
                        onBeforeApplySort={applyParameters}
                        onBeforeApplyFilter={applyParameters}
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        onBeforeMenuItemAction={applyParameters}
                        
                    />
                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="outline-secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_Cerrar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}