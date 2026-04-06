import React, { useState } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Form, Field, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { notificationType } from '../../components/Enums';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';

function InternalPRD190IdentificadoresEgreso(props) {
    const { t } = useTranslation();

    const handleClose = () => {
        props.onHide();
    };

    const addParameters = (context, data, nexus) => {
        data.parameters = [{ id: "ubicacion", value: props.ubicacion }]
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRD190_modalIdentificador_title_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Row>
                    <Col>
                        <Grid
                            application="PRD191"
                            id="PRD191_grid_1"
                            rowsToFetch={30}
                            rowsToDisplay={15}
                            onBeforeInitialize={addParameters}
                            onBeforeFetch={addParameters}
                            onBeforeValidateRow={addParameters}
                            onBeforeSelectSearch={addParameters}
                            onBeforeCommit={addParameters}
                        />
                    </Col>
                </Row>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_Cerrar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const PRD190IdentificadoresEgreso = withPageContext(InternalPRD190IdentificadoresEgreso);